using System.Windows;
using System.Collections.ObjectModel;
using System;
using RelationDiagram.Contract;

namespace NCRVisual.RelationDiagram.Algo
{
    public class FruchtermanAlgorithm:IAlgorithm
    {        
        const int maxRepulsiveForceDistance=6;
        const int iterations = 500;
        const double k = 0.2;
        const double c = 0.002;
        const double maxVertexMovement = 1;
        const int Factorx=400;
        const int Factory=400;
        private Collection<Node> Nodes { get; set; }
        private double[][] Weight = new double [100][] ;
        Point upperlimit;
        Point lowerlimit;
        public Collection<Point> RunAlgo(int[][] input, int vertexNumber)
        {
            Layout(input, vertexNumber);
            Collection<Point> Points = new Collection<Point>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                Points.Add(GetPoint(Nodes[i]));
            }
            return Points;
        }

        private Point GetPoint(Node node)
        {
            Point temp = new Point();
            temp.X = (node.LayoutPosX - lowerlimit.X)*Factorx;
            temp.Y = (node.LayoutPosY - lowerlimit.Y)*Factory;
            return temp;
        }

        public void Layout(int[][] input, int vertexNumber)
        {
            Init(input,vertexNumber);

            for (int i = 0; i < iterations; i++)
            {
                LayoutIteration(input);
            }

            LayoutCalcBounds();
        }

        public void Init(int[][] input,int vertexNumber)
        {
            Nodes = new Collection<Node>();
            for (int i = 0; i < vertexNumber; i++)
            {
                Nodes.Add(new Node());
            }

            for (int i = 0; i < vertexNumber; i++)
            {
                Weight[i] = new double[100];
                for (int j = 0; j < vertexNumber; j++)
                    if (input[i][j] > 0)
                    {
                        Weight[i][j] = 1;
                    }
            }
        }

        public void LayoutCalcBounds()
        {
            double minx = 0;
            double maxx = 0;
            double miny = 0;
            double maxy = 0;

            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].LayoutPosX > maxx)
                    maxx = Nodes[i].LayoutPosX;
                if (Nodes[i].LayoutPosX < minx)
                    minx = Nodes[i].LayoutPosX;
                if (Nodes[i].LayoutPosY > maxy)
                    maxy = Nodes[i].LayoutPosY;
                if (Nodes[i].LayoutPosY < miny)
                    miny = Nodes[i].LayoutPosY;
            }

            upperlimit = new Point(maxx, maxy);
            lowerlimit = new Point(minx, miny);
        }

        public void LayoutIteration(int[][] input)
        {
            // Forces on nodes due to node-node repulsions
            for (int i = 0; i < Nodes.Count; i++)
            {
                for (int j = i + 1; j < Nodes.Count; j++)
                    LayoutRepulsive(Nodes[i], Nodes[j]);                
            }
            // Forces on nodes due to edge attractions
            for (int i = 0; i < Nodes.Count; i++)
                for (int j = 0; j < Nodes.Count; j++)
                    if (i!=j)
                        if (input[i][j] > 0)
                        {
                            LayoutAttractive(Nodes[i],Nodes[j],ref Weight[i][j]);
                        }           

            // Move by the given force
            for (var i = 0; i < Nodes.Count; i++)
            {              
                double xmove = c * Nodes[i].LayoutForceX;
                double ymove = c * Nodes[i].LayoutForceY;

                double max = maxVertexMovement;
                if (xmove > max) xmove = max;
                if (xmove < -max) xmove = -max;
                if (ymove > max) ymove = max;
                if (ymove < -max) ymove = -max;

                Nodes[i].LayoutPosX += xmove;
                Nodes[i].LayoutPosY += ymove;
                Nodes[i].LayoutForceX = 0;
                Nodes[i].LayoutForceY = 0;
            }
        }

        public void LayoutRepulsive( Node node1,  Node node2)
        {
            double dx = node2.LayoutPosX - node1.LayoutPosX;
            double dy = node2.LayoutPosY - node1.LayoutPosY;
            double d2 = dx * dx + dy * dy;
            if (d2 < 0.01)
            {
                Random r1 = new Random();
                dx = 0.1 * r1.NextDouble() + 0.1;
                dy = 0.1 * r1.NextDouble() + 0.1;
                d2 = dx * dx + dy * dy;
            }
            var d = Math.Sqrt(d2);
            if (d < maxRepulsiveForceDistance)
            {
                var repulsiveForce = k * k / d;
                node2.LayoutForceX += repulsiveForce * dx / d;
                node2.LayoutForceY += repulsiveForce * dy / d;
                node1.LayoutForceX -= repulsiveForce * dx / d;
                node1.LayoutForceY -= repulsiveForce * dy / d;
            }
        }

        public void LayoutAttractive( Node node1, Node node2, ref double weight)
        {

                double dx = node2.LayoutPosX - node1.LayoutPosX;
                double dy = node2.LayoutPosY - node1.LayoutPosY;
                double d2 = dx * dx + dy * dy;
                if(d2 < 0.01) {
                        Random rand = new Random(); 
                        dx = 0.1 * rand.NextDouble() + 0.1;
                        dy = 0.1 * rand.NextDouble() + 0.1;
                        d2 = dx * dx + dy * dy;
                }
                double d = Math.Sqrt(d2);
                if(d > maxRepulsiveForceDistance) {
                        d = maxRepulsiveForceDistance;
                        d2 = d * d;
                }
                var attractiveForce = (d2 - k * k) / k;
                if (weight == 0 ||weight < 1) weight = 1;
                attractiveForce *= Math.Log(weight) * 0.5 + 1;
               
                node2.LayoutForceX -= attractiveForce * dx / d;
                node2.LayoutForceY -= attractiveForce * dy / d;
                node1.LayoutForceX += attractiveForce * dx / d;
                node1.LayoutForceY += attractiveForce * dy / d;
        }
    }
}
