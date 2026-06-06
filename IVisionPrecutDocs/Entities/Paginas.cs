using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVisionPrecutDocs.Entities
{
    public class Paginas
    {
        public int PK_Page { get; set; }
        public int FK_doc {  get; set; }
        public int FK_Node { get; set; }
        public string dirNamePDF { get; set; }
        public string fileNameFullJPG { get; set; }
        public string dirNameFullJPG { get; set; }
        public string fileNameMiniJPG { get; set; }
        public string dirNameMiniJPG { get; set; }
        public int pagenumber { get; set; }
        public string LocalImgFull { get; set; }
        public string LocalMini { get; set; }
        public int PageheightPX { get; set; }
        public int PagewidthPX { get; set; }
        public int PageheightPoints { get; set; }
        public int PagewidthPoints { get; set; }
        public int dpi { get; set; }
    }
}
