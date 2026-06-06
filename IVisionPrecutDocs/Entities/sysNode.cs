using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVisionPrecutDocs.Entities
{
    internal class sysNode
    {
        public int PK_Node { get; set; }
        public string ip { get; set; }
        public string storageDirectory { get; set; }
        public string ftpUser { get; set; }
        public string ftpPass { get; set; }
        public int ftpPort { get; set; }
        //public string wanIp { get; set; }
        //public int wwwPort { get; set; }
        //public string wwwDirectory { get; set; }
    }
}
