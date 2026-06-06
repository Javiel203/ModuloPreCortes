using DevExpress.XtraGrid;
using IVisionPrecutDocs.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IVisionPrecutDocs.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraBars.Ribbon;
using IVisionPrecutDocs.Proceso;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraGrid.Views.Grid;
using System.Threading;
using IVisionDrawSelection;
using DevExpress.Office.Utils;
using DevExpress.XtraBars;
using DevExpress.Data.Filtering;
using DevExpress.XtraCharts;
using System.Collections;
using System.Text.Json;

namespace IVisionPrecutDocs
{
    public partial class LabelsForm : Form
    {
        private ObservableCollection<LabelDoc> labelsObservable;
        public int FK_cat { get; set; } = 0;
        public int obGroupActual { get; set; } = 0;
        public List<Point> points { get; set; }
        public double widthpct { get; set; }
        public double heightpct { get; set; }
        PrecutDocs precutDocs;
        private GalleryItemGroup grupoMiniaturas;
        private static string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string folderTemp = Path.Combine(baseDir, "tmp\\Precortes");
        private readonly ContextMenuStrip rowMenu = new ContextMenuStrip();
        private readonly ContextMenuStrip rowMenuGallery = new ContextMenuStrip();     
        List<PagePrecrop> pagePrecropList = new List<PagePrecrop>();

        public LabelsForm(PrecutDocs pdocs)
        {
            InitializeComponent();
            this.Load += LabelsForm_Load;
            this.FormClosing += LabelsForm_FormClosing;
            chartControl1.RuntimeHitTesting = true;
            precutDocs = pdocs;           
            ToolStripMenuItem mVer = new ToolStripMenuItem("Ver Detalles", null, OnVerClick);
            ToolStripMenuItem mMove = new ToolStripMenuItem("Mover", null, OnMoveClick);
            ToolStripMenuItem mApp = new ToolStripMenuItem("Aprobar", null, OnApprove);
            ToolStripMenuItem mEditar = new ToolStripMenuItem("Editar", null, OnEditarClick);
            ToolStripMenuItem mEliminar = new ToolStripMenuItem("Eliminar", null, OnEliminarClick);
            rowMenu.Items.AddRange(new ToolStripItem[] {mEditar, mEliminar, new ToolStripSeparator()});
            gridView1.PopupMenuShowing += GridView1_PopupMenuShowing;
            rowMenuGallery.Items.AddRange(new ToolStripItem[] { mVer, mMove, mApp, new ToolStripSeparator() });
            galleryControl1.ContextMenuStrip = rowMenuGallery;
            var gal = galleryControl1.Gallery;
            gal.ItemCheckMode = DevExpress.XtraBars.Ribbon.Gallery.ItemCheckMode.SingleRadio;
            galleryControl1.MouseUp += GalleryControl1_MouseUp;
        }



        private void GalleryControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            RibbonHitInfo hi = galleryControl1.CalcHitInfo(e.Location);
            if (!hi.InGalleryItem || hi.GalleryItem == null) return;
            //galleryControl1.Gallery.FocusedItem = hi.GalleryItem;
            rowMenuGallery.Tag = hi.GalleryItem;
            rowMenuGallery.Show(galleryControl1, e.Location);
            //popupMenuGallery.Tag = hi.GalleryItem;
            //popupMenuGallery.ShowPopup(new Point(10,10));

        }

