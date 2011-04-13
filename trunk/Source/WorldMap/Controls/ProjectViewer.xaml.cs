using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
using NCRVisual.web.DataModel;

namespace WorldMap
{    

    public partial class ProjectViewer : UserControl
    {
        public EventHandler SaveFavoriteProject_Click;
        public ProjectViewer()
        {
            InitializeComponent();
            Loaded += (a, b) =>
            {
                LoadImages();
            };
        }

        public class Picture
        {
            public ImageSource Href { get; set; }
        }

        public void PopulateProjectData(tbl_projects project)
        {
            this.IdTxtBlck.Text = project.project_wb_id;
            this.NameTxtBlck.Text = project.project_name;
            this.LinkButton.NavigateUri = new Uri(project.project_link);
            this.StatusTxtBlck.Text = project.project_status;
            this.CostTxtBlck.Text = project.project_cost;
            this.ADateTxtBlck.Text = project.project_approval_date;
            this.CDateTxtBlck.Text = project.project_close_date;
            this.RegionTxtBlck.Text = project.project_region;
            this.BorrowerTxtBlck.Text = project.project_borrower;
            this.ImplementAgencyTxtBlck.Text = project.project_implement_agency;
            this.MajorSectorTxtBlck.Text = project.project_major_sector.Replace("\t", "").Replace("\n", " ").Replace("/n", "\n");
            this.ProjectThemesTxtBlck.Text = project.project_themes.Replace("\t", "").Replace("\n", " ").Replace("/n", "\n");
            this.OutComeTxtBlck.Text = project.project_outcome.Replace("\t", "").Replace("\n", " ").Replace("/n", "\n");
        }

        private void LoadImages()
        {
            List<Picture> coll = new List<Picture>();
            coll.Add(AddPicture("http://fc07.deviantart.net/fs70/i/2010/123/f/8/Reason_by_carrieola.jpg"));
            coll.Add(AddPicture("http://fc06.deviantart.net/fs70/i/2010/132/8/6/Me_heart_by_Skategirl.jpg"));
            coll.Add(AddPicture("http://fc01.deviantart.net/fs70/f/2010/132/4/7/Of_Day_and_Night_by_SomaKun.jpg"));
            coll.Add(AddPicture("http://fc09.deviantart.net/fs70/f/2010/132/e/0/Asya___Interface_by_MelihOG.jpg"));
            coll.Add(AddPicture("http://fc01.deviantart.net/fs70/f/2010/132/3/2/Betray_the_Oracle___NrNsNfNc__by_BassOvercast.jpg"));
            coll.Add(AddPicture("http://fc05.deviantart.net/fs70/i/2010/132/5/4/CHILI__S_BROCHURE_by_kungfuat.jpg"));
            coll.Add(AddPicture("http://fc04.deviantart.net/fs17/f/2007/202/f/a/Harry_Potter___Nostalgia_by_kurot.jpg"));
            coll.Add(AddPicture("http://fc07.deviantart.net/fs22/f/2007/337/5/7/578e0cea2cfa380e.jpg"));
            coll.Add(AddPicture("http://fc05.deviantart.net/fs71/f/2010/091/b/2/Princess_Mononoke_by_syncaidia.jpg"));
            coll.Add(AddPicture("http://fc09.deviantart.net/fs71/f/2010/062/6/b/__Naruto___Open_Your_Eyes___by_orin.jpg"));
            coll.Add(AddPicture("http://fc04.deviantart.net/fs27/f/2008/144/8/7/87002714fcfb3cad81b29e950c6fc1cd.jpg"));
            coll.Add(AddPicture("http://fc06.deviantart.net/fs51/f/2009/322/4/e/4e52300ac447c9619eb6e010d363a8ac.jpg"));
            coll.Add(AddPicture("http://fc03.deviantart.net/fs17/f/2007/176/b/a/Forgotten_Fairytales_by_zemotion.jpg"));
            coll.Add(AddPicture("http://fc00.deviantart.net/fs31/f/2008/190/e/d/rays_of_reflection_by_ssilence.jpg"));
            coll.Add(AddPicture("http://fc09.deviantart.net/fs31/i/2008/233/3/d/to_the_end_of_the_world_by_foureyes.jpg"));
            coll.Add(AddPicture("http://fc07.deviantart.net/fs31/i/2008/233/5/5/li_river_by_foureyes.jpg"));
            coll.Add(AddPicture("http://fc05.deviantart.net/fs33/f/2008/301/5/a/5a09315ee42ac66a4f6e86c69d255544.jpg"));
            lbImage.ItemsSource = coll;
        }

        private Picture AddPicture(string path)
        {
            return new Picture
            {
                Href = new BitmapImage(
                new Uri(
                    path,
                    UriKind.Absolute))
            };
        }

        private void lbImage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Preview.Source = ((Picture)lbImage.SelectedItem).Href;
        }

        public void SaveProject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFavoriteProject_Click(sender, e);
        }
    }
}
