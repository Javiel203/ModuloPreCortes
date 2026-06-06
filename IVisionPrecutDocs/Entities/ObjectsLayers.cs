using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVisionPrecutDocs.Entities
{
    internal class ObjectsLayers
    {
        public int PK_ObjectsLayer { get; set; }
        public string dirObject { get; set; }
        public string fileName { get; set; }
        public string dirObjectMini { get; set; }
        public string fileNameMini { get; set; }
        public int? FK_Node { get; set; }
        public int? FK_ObjectMaster {  get; set; }
    }
}
