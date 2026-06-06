using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVisionPrecutDocs.Entities
{
    internal class LabelDoc
    {
        public int PK_LabelDoc { get; set; }
        public int FK_doc { get; set; }
        public string namelabel { get; set; }
        public double widthPct { get; set; }
        public double heightPct { get; set; }
        public int FK_ObjectMaster { get; set; }
        public int FK_ObjectGroup { get; set; }
        public int CatImgIa { get; set; }
        public int CatImgNoIa { get; set; }
        public int CatImgOcr { get; set; }
        public int CatApproved { get; set; }
    }
}
