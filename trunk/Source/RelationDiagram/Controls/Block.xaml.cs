using System.Windows.Controls;

namespace NCRVisual.RelationDiagram
{
    public partial class Block : UserControl
    {       
        public Block()
        {
            InitializeComponent();
            LayoutRoot.MouseEnter += new System.Windows.Input.MouseEventHandler(LayoutRoot_MouseEnter);
            LayoutRoot.MouseLeave += new System.Windows.Input.MouseEventHandler(LayoutRoot_MouseLeave);
            LayoutRoot.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);            
        }
        

        void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MouseDownStoryBoard.Begin();
        }

        void LayoutRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {            
            this.MouseLeftStoryBoard.Begin();
        }

        void LayoutRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.MouseEnterStoryBoard.Begin();
        }
    }
}
