using System.Windows;
using System.Collections.ObjectModel;
using System;


namespace NCRVisual.RelationDiagram.Algo
{
    public class KKAlgorithm:IAlgorithm
    {
        private int Factor = 1;
        private double width = 800;
        private double LIMIT = 100;
        /// <summary>
        /// Width of the bounding box.
        /// </summary>
        public double Width
        {
            get { return width; }
            set
            {
                width = value;                
            }
        }

        private double height = 800;
        /// <summary>
        /// Height of the bounding box.
        /// </summary>
        public double Height
        {
            get { return height; }
            set
            {
                height = value;                
            }
        }

        private int maxIterations = 1000;
        /// <summary>
        /// Maximum number of the iterations.
        /// </summary>
        public int MaxIterations
        {
            get { return maxIterations; }
            set
            {
                maxIterations = value;                
            }
        }

        private double _k = 1;
        public double K
        {
            get { return _k; }
            set
            {
                _k = value;                
            }
        }


        private bool adjustForGravity;
        /// <summary>
        /// If true, then after the layout process, the vertices will be moved, so the barycenter will be
        /// in the center point of the bounding box.
        /// </summary>
        public bool AdjustForGravity
        {
            get { return adjustForGravity; }
            set
            {
                adjustForGravity = value;                
            }
        }

        private bool exchangeVertices;
        public bool ExchangeVertices
        {
            get { return exchangeVertices; }
            set
            {
                exchangeVertices = value;                
            }
        }

        private double lengthFactor = 1;
        /// <summary>
        /// Multiplier of the ideal edge length. (With this parameter the user can modify the ideal edge length).
        /// </summary>
        public double LengthFactor
        {
            get { return lengthFactor; }
            set
            {
                lengthFactor = value;                
            }
        }

        private double disconnectedMultiplier = 1;
        /// <summary>
        /// Ideal distance between the disconnected points (1 is equal the ideal edge length).
        /// </summary>
        public double DisconnectedMultiplier
        {
            get { return disconnectedMultiplier; }
            set
            {
                disconnectedMultiplier = value;                
            }
        }

        const int MAX = 100;
        const double MAXDIAMETER = 1600;
        private Collection<Point> Nodes { get; set; }
        private double[,] distances;
        private double[,] edgeLengths;
        private double[,] springConstants;

        //cache for speed-up        
        /// <summary>
        /// Positions of the vertices, stored by indices.
        /// </summary>
        private Point[] positions;

        private double diameter;
        private double idealEdgeLength;
        
        public Collection<Point> RunAlgo(int[][] input, int vertexNumber)
        {
            InternalCompute(input, vertexNumber);
            Collection<Point> points = new Collection<Point>();
            UpdatePosition(vertexNumber);
            for (int i = 0; i < vertexNumber; i++)
            {
                points.Add(new Point(positions[i].X*Factor,positions[i].Y*Factor));
            }
            return points;
        }

        private void UpdatePosition(int vertexNumber)
        {
            double minx = double.MaxValue;
            double miny = double.MaxValue;
            for (int i = 0; i < vertexNumber; i++)
            {
                if (positions[i].X < minx)
                    minx = positions[i].X;
                if (positions[i].Y < miny)
                    miny = positions[i].Y;
            }

            for (int i = 0; i < vertexNumber; i++)
            {
                positions[i].X = positions[i].X - minx;
                positions[i].Y = positions[i].Y - miny;
            }

            
        }

        public void Init(int[][] input, int vertexNumber)
        {
            Nodes = new Collection<Point>();
            Random ran = new Random();
            for (int i = 0; i < vertexNumber; i++)
            {
                Nodes.Add(new Point(ran.NextDouble()*Width,ran.NextDouble()*Height));
            }            

            edgeLengths = new double[MAX, MAX]; ;
            for (int i = 0; i < vertexNumber; i++)
                for (int j = 0; j < vertexNumber; j++)
                    edgeLengths[i,j] = input[i][j];

            distances = new double[MAX, MAX];
            for (int i = 0; i < vertexNumber; i++)
                for (int j = 0; j < vertexNumber; j++)
                    if (i != j && input[i][j] > 0)
                    {
                        distances[i, j] = GetDistance(Nodes[i], Nodes[j]);
                    }
                    else
                        distances[i, j] = LIMIT;

            springConstants = new double[MAX, MAX];
        }

