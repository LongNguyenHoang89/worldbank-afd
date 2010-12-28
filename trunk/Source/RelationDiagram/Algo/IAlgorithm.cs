
using System.Windows;
using System.Collections.ObjectModel;

namespace NCRVisual.RelationDiagram.Algo
{
    /// <summary>
    /// Interface for developing LayoutAlgorithm
    /// </summary>
    public interface IAlgorithm
    {                
        Collection<Point> RunAlgo(int[][] input, int vertexNumber);
    }
}