using System;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using NCRVisual.web.DataModel;

namespace WorldMap
{
    public partial class Workspace : UserControl
    {
        private List<int> _indicatorIDList;

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

        /// <summary>
        /// Default constructor
        /// </summary>
        public Workspace()
        {
            InitializeComponent();
            _indicatorIDList = new List<int>();
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

        private void SaveIndicatorButton_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
