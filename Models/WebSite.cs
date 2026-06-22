using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteWacher.Models
{
    public class WebSite
    {
        public Guid id { get; set; }
        public string Url { get; set; }
        public string xPathExpression { get; set; }
    }
}
