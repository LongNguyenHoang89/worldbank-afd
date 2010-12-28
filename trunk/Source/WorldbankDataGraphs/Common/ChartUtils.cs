using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Visifire.Charts;

namespace WorldbankDataGraphs.Common
{
    public class ChartUtils
    {
        /// <summary>
        /// This function reset all value on a chart to 0
        /// </summary>
        /// <param name="chartToClear"></param>
        public static void ClearChart(Chart chartToClear)
        {
            if (chartToClear != null)
            {
                foreach (DataSeries tmpDS in chartToClear.Series)
                {
                    foreach (DataPoint tmpDP in tmpDS.DataPoints)
                    {
                        tmpDP.YValue = 0;
                    }
                }
            }
        }
    }
}
