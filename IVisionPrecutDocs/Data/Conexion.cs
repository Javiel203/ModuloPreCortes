using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IVisionPrecutDocs.Data
{
    internal class Conexion
    {
        public string conexion =
            string.Format("Data Source={0};User ID={1};Password={2}",
                Decrypt(datoDataSource()),
                Decrypt(datoUser()),
                Decrypt(datoPass()));
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("abcdef9876543210");
        public static string Decrypt(string encryptedText)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            var encryptedBytes = Convert.FromBase64String(encryptedText);

            using var ms = new MemoryStream(encryptedBytes);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
        public static string datoDataSource()
        {
            string xmlFilePath = (System.Reflection.Assembly.GetExecutingAssembly().Location).Replace("IVisionPrecutDocs.dll", "") + "conexion.xml";
            XmlDocument xmlDoc = new XmlDocument();
            string DataSource = "";
            xmlDoc.Load(xmlFilePath);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNode conexionNode = root.SelectSingleNode("conexion");
            if (conexionNode != null)
            {
                DataSource = conexionNode.Attributes["DataSource"].Value;

            }
            return DataSource;
        }

        public static string datoUser()
        {
            string xmlFilePath = (System.Reflection.Assembly.GetExecutingAssembly().Location).Replace("IVisionPrecutDocs.dll", "") + "conexion.xml";
            XmlDocument xmlDoc = new XmlDocument();
            string user = "";
            xmlDoc.Load(xmlFilePath);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNode conexionNode = root.SelectSingleNode("conexion");
            if (conexionNode != null)
            {
                XmlNode datoNode = conexionNode.SelectSingleNode("User");
                if (datoNode != null)
                {
                    user = datoNode.InnerText;
                }

            }
            return user;
        }

        public static string datoPass()
        {
            string xmlFilePath = (System.Reflection.Assembly.GetExecutingAssembly().Location).Replace("IVisionPrecutDocs.dll", "") + "conexion.xml";
            XmlDocument xmlDoc = new XmlDocument();
            string pass = "";
            xmlDoc.Load(xmlFilePath);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNode conexionNode = root.SelectSingleNode("conexion");
            if (conexionNode != null)
            {
                XmlNode datoNode = conexionNode.SelectSingleNode("Password");
                if (datoNode != null)
                {
                    pass = datoNode.InnerText;
                }

            }
            return pass;
        }

    }
}
