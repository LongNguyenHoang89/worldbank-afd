using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Microsoft.Expression.Shapes;
using WorldMap.Helper;
using NCRVisual.web.DataModel;


namespace WorldMap
{
    public partial class FeedViewer : UserControl
    {
        /// <summary>
        /// get or set the Helper class of Feed viewer control
        /// </summary>
        public Controller FeedHelper { get; set; }

        public FeedViewer()
        {
            InitializeComponent();                       
            Uri uri = new System.Uri("http://vnexpress.net/rss/gl/trang-chu.rss");
            WebClient rssClient = new WebClient();
            rssClient.OpenReadCompleted += new OpenReadCompletedEventHandler(rssClient_OpenReadCompleted);

            rssClient.OpenReadAsync(uri);
        }

        public void PopulateCategory()
        {
            Collection<string> categoryList = new Collection<string>();
            Collection<int> tabId = new Collection<int>();            
            foreach (View_TabIndicator indicator in FeedHelper.Context.View_TabIndicators)
            {
                if (!tabId.Contains(indicator.tab_id_pk))
                {
                    tabId.Add(indicator.tab_id_pk);
                    categoryList.Add(indicator.tab_name);
                }                 
            }
            renderIndicatorList(categoryList);
        }

        private void rssClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XDocument doc = XDocument.Parse(e.Result.ToString()); // using Linq for XML to parse out the XML is easier than using XmlReader
            }
        }

        private void renderIndicatorList(Collection<string> indicatorList)
        {

            double size = 360 / indicatorList.Count - 5;
            foreach (string indicator in indicatorList)
            {
                //Add an Arc
                //<ed:Arc ArcThickness="1" ArcThicknessUnit="Percent" EndAngle="46" Height="220"
                //Stretch="None" Stroke="White" StartAngle="0" Width="220" Fill="#FF744040" Opacity="0.835" StrokeThickness="0"/>

                Arc arc = new Arc()
                {
                    ArcThickness = 1,
                    ArcThicknessUnit = Microsoft.Expression.Media.UnitType.Percent,
                    Height = 500,
                    Width = 500,
                    StrokeThickness = 0,
                    StartAngle = 0,
                    EndAngle = size,
                    Opacity = 0.835,
                    Fill = new SolidColorBrush(Colors.Yellow)
                };

                arc.MouseEnter += new System.Windows.Input.MouseEventHandler(arc_MouseEnter);
                arc.MouseLeave += new System.Windows.Input.MouseEventHandler(arc_MouseLeave);
                this.IndicatorsListBox.Items.Add(arc);

                //Add text along that arc
                //<TextBlock x:Name="months1_textBlock" HorizontalAlignment="Left" TextWrapping="Wrap" 
                //VerticalAlignment="Top" FontSize="15" Text="Country overview" Foreground="White"/>
                TextBlock text = new TextBlock()
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    FontSize = 10,
                    Text = indicator,
                    Foreground = new SolidColorBrush(Colors.White),
                };
                this.IndicatorTextListBox.Items.Add(text);
            }
        }

        void arc_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {            
        }

        void arc_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {            
        }
    }
}
