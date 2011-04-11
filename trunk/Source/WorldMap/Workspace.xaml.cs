using System;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using NCRVisual.web.DataModel;
using WorldMap.Helper;
using Microsoft.Maps.MapControl;

namespace WorldMap
{
    public partial class Workspace : UserControl
    {
        private List<int> _indicatorIDList;
        private Controller _workspacehelper;

        /// <summary>
        /// Get or set the List of Indicators that user concerns
        /// </summary>       
        public List<int> IndicatorIDList
        {
            get
            {
                _indicatorIDList.Clear();
                foreach (AccordionItem item in IndicatorsAccordion.Items)
                {
                    foreach (Grid grid in (item.Content as StackPanel).Children)
                    {
                        CheckBox chk = grid.Children[1] as CheckBox;
                        if (chk.IsChecked == true)
                        {
                            _indicatorIDList.Add((int)chk.Tag);
                        }
                    }
                }
                return _indicatorIDList;
            }
        }

        #region EventHandler
        public event EventHandler SaveIndicatorButton_Completed;
        public event EventHandler SearchCountryByIndicators_Completed;
        public event EventHandler MapNavigation;

        /// <summary>
        /// Event after clicking view shortcut
        /// </summary>
        public event EventHandler ShorcutView;
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Workspace()
        {
            InitializeComponent();
            _indicatorIDList = new List<int>();
        }

        public NCRVisual.web.Services.WBDomainContext WBDomainContext
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// Create Controller instance for Workspace
        /// </summary>
        public void InitializeController(Controller controller)
        {
            _workspacehelper = controller;

            _workspacehelper.SearchCountryByIndicators_Completed += new EventHandler(_workspacehelper_SearchCountryByIndicators_Completed);
        }

        /// <summary>
        /// Populate the Favourited Indicator Tab
        /// </summary>
        /// <param name="IndicatorList"></param>
        public void PopulateFavouritedIndicatorsTab(EntitySet<View_TabIndicator> IndicatorList)
        {
            List<int> tabId = new List<int>();
            foreach (View_TabIndicator indicator in IndicatorList)
            {
                if (!tabId.Contains(indicator.tab_id_pk))
                {
                    tabId.Add(indicator.tab_id_pk);
                    AccordionItem item = new AccordionItem();
                    item.Header = indicator.tab_name;
                    item.DataContext = indicator.tab_id_pk;
                    item.Content = new StackPanel();
                    this.IndicatorsAccordion.Items.Add(item);
                }

                foreach (AccordionItem item in this.IndicatorsAccordion.Items)
                {
                    if (item.Header.ToString() == indicator.tab_name.ToString())
                    {
                        Grid grid = new Grid();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength() });

                        ToolTipService.SetToolTip(grid, new ToolTip()
                        {
                            Content = indicator.indicator_description,
                        });

                        TextBlock name = new TextBlock { Text = indicator.indicator_name };
                        CheckBox chk = new CheckBox();
                        chk.Tag = indicator.indicator_id_pk;

                        grid.Children.Add(name);
                        grid.Children.Add(chk);

                        Grid.SetColumn(name, 1);

                        (item.Content as StackPanel).Children.Add(grid);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Action after login in
        /// </summary>
        public void InitializeAfterLogin()
        {
            this.CountryDetailsControl.ButtonSaveShortCut.Visibility = Visibility.Visible;
            this.CountryDetailsControl.tabItem_news.Visibility = Visibility.Visible;
            this.ShortcutTab.Visibility = Visibility.Visible;
            this.SaveIndicatorButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Action after login out
        /// </summary>
        public void InitializeAfterLogout()
        {
            this.CountryDetailsControl.ButtonSaveShortCut.Visibility = Visibility.Collapsed;
            this.CountryDetailsControl.tabItem_news.Visibility = Visibility.Collapsed;
            this.SaveIndicatorButton.Visibility = Visibility.Collapsed;
            this.ShortcutTab.Visibility = Visibility.Collapsed;
        }

        private void SaveIndicatorButton_Click(object sender, RoutedEventArgs e)
        {
            SaveIndicatorButton_Completed(sender, e);
        }

        /// <summary>
        /// Load Indicator List after user login
        /// </summary>
        /// <param name="favouritedIndicatorIdPKList">List of Indicator ID PK</param>
        public void LoadIndicatorsList(List<int> favouritedIndicatorIdPKList)
        {
            foreach (AccordionItem item in IndicatorsAccordion.Items)
            {
                foreach (Grid grid in (item.Content as StackPanel).Children)
                {
                    CheckBox chk = grid.Children[1] as CheckBox;
                    if (favouritedIndicatorIdPKList.Contains((int)chk.Tag))
                    {
                        chk.IsChecked = true;
                    }
                    else chk.IsChecked = false;
                }
            }
        }

        #region Search Control

        /// <summary>
        /// Populate everything in the Search by indicators Tab
        /// </summary>
        /// <param name="IndicatorList"></param>
        public void PopulateSearchByIndicatorsTab(EntitySet<View_TabIndicator> IndicatorList)
        {
            this.IndicatorComboBox.ItemsSource = IndicatorList;

            for (int i = 1996; i <= 2009; i++)
            {
                this.YearComboBox.Items.Add(i);
            }
        }

        private void SearchCountryByIndicatorsButton_Click(object sender, RoutedEventArgs e)
        {
            int indicatorId = (this.IndicatorComboBox.SelectedItem as View_TabIndicator).indicator_id_pk;
            int year = (int)YearComboBox.SelectedItem;
            int? fromValue = null;
            int? toValue = null;

            if (!string.IsNullOrEmpty(FromValueTextBox.Text))
            {
                fromValue = int.Parse(FromValueTextBox.Text);
            }

            if (!string.IsNullOrEmpty(ToValueTextBox.Text))
            {
                toValue = int.Parse(ToValueTextBox.Text);
            }

            this._workspacehelper.SearchCountryByIndicator(indicatorId, year, fromValue, toValue);
        }

        private void _workspacehelper_SearchCountryByIndicators_Completed(object sender, EventArgs e)
        {
            if (this.SearchCountryByIndicators_Completed != null)
            {
                this.SearchCountryByIndicators_Completed(sender, e);
            }
        }

        private void SearchByIndicatorResultListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbl_countries country = (sender as ListBox).SelectedItem as tbl_countries;
            if (MapNavigation != null && country != null)
            {
                MapNavigation(country, null);
            }
        }

        /// <summary>
        /// Populate the Result box
        /// </summary>
        public void PopulateSearchByIndicatorResultBox(List<tbl_countries> result)
        {
            this.SearchByIndicatorResultListBox.ItemsSource = result;
        }

        #endregion

        #region Shortcut tab
        /// <summary>
        /// Load Shortcut list box
        /// </summary>
        /// <param name="shortcutList"></param>
        public void PopulateShortcutListbox(List<tbl_graphs> shortcutList)
        {
            this.ShortcutListBox.ItemsSource = shortcutList;
        }

        private void ViewShortcut_Click(object sender, RoutedEventArgs e)
        {
            tbl_graphs graph = ((sender as Image).Parent as Grid).DataContext as tbl_graphs;
            if (ShorcutView != null)
            {
                ShorcutView(graph, null);
            }
        }

        private void RemoveShortCut_click(object sender, RoutedEventArgs e)
        {
            //this.ShortcutListBox.Items.Remove( sender as but
        }
        #endregion

        #region Navigation Event
        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            int delta = 50;

            Point viewportPoint;
            if (map.TryLocationToViewportPoint(map.Center, out viewportPoint))
            {
                viewportPoint.Y -= delta;
                Location newCenter;
                if (map.TryViewportPointToLocation(viewportPoint, out newCenter))
                {
                    map.Center = newCenter;
                }
            }
        }

        private void MoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            int delta = 50;

            Point viewportPoint;
            if (map.TryLocationToViewportPoint(map.Center, out viewportPoint))
            {
                viewportPoint.X -= delta;
                Location newCenter;
                if (map.TryViewportPointToLocation(viewportPoint, out newCenter))
                {
                    map.Center = newCenter;
                }
            }
        }

