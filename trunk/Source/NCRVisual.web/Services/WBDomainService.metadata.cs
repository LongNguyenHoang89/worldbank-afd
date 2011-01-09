
namespace NCRVisual.web.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


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
