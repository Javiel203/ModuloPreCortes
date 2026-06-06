using IVisionPrecutDocs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeOnlyDo.Client;
using VERONICA.FTPTransfer;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using static System.Net.WebRequestMethods;
using DevExpress.XtraEditors.Filtering;

namespace IVisionPrecutDocs.Proceso
{
    internal class FTP
    {
        
        public bool DeleteRemoteFile(string strRemoteFile, FtpDLX ftp)
        {
            try
            {
                strRemoteFile = strRemoteFile.Replace("\\", "/");
                ftp.DeleteFile(strRemoteFile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool uploadImageBytes(Bitmap bmp,string remoteFilePath,  string fileName, sysNode node)
        {
            try
            {
                remoteFilePath = (remoteFilePath.Replace(node.storageDirectory, "")).Replace("\\", "/");
                bool estado = false;
                FtpDLX ftp = FTPConnect(node.ip, node.ftpPort, node.ftpUser, node.ftpPass);
                if (ftp!=null)
                {
                    try
                    {
                        CrearDirectoriosFtp(ftp,remoteFilePath);
                    }
                    catch (Exception)
                    {
            
                    }
                    
                    ImageFormat format = null;
                    if (bmp == null) throw new ArgumentNullException(nameof(bmp));
                    format ??= ImageFormat.Jpeg;
                    using var ms = new MemoryStream();
                    bmp.Save(ms, format);
                    ms.Position = 0;
                    ftp.PutFile(ms, $"{remoteFilePath}/{fileName}");
                    estado = true;
                    Disconnect(ftp);
                }
                return estado;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CrearDirectoriosFtp(WeOnlyDo.Client.FtpDLX ftp, string rutaCompleta)
        {
            // 1. Normalizar las barras. Los servidores FTP casi siempre usan '/' en lugar de '\'
            string rutaNormalizada = rutaCompleta.Replace("\\", "/");

            // 2. Separar los niveles de las carpetas
            string[] niveles = rutaNormalizada.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Si tu ruta inicia desde la raíz absoluta del FTP, debes empezar la variable con "/"
            // De lo contrario, se inicia vacía para que sea una ruta relativa.
            string rutaActual = rutaCompleta.StartsWith("/") ? "/" : "";

            foreach (string nivel in niveles)
            {
                // Construimos el nivel paso a paso
                rutaActual += nivel + "/";

                try
                {
                    // Intentamos crear la carpeta de este nivel
                    ftp.MakeDir(rutaActual);
                }
                catch (WeOnlyDo.Exceptions.FtpDLX.ProtocolException)
                {
                    // ERROR ESPERADO: Si la carpeta ya estaba creada, WeOnlyDo lanza 
                    // esta excepción. Al dejar el bloque 'catch' vacío (o imprimir un 
                    // log), el ciclo no se rompe y avanza a crear la siguiente subcarpeta.
                }
                catch (Exception ex)
                {
                    // Si es un error de desconexión u otro problema ajeno al FTP,
                    // aquí sí es buena idea lanzar la excepción.
                    throw new Exception($"Error crítico al intentar crear el directorio {rutaActual}: {ex.Message}");
                }
            }
        }

        public bool FTPDowload(string remoteFilePath, string localFile, sysNode node)
        {
            bool result = false;
            try
            {
                FtpDLX ftp = FTPConnect(node.ip, node.ftpPort, node.ftpUser, node.ftpPass);
                remoteFilePath = remoteFilePath.Replace("\\", "/");
                ftp.GetFile(localFile, remoteFilePath);
                Disconnect(ftp);
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
            
        }


        public FtpDLX FTPConnect(string strRemoteHost, int nRemotePort, string strUserName, string strPassword, bool Passive = false, bool Blocking = true)
        {
            try
            {
                FtpDLX FTP;
                FTP = new FtpDLX();
                FTP.LicenseKey = "EPAV-TJ3A-ESFN-5F45";
                FTP.LoopErrorEvent += FTP_LoopErrorEvent;
                FTP.DoneEvent += FTP_DoneEvent;
                FTP.ProgressEvent += FTP_ProgressEvent;
                FTP.Blocking = false;

                FTP.Hostname = strRemoteHost;
                FTP.Login = strUserName;
                FTP.Password = strPassword;
                FTP.Protocol = Protocols.FTP;
                FTP.Port = nRemotePort;
                FTP.Timeout = 200;
                FTP.Passive = Passive;
                FTP.Blocking = Blocking;
                FTP.Connect();
                return FTP;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public void Disconnect(FtpDLX FTP)
        {
            try
            {
                FTP.Disconnect();
                FTP.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void FTP_LoopErrorEvent(object Sender, FtpLoopArgs Args)
        {
            FTPErrorEventArgs fTPErrorEventArgs = new FTPErrorEventArgs();
            fTPErrorEventArgs.ErrorMessage = Args.Error.Message;
            
        }

        private void FTP_DoneEvent(object Sender, FtpDoneArgs Args)
        {
            if (Args.Error != 0)
            {
                FTPErrorEventArgs fTPErrorEventArgs = new FTPErrorEventArgs();
                fTPErrorEventArgs.ErrorMessage = "Codigo de Error -" + Args.Error + ": " + Args.Description;
                
            }
        }

        private void FTP_ProgressEvent(object Sender, FtpProgressArgs Args)
        {
            long num = Args.Position * 100 / Args.Total;
            FTPProgressEventArgs fTPProgressEventArgs = new FTPProgressEventArgs();
            fTPProgressEventArgs.Progress = (int)num;
            
        }

        public bool FTP_Rename(string destino, string origen, sysNode node)
        {
            bool result = false;
            try
            {
                FtpDLX ftp = FTPConnect(node.ip, node.ftpPort, node.ftpUser, node.ftpPass);               
                ftp.Rename(destino, origen);
                Disconnect(ftp);
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }      
    }
}
