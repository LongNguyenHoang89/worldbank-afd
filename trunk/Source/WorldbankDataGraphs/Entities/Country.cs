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
