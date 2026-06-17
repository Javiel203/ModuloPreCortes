using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace IVisionPrecutDocs.Data
{
    internal class Updats
    {
        string con = new Conexion().conexion;

        public bool UpdateTaskPagePrecutbyIA(int FK_doc, int taskPagePrecutbyIA=0)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[docs].[Pages]
                    SET [taskPagePrecutbyIA] = @taskPagePrecutbyIA
                    WHERE FK_doc = @Id;";

                    int rowsAffected = cn.Execute(sql, new { taskPagePrecutbyIA = taskPagePrecutbyIA, Id = FK_doc });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateCatPk(int pk, string name)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[core].[Cats]
                    SET [Cat] = @Cat
                    WHERE [PK_cat] = @Id;";

                    int rowsAffected = cn.Execute(sql, new { Cat = name, Id = pk });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateTaskPagePrecutbyOCR(int FK_doc, int taskPagePrecutbyOCR = 0)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[docs].[Pages]
                    SET [taskPagePrecutbyOCR] = @taskPagePrecutbyOCR
                    WHERE FK_doc = @Id;";

                    int rowsAffected = cn.Execute(sql, new { taskPagePrecutbyOCR = taskPagePrecutbyOCR, Id = FK_doc });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateNameLabel(int pkLabelDoc, string newNameLabel)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[docs].[LabelDoc]
                    SET namelabel = @NameLabel
                    WHERE PK_LabelDoc = @Id;";

                    int rowsAffected = cn.Execute(sql, new { NameLabel = newNameLabel, Id = pkLabelDoc });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public bool UpdateNameObjectMaster(int pkObjectMaster, string nameObject)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[obj].[ObjectMaster]
                    SET nameObject = @nameObject
                    WHERE PK_ObjectMaster = @Id;";

                    int rowsAffected = cn.Execute(sql, new { nameObject = nameObject, Id = pkObjectMaster });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool UpdateActiveObjectMaster(int pkObjectMaster, int active = 0)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[obj].[ObjectMaster]
                    SET active = @active
                    WHERE PK_ObjectMaster = @Id;";

                    int rowsAffected = cn.Execute(sql, new { active = active, Id = pkObjectMaster });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateObjectsLayers(int pk, string fileName, string fileNameMini)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[obj].[ObjectsLayers]
                    SET [fileName] = @fileName,
                        [fileNameMini] = @fileNameMini
                    WHERE [PK_ObjectsLayer] = @Id;";

                    int rowsAffected = cn.Execute(sql, new { fileName = fileName, fileNameMini = fileNameMini, Id = pk });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePagePrecropByMove(int pk_PagePrecrop, string fileName, int fk_LabelDoc)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[docs].[PagePrecrop]
                    SET [fileNameCrop] = @fileNameCrop,
                        [FK_LabelDoc] = @FK_LabelDoc                 
                    WHERE [PK_PagePrecrop] = @Id;";

                    int rowsAffected = cn.Execute(sql, new { fileNameCrop = fileName, FK_LabelDoc = fk_LabelDoc, Id = pk_PagePrecrop });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateObjectsLayersByMove(int PK_ObjectsLayer, string dirObject, string dirObjectMini, int FK_ObjectMaster)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[obj].[ObjectsLayers]
                    SET [dirObject] = @dirObject,
                        [dirObjectMini] = @dirObjectMini,
                        [FK_ObjectMaster] = @FK_ObjectMaster
                    WHERE [PK_ObjectsLayer] = @Id;";

                    int rowsAffected = cn.Execute(sql, new { dirObject = dirObject, dirObjectMini = dirObjectMini, FK_ObjectMaster = FK_ObjectMaster, Id = PK_ObjectsLayer });

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePagePrecropFkObjLayer(int PK_PagePrecrop, int FK_ObjectsLayer)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(con))
                {
                    const string sql = @"
                    UPDATE [RevistasIA].[docs].[PagePrecrop]
                    SET [FK_ObjectsLayer] = @FK_ObjectsLayer,
                    [PrecropPointsValidation] = 1
                    WHERE [PK_PagePrecrop] = @PK_PagePrecrop;";

                    int rowsAffected = cn.Execute(sql, new { FK_ObjectsLayer = FK_ObjectsLayer, PK_PagePrecrop = PK_PagePrecrop});

                    return rowsAffected > 0; // True si se actualizó al menos una fila
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
