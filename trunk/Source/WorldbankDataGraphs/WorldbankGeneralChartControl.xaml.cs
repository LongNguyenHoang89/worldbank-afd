using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Visifire.Charts;
using WorldbankDataGraphs.Common;
using WorldbankDataGraphs.Entities;
namespace WorldbankDataGraphs
{
    public partial class WorldbankGeneralChartControl : UserControl
    {
        #region constants for this control
        #endregion

        #region public constants
        public const int RA_COLUMN = 1;
        public const string RA_COLUMN_DESC = "Column chart";
        public const int RA_LINE = 2;
        public const string RA_LINE_DESC = "Line chart";
        public const int RA_BAR = 4;
        public const string RA_BAR_DESC = "Bar chart";
        public const int RA_AREA = 8;
        public const string RA_AREA_DESC = "Area chart";
        #endregion

        #region private vars
        // normal vars
        private List<Country> countriesShown = null;
        // chart vars
        private Chart worldbankChart = null;
        private string attributeShownName = null;
        private Axis YAxis;
        private RenderAs _thisChartRenderAs = RenderAs.Column;
        private bool _renderAs3D = false;
        #endregion

        #region normal getters & setters
        public bool RenderAs3D
        {
            get
            {
                if (worldbankChart == null)
                {
                    return _renderAs3D;
                }
                else
                {
                    return worldbankChart.View3D;
                }
            }
            set
            {
                if (worldbankChart == null)
                {
                    _renderAs3D = value;
                }
                else
                {
                    worldbankChart.View3D = value;
                }
            }
        }
        public int ThisChartRenderAs
        {
            set
            {
                switch (value) {
                    case RA_COLUMN:
                        _thisChartRenderAs = RenderAs.Column;
                        break;
                    case RA_LINE:
                        _thisChartRenderAs = RenderAs.Line;
                        break;
                    case RA_BAR:
                        _thisChartRenderAs = RenderAs.Bar;
                        break;
                    case RA_AREA:
                        _thisChartRenderAs = RenderAs.Area;
                        break;
                }
            }
        }
        public string AttributeShownName
        {
            get { return attributeShownName; }
            set { attributeShownName = value; }
        }
        #endregion

        #region getters & setters with advanced function (refresh graph on data update)
        public string ChartTitle
        {
            get
            {
                if (YAxis != null)
                {
                    return YAxis.Title;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (YAxis != null)
                {
                    YAxis.Title = value;
                }
                else
                {
                    YAxis = new Axis();
                    YAxis.Title = value;
                }
            }
        }
        public List<Country> CountriesShown
        {
            get { return countriesShown; }
            set
            {
                countriesShown = value; // do the normal task of a setter
                List<int> allYears = getAllYears(countriesShown);
                // then refresh the graph
                if (worldbankChart == null) // init the chart if it's null
                {
                    worldbankChart = new Chart();
                    // set 3d
                    worldbankChart.View3D = _renderAs3D;
                    // set type of the chart
                    // set the caption of the YAxis
                    if (YAxis == null)
                    {
                        YAxis = new Axis();
                    }
                    worldbankChart.AxesY.Add(YAxis);
                    // init the data
                    bool isFirstLoop = true; // flag to know which loop is first (to set label)
                    foreach (Country tmpC in countriesShown)
                    {
                        DataSeries tmpDS = new DataSeries();
                        tmpDS.LegendText = tmpC.Name;
                        worldbankChart.Series.Add(tmpDS);
                        tmpDS.RenderAs = _thisChartRenderAs;
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
                    LayoutRoot.Children.Add(worldbankChart);
                }
                else
                {
                    ChartUtils.ClearChart(worldbankChart);
                }
                // refresh values of the chart
                if (attributeShownName != null)
                {
                    processChartValue(this.countriesShown, allYears, attributeShownName);
                }
                else
                {
                    processChartValue(this.countriesShown, allYears, Constants.GDP_KEY);
                }
            }
        }
        #endregion

        #region private funcs

        private void processChartValue(List<Country> countryList, List<int> allYears, string valueKey)
        {
            for(int j = 0; j < worldbankChart.Series.Count; j++)
            {
                DataSeries tmpDS = worldbankChart.Series[j];
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
                for (int i = 0; i < allYears.Count; i++ )
                {
                    if (found && z <= i)
                    {
                        tmpDP = tmpDS.DataPoints[i];
                        tmpDP.YValue = Convert.ToDouble(tmpCountry.Years[counter].Attributes[valueKey]);
                        counter++;
                    }
                    else
                    {
                        tmpDP.YValue = 0;
                    }
                }
            }
        }

        private void shellSortYears(List<YearData> inputList)
        {
            // ShellSort
            // h is the separation between items we compare.
            int h = 1;
            while (h < inputList.Count)
            {
                h = 3 * h + 1;
            }

            while ( h > 0 ) {
                h = ( h - 1 ) / 3;
                for (int i = h; i < inputList.Count; ++i)
                {
                    YearData item = inputList[i];
                    int j = 0;
                    for (j = i - h; j >= 0 && item.Year < inputList[j].Year; j -= h)
                    {
                        inputList[j + h] = inputList[j];
                    }// end inner for
                    inputList[j + h] = item;
                }// end outer for
            }// end while
        }

        // Get all the statistics by year of all countries shown on graph
        private List<int> getAllYears(List<Country> countryList)
        {
            int firstYear = Int32.MaxValue;
            int lastYear = Int32.MinValue;
            foreach (Country tmpC in countryList)
            {
                shellSortYears(tmpC.Years);
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

        public WorldbankGeneralChartControl()
        {
            InitializeComponent();
            //#region create dummy data
            //DummyDataGenerator dummyDataGen = new DummyDataGenerator();
            //this.CountriesShown = dummyDataGen.GenerateMultiData();
            //#endregion
        }
    }
}
