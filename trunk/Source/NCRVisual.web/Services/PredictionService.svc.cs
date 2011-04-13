using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DemoPrediction.Web;
using System.ServiceModel.Activation;

namespace NCRVisual.web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PredictionService : IPredictionService
    {
        public List<refCountryIndicator> PredictDataNextYears(List<refCountryIndicator> presentData, int yearsToPredict)
        {
            // sort foo
            presentData = (from f in presentData
                   orderby f.country_indicator_year ascending
                           select f).ToList<refCountryIndicator>();

            // convert it to a list of decimal value
            var listValues =
                from c in presentData
                orderby c.country_indicator_year ascending
                select (decimal)c.country_indicator_value;


            List<decimal> listInput = listValues.ToList();
            List<decimal> listDiff = new List<decimal>();
            // create a list of diff between the previous year and this year
            for (int i = 1; i < listInput.Count; i++)
            {
                listDiff.Add(listInput[i] - listInput[i - 1]);
            }

            // predict the new values
            for (int i = 0; i < yearsToPredict; i++)
            {
                // predict new year value
                Prediction.DemoPrediction(ref listDiff);
                // add next year diff to last year value to create the new next year value
                refCountryIndicator temp = new refCountryIndicator();
                temp.country_id = presentData[0].country_id;
                temp.indicator_id = presentData[0].indicator_id;
                temp.country_indicator_year = presentData.Last().country_indicator_year + 1;
                temp.country_indicator_value = (float)((decimal)presentData.Last().country_indicator_value + listDiff.Last());
                //temp.country_indicator_id_pk = foo.Last().country_indicator_id_pk++;
                presentData.Add(temp);
            }

            return presentData;
        }
    }
}
