using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NCRVisual.web.DataModel;
using System.ServiceModel.DomainServices.Client;

namespace WorldMap
{
    public partial class Workspace : UserControl
    {
        public Workspace()
        {
            InitializeComponent();
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

                        //chk.Checked += new RoutedEventHandler(IndicatorCheckbox_Checked);
                        //chk.Unchecked += new RoutedEventHandler(IndicatorCheckbox_Unchecked);

                        grid.Children.Add(name);
                        grid.Children.Add(chk);

                        Grid.SetColumn(name, 1);

                        (item.Content as StackPanel).Children.Add(grid);
                        break;
                    }
                }
            }
        }
    }
}
