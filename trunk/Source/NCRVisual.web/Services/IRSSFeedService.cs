using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;

namespace NCRVisual.web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRSSFeedService" in both code and config file together.
    [ServiceContract]
    public interface IRSSFeedService
    {
        [OperationContract]
        string GetRSSString(string rssUrl);
    }
}
