using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NCRVisual.RelationDiagram
{
    /// <summary>
    /// Controls the in-game starfield
    /// </summary>
    public class StarField
    {
        /// <summary>
        /// number of stars on screen
        /// </summary>
        private int _starCount = 350;

        /// <summary>
        /// List of stars
        /// </summary>
        Star[] _stars;

        /// <summary>
        /// Panel containing all stars
        /// </summary>
        private Panel _panelField;

        private static Brush _starColor1 = new SolidColorBrush(Colors.White);

        protected StarField()
        {
        }

        public StarField(Panel panelField)
        {
            _panelField = panelField;
            CreateStars();
        }

        private void CreateStars()
        {
            _stars = new Star[_starCount];
            int xMax = (int) Globals.ScreenWidth;
            int yMax = (int) Globals.ScreenHeight;

            for (int i = 0; i < _starCount; i++)
            {
                Star star = new Star();
                _stars[i] = star;
                star.X = Globals.Random.Next(xMax);
                star.Y = Globals.Random.Next(yMax);
                star.Z = ((double) Globals.Random.Next(256)) / 256;
                star.Brightness = Globals.Random.Next(256);

                Ellipse it = new Ellipse();
                it.Width = (2-star.Z);
                it.Height = (2 - star.Z);
                it.Fill = _starColor1;
                it.Opacity = star.Brightness * star.Z;
                Canvas.SetLeft(it, star.X);
                Canvas.SetTop(it, star.Y);

                star.It = it;
                _panelField.Children.Add(it);
            }
        }

        /// <summary>
        /// Updates the star field
        /// </summary>
        /// <param name="msec">time since the last update</param>
        public void UpdateStars(double msec)
        {
            int xMax = (int) Globals.ScreenWidth;
            int yMax = (int) Globals.ScreenHeight;

            for (int i = 0; i < _starCount; i++)
            {
                Star star = _stars[i];
                star.X = star.X - (msec / 10) * (1.1-star.Z);
                if (star.X < 0)
                {
                    star.X = xMax;
                    star.Y = Globals.Random.Next(yMax);
                }

                Canvas.SetLeft(star.It, star.X);
            }
        }
    }
}
