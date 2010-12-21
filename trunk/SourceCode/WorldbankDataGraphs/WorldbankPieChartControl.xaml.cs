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
using Visifire.Charts;
using WorldbankDataGraphs.Entities;
using WorldbankDataGraphs.Common;

namespace WorldbankDataGraphs
{
    public partial class WorldbankPieChartControl : UserControl
    {
        #region constants
        private static string DEF_TITLE = "Worldbank pie chart";
        #endregion

        #region private vars
        // normal vars
        private List<PieSlice> pieSliceList= new List<PieSlice>();
        // graph var
        Chart worldbankPieChart = null;
        #endregion

        #region private functions
        private void processChartValue(List<PieSlice> pieSliceList)
        {
            // init the data
            DataSeries tmpDS = new DataSeries();
            tmpDS.RenderAs = RenderAs.Pie; // set the type of chart
            int seriesCount = worldbankPieChart.Series.Count;
            // remove all series
            for (int i = 0; i < seriesCount; i++ )
            {
                worldbankPieChart.Series.RemoveAt(0);
            }
            worldbankPieChart.Series.Add(tmpDS);
            DataPoint tmpDP = null;
            for (int i = 0; i < pieSliceList.Count; i++)
            {
                tmpDP = new DataPoint();
                tmpDP.YValue = pieSliceList[i].Value;
                tmpDP.AxisXLabel = pieSliceList[i].Name;
                tmpDS.DataPoints.Add(tmpDP);
            }
        }
        #endregion

        #region normal setters & getters
        #endregion

        #region getters & setters with advance functions (refresh graph when set)
        public List<PieSlice> PieSliceList
        {
            get { return pieSliceList; }
            set
            {
                pieSliceList = value; // do the normal task of a setter
                // then refresh the graph
                if (worldbankPieChart == null) // init the chart if it's null
                {
                    worldbankPieChart = new Chart();
                    // set the caption of the YAxis
                    Axis YAxis = new Axis();
                    if (ChartTitle.Equals(""))
                    {
                        ChartTitle = DEF_TITLE;
                    }
                    worldbankPieChart.AxesY.Add(YAxis);
                    // Add the chart to the control
                    LayoutRoot.Children.Add(worldbankPieChart);
                }
                else
                {
                    ChartUtils.ClearChart(worldbankPieChart);
                }
                // refresh values of the chart (or set if this is the 1st time)
                processChartValue(this.pieSliceList);
            }
        }

        public string ChartTitle
        {
            get  
            {
                if (worldbankPieChart != null && worldbankPieChart.Titles.Count > 0) {
                    return worldbankPieChart.Titles[0].Text;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (worldbankPieChart != null)
                {
                    int titleCount = worldbankPieChart.Titles.Count;
                    for (int i = 0; i < titleCount; i++)
                    {
                        worldbankPieChart.Titles.RemoveAt(0);
                    }
                    Title newTitle = new Title();
                    newTitle.Text = value;
                    worldbankPieChart.Titles.Add(newTitle);
                }
            }
        }
        #endregion

        public WorldbankPieChartControl()
        {
            InitializeComponent();
            #region debug data
            DummyDataGenerator dataGen = new DummyDataGenerator();
            this.PieSliceList = dataGen.GenerateDataForPieChart();
            #endregion
        }
    }
}