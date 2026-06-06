using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IVisionPrecutDocs.Entities;
using IVisionPrecutDocs.Data;
using IVisionPrecutDocs.Proceso;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using System.IO;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraSplashScreen;
using System.Collections.ObjectModel;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraRichEdit.API.Native;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using System.Diagnostics;
using System.Text.Json;
using static DevExpress.Utils.Drawing.Helpers.NativeMethods;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.Xml;
using IVisionDrawSelection;
using NetTopologySuite.Geometries;

//using static DevExpress.Data.Mask.Internal.MaskSettings<T>;

namespace IVisionPrecutDocs
{ 
    public partial class PrecutDocs : UserControl
    {
        private static string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string folderTemp = Path.Combine(baseDir, "tmp");
        private GalleryItemGroup grupoMiniaturas;
        private List<Paginas> listaPaginas;
        private ObservableCollection<Zone> zonesObservable;
        public Paginas paginaActual { get; set; }
        private Docs docActual;
        private List<PagePrecrop> listPagePrecrop;
        private readonly ContextMenuStrip rowMenuGrid1 = new ContextMenuStrip();
        private readonly ContextMenuStrip rowMenuGrid2 = new ContextMenuStrip();
        public int lblWitdth { get; set; } = 2;
        public int lblHeight { get; set; } = 2;
        public int SelectIndexCbFiletLabelForms { get; set; } = 4;
        public int obGroupActual { get; set; } = 0;
        
        private static IConnection connection;
        private static ISession session;
        private static IMessageProducer producer;


        public PrecutDocs()
        {
            InitializeComponent();
            
            this.Load += PrecutDocs_Load;

            draw1.newZone += Draw1_newZone;
            draw2.pictureBoxCanva.MouseClick += PictureBoxCanva_MouseClick;
            galleryControl1.Gallery.ItemDoubleClick += Gallery_ItemDoubleClick;
            galleryControl1.Gallery.ItemClick += Gallery_ItemClick;
            ToolStripMenuItem mDelete = new ToolStripMenuItem("Eliminar", null, OnDeleteClick);
            ToolStripMenuItem mVer = new ToolStripMenuItem("Ver Detalles", null, OnVerClick);
            ToolStripMenuItem mApprove = new ToolStripMenuItem("Aprobar", null, OnApproveClick);
            ToolStripMenuItem mMove = new ToolStripMenuItem("Mover", null, OnMoveClick);
            ToolStripMenuItem ProcessIA = new ToolStripMenuItem("Procesar IA", null, OnProcessIAClick);
            ToolStripMenuItem ProcessOCR = new ToolStripMenuItem("Procesar OCR", null, OnProcessOCRClick);
            rowMenuGrid2.Items.AddRange(new ToolStripItem[] { mDelete, mVer, mMove, mApprove, new ToolStripSeparator() });
            rowMenuGrid1.Items.AddRange(new ToolStripItem[] { ProcessIA, ProcessOCR, new ToolStripSeparator() });
            gridView2.PopupMenuShowing += GridView2_PopupMenuShowing;
            gridView1.PopupMenuShowing += GridView1_PopupMenuShowing;
            //this.Disposed += (s, e) => OnControlClosed("Disposed");

        }

        private void PictureBoxCanva_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (draw2.pictureBoxCanva.Image == null)
                    return;
                
                double imgX = e.X * paginaActual.PagewidthPoints / draw2.pictureBoxCanva.ClientSize.Width;
                double imgY = e.Y * paginaActual.PageheightPoints / draw2.pictureBoxCanva.ClientSize.Height;

                List<PagePrecrop> pp = new Consults().GetCropsByPoint(paginaActual.PK_Page, imgX, imgY, paginaActual.PageheightPoints);
                int index = -1;
                foreach (var zone in zonesObservable)
                {
                    SetColorZone(int.Parse(zone.id),zone.color);                  
                }

