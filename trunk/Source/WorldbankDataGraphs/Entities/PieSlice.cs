
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
