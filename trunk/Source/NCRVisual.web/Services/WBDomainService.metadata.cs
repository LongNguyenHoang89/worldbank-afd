
namespace NCRVisual.web.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // The MetadataTypeAttribute identifies ref_country_indicatorMetadata as the class
    // that carries additional metadata for the ref_country_indicator class.
    [MetadataTypeAttribute(typeof(ref_country_indicator.ref_country_indicatorMetadata))]
    public partial class ref_country_indicator
    {

        // This class allows you to attach custom attributes to properties
        // of the ref_country_indicator class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ref_country_indicatorMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ref_country_indicatorMetadata()
            {
            }

            public Nullable<int> country_id { get; set; }

            public int country_indicator_id_pk { get; set; }

            public Nullable<float> country_indicator_value { get; set; }

            public Nullable<int> country_indicator_year { get; set; }

            public Nullable<int> indicator_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ref_project_milleniumMetadata as the class
    // that carries additional metadata for the ref_project_millenium class.
    [MetadataTypeAttribute(typeof(ref_project_millenium.ref_project_milleniumMetadata))]
    public partial class ref_project_millenium
    {

        // This class allows you to attach custom attributes to properties
        // of the ref_project_millenium class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ref_project_milleniumMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ref_project_milleniumMetadata()
            {
            }

            public Nullable<int> millenium_id { get; set; }

            public Nullable<int> project_id { get; set; }

            public int ref_project_millenium_id_pk { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ref_tab_indicatorMetadata as the class
    // that carries additional metadata for the ref_tab_indicator class.
    [MetadataTypeAttribute(typeof(ref_tab_indicator.ref_tab_indicatorMetadata))]
    public partial class ref_tab_indicator
    {

        // This class allows you to attach custom attributes to properties
        // of the ref_tab_indicator class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ref_tab_indicatorMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ref_tab_indicatorMetadata()
            {
            }

            public Nullable<int> indicator_id { get; set; }

            public Nullable<int> tab_id { get; set; }

            public int tab_indicator_id_pk { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ref_user_countryMetadata as the class
    // that carries additional metadata for the ref_user_country class.
    [MetadataTypeAttribute(typeof(ref_user_country.ref_user_countryMetadata))]
    public partial class ref_user_country
    {

        // This class allows you to attach custom attributes to properties
        // of the ref_user_country class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ref_user_countryMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ref_user_countryMetadata()
            {
            }

            public int country_id { get; set; }

            public Nullable<decimal> lat { get; set; }

            public Nullable<decimal> @long { get; set; }

            public int user_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ref_user_indicatorMetadata as the class
    // that carries additional metadata for the ref_user_indicator class.
    [MetadataTypeAttribute(typeof(ref_user_indicator.ref_user_indicatorMetadata))]
    public partial class ref_user_indicator
    {

        // This class allows you to attach custom attributes to properties
        // of the ref_user_indicator class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ref_user_indicatorMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ref_user_indicatorMetadata()
            {
            }

            public int indicator_id { get; set; }

            public int user_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies ref_user_tabMetadata as the class
    // that carries additional metadata for the ref_user_tab class.
    [MetadataTypeAttribute(typeof(ref_user_tab.ref_user_tabMetadata))]
    public partial class ref_user_tab
    {

        // This class allows you to attach custom attributes to properties
        // of the ref_user_tab class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class ref_user_tabMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private ref_user_tabMetadata()
            {
            }

            public string feed_link { get; set; }

            public int tab_id { get; set; }

            public int user_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_countriesMetadata as the class
    // that carries additional metadata for the tbl_countries class.
    [MetadataTypeAttribute(typeof(tbl_countries.tbl_countriesMetadata))]
    public partial class tbl_countries
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_countries class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_countriesMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_countriesMetadata()
            {
            }

            public Nullable<int> country_code { get; set; }

            public int country_id_pk { get; set; }

            public string country_iso_code { get; set; }

            public Nullable<decimal> country_latitude { get; set; }

            public Nullable<decimal> country_longitude { get; set; }

            public string country_name { get; set; }

            public Nullable<bool> has_flag { get; set; }

            public string income_level_id { get; set; }

            public Nullable<bool> is_region { get; set; }

            public string lending_type_id { get; set; }

            public string region_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_graphsMetadata as the class
    // that carries additional metadata for the tbl_graphs class.
    [MetadataTypeAttribute(typeof(tbl_graphs.tbl_graphsMetadata))]
    public partial class tbl_graphs
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_graphs class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_graphsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_graphsMetadata()
            {
            }

            public int graph_id_pk { get; set; }

            public string graph_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_income_levelsMetadata as the class
    // that carries additional metadata for the tbl_income_levels class.
    [MetadataTypeAttribute(typeof(tbl_income_levels.tbl_income_levelsMetadata))]
    public partial class tbl_income_levels
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_income_levels class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_income_levelsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_income_levelsMetadata()
            {
            }

            public string income_level_id_pk { get; set; }

            public string income_level_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_indicatorsMetadata as the class
    // that carries additional metadata for the tbl_indicators class.
    [MetadataTypeAttribute(typeof(tbl_indicators.tbl_indicatorsMetadata))]
    public partial class tbl_indicators
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_indicators class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_indicatorsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_indicatorsMetadata()
            {
            }

            public string indicator_code { get; set; }

            public string indicator_description { get; set; }

            public int indicator_id_pk { get; set; }

            public string indicator_name { get; set; }

            public string indicator_unit { get; set; }

            public Nullable<bool> is_gotten { get; set; }

            public Nullable<bool> is_yearly { get; set; }

            public Nullable<int> last_update { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_lending_typesMetadata as the class
    // that carries additional metadata for the tbl_lending_types class.
    [MetadataTypeAttribute(typeof(tbl_lending_types.tbl_lending_typesMetadata))]
    public partial class tbl_lending_types
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_lending_types class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_lending_typesMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_lending_typesMetadata()
            {
            }

            public string lending_type_id_pk { get; set; }

            public string lending_type_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_milleniumMetadata as the class
    // that carries additional metadata for the tbl_millenium class.
    [MetadataTypeAttribute(typeof(tbl_millenium.tbl_milleniumMetadata))]
    public partial class tbl_millenium
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_millenium class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_milleniumMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_milleniumMetadata()
            {
            }

            public int millenium_id_pk { get; set; }

            public string millenium_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_projectsMetadata as the class
    // that carries additional metadata for the tbl_projects class.
    [MetadataTypeAttribute(typeof(tbl_projects.tbl_projectsMetadata))]
    public partial class tbl_projects
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_projects class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_projectsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_projectsMetadata()
            {
            }

            public Nullable<int> country_id { get; set; }

            public string project_approval_date { get; set; }

            public string project_borrower { get; set; }

            public string project_close_date { get; set; }

            public string project_cost { get; set; }

            public int project_id_pk { get; set; }

            public string project_implement_agency { get; set; }

            public string project_link { get; set; }

            public string project_major_sector { get; set; }

            public string project_name { get; set; }

            public string project_outcome { get; set; }

            public string project_region { get; set; }

            public string project_status { get; set; }

            public string project_themes { get; set; }

            public string project_wb_id { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_regionsMetadata as the class
    // that carries additional metadata for the tbl_regions class.
    [MetadataTypeAttribute(typeof(tbl_regions.tbl_regionsMetadata))]
    public partial class tbl_regions
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_regions class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_regionsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_regionsMetadata()
            {
            }

            public string region_id_pk { get; set; }

            public string region_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_tabsMetadata as the class
    // that carries additional metadata for the tbl_tabs class.
    [MetadataTypeAttribute(typeof(tbl_tabs.tbl_tabsMetadata))]
    public partial class tbl_tabs
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_tabs class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_tabsMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_tabsMetadata()
            {
            }

            public string tab_feed_link { get; set; }

            public int tab_id_pk { get; set; }

            public string tab_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_tradesMetadata as the class
    // that carries additional metadata for the tbl_trades class.
    [MetadataTypeAttribute(typeof(tbl_trades.tbl_tradesMetadata))]
    public partial class tbl_trades
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_trades class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_tradesMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_tradesMetadata()
            {
            }

            public Nullable<int> country_from_id { get; set; }

            public Nullable<int> country_to_id { get; set; }

            public Nullable<decimal> export_value { get; set; }

            public Nullable<decimal> import_value { get; set; }

            public int trade_id_pk { get; set; }

            public Nullable<int> trade_year { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies tbl_usersMetadata as the class
    // that carries additional metadata for the tbl_users class.
    [MetadataTypeAttribute(typeof(tbl_users.tbl_usersMetadata))]
    public partial class tbl_users
    {

        // This class allows you to attach custom attributes to properties
        // of the tbl_users class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class tbl_usersMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private tbl_usersMetadata()
            {
            }

            public string facebook_id { get; set; }

            public string msn_id { get; set; }

            public string password { get; set; }

            public string twitter_id { get; set; }

            public int user_id_pk { get; set; }

            public string user_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies TM_WORLD_BORDERS_SIMPL_0_2Metadata as the class
    // that carries additional metadata for the TM_WORLD_BORDERS_SIMPL_0_2 class.
    [MetadataTypeAttribute(typeof(TM_WORLD_BORDERS_SIMPL_0_2.TM_WORLD_BORDERS_SIMPL_0_2Metadata))]
    public partial class TM_WORLD_BORDERS_SIMPL_0_2
    {

        // This class allows you to attach custom attributes to properties
        // of the TM_WORLD_BORDERS_SIMPL_0_2 class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class TM_WORLD_BORDERS_SIMPL_0_2Metadata
        {

            // Metadata classes are not meant to be instantiated.
            private TM_WORLD_BORDERS_SIMPL_0_2Metadata()
            {
            }

            public Nullable<long> AREA { get; set; }

            public string FIPS { get; set; }

            public int ID { get; set; }

            public string ISO2 { get; set; }

            public string ISO3 { get; set; }

            public Nullable<double> LAT { get; set; }

            public Nullable<double> LON { get; set; }

            public string NAME { get; set; }

            public Nullable<long> POP2005 { get; set; }

            public Nullable<int> REGION { get; set; }

            public Nullable<int> SUBREGION { get; set; }

            public Nullable<int> UN { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies View_CountryBorderMetadata as the class
    // that carries additional metadata for the View_CountryBorder class.
    [MetadataTypeAttribute(typeof(View_CountryBorder.View_CountryBorderMetadata))]
    public partial class View_CountryBorder
    {

        // This class allows you to attach custom attributes to properties
        // of the View_CountryBorder class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class View_CountryBorderMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private View_CountryBorderMetadata()
            {
            }

            public Nullable<long> AREA { get; set; }

            public string FIPS { get; set; }

            public string geom { get; set; }

            public int ID { get; set; }

            public string ISO2 { get; set; }

            public string ISO3 { get; set; }

            public Nullable<double> LAT { get; set; }

            public Nullable<double> LON { get; set; }

            public string NAME { get; set; }

            public Nullable<long> POP2005 { get; set; }

            public Nullable<int> REGION { get; set; }

            public Nullable<int> SUBREGION { get; set; }

            public Nullable<int> UN { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies View_CountryIndicatorMetadata as the class
    // that carries additional metadata for the View_CountryIndicator class.
    [MetadataTypeAttribute(typeof(View_CountryIndicator.View_CountryIndicatorMetadata))]
    public partial class View_CountryIndicator
    {

        // This class allows you to attach custom attributes to properties
        // of the View_CountryIndicator class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class View_CountryIndicatorMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private View_CountryIndicatorMetadata()
            {
            }

            public int country_id_pk { get; set; }

            public Nullable<decimal> country_indicator_value { get; set; }

            public Nullable<int> country_indicator_year { get; set; }

            public string country_iso_code { get; set; }

            public string country_name { get; set; }

            public string indicator_code { get; set; }

            public string indicator_description { get; set; }

            public string indicator_name { get; set; }

            public string indicator_unit { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies View_CountryIndicatorTabMetadata as the class
    // that carries additional metadata for the View_CountryIndicatorTab class.
    [MetadataTypeAttribute(typeof(View_CountryIndicatorTab.View_CountryIndicatorTabMetadata))]
    public partial class View_CountryIndicatorTab
    {

        // This class allows you to attach custom attributes to properties
        // of the View_CountryIndicatorTab class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class View_CountryIndicatorTabMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private View_CountryIndicatorTabMetadata()
            {
            }

            public int country_id_pk { get; set; }

            public Nullable<decimal> country_indicator_value { get; set; }

            public Nullable<int> country_indicator_year { get; set; }

            public string country_iso_code { get; set; }

            public string country_name { get; set; }

            public string indicator_code { get; set; }

            public string indicator_description { get; set; }

            public string indicator_name { get; set; }

            public string indicator_unit { get; set; }

            public int tab_id_pk { get; set; }

            public string tab_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies View_GeneralCountryMetadata as the class
    // that carries additional metadata for the View_GeneralCountry class.
    [MetadataTypeAttribute(typeof(View_GeneralCountry.View_GeneralCountryMetadata))]
    public partial class View_GeneralCountry
    {

        // This class allows you to attach custom attributes to properties
        // of the View_GeneralCountry class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class View_GeneralCountryMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private View_GeneralCountryMetadata()
            {
            }

            public int country_id_pk { get; set; }

            public string country_name { get; set; }

            public string income_level_name { get; set; }

            public string lending_type_name { get; set; }

            public string region_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies View_TabIndicatorMetadata as the class
    // that carries additional metadata for the View_TabIndicator class.
    [MetadataTypeAttribute(typeof(View_TabIndicator.View_TabIndicatorMetadata))]
    public partial class View_TabIndicator
    {

        // This class allows you to attach custom attributes to properties
        // of the View_TabIndicator class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class View_TabIndicatorMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private View_TabIndicatorMetadata()
            {
            }

            public string indicator_code { get; set; }

            public string indicator_description { get; set; }

            public int indicator_id_pk { get; set; }

            public string indicator_name { get; set; }

            public string indicator_unit { get; set; }

            public int tab_id_pk { get; set; }

            public string tab_name { get; set; }
        }
    }

    // The MetadataTypeAttribute identifies View_User_CountryMetadata as the class
    // that carries additional metadata for the View_User_Country class.
    [MetadataTypeAttribute(typeof(View_User_Country.View_User_CountryMetadata))]
    public partial class View_User_Country
    {

        // This class allows you to attach custom attributes to properties
        // of the View_User_Country class.
        //
        // For example, the following marks the Xyz property as a
        // required property and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression("[A-Z][A-Za-z0-9]*")]
        //    [StringLength(32)]
        //    public string Xyz { get; set; }
        internal sealed class View_User_CountryMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private View_User_CountryMetadata()
            {
            }

            public int country_id_pk { get; set; }

            public string country_iso_code { get; set; }

            public string country_name { get; set; }

            public int user_id_pk { get; set; }

            public string user_name { get; set; }
        }
    }
}
