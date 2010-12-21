using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NCRVisual.RelationDiagram.Algo;
using PersonalInfoAndStatistics;
using RelationDiagram;

namespace NCRVisual.RelationDiagram
{
    public partial class RelationDiagramControl : UserControl
    {

        #region private
        private int count = 0;

        private bool _searchBorderStatus = false;
        private bool _topTenBorderStatus = false;

        //Private variable for render
        private DiagramController _myDiagramController;
        private Collection<EntityControl> _entityControlCollection;
        private ScaleTransform _scale;

        //Private variable for handling background and mouse
        private StarField _starField;
        private DateTime _lastUpdate = DateTime.Now;
        private ColorfulFireworks _colourfulFirework;
        private bool _mouseDown = false;

        //Private variable for handling transformation of layoutroot
        private Transform FirstTransform;
        private double firstX;
        private double firstY;

        private Point _mouseDownPoint = new Point();
        private Point _lastMouseDownPoint = new Point();

        private CustomChildWindow _algorirthmChildWindow;

        #endregion

        #region Properties
        /// <summary>
        /// Dictionary for maping entity and entityControl
        /// </summary>
        public Dictionary<IEntity, EntityControl> EntityRegistry { get; set; }

        /// <summary>
        /// Collection of pointpositions
        /// </summary>
        public Collection<Point> PointPositions { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RelationDiagramControl(string _inputFileName)
        {
            InitializeComponent();

            _entityControlCollection = new Collection<EntityControl>();
            EntityRegistry = new Dictionary<IEntity, EntityControl>();
            _scale = new ScaleTransform();
            _myDiagramController = new DiagramController(_inputFileName);

            //Event handler
            _myDiagramController.InputReadingComplete += new EventHandler(_myDiagramController_InputReadingComplete);

            MainGrid.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(MainGrid_MouseLeftButtonDown);
            MainGrid.MouseMove += new System.Windows.Input.MouseEventHandler(CircleField_MouseMove);
            MainGrid.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MainGrid_MouseLeftButtonUp);

            AlgoButton.MouseClick += new EventHandler(AlgoButton_MouseClick);

            // add the controls to the back          
            _colourfulFirework = new ColorfulFireworks();
            MainGrid.Children.Add(_colourfulFirework);
            _colourfulFirework.Start();

            //Populate personalStatistic Control
            PopulatePersonalStatistics();
        }

        #endregion

        private void PopulatePersonalStatistics()
        {
            PersonalStatisticsControl psc = new PersonalStatisticsControl();
            this.DetailBorder.Child = psc;
        }

        private static bool _initializedAfterScreenSizeChanged = false;

        private void InitIfNeededAfterScreenSizeIsKnown()
        {
            if (_initializedAfterScreenSizeChanged) return;
            _initializedAfterScreenSizeChanged = true;

            _starField = new StarField(panelStarfield);
            _lastUpdate = DateTime.Now;
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            double msec = (now - _lastUpdate).TotalMilliseconds;
            if (msec == 0)
            {
                return;
            }

            _starField.UpdateStars(msec);

            _lastUpdate = DateTime.Now;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Globals.ScreenWidth = MainGrid.ActualWidth;
            Globals.ScreenHeight = MainGrid.ActualHeight;

            InitIfNeededAfterScreenSizeIsKnown();
        }

        private void DisableForLoading()
        {
            this.progressBar.Visibility = Visibility.Visible;
            this.AlgoButton.IsEnabled = false;
            this.Home.IsHitTestVisible = false;
            this.ZoomIn.IsHitTestVisible = false;
            this.ZoomOut.IsHitTestVisible = false;
            this.Search.IsHitTestVisible = false;
            this.DetailBorderMoveOut.Begin();
            this.SearchBorderMoveOut.Begin();
        }

        private void EnableAfterLoaded()
        {
            this.progressBar.Visibility = Visibility.Collapsed;
            this.AlgoButton.IsEnabled = true;
            this.Home.IsHitTestVisible = true;
            this.ZoomIn.IsHitTestVisible = true;
            this.ZoomOut.IsHitTestVisible = true;
            this.Search.IsHitTestVisible = true;
        }

        private void PopulateSearchingNodeList()
        {
            //this.NodeList
            foreach (MailListEntity entity in _myDiagramController.entityCollection)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = entity.Email;
                this.NodeList.Items.Add(item);
            }
        }

        #region EventHandler

