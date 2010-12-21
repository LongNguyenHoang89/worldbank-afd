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
    public class Country
    {
        #region private variables
        private List<YearData> years = new List<YearData>();
        #endregion

        #region getters & setters

        public string Name { get; set; }

        public List<YearData> Years
        {
            get { return years; }
            set { this.years = value; }
        }

        #endregion
    }
}
