using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NCRVisual.web.DataModel;

namespace NCRVisual.web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPredictionService" in both code and config file together.
    [ServiceContract]
    public interface IPredictionService
    {
        [OperationContract]
        List<refCountryIndicator> PredictDataNextYears(List<refCountryIndicator> presentData, int yearsToPredict);
    }

    [DataContract]
    public class refCountryIndicator
    {
        [DataMember]
        public int country_id { get; set; }
        [DataMember]
        public float country_indicator_value { get; set; }
        [DataMember]
        public int country_indicator_year { get; set; }
        [DataMember]
        public int indicator_id { get; set; }
    }
}