        void MainGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!_mouseDown)
            {
                // get some initial properties
                _mouseDownPoint = e.GetPosition(this);
                _lastMouseDownPoint = e.GetPosition(this);
                _mouseDown = true;
                MainGrid.Cursor = Cursors.Hand;
            }
        }

        void CircleField_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this._colourfulFirework.GoFire(e.GetPosition(this).X, e.GetPosition(this).Y);

            if (_mouseDown)
            {
                // update the element position
                Point point = e.GetPosition(this);
                Point diff = new Point((point.X - _mouseDownPoint.X) * 0.5, (point.Y - _mouseDownPoint.Y) * 0.5);
                Canvas.SetTop(LayoutRoot, Canvas.GetTop(LayoutRoot) + diff.Y);
                Canvas.SetLeft(LayoutRoot, Canvas.GetLeft(LayoutRoot) + diff.X);

                _lastMouseDownPoint = e.GetPosition(this);
            }
        }

        void MainGrid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                MainGrid.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Event handler after reading input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _myDiagramController_InputReadingComplete(object sender, EventArgs e)
        {
            this.AlgoButton.IsEnabled = true;
            this.TopTenBlock.IsHitTestVisible = true;
            PopulateSearchingNodeList();
            PopulateTopTen();

            _algorirthmChildWindow = new CustomChildWindow();
            _algorirthmChildWindow.SelectAlgorithmCompleted += new EventHandler(_algorirthmChildWindow_SelectAlgorithmCompleted);
            _algorirthmChildWindow.InitAlgo();
        }

        private void PopulateTopTen()
        {
            ObservableCollection<TopTen.UserInfo> col = new ObservableCollection<TopTen.UserInfo>();

            foreach (MailListEntity ent in _myDiagramController.entityCollection)
            {
                col.Add(new TopTen.UserInfo(ent.Email,
                    ent.NumMessagesSent,
                    ent.NumMessagesRecv,
                    ent.NumMessagesRepl));
            }

            this.Top10Control.UserList = col;
        }

        private void Centerize()
        {
            try
            {
                double middleX = (_myDiagramController.LowRight.X - _myDiagramController.TopLeft.X) / 2.0;
                double middleY = (_myDiagramController.LowRight.Y - _myDiagramController.TopLeft.Y) / 2.0;

                double diffX = middleX - (App.Current.Host.Content.ActualWidth) / 2.0;
                double diffY = middleX - (App.Current.Host.Content.ActualHeight) / 2.0;

                Canvas.SetTop(LayoutRoot, -diffY);
                Canvas.SetLeft(LayoutRoot, -diffX);

                double scaleX = (double)(panelStarfield.ActualWidth) / (double)(_myDiagramController.LowRight.X - _myDiagramController.TopLeft.X);
                double scaleY = (double)(panelStarfield.ActualHeight) / (double)(_myDiagramController.LowRight.Y - _myDiagramController.TopLeft.Y);
                ScaleTransform scale = new ScaleTransform();
                scale.CenterX = middleX;
                scale.CenterY = middleY;
                scale.ScaleX = Math.Min(scaleX, scaleY) - 0.1;
                scale.ScaleY = scale.ScaleX;

                this.LayoutRoot.RenderTransform = scale;
            }
            catch
            {
                this.LayoutRoot.RenderTransform = new ScaleTransform();
            }
        }

        private void Home_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.LayoutRoot.RenderTransform = FirstTransform;
            Canvas.SetLeft(LayoutRoot, firstX);
            Canvas.SetTop(LayoutRoot, firstY);
        }

        private void ZoomIn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ScaleTransform scale = (ScaleTransform)this.LayoutRoot.RenderTransform;
            scale.ScaleX += 0.1;
            scale.ScaleY += 0.1;
        }

        private void ZoomOut_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ScaleTransform scale = (ScaleTransform)this.LayoutRoot.RenderTransform;
            scale.ScaleX -= 0.1;
            scale.ScaleY -= 0.1;
        }

        void _algorirthmChildWindow_SelectAlgorithmCompleted(object sender, EventArgs e)
        {
            this.LayoutRoot.Children.Clear();
            this.EntityRegistry.Clear();
            this._entityControlCollection.Clear();
            count = 0;
            this.LayoutRoot.RenderTransform = new MatrixTransform();
            Canvas.SetLeft(LayoutRoot, 0);
            Canvas.SetTop(LayoutRoot, 0);

            IAlgorithm algo = new KKAlgorithm();

            switch (sender.ToString())
            {
                case "Circular":
                    DrawCircular();
                    break;
                case "Kamada - Kawai":
                    algo = new KKAlgorithm();
                    DrawGraphLayout(algo);
                    break;
                case "Fruchterman":
                    algo = new FruchtermanAlgorithm();
                    DrawGraphLayout(algo);
                    break;
                case "TreeNode":
                    algo = new TreeNodeAlgorithm();
                    DrawGraphLayout(algo);
                    break;
                case "Rectangle":
                    algo = new RectangleAlgorithm();
                    DrawGraphLayout(algo);
                    break;
            }

        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PointPositions = (Collection<Point>)e.Result;

            //Generate node
            for (int i = 0; i < PointPositions.Count; i++)
            {
                CreateNode(_myDiagramController.entityCollection[i], PointPositions[i].X, PointPositions[i].Y);
            }

            //Generate Connection
            for (int i = 0; i < _entityControlCollection.Count; i++)
            {
                int t = 0;
                foreach (IConnection con in _entityControlCollection[i].Entity.Connections)
                {
                    if (con.Destination != null)
                    {
                        if (!con.Source.Equals(con.Destination))
                        {

                            drawConnection(_entityControlCollection[i], EntityRegistry[con.Destination], con.Value);
                            t = _entityControlCollection.IndexOf(EntityRegistry[con.Destination]);
                        }
                    }
                }
            }

            Centerize();

            //Deep copy for save the first transform of the diagram
            FirstTransform = new ScaleTransform
            {
                CenterX = double.Parse(((ScaleTransform)LayoutRoot.RenderTransform).CenterX.ToString()),
                CenterY = double.Parse(((ScaleTransform)LayoutRoot.RenderTransform).CenterY.ToString()),
                ScaleX = double.Parse(((ScaleTransform)LayoutRoot.RenderTransform).ScaleX.ToString()),
                ScaleY = double.Parse(((ScaleTransform)LayoutRoot.RenderTransform).ScaleY.ToString()),
            };

            firstX = Canvas.GetLeft(LayoutRoot);
            firstY = Canvas.GetTop(LayoutRoot);

            EnableAfterLoaded();
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = this._myDiagramController.RunAlgo((IAlgorithm)e.Argument);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DetailBorderMoveOut.Begin();
        }

        void AlgoButton_MouseClick(object sender, EventArgs e)
        {
            _algorirthmChildWindow.Show();
        }

        private void Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_searchBorderStatus)
            {
                this.SearchBorderMoveIn.Begin();
            }
            else
            {
                this.SearchBorderMoveOut.Begin();
            }
            _searchBorderStatus = !_searchBorderStatus;
        }

        private void NodeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SearchTextBox.Text = ((ListBoxItem)NodeList.SelectedItem).Content.ToString();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                EntityControl result = SearchEntity(SearchTextBox.Text);
                if (result != null)
                {
                    result.HighLightAllConnected();
                    DisplayDetail(result.Entity);
                }
            }
        }

        private EntityControl SearchEntity(string name)
        {
            EntityControl returned = new EntityControl(null, null);
            bool result = false;

            foreach (EntityControl ec in _entityControlCollection)
            {
                if (ec.Entity.Name == SearchTextBox.Text)
                {
                    result = true;
                    returned = ec;
                }
                else
                {
                    ec.UnHightListAllConnected();
                }
            }

            if (result)
            {
                return returned;
            }
            else
            {
                return null;
            }

        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_topTenBorderStatus)
            {
                this.TopTenBorderMoveIn.Begin();
            }
            else
            {
                this.TopTenBorderMoveOut.Begin();
            }
            _topTenBorderStatus = !_topTenBorderStatus;
        }
        #endregion

        #region Render

        private void DrawCircular()
        {
            double MiddleX = panelStarfield.ActualWidth / 2.0;
            double MiddleY = panelStarfield.ActualHeight / 2.0;

            for (int i = 0; i < _myDiagramController.VertexNumber; i++)
            {
                double celcius = 360.0 / (_myDiagramController.VertexNumber + 1);

                Block block = new Block();
                LayoutRoot.Children.Add(block);
                Canvas.SetLeft(block, MiddleX);
                Canvas.SetTop(block, MiddleY);

                block.TextField.Text = _myDiagramController.entityCollection[i].Email;
                block.Foreground = new SolidColorBrush(Colors.White);

                RotateTransform Rotate = new RotateTransform();
                Rotate.Angle = i * celcius;

                TranslateTransform translate = new TranslateTransform();
                translate.X = 150 * Math.Cos(-i * celcius * Math.PI / 180);
                translate.Y = -150 * Math.Sin(-i * celcius * Math.PI / 180);

                TransformGroup grouptransform = new TransformGroup();
                grouptransform.Children.Add(Rotate);
                grouptransform.Children.Add(translate);

                block.RenderTransform = grouptransform;
            }

            LayoutRoot.RenderTransform = new ScaleTransform();

            FirstTransform = LayoutRoot.RenderTransform;
            firstX = Canvas.GetLeft(LayoutRoot);
            firstY = Canvas.GetTop(LayoutRoot);

            //Render.IsEnabled = true;
        }

        private void DrawGraphLayout(IAlgorithm algo)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.WorkerReportsProgress = true;
            bg.WorkerSupportsCancellation = true;
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
            DisableForLoading();
            bg.RunWorkerAsync(algo);
        }

        /// <summary>
        /// Create a node of the graph ( entity Control )
        /// </summary>
        /// <param name="Display">The title of the node</param>
        /// <param name="left">The distance between the top-left of the node to the left of the browser</param>
        /// <param name="top">The distance between the top-left of the node to the top of the browser</param>
        public EntityControl CreateNode(IEntity entity, double left, double top)
        {
            //Create the node control            
            EntityControl ctrl = new EntityControl(entity, this.EntityRegistry);
            ctrl.MouseDoubleClick += new EventHandler(ctrl_MouseDoubleClick);
            ctrl.Title = count.ToString();
            count++;
            //ctrl.Title = "1";
            ctrl.Width = ctrl.DesiredWidth;
            ctrl.Height = ctrl.DesiredHeight;

            LayoutRoot.Children.Add(ctrl);
            _entityControlCollection.Add(ctrl);

            EntityRegistry.Add(entity, ctrl);

            //Set the Anxis of the node
            Canvas.SetLeft(ctrl, left);
            Canvas.SetTop(ctrl, top);

            return ctrl;
        }

        void ctrl_MouseDoubleClick(object sender, EventArgs e)
        {
            DisplayDetail(sender);
        }

        private void DisplayDetail(object sender)
        {
            MailListEntity me = (MailListEntity)sender;
            PersonalStatisticsControl statisticsControl = (PersonalStatisticsControl)this.DetailBorder.Child;
            statisticsControl.EmailAddr = me.Email;
            statisticsControl.MemberName = me.Email.Remove(me.Email.IndexOf('@'));
            statisticsControl.MessagesSent = 0;

            List<DateTime> sentTime = new List<DateTime>();
            List<DateTime> receivedTime = new List<DateTime>();

            foreach (Connection con in me.Connections)
            {
                if (con.Destination == null)
                {
                    foreach (DateTime time in con.SendDate)
                    {
                        sentTime.Add(time);
                    }
                }
                else
                {
                    foreach (DateTime time in con.SendDate)
                    {
                        sentTime.Add(time);
                    }

                }
            }
            statisticsControl.MessagesSent = sentTime.Count;
            statisticsControl.AllMessagesSentTime = sentTime;
            this.DetailBorderMoveIn.Begin();
        }

        private Random seed = new Random();

        /// <summary>
        /// Draw connection between 2 entity control
        /// </summary>
        /// <param name="startingPoint">Source entity</param>
        /// <param name="endPoint">Final entity</param>
        public void drawConnection(EntityControl startingPoint, EntityControl endPoint, int value)
        {
            double lambda = 255 / (_myDiagramController.MaxSingleConnection - _myDiagramController.MinSingleConnection);

            EdgeControl edge = new EdgeControl(null, null, Colors.White);
            //edge = new EdgeControl(startingPoint, endPoint, Color.FromArgb(255, byte.Parse(Math.Round((0 + lambda) * value).ToString()), 255, byte.Parse(Math.Round((0 + lambda) * value).ToString())));

            edge = new EdgeControl(startingPoint, endPoint, Color.FromArgb(255, byte.Parse(seed.Next(0, 255).ToString()), byte.Parse(seed.Next(0, 255).ToString()), byte.Parse(seed.Next(0, 255).ToString())));

            startingPoint.ConnectedEdges.Add(edge);
            endPoint.ConnectedEdges.Add(edge);
            edge.drawEdge();
            LayoutRoot.Children.Insert(LayoutRoot.Children.IndexOf(_entityControlCollection[0]), edge);
        }

        #endregion render
    }
}
