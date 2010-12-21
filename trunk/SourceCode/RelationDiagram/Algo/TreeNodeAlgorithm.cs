using System.Windows;
using System.Collections.ObjectModel;

namespace NCRVisual.RelationDiagram.Algo
{
    public class TreeNodeAlgorithm : IAlgorithm
    {       
        #region IALgorithm Members

        public Collection<Point> RunAlgo(int[][] input, int vertexNumber)
        {
            int[] save = new int[100];
            Collection<Point> PointPositions = new Collection<Point>();
            for (int i = 0; i < vertexNumber; i++)
            {
                save[i] = 0;
            }
            for (int i = 0; i < vertexNumber; i++)
            {
                int count = 0;
                for (int j = 0; j < vertexNumber; j++)
                    if (input[i][j] > 0)
                        count++;
                PointPositions.Add(new Point(save[count] *250, count * 50));
                save[count]++;
            }

            return PointPositions;
        }

        #endregion
    }
}
