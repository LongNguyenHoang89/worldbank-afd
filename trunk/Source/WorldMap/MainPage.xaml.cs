using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using BingMapsSL.MultiShape;
using Microsoft.Expression.Controls;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Design;
using NCRVisual.web.DataModel;
using WorldMap.Helper;
using System.Linq;

namespace WorldMap
{
    [ScriptableType()]
    public partial class MainPage : UserControl
    {
        #region private variables
        private LocationConverter locConverter = new LocationConverter();
        private int _currentLocationCountry;

        private bool _isAddingNewPushPin;

        private int _currentImportCountryPK = 0;
        private int _currentExportCountryPK = 0;

        private bool _isSearchingCountry = false;
        # endregion

        #region properties

        /// <summary>
        /// public controller
        /// </summary>
        public Controller WorldMapController { get; set; }

        /// <summary>
        /// Map Layer for custom pushpin
        /// </summary>
        public MapLayer PushPinLayer { get; set; }

        public tbl_users user = new tbl_users();//{ get; set; }
        #endregion

        /// <summary>
        /// default constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            //Event Handler
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            this.MyWorkSpace.FullScreenButton.Click += new RoutedEventHandler(FullScreenButton_Click);
        }

        void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            App.Current.Host.Content.IsFullScreen = !App.Current.Host.Content.IsFullScreen;

            bool fullScreen = App.Current.Host.Content.IsFullScreen;