                if (pp.Count > 0 || pp != null)
                {
                    foreach (PagePrecrop p in pp)
                    {
                        if (zonesObservable.Any(x => x.id == p.PK_PagePrecrop.ToString()))
                        {
                            index = SetColorZone((int)p.PK_PagePrecrop, Color.Yellow);
                        }
                        
                    }
                }
                if (index >= 0)
                {
                    GridView view = gridControl2.MainView as GridView;
                    int rowHandle = index;
                    view.ClearSelection();
                    view.SelectRow(rowHandle);
                    view.FocusedRowHandle = rowHandle;
                    view.MakeRowVisible(rowHandle);
                }             
            }
            catch (Exception)
            {

            }                
        }

        private int SetColorZone(int id, Color color)
        {
            int index = -1;
            foreach (var item in draw2.listZonesPolygons)
            {
                if (int.Parse(item.id) == id)
                {
                    item.color = color;
                    index = draw2.listZonesPolygons.IndexOf(item);
                }
            }
            return index;
        }

        private void GridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow || e.HitInfo.InRowCell)
            {
                var view = (GridView)sender;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                e.Allow = false;
                rowMenuGrid1.Show(gridControl1, e.Point);
            }
        }

        private void GridView2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow || e.HitInfo.InRowCell)
            {
                var view = (GridView)sender;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                e.Allow = false;
                rowMenuGrid2.Show(gridControl2, e.Point);
            }
        }

        private void OnProcessIAClick(object sender, EventArgs e)
        {
            try
            {
                var main = gridControl1.MainView as ColumnView;
                Docs doc = main.GetRow(main.FocusedRowHandle) as Docs;
                //MessageBox.Show(doc.Pk_Doc.ToString());
                if (new Updats().UpdateTaskPagePrecutbyIA(doc.Pk_Doc))
                {
                    List<int> list = new Consults().GetPagesByFkDoc(doc.Pk_Doc);
                    if (list.Count > 0)
                    {
                        Uri connecturi = new Uri(GetDataXml("ServerQueue"));
                        ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);
                        connection = connectionFactory.CreateConnection();
                        connection.Start();
                        session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                        IDestination destination = session.GetQueue(GetDataXml("QueueGet2"));
                        producer = session.CreateProducer(destination);
                        producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                        foreach (var item in list)
                        {
                            ITextMessage message = session.CreateTextMessage(item.ToString());
                            producer.Send(message);
                        }
                        DataSetGrid(dateEdit1.DateTime);
                    }

                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error al procesar Data");
            }          
        }

        private void OnProcessOCRClick(object sender, EventArgs e)
        {
            try
            {
                var main = gridControl1.MainView as ColumnView;
                Docs doc = main.GetRow(main.FocusedRowHandle) as Docs;
                //MessageBox.Show(doc.Pk_Doc.ToString());
                if (new Updats().UpdateTaskPagePrecutbyOCR(doc.Pk_Doc))
                {
                    List<int> list = new Consults().GetPagesByFkDoc(doc.Pk_Doc);
                    if (list.Count > 0)
                    {
                        Uri connecturi = new Uri(GetDataXml("ServerQueue"));
                        ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);
                        connection = connectionFactory.CreateConnection();
                        connection.Start();
                        session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                        IDestination destination = session.GetQueue(GetDataXml("QueueGet"));
                        producer = session.CreateProducer(destination);
                        producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                        foreach (var item in list)
                        {
                            ITextMessage message = session.CreateTextMessage(item.ToString());
                            producer.Send(message);
                        }
                        DataSetGrid(dateEdit1.DateTime);
                    }

                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error al procesar Data");
            }


        }



        private void OnDeleteClick(object sender, EventArgs e)
        {
            var main = gridControl2.MainView as ColumnView;
            Zone zone = main.GetRow(main.FocusedRowHandle) as Zone;         
            new Pro().DeletePreCor(Int32.Parse(zone.id));
            zonesObservable.Remove(zone);
            draw2.ClearCanva();
            if (zonesObservable.Count > 0)
            {
                foreach (var item in zonesObservable)
                {
                    IVisionDrawSelection.Entities.Zone zoneDraw = new IVisionDrawSelection.Entities.Zone();
                    zoneDraw.color = item.color;
                    zoneDraw.id = item.id;
                    zoneDraw.points = item.points;
                    draw2.WriteZone(zoneDraw);
                }
            }
            CounterZonesIAMa();
            
        }

        private void OnVerClick(object sender, EventArgs e)
        {
            var main = gridControl2.MainView as ColumnView;
            Zone zone = main.GetRow(main.FocusedRowHandle) as Zone;
            //MessageBox.Show(zone.id.ToString());
            DetailsPrecut formDetail = new DetailsPrecut(Int32.Parse(zone.id));
            formDetail.Show();
        }

        private void OnApproveClick(object sender, EventArgs e)
        {
            try
            {
                var main = gridControl2.MainView as ColumnView;
                Zone zone = main.GetRow(main.FocusedRowHandle) as Zone;

                PagePrecrop pp = new Consults().GetPagePrecropById(Int32.Parse(zone.id));

                LabelDoc labelDoc = new Consults().GetLabelDocByPK((int)pp.FK_LabelDoc);

                int idObjLayer = new Pro().ProInsertObjectsLayers(obGroupActual,
                labelDoc.FK_ObjectMaster,
                FarmatYoloString(zone.points, draw1.WidthPx, draw1.HeightPx, 8),
                Points2d(zone.points),
                paginaActual.LocalImgFull,
                zone.points);
                
                if (idObjLayer > 0)
                {
                    if (new Updats().UpdatePagePrecropFkObjLayer((int)pp.PK_PagePrecrop, idObjLayer))
                    {
                        zone.Aprobado = 1;
                        MessageBox.Show("objeto agregado correctamente");
                    }
                    else
                    {
                        MessageBox.Show("No se actualizo en la Tabla PagePrecrop");
                    }
                }
                else
                {
                    
                    MessageBox.Show("No se incerto el objeto");
                }
                
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error : {ex}");
            }
           

            //MessageBox.Show(zone.id.ToString());
            //DetailsPrecut formDetail = new DetailsPrecut(Int32.Parse(zone.id));
            //formDetail.Show();
        }

        private void OnMoveClick(object sender, EventArgs e)
        {
            var main = gridControl2.MainView as ColumnView;
            Zone zone = main.GetRow(main.FocusedRowHandle) as Zone;
            XFormMove frmMove = new XFormMove();
            frmMove.FK_cat = docActual.FK_cat;
            frmMove.FK_ObjGroup = obGroupActual;
            frmMove.pk_PagePrecrop = Int32.Parse(zone.id);
            frmMove.Show();
            frmMove.FormClosed += FrmMove_FormClosed;
        }

        private void FrmMove_FormClosed(object sender, FormClosedEventArgs e)
        {

            listPagePrecrop = new Consults().GetPagePrecrops(paginaActual.PK_Page);
            zonesObservable.Clear();
            if (listPagePrecrop != null)
            {
                if (listPagePrecrop.Count > 0)
                {
                    foreach (var precrop in listPagePrecrop)
                    {
                        if (paginaActual != null && docActual != null)
                        {

                            if (precrop.ProcessbyIA == null)
                            {
                                precrop.ProcessbyIA = 0;
                            }
                            if (precrop.ProcessIAscore == null)
                            {
                                precrop.ProcessIAscore = 0;
                            }
                            if (precrop.ProcessbyIA == 0)
                            {
                                Zone zone = new Zone(
                                precrop.PK_PagePrecrop.ToString(),
                                docActual.Cat, precrop.namelabel,
                                DeserializePoints2d(precrop.Points2D),
                                Color.LightGreen,
                                (int)precrop.ProcessbyIA,
                                (int)precrop.ProcessIAscore);
                                zonesObservable.Add(zone);
                            }
                            else
                            {
                                Zone zone = new Zone(
                                precrop.PK_PagePrecrop.ToString(),
                                docActual.Cat, precrop.namelabel,
                                DeserializePoints2d(precrop.Points2D),
                                Color.SkyBlue,
                                (int)precrop.ProcessbyIA,
                                (int)precrop.ProcessIAscore);
                                zonesObservable.Add(zone);
                            }
                            
                            


                            //IVisionDrawSelection.Entities.Zone zoneDraw = new IVisionDrawSelection.Entities.Zone();
                            //zoneDraw.color = zone.color;
                            //zoneDraw.id = zone.id;
                            //zoneDraw.points = zone.points;
                            //draw2.WriteZone(zoneDraw);
                        }
                    }
                    CounterZonesIAMa();
                }
            }
        }

        private void Draw1_newZone(List<System.Drawing.Point> obj)
        {
            var puntosF = obj.Select(p => new PointF(p.X, p.Y)).ToList();
            (double wPct, double hPct) = 
                GetMinRectPercentages(puntosF, draw1.WidthPx,draw1.HeightPx);
           
            LabelsForm formLabel = new LabelsForm(this);
            formLabel.FK_cat = docActual.FK_cat;
            formLabel.points = obj;
            formLabel.widthpct = wPct;
            formLabel.heightpct = hPct;
            formLabel.Show();
                                     
        }



        public void InsertPrecort(List<System.Drawing.Point> obj, int idLabelDoc, string nameLabel, double wp, double hp, int idObjLayer)
        {
            try
            {
                int id = new Consults().IdentCurrentPagePrecrop();
                if (id != 0)
                {
                    if (paginaActual != null && docActual != null)
                    {
                        sysNode node = new Consults().GetNodeByFk(paginaActual.FK_Node);
                        string remoteFile = $"{paginaActual.dirNamePDF}Precortes\\";
                        string fileNameCrop = $"{docActual.Cat}_{paginaActual.pagenumber.ToString("000")}_{nameLabel}_Precorte{(id + 1).ToString()}.jpg";
                        //string fileNameCrop = $"{docActual.Cat}_{paginaActual.pagenumber.ToString("000")}_{nameLabel}_Precorte";
                        using (Bitmap bmp = new Bitmap(draw1.pictureBoxCanva.Width, draw1.pictureBoxCanva.Height))
                        {
                            // Dibujar el control en el bitmap
                            draw1.pictureBoxCanva.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                            bool result = new FTP().uploadImageBytes(bmp, remoteFile, fileNameCrop, node);
                        }

                        int idZone = 
                            new Inserts().InsertPrecrop(new PagePrecrop(
                                null, 
                                paginaActual.PK_Page,
                                fileNameCrop, 
                                remoteFile, 
                                paginaActual.FK_Node, 
                                PointsToWKT(obj, paginaActual.PageheightPX),
                                FarmatYoloString(obj,draw1.WidthPx,draw1.HeightPx,8), 
                                idLabelDoc, 
                                wp, 
                                hp,
                                Points2d(obj),
                                idObjLayer,
                                "",
                                0,
                                0,
                                PointsToWKT(PixelesAListaPuntos(
                                    obj,paginaActual.PagewidthPX,
                                    paginaActual.PageheightPX,
                                    paginaActual.PagewidthPoints,
                                    paginaActual.PageheightPoints),paginaActual.PageheightPoints),
                                0,
                                1));
                        Zone zone = new Zone(
                            idZone.ToString(), 
                            docActual.Cat,
                            nameLabel, 
                            obj.ToList(), 
                            Color.Green,
                            0,
                            0,
                            0,
                            1);
                        zonesObservable.Add(zone);
                        draw1.ClearCanva();

                        IVisionDrawSelection.Entities.Zone zoneDraw = new IVisionDrawSelection.Entities.Zone();
                        zoneDraw.color = zone.color;
                        zoneDraw.id = zone.id;
                        zoneDraw.points = zone.points;
                        draw2.WriteZone(zoneDraw);
                        CounterZonesIAMa();
                    }
                }
                else
                {
                    MessageBox.Show("Error con la base de datos");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.ToString()}");
            }
        }

        public string Points2d(List<System.Drawing.Point> points)
        {
            var arr = points.Select(p => $"{p.X}, {p.Y}").ToList();
            return JsonSerializer.Serialize(arr, new JsonSerializerOptions { WriteIndented = true });
        }

        private string PointsToWKT(List<System.Drawing.Point> list, int pageHeight)
        {
            if (list == null || list.Count < 3) return string.Empty;
           
            var puntos = list.Select(p => new { X = p.X, Y = pageHeight - p.Y }).ToList();

            if (puntos.First().X == puntos.Last().X && puntos.First().Y == puntos.Last().Y)
            {
                puntos.RemoveAt(puntos.Count - 1);
            }

            string coordenadas = string.Join(", ",
                puntos.Concat(new[] { puntos.First() })
                      .Select(p => $"{p.X} {p.Y}"));

            return $"POLYGON (( {coordenadas} ))";
        }




        //public byte[] PixelesABytesWKB(
        //List<System.Drawing.Point> listaPuntosPx,
        //double anchoTotalPx,
        //double altoTotalPx,
        //double anchoTotalPt,
        //double altoTotalPt)
        //{
        //    // 1. Validaciones
        //    if (listaPuntosPx == null || listaPuntosPx.Count < 3) return null;

        //    // --- PASO CRÍTICO: CERRAR EL POLÍGONO ---
        //    // Para que sea un polígono sólido, debe terminar donde empezó.
        //    // Creamos una copia para no modificar la lista original fuera de la función.
        //    var puntosProcesar = new List<System.Drawing.Point>(listaPuntosPx);

        //    if (puntosProcesar[0] != puntosProcesar[puntosProcesar.Count - 1])
        //    {
        //        puntosProcesar.Add(puntosProcesar[0]); // Repetimos el primer punto al final
        //    }

        //    // 2. Factores de escala
        //    double factorX = anchoTotalPt / anchoTotalPx;
        //    double factorY = altoTotalPt / altoTotalPx;

        //    using (var ms = new MemoryStream())
        //    using (var writer = new BinaryWriter(ms))
        //    {
        //        // --- FORMATO WKB PARA POLYGON ---

        //        // A. Byte Order (1 byte): Little Endian
        //        writer.Write((byte)1);

        //        // B. Tipo de Geometría (4 bytes): 3 = POLYGON
        //        // (Antes usábamos 2 para LineString)
        //        writer.Write((int)3);

        //        // C. Cantidad de Anillos (4 bytes): 1
        //        // Un polígono simple tiene 1 anillo exterior. 
        //        // (Si tuviera agujeros en medio, serían más anillos).
        //        writer.Write((int)1);

        //        // D. Cantidad de Puntos en el anillo (4 bytes)
        //        writer.Write((int)puntosProcesar.Count);

        //        // E. Escribir los puntos escalados
        //        foreach (var p in puntosProcesar)
        //        {
        //            double xFinal = p.X * factorX;
        //            double yFinal = p.Y * factorY;

        //            writer.Write(xFinal);
        //            writer.Write(yFinal);
        //        }

        //        return ms.ToArray();
        //    }
        //}

        public List<System.Drawing.Point> PixelesAListaPuntos(
        List<System.Drawing.Point> listaPuntosPx,
        double anchoTotalPx,
        double altoTotalPx,
        double anchoTotalPt,
        double altoTotalPt)
        {
            // ... (Validaciones y cierre de polígono igual que arriba) ...
            if (listaPuntosPx == null || listaPuntosPx.Count < 3) return new List<System.Drawing.Point>();

            var puntosProcesar = new List<System.Drawing.Point>(listaPuntosPx);
            if (puntosProcesar[0] != puntosProcesar[puntosProcesar.Count - 1])
                puntosProcesar.Add(puntosProcesar[0]);

            double factorX = anchoTotalPt / anchoTotalPx;
            double factorY = altoTotalPt / altoTotalPx;

            List<System.Drawing.Point> listaResultado = new List<System.Drawing.Point>();

            foreach (var p in puntosProcesar)
            {
                // Convertimos a int (esto trunca o redondea los decimales)
                int xFinal = (int)(p.X * factorX);
                int yFinal = (int)(p.Y * factorY);

                listaResultado.Add(new System.Drawing.Point(xFinal, yFinal));
            }

            return listaResultado;
        }



        public string FarmatYoloString(List<System.Drawing.Point> list, int width, int height, int decimals = 6)
        {
            string puntos = string.Empty;
            double x, y, newX, newY;

            foreach (System.Drawing.Point p in list)
            {
                x = p.X;
                y = p.Y;
                newX = Math.Round(x / width, decimals);
                newY = Math.Round(y / height, decimals);
                if (string.IsNullOrEmpty(puntos))
                {
                    puntos = newX.ToString(CultureInfo.InvariantCulture) + " " + newY.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    puntos += " " + newX.ToString(CultureInfo.InvariantCulture) + " " + newY.ToString(CultureInfo.InvariantCulture);
                }
            }
            return puntos;
        }

        private void Gallery_ItemClick(object sender, GalleryItemClickEventArgs e)
        {
            try
            {
                if (listaPaginas.Count > 0)
                {
                    LabelTo0();
                    
                    foreach (var item in listaPaginas)
                    {
                        if (item.pagenumber == Int32.Parse(e.Item.Caption))
                        {
                            paginaActual = item;
                            draw1.LoadImage(item.LocalImgFull);
                            draw2.LoadImage(item.LocalImgFull);


                            draw1.ClearCanva();
                            zonesObservable.Clear();
                            listPagePrecrop = new Consults().GetPagePrecrops(item.PK_Page);

                            ResetListPrecropUI();

                            //if (listPagePrecrop != null)
                            //{
                            //    if (listPagePrecrop.Count > 0)
                            //    {
                            //        foreach (var precrop in listPagePrecrop)
                            //        {
                            //            if (paginaActual != null && docActual != null)
                            //            {

                            //                if (precrop.ProcessbyIA == null)
                            //                {
                            //                    precrop.ProcessbyIA = 0;
                            //                }
                            //                if (precrop.ProcessIAscore == null)
                            //                {
                            //                    precrop.ProcessIAscore = 0;
                            //                }
                            //                Zone zone = new Zone();

                            //                if (precrop.ProcessbyIA == 1)
                            //                {
                            //                    if (cbIA.Checked)
                            //                    {
                            //                        zone = new Zone(
                            //                        precrop.PK_PagePrecrop.ToString(),
                            //                        docActual.Cat, precrop.namelabel,
                            //                        DeserializePoints2d(precrop.Points2D),
                            //                        Color.SkyBlue,
                            //                        (int)precrop.ProcessbyIA,
                            //                        (int)precrop.ProcessIAscore,
                            //                        (int)precrop.precorteOCR, 
                            //                        (int)precrop.FK_ObjectsLayer);
                            //                        zonesObservable.Add(zone);
                            //                    }                                              
                            //                }
                            //                else if (precrop.precorteOCR == 1)
                            //                {
                            //                    if (cbOCR.Checked)
                            //                    {
                            //                        zone = new Zone(
                            //                        precrop.PK_PagePrecrop.ToString(),
                            //                        docActual.Cat, precrop.namelabel,
                            //                        DeserializePoints2d(precrop.Points2D),
                            //                        Color.FromArgb(255, 255, 165, 80),
                            //                        (int)precrop.ProcessbyIA,
                            //                        (int)precrop.ProcessIAscore,
                            //                        (int)precrop.precorteOCR, 
                            //                        (int)precrop.FK_ObjectsLayer);
                            //                        zonesObservable.Add(zone);
                            //                    }                                               
                            //                }
                            //                else
                            //                {
                            //                    zone = new Zone(
                            //                    precrop.PK_PagePrecrop.ToString(),
                            //                    docActual.Cat, precrop.namelabel,
                            //                    DeserializePoints2d(precrop.Points2D),
                            //                    Color.Green,
                            //                    (int)precrop.ProcessbyIA,
                            //                    (int)precrop.ProcessIAscore,
                            //                    (int)precrop.precorteOCR, 
                            //                    (int)precrop.FK_ObjectsLayer);
                            //                    zonesObservable.Add(zone);
                            //                    //counterIA++;
                            //                }

                            //                IVisionDrawSelection.Entities.Zone zoneDraw = new IVisionDrawSelection.Entities.Zone();
                            //                zoneDraw.color = zone.color;
                            //                zoneDraw.id = zone.id;
                            //                zoneDraw.points = zone.points;
                            //                draw2.WriteZone(zoneDraw);


                            //            }
                            //        }

                            //        CounterZonesIAMa();
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.ToString()}");
            }
            
        }

        private void CounterZonesIAMa()
        {
            int counterIA = 0, counterM = 0, counterO = 0;
            foreach (var item in zonesObservable)
            {
                if (item.ProcessbyIA == 1 && item.precorteOCR == 0)
                {
                    counterIA++;
                }
                else if(item.ProcessbyIA == 0 && item.precorteOCR == 0)
                {
                    counterM++;
                }
                else if (item.ProcessbyIA == 0 && item.precorteOCR == 1)
                {
                    counterO++;
                }
            }
            lblCIA.Text = counterIA.ToString();
            lblCM.Text = counterM.ToString();
            lblCO.Text = counterO.ToString();
        }

        private void Gallery_ItemDoubleClick(object sender, GalleryItemClickEventArgs e)
        {           
            
        }

        public static List<System.Drawing.Point> DeserializePoints2d(string json) =>
        (JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>())
        .Select(s => s.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        .Select(a => new System.Drawing.Point(int.Parse(a[0].Trim()), int.Parse(a[1].Trim())))
        .ToList();

        public List<System.Drawing.Point> PolygonStringToPoints(string wkt)
        {
            // 1. Extraer la parte interior de los paréntesis: todo lo que está entre "((" y "))"
            int start = wkt.IndexOf("((") + 2;
            int end = wkt.LastIndexOf("))");
            if (start < 2 || end < 0 || end <= start)
                throw new ArgumentException("WKT no válido.");

            string interior = wkt.Substring(start, end - start);

            // 2. Dividir por comas para obtener cada "X Y"
            string[] parejas = interior
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var puntos = new List<System.Drawing.Point>();
            foreach (var pareja in parejas)
            {
                // 3. Separar X e Y (puede haber múltiples espacios)
                var coords = pareja
                    .Trim()
                    .Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                if (coords.Length != 2
                 || !int.TryParse(coords[0], out int x)
                 || !int.TryParse(coords[1], out int y))
                    throw new FormatException($"Coordenada inválida: '{pareja}'");

                puntos.Add(new System.Drawing.Point(x, y));
            }

            // 4. (Opcional) Si el último punto coincide con el primero, lo quitamos
            if (puntos.Count > 1 && puntos.First().Equals(puntos.Last()))
                puntos.RemoveAt(puntos.Count - 1);

            return puntos;
        }

        private void PrecutDocs_Load(object sender, EventArgs e)
        {
            gridView2.CustomColumnDisplayText += GridView2_CustomColumnDisplayText;
            //draw2.Enabled = true;
            
            gridView1.OptionsView.ShowGroupPanel = false;
            FormatGridControl2();
            dateEdit1.DateTime = new DateTime(2025,12,01);
            galleryControl1.Gallery.FixedImageSize = true;
            galleryControl1.Gallery.ImageSize = new Size(155, 170);
            galleryControl1.Gallery.ShowItemText = true;
            galleryControl1.Gallery.OptionsImageLoad.AsyncLoad = true;
            var gal = galleryControl1.Gallery;
            gal.BeginUpdate();
            gal.Groups.Clear();
            grupoMiniaturas = new GalleryItemGroup();
            gal.Groups.Add(grupoMiniaturas);
            gal.EndUpdate();
            gal.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
            
                       
        }

        private void GridView2_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            // IA: 0/1 -> "No"/"Sí"
            if (e.Column.FieldName == "ProcessbyIA")
            {
                if (e.Value == null || e.Value == DBNull.Value) { e.DisplayText = ""; return; }
                int v;
                if (int.TryParse(Convert.ToString(e.Value), out v))
                    e.DisplayText = (v == 1) ? "Sí" : "No";
                else if (e.Value is bool b)
                    e.DisplayText = b ? "Sí" : "No";
            }

            // Score: 54 -> "54%"
            if (e.Column.FieldName == "ProcessIAscore")
            {
                if (e.Value == null || e.Value == DBNull.Value) { e.DisplayText = ""; return; }
                e.DisplayText = $"{e.Value}%";
            }

            if (e.Column.FieldName == "precorteOCR")
            {
                if (e.Value == null || e.Value == DBNull.Value) { e.DisplayText = ""; return; }
                int v;
                if (int.TryParse(Convert.ToString(e.Value), out v))
                    e.DisplayText = (v == 1) ? "Sí" : "No";
                else if (e.Value is bool b)
                    e.DisplayText = b ? "Sí" : "No";
            }

            // AP: 0/1 -> "No"/"Sí"
            if (e.Column.FieldName == "Aprobado")
            {
                if (e.Value == null || e.Value == DBNull.Value) { e.DisplayText = ""; return; }
                int v;
                if (int.TryParse(Convert.ToString(e.Value), out v))
                    e.DisplayText = (v == 0) ? "No" : "Sí";
                else if (e.Value is bool b)
                    e.DisplayText = b ? "Sí" : "No";
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private bool checkSubcription(int fk_cat)
        {
            try
            {
                obGroupActual = new Consults().GetFkObjectGroup(fk_cat);
                if (obGroupActual != 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }                    
        }

        public static void VaciarCarpeta(string folderTemp)
        {
            if (string.IsNullOrWhiteSpace(folderTemp) || !Directory.Exists(folderTemp))
                return;

            foreach (var file in Directory.EnumerateFiles(folderTemp, "*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    // Quita ReadOnly/Hidden si los tuviera
                    File.SetAttributes(file, FileAttributes.Normal);

                    // Reintentos por archivo en uso/antivirus
                    int intentos = 5;
                    for (int i = 1; i <= intentos; i++)
                    {
                        try
                        {
                            //var trabajo = Task.Run(() => File.Delete(file));

                            //await Task.Run(() => File.Delete(file));
                            File.Delete(file);
                            //await Task.Delay(1000);
                            break; // borrado ok
                        }
                        catch (System.IO.IOException) when (i < intentos)
                        {
                            System.Threading.Thread.Sleep(150 * i);
                        }
                        catch (UnauthorizedAccessException) when (i < intentos)
                        {
                            System.Threading.Thread.Sleep(150 * i);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"No se pudo borrar {file}: {ex.Message}");           
                }
            }
        }


        private void gridControl2_Click(object sender, EventArgs e)
        {
            try
            {
                var main = gridControl2.MainView as ColumnView;
                Zone zone = main.GetRow(main.FocusedRowHandle) as Zone;
                Zone zone2 = new Zone();
                
                foreach (var item in draw2.listZonesPolygons)
                {
                    if (zone.id == item.id)
                    {
                        item.color = Color.Yellow;
                    }
                    else
                    {
                        foreach (var zo in zonesObservable)
                        {
                            if (zo.id == item.id)
                            {
                                zone2 = zo;
                            }
                        }
                        if (zone2.ProcessbyIA == 1)
                        {
                            if (item.color == Color.Yellow)
                            {
                                item.color = Color.SkyBlue;
                            }
                        }
                        else if(zone2.precorteOCR == 1)
                        {
                            if (item.color == Color.Yellow)
                            {
                                item.color = Color.FromArgb(255, 255, 165, 80);
                            }
                        }
                        else
                        {
                            if (item.color == Color.Yellow)
                            {
                                item.color = Color.Green;
                            }
                        }                                        
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void gridControl2_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void tableLayoutPanel2_DoubleClick(object sender, EventArgs e)
        {
            
        }

        
        private System.Drawing.Image LoadFreshImage(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                using (var ms = new MemoryStream(bytes))
                {
                    
                    return new Bitmap(ms);
                    
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public void FormatGridControl2()
        {
            zonesObservable = new ObservableCollection<Zone>();
            gridControl2.DataSource = zonesObservable;           
            gridView2.PopulateColumns();
            gridView2.Columns["id"].Visible = false;
            gridView2.Columns["color"].Visible = false;
            gridView2.Columns["periodico"].Visible = false;
            //gridView2.Columns["precorteOCR"].Visible = false;
            gridView2.Columns["tipoDeCorte"].OptionsColumn.AllowEdit = false;
            gridView2.Columns["tipoDeCorte"].OptionsColumn.FixedWidth = true;
            gridView2.Columns["tipoDeCorte"].OptionsColumn.AllowSize = false;
            gridView2.Columns["tipoDeCorte"].Width = 150;
            gridView2.Columns["tipoDeCorte"].Caption = "Tipo De Corte";
            gridView2.Columns["ProcessbyIA"].OptionsColumn.AllowEdit = false;
            gridView2.Columns["ProcessbyIA"].OptionsColumn.FixedWidth = true;
            gridView2.Columns["ProcessbyIA"].OptionsColumn.AllowSize = false;
            gridView2.Columns["ProcessbyIA"].Width = 50;
            gridView2.Columns["ProcessbyIA"].Caption = "IA";
            gridView2.Columns["ProcessIAscore"].OptionsColumn.AllowEdit = false;
            gridView2.Columns["ProcessIAscore"].OptionsColumn.FixedWidth = true;
            gridView2.Columns["ProcessIAscore"].OptionsColumn.AllowSize = false;
            gridView2.Columns["ProcessIAscore"].Width = 50;
            gridView2.Columns["ProcessIAscore"].Caption = "Score";
            gridView2.Columns["precorteOCR"].OptionsColumn.AllowEdit = false;
            gridView2.Columns["precorteOCR"].OptionsColumn.FixedWidth = true;
            gridView2.Columns["precorteOCR"].OptionsColumn.AllowSize = false;
            gridView2.Columns["precorteOCR"].Width = 50;
            gridView2.Columns["precorteOCR"].Caption = "Ocr";
            gridView2.Columns["Aprobado"].OptionsColumn.AllowEdit = false;
            gridView2.Columns["Aprobado"].OptionsColumn.FixedWidth = true;
            gridView2.Columns["Aprobado"].OptionsColumn.AllowSize = false;
            gridView2.Columns["Aprobado"].Width = 50;
            gridView2.Columns["Aprobado"].Caption = "AP";
        }

        public void DataSetGrid(System.DateTime dateTime)
        {
            List<Docs> listaDocumentos = new Consults().GetDocs(dateTime);
            if (listaDocumentos != null && listaDocumentos.Count > 0)
            {
                gridControl1.DataSource = listaDocumentos;
                gridView1.PopulateColumns();
                gridView1.Columns["Pk_Doc"].Visible = false;
                gridView1.Columns["dateInclude"].Visible = false;
                gridView1.Columns["FK_cat"].Visible = false;
                gridView1.Columns["NItem"].OptionsColumn.AllowEdit = false;
                gridView1.Columns["NItem"].OptionsColumn.FixedWidth = true;
                gridView1.Columns["NItem"].OptionsColumn.AllowSize = false;
                gridView1.Columns["NItem"].Width = 30;
                gridView1.Columns["Cat"].OptionsColumn.AllowEdit = false;
                gridView1.Columns["Cat"].OptionsColumn.FixedWidth = true;
                gridView1.Columns["Cat"].OptionsColumn.AllowSize = false;
                gridView1.Columns["Cat"].Width = 150;
                //gridView1.Columns["TotalPages"].OptionsColumn.AllowEdit = false;
                //gridView1.Columns["TotalPages"].OptionsColumn.FixedWidth = true;
                //gridView1.Columns["TotalPages"].OptionsColumn.AllowSize = false;
                //gridView1.Columns["TotalPages"].Width = 30;
                //gridView1.Columns["TotalPagesIA"].OptionsColumn.AllowEdit = false;
                //gridView1.Columns["TotalPagesIA"].OptionsColumn.FixedWidth = true;
                //gridView1.Columns["TotalPagesIA"].OptionsColumn.AllowSize = false;
                //gridView1.Columns["TotalPagesIA"].Width = 30;
                //gridView1.Columns["TotalPagesOCR"].OptionsColumn.AllowEdit = false;
                //gridView1.Columns["TotalPagesOCR"].OptionsColumn.FixedWidth = true;
                //gridView1.Columns["TotalPagesOCR"].OptionsColumn.AllowSize = false;
                //gridView1.Columns["TotalPagesOCR"].Width = 30;
                //gridView1.Columns["TotalTimePaddle"].OptionsColumn.AllowEdit = false;
                //gridView1.Columns["TotalTimePaddle"].OptionsColumn.FixedWidth = true;
                //gridView1.Columns["TotalTimePaddle"].OptionsColumn.AllowSize = false;
                //gridView1.Columns["TotalTimePaddle"].Width = 120;
                gridView1.Columns["NItem"].Caption = "Nº";
                gridView1.Columns["Cat"].Caption = "Categoria";
                //gridView1.Columns["TotalPages"].Caption = "Pg";
                //gridView1.Columns["TotalPagesIA"].Caption = "IA";
                //gridView1.Columns["TotalPagesOCR"].Caption = "OCR";
                //gridView1.Columns["TotalTimePaddle"].Caption = "TO";
            }
            else
            {
                gridControl1.DataSource = null;
            }
        }

        public static (double widthPct, double heightPct)
        GetMinRectPercentages(IReadOnlyList<PointF> points, double imgWidth, double imgHeight)
        {
            if (points == null || points.Count < 2 || imgWidth <= 0 || imgHeight <= 0)
                return (0, 0);

            // --- Ordenar y quitar duplicados exactos sin usar Index-from-end ---
            var pts = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            var uniq = new List<PointF>(pts.Count);
            for (int i = 0; i < pts.Count; i++)
            {
                if (i == 0 || pts[i].X != pts[i - 1].X || pts[i].Y != pts[i - 1].Y)
                    uniq.Add(pts[i]);
            }

            if (uniq.Count == 1) return (0, 0);
            if (uniq.Count == 2)
            {
                var a = uniq[0]; var b = uniq[1];
                double len = Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
                return (len / imgWidth * 100.0, 0.0);
            }

            // --- Envolvente convexa (Andrew monotone chain) sin ^ ---
            var lower = new List<PointF>();
            for (int i = 0; i < uniq.Count; i++)
            {
                var p = uniq[i];
                while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                    lower.RemoveAt(lower.Count - 1);
                lower.Add(p);
            }

            var upper = new List<PointF>();
            for (int i = uniq.Count - 1; i >= 0; i--)
            {
                var p = uniq[i];
                while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                    upper.RemoveAt(upper.Count - 1);
                upper.Add(p);
            }

            // Combinar sin repetir extremos
            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);
            var hull = lower;
            int h = hull.Count;

            if (h == 2) // colineal
            {
                var a = hull[0]; var b = hull[1];
                double len = Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
                return (len / imgWidth * 100.0, 0.0);
            }

            // --- Rectángulo mínimo exacto probando cada arista del hull ---
            double bestArea = double.PositiveInfinity;
            double bestW = 0, bestH = 0;

            for (int i = 0; i < h; i++)
            {
                var p0 = hull[i];
                var p1 = hull[(i + 1) % h];
                double dx = p1.X - p0.X, dy = p1.Y - p0.Y;
                double theta = Math.Atan2(dy, dx);
                double cos = Math.Cos(theta), sin = Math.Sin(theta);

                double minX = double.PositiveInfinity, maxX = double.NegativeInfinity;
                double minY = double.PositiveInfinity, maxY = double.NegativeInfinity;

                // Rotación por -theta (alinear arista con eje X)
                for (int k = 0; k < h; k++)
                {
                    var p = hull[k];
                    double rx = p.X * cos + p.Y * sin;
                    double ry = -p.X * sin + p.Y * cos;

                    if (rx < minX) minX = rx;
                    if (rx > maxX) maxX = rx;
                    if (ry < minY) minY = ry;
                    if (ry > maxY) maxY = ry;
                }

                double w = Math.Max(0, maxX - minX);
                double hgt = Math.Max(0, maxY - minY);
                double area = w * hgt;

                if (area < bestArea - 1e-9)
                {
                    bestArea = area;
                    bestW = w;
                    bestH = hgt;
                }
            }

            double widthPct = bestW / imgWidth * 100.0;
            double heightPct = bestH / imgHeight * 100.0;
            return (widthPct, heightPct);

            
        }

        private static double Cross(PointF o, PointF a, PointF b)
            => (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);

        private async void OnControlClosed(string reason)
        {
            await Task.Run(() => ForceDelete(folderTemp));
        }

        public static void ForceDelete(string ruta)
        {
            try
            {
                // Fuerza con el comando de Windows (ignora ReadOnly, etc.)
                var psi = new ProcessStartInfo("cmd.exe", $"/c rmdir /s /q \"{ruta}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                p.WaitForExit();
            }
            catch (Exception)
            {

            }
            
        }

        private void dateEdit1_EditValueChanged(object sender, EventArgs e)
        {
            //LabelTo0();
            DataSetGrid(dateEdit1.DateTime);
            //ClearDrawsGrid2AndGallery();                            
        }

        private void ClearDrawsGrid2AndGallery()
        {
            if (draw1.panelChildren.BackgroundImage != null)
            {
                draw1.panelChildren.BackgroundImage.Dispose();
                draw1.panelChildren.BackgroundImage = null;
                draw1.ClearCanva();
            }
            if (draw2.panelChildren.BackgroundImage != null)
            {
                draw2.panelChildren.BackgroundImage.Dispose();
                draw2.panelChildren.BackgroundImage = null;
                draw2.ClearCanva();
            }
            zonesObservable.Clear();
            if (grupoMiniaturas != null)
            {
                foreach (GalleryItem oldItem in grupoMiniaturas.Items)
                {
                    oldItem.Image?.Dispose();
                }
                grupoMiniaturas.Items.Clear();
            }
        }

        private void galleryControl1_Click(object sender, EventArgs e)
        {

        }

        public static string GetDataXml(string nodo)
        {
            string xmlFilePath = (System.Reflection.Assembly.GetExecutingAssembly().Location).Replace("IVisionPrecutDocs.dll", "") + "conexion.xml";
            XmlDocument xmlDoc = new XmlDocument();
            string data = "";
            xmlDoc.Load(xmlFilePath);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNode conexionNode = root.SelectSingleNode("config");
            if (conexionNode != null)
            {
                XmlNode datoNode = conexionNode.SelectSingleNode(nodo);
                if (datoNode != null)
                {
                    data = datoNode.InnerText;
                }

            }
            return data;
        }

        private void sbtnLabels_Click(object sender, EventArgs e)
        {
            if (obGroupActual !=0 && docActual != null)
            {
                LabelsForm formLabel = new LabelsForm(this);
                formLabel.FK_cat = docActual.FK_cat;
                formLabel.obGroupActual = obGroupActual;
                formLabel.checkBox1.Checked = false;
                formLabel.Show();
                
            }
            else
            {
                MessageBox.Show("Seleccionar un periodico de su preferencia");
            }
            
        }

        private void sbtnUpdate_Click(object sender, EventArgs e)
        {
            LabelTo0();
            DataSetGrid(dateEdit1.DateTime);
            ClearDrawsGrid2AndGallery();
        }

        private void LabelTo0()
        {
            lblCIA.Text = "0";
            lblCM.Text = "0";
            lblCO.Text = "0";
        }

        private async void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                LabelTo0();
                ClearDrawsGrid2AndGallery();
                var main = gridControl1.MainView as ColumnView;
                Docs doc = main.GetRow(main.FocusedRowHandle) as Docs;
                docActual = doc;
                if (!checkSubcription(docActual.FK_cat))
                {
                    MessageBox.Show("No esta suscrito en objetos");
                }
                else
                {
                

                    TimeSpan timeout = TimeSpan.FromMinutes(3);
                    splashScreenManager1.ShowWaitForm();
                    Task descarga = Task.Run(() =>
                    {

                        if (doc != null && doc.Pk_Doc > 0)
                        {
                            listaPaginas = new Consults().GetPagesByFk(doc.Pk_Doc);
                            if (listaPaginas != null && listaPaginas.Count > 0)
                            {
                                Pro pr = new Pro();
                                pr.DescargarPaginas(listaPaginas, folderTemp);
                                //splashScreenManager1.CloseWaitForm();                               
                            }
                        }
                    });
                    var ganador = await Task.WhenAny(descarga, Task.Delay(timeout));
                    splashScreenManager1.CloseWaitForm();
                    if (ganador != descarga)
                    {
                        throw new TimeoutException($"La descarga superó {timeout.TotalMinutes} minutos de descaga.");
                    }
                    else
                    {
                        if (listaPaginas != null && listaPaginas.Count > 0)
                        {
                            foreach (var item in listaPaginas.AsEnumerable().Reverse())
                            {
                                if (File.Exists(item.LocalMini))
                                {
                                    grupoMiniaturas.Items.Add(new GalleryItem(LoadFreshImage(item.LocalMini), item.pagenumber.ToString(), ""));

                                }
                                else
                                {
                                    grupoMiniaturas.Items.Add(new GalleryItem(LoadFreshImage(Path.Combine(folderTemp, item.fileNameMiniJPG)), item.pagenumber.ToString(), ""));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
        }

        private void cbIA_CheckedChanged(object sender, EventArgs e)
        {
            ResetListPrecropUI();
            //if (listPagePrecrop != null)
            //{
            //    if (listPagePrecrop.Count > 0)
            //    {
            //        draw2.ClearCanva();
            //        zonesObservable.Clear();
            //        foreach (var precrop in listPagePrecrop)
            //        {
            //            if (paginaActual != null && docActual != null)
            //            {

            //                if (precrop.ProcessbyIA == null)
            //                {
            //                    precrop.ProcessbyIA = 0;
            //                }
            //                if (precrop.ProcessIAscore == null)
            //                {
            //                    precrop.ProcessIAscore = 0;
            //                }
            //                Zone zone = new Zone();

            //                if (precrop.ProcessbyIA == 1)
            //                {
            //                    if (cbIA.Checked)
            //                    {
            //                        zone = new Zone(
            //                        precrop.PK_PagePrecrop.ToString(),
            //                        docActual.Cat, precrop.namelabel,
            //                        DeserializePoints2d(precrop.Points2D),
            //                        Color.SkyBlue,
            //                        (int)precrop.ProcessbyIA,
            //                        (int)precrop.ProcessIAscore,
            //                        (int)precrop.precorteOCR,
            //                        (int)precrop.FK_ObjectsLayer);    ;
            //                        zonesObservable.Add(zone);
            //                    }
            //                }
            //                else if (precrop.precorteOCR == 1)
            //                {
            //                    if (cbOCR.Checked)
            //                    {
            //                        zone = new Zone(
            //                        precrop.PK_PagePrecrop.ToString(),
            //                        docActual.Cat, precrop.namelabel,
            //                        DeserializePoints2d(precrop.Points2D),
            //                        Color.FromArgb(255, 255, 165, 80),
            //                        (int)precrop.ProcessbyIA,
            //                        (int)precrop.ProcessIAscore,
            //                        (int)precrop.precorteOCR,
            //                        (int)precrop.FK_ObjectsLayer);
            //                        zonesObservable.Add(zone);
            //                    }
            //                }
            //                else
            //                {
            //                    zone = new Zone(
            //                    precrop.PK_PagePrecrop.ToString(),
            //                    docActual.Cat, precrop.namelabel,
            //                    DeserializePoints2d(precrop.Points2D),
            //                    Color.Green,
            //                    (int)precrop.ProcessbyIA,
            //                    (int)precrop.ProcessIAscore,
            //                    (int)precrop.precorteOCR,
            //                    (int)precrop.FK_ObjectsLayer);
            //                    zonesObservable.Add(zone);
            //                    //counterIA++;
            //                }

            //                IVisionDrawSelection.Entities.Zone zoneDraw = new IVisionDrawSelection.Entities.Zone();
            //                zoneDraw.color = zone.color;
            //                zoneDraw.id = zone.id;
            //                zoneDraw.points = zone.points;
            //                draw2.WriteZone(zoneDraw);


            //            }
            //        }

            //        CounterZonesIAMa();
            //    }
            //}
        }

        private void cbOCR_CheckedChanged(object sender, EventArgs e)
        {
            ResetListPrecropUI();
        }

        private void cbVal_CheckedChanged(object sender, EventArgs e)
        {
            ResetListPrecropUI();
        }

        private void ResetListPrecropUI()
        {
            if (listPagePrecrop != null)
            {

                if (listPagePrecrop.Count > 0)
                {
                    draw2.ClearCanva();
                    zonesObservable.Clear();
                    foreach (var precrop in listPagePrecrop)
                    {
                        //if (cbVal.Checked)
                        //{
                        //    if (precrop.PrecropPointsValidation != 1)
                        //    {
                        //        continue;
                        //    }

                        //}
                        if (paginaActual != null && docActual != null)
                        {

                            if (precrop.ProcessbyIA == null)
                            {
                                precrop.ProcessbyIA = 0;
                            }
                            if (precrop.ProcessIAscore == null)
                            {
                                precrop.ProcessIAscore = 0;
                            }
                            Zone zone = new Zone();

                            if (precrop.ProcessbyIA == 1)
                            {
                                if (cbIA.Checked)
                                {
                                    zone = new Zone(
                                    precrop.PK_PagePrecrop.ToString(),
                                    docActual.Cat, precrop.namelabel,
                                    DeserializePoints2d(precrop.Points2D),
                                    Color.SkyBlue,
                                    (int)precrop.ProcessbyIA,
                                    (int)precrop.ProcessIAscore,
                                    (int)precrop.precorteOCR,
                                    (int)precrop.FK_ObjectsLayer);
                                    zonesObservable.Add(zone);
                                    AddDraw2Zone(zone);
                                }
                            }
                            else if (precrop.precorteOCR == 1)
                            {
                                //if (cbOCR.Checked)
                                //{
                                //    zone = new Zone(
                                //    precrop.PK_PagePrecrop.ToString(),
                                //    docActual.Cat, precrop.namelabel,
                                //    DeserializePoints2d(precrop.Points2D),
                                //    Color.FromArgb(255, 255, 165, 80),
                                //    (int)precrop.ProcessbyIA,
                                //    (int)precrop.ProcessIAscore,
                                //    (int)precrop.precorteOCR,
                                //    (int)precrop.FK_ObjectsLayer);
                                //    zonesObservable.Add(zone);
                                //    AddDraw2Zone(zone);
                                //}
                            }
                            else
                            {
                                zone = new Zone(
                                precrop.PK_PagePrecrop.ToString(),
                                docActual.Cat, precrop.namelabel,
                                DeserializePoints2d(precrop.Points2D),
                                Color.Green,
                                (int)precrop.ProcessbyIA,
                                (int)precrop.ProcessIAscore,
                                (int)precrop.precorteOCR,
                                (int)precrop.FK_ObjectsLayer);
                                zonesObservable.Add(zone);
                                AddDraw2Zone(zone);
                                //counterIA++;
                            }

                            
                        }
                    }

                    CounterZonesIAMa();
                }
            }
        }

        private void AddDraw2Zone(Zone zone)
        {
            IVisionDrawSelection.Entities.Zone zoneDraw = new IVisionDrawSelection.Entities.Zone();
            zoneDraw.color = zone.color;
            zoneDraw.id = zone.id;
            zoneDraw.points = zone.points;
            draw2.WriteZone(zoneDraw);
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            AdminDocs ad = new AdminDocs();
            ad.Show();
        }
    }
}
