using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Design;
using NCRVisual.web.DataModel;
using System.Globalization;
using Microsoft.Expression.Controls;
using System.Windows.Media.Effects;

namespace WorldMap
{
    public partial class MainPage : UserControl
    {
        #region private variables
        private LocationConverter locConverter = new LocationConverter();
        private bool _isControlpanelOpened = false;
        private bool _isCountryListPanelOpened = true;
        private DraggablePushpin _currentPushpin;
        private bool _isAddingNewPushPin;
        private List<int> selectedIndicatorPKs = new List<int>();

        private int _currentImportCountryPK = 0;
        private int _currentExportCountryPK = 0;

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
        #endregion

        /// <summary>
        /// default constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            //Event Handler
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            this.ControlPanelButton.Click += new RoutedEventHandler(ControlPanelButton_Click);
            this.CountryListPanelButton.Click += new RoutedEventHandler(CountryListPanelButton_Click);
        }

        /// <summary>
        /// Run after all page is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.WorldMapController = new Controller();
            this.WorldMapController.LoadInitDataCompleted += new EventHandler(WorldMapController_LoadInitDataCompleted);
            this.WorldMapController.GetView_TabIndicatorQueryCompleted += new EventHandler(WorldMapController_GetView_TabIndicatorQueryCompleted);
        }

        void WorldMapController_GetView_TabIndicatorQueryCompleted(object sender, EventArgs e)
        {
            //TreeViewItem item = new
            List<int> tabId = new List<int>();
            foreach (View_TabIndicator indicator in WorldMapController.Context.View_TabIndicators)
            {
                if (!tabId.Contains(indicator.tab_id_pk))
                {
                    tabId.Add(indicator.tab_id_pk);
                    TreeViewItem item = new TreeViewItem();
                    item.Header = indicator.tab_name;
                    item.DataContext = indicator.tab_id_pk;
                    this.IndicatorTreeView.Items.Add(item);
                }

                foreach (TreeViewItem item in this.IndicatorTreeView.Items)
                {
                    if (item.Header.ToString() == indicator.tab_name.ToString())
                    {
                        Grid grid = new Grid();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });                        
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength() });

                        ToolTipService.SetToolTip(grid, new ToolTip()
                        {
                            Content = indicator.indicator_description,
                            //Style = this.Resources["CustomInfoboxStyle"] as Style,
                        });
                        
                        TextBlock name = new TextBlock { Text = indicator.indicator_name };
                        CheckBox chk = new CheckBox();
                        chk.Tag = indicator.indicator_id_pk;
                        chk.Checked += new RoutedEventHandler(IndicatorCheckbox_Checked);
                        chk.Unchecked += new RoutedEventHandler(IndicatorCheckbox_Unchecked);
                        
                        grid.Children.Add(name);
                        grid.Children.Add(chk);

                        Grid.SetColumn(name, 1);

                        item.Items.Add(grid);
                        break;
                    }
                }
            }
        }

        void WorldMapController_LoadInitDataCompleted(object sender, EventArgs e)
        {
            this.LoadingPanel.Visibility = Visibility.Collapsed;
            this.MyMap.IsEnabled = true;

            //Default Pushpin, put here cuz we have to wait the controller data loading process completed.
            PushPinLayer = new MapLayer();
            PushPinLayer.Name = "PushPinLayer";
            MyMap.Children.Add(PushPinLayer);

            DraggablePushpin DefaultPushPin = new DraggablePushpin(PushPinLayer);
            PushPinPanel.Children.Add(DefaultPushPin);
            DefaultPushPin.Pinned += new EventHandler(DefaultPushPin_Pinned);
        }

        void ControlPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isControlpanelOpened)
            {
                this.IndicatorListOpen.Begin();
            }
            else
            {
                this.IndicatorListClose.Begin();
            }
            _isControlpanelOpened = !_isControlpanelOpened;
        }

        void CountryListPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCountryListPanelOpened)
            {
                this.CountryListOpen.Begin();
            }
            else
            {
                this.CountryListClose.Begin();
            }
            _isCountryListPanelOpened = !_isCountryListPanelOpened;
        }

        void DefaultPushPin_Pinned(object sender, EventArgs e)
        {
            DraggablePushpin pushPin = new DraggablePushpin(PushPinLayer);
            pushPin.IsOnMap = true;
            PushPinLayer.AddChild(pushPin, (sender as DraggablePushpin).Location);
            pushPin.Location = (sender as DraggablePushpin).Location;

            _isAddingNewPushPin = true;
            ReverseGeocodeLocation((sender as DraggablePushpin).Location);
            _currentPushpin = pushPin;

            pushPin.Pinned += new EventHandler(MapPushpin_Pinned);
            pushPin.Clicked += new EventHandler(MapPushpin_Clicked);
        }

        void MapPushpin_Pinned(object sender, EventArgs e)
        {
            DraggablePushpin p = sender as DraggablePushpin;

            _isAddingNewPushPin = false;
            ReverseGeocodeLocation(p.Location);
            _currentPushpin = p;

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
            // init a new CustomChildWindow
            CustomChildWindow child = new CustomChildWindow(WorldMapController, thisPinOnCountry, selectedIndicatorPKs);
            child.Show();
        }

        void CreateCountryPushPin(DraggablePushpin pushpin)
        {
            StackPanel panel = new StackPanel();
            panel.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            panel.Orientation = Orientation.Horizontal;
            
            DraggablePushpin dp = new DraggablePushpin();
            dp.Background = pushpin.Background;
            dp.country = pushpin.country;                        
            TextBlock tb = new TextBlock();
            tb.TextAlignment = TextAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.Margin = new Thickness(5, 0, 0, 0);

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
        }
        #endregion

        void UpdateCountryPushPin(StackPanel pushpinPanel)
        {
            DraggablePushpin pushpin = pushpinPanel.DataContext as DraggablePushpin;
            if (pushpin.country != null)
            {
                int a = pushpinPanel.Children.Count;
                TextBlock tb = pushpinPanel.Children[3] as TextBlock;
                tb.Text = pushpin.country.country_name;
            }
            else
            {
                (PushPinPanel.Children[3] as TextBlock).Text = "Error country data";
            }
        }
     
        private void buttonCompareCountries_Click(object sender, RoutedEventArgs e)
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

            if (selectedCountries.Count > 1 && selectedIndicatorPKs.Count > 0)
            {
                // init a new CustomChildWindow
                CompareCountriesChildWindow child = new CompareCountriesChildWindow(WorldMapController, selectedCountries, selectedIndicatorPKs);
                child.Show();
            }
            else if (selectedIndicatorPKs.Count < 1)
            {
                ErrorNotification errorPopup = new ErrorNotification("You must select at least 1 indicator");
                errorPopup.Show();
            }
            else
            {
                ErrorNotification errorPopup = new ErrorNotification("You must select at least 2 country to compare them");
                errorPopup.Show();
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
                    drawArrow(start, pp.Location,"");
                }
            }
            
        }

        private void drawArrow(Location start, Location end, string value)
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
                    drawArrow (pp.Location, end,"");
                }
            }
        }

        private void ViewTrade_Click(object sender, RoutedEventArgs e)
        {
            List<tbl_countries> countryList = new List<tbl_countries>();
            foreach (StackPanel panel in CountryListBox.Items)
            {
                DraggablePushpin pp = panel.DataContext as DraggablePushpin;
                countryList.Add(pp.country);
            }

            TradeMode tradeWindow = new TradeMode(countryList);
            tradeWindow.Closed += new EventHandler(tradeWindow_Closed);
            tradeWindow.Show();                        
        }

        void tradeWindow_Closed(object sender, EventArgs e)
        {            
            TradeMode tm = sender as TradeMode;
            if (tm.DialogResult == true)
            {
                tbl_countries a = tm.CountryComboBox.SelectedItem as tbl_countries;
                string b = tm.TypeComboBox.SelectedItem.ToString();
                int c = int.Parse(tm.YearComboBox.SelectedItem.ToString());

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
            }
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
                if ((ele.DataContext as DraggablePushpin).country.country_id_pk == _currentImportCountryPK )
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
        private void ReverseGeocodeAsync(Location location)
        {
            PlatformServices.ReverseGeocodeRequest request = new PlatformServices.ReverseGeocodeRequest();            
            request.Culture = "en-US";            
            request.Location = new Location();
            request.Location.Latitude = location.Latitude;
            request.Location.Longitude = location.Longitude;
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
                    GeocodeClient.ReverseGeocodeAsync(request, location);
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

            Location location = (Location)e.UserState;
            string outputString = string.Format("Location ({0:f6}, {1:f6}) : ", location.Latitude, location.Longitude);

            try
            {
                if (e.Result.ResponseSummary.StatusCode != PlatformServices.ResponseStatusCode.Success)
                {
                    //Output.Text = "error geocoding ... status <" + e.Result.ResponseSummary.StatusCode.ToString() + ">";
                    ErrorNotification cw = new ErrorNotification("Error while geocoding, please choose another location");
                    cw.Show();
                    if (_currentPushpin.DataContext != null)
                    {
                        this.CountryListBox.Items.Remove(_currentPushpin.DataContext as StackPanel);
                    }

                    this.PushPinLayer.Children.Remove(_currentPushpin);
                }
                else if (0 == e.Result.Results.Count)
                {
                    //Output.Text = outputString + "No results";
                    ErrorNotification cw = new ErrorNotification("Error while geocoding, please choose another location");
                    cw.Show();

                    if (_currentPushpin.DataContext != null)
                    {
                        this.CountryListBox.Items.Remove(_currentPushpin.DataContext as StackPanel);
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


                    //Add to pushpin title

                    tbl_countries country = WorldMapController.GetCountry(formatted);
                    _currentPushpin.Title = formatted;
                    _currentPushpin.country = country;

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
                        UpdateCountryPushPin(_currentPushpin.DataContext as StackPanel);
                    }

                }
            }
            catch (Exception ex)
            {                
                ErrorNotification cw = new ErrorNotification("Error while geocoding the location, please choose another country");
                cw.Show();                
            }

            // See if there are more waiting to run.
            ReverseGeocodeFromQueue();
        }

        // Throttle calls on geocoding service.
        private const int MaxGeocodes = 3;

        // Locations waiting to be reverse geocoded.
        private Queue<Location> waitingToReverseGeocode = new Queue<Location>();

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


        private void ReverseGeocodeLocation(Location location)
        {
            // All calls go through the queue.
            lock (waitingToReverseGeocode)
            {
                waitingToReverseGeocode.Enqueue(location);
                ReverseGeocodeFromQueue();
            }
        }

        private void ReverseGeocodeLocation(IList<Location> locations)
        {
            lock (waitingToReverseGeocode)
            {
                foreach (Location location in locations)
                {
                    waitingToReverseGeocode.Enqueue(location);
                }
                ReverseGeocodeFromQueue();
            }
        }

        #endregion

        #region indicator checkboxes event handlers
        private void IndicatorCheckbox_Checked(object sender, RoutedEventArgs e)
        {

            //if (CountryListBox.Items.Count > 1 && selectedIndicatorPKs.Count > 0)
            //{
            //    MessageBox.Show("You can only choose 1 indicator if 2 or more countries are choosen");
            //    ((CheckBox)sender).IsChecked = false;
            //}
            //else
            //{
            selectedIndicatorPKs.Add(Convert.ToInt32(((CheckBox)sender).Tag));
            //}
        }

        private void IndicatorCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            selectedIndicatorPKs.Remove(Convert.ToInt32(((CheckBox)sender).Tag));
        }
        #endregion             
    }
}
