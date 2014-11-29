using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tenjin
{
    public class MarkdownModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
        public IEnumerable<Breadcrumb> Breadcrumbs { get; set; }

        public class Breadcrumb
        {
            public string Href { get; set; }
            public string Name { get; set; }
            public bool Active { get; set; }
        }
    }
}