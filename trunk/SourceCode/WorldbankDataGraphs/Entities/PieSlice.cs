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

namespace WorldbankDataGraphs.Entities
{
    public class PieSlice
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public PieSlice(string sliceName, double sliceValue)
        {
            Name = sliceName;
            Value = sliceValue;
        }
    }
}