        private void MoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            int delta = 50;

            Point viewportPoint;
            if (map.TryLocationToViewportPoint(map.Center, out viewportPoint))
            {
                viewportPoint.X += delta;
                Location newCenter;
                if (map.TryViewportPointToLocation(viewportPoint, out newCenter))
                {
                    map.Center = newCenter;
                }
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            int delta = 50;

            Point viewportPoint;
            if (map.TryLocationToViewportPoint(map.Center, out viewportPoint))
            {
                viewportPoint.Y += delta;
                Location newCenter;
                if (map.TryViewportPointToLocation(viewportPoint, out newCenter))
                {
                    map.Center = newCenter;
                }
            }
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Image)sender;
            App.Current.Host.Content.IsFullScreen = !App.Current.Host.Content.IsFullScreen;

            bool fullScreen = App.Current.Host.Content.IsFullScreen;
        }
        #endregion

        #region Map Mode
        private void RoadModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Mode = new RoadMode();
        }

        private void AerialModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Mode = new AerialMode(false);
        }

        private void AerialWithLablesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Mode = new AerialMode(true);
        }

        private void ShowPushpinLayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Children[1].Visibility = Visibility.Visible;
            map.Children[3].Visibility = Visibility.Visible;
        }

        private void ShowPushpinLayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Children[1].Visibility = Visibility.Collapsed;
            map.Children[3].Visibility = Visibility.Collapsed;
        }

        private void ShowMarkupLayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Children[2].Visibility = Visibility.Visible;
        }

        private void ShowMarkupLayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Children[2].Visibility = Visibility.Collapsed;
        }

        private void ShowTradeDataLayerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Children[0].Visibility = Visibility.Visible;
        }

        private void ShowTradeDataLayerCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.Children[0].Visibility = Visibility.Collapsed;
        }

        private void ZoomOutButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.ZoomLevel -= 1;
        }

        private void ZoomInButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Map map = (this.Parent as Grid).Children[0] as Map;
            map.ZoomLevel += 1;
        }
        #endregion
    }
}
