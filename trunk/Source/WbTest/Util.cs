using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using WbWCF.Contract.Data;
using System.Collections.ObjectModel;
using WbTest;
using Excel = Microsoft.Office.Interop.Excel;
using WbWCF.DataAccess;
using System.IO;
using System.Reflection;
using System.Net;

namespace WbTest
{
    public class Utils
    {
        public static Collection<CountryEntry> GetAllCountries()
        {
            
            string url="http://api.worldbank.org/countries/all?per_page=300";
            //
            string country_tag="wb:country";
            string iso_code_tag="wb:iso2Code";
            string name_tag="wb:name";
            string region_code_tag="wb:region";
            string lending_types_code_tag="wb:lendingType";
            string income_level_code_tag="wb:incomeLevel";
            string longitude_tag="wb:longitude";
            string latitude_tag="wb:latitude";
            string common_attribute="id";            

            XmlTextReader reader = new XmlTextReader(url);
            Collection<CountryEntry> countries = new Collection<CountryEntry>();
            int count = 0;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == country_tag)
                {
                    count++;
                    CountryEntry country = new CountryEntry();
                    country.country_id_pk = count;
                    //read iso code
                    reader.ReadToFollowing(iso_code_tag);
                    country.country_iso_code=reader.ReadElementContentAsString();

                    //read name
                    reader.ReadToFollowing(name_tag);
                    country.country_name=reader.ReadElementContentAsString();

                    //read region
                    reader.ReadToFollowing(region_code_tag);
                    // read region id , save
                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        string attribute_name = reader.Name;
                        string attribute_value = reader.Value;
                        if (attribute_name.Equals(common_attribute))
                            country.region_id = attribute_value;
                    }   

                    //read income level
                    reader.ReadToFollowing(income_level_code_tag);
                    // read income level id , save
                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        string attribute_name = reader.Name;
                        string attribute_value = reader.Value;
                        if (attribute_name.Equals(common_attribute))
                            country.income_level_id = attribute_value;
                    }

