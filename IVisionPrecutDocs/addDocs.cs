using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraSplashScreen;
using IVisionPrecutDocs.Data;
using IVisionPrecutDocs.Entities;
using IVisionPrecutDocs.Proceso;
using NetTopologySuite.Planargraph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace IVisionPrecutDocs
{
    public partial class addDocs : Form
    {
        public int fk_cat {  get; set; }
        public string nameCat { get; set; }
        List<(int, string, string,int,int,int)> miLista = new List<(int, string, string,int,int,int)>();
        public addDocs()
        {
            InitializeComponent();
            this.Load += AddDocs_Load;
        }

        private void AddDocs_Load(object sender, EventArgs e)
        {
            cbAnio.SelectedIndex = 0;
            cbMes.SelectedIndex = 0;
            cbDia.SelectedIndex = 0;
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Multiselect = true;
                    ofd.Filter = "Archivos de imagen (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                    ofd.Title = "Selecciona una o varias imágenes";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        splashScreenManager1.ShowWaitForm();
                        foreach (string ruta in ofd.FileNames)
                        {
                            ObtenerDatosImagen(ruta);

                        }

                        int fkDoc = new Inserts().InsertDoc($"{cbAnio.SelectedItem.ToString()}-{cbMes.SelectedItem.ToString()}-{cbDia.SelectedItem.ToString()}",
                            fk_cat,
                            1);

                        if (fkDoc != 0)
                        {
                            foreach (var elemento in miLista)
                            {

                                //MessageBox.Show($"pg: {elemento.Item1} | Nombre: {elemento.Item2} | Nombre2: {elemento.Item3}  | alto: {elemento.Item4} | ancho: {elemento.Item5} | dpi: {elemento.Item6}");
                                int idPg = new Inserts().InsertPage(
                                     fkDoc,
                                     elemento.Item1,
                                     1,
                                     elemento.Item2,
                                     $"X:\\BIGDATA\\Metadata\\docs\\{cbAnio.SelectedItem.ToString()}\\{cbMes.SelectedItem.ToString()}\\{cbDia.SelectedItem.ToString()}\\{nameCat}\\Miniaturas\\",
                                     elemento.Item3,
                                     elemento.Item4,
                                     elemento.Item5,
                                     elemento.Item6,
                                     0,
                                     "",
                                     0,
                                     ConvertirPixelesAPuntos(elemento.Item5, elemento.Item6),
                                     ConvertirPixelesAPuntos(elemento.Item4, elemento.Item6)
                                     );
                                if (idPg>0)
                                {
                                    SendMQ(idPg.ToString());
                                }
                                
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error al insertar documento");
                        }
                        splashScreenManager1.CloseWaitForm();

                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error al insertar documento");
            }
            
        }


        public int ConvertirPixelesAPuntos(int pixels, int dpi)
        {
            if (dpi <= 0)
            {
                return 0; // Retorna 0 por defecto, igual que tu base de datos
            }
            double puntos = (pixels * 72.0) / dpi;
            return (int)Math.Round(puntos, MidpointRounding.AwayFromZero);
            
        }

        private static IConnection connection;
        private static ISession session;
        private static IMessageProducer producer;
        private void SendMQ(string sms)
        {
            Uri connecturi = new Uri(GetDataXml("ServerQueue"));
            ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);
            connection = connectionFactory.CreateConnection();
            connection.Start();
            session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination destination = session.GetQueue(GetDataXml("QueueGet"));
            producer = session.CreateProducer(destination);
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            ITextMessage message = session.CreateTextMessage(sms);
            producer.Send(message);
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

        private void ObtenerDatosImagen(string ruta)
        {
            try
            {
                // El bloque 'using' asegura que el archivo se libere de la memoria al terminar
                using (Image imagen = Image.FromFile(ruta))
                {
                    // Obtener el nombre del archivo
                    string nombreArchivo = Path.GetFileName(ruta);

                    // Extraer los datos
                    int ancho = imagen.Width;
                    int alto = imagen.Height;
                    float resHorizontal = imagen.HorizontalResolution;
                    float resVertical = imagen.VerticalResolution;
                    int pg = ObtenerPagina(nombreArchivo);
                    Bitmap miBitmap;
                    sysNode node = new Consults().GetNodeByFk(1);
                    using (Bitmap tempBitmap = new Bitmap(ruta))
                    {
                        // Hacemos una copia exacta en memoria (clonamos)
                        miBitmap = new Bitmap(tempBitmap);
                    }
                    new FTP().uploadImageBytes(
                        miBitmap,
                        $"X:\\BIGDATA\\Metadata\\docs\\{cbAnio.SelectedItem.ToString()}\\{cbMes.SelectedItem.ToString()}\\{cbDia.SelectedItem.ToString()}\\{nameCat}\\Miniaturas\\"
                        , nombreArchivo
                        , node);

                    if (miLista.Count > 0 && BuscarPaginaDuplicada(miLista,pg))
                    {
                        int indice = miLista.FindIndex(t => t.Item1 == pg);
                        if (ancho > 150 && alto > 150)
                        {
                            miLista[indice] = (pg, nombreArchivo, miLista[indice].Item3,alto, ancho, (int)resHorizontal);
                        }
                        else
                        {
                            miLista[indice] = (pg, miLista[indice].Item2, nombreArchivo, alto, ancho, (int)resHorizontal);
                        }
                    }
                    else
                    {
                        if (ancho > 150 && alto > 150)
                        {
                            miLista.Add((pg, nombreArchivo, "", alto, ancho, (int)resHorizontal));
                        }
                        else
                        {
                            miLista.Add((pg, "", nombreArchivo,alto,ancho,(int)resHorizontal));
                        }

                    }

                    // Imprimir los resultados con el formato que solicitas
                    //MessageBox.Show($"fkcat : {fk_cat.ToString()}" +
                    //        $"\nNombre: {nombreArchivo}\nAncho: {ancho} píxeles" +
                    //        $" \nAlto: {alto} píxeles" +
                    //        $"\nDPI {resHorizontal} ppp" +
                    //        $"\n pagina: {ObtenerPagina(nombreArchivo).ToString()}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar {ruta}: {ex.Message}");
            }
        }

        private bool BuscarPaginaDuplicada(List<(int, string, string,int,int,int)> ls, int pg)
        {
            if (!ls.Any()) return false;
            foreach (var mi in ls)
            {
                if (mi.Item1 == pg)
                {
                    return true;
                }
                
            }
            return false;
        }

        private int ObtenerPagina(string nombre)
        {
            // 1. Dividir el texto cada vez que encuentre un guion bajo '_'
            string[] partes = nombre.Split('_');
            if (partes.Length >= 2)
            {
                // 2. El valor que buscamos está en la posición 1 (el segundo elemento)
                string valorMedio = partes[1]; // Esto guardará "001"

                // 3. Quitar los ceros a la izquierda
                // Al convertirlo a entero (int), los ceros a la izquierda desaparecen solos
                return int.Parse(valorMedio);

            }
            return 0;

        }

    }
}
