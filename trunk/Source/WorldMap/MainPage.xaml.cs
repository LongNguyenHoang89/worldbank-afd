using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.Design;
using NCRVisual.web.Services;

namespace WorldMap
{
    public partial class MainPage : UserControl
    {
        #region private variables
        private LocationConverter locConverter = new LocationConverter();
        private bool _isControlpanelOpened = false;
        private DraggablePushpin _currentPushpin;        
        # endregion

        #region properties
        /// <summary>
        /// The context of services working with WB database
        /// </summary>
        public WBDomainContext Context { get; set; }

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
            MyMap.MouseClick += new EventHandler<Microsoft.Maps.MapControl.MapMouseEventArgs>(MyMap_MouseClick);
            this.ControlPanelButton.Click += new RoutedEventHandler(ControlPanelButton_Click);

            //Set default visual state
            VisualStateManager.GoToState(this, "ControlPanelClose", true);
        }
       
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Default indicators
            Context = new WBDomainContext();

            //Note: Get indicator data
            Context.Load(Context.GetTbl_indicatorsQuery());

            this.IndicatorListBox.ItemsSource = Context.tbl_indicators;

            //Default Pushpin
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

        void DefaultPushPin_Pinned(object sender, EventArgs e)
        {
            DraggablePushpin pushPin = new DraggablePushpin(PushPinLayer);            
            pushPin.IsOnMap = true;
            PushPinLayer.AddChild(pushPin, (sender as DraggablePushpin).Location);

            pushPin.Pinned += new EventHandler(MapPushpin_Pinned);
            pushPin.Clicked += new EventHandler(MapPushpin_Clicked);                       
        }

        void MapPushpin_Pinned(object sender, EventArgs e)
        {
            DraggablePushpin p = sender as DraggablePushpin;
            ReverseGeocodeLocation(p.Location);
            _currentPushpin = p;
        }

        void MapPushpin_Clicked(object sender, EventArgs e)
        {
            CustomChildWindow child = new CustomChildWindow();            
            child.Show();
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
                    Output.Text = "error geocoding ... status <" + e.Result.ResponseSummary.StatusCode.ToString() + ">";
                }
                else if (0 == e.Result.Results.Count)
                {
                    Output.Text = outputString + "No results";
                }
                else
                {                                        
                    string formatted = e.Result.Results[0].Address.CountryRegion;
                    object a = e.Result.Results[0].MatchCodes.ToString();

                    outputString = outputString + formatted;

                    //Check duplicated
                    if (found.ContainsKey(formatted))
                    {
                        Output.Text = outputString + " (duplicate)";
                    }
                    else
                    {
                        found[formatted] = true;
                        Output.Text = outputString + "  (" + e.Result.Results[0].Locations[0].CalculationMethod + ")";
                        GeocodeLayer.AddResult(e.Result.Results[0]);
                    }


                    //Add to pushpin title
                    _currentPushpin.Title = formatted;

                    ToolTipService.SetToolTip(_currentPushpin, new ToolTip()
                    {
                        DataContext = _currentPushpin,
                        Style = this.Resources["CustomInfoboxStyle"] as Style
                    });

                }
            }
            catch (Exception)
            {
                Output.Text = "Exception raised calling reverse geocoder";
            }

            // See if there are more waiting to run.
            ReverseGeocodeFromQueue();
        }

        private void MyMap_MouseClick(object sender, Microsoft.Maps.MapControl.MapMouseEventArgs e)
        {
            Location location;
            if (MyMap.TryViewportPointToLocation(e.ViewportPoint, out location))
            {
                string text = String.Format("Reverse geocoding ... Latitude={0:f6}, Longitude={1:f6}", location.Latitude, location.Longitude);
                Output.Text = text;
                // Async ... might also be queued if there are others waiting to run.
                ReverseGeocodeLocation(location);
            }
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

    }
}