                    //read lending type level
                    reader.ReadToFollowing(lending_types_code_tag);
                    // read region id , save
                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        string attribute_name = reader.Name;
                        string attribute_value = reader.Value;
                        if (attribute_name.Equals(common_attribute))
                            country.lending_type_id = attribute_value;
                    }

                    //read longtitude
                    reader.ReadToFollowing(longitude_tag);
                    if (!reader.IsEmptyElement)
                    {
                        country.country_longitude = reader.ReadElementContentAsDouble();
                        country.is_region = false;
                    }
                    else
                        country.is_region = true;
                    //read latitude
                    reader.ReadToFollowing(latitude_tag);
                    if (!reader.IsEmptyElement)
                        country.country_latitude = reader.ReadElementContentAsDouble();
                    countries.Add(country);
                }               
            }            
            return countries;
        }

        public static Collection<IndicatorEntry> GetAllIndicators()
        {

            string url = "http://api.worldbank.org/indicators?per_page=4000";
            //
            string indicator_tag = "wb:indicator";            
            string name_tag = "wb:name";
            string description_tag = "wb:sourceNote";            
            string common_attribute = "id";

            XmlTextReader reader = new XmlTextReader(url);
            Collection<IndicatorEntry> indicators = new Collection<IndicatorEntry>();
            int count = 0;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == indicator_tag)
                {
                    count++;
                    IndicatorEntry indicator = new IndicatorEntry();
                    indicator.indicator_id_pk = count;

                    //read indicator code                    
                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        string attribute_name = reader.Name;
                        string attribute_value = reader.Value;
                        if (attribute_name.Equals(common_attribute))
                            indicator.indicator_code = attribute_value;
                    }

                    //read indicator name
                    reader.ReadToFollowing(name_tag);
                    indicator.indicator_name = reader.ReadElementContentAsString();

                    //read indicator decription
                    reader.ReadToFollowing(description_tag);
                    indicator.indicator_description = reader.ReadElementContentAsString();
                    indicators.Add(indicator);
                    if (count % 10 == 0)
                        count = count;
                   
                }
            }
            return indicators;
        }

        public static Collection<CountryIndicatorEntry> GetAllIndicatorValue(IndicatorEntry indicator)
        {

            string url = "http://api.worldbank.org/countries/all/indicators/" + indicator.indicator_code + "?per_page=30000&date=1996:2009";
            //string url = "http://api.worldbank.org/countries/VN/indicators/" + indicator.indicator_code + "?per_page=30000&date=1996:2009";
            //
            string data_tag = "wb:data";
            string country_tag = "wb:country";
            string date_tag = "wb:date";
            string value_tag = "wb:value";
            string common_attribute = "id";

            XmlTextReader reader = new XmlTextReader(url);
            Collection<CountryIndicatorEntry> indicators = new Collection<CountryIndicatorEntry>();
            int count = 0;
            Dictionary<string, int> mapping = WBAccess.MapCountryIdToIsoCode();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == data_tag)
                {
                    count++;
                    CountryIndicatorEntry ref_value = new CountryIndicatorEntry();                    

                    

                    //read indicator name
                    reader.ReadToFollowing(country_tag);
                    //read indicator code                    
                    string iso_code = "";
                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        string attribute_name = reader.Name;
                        string attribute_value = reader.Value;
                        if (attribute_name.Equals(common_attribute))
                            iso_code = attribute_value;
                    }
                    if (iso_code != null && iso_code.Trim().Length > 0)
                    {
                        if (mapping.ContainsKey(iso_code))
                        {
                            ref_value.country_id = mapping[iso_code];
                        }
                        else continue;
                    }
                    else
                        continue;
                    //read ref_value year
                    reader.ReadToFollowing(date_tag);
                    ref_value.country_indicator_year = reader.ReadElementContentAsInt();

                    //read ref_value value
                    reader.ReadToFollowing(value_tag);
                    try
                    {
                        ref_value.country_indicator_value = reader.ReadElementContentAsDouble();
                    }
                    catch
                    {
                        ref_value.country_indicator_value = 0;
                    }

                    ref_value.indicator_id = indicator.indicator_id_pk;

                    indicators.Add(ref_value);                                                            
                }
            }
            return indicators;
        }

        public static Collection<RegionEntry> GetAllRegions()
        {
            string url="http://api.worldbank.org/regions";
            //
            string region_tag="wb:region";
            string code_tag="wb:code";
            string name_tag="wb:name";                     

            XmlTextReader reader = new XmlTextReader(url);
            Collection<RegionEntry> regions = new Collection<RegionEntry>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == region_tag)
                {
                    RegionEntry region = new RegionEntry();
                    //read code
                    reader.ReadToFollowing(code_tag);
                    region.region_id_pk = reader.ReadElementContentAsString();

                    //read name
                    reader.ReadToFollowing(name_tag);
                    region.region_name = reader.ReadElementContentAsString();

                    regions.Add(region);
                }
            }

            return regions;
        }

        public static Collection<LendingTypeEntry> GetAllLendingTypes()
        {
            string url = "http://api.worldbank.org/lendingTypes";
            //
            string lending_tag = "wb:lendingTypes";
            string lending_type_tag = "wb:lendingType";
            string attribute = "id";

            XmlTextReader reader = new XmlTextReader(url);
            Collection<LendingTypeEntry> lending_types = new Collection<LendingTypeEntry>();            
            while (true)
            {                
                    LendingTypeEntry lending_type = new LendingTypeEntry();
                    //read code
                    reader.ReadToFollowing(lending_type_tag);                    
                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        string attribute_name = reader.Name;
                        string attribute_value = reader.Value;
                        if (attribute_name.Equals(attribute))
                            lending_type.lending_type_id_pk = attribute_value;
                    }
                    reader.MoveToContent();
                    if (reader.EOF)
                        break;
                    lending_type.lending_type_name = reader.ReadElementContentAsString();
                    lending_types.Add(lending_type);                
            }

            return lending_types;
        }

        public static Collection<IncomeLevelEntry> GetAllIncomeLevels()
        {
            string url = "http://api.worldbank.org/incomeLevels";
            //
            string income_tag = "wb:IncomeLevels";
            string income_level_tag = "wb:incomeLevel";
            string attribute = "id";

            XmlTextReader reader = new XmlTextReader(url);
            Collection<IncomeLevelEntry> income_levels = new Collection<IncomeLevelEntry>();
            while (true)
            {                
                    IncomeLevelEntry income_level = new IncomeLevelEntry();
                    //read code
                    reader.ReadToFollowing(income_level_tag);
                    while (reader.MoveToNextAttribute()) // Read the attributes.
                    {
                        string attribute_name = reader.Name;
                        string attribute_value = reader.Value;
                        if (attribute_name.Equals(attribute))
                            income_level.income_level_id_pk = attribute_value;
                    }
                    reader.MoveToContent();
                    if (reader.EOF)
                        break;
                    income_level.income_level_name = reader.ReadElementContentAsString();
                    income_levels.Add(income_level);                
            }

            return income_levels;
        }
        

        public static Collection<CountryEntry> GetAllCountryCode()
        {
            //string file = @"C:\Users\Chris\Desktop\TestSheet.xls";
            string binPath=Directory.GetCurrentDirectory();
            binPath = Directory.GetParent(binPath).ToString();
            string file = Directory.GetParent(binPath) + @"\TradeData\Country.xls";            


            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            Excel.Range range;
            string str;
            int rCnt = 0;
            int cCnt = 0;
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(file, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            range = xlWorkSheet.UsedRange;
            Collection<CountryEntry> countries = new Collection<CountryEntry>();
            for (rCnt = 2; rCnt <= range.Rows.Count; rCnt++)
            {
                CountryEntry country = new CountryEntry();
                country.country_code=(int)(range.Cells[rCnt, 1] as Excel.Range).Value2;
                country.country_iso_code = (string)(range.Cells[rCnt, 6] as Excel.Range).Value2;
                countries.Add(country);
            }
            
            xlWorkBook.Close(true, null, null);
            xlApp.Quit();

            releaseObject(xlWorkSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            return countries;
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;                
            }
            finally
            {
                GC.Collect();
            }
        }

        private static void downloadImage(string link,string name)
        {
            WebClient webclient = new WebClient();
            webclient.DownloadFile(link, @"c:\flags\"+name);

        }
        
        public static void downloadFlag(Dictionary<string,int> mapping)
        {
            
            string url = "http://www.photius.com/flags/alphabetic_list.html";
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();

            foreach (string country_code in mapping.Keys)
            {
                string flag_position = country_code.ToLower() + "-t.gif";
                int flag_index = result.IndexOf(flag_position);
                WBAccess.UpdateCountryFlag(mapping[country_code], false);
                if (flag_index > 100)
                {
                    string string_contain_link = result.Substring(flag_index - 100, 150);
                    int begin_link = string_contain_link.IndexOf("src=\"http://");
                    if (begin_link != 1)
                    {
                        int end_link = string_contain_link.IndexOf(".gif", begin_link);
                        if (end_link > begin_link)
                        {                            
                            string link = string_contain_link.Substring(begin_link + 5, end_link - begin_link - 1);
                            string name = country_code.ToLower() + ".gif";
                            downloadImage(link, name);
                            WBAccess.UpdateCountryFlag(mapping[country_code], true);
                        }
                    }                    
                }
            }

            return;
        }
    }
}