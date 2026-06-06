using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVisionPrecutDocs.Entities
{
    internal class Zone
    {
        public string id { get; set; }
        public string periodico { get; set; }
        public string tipoDeCorte { get; set; }
        public int ProcessbyIA { get; set; }
        public int ProcessIAscore { get; set; }
        public int precorteOCR { get; set; }
        public int Aprobado { get; set; }
        public List<Point> points { get; set; }
        public Color color { get; set; } = Color.LimeGreen;

        public Zone()
        {
            id = string.Empty;
            periodico = string.Empty;
            tipoDeCorte = string.Empty;
            ProcessbyIA = 0;
            ProcessIAscore = 0;
            precorteOCR = 0;
            Aprobado = 0;
            points = new List<Point>();

        }

        public Zone(string id, string periodico, string tipoDeCorte, List<Point> points, Color color, int processbyIA = 0, int processIAscore = 0, int precorteOCR = 0, int aprobado = 0)
        {
            this.id = id;
            this.periodico = periodico;
            this.tipoDeCorte = tipoDeCorte;
            this.points = points ?? new List<Point>();
            this.color = color;
            this.ProcessbyIA = processbyIA;
            this.ProcessIAscore = processIAscore;
            this.precorteOCR = precorteOCR;
            this.Aprobado = aprobado;
        }
    }
}
