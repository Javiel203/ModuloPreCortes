using IVisionPrecutDocs.Data;
using IVisionPrecutDocs.Entities;
using IVisionPrecutDocs.Proceso;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IVisionPrecutDocs
{
    public partial class DetailsPrecut : Form
    {
        int id = 0;
        int FK_cat = 0;
        int FK_ObjGroup = 0;
        private static string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string foldertemp = Path.Combine(baseDir, "tmp\\Precortes");
        public DetailsPrecut(int pkPrecrop)
        {
            InitializeComponent();

            try
            {
                id = pkPrecrop;
                PagePrecrop pp = new Consults().GetPagePrecropById(pkPrecrop);
                sysNode node = new Consults().GetNodeByFk(pp.FK_node);
                foldertemp = $"{foldertemp}\\{DateTime.Now.ToString("HHmmss")}";
                if (!Directory.Exists(foldertemp))
                {
                    Directory.CreateDirectory(foldertemp);
                }
                string remoteFile = $"{pp.dirnameCrop.Replace(node.storageDirectory, "")}{pp.fileNameCrop}";
                string localfile = Path.Combine(foldertemp, pp.fileNameCrop);
                if (new FTP().FTPDowload(remoteFile, localfile, node))
                {
                    pictureEditPrecut.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                    pictureEditPrecut.Image = Image.FromFile(localfile);
                }
                if (pp.FK_ObjectsLayer != 0)
                {
                    ObjectsLayers objLayer = new Consults().GetObjetsLayers((int)pp.FK_ObjectsLayer);
                    sysNode nodeObj = new Consults().GetNodeByFk((int)objLayer.FK_Node);
                    string remoteFileObj = $"{objLayer.dirObjectMini.Replace(nodeObj.storageDirectory, "")}{objLayer.fileNameMini}";
                    string localfileObj = Path.Combine(foldertemp, objLayer.fileNameMini);
                    if (new FTP().FTPDowload(remoteFileObj, localfileObj, nodeObj))
                    {
                        pictureEditObj.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
                        pictureEditObj.Image = Image.FromFile(localfileObj);
                    }
                }                            
                Paginas p = new Consults().GetPage(pp.FK_Page);
                Docs d = new Consults().GetDocByPK(p.FK_doc);
                lblNumPage.Text = p.pagenumber.ToString();
                lblFechaPage.Text = d.dateInclude.ToString("dd-MM-yyyy");
                FK_cat = d.FK_cat;
                FK_ObjGroup = new Consults().GetFkObjectGroup(d.FK_cat);
            }
            catch (Exception)
            {
                MessageBox.Show("Error al cargar imagenes");               
            }
            
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1200,700);
           
            this.Load += DetailsPrecut_Load;
        }

        private void DetailsPrecut_Load(object sender, EventArgs e)
        {
            
        }

        private void sbtnMove_Click(object sender, EventArgs e)
        {
            XFormMove frmMove = new XFormMove();
            frmMove.FK_cat = FK_cat;
            frmMove.FK_ObjGroup = FK_ObjGroup;
            frmMove.pk_PagePrecrop = id;
            frmMove.Show();
        }
    }
}
