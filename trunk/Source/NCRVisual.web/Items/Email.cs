using System;
using System.Collections.Generic;
using System.Web;

namespace NCRVisual.Web.Items
{
    public class Email
    {
        public string MessageId { get; set; }

        public int UserId { get; set; }

        public string MessageSubject { get; set; }

        public string SendDate { get; set; }

        public int UserTo { get; set; }
    }
}
