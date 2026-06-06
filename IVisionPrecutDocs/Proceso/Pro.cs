using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using DevExpress.Skins;
using IVisionPrecutDocs.Data;
using IVisionPrecutDocs.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace IVisionPrecutDocs.Proceso
{
    internal class Pro
    {
        //public event Action<int, int> Aplicar;

        public void DeletePreCor(int pk)
        {
            try
            {
                FTP ftp = new FTP();
                PagePrecrop pp = new Consults().GetPagePrecropById(pk);
                if (pp.fileNameCrop != null && pp.dirnameCrop != null)
                {
                    sysNode node = new Consults().GetNodeByFk(pp.FK_node);
                    string remoteFile = $"{(pp.dirnameCrop).Replace(node.storageDirectory, "")}{pp.fileNameCrop}";

                    bool resul = ftp.DeleteRemoteFile(remoteFile, ftp.FTPConnect(node.ip, node.ftpPort, node.ftpUser, node.ftpPass));
                    new Delete().DeletePagePrecropById(pk);
                    if (resul && pp.FK_ObjectsLayer > 0)
                    {
                        try
                        {
                            //new Delete().DeletePagePrecropById(pk);
                            ObjectsLayers ol = new Consults().GetObjetsLayers((int)pp.FK_ObjectsLayer);
                            if (ol.dirObject != null)
                            {
                                sysNode nodeObj = new Consults().GetNodeByFk((int)ol.FK_Node);
                                bool delete1 = ftp.DeleteRemoteFile(
                                    $"{(ol.dirObject).Replace(nodeObj.storageDirectory, "")}{ol.fileName}",
                                    ftp.FTPConnect(nodeObj.ip,
                                    nodeObj.ftpPort,
                                    nodeObj.ftpUser,
                                    nodeObj.ftpPass));

                                bool delete2 = ftp.DeleteRemoteFile(
                                    $"{(ol.dirObjectMini).Replace(nodeObj.storageDirectory, "")}{ol.fileNameMini}",
                                    ftp.FTPConnect(nodeObj.ip,
                                    nodeObj.ftpPort,
                                    nodeObj.ftpUser,
                                    nodeObj.ftpPass));

                                if (delete1 && delete2)
                                {
                                    new Delete().DeleteObjectsLayerById(ol.PK_ObjectsLayer);
                                    
                                }
                            }
                            else
                            {
                                MessageBox.Show("No exiten imagenes en ObjetLayer");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error al eliminar el objeto: {ex.ToString()}");
                        }

                    }
                    else
                    {
                        //MessageBox.Show("Solo Page Precrop Eliminado");
                    }
                }
                else
                {                                    
                    MessageBox.Show("No exiten imagenes en PagePrecrop a Eliminar");                   
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar: Error{ex.ToString()}");
            }
            
        }

        public void DescargarPaginas(List<Paginas> listaPaginas, string foldertemp)
        {
            try
            {
                foldertemp = $"{foldertemp}\\{DateTime.Now.ToString("HHmmss")}";
                if (!Directory.Exists(foldertemp))
                {
                    Directory.CreateDirectory(foldertemp);
                }
                int total = listaPaginas.Count;
                int conta = 0;

                int FK_Node = listaPaginas.Max(d => d.FK_Node);
                sysNode node = new Consults().GetNodeByFk(FK_Node);
                if (node != null && node.PK_Node > 0)
                {
                    SemaphoreSlim sema = new SemaphoreSlim(3);
                    List<Task> listaTareas = new List<Task>();
                    Task descarga = null;
                    foreach (var paginas in listaPaginas)
                    {
                        if (!string.IsNullOrEmpty(paginas.dirNameFullJPG))
                        {

                            string remoteFileF = (paginas.dirNameFullJPG + paginas.fileNameFullJPG).Replace(node.storageDirectory, "");
                            string localfileF = Path.Combine(foldertemp, paginas.fileNameFullJPG);
                            string remoteFile = (paginas.dirNameFullJPG + paginas.fileNameMiniJPG).Replace(node.storageDirectory, "");
                            string localfile = Path.Combine(foldertemp, paginas.fileNameMiniJPG);
                            //TimeSpan timeout = TimeSpan.FromMinutes(3);
                            descarga = Task.Factory.StartNew(() =>
                            //descarga = Task.Run(() =>
                            {
                                sema.Wait();
                                conta++;
                                if (new FTP().FTPDowload(remoteFileF, localfileF, node))
                                {
                                    paginas.LocalImgFull = localfileF;
                                }                               
                                if (new FTP().FTPDowload(remoteFile, localfile, node))
                                {
                                    paginas.LocalMini = localfile;
                                }
                            });
                            
                            listaTareas.Add(descarga);
                            sema.Release();
                        }
                        

                    }
                    Task.WaitAll(listaTareas.ToArray());
                    foreach (var item in listaTareas)
                    {
                        item.Dispose();
                    }
                    sema.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }
        }

        public void DescargarImagenFull(Paginas p, string folderTemp)
        {
            try
            {

                if (!Directory.Exists(folderTemp))
                {
                    Directory.CreateDirectory(folderTemp);
                }
                sysNode node = new Consults().GetNodeByFk(p.FK_Node);
                if (node != null && node.PK_Node > 0)
                {
                    string remoteFile = (p.dirNameFullJPG + p.fileNameFullJPG).Replace(node.storageDirectory, "");
                    string localfile = Path.Combine(folderTemp, p.fileNameFullJPG);
                    if (System.IO.File.Exists(localfile))
                    {
                        p.LocalImgFull = localfile;
                    }
                    else
                    {
                        if (new FTP().FTPDowload(remoteFile, localfile, node))
                        {
                            p.LocalImgFull = localfile;
                        }
                    }
                }
            }
            catch (Exception)
            {


            }
        }

        public void DescargarPrecortes(List<PagePrecrop> lista, string foldertemp)
        {
            try
            {
                foldertemp = $"{foldertemp}\\{DateTime.Now.ToString("HHmmss")}";
                if (!Directory.Exists(foldertemp))
                {
                    Directory.CreateDirectory(foldertemp);
                }
                int total = lista.Count;
                int conta = 0;

                int FK_Node = lista.Max(d => d.FK_node);
                sysNode node = new Consults().GetNodeByFk(FK_Node);
                if (node != null && node.PK_Node > 0)
                {
                    SemaphoreSlim sema = new SemaphoreSlim(3);
                    List<Task> listaTareas = new List<Task>();
                    Task descarga = null;
                    foreach (var paginas in lista)
                    {
                        if (!string.IsNullOrEmpty(paginas.fileNameCrop))
                        {
                            string remoteFile = $"{paginas.dirnameCrop.Replace(node.storageDirectory, "")}{paginas.fileNameCrop}";
                            string localfile = Path.Combine(foldertemp,paginas.fileNameCrop);
                            //string remoteFile = (paginas.dirNameMiniJPG + paginas.fileNameMiniJPG).Replace(node.storageDirectory, "");
                            //string localfile = Path.Combine(foldertemp, paginas.fileNameMiniJPG);
                            descarga = Task.Factory.StartNew(() =>
                            {
                                sema.Wait();
                                conta++;
                                
                                if (new FTP().FTPDowload(remoteFile, localfile, node))
                                {
                                    paginas.localmage = localfile;
                                }
 
                            });
 
                            paginas.localmage = localfile;
                            listaTareas.Add(descarga);
                            sema.Release();
                        }


                    }
                    Task.WaitAll(listaTareas.ToArray());
                    foreach (var item in listaTareas)
                    {
                        item.Dispose();
                    }
                    sema.Dispose();

                }

            }
            catch (Exception)
            {


            }
        }

        public int ProInsertObjectsLayers(int groupObject, int objMaster, string textYolo, string points2D, string LocalImgFull, List<Point> points)
        {
            try
            {
                string dirObj = $"X:\\BIGDATA\\Objectdata\\Group{groupObject}\\Object{objMaster}\\";
                int idobjLayer = new Inserts().InsertObjectsLayers(dirObj, objMaster, dirObj, textYolo, points2D, 1);
                if (idobjLayer > 0)
                {
                    string fileName = $"Full_{idobjLayer}.jpg";
                    string fileNameMini = $"Crop_{idobjLayer}.jpg";
                    if (new Updats().UpdateObjectsLayers(idobjLayer, fileName, fileNameMini))
                    {
                        sysNode node = new Consults().GetNodeByFk(1);
                        Bitmap bmp = null;
                        using (var bmpTemp = new Bitmap(LocalImgFull))
                        {
                            bmp = new Bitmap(bmpTemp);
                            bool result = new FTP().uploadImageBytes(bmp, dirObj, fileName, node);
                            if (!result)
                            {
                                MessageBox.Show("Error al insertar Imagen full");
                                return 0;
                            }
                        }
                        if (bmp!=null)
                        {
                            bool resultMini = new FTP().uploadImageBytes(CropByPolygon(bmp, points), dirObj, fileNameMini, node);
                            if (!resultMini)
                            {
                                MessageBox.Show("Error al insertar Imagen Mini");
                                return 0;
                            }
                        }
                       
                        return idobjLayer;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;              
            }
            
            
        }

        public static Bitmap CropByPolygon(Bitmap src, IList<Point> polygon)
        {
            if (src == null) throw new ArgumentNullException(nameof(src));
            if (polygon == null || polygon.Count < 3)
                throw new ArgumentException("Se requieren al menos 3 puntos para un polígono.", nameof(polygon));

            int minX = polygon.Min(p => p.X);
            int minY = polygon.Min(p => p.Y);
            int maxX = polygon.Max(p => p.X);
            int maxY = polygon.Max(p => p.Y);

            int w = Math.Max(1, maxX - minX);
            int h = Math.Max(1, maxY - minY);

            // Ajustar a límites de la imagen
            minX = Math.Max(0, minX);
            minY = Math.Max(0, minY);
            if (minX + w > src.Width) w = src.Width - minX;
            if (minY + h > src.Height) h = src.Height - minY;

            var dst = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            dst.SetResolution(src.HorizontalResolution, src.VerticalResolution);

            // Traslada los puntos al sistema local del recorte (origen en 0,0)
            var local = polygon.Select(p => new Point(p.X - minX, p.Y - minY)).ToArray();

            using (var g = Graphics.FromImage(dst))
            using (var path = new GraphicsPath())
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceCopy;
                g.SmoothingMode = SmoothingMode.AntiAlias;     // bordes suaves en el contorno
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                path.AddPolygon(local);
                g.SetClip(path); // Todo lo que se dibuje fuera del polígono queda recortado

                // Dibuja la porción de la imagen original alineada al recorte
                g.DrawImage(
                    src,
                    new Rectangle(0, 0, w, h),               // destino
                    new Rectangle(minX, minY, w, h),         // origen
                    GraphicsUnit.Pixel);

                g.ResetClip();
            }

            return dst;
        }

    }
}
