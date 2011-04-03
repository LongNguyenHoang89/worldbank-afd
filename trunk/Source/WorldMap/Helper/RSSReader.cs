using System;
using System.Net;
using System.Xml;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Collections.Generic;
using NCRVisual.Web.Items;
using WorldMap.RSSFeedService;
using System.Windows;

namespace NCRVisual.Web.Helper
{
    public class RSSReader
    {
        private XmlReader xmlReader;
        private SyndicationFeed feed;

        #region properties
        public int FeedLimit { get; set; }
        public List<RSSFeed> RSSFeedList { get; set; }
        #endregion

        #region private funcs
        public void ReadRSS(string rssURL)
        {
            RSSFeedServiceClient rssFeedService = new RSSFeedServiceClient();
            rssFeedService.GetRSSStringCompleted += new EventHandler<GetRSSStringCompletedEventArgs>(rssFeedService_GetRSSStringCompleted);
            rssFeedService.GetRSSStringAsync(rssURL);
            // download the xml that contain the RSS
        }

        public void ReadRSS(string rssURL, int feedLimit)
        {
            ReadRSS(rssURL);
            this.FeedLimit = feedLimit;
        }
        #endregion

        #region event handlers
        private void rssFeedService_GetRSSStringCompleted(object source, GetRSSStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                int localFeedLimit = this.FeedLimit;
                xmlReader = XmlReader.Create(new StringReader(e.Result));
                feed = SyndicationFeed.Load(xmlReader); // read xml as feed

                // count the total item from the downloaded xml
                int totalCount = 0;
                foreach (object o in feed.Items)
                {
                    totalCount++;
                }

                // if total item smaller than feed limit, it will cause out of bound error, so we need to lower the limit
                if (totalCount < localFeedLimit)
                {
                    localFeedLimit = totalCount;
                }

                this.RSSFeedList = new List<RSSFeed>();

                int i = 0;
                foreach (SyndicationItem si in feed.Items.Reverse<SyndicationItem>()) // only read as many as the limit was set
                {
                    if (si.Title != null && si.Summary != null && si.Links.Count > 0)
                    {
                        RSSFeed rssFeed = new RSSFeed(si.Title.Text, si.Summary.Text, si.Links[0].Uri);
                        this.RSSFeedList.Add(rssFeed);
                        i++;
                        if (localFeedLimit > 0 && i >= localFeedLimit) // stop if reach limit
                        {
                            break;
                        }
                    }
                    else
                    {
                        // Ignore the syndication item
                    }
                }

                RSSReadCompleted(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Error: " + e.Error.Message);
            }
        }
        #endregion

        #region event provider
        public delegate void RSSReadCompletedDelegate(object o, EventArgs e);
        public event RSSReadCompletedDelegate RSSReadCompleted;
        #endregion
    }
}
