using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace IVisionPrecutDocs.Data
{
    internal class Delete
    {
        string con = new Conexion().conexion;

        public bool DeleteObjectsLayerById(int id)
        {
            try
            {
                using var cn = new SqlConnection(con);
                const string sql = @"
                DELETE FROM [MetaObject].[dbo].[ObjectsLayers]
                WHERE PK_ObjectsLayer = @id;";

                int rows = cn.Execute(sql, new { id });
                return rows > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public bool DeleteCategoriaById(int id)
        {
            try
            {
                using var cn = new SqlConnection(con);
                const string sql = @"
                DELETE FROM [RevistasIA].[core].[Cats]
                WHERE [PK_cat] = @id;";

                int rows = cn.Execute(sql, new { id });
                return rows > 0;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public bool DeletePagePrecropById(int id)
        {
            try
            {
                using var cn = new SqlConnection(con);
                const string sql = @"
                DELETE FROM [MetaDocs].[dbo].[PagePrecrop]
                WHERE PK_PagePrecrop = @id;";

                int rows = cn.Execute(sql, new { id });
                return rows > 0;
            }
            catch (Exception)
            {

                return false;
            }
            
        }

        public bool DeleteLabelDocById(int id)
        {
            try
            {
                using var cn = new SqlConnection(con);
                const string sql = @"
                DELETE FROM [MetaDocs].[dbo].[LabelDoc]
                WHERE [PK_LabelDoc] = @id;";

                int rows = cn.Execute(sql, new { id });
                return rows > 0;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
