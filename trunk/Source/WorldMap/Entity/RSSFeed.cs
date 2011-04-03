using System;
using System.Net;
using System.Runtime.Serialization;

namespace NCRVisual.Web.Items
{
    [DataContract]
    public class RSSFeed
    {
        #region Properties
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri ArticleLink { get; set; }
        #endregion

        #region constructor
        public RSSFeed(string title, string desc, Uri articleLink)
        {
            this.Title = title;
            this.Description = desc;
            this.ArticleLink = articleLink;
        }
        public RSSFeed()
        {
        }
        #endregion

        override
        public string ToString()
        {
            return Title;
        }
    }
}
