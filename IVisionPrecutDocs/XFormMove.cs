using DevExpress.XtraEditors;
using IVisionPrecutDocs.Data;
using IVisionPrecutDocs.Entities;
using IVisionPrecutDocs.Proceso;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IVisionPrecutDocs
{
    public partial class XFormMove : DevExpress.XtraEditors.XtraForm
    {
        public int FK_cat { get; set; } = 0;
        public int FK_ObjGroup { get; set; } = 0;
        public int pk_PagePrecrop { get; set; } = 0;
        List<LabelDoc> listLabel = new List<LabelDoc>();
        public XFormMove()
        {
            InitializeComponent();
            this.Load += XFormMove_Load;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
        }

        private void XFormMove_Load(object sender, EventArgs e)
        {
            listLabel = new Consults().GetLebelsDoc(FK_cat, FK_ObjGroup);
            foreach (var item in listLabel)
            {
                comboBoxEdit1.Properties.Items.Add(item.namelabel);
            }
            comboBoxEdit1.SelectedIndex = 0;

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (pk_PagePrecrop != 0)
                {
                    PagePrecrop pp = new Consults().GetPagePrecropById(pk_PagePrecrop);
                    sysNode node = new Consults().GetNodeByFk(pp.FK_node);

                    ObjectsLayers objLayer = new Consults().GetObjetsLayers((int)pp.FK_ObjectsLayer);
                    sysNode nodeObj = new Consults().GetNodeByFk((int)objLayer.FK_Node);

                    LabelDoc labeldocSelected = listLabel[comboBoxEdit1.SelectedIndex];
                    foreach (var item in listLabel)
                    {
                        if (item.PK_LabelDoc == pp.FK_LabelDoc)
                        {
                            string nombreActual = item.namelabel;
                            string nombreNuevo = labeldocSelected.namelabel;
                            string newFileNameCrop = pp.fileNameCrop.Replace(nombreActual, nombreNuevo);
                            string origen = $"{pp.dirnameCrop.Replace(node.storageDirectory, "")}{pp.fileNameCrop}";
                            string destino = $"{pp.dirnameCrop.Replace(node.storageDirectory, "")}{newFileNameCrop}";
                            if (new FTP().FTP_Rename(destino, origen, node))
                            {
                                new Updats().UpdatePagePrecropByMove(pk_PagePrecrop, newFileNameCrop, labeldocSelected.PK_LabelDoc);
                            }

                            string origenMini = $"{objLayer.dirObjectMini.Replace(nodeObj.storageDirectory, "")}{objLayer.fileNameMini}";
                            string origenFull = $"{objLayer.dirObject.Replace(nodeObj.storageDirectory, "")}{objLayer.fileName}";

                            string destinoDirMini = objLayer.dirObjectMini.Replace(objLayer.FK_ObjectMaster.ToString(), labeldocSelected.FK_ObjectMaster.ToString());
                            string destinoDirFull = objLayer.dirObject.Replace(objLayer.FK_ObjectMaster.ToString(), labeldocSelected.FK_ObjectMaster.ToString());

                            string destinoMini = $"{destinoDirMini.Replace(nodeObj.storageDirectory, "")}{objLayer.fileNameMini}";
                            string destinoFull = $"{destinoDirFull.Replace(nodeObj.storageDirectory, "")}{objLayer.fileName}";

                            if (new FTP().FTP_Rename(destinoMini, origenMini, nodeObj))
                            {
                                if (new FTP().FTP_Rename(destinoFull, origenFull, nodeObj))
                                {
                                    new Updats().UpdateObjectsLayersByMove((int)pp.FK_ObjectsLayer, destinoDirFull, destinoDirMini, labeldocSelected.FK_ObjectMaster);
                                }
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error para mover el archivo");
            }                     
        }
    }
}