            if (fullScreen)
                btn.Content = "ON";
            else
                btn.Content = "OFF";
        }

        /// <summary>
        /// Run after all page is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Populate Helper for this control
            this.WorldMapController = new Controller();
            this.WorldMapController.LoadInitDataCompleted += new EventHandler(WorldMapController_LoadInitDataCompleted);
            this.WorldMapController.GetView_TabIndicatorQueryCompleted += new EventHandler(WorldMapController_GetView_TabIndicatorQueryCompleted);
            this.WorldMapController.GetBorder_completed += new EventHandler(WorldMapController_GetBorder_completed);
            this.WorldMapController.SaveCountryListCompleted += new EventHandler(WorldMapController_SaveCountryListCompleted);
            this.WorldMapController.SaveIndicatorListCompleted += new EventHandler(WorldMapController_SaveIndicatorListCompleted);
            this.WorldMapController.SaveGraphCompleted += new EventHandler(WorldMapController_SaveGraphCompleted);

            //Populate Helper for workspace
            this.MyWorkSpace.InitializeController(this.WorldMapController);

            // Register this as scriptable object (for running JS)
            HtmlPage.RegisterScriptableObject("MainPage", this);
            user.user_id_pk = 4;

            //Populate event for workspace
            this.MyWorkSpace.CompareControl.Refresh += new EventHandler(CompareControl_Refresh);
            this.MyWorkSpace.CountryDetailsControl.mainPage = this;
            this.MyWorkSpace.CountryDetailsControl._worldMapController = this.WorldMapController;
            this.MyWorkSpace.TradeDataControl.Refresh += new EventHandler(TradeDataControl_Refresh);
            this.MyWorkSpace.TradeDataControl.Complete += new EventHandler(TradeDataControl_Complete);
            this.MyWorkSpace.SearchCountryByIndicators_Completed += new EventHandler(MyWorkSpace_SearchCountryByIndicators_Completed);
            this.MyWorkSpace.MapNavigation += new EventHandler(MyWorkSpace_MapNavigation);
            this.MyWorkSpace.ShorcutView += new EventHandler(MyWorkSpace_ShorcutView);

            //save indicator event
            this.MyWorkSpace.SaveIndicatorButton_Completed += new EventHandler(Workspace_SaveIndicatorButton_Completed);
            this.MyWorkSpace.CountryDetailsControl.SaveGraphButton_Completed += new EventHandler(CountryDetailControl_SaveGraphButton_Completed);
            WorldMapController.InsertMsnUser_Completed += new EventHandler(getCurrentUser);
        }

        #region Draw Country Borders

        void WorldMapController_GetBorder_completed(object sender, EventArgs e)
        {
            object[] result = new object[4];
            (sender as Array).CopyTo(result as Array, 0);

            object[] borderResult = new object[2];
            (result[0] as Array).CopyTo(borderResult as Array, 0);

            if (borderResult[1].ToString() == "MultiPolygon")
            {
                DrawMultiPolygon(borderResult[0] as List<LocationCollection>, result[1] as SolidColorBrush, result[2].ToString(), result[3].ToString());
            }
            else if (borderResult[1].ToString() == "Polygon")
            {
                DrawPolygon(borderResult[0] as LocationCollection, result[1] as SolidColorBrush, result[2].ToString(), result[3].ToString());
            }
        }

        private void DrawMultiPolygon(List<LocationCollection> vertices, SolidColorBrush color, string countryCode, string tooltip)
        {
            MapMultiPolygon myPoly = new MapMultiPolygon();
            myPoly.Vertices = vertices;
            myPoly.Fill = color;
            myPoly.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            myPoly.Opacity = 0.5;
            myPoly.DataContext = countryCode;

            //Add MultiPolygon to map layer
            if (!_isSearchingCountry)
            {
                PolygonLayer.Children.Add(myPoly);
            }
            else
            {
                ToolTipService.SetToolTip(myPoly, tooltip);
                MarkCountryLayer.Children.Add(myPoly);
            }

        }

        private void DrawPolygon(LocationCollection vertices, SolidColorBrush color, string countryCode, string tooltip)
        {
            MapPolygon myPoly = new MapPolygon();
            myPoly.Locations = vertices;
            myPoly.Fill = color;
            myPoly.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            myPoly.StrokeThickness = 1;
            myPoly.Opacity = 0.5;
            myPoly.DataContext = countryCode;

            //Add Polygon to map layer
            if (!_isSearchingCountry)
            {
                PolygonLayer.Children.Add(myPoly);
            }
            else
            {
                ToolTipService.SetToolTip(myPoly, tooltip);
                MarkCountryLayer.Children.Add(myPoly);
            }
        }

        private void DrawCountryBorder(DraggablePushpin p, string tooltip)
        {
            _isSearchingCountry = false;
            WorldMapController.GetCountryBorder(p.country.country_iso_code, p.Background as SolidColorBrush, tooltip);
        }

        #endregion

        void WorldMapController_GetView_TabIndicatorQueryCompleted(object sender, EventArgs e)
        {
            MyWorkSpace.PopulateFavouritedIndicatorsTab(WorldMapController.Context.View_TabIndicators);
            MyWorkSpace.PopulateSearchByIndicatorsTab(WorldMapController.Context.View_TabIndicators);
        }

        void WorldMapController_LoadInitDataCompleted(object sender, EventArgs e)
        {
            this.loadingIndicator.IsBusy = false;
            this.MyMap.IsEnabled = true;

            //Default Pushpin, put here cuz we have to wait the controller data loading process completed.
            PushPinLayer = new MapLayer();
            PushPinLayer.Name = "PushPinLayer";
            MyMap.Children.Add(PushPinLayer);

            DraggablePushpin DefaultPushPin = new DraggablePushpin(PushPinLayer, new Random());
            PushPinPanel.Children.Add(DefaultPushPin);
            DefaultPushPin.Pinned += new EventHandler(DefaultPushPin_Pinned);

            try
            {
                string Country = System.Windows.Browser.HtmlPage.Document.QueryString["a"];
                string IndicatorId = System.Windows.Browser.HtmlPage.Document.QueryString["b"];
                this.KFC(Country, IndicatorId);
            }
            catch (Exception ex)
            {
            }
        }

        void DefaultPushPin_Pinned(object sender, EventArgs e)
        {
            DraggablePushpin pushPin = new DraggablePushpin(PushPinLayer, new Random());
            pushPin.IsOnMap = true;
            PushPinLayer.AddChild(pushPin, (sender as DraggablePushpin).Location);
            pushPin.Location = (sender as DraggablePushpin).Location;

            _isAddingNewPushPin = true;
            ReverseGeocodeLocation(pushPin);

            pushPin.Pinned += new EventHandler(MapPushpin_Pinned);
            pushPin.Clicked += new EventHandler(MapPushpin_Clicked);

            //Mapnavigation
            MyMap.Center = pushPin.Location;
            MyMap.ZoomLevel = 3;
        }

        void MapPushpin_Pinned(object sender, EventArgs e)
        {
            DraggablePushpin p = sender as DraggablePushpin;
            _isAddingNewPushPin = false;
            _currentLocationCountry = p.country.country_id_pk;
            ReverseGeocodeLocation(p);
            this.ArrowLayer.Children.Clear();
        }

        /// <summary>
        /// The event where a pin is clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MapPushpin_Clicked(object sender, EventArgs e)
        {
            // get the selected countries
            tbl_countries thisPinOnCountry = ((DraggablePushpin)sender).country;
            this.MyMap.Center = ((DraggablePushpin)sender).Location;
            this.MyMap.ZoomLevel = 5;

            /* Obsolete code from the previous version using custom child window
            // init a new CustomChildWindow
            CustomChildWindow child = new CustomChildWindow(WorldMapController, thisPinOnCountry, selectedIndicatorPKs);
            child.Show();
             */

            MyWorkSpace.CountryDetailsControl.PopulateData(WorldMapController, thisPinOnCountry, MyWorkSpace.IndicatorIDList);
            MyWorkSpace.MainTabControl.SelectedIndex = 2;            
        }

        void CreateCountryPushPin(DraggablePushpin pushpin)
        {
            StackPanel panel = new StackPanel();

            panel.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            panel.Orientation = System.Windows.Controls.Orientation.Horizontal;

            DraggablePushpin dp = new DraggablePushpin();
            dp.Background = pushpin.Background;
            dp.country = pushpin.country;
            TextBlock tb = new TextBlock();
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Margin = new Thickness(5, 0, 0, 0);
            tb.Foreground = new SolidColorBrush(Colors.White);

            if (pushpin.country != null)
            {
                tb.Text = pushpin.country.country_name;
            }
            else
            {
                tb.Text = "Error country data";
            }

            Button bt = new Button
            {
                Height = 25,
                Template = this.Resources["RemoveButton"] as ControlTemplate
            };
            bt.MouseEnter += new System.Windows.Input.MouseEventHandler(bt_MouseEnter);
            bt.MouseLeave += new System.Windows.Input.MouseEventHandler(bt_MouseLeave);
            bt.Click += new RoutedEventHandler(bt_Click);


            CheckBox chk = new CheckBox();
            chk.VerticalAlignment = VerticalAlignment.Center;
            chk.Margin = new Thickness(5, 0, 5, 0);

            panel.Children.Add(chk);
            panel.Children.Add(bt);
            panel.Children.Add(dp);
            panel.Children.Add(tb);

            panel.DataContext = pushpin;
            pushpin.DataContext = panel;
            CountryListBox.Items.Add(panel);
            pushpin.DataContext = panel;

            DrawCountryBorder(pushpin, "");
        }

        #region removeButton mouse event
        void bt_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        void bt_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        void bt_Click(object sender, RoutedEventArgs e)
        {
            StackPanel item = VisualTreeHelper.GetParent(sender as UIElement) as StackPanel;
            PushPinLayer.Children.Remove(item.DataContext as DraggablePushpin);
            CountryListBox.Items.Remove(item);
            this.ArrowLayer.Children.Clear();

            //Remove polygon in Polygon Layers
            IEnumerable<UIElement> polygons = from n in PolygonLayer.Children
                                              where (n as FrameworkElement).DataContext.ToString() == (item.DataContext as DraggablePushpin).country.country_iso_code
                                              select n;

            foreach (UIElement pol in polygons.ToList<UIElement>())
            {
                PolygonLayer.Children.Remove(pol);
            }
        }
        #endregion

        void UpdateCountryPushPin(DraggablePushpin p)
        {
            StackPanel pushpinPanel = p.DataContext as StackPanel;
            DraggablePushpin pushpin = pushpinPanel.DataContext as DraggablePushpin;
            if (pushpin.country != null)
            {
                TextBlock tb = pushpinPanel.Children[3] as TextBlock;
                tb.Text = pushpin.country.country_name;
            }
            else
            {
                (PushPinPanel.Children[3] as TextBlock).Text = "Error country data";
            }

            DrawCountryBorder(p, "");
        }

        void CompareControl_Refresh(object sender, EventArgs e)
        {
            // get the selected countries
            List<tbl_countries> selectedCountries = new List<tbl_countries>();
            foreach (UIElement test in CountryListBox.Items)
            {
                if (((test as StackPanel).Children[0] as CheckBox).IsChecked == true)
                {
                    DraggablePushpin tmpDP = (test as StackPanel).Children[2] as DraggablePushpin;
                    selectedCountries.Add(tmpDP.country);
                }
            }

            List<int> selectedIndicatorPKs = MyWorkSpace.IndicatorIDList;

            if (selectedCountries.Count > 1 && selectedIndicatorPKs.Count > 0)
            {
                // init a new CustomChildWindow
                //CompareCountriesChildWindow child = new CompareCountriesChildWindow(WorldMapController, selectedCountries, selectedIndicatorPKs);
                //child.Show();
                MyWorkSpace.CompareControl.PopulateData(WorldMapController, selectedCountries, selectedIndicatorPKs);
            }
            else if (selectedIndicatorPKs.Count < 1)
            {
                //ErrorNotification errorPopup = new ErrorNotification("You must select at least 1 indicator");
                //errorPopup.Show();
            }
            else
            {
                //ErrorNotification errorPopup = new ErrorNotification("You must select at least 2 country to compare them");
                //errorPopup.Show();
            }
        }

        void exbt_Click(object sender, RoutedEventArgs e)
        {
            this.ArrowLayer.Children.Clear();
            StackPanel startPanel = VisualTreeHelper.GetParent(sender as Button) as StackPanel;

            Location start = (startPanel.DataContext as DraggablePushpin).Location;

            foreach (StackPanel ele in CountryListBox.Items)
            {

                if (!ele.Equals(startPanel))
                {
                    DraggablePushpin pp = ele.DataContext as DraggablePushpin;
                    drawArrow(start, pp.Location, "");
                }
            }

        }

        void drawArrow(Location start, Location end, string value)
        {
            double changeY = start.Latitude - end.Latitude;
            double changeX = start.Longitude - end.Longitude;

            Grid gr = new Grid();
            ArrowLayer.AddChild(gr, new LocationRect(start, end));
            LineArrow arrow = new LineArrow();

            arrow.StartArrow = Microsoft.Expression.Media.ArrowType.NoArrow;
            arrow.EndArrow = Microsoft.Expression.Media.ArrowType.StealthArrow;
            arrow.ArrowSize = 5;
            arrow.Stroke = new SolidColorBrush(Colors.Black);
            arrow.BendAmount = 0.8;
            arrow.StrokeThickness = 3;
            arrow.Effect = new DropShadowEffect()
            {
                BlurRadius = 10
            };

            if (changeX >= 0 && changeY >= 0)
            {
                arrow.StartCorner = Microsoft.Expression.Media.CornerType.TopRight;
            }

            if (changeX >= 0 && changeY < 0)
            {
                arrow.StartCorner = Microsoft.Expression.Media.CornerType.BottomRight;
            }

            if (changeX < 0 && changeY < 0)
            {
                arrow.StartCorner = Microsoft.Expression.Media.CornerType.BottomLeft;
            }

            ToolTipService.SetToolTip(arrow, value);
            Border border = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                CornerRadius = new CornerRadius(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock content = new TextBlock
            {
                Text = value,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(2)
            };

            border.Child = content;

            gr.Children.Add(arrow);
            gr.Children.Add(border);

            gr.Margin = new Thickness(5);
        }

        void imbt_Click(object sender, RoutedEventArgs e)
        {
            this.ArrowLayer.Children.Clear();
            StackPanel startPanel = VisualTreeHelper.GetParent(sender as Button) as StackPanel;

            Location end = (startPanel.DataContext as DraggablePushpin).Location;

            foreach (StackPanel ele in CountryListBox.Items)
            {

                if (!ele.Equals(startPanel))
                {
                    DraggablePushpin pp = ele.DataContext as DraggablePushpin;
                    drawArrow(pp.Location, end, "");
                }
            }
        }

        void TradeDataControl_Refresh(object sender, EventArgs e)
        {
            List<tbl_countries> countryList = new List<tbl_countries>();
            foreach (StackPanel panel in CountryListBox.Items)
            {
                DraggablePushpin pp = panel.DataContext as DraggablePushpin;
                countryList.Add(pp.country);
            }

            MyWorkSpace.TradeDataControl.PopulateData(countryList);
        }

        void TradeDataControl_Complete(object sender, EventArgs e)
        {

            tbl_countries a = MyWorkSpace.TradeDataControl.CountryComboBox.SelectedItem as tbl_countries;
            string b = MyWorkSpace.TradeDataControl.TypeComboBox.SelectedItem.ToString();
            int c = int.Parse(MyWorkSpace.TradeDataControl.YearComboBox.SelectedItem.ToString());

            if (b == "Import")
            {
                List<int> exportList = new List<int>();
                foreach (StackPanel panel in CountryListBox.Items)
                {
                    DraggablePushpin pp = panel.DataContext as DraggablePushpin;
                    if (a.country_id_pk != pp.country.country_id_pk)
                    {
                        exportList.Add(pp.country.country_id_pk);
                    }
                }

                _currentImportCountryPK = a.country_id_pk;
                this.WorldMapController.GetImportData(a.country_id_pk, exportList, c);
                this.WorldMapController.GetImportData_Completed += new EventHandler(WorldMapController_GetImportData_Completed);
            }
            else
            {
                List<int> importList = new List<int>();
                foreach (StackPanel panel in CountryListBox.Items)
                {
                    DraggablePushpin pp = panel.DataContext as DraggablePushpin;
                    if (a.country_id_pk != pp.country.country_id_pk)
                    {
                        importList.Add(pp.country.country_id_pk);
                    }
                }

                _currentExportCountryPK = a.country_id_pk;
                this.WorldMapController.GetExportData(a.country_id_pk, importList, c);
                this.WorldMapController.GetExportData_Completed += new EventHandler(WorldMapController_GetExportData_Completed);
            }
            //}
        }

        void WorldMapController_GetExportData_Completed(object sender, EventArgs e)
        {
            this.ArrowLayer.Children.Clear();
            Location start = new Location();

            foreach (StackPanel ele in CountryListBox.Items)
            {
                if ((ele.DataContext as DraggablePushpin).country.country_id_pk == _currentExportCountryPK)
                {
                    start = (ele.DataContext as DraggablePushpin).Location;
                }
            }

            foreach (StackPanel ele in CountryListBox.Items)
            {
                if ((ele.DataContext as DraggablePushpin).country.country_id_pk != _currentImportCountryPK)
                {
                    decimal? value = null;
                    DraggablePushpin pp = ele.DataContext as DraggablePushpin;
                    foreach (tbl_trades trd in this.WorldMapController.Context.tbl_trades)
                    {
                        if (trd.country_from_id == pp.country.country_id_pk)
                        {
                            value = trd.import_value;
                        }
                    }

                    if (value != null)
                    {
                        drawArrow(start, (ele.DataContext as DraggablePushpin).Location, value.ToString());
                    }
                    else
                    {
                        drawArrow(start, (ele.DataContext as DraggablePushpin).Location, "No data");
                    }
                }
            }
        }

        void WorldMapController_GetImportData_Completed(object sender, EventArgs e)
        {
            this.ArrowLayer.Children.Clear();
            Location end = new Location();

            foreach (StackPanel ele in CountryListBox.Items)
            {
                if ((ele.DataContext as DraggablePushpin).country.country_id_pk == _currentImportCountryPK)
                {
                    end = (ele.DataContext as DraggablePushpin).Location;
                }
            }

            foreach (StackPanel ele in CountryListBox.Items)
            {
                if ((ele.DataContext as DraggablePushpin).country.country_id_pk != _currentImportCountryPK)
                {
                    decimal? value = null;
                    DraggablePushpin pp = ele.DataContext as DraggablePushpin;
                    foreach (tbl_trades trd in this.WorldMapController.Context.tbl_trades)
                    {
                        if (trd.country_from_id == pp.country.country_id_pk)
                        {
                            value = trd.import_value;
                        }
                    }

                    if (value != null)
                    {
                        drawArrow((ele.DataContext as DraggablePushpin).Location, end, value.ToString());
                    }
                    else
                    {
                        drawArrow((ele.DataContext as DraggablePushpin).Location, end, "No data");
                    }
                }
            }
        }

        void LoadCountryPushpin(Location loc, Random seed)
        {
            DraggablePushpin pushPin = new DraggablePushpin(PushPinLayer, seed);
            pushPin.IsOnMap = true;
            PushPinLayer.AddChild(pushPin, loc);
            pushPin.Location = loc;

            _isAddingNewPushPin = true;
            ReverseGeocodeLocation(pushPin);
            pushPin.Pinned += new EventHandler(MapPushpin_Pinned);
            pushPin.Clicked += new EventHandler(MapPushpin_Clicked);
        }

        void HelpToggleButton_Click(object sender, RoutedEventArgs e)
        {
            About b = new About();
            b.Show();
        }

        void InitializeAfterLoggigIn()
        {
            MyWorkSpace.InitializeAfterLogin();
            SaveCountry.Visibility = Visibility.Visible;
        }

        void InitializeAfterLoggigOut()
        {
            MyWorkSpace.InitializeAfterLogout();
            SaveCountry.Visibility = Visibility.Collapsed;
        }

        #region Reverse Geocode region
        private PlatformServices.GeocodeServiceClient geocodeClient;

        private PlatformServices.GeocodeServiceClient GeocodeClient
        {
            get
            {
                if (null == geocodeClient)
                {
                    //Handle http/https
                    bool httpsUriScheme = !Application.Current.IsRunningOutOfBrowser && HtmlPage.Document.DocumentUri.Scheme.Equals(Uri.UriSchemeHttps);

                    BasicHttpBinding binding = new BasicHttpBinding(httpsUriScheme ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None);
                    UriBuilder serviceUri = new UriBuilder("http://dev.virtualearth.net/webservices/v1/GeocodeService/GeocodeService.svc");

                    if (httpsUriScheme)
                    {
                        //For https, change the UriSceheme to https and change it to use the default https port.
                        serviceUri.Scheme = Uri.UriSchemeHttps;
                        serviceUri.Port = -1;
                    }

                    //Create the Service Client
                    geocodeClient = new PlatformServices.GeocodeServiceClient(binding, new EndpointAddress(serviceUri.Uri));
                    geocodeClient.ReverseGeocodeCompleted += new EventHandler<PlatformServices.ReverseGeocodeCompletedEventArgs>(client_ReverseGeocodeCompleted);
                }
                return geocodeClient;
            }
        }

        private GeocodeLayer geocodeLayer;

        private GeocodeLayer GeocodeLayer
        {
            get
            {
                if (null == geocodeLayer)
                {
                    geocodeLayer = new GeocodeLayer(MyMap);
                }
                return geocodeLayer;
            }
        }

        // Call service to do reverse geocode ... async call.
        private void ReverseGeocodeAsync(DraggablePushpin pushpin)
        {
            PlatformServices.ReverseGeocodeRequest request = new PlatformServices.ReverseGeocodeRequest();
            request.Culture = "en-US";
            request.Location = new Location();
            request.Location.Latitude = pushpin.Location.Latitude;
            request.Location.Longitude = pushpin.Location.Longitude;
            // Don't raise exceptions.
            request.ExecutionOptions = new PlatformServices.ExecutionOptions();
            request.ExecutionOptions.SuppressFaults = true;

            geocodesInProgress++;

            MyMap.CredentialsProvider.GetCredentials(
                (Credentials credentials) =>
                {
                    //Pass in credentials for web services call.
                    //Replace with your own Credentials.
                    request.Credentials = credentials;

                    // Make asynchronous call to fetch the data ... pass state object.
                    GeocodeClient.ReverseGeocodeAsync(request, pushpin);
                });
        }

        // Keep track of addresses already found.
        private Dictionary<string, bool> found = new Dictionary<string, bool>();

        private void client_ReverseGeocodeCompleted(object sender, PlatformServices.ReverseGeocodeCompletedEventArgs e)
        {
            // Finished.
            lock (waitingToReverseGeocode)
            {
                geocodesInProgress--;
            }

            DraggablePushpin _currentPushpin = (DraggablePushpin)e.UserState;
            Location location = _currentPushpin.Location;
            string outputString = string.Format("Location ({0:f6}, {1:f6}) : ", location.Latitude, location.Longitude);

            try
            {
                if (e.Result.ResponseSummary.StatusCode != PlatformServices.ResponseStatusCode.Success)
                {
                    //Output.Text = "error geocoding ... status <" + e.Result.ResponseSummary.StatusCode.ToString() + ">";
                    ErrorNotification cw = new ErrorNotification("Error1: Error while geocoding, please choose another location");
                    cw.Show();
                    if (_currentPushpin.DataContext != null)
                    {
                        this.CountryListBox.Items.Remove(_currentPushpin.DataContext as StackPanel);
                    }

                    foreach (UIElement ue in PolygonLayer.Children)
                    {
                        if (_currentPushpin != null && _currentPushpin.country != null && (ue as UserControl).DataContext.ToString() == _currentPushpin.country.country_iso_code)
                        {
                            PolygonLayer.Children.Remove(ue);
                            break;
                        }
                    }

                    this.PushPinLayer.Children.Remove(_currentPushpin);
                }
                else if (0 == e.Result.Results.Count)
                {
                    //Output.Text = outputString + "No results";
                    ErrorNotification cw = new ErrorNotification("Error2: Error while geocoding, please choose another location");
                    cw.Show();

                    if (_currentPushpin.DataContext != null)
                    {
                        this.CountryListBox.Items.Remove(_currentPushpin.DataContext as StackPanel);
                    }

                    foreach (UIElement ue in PolygonLayer.Children)
                    {
                        if (_currentPushpin != null && _currentPushpin.country != null && (ue as UserControl).DataContext.ToString() == _currentPushpin.country.country_iso_code)
                        {
                            PolygonLayer.Children.Remove(ue);
                            break;
                        }
                    }

                    this.PushPinLayer.Children.Remove(_currentPushpin);
                }
                else
                {
                    string formatted = e.Result.Results[0].Address.CountryRegion;
                    object a = e.Result.Results[0].MatchCodes.ToString();

                    outputString = outputString + formatted;

                    //Check duplicated
                    if (found.ContainsKey(formatted))
                    {
                        //Output.Text = outputString + " (duplicate)";
                    }
                    else
                    {
                        found[formatted] = true;
                        //Output.Text = outputString + "  (" + e.Result.Results[0].Locations[0].CalculationMethod + ")";
                        GeocodeLayer.AddResult(e.Result.Results[0]);
                    }

                    tbl_countries country = WorldMapController.GetCountry(formatted);

                    if (!_isAddingNewPushPin && _currentLocationCountry == country.country_id_pk)
                    {
                        //Do nothing cuz this means moving the pushpin within the country
                    }
                    else
                    {
                        //Check if Pushpin is existed ?
                        bool isExisting = false;
                        foreach (DraggablePushpin pp in PushPinLayer.Children)
                        {
                            if (pp.country != null && country.country_id_pk == pp.country.country_id_pk)
                            {
                                isExisting = true;
                                break;
                            }
                        }

                        foreach (UIElement ue in PolygonLayer.Children)
                        {
                            if (_currentPushpin != null && _currentPushpin.country != null && (ue as UserControl).DataContext.ToString() == _currentPushpin.country.country_iso_code)
                            {
                                PolygonLayer.Children.Remove(ue);
                                break;
                            }
                        }

                        //Add to pushpin title
                        _currentPushpin.Title = formatted;
                        _currentPushpin.country = country;

                        if (!isExisting)
                        {
                            ToolTipService.SetToolTip(_currentPushpin, new ToolTip()
                            {
                                DataContext = _currentPushpin,
                                Style = this.Resources["CustomInfoboxStyle"] as Style
                            });

                            if (_isAddingNewPushPin)
                            {
                                CreateCountryPushPin(_currentPushpin);
                            }
                            else
                            {
                                UpdateCountryPushPin(_currentPushpin);
                            }
                        }
                        else
                        {
                            this.PushPinLayer.Children.Remove(_currentPushpin);
                            StackPanel pushpinPanel = _currentPushpin.DataContext as StackPanel;
                            this.CountryListBox.Items.Remove(pushpinPanel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorNotification cw = new ErrorNotification("Error3: Error while geocoding the location, please choose another country");
                cw.Show();
            }

            // See if there are more waiting to run.
            ReverseGeocodeFromQueue();
        }

        // Throttle calls on geocoding service.
        private const int MaxGeocodes = 1;

        // Locations waiting to be reverse geocoded.
        private Queue<DraggablePushpin> waitingToReverseGeocode = new Queue<DraggablePushpin>();

        // Waiting for results from the server for this many.
        int geocodesInProgress = 0;

        bool Geocoding
        {
            get { return geocodesInProgress > 0; }
        }

        // Runs as many reverse geocodes as it can from the queue.
        private void ReverseGeocodeFromQueue()
        {
            lock (waitingToReverseGeocode)
            {
                while (geocodesInProgress < MaxGeocodes && waitingToReverseGeocode.Count > 0)
                {
                    ReverseGeocodeAsync(waitingToReverseGeocode.Dequeue());
                }
            }
        }

        private void ReverseGeocodeLocation(DraggablePushpin pushpin)
        {
            // All calls go through the queue.
            lock (waitingToReverseGeocode)
            {
                waitingToReverseGeocode.Enqueue(pushpin);
                ReverseGeocodeFromQueue();
            }
        }

        private void ReverseGeocodeLocation(IList<DraggablePushpin> locations)
        {
            lock (waitingToReverseGeocode)
            {
                foreach (DraggablePushpin location in locations)
                {
                    waitingToReverseGeocode.Enqueue(location);
                }
                ReverseGeocodeFromQueue();
            }
        }

        #endregion

        #region Live ID functions

        private void SignInOff_Checked(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Invoke("signIn");
            SignInOut.IsChecked = false;
        }

        private void SignInOff_Unchecked(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Invoke("signOut");
        }

        [ScriptableMember()]
        public void SignInCompleted(bool signedin, string cid)
        {
            if (signedin)
            {
                SignInOut.Content = "Sign-Out";
                SignInInformation.Text = "Pending info...";
                //SignInInformation.Text = cid + " is signed in...";
            }
            else
            {
                SignInOut.Content = "Sign-In";
                SignInInformation.Text = "Not signed in";
                //SignInInformation.Text = cid + " is signed out...";
            }
        }

        #endregion
       
        #region workspace save n load

        private void SaveCountry_Click(object sender, RoutedEventArgs e)
        {
            SaveCountryList(user);
        }

        void getCurrentUser(object sender, EventArgs e)
        {
            WorldMapController.CheckExist(user.msn_id);
        }

        [ScriptableMember()]
        public void SignInMessengerCompleted(string cid, string userName, string contacts)
        {
            //check if first time user            
            user = new tbl_users();
            user.user_name = userName;
            user.msn_id = cid;
            WorldMapController.CheckExist(cid);
            WorldMapController.LoadUserData_Completed += new EventHandler(WorldMapController_LoadUserData_Completed);
        }

        void WorldMapController_LoadUserData_Completed(object sender, EventArgs e)
        {
            string userName = "";
            if (sender != null)
                user = sender as tbl_users;
            userName = user.user_name;
            if (sender == null)
            {
                userName = user.user_name;
                WorldMapController.InsertUser(user);
                //map msn_id to user                
            }
            else
            {
                SignInInformation.Text = userName + " is signed in...";
                // load data here
                WorldMapController.LoadUserCountry_Completed += new EventHandler(WorldMapController_LoadUserCountry_Completed);
                WorldMapController.LoadUserCountry();
                WorldMapController.LoadUserIndicator_Completed += new EventHandler(WorldMapController_LoadUserIndicator_Compelted);
                WorldMapController.LoadUserIndicator();
                WorldMapController.LoadUserGraph_Completed += new EventHandler(WorldMapController_LoadUserGraph_Completed);
                WorldMapController.LoadUserGraph();

                InitializeAfterLoggigIn();
            }
        }

        private void SaveCountryList(tbl_users tbl_user)
        {
            List<ref_user_country> countryList = new List<ref_user_country>();
            foreach (StackPanel panel in CountryListBox.Items)
            {
                DraggablePushpin pp = panel.DataContext as DraggablePushpin;
                Location location = pp.Location;
                ref_user_country user_country = new ref_user_country();
                user_country.country_id = pp.country.country_id_pk;
                user_country.lat = (decimal)location.Latitude;
                user_country.@long = (decimal)location.Longitude;
                countryList.Add(user_country);
            }
            WorldMapController.InsertUserCountry(tbl_user, countryList);
        }

        void WorldMapController_LoadUserCountry_Completed(object sender, EventArgs e)
        {
            LoadCountryList();
        }

        private void LoadCountryList()
        {
            List<ref_user_country> user_countries = WorldMapController.LoadRefUserCountry(user);
            //TEST
            Random backgroundSeed = new Random();
            foreach (ref_user_country user_country in user_countries)
            {
                LoadCountryPushpin(new Location((double)user_country.lat, (double)user_country.@long), backgroundSeed);
            }
            //LoadCountryPushpin(new Location(34.52280, 69.17610), backgroundSeed);
            //LoadCountryPushpin(new Location(69.17610, 34.52280), backgroundSeed);
            //LoadCountryPushpin(new Location(-34.61180, -58.41730), backgroundSeed);
            //LoadCountryPushpin(new Location(45.42150, -75.69190), backgroundSeed);
        }

        private void LoadIndicatorList()
        {
            List<ref_user_indicator> user_indicators = WorldMapController.LoadRefUserIndicator(user);
            List<int> indicatorList = new List<int>();
            foreach (ref_user_indicator user_indicator in user_indicators)
            {
                indicatorList.Add(user_indicator.indicator_id);
            }
            this.MyWorkSpace.LoadIndicatorsList(indicatorList);
        }

        void WorldMapController_SaveCountryListCompleted(object sender, EventArgs e)
        {
            ChildWindow ch = new ChildWindow();
            ch.Content = new TextBlock
            {
                Text = sender.ToString()
            };
            ch.Show();
        }

        #endregion

        #region save n load indicator
        void Workspace_SaveIndicatorButton_Completed(object sender, EventArgs e)
        {
            List<int> indicatorList = MyWorkSpace.IndicatorIDList;
            WorldMapController.SaveIndicatorList(indicatorList, user);
        }

        void WorldMapController_LoadUserIndicator_Compelted(object sender, EventArgs e)
        {
            LoadIndicatorList();
        }

        void WorldMapController_SaveIndicatorListCompleted(object sender, EventArgs e)
        {
            ChildWindow ch = new ChildWindow();
            ch.Content = new TextBlock
            {
                Text = sender.ToString()
            };
            ch.Show();
        }
        #endregion

        #region Load Graph By Shortcut

        private void KFC(string countryName, string IndicatorId)
        {
            List<int> iList = new List<int>();
            iList.Add(int.Parse(IndicatorId));

            tbl_countries c = WorldMapController.GetCountry(countryName);

            CustomChildWindow flotwin = new CustomChildWindow(WorldMapController, c, iList);
            flotwin.Title = countryName;

            System.Windows.Controls.Primitives.Popup popup = new System.Windows.Controls.Primitives.Popup();            
            flotwin.MouseLeftButtonDown += (a, x) =>
            {
                this.LayoutRoot.Children.Remove(popup);
                this.LayoutRoot.Children.Add(popup);
                popup.IsOpen = true;
            };

            flotwin.Closing += (ss, ee) =>
            {
                popup.IsOpen = false;
            };
            popup.Child = flotwin;
            popup.IsOpen = true;


            LayoutRoot.Children.Add(popup);
        }

        private void LoadGraph(tbl_countries country, List<int> indicatorIdList)
        {
            CustomChildWindow flotwin = new CustomChildWindow(WorldMapController, country, indicatorIdList);
            flotwin.Title = country.country_name;

            System.Windows.Controls.Primitives.Popup popup = new System.Windows.Controls.Primitives.Popup();            

            flotwin.MouseLeftButtonDown += (a, x) =>
            {
                this.LayoutRoot.Children.Remove(popup);
                this.LayoutRoot.Children.Add(popup);
                popup.IsOpen = true;
            };

            flotwin.Closing += (ss, ee) =>
            {
                popup.IsOpen = false;
            };
            popup.Child = flotwin;
            popup.IsOpen = true;

            LayoutRoot.Children.Add(popup);

            //CustomChildWindow window = new CustomChildWindow(WorldMapController, country, indicatorIdList);
            //window.Show();
        }

        public void LoadUserGraph()
        {
            List<tbl_graphs> graphs = WorldMapController.LoadUserGraph(user);
            MyWorkSpace.PopulateShortcutListbox(graphs);
        }

        private void WorldMapController_LoadUserGraph_Completed(object sender, EventArgs e)
        {
            LoadUserGraph();
        }

        public void CountryDetailControl_SaveGraphButton_Completed(object sender, EventArgs e)
        {
            tbl_graphs graph = new tbl_graphs();
            graph.user_id = user.user_id_pk;
            graph.indicator_list = "";
            int count = 0;
            foreach (tbl_indicators i in MyWorkSpace.CountryDetailsControl.shortListIndicatorsSelected)
            {
                count++;
                if (count == MyWorkSpace.CountryDetailsControl.shortListIndicatorsSelected.Count)
                    graph.indicator_list += i.indicator_id_pk + "";
                else
                    graph.indicator_list += i.indicator_id_pk + "|";
            }


            graph.country_list = MyWorkSpace.CountryDetailsControl._selectedCountry.country_id_pk + "";
            WorldMapController.saveGraph(graph);
        }

        void WorldMapController_SaveGraphCompleted(object sender, EventArgs e)
        {
            ChildWindow ch = new ChildWindow();
            ch.Content = new TextBlock
            {
                Text = sender.ToString()
            };
            ch.Show();

            MyWorkSpace.PopulateShortcutListbox(WorldMapController.Context.tbl_graphs.ToList());
        }

        void MyWorkSpace_ShorcutView(object sender, EventArgs e)
        {
            tbl_graphs graph = sender as tbl_graphs;

            tbl_countries tbl_country = new tbl_countries();

            List<int> indicatorIdList = new List<int>();
            string country_list = graph.country_list;
            string[] countries = country_list.Split('|');
            foreach (string country in countries)
            {
                tbl_country = WorldMapController.GetCountry(Int32.Parse(country));
            }

            string indicator_list = graph.indicator_list;
            string[] indicators = graph.indicator_list.Split('|');
            foreach (string indicator in indicators)
            {
                indicatorIdList.Add(Int32.Parse(indicator));
            }

            LoadGraph(tbl_country, indicatorIdList);
        }
        #endregion
        
        #region Search Country
        void MyWorkSpace_SearchCountryByIndicators_Completed(object sender, EventArgs e)
        {
            IEnumerable<ref_country_indicator> result = sender as IEnumerable<ref_country_indicator>;
            _isSearchingCountry = true;
            MarkCountryLayer.Children.Clear();

            float? maxValue = (from val in result
                               select val.country_indicator_value).Max();

            float? minValue = (from val in result
                               select val.country_indicator_value).Min();

            List<tbl_countries> resultList = new List<tbl_countries>();

            foreach (ref_country_indicator r in result)
            {
                tbl_countries country = WorldMapController.GetCountry(r.country_id.Value);
                resultList.Add(country);
                string isoCode = country.country_iso_code;

                float? percent;

                if (maxValue != minValue)
                {
                    percent = (r.country_indicator_value - minValue) / (maxValue - minValue) * 100;
                }
                else
                {
                    percent = 100;
                }

                byte opacity = (byte)(255 * percent / 100);
                WorldMapController.GetCountryBorder(isoCode, new SolidColorBrush(Color.FromArgb(255, 39, 136, 59)), country.country_name + ": " + r.country_indicator_value.ToString());
            }

            MyWorkSpace.PopulateSearchByIndicatorResultBox(resultList);
        }

        void MyWorkSpace_MapNavigation(object sender, EventArgs e)
        {
            tbl_countries country = sender as tbl_countries;
            MyMap.Center = new Location((double)country.country_latitude.Value, (double)country.country_longitude.Value);
            MyMap.ZoomLevel = 3;
        }
        #endregion
    }
}
