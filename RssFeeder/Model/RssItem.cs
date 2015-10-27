using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssFeeder.Model
{
    public class RssItem
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
    }
}
