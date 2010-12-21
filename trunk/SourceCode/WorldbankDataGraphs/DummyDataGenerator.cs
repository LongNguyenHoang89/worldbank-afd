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
using WorldbankDataGraphs.Entities;
using System.Collections.Generic;

namespace WorldbankDataGraphs
{
    public class DummyDataGenerator
    {
        Random random = new Random();

        public List<Country> GenerateMultiData()
        {
            List<Country> returnList = new List<Country>();
            returnList.Add(GenerateData("Vietnam"));
            returnList.Add(GenerateData("USA"));
            returnList.Add(GenerateData("Japan"));
            return returnList;
        }

        public Country GenerateData(string countryName)
        {
            Country country = new Country();

            country.Name = countryName;
            country.Years = GenerateYearData();

            return country;
        }

        private List<YearData> GenerateYearData()
        {
            YearData yearData = null;
            List<YearData> tmpList = new List<YearData>();
            for (int i = 1980; i < 2009; i++)
            {
                yearData = new YearData();
                yearData.Year = i;
                yearData.Attributes.Add(Constants.GDP_KEY, (random.Next(1, 1000) * 10000000000).ToString());
                yearData.Attributes.Add(Constants.EXPORT_KEY, random.Next(30, 100) + "");
                yearData.Attributes.Add(Constants.IMPORT_KEY, random.Next(30, 100) + "");
                tmpList.Add(yearData);
            }
            return tmpList;
        }

        public List<PieSlice> GenerateDataForPieChart()
        {
            List<PieSlice> pieSliceList = new List<PieSlice>();
            for (int i = 0; i < 6; i++)
            {
                pieSliceList.Add(new PieSlice(((char)('A' + i)) + "", random.Next(0, 100)));
            }
            return pieSliceList;
        }
    }
}
