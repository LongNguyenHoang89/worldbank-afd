namespace RelationDiagram.Contract
{
    public class Node
    {
        public double LayoutPosX { get; set; }

        public double LayoutPosY { get; set; }

        public double LayoutForceX { get; set; }

        public double LayoutForceY { get; set; }

        public Node()
        {
            LayoutForceX = 0;
            LayoutForceY = 0;
            LayoutPosX = 0;
            LayoutPosY = 0;
        }
    }
}
