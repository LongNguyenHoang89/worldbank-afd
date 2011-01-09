using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using WbWCF;
using System.Collections.ObjectModel;
using WbWCF.Contract.Data;
using WbWCF.DataAccess;

namespace WbTest
{
    class Program
    {
        const int UPDATED_YEAR=2009;
        const int BEGIN_YEAR = 1996;
        static void Main(string[] args)
        {
            //GetStaticDataFromWorldBank();
            //GetTradeData();
            //GetAllIndicatorDataFromWorldBank();
            GetIndicatorsValue(); //Get indicators 's values which have is_gotten=0
            //GetAllFlag();
            Console.WriteLine("Finish all data");
            Console.ReadKey();
        }

        static void GetAllFlag()
        {
            Dictionary<string, int> mapping = WBAccess.MapCountryIdToIsoCode();
            Utils.downloadFlag(mapping);            
        }

        static void GetAllIndicatorDataFromWorldBank()
        {
            Collection<IndicatorEntry> indicators= Utils.GetAllIndicators();            
            WBAccess.InsertIndicators(indicators);
        }

        private static string GetUnit(string indicator_name)
        {
            string result = "";
            if (indicator_name[indicator_name.Length - 1] != ')')
                return "";
            for (int i = indicator_name.Length - 1; i > 0; i--)
            {
                if (indicator_name[i] != '(')
                {
                    result = indicator_name[i] + result;
                }
                else
                {
                    result = '(' + result;
                    break;
                }
            }
            return result;
        }

        static void GetIndicatorsValue()
        {
            Collection<IndicatorEntry> indicators=WBAccess.GetSelectedIndicator();
            for (int i = 0; i < indicators.Count; i++)
            {
                indicators[i].indicator_unit = GetUnit(indicators[i].indicator_name);
                Collection<CountryIndicatorEntry> entries = Utils.GetAllIndicatorValue(indicators[i]);
                WBAccess.InsertIndicatorValue(entries);
                WBAccess.UpdateIndicator(indicators[i]);
                Console.WriteLine("finish indicator:" + indicators[i].indicator_name);
            }
        }

        static void GetStaticDataFromWorldBank()
        {
            Console.WriteLine("Downloading data. Please wait... ");
            //delete all statistic data in db
            WBAccess.DeleteAllStaticData();
            Collection<CountryEntry> countries = Utils.GetAllCountries();          
            WBAccess.InsertCountries(countries);
            Console.WriteLine("Download countries complete.");
            Collection<RegionEntry> regions = Utils.GetAllRegions();
            WBAccess.InsertRegions(regions);
            Console.WriteLine("Download reions complete.");
            Collection<LendingTypeEntry> lending_types = Utils.GetAllLendingTypes();
            WBAccess.InsertLendingTypes(lending_types);
            Console.WriteLine("Download lending types complete.");
            Collection<IncomeLevelEntry> income_levels = Utils.GetAllIncomeLevels();
            WBAccess.InsertIncomeLevels(income_levels);
            Console.WriteLine("Download income levels complete.");
        }

        static void GetTradeData()
        {
            //update country data
            Collection<CountryEntry> countries = new Collection<CountryEntry>();
            countries = Utils.GetAllCountryCode();
            WBAccess.UpdateCountry(countries);
            Console.WriteLine("Update country code complete.");
            //update trade relation            
            Dictionary<int, int> mapping = WBAccess.MapCountryIdToCode();
            long total_records = 0;
            for (int i = 1996; i < UPDATED_YEAR; i++)
            {                
                string binPath = Directory.GetCurrentDirectory();
                binPath = Directory.GetParent(binPath).ToString();
                string file_name = Directory.GetParent(binPath) + @"\TradeData\comtrade_trade_data_" + i + ".csv";
                StreamReader sr = new StreamReader(@file_name);
                
                string strline = "";
                string[] _values = null;
                int x = 0;
                while (!sr.EndOfStream)
                {                    
                    x++;                    
                    strline = sr.ReadLine();
                    _values = strline.Split(';');
                    if (_values != null&&_values[0]!=null&&x>1)
                    {
                        if (total_records % 5000 == 0)
                            Console.WriteLine("The current total read rows is {0}.", total_records);     
                        string[] value = _values[0].Split(',');
                        if (value.Length == 11)
                        {
                            TradeEntry entry=new TradeEntry();
                            entry.trade_year = i;
                            int from_code = Int32.Parse(value[1]);
                            if (mapping.ContainsKey(from_code))
                            {
                                entry.country_from_id = mapping[from_code];
                            }
                            int to_code = Int32.Parse(value[3]);
                            if (mapping.ContainsKey(to_code))
                            {
                                entry.country_to_id = mapping[to_code];
                            }                            
                            double total_value = Double.Parse(value[9]);                            
                            int type = Int32.Parse(value[2]);
                            if (type == 1)
                            {
                                entry.import_value = total_value;
                                total_records++;
                                WBAccess.UpdateTradeImport(entry);
                            }
                            else
                            {
                                total_records++;
                                entry.export_value = total_value;
                                WBAccess.UpdateTradeExport(entry);
                            }
                        }
                    }
                }
                sr.Close();
                Console.WriteLine("Update trade in year {0} complete.",i);
                Console.WriteLine("The current total read rows is{0}.", total_records);                
            }
        }
    }
}
