using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVisionPrecutDocs.Entities
{
    internal class Docs
    {
        public int NItem { get; set; }
        public int Pk_Doc { get; set; }
        public int FK_cat { get; set; }
        public DateTime dateInclude { get; set; }
        public string Cat { get; set; }
        public int TotalPages { get; set; }
        public int TotalPagesOCR { get; set; }
        public long TotalTimeOCR { get; set; }
        public int TotalPagesIA { get; set; }
        public long TotalTimeIA { get; set; }

    }
}
