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
using System.Collections.Generic;

namespace WorldbankDataGraphs.Entities
{
    public class YearData
    {
        #region getters and setters
        public int Year { get; set; }
        public Dictionary<string, string> Attributes
        {
            get { return attributes; }
            set { this.attributes = value; }
        }
        #endregion

        #region private vars
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        #endregion
    }
}
