using System.Windows;
using System.Collections.ObjectModel;
using System;

namespace NCRVisual.RelationDiagram.Algo
{
    public class RectangleAlgorithm : IAlgorithm
    {

        #region IALgorithm Members
        
        public Collection<Point> RunAlgo(int[][] input, int vertexNumber)
        {
            double distance = 70;
            int vNum = (int)(Math.Ceiling(vertexNumber / 4));
            double length = distance * vNum;
            double pre;
            Collection<Point> PointPositions = new Collection<Point>();
            
            int vTemp = vNum;
            if (vertexNumber % 4 == 1)
                vTemp = vNum + 1;
            int count = 1;
            for (int i = 1; i <= vTemp; i++)
            {
                PointPositions.Add(new Point(i * distance, distance));
                count++;
            }
            pre = vTemp * distance + distance;
            vTemp = vNum;
            if (vertexNumber % 4 == 2)
                vTemp = vNum + 1;
            for (int i = 1; i <= vTemp; i++)
            {
                PointPositions.Add(new Point(pre, i * distance));
                count++;
            }

            pre = vTemp * distance + distance;
            vTemp = vNum;

            if (vertexNumber % 4 == 3)
                vTemp = vNum + 1;
            for (int i = vTemp + 1; i >= 2; i--)
            {
                PointPositions.Add(new Point(i * distance, pre));
                count++;
            }

            for (int i = vNum + 1; i >= 2; i--)
            {
                PointPositions.Add(new Point(distance, i * distance));
                count++;
            }

            return PointPositions;
        }

        #endregion
    }
}