        private void BtnVer_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private void LabelsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                precutDocs.draw1.ClearCanva();
                
            }
            catch (Exception)
            {

            }
            
        }

        private void GridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow || e.HitInfo.InRowCell)
            {
                var view = (GridView)sender;
                // Enfocar la fila bajo el mouse (muy importante)
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                // Cancelar el menú por defecto de DevExpress
                e.Allow = false;
                // Mostrar tu ContextMenuStrip en la posición del clic
                rowMenu.Show(gridControl1, e.Point);
            }
        }

        private void OnVerClick(object sender, EventArgs e)
        {
            try
            {
                GalleryItem gi = rowMenuGallery.Tag as GalleryItem;
                if (gi == null) return;
                var group = gi.GalleryGroup
                     ?? galleryControl1.Gallery.Groups
                          .Cast<GalleryItemGroup>()
                          .FirstOrDefault(g => g.Items.Contains(gi));
                int index = group?.Items.IndexOf(gi) ?? -1;
                DetailsPrecut formDetail = new DetailsPrecut((int)pagePrecropList[index].PK_PagePrecrop);
                formDetail.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Error para ver el precorte");
            }
        }

        private void OnApprove(object sender, EventArgs e)
        {
            GalleryItem gi = rowMenuGallery.Tag as GalleryItem;
            if (gi == null) return;
            var group = gi.GalleryGroup
                 ?? galleryControl1.Gallery.Groups
                      .Cast<GalleryItemGroup>()
                      .FirstOrDefault(g => g.Items.Contains(gi));
            int index = group?.Items.IndexOf(gi) ?? -1;
         

            PagePrecrop pp = new Consults().GetPagePrecropById((int)pagePrecropList[index].PK_PagePrecrop);
            Paginas p = new Consults().GetPage(pp.FK_Page);
            LabelDoc labelDoc = new Consults().GetLabelDocByPK((int)pp.FK_LabelDoc);

            //int idObjLayer = new Pro().ProInsertObjectsLayers(obGroupActual,
            //labelDoc.FK_ObjectMaster,
            //pp.PolyTextYoloFormat,
            //pp.Points2D,
            //p.LocalImgFull,
            //ConvertirStringAPuntos(pp.Points2D));

            //if (idObjLayer > 0)
            //{
            //    if (new Updats().UpdatePagePrecropFkObjLayer((int)pp.PK_PagePrecrop, idObjLayer))
            //    {
            //        //zone.Aprobado = 1;
            //        MessageBox.Show("objeto agregado correctamente");
            //    }
            //    else
            //    {
            //        MessageBox.Show("No se actualizo en la Tabla PagePrecrop");
            //    }
            //}
            //else
            //{

            //    MessageBox.Show("No se incerto el objeto");
            //}
        }

        public List<Point> ConvertirStringAPuntos(string textoCoordenadas)
        {
            List<Point> listaPuntos = new List<Point>();

            try
            {
                // 1. Interpreta tu string como un arreglo JSON y lo pasa a una lista de C#
                List<string> coordenadas = JsonSerializer.Deserialize<List<string>>(textoCoordenadas);

                if (coordenadas != null)
                {
                    // 2. Recorre cada elemento 
                    foreach (string coord in coordenadas)
                    {
                        string[] partes = coord.Split(',');

                        if (partes.Length == 2)
                        {
                            // Limpia los espacios y convierte a enteros
                            int x = int.Parse(partes[0].Trim());
                            int y = int.Parse(partes[1].Trim());

                            listaPuntos.Add(new Point(x, y));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Por si el string llega mal formado o vacío
                Console.WriteLine($"Error al procesar el texto: {ex.Message}");
            }

            return listaPuntos;
        }

        private void OnMoveClick(object sender, EventArgs e)
        {
            GalleryItem gi = rowMenuGallery.Tag as GalleryItem;
            if (gi == null) return;
            var group = gi.GalleryGroup
                 ?? galleryControl1.Gallery.Groups
                      .Cast<GalleryItemGroup>()
                      .FirstOrDefault(g => g.Items.Contains(gi));
            int index = group?.Items.IndexOf(gi) ?? -1;

            XFormMove frmMove = new XFormMove();
            frmMove.FK_cat = FK_cat;
            frmMove.FK_ObjGroup = precutDocs.obGroupActual;
            frmMove.pk_PagePrecrop = (int)pagePrecropList[index].PK_PagePrecrop;
            frmMove.Show();
            frmMove.simpleButton1.Click += SimpleButton1_Click;
        }

        private void SimpleButton1_Click(object sender, EventArgs e)
        {
            gridControl1_Click(null, null);
        }

        private void OnEditarClick(object sender, EventArgs e)
        {
            var main = gridControl1.MainView as ColumnView;
            LabelDoc ldoc = main.GetRow(main.FocusedRowHandle) as LabelDoc;
            
            object r = XtraInputBox.Show(
                "Ingrese el nombre:",
                "Editar Label",
                defaultResponse: ldoc.namelabel
            );
            if (r != null) // OK
            {
                if (new Updats().UpdateNameLabel(ldoc.PK_LabelDoc,r.ToString()))
                {
                    if(!new Updats().UpdateNameObjectMaster(ldoc.FK_ObjectMaster, r.ToString()))
                    {
                        MessageBox.Show("Tabla ObjectMaster no actualizado");
                    }
                    
                    UpdateGridLabels();
                }
                else
                {
                    MessageBox.Show("Error al conectar con la BD");
                }                             
            }
        }

        private void OnEliminarClick(object sender, EventArgs e)
        {
            try
            {
                var main = gridControl1.MainView as ColumnView;
                LabelDoc ldoc = main.GetRow(main.FocusedRowHandle) as LabelDoc;

                var dr = MessageBox.Show(
                    this,                 // owner: asegura que salga al frente (en Form) 
                    $"Seguro de eliminar el Label {ldoc.namelabel}",
                    "Eliminar Label",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2   // por defecto queda en "No"
                    );
                if (dr == DialogResult.Yes)
                {
                    splashScreenManager1.ShowWaitForm();
                    List<PagePrecrop> list = new List<PagePrecrop>();

                    if (ldoc != null && ldoc.PK_LabelDoc > 0)
                    {
                        list = new Consults().GetPagePrecropsByFk(ldoc.PK_LabelDoc);
                    }
                    if (list.Count > 0)
                    {
                        foreach (PagePrecrop precrop in list)
                        {
                            new Pro().DeletePreCor((int)precrop.PK_PagePrecrop);
                        }
                    }
                    new Delete().DeleteLabelDocById(ldoc.PK_LabelDoc);
                    if (!new Updats().UpdateActiveObjectMaster(ldoc.FK_ObjectMaster))
                    {
                        MessageBox.Show("No se elimino en Object Master");
                    }
                    checkBox1_CheckedChanged(null, null);
                    
                    splashScreenManager1.CloseWaitForm();
                    //lblTotal.Text = list.Count.ToString();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error al eliminar el Label");
                
            }
            
         
        }

        private void LabelsForm_Load(object sender, EventArgs e)
        {
            comboBoxEdit1.EditValue = precutDocs.lblWitdth;
            comboBoxEdit2.EditValue = precutDocs.lblHeight;
            cbeFilterPreco.EditValue = 6;
            cbeFilterImg.SelectedIndex = precutDocs.SelectIndexCbFiletLabelForms;
            cbeFilterImg.SelectedIndexChanged += CbeFilterImg_SelectedIndexChanged;

            List<LabelDoc> list;
            if (FK_cat != 0 && widthpct != 0 && heightpct != 0 )
            {
                
                if (checkBox1.Checked)
                {
                    list = new Consults().GetLabelDocsWithinRange(
                        FK_cat, 
                        widthpct,
                        heightpct, 
                        Convert.ToInt32(comboBoxEdit1.EditValue),
                        Convert.ToInt32(comboBoxEdit2.EditValue),
                        precutDocs.obGroupActual);
                }
                else
                {
                    list = new Consults().GetLebelsDoc(FK_cat, precutDocs.obGroupActual);
                }

                labelsObservable = new ObservableCollection<LabelDoc>(list);
                FormatGridControl1();
                textBox2.Text = $"_W{Math.Round(widthpct)}H{Math.Round(heightpct)}";
            }
            else
            {
                
                list = new Consults().GetLebelsDoc(FK_cat, precutDocs.obGroupActual);
                labelsObservable = new ObservableCollection<LabelDoc>(list);
                FormatGridControl1();
                textBox1.Enabled = true;
                textBox2.Enabled = false;
                sbtnSelect.Enabled = false;
                comboBoxEdit1.Enabled = false;
                comboBoxEdit2.Enabled = false;
                checkBox1.Enabled = false;
                btnAgregar.Enabled = false;
            }

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
            lblTotal.Text = list.Count.ToString();
            StyleChartControl1();
            //this.WindowState = FormWindowState.Maximized;
        }

        private void CbeFilterImg_SelectedIndexChanged(object sender, EventArgs e)
        {
            precutDocs.SelectIndexCbFiletLabelForms = cbeFilterImg.SelectedIndex;
            gridControl1_Click(null, null);
        }

        public void FormatGridControl1()
        {
            //labelsObservable = new ObservableCollection<LabelDoc>();
            gridControl1.DataSource = labelsObservable;
            ////gridView2.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
            gridView1.PopulateColumns();
            gridView1.Columns["PK_LabelDoc"].Visible = false;
            gridView1.Columns["FK_doc"].Visible = false;
            gridView1.Columns["FK_ObjectMaster"].Visible = false;
            gridView1.Columns["FK_ObjectGroup"].Visible = false;
            gridView1.Columns["namelabel"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["namelabel"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["namelabel"].OptionsColumn.AllowSize = false;
            gridView1.Columns["namelabel"].Width = 250;
            gridView1.Columns["namelabel"].Caption = "Nombre";
            gridView1.Columns["widthPct"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["widthPct"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["widthPct"].OptionsColumn.AllowSize = false;
            gridView1.Columns["widthPct"].Width = 70;
            gridView1.Columns["widthPct"].Caption = "W%";
            gridView1.Columns["heightPct"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["heightPct"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["heightPct"].OptionsColumn.AllowSize = false;
            gridView1.Columns["heightPct"].Width = 70;
            gridView1.Columns["heightPct"].Caption = "H%";
            gridView1.Columns["CatImgIa"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["CatImgIa"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["CatImgIa"].OptionsColumn.AllowSize = false;
            gridView1.Columns["CatImgIa"].Width = 70;
            gridView1.Columns["CatImgIa"].Caption = "IMGIA";
            gridView1.Columns["CatImgNoIa"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["CatImgNoIa"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["CatImgNoIa"].OptionsColumn.AllowSize = false;
            gridView1.Columns["CatImgNoIa"].Width = 70;
            gridView1.Columns["CatImgNoIa"].Caption = "IMGNoIA";
            gridView1.Columns["CatImgOcr"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["CatImgOcr"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["CatImgOcr"].OptionsColumn.AllowSize = false;
            gridView1.Columns["CatImgOcr"].Width = 70;
            gridView1.Columns["CatImgOcr"].Caption = "IMGOcr";
            gridView1.Columns["CatApproved"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["CatApproved"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["CatApproved"].OptionsColumn.AllowSize = false;
            gridView1.Columns["CatApproved"].Width = 70;
            gridView1.Columns["CatApproved"].Caption = "AP";
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    int idObjMaster = new Inserts().InsertObjetcMaster($"{textBox1.Text}{textBox2.Text}", "", precutDocs.obGroupActual);
                    int id = new Inserts().InsertLabelDoc(FK_cat, $"{textBox1.Text}{textBox2.Text}", widthpct, heightpct, idObjMaster, precutDocs.obGroupActual);
                    if (id != 0)
                    {
                        int idObjLayer = new Pro().ProInsertObjectsLayers(
                            precutDocs.obGroupActual, 
                            idObjMaster, 
                            precutDocs.FarmatYoloString(points, precutDocs.draw1.WidthPx, precutDocs.draw1.HeightPx, 8),
                            precutDocs.Points2d(points)
                            ,precutDocs.paginaActual.LocalImgFull,
                            points);
                        if (idObjLayer != 0)
                        {
                            precutDocs.InsertPrecort(points, id, $"{textBox1.Text}{textBox2.Text}", widthpct, heightpct, idObjLayer);
                            this.Close();
                            this.Dispose();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Error a insertar dato");
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error a insertar dato");
            }
                   
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            //try
            //{
            //    var main = gridControl1.MainView as ColumnView;
            //    LabelDoc ldoc = main.GetRow(main.FocusedRowHandle) as LabelDoc;

            //    int idObjLayer = new Pro().ProInsertObjectsLayers(precutDocs.obGroupActual,
            //        ldoc.FK_ObjectMaster,
            //        precutDocs.FarmatYoloString(points, precutDocs.draw1.WidthPx, precutDocs.draw1.HeightPx, 8),
            //        precutDocs.Points2d(points),
            //        precutDocs.paginaActual.LocalImgFull,
            //        points);
            //    if (idObjLayer != 0)
            //    {
            //        precutDocs.InsertPrecort(points, ldoc.PK_LabelDoc, ldoc.namelabel, widthpct, heightpct, idObjLayer);
            //        this.Close();
            //        this.Dispose();
            //    }
            //    else
            //    {
            //        MessageBox.Show("Error a insertar dato");
            //    }


            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Error a insertar dato");
            //}

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            List<LabelDoc> list;
            if (checkBox1.Checked)
            {
                list = 
                    new Consults().GetLabelDocsWithinRange(
                        FK_cat,
                        widthpct,
                        heightpct,
                        Convert.ToInt32(comboBoxEdit1.EditValue),
                        Convert.ToInt32(comboBoxEdit2.EditValue),
                        precutDocs.obGroupActual);
            }
            else
            {
                list = new Consults().GetLebelsDoc(FK_cat, precutDocs.obGroupActual);
            }
            labelsObservable = new ObservableCollection<LabelDoc>(list);
            lblTotal.Text = list.Count.ToString();
            gridControl1.DataSource = labelsObservable;
        }

        private void comboBoxEdit1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                List<LabelDoc> list;
                precutDocs.lblWitdth = Convert.ToInt32(comboBoxEdit1.EditValue);
                if (checkBox1.Checked)
                {
                    list =
                        new Consults().GetLabelDocsWithinRange(
                            FK_cat,
                            widthpct,
                            heightpct,
                            Convert.ToInt32(comboBoxEdit1.EditValue),
                            Convert.ToInt32(comboBoxEdit2.EditValue),
                            precutDocs.obGroupActual);
                    labelsObservable = new ObservableCollection<LabelDoc>(list);
                    gridControl1.DataSource = labelsObservable;
                    lblTotal.Text = list.Count.ToString();
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Error al cargar Labels");
            }
                 
        }

        private void comboBoxEdit2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                List<LabelDoc> list;
                precutDocs.lblHeight = Convert.ToInt32(comboBoxEdit2.EditValue);
                if (checkBox1.Checked)
                {
                    list =
                        new Consults().GetLabelDocsWithinRange(
                            FK_cat,
                            widthpct,
                            heightpct,
                            Convert.ToInt32(comboBoxEdit1.EditValue),
                            Convert.ToInt32(comboBoxEdit2.EditValue),
                            precutDocs.obGroupActual);
                    labelsObservable = new ObservableCollection<LabelDoc>(list);
                    gridControl1.DataSource = labelsObservable;
                    lblTotal.Text = list.Count.ToString();
                }
            }
            catch (Exception)
            {
                          
            }
            
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                splashScreenManager1.ShowWaitForm();

                //VaciarCarpeta(folderTemp);

                foreach (GalleryItem oldItem in grupoMiniaturas.Items)
                {
                    oldItem.Image?.Dispose();
                }
                grupoMiniaturas.Items.Clear();

                var main = gridControl1.MainView as ColumnView;
                LabelDoc ldoc = main.GetRow(main.FocusedRowHandle) as LabelDoc;
                //pagePrecropList = new List<PagePrecrop>();
                pagePrecropList.Clear();

                if (ldoc != null && ldoc.PK_LabelDoc > 0)
                {
                    if (cbeFilterPreco.EditValue.ToString() == "Todo")
                    {
                        pagePrecropList = new Consults().GetFilterPagePrecropsByFk(ldoc.PK_LabelDoc,0,cbeFilterImg.SelectedIndex);
                    }
                    else
                    {
                        pagePrecropList = new Consults().GetFilterPagePrecropsByFk(ldoc.PK_LabelDoc, Convert.ToInt32(cbeFilterPreco.EditValue), cbeFilterImg.SelectedIndex);
                    }
                    
                }
                if (pagePrecropList.Count > 0 && pagePrecropList != null)
                {

                    Pro pr = new Pro();
                    pr.DescargarPrecortes(pagePrecropList, folderTemp);

                    foreach (var item in pagePrecropList.AsEnumerable())
                    {
                        if (File.Exists(item.localmage))
                        {
                            Image image = LoadFreshImage(item.localmage);
                            if (image != null)
                            {
                                grupoMiniaturas.Items.Add(new GalleryItem(LoadFreshImage(item.localmage), "", ""));
                                
                            }
                        }
                        else
                        {
                            //grupoMiniaturas.Items.Add(new GalleryItem(LoadFreshImage(Path.Combine(folderTemp, item.localmage)), "", ""));
                        }
                    }                  
                }
                lblTimg.Text = pagePrecropList.Count.ToString();
                splashScreenManager1.CloseWaitForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.ToString()}");
            }
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

        private void UpdateGridLabels()
        {
            List<LabelDoc> list;
            if (checkBox1.Checked)
            {
                list =
                    new Consults().GetLabelDocsWithinRange(
                        FK_cat,
                        widthpct,
                        heightpct,
                        Convert.ToInt32(comboBoxEdit1.EditValue),
                        Convert.ToInt32(comboBoxEdit2.EditValue),
                        precutDocs.obGroupActual);
            }
            else
            {
                list = new Consults().GetLebelsDoc(FK_cat, precutDocs.obGroupActual);
            }
            labelsObservable = new ObservableCollection<LabelDoc>(list);
            gridControl1.DataSource = labelsObservable;
        }

        public static async void VaciarCarpeta(string folderTemp)
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
                            await Task.Run(() => File.Delete(file));
                            break; // borrado ok
                        }
                        catch (IOException) when (i < intentos)
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
                    // Si quieres ver el archivo problemático:
                    // MessageBox.Show($"No se pudo borrar:\n{file}\n\n{ex}");
                }
            }
        }

        private void sbtnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                var main = gridControl1.MainView as ColumnView;
                LabelDoc ldoc = main.GetRow(main.FocusedRowHandle) as LabelDoc;

                int idObjLayer = new Pro().ProInsertObjectsLayers(precutDocs.obGroupActual,
                    ldoc.FK_ObjectMaster,
                    precutDocs.FarmatYoloString(points, precutDocs.draw1.WidthPx, precutDocs.draw1.HeightPx, 8),
                    precutDocs.Points2d(points),
                    precutDocs.paginaActual.LocalImgFull,
                    points);
                if (idObjLayer != 0)
                {
                    precutDocs.InsertPrecort(points, ldoc.PK_LabelDoc, ldoc.namelabel, widthpct, heightpct, idObjLayer);
                    this.Close();
                    this.Dispose();
                }
                else
                {
                    MessageBox.Show("Error a insertar dato");
                }


            }
            catch (Exception)
            {
                MessageBox.Show("Error a insertar dato");
            }
        }

        private void cbeFilterPreco_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridControl1_Click(null, null);
        }

        private void cbeFilterImg_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void sbtnActualizar_Click(object sender, EventArgs e)
        {
            List<LabelDoc> list;
            if (FK_cat != 0 && widthpct != 0 && heightpct != 0)
            {

                if (checkBox1.Checked)
                {
                    list = new Consults().GetLabelDocsWithinRange(
                        FK_cat,
                        widthpct,
                        heightpct,
                        Convert.ToInt32(comboBoxEdit1.EditValue),
                        Convert.ToInt32(comboBoxEdit2.EditValue),
                        precutDocs.obGroupActual);
                }
                else
                {
                    list = new Consults().GetLebelsDoc(FK_cat, precutDocs.obGroupActual);
                }

                labelsObservable = new ObservableCollection<LabelDoc>(list);
                FormatGridControl1();
                textBox2.Text = $"_W{Math.Round(widthpct)}H{Math.Round(heightpct)}";
            }
            else
            {

                list = new Consults().GetLebelsDoc(FK_cat, precutDocs.obGroupActual);
                labelsObservable = new ObservableCollection<LabelDoc>(list);
                FormatGridControl1();
                textBox1.Enabled = true;
                textBox2.Enabled = false;
                sbtnSelect.Enabled = false;
                comboBoxEdit1.Enabled = false;
                comboBoxEdit2.Enabled = false;
                checkBox1.Enabled = false;
                btnAgregar.Enabled = false;
            }

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
            lblTotal.Text = list.Count.ToString();
            StyleChartControl1();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var t = textBox1.Text.Trim();
            var col = gridView1.Columns["namelabel"];
            if (col == null) return;
            if (string.IsNullOrEmpty(t))
            {
                gridView1.ActiveFilterCriteria = null;
            }
            else
            {
                gridView1.ActiveFilterCriteria = new FunctionOperator(
                    FunctionOperatorType.Contains,
                    new OperandProperty(col.FieldName),
                    new OperandValue(t)
                    );
            }
        }

        private void StyleChartControl1()
        {
            try
            {
                List<ObjectMaster> list = new Consults().GetObjectsLayersByPkGroup(precutDocs.obGroupActual);

                if (list.Count > 0)
                {
                    chartControl1.Series.Clear();

                    var serie = new Series("", ViewType.Bar)
                    {
                        ArgumentScaleType = ScaleType.Qualitative,
                        ValueScaleType = ScaleType.Numerical
                    };
                    int maxCantidad = 0;
                    if (list != null || list.Count != 0)
                    {
                        maxCantidad = list.Max(o => o.cantidad);
                    }

                    foreach (var x in list)
                    {
                        var pt = new SeriesPoint(x.nameObject, x.cantidad)
                        {
                            Tag = x.PK_ObjectMaster
                        };
                        serie.Points.Add(pt);
                    }

                    chartControl1.Series.Add(serie);

                    var diagram = chartControl1.Diagram as XYDiagram;
                    if (diagram != null)
                    {
                        diagram.AxisY.WholeRange.Auto = false;
                        diagram.AxisY.WholeRange.SetMinMaxValues(0, maxCantidad + 10);
                        diagram.AxisY.NumericScaleOptions.AutoGrid = false;
                        diagram.AxisY.NumericScaleOptions.GridAlignment = NumericGridAlignment.Custom;
                        diagram.AxisY.NumericScaleOptions.GridSpacing = 10;
                        diagram.AxisY.GridLines.Visible = true;
                    }
                }
                
            }
            catch (Exception)
            {

            }
            
        }

        private void chartControl1_Click(object sender, EventArgs e)
        {
            
        }

        private void chartControl1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var hi = chartControl1.CalcHitInfo(e.Location);
                if (hi != null && hi.InSeries && hi.SeriesPoint != null)
                {
                    SeriesPoint p = hi.SeriesPoint;
                    int id = (p.Tag is int id32) ? id32
                            : (p.Tag is long id64) ? checked((int)id64)
                            : Convert.ToInt32(p.Tag);
                    string categoria = p.Argument;
                    double valor = p.Values[0];
                    //MessageBox.Show($"Click en id {id}: {categoria} = {valor}");
                    LabelDoc ldoc = new Consults().GetLabelDocByFkObjMaster(id);
                    splashScreenManager1.ShowWaitForm();
                    foreach (GalleryItem oldItem in grupoMiniaturas.Items)
                    {
                        oldItem.Image?.Dispose();
                    }
                    grupoMiniaturas.Items.Clear();
                    pagePrecropList.Clear();
                    if (ldoc != null && ldoc.PK_LabelDoc > 0)
                    {
                        if (cbeFilterPreco.EditValue.ToString() == "Todo")
                        {
                            pagePrecropList = new Consults().GetFilterPagePrecropsByFk(ldoc.PK_LabelDoc, 0, cbeFilterImg.SelectedIndex);
                        }
                        else
                        {
                            pagePrecropList = new Consults().GetFilterPagePrecropsByFk(ldoc.PK_LabelDoc, Convert.ToInt32(cbeFilterPreco.EditValue), cbeFilterImg.SelectedIndex);
                        }

                    }
                    if (pagePrecropList.Count > 0 && pagePrecropList != null)
                    {

                        Pro pr = new Pro();
                        pr.DescargarPrecortes(pagePrecropList, folderTemp);

                        foreach (var item in pagePrecropList.AsEnumerable())
                        {
                            if (File.Exists(item.localmage))
                            {
                                Image image = LoadFreshImage(item.localmage);
                                if (image != null)
                                {
                                    grupoMiniaturas.Items.Add(new GalleryItem(LoadFreshImage(item.localmage), "", ""));

                                }
                            }
                            else
                            {
                                //grupoMiniaturas.Items.Add(new GalleryItem(LoadFreshImage(Path.Combine(folderTemp, item.localmage)), "", ""));
                            }
                        }
                    }
                    lblTimg.Text = pagePrecropList.Count.ToString();
                    splashScreenManager1.CloseWaitForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }          
        }
    }
}
