using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace NCRVisual.RelationDiagram
{
    public partial class EntityControl : UserControl
    {
        #region private
        private Color _startColor;
        private Color _highlightColor;
        private IEntity _entity;
        private Point _position;

        private DateTime _lastClick = DateTime.Now;
        private bool _firstClickDone = false;

        #endregion

        /// <summary>
        /// Get the desired width of the control after render
        /// </summary>
        public double DesiredWidth
        {
            get
            {
                return this.titleBlock.ActualWidth + (this.rectangle.Padding.Left + this.rectangle.BorderThickness.Left) * 2 + 1;
            }
        }

        /// <summary>
        /// Get the desired Height of the control after render
        /// </summary>
        public double DesiredHeight
        {
            get
            {
                return this.titleBlock.ActualHeight + (this.rectangle.Padding.Top + this.rectangle.BorderThickness.Top) * 2 + 1;
            }
        }

        /// <summary>
        /// Collection of connected edges to this entity control
        /// </summary>
        public List<EdgeControl> ConnectedEdges { get; set; }

        public event EventHandler MouseDoubleClick;

        private Dictionary<IEntity, EntityControl> _entityRegistry;

        public EntityControl(IEntity entity, Dictionary<IEntity, EntityControl> reg)
        {
            InitializeComponent();
            this._entity = entity;
            this._highlightColor = Colors.Yellow;
            this._startColor = this.endStop.Color;
            this.MouseLeftButtonDown += new MouseButtonEventHandler(EntityControl_MouseLeftButtonDown);
            this._entityRegistry = reg;
            ConnectedEdges = new List<EdgeControl>();
        }

        void EntityControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.OnClicked();
        }

        public IEntity Entity
        {
            get { return this._entity; }
            set { this._entity = value; }
        }

        public string Title
        {
            get { return this.titleBlock.Text; }
            set { this.titleBlock.Text = value; }
        }

        public double RectangleOpacity
        {
            get { return this.rectangle.Opacity; }
            set { this.rectangle.Opacity = value; }
        }

        public Color Color
        {
            get { return this.endStop.Color; }
            set
            {
                this.endStop.Color = value;
                this._startColor = this.endStop.Color;
            }
        }

        public Point Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        public void Highlight()
        {
            this.highlightStoryboard.Begin();
        }

        public void HighlightSecondary()
        {
            this.highlightSecondaryStoryboard.Begin();
        }

        public void UnHighlight()
        {
            this.unHighlightAnimation.To = this._startColor;
            this.unHighlightStoryboard.Begin();
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            HighLightAllConnected();
        }

        public void HighLightAllConnected()
        {           
            foreach (IConnection con in this.Entity.Connections)
            {
                if (con.Destination != null)
                {
                    EntityControl ec = (EntityControl)_entityRegistry[con.Destination];
                    ec.HighlightSecondary();
                }
            }

            foreach (EdgeControl edc in this.ConnectedEdges)
            {
                edc.HighLight();
            }
            this.Highlight();
        }

        public void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            UnHightListAllConnected();
        }

        public void UnHightListAllConnected()
        {
            this.UnHighlight();
            foreach (IConnection con in this.Entity.Connections)
            {
                if (con.Destination != null)
                {
                    EntityControl ec = (EntityControl)_entityRegistry[con.Destination];
                    ec.UnHighlight();
                }
            }

            foreach (EdgeControl edc in this.ConnectedEdges)
            {
                edc.UnHighLight();
            }
        }

        public event EventHandler<EntityControlClickedEventArgs> Clicked;

        protected void OnClicked()
        {
            if (this.Clicked != null)
            {
                EntityControlClickedEventArgs args = new EntityControlClickedEventArgs(this.Entity);
                this.Clicked(this, args);
            }
        }

        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            DateTime clickTime = DateTime.Now;
            TimeSpan span = clickTime - _lastClick;

            Point _clickPosition = e.GetPosition(element);

            if (span.TotalMilliseconds > 300 || _firstClickDone == false)
            {

                _firstClickDone = true;
                _lastClick = DateTime.Now;
            }
            else
            {
                Point position = e.GetPosition(element);
                if (Math.Abs(_clickPosition.X - position.X) < 4 &&
                    Math.Abs(_clickPosition.Y - position.Y) < 4)
                {
                    MouseDoubleClick(this.Entity, null);
                }

                _firstClickDone = false;
            }
        }
    }
}
