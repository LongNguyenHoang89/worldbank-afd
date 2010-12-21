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
using WorldbankDataGraphs.Entities;
using Visifire.Charts;
using WorldbankDataGraphs.Common;

namespace WorldbankDataGraphs
{
    public partial class WorldbankPointChartControl : UserControl
    {

        #region constants for this control
        private static string DEF_CHART_TITLE = "Point chart";
        #endregion

        #region private vars
        // normal vars
        private List<Country> countriesShown = new List<Country>();
        // chart vars
        private Chart worldbankColumnChart = null;
        #endregion

        #region normal getters & setters
        public string ChartTitle = "";
        #endregion

        #region getters & setters with advanced function (refresh graph on data update)
        public List<Country> CountriesShown
        {
            get { return countriesShown; }
            set
            {
                countriesShown = value; // do the normal task of a setter
                List<int> allYears = getAllYears(countriesShown);
                // then refresh the graph
                if (worldbankColumnChart == null) // init the chart if it's null
                {
                    worldbankColumnChart = new Chart();
                    // set type of the chart
                    // set the caption of the YAxis
                    Axis YAxis = new Axis();
                    if (ChartTitle.Equals(""))
                    {
                        YAxis.Title = DEF_CHART_TITLE;
                    }
                    else
                    {
                        YAxis.Title = ChartTitle;
                    }
                    worldbankColumnChart.AxesY.Add(YAxis);
                    // init the data
                    bool isFirstLoop = true; // flag to know which loop is first (to set label)
                    foreach (Country tmpC in countriesShown)
                    {
                        DataSeries tmpDS = new DataSeries();
                        tmpDS.RenderAs = RenderAs.Point;
                        tmpDS.LegendText = tmpC.Name;
                        worldbankColumnChart.Series.Add(tmpDS);
                        DataPoint tmpDP = null;
                        for (int i = 0; i < allYears.Count; i++)
                        {
                            tmpDP = new DataPoint();
                            tmpDP.YValue = 0;
                            if (isFirstLoop)
                            {
                                tmpDP.AxisXLabel = allYears[i].ToString();
                            }
                            tmpDP.XValue = i + 1;
                            tmpDS.DataPoints.Add(tmpDP);
                        }
                        isFirstLoop = false;
                    }
                    // Add the chart to the control
                    LayoutRoot.Children.Add(worldbankColumnChart);
                }
                else
                {
                    ChartUtils.ClearChart(worldbankColumnChart);
                }
                // refresh values of the chart
                processChartValue(this.countriesShown, allYears, Constants.GDP_KEY);
            }
        }
        #endregion

        #region private funcs

        private void processChartValue(List<Country> countryList, List<int> allYears, string valueKey)
        {
            for (int j = 0; j < worldbankColumnChart.Series.Count; j++)
            {
                DataSeries tmpDS = worldbankColumnChart.Series[j];
                Country tmpCountry = countriesShown[j];
                DataPoint tmpDP = null;
                int z = 0; // this is the year index where country begin to have statistics
                bool found = false;
                for (int l = 0; l < allYears.Count; l++)
                {
                    if (tmpCountry.Years[0].Year == allYears[l])
                    {
                        found = true;
                        z = l;
                        break;
                    }
                }
                int counter = 0;
                for (int i = 0; i < allYears.Count; i++)
                {
                    if (found && z <= i)
                    {
                        tmpDP = tmpDS.DataPoints[i];
                        tmpDP.YValue = Convert.ToInt64(tmpCountry.Years[counter].Attributes[valueKey]);
                        counter++;
                    }
                    else
                    {
                        tmpDP.YValue = 0;
                    }
                }
            }
        }
        private List<int> getAllYears(List<Country> countryList)
        {
            int firstYear = Int32.MaxValue;
            int lastYear = Int32.MinValue;
            foreach (Country tmpC in countryList)
            {
                if (tmpC.Years[0].Year < firstYear)
                {
                    firstYear = tmpC.Years[0].Year;
                }
                if (tmpC.Years[tmpC.Years.Count - 1].Year > lastYear)
                {
                    lastYear = tmpC.Years[tmpC.Years.Count - 1].Year;
                }
            }

            List<int> returnYearList = new List<int>();
            for (int i = firstYear; i < lastYear + 1; i++)
            {
                returnYearList.Add(i);
            }
            return returnYearList;
        }

        #endregion

        public WorldbankPointChartControl()
        {
            InitializeComponent();
            #region create dummy data
            DummyDataGenerator dummyDataGen = new DummyDataGenerator();
            this.CountriesShown = dummyDataGen.GenerateMultiData();
            #endregion
        }
    }
}
