using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maps.MapControl;

namespace WorldMap.Helper
{
    /// <summary>
    /// Convert a geometry from WKT (Well-Known-Text) to Silverlight Geomatry Point
    /// 
    /// Modify source from
    /// http://forums.silverlight.net/forums/p/187451/461613.aspx
    /// </summary>
    public class WKTToGeomatryPoint
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="wkt"></param>
        public WKTToGeomatryPoint()
        {            
        }

        public object GetObject(string WKT)
        {
            switch (WKT.Substring(0, 7))
            {
                case "POLYGON":
                    return new object[2] { GetPOLYGON(WKT), "Polygon" };
                    
            }
            switch (WKT.Substring(0, 12))
            {
                case "MULTIPOLYGON":
                    return new object[2] { GetMultiPOLYGON(WKT), "MultiPolygon" };
            }
            return null;
        }
     
        private List<LocationCollection> GetMultiPOLYGON(string WKT)
        {
            List<LocationCollection> ColData = new List<LocationCollection>();            
            string WKTs = WKT.Substring((WKT.Length - (WKT.Length - 14)), (WKT.Length - 15));

            char[] Spliter = new char[3];

            // Spliter[0] = ' ';
            Spliter[1] = '('; Spliter[2] = ')';
            string[] data = WKTs.Split(Spliter);
            for (int j = 0; j < data.Length; j++)
            {                
                if (data[j] != " " && data[j] != "" && data[j] != ", ")
                {
                    LocationCollection cn = new LocationCollection();
                    char[] subSpliter = new char[2];

                    subSpliter[0] = ' ';
                    subSpliter[1] = ',';

                    // subSpliter[1] = ' ';
                    List<string> StringPoint = new List<string>(data[j].Split(subSpliter));

                    var remover = from c in StringPoint

                                  where c == "" || c == " " || c == ","
                                  select c; foreach (string item in remover.ToList())
                    {

                        StringPoint.Remove(item);

                    }
                    int count = (StringPoint.Count / 2);

                    int a = 0;
                    for (int i = 0; i < count; i++)
                    {
                        cn.Add(new Location(Convert.ToDouble(StringPoint[i + 1 + a].Remove(StringPoint[i + 1 + a].Length - 1)),Convert.ToDouble(StringPoint[i + a])));
                        a++;
                    }
                    ColData.Add(cn);
                }               
            }
            return ColData;
        }

        private LocationCollection GetPOLYGON(string WKT)
        {
            LocationCollection cn = new LocationCollection();
            string WKTs = WKT.Substring((WKT.Length - (WKT.Length - 9)), (WKT.Length - 9));
            char[] Spliter = new char[3];
            Spliter[0] = ' ';
            Spliter[1] = '('; Spliter[2] = ')';
            string[] data = WKTs.Split(Spliter);
            List<string> StringPoint = new List<string>(data);
            var remover = from c in StringPoint

                          where c == "" || c == " " || c == ","
                          select c; foreach (string item in remover.ToList())
            {
                StringPoint.Remove(item);
            }
            int count = (StringPoint.Count / 2);

            int a = 0;
            for (int i = 0; i < count; i++)
            {
                cn.Add(new Location(Convert.ToDouble(StringPoint[i + 1 + a].Remove(StringPoint[i + 1 + a].Length - 1)),Convert.ToDouble(StringPoint[i + a])));

                a++;

            }
            return cn;

        }
    }
}