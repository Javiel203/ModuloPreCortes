using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

using static DevExpress.Utils.Drawing.Helpers.NativeMethods;

namespace IVisionPrecutDocs.Entities
{
    internal class PagePrecrop
    {
        public int? PK_PagePrecrop { get; set; }
        public int FK_Page { get; set; }
        public string fileNameCrop {  get; set; }
        public string dirnameCrop { get; set; }
        public int FK_node { get; set; }
        public string PolyTextGeometry { get; set; }
        public string PolyTextYoloFormat { get; set; }
        public int? FK_LabelDoc { get; set; }
        public double? widthPct { get; set; }
        public double? heightPct { get; set; }
        public string Points2D { get; set; }
        public int? FK_ObjectsLayer { get; set; }
        public string namelabel { get; set; }
        public string localmage { get; set; }
        public int? ProcessbyIA { get; set; }
        public int? ProcessIAscore { get; set; }
        public string PolyTextGeometryPoints { get; set; }
        public int? precorteOCR { get; set; }
        public int PrecropPointsValidation { get; set; }


        public PagePrecrop() { }


        public PagePrecrop(int? PK_PagePrecrop,
            int FK_Page,
            string fileNameCrop,
            string dirnameCrop, 
            int FK_node, 
            string PolyTextGeometry, 
            string PolyTextYoloFormat, 
            int FK_LabelDoc, 
            double widthPct, 
            double heightPct,
            string Points2D, 
            int FK_ObjectsLayer,
            string namelabel,
            int ProcessbyIA,
            int ProcessIAscore,
            string PolyTextGeometryPoints,
            int precorteOCR,
            int PrecropPointsValidation)
        {
            this.PK_PagePrecrop = PK_PagePrecrop;
            this.FK_Page = FK_Page;
            this.fileNameCrop = fileNameCrop;
            this.dirnameCrop = dirnameCrop;
            this.FK_node = FK_node;
            this.PolyTextGeometry = PolyTextGeometry;  
            this.PolyTextYoloFormat = PolyTextYoloFormat;
            this.FK_LabelDoc = FK_LabelDoc;
            this.widthPct = widthPct;
            this.heightPct = heightPct;
            this.Points2D = Points2D;
            this.FK_ObjectsLayer = FK_ObjectsLayer;
            this.namelabel = namelabel;     
            this.ProcessbyIA = ProcessbyIA;
            this.ProcessIAscore = ProcessIAscore;
            this.PolyTextGeometryPoints = PolyTextGeometryPoints;
            this.precorteOCR = precorteOCR;
            this.PrecropPointsValidation = PrecropPointsValidation;
        }
    }
}
