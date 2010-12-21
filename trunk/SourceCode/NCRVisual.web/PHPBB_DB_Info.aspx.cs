using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCRVisual.Web.Controllers;
using NCRVisual.Web.Items;

namespace NCRVisual.Web
{
    public partial class PHP_BB_Info : System.Web.UI.Page
    {
        private string ClientBinPath = ""; //path to store input and output data data
        private bool buttonDeactivated = false;
        private string OutputFileName;

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientBinPath = (this.Server.MapPath(this.Request.ApplicationPath).Replace("/", "\\") + "\\Output\\").Replace("\\\\","\\");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (!buttonDeactivated)
            {
                PHPBBInputController phpbb = new PHPBBInputController();
                List<ForumPost> posts = phpbb.GetPostsInPHPBBForum(TextBox1.Text, TextBox2.Text, TextBox3.Text, TextBox4.Text);
                GridView1.DataSource = posts;
                GridView1.DataBind();
                PHPBBInputController phpInputController = new PHPBBInputController();
                phpInputController.ForumPosts = posts;

                buttonDeactivated = true;

                string result = phpInputController.processAndOutputToXML(ClientBinPath);

                if (result != string.Empty)
                {
                    LabelStatus.Text = "Data retrieved successfully";
                    this.OutputFileName = result;
                }
                else
                {
                    LabelStatus.Text = "There was an error while trying to retrieve data, please check your input";
                }
                buttonDeactivated = false;
                HyperLink1.NavigateUrl = "NcrVisual.aspx?Param=php&FileName=" + this.OutputFileName;
            }
        }      
    }
}