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

        public static void crawlOneProject(string link,string countryid)
        {
            try
            {
                string url = link;
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                ProjectEntry entry = new ProjectEntry();
                entry.country_id = Int32.Parse(countryid);
                entry.project_link = url;
                // get project_name
                int td_name_tag_pos = result.IndexOf("<td class\"AWheading\">");
                int begin_name_tag_pos = result.IndexOf("<h1>", td_name_tag_pos + 1);
                int end_name_tag_pos = result.IndexOf("</h1>", begin_name_tag_pos + 1);
                string project_name = result.Substring(begin_name_tag_pos + 4, end_name_tag_pos - begin_name_tag_pos - 4);
                entry.project_name = project_name;

                //get project_wb_id and project_status
                int tr_id_tag_pos = result.IndexOf("tabularyellowbg");
                int strong_end_tag_pos = result.IndexOf("</strong>", tr_id_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                int end_project_id = result.IndexOf("|", strong_end_tag_pos + 1);
                string project_wb_id = result.Substring(strong_end_tag_pos + 9, end_project_id - strong_end_tag_pos - 10);
                entry.project_wb_id = project_wb_id;

                //get project_wb_status
                strong_end_tag_pos = result.IndexOf("</strong>", end_project_id + 1, StringComparison.CurrentCultureIgnoreCase);
                int td_end_tag_pos = result.IndexOf("</td>", strong_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string project_wb_status = result.Substring(strong_end_tag_pos + 9, td_end_tag_pos - strong_end_tag_pos - 9);
                entry.project_status = project_wb_status;

                //get project_approval date
                int approval_date_tag = result.IndexOf("Approval Date</a>", td_end_tag_pos, StringComparison.CurrentCultureIgnoreCase);
                int acroynym_tag = result.IndexOf("acronym", approval_date_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                int acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                int acronym_end_tag_pos = result.IndexOf("</acronym>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string approval_date = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_approval_date = approval_date;

                //get closing_date
                int closing_date_tag = result.IndexOf("Closing Date</a>", acronym_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                acroynym_tag = result.IndexOf("acronym", closing_date_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1);
                acronym_end_tag_pos = result.IndexOf("</acronym>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string closing_date = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_closing_date = closing_date;

                //get project_cost
                int project_cost_tag = result.IndexOf("Total Project Cost**</a>", acronym_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                acroynym_tag = result.IndexOf("class=\"Txt_AW\"", project_cost_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1);
                acronym_end_tag_pos = result.IndexOf("</td>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string project_cost = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_cost = project_cost;

                //get project region
                int region_tag = result.IndexOf("Region</a>", acronym_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                acroynym_tag = result.IndexOf("class=\"Txt_AW\"", region_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1);
                acronym_end_tag_pos = result.IndexOf("</td>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string region = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_region = region;

                //get project major sector
                int major_sector_tag = result.IndexOf("Major Sector (Sector) (%)</a>", acronym_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                acroynym_tag = result.IndexOf("class=\"Txt_AW\"", major_sector_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1);
                acronym_end_tag_pos = result.IndexOf("</td>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string major_sector = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_major_sector = major_sector;

                //get project_themes 
                int theme_tag = result.IndexOf("Themes (%)</a>", acronym_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                acroynym_tag = result.IndexOf("class=\"Txt_AW\"", theme_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1);
                acronym_end_tag_pos = result.IndexOf("</td>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string theme = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_themes = theme;

                //get project_borrower
                int borrower_tag = result.IndexOf("Borrower/Recipient</a>", acronym_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                acroynym_tag = result.IndexOf("class=\"Txt_AW\"", borrower_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1);
                acronym_end_tag_pos = result.IndexOf("</td>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string borrower = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_borrower = borrower;

                //get project implementation agency
                int implement_agency_tag = result.IndexOf("Implementing Agency</a>", acronym_end_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                acroynym_tag = result.IndexOf("class=\"Txt_AW\"", implement_agency_tag + 1, StringComparison.CurrentCultureIgnoreCase);
                acronym_close_tag_pos = result.IndexOf(">", acroynym_tag + 1);
                acronym_end_tag_pos = result.IndexOf("</td>", acronym_close_tag_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                string implement_agency = result.Substring(acronym_close_tag_pos + 1, acronym_end_tag_pos - acronym_close_tag_pos - 1);
                entry.project_implement_agency = implement_agency;

                //get outcome
                int outcome_link = result.IndexOf("&Type=Implementation", 9, StringComparison.CurrentCultureIgnoreCase);
                int outcome_href = result.IndexOf("href", outcome_link - 200);
                int outcome_end_link = result.IndexOf("\"", outcome_link);
                string outcome_string = result.Substring(outcome_href + 6, outcome_end_link - outcome_href - 6);

                url = outcome_string;
                myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                myResponse = myRequest.GetResponse();
                sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                result = sr.ReadToEnd();

                //get thematic outcome
                int first_tr_pos = result.IndexOf("Targeted Thematic Outcomes...");
                int first_tr_end_pos = result.IndexOf("</tr>", first_tr_pos, StringComparison.CurrentCultureIgnoreCase);
                int tbody_end_pos = result.IndexOf("</table>", first_tr_end_pos, StringComparison.CurrentCultureIgnoreCase);
                int end_tr_pos = first_tr_end_pos;
                string outcome = "";
                bool ok = true;
                do
                {
                    int value_tr_pos = result.IndexOf("<tr", end_tr_pos + 1, StringComparison.CurrentCultureIgnoreCase);
                    int value_td_pos = result.IndexOf("<td", value_tr_pos, StringComparison.CurrentCultureIgnoreCase);
                    int value_td_end_pos = result.IndexOf(">", value_td_pos + 1);
                    int value_td_close_pos = result.IndexOf("</td", value_td_end_pos, StringComparison.CurrentCultureIgnoreCase);
                    if (value_td_close_pos < tbody_end_pos)
                    {
                        string value = result.Substring(value_td_end_pos + 1, value_td_close_pos - value_td_end_pos - 1);
                        outcome = outcome + value + " /n";
                    }
                    end_tr_pos = result.IndexOf("</tr>", value_td_close_pos + 1, StringComparison.CurrentCultureIgnoreCase);

                    ok = (value_tr_pos > 0) && (value_td_pos > 0) && (value_td_end_pos > 0) && (value_td_close_pos > 0);
                }
                while (end_tr_pos < tbody_end_pos && end_tr_pos > first_tr_pos && ok);

                entry.project_outcome = outcome;
                WBAccess.InsertProject(entry);
            }
            catch
            {
            }
            // get millenium goals
            //first_tr_pos = result.IndexOf("Millennium Development Goals for This Project");
            //first_tr_end_pos = result.IndexOf("</tr>", first_tr_pos, StringComparison.CurrentCultureIgnoreCase);
            //tbody_end_pos = result.IndexOf("</table>", first_tr_end_pos, StringComparison.CurrentCultureIgnoreCase);
            //end_tr_pos = first_tr_end_pos;

            //string goals = "";
            //do
            //{
            //    int value_tr_pos = result.IndexOf("<tr", end_tr_pos + 1, StringComparison.CurrentCultureIgnoreCase);
            //    int value_td_pos = result.IndexOf("<td", value_tr_pos, StringComparison.CurrentCultureIgnoreCase);
            //    //check if have javascript
            //    int java_script = result.IndexOf("</script>", value_td_pos, StringComparison.CurrentCultureIgnoreCase);
            //    if (java_script < tbody_end_pos && java_script > value_td_pos)
            //        value_td_pos = java_script;

            //    int value_td_end_pos = result.IndexOf(">", value_td_pos + 1);                
            //    int value_td_close_pos = result.IndexOf("</td", value_td_end_pos, StringComparison.CurrentCultureIgnoreCase);
            //    string value = result.Substring(value_td_end_pos + 1, value_td_close_pos - value_td_end_pos - 1);
            //    goals = goals + value + " /n";
            //    end_tr_pos = result.IndexOf("</tr>", value_td_close_pos + 1, StringComparison.CurrentCultureIgnoreCase);
            //    ok = (value_tr_pos > 0) && (value_td_pos > 0) && (value_td_end_pos > 0) && (value_td_close_pos > 0);
            //}
            //while (end_tr_pos < tbody_end_pos && end_tr_pos > first_tr_pos);

            //entry.project_millenium_outcome = goals;

        }

        public static void crawlAllCountryProject(string link,string countryid)
        {
            try
            {
                string url = link;
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                // get total page
                int class_pos = result.IndexOf("AWProjectsContractNormalText");
                if (class_pos == -1)
                    return;
                int strong_pos = result.IndexOf("<strong>", class_pos);
                strong_pos = result.IndexOf("<strong>", strong_pos + 1);
                int end_strong_pos = result.IndexOf("</strong>", strong_pos);

                int total_page = Int16.Parse(result.Substring(strong_pos + 8, end_strong_pos - strong_pos - 8));

                //get page 1 project
                //thead1 
                int end = 0;
                bool ok = true;
                do
                {
                    int first_thead1 = result.IndexOf("Thead1", end,StringComparison.CurrentCultureIgnoreCase);
                    //get project link :
                    int link_class = result.IndexOf("AWcontentSmallLink", first_thead1 + 1);
                    int link_href = result.IndexOf("href", link_class + 1);
                    int link_end = result.IndexOf("\"", link_href + 6);
                    string link_project = result.Substring(link_href + 6, link_end - link_href - 6);
                    end = link_end + 1;
                    link_project = link_project.Replace("amp;", "");
                    crawlOneProject(link_project, countryid);
                    ok = (first_thead1 > 0) && (link_class > 0) && (link_href > 0) && (link_end > 0);
                }
                while (ok);
            }
            catch
            {
            }
        }

        public static void crawlAllCountryProjectLinks(string country_id, string country_iso_code)
        {
            try
            {
                string url = "http://web.worldbank.org/WBSITE/EXTERNAL/PROJECTS/0,,category:country~menuPK:51559~pagePK:221246~piPK:95913~theSitePK:40941,00.html";
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();

                //get link 
                int country_link_pos = result.IndexOf("countrycode=" + country_iso_code, StringComparison.CurrentCultureIgnoreCase);
                if (country_link_pos <= 0) return;
                int country_begin_link_pos = result.IndexOf("href", country_link_pos - 200, StringComparison.CurrentCultureIgnoreCase);
                int country_end_link_pos = result.IndexOf("\"", country_link_pos);
                string link = result.Substring(country_begin_link_pos + 6, country_end_link_pos - country_begin_link_pos - 6) + "&sortby=PROJECTID&sortorder=DESC";
                link = link.Replace("amp;", "");
                crawlAllCountryProject(link, country_id);
            }
            catch
            {
            }
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