
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
}
