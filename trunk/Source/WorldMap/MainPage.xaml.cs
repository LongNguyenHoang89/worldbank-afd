using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using Microsoft.Expression.Controls;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Design;
using NCRVisual.web.Services;
using NCRVisual.web.DataModel;
using System.Windows.Media;
using System.Windows.Data;

namespace WorldMap
{
    public partial class MainPage : UserControl
    {
        #region private variables
        private LocationConverter locConverter = new LocationConverter();
        private bool _isControlpanelOpened = false;
        private bool _isCountryListPanelOpened = false;
        private DraggablePushpin _currentPushpin;
        private bool _isAddingNewPushPin;
        private List<int> selectedIndicatorPKs = new List<int>();
        private HashSet<tbl_countries> selectedCountries = new HashSet<tbl_countries>();
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

            //Set default visual state
            VisualStateManager.GoToState(this, "ControlPanelClose", true);            
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

            this.IndicatorListBox.ItemsSource = WorldMapController.Context.tbl_indicators;
        }

        void WorldMapController_LoadInitDataCompleted(object sender, EventArgs e)
        {
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
                VisualStateManager.GoToState(this, "ControlPanelOpen", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "ControlPanelClose", true);
            }
            _isControlpanelOpened = !_isControlpanelOpened;
        }

        void CountryListPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isCountryListPanelOpened)
            {
                VisualStateManager.GoToState(this, "CountryListPanelOpen", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "CountryListPanelClose", true);
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
        }

        void MapPushpin_Clicked(object sender, EventArgs e)
        {
            // get the selected countries
            List<tbl_countries> selectedCountries = new List<tbl_countries>();
            foreach ( object test in CountryListBox.Items )
            {
                Panel tmpP = (Panel)test;
                foreach (object tmpO in tmpP.Children)
                {
                    if (tmpO.GetType().Equals(typeof(DraggablePushpin)))
                    {
                        DraggablePushpin tmpDP = (DraggablePushpin)tmpO;
                        selectedCountries.Add(tmpDP.country);
                    }
                }
            }
            // init a new CustomChildWindow
            CustomChildWindow child = new CustomChildWindow(WorldMapController, selectedCountries, selectedIndicatorPKs);
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
                Height = 20,
                Width = 50,
                Content = "Remove",                
            };

            bt.Click += new RoutedEventHandler(bt_Click);

            panel.Children.Add(bt);
            panel.Children.Add(dp);
            panel.Children.Add(tb);

            panel.DataContext = pushpin;
            pushpin.DataContext = panel;
            CountryListBox.Items.Add(panel);
        }

        void UpdateCountryPushPin(StackPanel pushpinPanel)
        {         
            DraggablePushpin pushpin = pushpinPanel.DataContext as DraggablePushpin;
            if (pushpin.country != null)
            {
                int a = pushpinPanel.Children.Count;
                TextBlock tb = pushpinPanel.Children[2] as TextBlock;
                tb.Text = pushpin.country.country_name;                
            }
            else
            {
                (PushPinPanel.Children[2] as TextBlock).Text = "Error country data";
            }             
        }

        void bt_Click(object sender, RoutedEventArgs e)
        {
            StackPanel item = VisualTreeHelper.GetParent(sender as UIElement) as StackPanel;            
            PushPinLayer.Children.Remove(item.DataContext as DraggablePushpin);
            CountryListBox.Items.Remove(item);
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
            request.Culture = MyMap.Culture;
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
                }
                else if (0 == e.Result.Results.Count)
                {
                    //Output.Text = outputString + "No results";
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
                //Output.Text = "Exception raised calling reverse geocoder";
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

            if (CountryListBox.Items.Count > 1 && selectedIndicatorPKs.Count > 0)
            {
                MessageBox.Show("You can only choose 1 indicator if 2 or more countries are choosen");
                ((CheckBox)sender).IsChecked = false;
            }
            else
            {
                selectedIndicatorPKs.Add(Convert.ToInt32(((CheckBox)sender).Tag));
            }
        }

        private void IndicatorCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            selectedIndicatorPKs.Remove(Convert.ToInt32(((CheckBox)sender).Tag));
        }
        #endregion

    }
}
