using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;
using System.ServiceModel.Activation;

namespace NCRVisual.web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RSSFeedService : IRSSFeedService
    {
        public string GetRSSString(string rssUrl)
        {
            WebClient client = new WebClient();
            return client.DownloadString(new Uri(rssUrl));
        }
    }
}
