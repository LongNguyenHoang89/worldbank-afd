using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using WbWCF.Contract.Data;
using System.Collections.ObjectModel;
using WbTest;
using System.Data;
using WbTest.DataAccess;

namespace WbWCF.DataAccess
{
    public class WBAccess : BaseDataAccess
    {
        public static bool DeleteAllStaticData()
        {
            SqlConnection con = GetConnection();
            con.Open();
            string sql = @"DELETE FROM [WB].[dbo].[tbl_countries];
                        DELETE FROM [WB].[dbo].[tbl_regions];
                        DELETE FROM [WB].[dbo].[tbl_income_levels];
                        DELETE FROM [WB].[dbo].[tbl_lending_types];
                        DELETE FROM [WB].[dbo].[tbl_trades];";
            SqlCommand command = new SqlCommand(sql, con);
            command.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static void InsertCountries(Collection<CountryEntry> countries)
        {
            SqlConnection con = GetConnection();
            con.Open();
            for (int i = 0; i < countries.Count; i++)
            {
                string sql = @"INSERT INTO [tbl_countries]
                           ([country_id_pk]
                           ,[country_iso_code]
                           ,[country_name]
                           ,[region_id]
                           ,[income_level_id]
                           ,[lending_type_id]
                           ,[country_longitude]
                           ,[country_latitude]
                           ,[is_region])
                     VALUES
                           (@country_id_pk
                           ,@country_iso_code
                           ,@country_name
                           ,@region_id
                           ,@income_level_id
                           ,@lending_type_id
                           ,@country_longitude
                           ,@country_latitude
                           ,@is_region)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@country_id_pk", countries[i].country_id_pk);
                cmd.Parameters.AddWithValue("@country_iso_code", countries[i].country_iso_code);
                cmd.Parameters.AddWithValue("@country_name", countries[i].country_name);
                cmd.Parameters.AddWithValue("@region_id", countries[i].region_id);
                cmd.Parameters.AddWithValue("@income_level_id", countries[i].income_level_id);
                cmd.Parameters.AddWithValue("@lending_type_id", countries[i].lending_type_id);
                cmd.Parameters.AddWithValue("@country_longitude", countries[i].country_longitude);
                cmd.Parameters.AddWithValue("@country_latitude", countries[i].country_latitude);
                cmd.Parameters.AddWithValue("@is_region", countries[i].is_region);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static void InsertIndicators(Collection<IndicatorEntry> indicators)
        {
            SqlConnection con = GetConnection();
            con.Open();
            for (int i = 0; i < indicators.Count; i++)
            {
                string sql = @"INSERT INTO [tbl_indicators]
                           ([indicator_id_pk]
                           ,[indicator_code]
                           ,[indicator_name]
                           ,[indicator_description])
                     VALUES
                           (@indicator_id_pk
                           ,@indicator_code
                           ,@indicator_name
                           ,@indicator_description)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@indicator_id_pk", indicators[i].indicator_id_pk);
                cmd.Parameters.AddWithValue("@indicator_code", indicators[i].indicator_code);
                cmd.Parameters.AddWithValue("@indicator_name", indicators[i].indicator_name);
                cmd.Parameters.AddWithValue("@indicator_description", indicators[i].indicator_description);                
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static void InsertIncomeLevels(Collection<IncomeLevelEntry> income_levels)
        {
            SqlConnection con = GetConnection();
            con.Open();
            for (int i = 0; i < income_levels.Count; i++)
            {
                string sql = @"INSERT INTO [tbl_income_levels]
                            ([income_level_id_pk]
                            ,[income_level_name])
                        VALUES
                            (@income_level_id_pk
                            ,@income_level_name)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@income_level_id_pk", income_levels[i].income_level_id_pk);
                cmd.Parameters.AddWithValue("@income_level_name", income_levels[i].income_level_name);                
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static void InsertRegions(Collection<RegionEntry> regions)
        {
            SqlConnection con = GetConnection();
            con.Open();
            for (int i = 0; i < regions.Count; i++)
            {
                string sql = @"INSERT INTO [tbl_regions]
                            ([region_id_pk]
                            ,[region_name])
                        VALUES
                            (@region_id_pk
                            ,@region_name)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@region_id_pk", regions[i].region_id_pk);
                cmd.Parameters.AddWithValue("@region_name", regions[i].region_name);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static void UpdateCountry(Collection<CountryEntry> countries)
        {
            SqlConnection con = GetConnection();
            con.Open();
            for (int i = 0; i < countries.Count; i++)
            {
                string sql = @"UPDATE [tbl_countries]
                             SET [country_code] = @country_code
                             WHERE [country_iso_code]=@country_iso_code and is_region=0;
                             UPDATE [tbl_countries]
                             SET [country_code] = 0
                             WHERE [country_iso_code]='1W'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@country_code", countries[i].country_code);
                cmd.Parameters.AddWithValue("@country_iso_code", countries[i].country_iso_code);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static void UpdateCountryFlag(int country_id_pk, bool has_flag)
        {
            SqlConnection con = GetConnection();
            con.Open();            
            string sql = @"UPDATE [tbl_countries]
                            SET [has_flag] = @has_flag
                            WHERE [country_id_pk]=@country_iso_code and is_region=0";                            
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@has_flag", has_flag);
            cmd.Parameters.AddWithValue("@country_iso_code", country_id_pk);
            cmd.ExecuteNonQuery();
            con.Close();
        }


        public static void InsertLendingTypes(Collection<LendingTypeEntry> lending_types)
        {
            SqlConnection con = GetConnection();
            con.Open();
            for (int i = 0; i < lending_types.Count; i++)
            {
                string sql = @"INSERT INTO [tbl_lending_types]
                               ([lending_type_id_pk]
                               ,[lending_type_name])
                         VALUES
                               (@lending_type_id_pk
                               ,@lending_type_name)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@lending_type_id_pk", lending_types[i].lending_type_id_pk);
                cmd.Parameters.AddWithValue("@lending_type_name", lending_types[i].lending_type_name);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static void InsertIndicatorValue(Collection<CountryIndicatorEntry> ref_values)
        {
            SqlConnection con = GetConnection();
            con.Open();
            for (int i = 0; i < ref_values.Count; i++)
            {
                string sql = @"INSERT INTO [ref_country_indicator]
                               ([country_indicator_year]
                               ,[country_id]
                               ,[indicator_id]
                               ,[country_indicator_value])
                         VALUES
                               (@country_indicator_year
                               ,@country_id
                               ,@indicator_id
                               ,@country_indicator_value)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@country_indicator_year", ref_values[i].country_indicator_year);
                cmd.Parameters.AddWithValue("@country_id", ref_values[i].country_id);
                cmd.Parameters.AddWithValue("@indicator_id", ref_values[i].indicator_id);
                cmd.Parameters.AddWithValue("@country_indicator_value", ref_values[i].country_indicator_value);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        private static bool Is_New_Trade(TradeEntry entry)
        {
            SqlConnection con = GetConnection();
            con.Open();
            string sql = @"SELECT trade_id_pk 
                         FROM tbl_trades where country_from_id=" + entry.country_from_id;
            sql += " and country_to_id=" + entry.country_to_id;
            sql += " and trade_year=" + entry.trade_year;
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "check");
            DataTable dt = ds.Tables["check"];
            con.Close();
            return dt.Rows.Count == 0;
        }

        public static void UpdateTradeImport(TradeEntry entry)
        {

            if (entry.country_from_id != null && entry.country_to_id != null)
            {
                bool is_new = Is_New_Trade(entry);
                if (is_new)
                {
                    SqlConnection con = GetConnection();
                    con.Open();
                    string sql = @"INSERT INTO [tbl_trades]
                               ([country_from_id]
                               ,[country_to_id]
                               ,[import_value]
                               ,[trade_year])
                         VALUES
                               (@country_from_id
                               ,@country_to_id
                               ,@import_value
                               ,@trade_year)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@country_from_id", entry.country_from_id);
                    cmd.Parameters.AddWithValue("@country_to_id", entry.country_to_id);
                    cmd.Parameters.AddWithValue("@import_value", entry.import_value);
                    cmd.Parameters.AddWithValue("@trade_year", entry.trade_year);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    SqlConnection con = GetConnection();
                    con.Open();
                    string sql = @"UPDATE [tbl_trades]
                                SET [import_value] =@import_value
                                WHERE [country_from_id]=@country_from_id and [country_to_id]=@country_to_id and trade_year=@trade_year";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@country_from_id", entry.country_from_id);
                    cmd.Parameters.AddWithValue("@country_to_id", entry.country_to_id);
                    cmd.Parameters.AddWithValue("@import_value", entry.import_value);
                    cmd.Parameters.AddWithValue("@trade_year", entry.trade_year);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                
            }
            
        }

        public static void UpdateIndicator(IndicatorEntry entry)
        {
            SqlConnection con = GetConnection();
            con.Open();
            string sql = @"UPDATE [tbl_indicators]
                        SET [indicator_unit] =@indicator_unit , [is_gotten]=1
                        WHERE [indicator_id_pk]=@indicator_id_pk";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@indicator_id_pk", entry.indicator_id_pk);
            cmd.Parameters.AddWithValue("@indicator_unit", entry.indicator_unit);
            cmd.ExecuteNonQuery();
        }

        public static Collection<IndicatorEntry> GetSelectedIndicator()
        {
            SqlConnection con = GetConnection();
            con.Open();
            string sql = @"SELECT *
                         FROM tbl_indicators
                         WHERE is_gotten=0";
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "tbl_indicators");
            Collection<IndicatorEntry> indicators = ds.Tables["tbl_indicators"].Rows.ToIndicatorEntries();
            return indicators;
        }

        public static void UpdateTradeExport(TradeEntry entry)
        {

            if (entry.country_from_id != null && entry.country_to_id != null)
            {
                bool is_new = Is_New_Trade(entry);
                if (is_new)
                {
                    SqlConnection con = GetConnection();
                    con.Open();
                    string sql = @"INSERT INTO [tbl_trades]
                               ([country_from_id]
                               ,[country_to_id]
                               ,[export_value]
                               ,[trade_year])
                         VALUES
                               (@country_from_id
                               ,@country_to_id
                               ,@export_value
                               ,@trade_year)";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@country_from_id", entry.country_from_id);
                    cmd.Parameters.AddWithValue("@country_to_id", entry.country_to_id);
                    cmd.Parameters.AddWithValue("@export_value", entry.export_value);
                    cmd.Parameters.AddWithValue("@trade_year", entry.trade_year);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    SqlConnection con = GetConnection();
                    con.Open();
                    string sql = @"UPDATE [tbl_trades]
                                SET [export_value] =@export_value
                                WHERE [country_from_id]=@country_from_id and [country_to_id]=@country_to_id and trade_year=@trade_year";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@country_from_id", entry.country_from_id);
                    cmd.Parameters.AddWithValue("@country_to_id", entry.country_to_id);
                    cmd.Parameters.AddWithValue("@export_value", entry.export_value);
                    cmd.Parameters.AddWithValue("@trade_year", entry.trade_year);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                
            }
            
        }

        public static Dictionary<int, int> MapCountryIdToCode()
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            SqlConnection con = GetConnection();
            con.Open();
            string sql = @"SELECT country_id_pk,country_code 
                         FROM tbl_countries";
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "tbl_countries");
            DataTable dt = ds.Tables["tbl_countries"];
            for (int i = 0; i < dt.Rows.Count; i++)            
            {
                int id = Int32.Parse(dt.Rows[i]["country_id_pk"].ToString());
                if (dt.Rows[i]["country_code"] != null)
                {
                    try
                    {
                        int code = Int32.Parse(dt.Rows[i]["country_code"].ToString());
                        result.Add(code, id);
                    }
                    catch
                    {
                    }
                }
            }
            con.Close();
            return result;
                        
        }

        public static Dictionary<string, int> MapCountryIdToIsoCode()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            SqlConnection con = GetConnection();
            con.Open();
            string sql = @"SELECT country_id_pk,country_iso_code 
                         FROM tbl_countries";
            SqlDataAdapter da = new SqlDataAdapter(sql, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "tbl_countries");
            DataTable dt = ds.Tables["tbl_countries"];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int id = Int32.Parse(dt.Rows[i]["country_id_pk"].ToString());
                if (dt.Rows[i]["country_iso_code"] != null)
                {
                    try
                    {
                        string code = dt.Rows[i]["country_iso_code"].ToString();
                        result.Add(code, id);
                    }
                    catch
                    {
                    }
                }
            }
            con.Close();
            return result;

        }
        
    }
}