        private void calEdgeLength(int vertexNumber)
        {
            for (int i = 0; i < vertexNumber; i++)
                for (int j = 0; j < vertexNumber; j++)
                    edgeLengths[i, j] = GetDistance(positions[i], positions[j]);
        }
        private double GetDistance(Point p1, Point p2)
        {
            return (Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)));            
        }

        private Point AddPoint(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }


        private void InternalCompute(int[][] input, int vertexNumber)
        {
                        
            positions = new Point[MAX];
            Init(input,vertexNumber);            

            //copy positions into array (speed-up)
            int index = 0;
            foreach (Point i in Nodes)
            {
                positions[index] = i;
                index++;
            }

            //calculating the diameter of the graph
            //TODO check the diameter algorithm
            diameter = MAXDIAMETER;

            //L0 is the length of a side of the display area
            double L0 = Math.Min( Width, Height );

            //ideal length = L0 / max d_i,j
            idealEdgeLength = ( L0 / diameter ) * LengthFactor;
        

            //calculating the ideal distance between the nodes
            for ( int i = 0; i < vertexNumber - 1; i++ )
            {
                for ( int j = i + 1; j < vertexNumber; j++ )
                {
                    //distance between non-adjacent vertices
                    double dist = diameter * DisconnectedMultiplier;

                    //calculating the minimal distance between the vertices
                    if (distances[i, j] != LIMIT)
                        dist = Math.Min( distances[i, j], dist );
                    if (distances[j, i] != LIMIT)
                        dist = Math.Min( distances[j, i], dist );
                    distances[i, j] = distances[j, i] = dist;
                    edgeLengths[i, j] = edgeLengths[j, i] = idealEdgeLength * dist;
                    springConstants[i, j] = springConstants[j, i] = K / Math.Pow( dist, 2 );
                }
            }            

            int n = vertexNumber;
            if ( n == 0 )
                return;

            //TODO check this condition
            for ( int currentIteration = 0; currentIteration < MaxIterations; currentIteration++ )
            {
                
                double maxDeltaM = double.NegativeInfinity;
                int pm = -1;

                //get the 'p' with the max delta_m
                for ( int i = 0; i < n; i++ )
                {
                    double deltaM = CalculateEnergyGradient( i );
                    if ( maxDeltaM < deltaM )
                    {
                        maxDeltaM = deltaM;
                        pm = i;
                    }
                }
                //TODO is needed?
                if ( pm == -1 )
                    return;

                //calculating the delta_x & delta_y with the Newton-Raphson method
                //there is an upper-bound for the while (deltaM > epsilon) {...} cycle (100)
                for ( int i = 0; i < 100; i++ )
                {
                    positions[pm] = AddPoint(positions[pm], CalcDeltaXY( pm ));

                    double deltaM = CalculateEnergyGradient( pm );
                    //real stop condition
                    if ( deltaM < double.Epsilon )
                        break;
                }

                //what if some of the vertices would be exchanged?
                if ( ExchangeVertices && maxDeltaM < double.Epsilon )
                {
                    double energy = CalcEnergy();
                    for ( int i = 0; i < n - 1; i++ )
                    {
                        for ( int j = i + 1; j < n; j++ )
                        {
                            double xenergy = CalcEnergyIfExchanged( i, j );
                            if ( energy > xenergy )
                            {
                                Point p = positions[i];
                                positions[i] = positions[j];
                                positions[j] = p;
                                return;
                            }
                        }
                    }
                }

                //calEdgeLength(vertexNumber);
            }           
        }
      

        /// <returns>
        /// Calculates the energy of the state where 
        /// the positions of the vertex 'p' & 'q' are exchanged.
        /// </returns>
        private double CalcEnergyIfExchanged( int p, int q )
        {
            if (p > q)
                return 0;
            double energy = 0;
            for ( int i = 0; i < Nodes.Count - 1; i++ )
            {
                for ( int j = i + 1; j < Nodes.Count; j++ )
                {
                    int ii = ( i == p ) ? q : i;
                    int jj = ( j == q ) ? p : j;

                    double l_ij = edgeLengths[i, j];
                    double k_ij = springConstants[i, j];
                    double dx = positions[ii].X - positions[jj].X;
                    double dy = positions[ii].Y - positions[jj].Y;

                    energy += k_ij / 2 * ( dx * dx + dy * dy + l_ij * l_ij -
                                           2 * l_ij * Math.Sqrt( dx * dx + dy * dy ) );
                }
            }
            return energy;
        }

        /// <summary>
        /// Calculates the energy of the spring system.
        /// </summary>
        /// <returns>Returns with the energy of the spring system.</returns>
        private double CalcEnergy()
        {
            double energy = 0, dist, l_ij, k_ij, dx, dy;
            for ( int i = 0; i < Nodes.Count - 1; i++ )
            {
                for ( int j = i + 1; j < Nodes.Count; j++ )
                {
                    dist = distances[i, j];
                    l_ij = edgeLengths[i, j];
                    k_ij = springConstants[i, j];

                    dx = positions[i].X - positions[j].X;
                    dy = positions[i].Y - positions[j].Y;

                    energy += k_ij / 2 * ( dx * dx + dy * dy + l_ij * l_ij -
                                           2 * l_ij * Math.Sqrt( dx * dx + dy * dy ) );
                }
            }
            return energy;
        }

        /// <summary>
        /// Determines a step to new position of the vertex m.
        /// </summary>
        /// <returns></returns>
        private Point CalcDeltaXY( int m )
        {
            double dxm = 0, dym = 0, d2xm = 0, dxmdym = 0, dymdxm = 0, d2ym = 0;
            double l, k, dx, dy, d, ddd;

            for ( int i = 0; i < Nodes.Count; i++ )
            {
                if ( i != m )
                {
                    //common things
                    l = edgeLengths[m, i];
                    k = springConstants[m, i];
                    dx = positions[m].X - positions[i].X;
                    dy = positions[m].Y - positions[i].Y;

                    //distance between the points
                    d = Math.Sqrt( dx * dx + dy * dy );
                    ddd = Math.Pow( d, 3 );

                    dxm += k * ( 1 - l / d ) * dx;
                    dym += k * ( 1 - l / d ) * dy;
                    //TODO isn't it wrong?
                    d2xm += k * ( 1 - l * Math.Pow( dy, 2 ) / ddd );
                    //d2E_d2xm += k_mi * ( 1 - l_mi / d + l_mi * dx * dx / ddd );
                    dxmdym += k * l * dx * dy / ddd;
                    //d2E_d2ym += k_mi * ( 1 - l_mi / d + l_mi * dy * dy / ddd );
                    //TODO isn't it wrong?
                    d2ym += k * ( 1 - l * Math.Pow( dx, 2 ) / ddd );
                }
            }
            // d2E_dymdxm equals to d2E_dxmdym
            dymdxm = dxmdym;

            double denomi = d2xm * d2ym - dxmdym * dymdxm;
            double deltaX = ( dxmdym * dym - d2ym * dxm ) / denomi;
            double deltaY = ( dymdxm * dxm - d2xm * dym ) / denomi;
            return new Point( deltaX, deltaY );
        }

        /// <summary>
        /// Calculates the gradient energy of a vertex.
        /// </summary>
        /// <param name="m">The index of the vertex.</param>
        /// <returns>Calculates the gradient energy of the vertex <code>m</code>.</returns>
        private double CalculateEnergyGradient( int m )
        {
            double dxm = 0, dym = 0, dx, dy, d, common;
            //        {  1, if m < i
            // sign = { 
            //        { -1, if m > i
            for ( int i = 0; i < Nodes.Count; i++ )
            {
                if ( i == m )
                    continue;

                //differences of the positions
                dx = ( positions[m].X - positions[i].X );
                dy = ( positions[m].Y - positions[i].Y );

                //distances of the two vertex (by positions)
                d = Math.Sqrt( dx * dx + dy * dy );

                common = springConstants[m, i] * ( 1 - edgeLengths[m, i] / d );
                dxm += common * dx;
                dym += common * dy;
            }
            // delta_m = sqrt((dE/dx)^2 + (dE/dy)^2)
            return Math.Sqrt( dxm * dxm + dym * dym );
        }
    }
}