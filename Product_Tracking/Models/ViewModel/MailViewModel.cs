using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Product_Tracking.Models.ViewModel
{
    public class MailViewModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachment { get; set; }
    }
}