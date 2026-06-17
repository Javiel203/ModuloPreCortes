using IVisionPrecutDocs.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace IVisionPrecutDocs.Data
{
    internal class Consults
    {
        string conexion = new Conexion().conexion;

        public List<LabelDoc> GetLebelsDoc(int fkCat, int fkObj)
        {
            try
            {
                List<LabelDoc> lista = new List<LabelDoc>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery =
                        @"
                            WITH CountsPorLabel AS (
                              SELECT
                                  P.FK_LabelDoc,
                                  SUM(CASE WHEN P.ProcessbyIA = 0 AND P.precorteOCR = 0 THEN 1 ELSE 0 END) AS CatImgNoIa,     
                                  SUM(CASE WHEN P.ProcessbyIA = 1 AND P.precorteOCR = 0 THEN 1 ELSE 0 END) AS CatImgIa,
                                  SUM(CASE WHEN P.ProcessbyIA = 0 AND P.precorteOCR = 1 THEN 1 ELSE 0 END) AS CatImgOcr,
                                  SUM(CASE WHEN P.FK_ObjectsLayer > 0 THEN 1 ELSE 0 END) AS CatApproved
                              FROM [RevistasIA].[docs].[PagePrecrop] AS P
                              GROUP BY P.FK_LabelDoc
                            )
                            SELECT
                                LD.PK_LabelDoc,
	                            LD.FK_cat,
                                LD.namelabel,
	                            LD.widthPct,
	                            LD.heightPct,
                                LD.FK_ObjectMaster,
                                LD.FK_ObjectGroup,
                                COALESCE(CPL.CatImgNoIa, 0)   AS CatImgNoIa,
                                COALESCE(CPL.CatImgIa, 0) AS CatImgIa,
                                COALESCE(CPL.CatImgOcr, 0) AS CatImgOcr,
                                COALESCE(CPL.CatApproved, 0) AS CatApproved
                            FROM [RevistasIA].[docs].[LabelDoc] AS LD
                            LEFT JOIN CountsPorLabel AS CPL
                                   ON CPL.FK_LabelDoc = LD.PK_LabelDoc
                            WHERE LD.FK_cat = @fkCat AND LD.FK_ObjectGroup = @fkObj
                        ;";

                    var parametros = new DynamicParameters();
                    parametros.Add("@fkCat", fkCat);
                    parametros.Add("@fkObj", fkObj);
                    lista = cn.Query<LabelDoc>(squery, parametros, null, true, 0, CommandType.Text).ToList();
                }
                return lista;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<LabelDoc> GetLabelDocsWithinRange(int fkDoc, double width, double height, int umbralW, int umbralH, int fkObjG)
        {
            try
            {
                List<LabelDoc> lista = new List<LabelDoc>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery =
                            @"WITH CountsPorLabel AS (
                              SELECT
                                  P.FK_LabelDoc,
                                  SUM(CASE WHEN P.ProcessbyIA = 0 AND P.precorteOCR = 0 THEN 1 ELSE 0 END) AS CatImgNoIa,     
                                  SUM(CASE WHEN P.ProcessbyIA = 1 AND P.precorteOCR = 0 THEN 1 ELSE 0 END) AS CatImgIa,
                                  SUM(CASE WHEN P.ProcessbyIA = 0 AND P.precorteOCR = 1 THEN 1 ELSE 0 END) AS CatImgOcr,
                                  SUM(CASE WHEN P.FK_ObjectsLayer > 0 THEN 1 ELSE 0 END) AS CatApproved
                              FROM [RevistasIA].[docs].[PagePrecrop] AS P
                              GROUP BY P.FK_LabelDoc
                            )
                            SELECT
                                LD.PK_LabelDoc,
	                            LD.FK_cat,
                                LD.namelabel,
	                            LD.widthPct,
	                            LD.heightPct,
                                LD.FK_ObjectMaster,
                                LD.FK_ObjectGroup,
                                COALESCE(CPL.CatImgNoIa, 0)   AS CatImgNoIa,
                                COALESCE(CPL.CatImgIa, 0) AS CatImgIa,
                                COALESCE(CPL.CatImgOcr, 0) AS CatImgOcr,
                                COALESCE(CPL.CatApproved, 0) AS CatApproved
                            FROM [RevistasIA].[docs].[LabelDoc] AS LD
                            LEFT JOIN CountsPorLabel AS CPL
                                   ON CPL.FK_LabelDoc = LD.PK_LabelDoc
                            WHERE [FK_cat] = @date
                            AND [FK_ObjectGroup] = @fkObjG 
                            AND [widthPct]  IS NOT NULL 
                            AND [heightPct] IS NOT NULL 
                            AND ABS([widthPct]  - @width)  <= @umbralW 
                            AND ABS([heightPct] - @height) <= @umbralH 
                            ORDER BY 
                            ABS([widthPct]  - @width) + ABS([heightPct] - @height);";
                    var parametros = new DynamicParameters();
                    parametros.Add("@date", fkDoc);
                    parametros.Add("@fkObjG", fkObjG);
                    parametros.Add("@width", width);
                    parametros.Add("@height", height);
                    parametros.Add("@umbralW", umbralW);
                    parametros.Add("@umbralH", umbralH);
                    lista = cn.Query<LabelDoc>(squery, parametros, null, true, 0, CommandType.Text).ToList();
                }
                return lista;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int IdentCurrentPagePrecrop()
        {
            try
            {
                int lastIdIdent;
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    lastIdIdent = cn.ExecuteScalar<int>(
                    "SELECT CONVERT(int, ISNULL(IDENT_CURRENT('[RevistasIA].[docs].[PagePrecrop]'), 0))");
                }
                return lastIdIdent;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public List<int> GetPagesByFkDoc(int fk_Doc)
        {
            List<int> lista = new List<int>();
            try
            {
                
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("select [PK_Page]  from  [RevistasIA].[docs].[Pages] where FK_doc ={0}", fk_Doc);
                    lista = cn.Query<int>(squery, null, null, true, 0, CommandType.Text).ToList();
                }
                return lista;

            }
            catch (Exception)
            {
                return lista;

            }
        }

        public List<PagePrecrop> GetPagePrecrops(int pk)
        {
            try
            {
                List<PagePrecrop> lista = new List<PagePrecrop>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = @"
                    SELECT 
                        pp.[PK_PagePrecrop], 
                        pp.[FK_Page], 
                        pp.[PolyTextGeometry],
                        pp.[Points2D],
                        pp.[FK_LabelDoc],                      
                        pp.[widthPct],
                        pp.[heightPct],
                        pp.[ProcessbyIA],
                        pp.[ProcessIAscore],
                        pp.[FK_ObjectsLayer],
                        ld.[namelabel],
                        pp.[precorteOCR],
                        pp.[PrecropPointsValidation]
                         
                    FROM [RevistasIA].[docs].[PagePrecrop] pp
                    INNER JOIN [RevistasIA].[docs].[LabelDoc]ld
                    ON pp.[FK_LabelDoc] = ld.[PK_LabelDoc]
                    WHERE pp.[FK_Page] = @date;";

                    var parametros = new DynamicParameters();
                    parametros.Add("@date", pk);
                    lista = cn.Query<PagePrecrop>(squery, parametros, null, true, 0, CommandType.Text).ToList();
                }

                return lista;
            }
            catch (Exception)
            {
                return null;
            }
        }

        

        public List<Docs> GetDocs(DateTime fechaBusqueda)
        {
            try
            {
                List<Docs> lista = new List<Docs>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    //string squery = string.Format("select ROW_NUMBER () over (order by Pk_Doc desc) 'NItem', * from [MetaDocs].[dbo].[Docs] d inner join [sysCloud].[dbo].[Cats] c on (d.FK_cat=c.PK_cat) where convert (date, d.dateInclude)=@date");
                    const string squery = @"
	                     SELECT
                         ROW_NUMBER() OVER (ORDER BY d.Pk_Doc DESC) AS NItem,
                         d.Pk_Doc,
                         d.FK_cat,
                         d.dateInclude,
                         c.Cat AS Cat,  -- ajusta al nombre real de la columna en Cats  
                         (SELECT COUNT(*) FROM [RevistasIA].[docs].[Pages] p WHERE p.FK_doc = d.Pk_Doc) AS TotalPages,
                         (SELECT COUNT(*) FROM [RevistasIA].[docs].[Pages] p WHERE p.FK_doc = d.Pk_Doc AND p.taskPageOcr > 0) AS TotalPagesOCR,
                         CAST (ISNULL((SELECT SUM(p.processTime) FROM [RevistasIA].[docs].[Pages] p WHERE p.FK_doc = d.Pk_Doc AND taskPageOcr > 0),0) /60000.0 AS DECIMAL(10,2)) AS TotalTimeOCR
                         FROM [RevistasIA].[docs].[Docs] d
                         INNER JOIN [RevistasIA].[core].[Cats] c
                         ON d.FK_cat = c.PK_cat
                         WHERE CONVERT(date, d.dateInclude) = @date;";
                    var parametros = new DynamicParameters();
                    parametros.Add("@date", fechaBusqueda.Date);
                    lista = cn.Query<Docs>(squery, parametros, null, true, 0, CommandType.Text).ToList();
                }
                return lista;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public Docs GetDocByPK(int pkDoc)
        {
            try
            {
                Docs doc = new Docs();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("SELECT  *  FROM [RevistasIA].[docs].[Docs] where [Pk_Doc]={0} ", pkDoc);
                    doc = cn.QueryFirst<Docs>(squery, null, null, 0, CommandType.Text);
                }
                return doc;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public List<Paginas> GetPagesByFk(int pk_Doc)
        {
            try
            {
                List<Paginas> lista = new List<Paginas>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("select *  from  [RevistasIA].[docs].[Pages] where FK_doc ={0} and dirNameFullJPG is not null order by pagenumber desc", pk_Doc);
                    lista = cn.Query<Paginas>(squery, null, null, true, 0, CommandType.Text).ToList();
                }
                return lista;

            }
            catch (Exception)
            {
                return null;

            }
        }

        public PagePrecrop GetPagePrecropById(int id)
        {
            try
            {
                using var cn = new SqlConnection(conexion);
                const string sql = @"
                SELECT 
                        PK_PagePrecrop,
                        FK_Page,
                        fileNameCrop,
                        dirnameCrop,
                        FK_node,
                        PolyTextGeometry,
                        PolyTextYoloFormat,
                        FK_LabelDoc,
                        widthPct,
                        heightPct,
                        Points2D,
                        FK_ObjectsLayer,
                        ProcessbyIA,
                        ProcessIAscore,
                        PolyGeometryPoints.STAsBinary() as PolyGeometryPoints
                FROM [RevistasIA].[docs].[PagePrecrop]
                WHERE PK_PagePrecrop = @id;";

                return cn.QueryFirstOrDefault<PagePrecrop>(sql, new { id });
            }
            catch (Exception)
            {

                return null;
            }
            
        }

        public int GetFkObjectGroup(int FK_cat)
        {
            try
            {
                using var cn = new SqlConnection(conexion);
                const string sql = @"
                SELECT [FK_ObjectGroup]
                FROM [RevistasIA].[docs].[ObjectDocCats]
                WHERE FK_cat = @FK_cat;";

                return cn.QueryFirstOrDefault<int>(sql, new { FK_cat });
            }
            catch (Exception)
            {

                return 0;
            }

        }


        public List<PagePrecrop> GetFilterPagePrecropsByFk(int pk, int take = 0, int filter = 4)
        {
            try
            {
                if (take <= 0) { take = 1000000; }
                List<PagePrecrop> lista = new List<PagePrecrop>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = "";
                    if (filter == 4)
                    {
                        squery = @"
                        SELECT TOP (@take) 
                        PK_PagePrecrop,
                        FK_Page,
                        fileNameCrop,
                        dirnameCrop,
                        FK_node,
                        PolyTextGeometry,
                        PolyTextYoloFormat,
                        FK_LabelDoc,
                        widthPct,
                        heightPct,
                        Points2D,
                        FK_ObjectsLayer,
                        ProcessbyIA,
                        ProcessIAscore,
                        PolyGeometryPoints.STAsBinary() as PolyGeometryPoints
                        FROM [RevistasIA].[docs].[PagePrecrop]
                        WHERE FK_LabelDoc = @fk AND FK_ObjectsLayer > 0                  
                        ORDER BY PK_PagePrecrop DESC;";
                    }
                    else if (filter == 3)
                    {
                        squery = @"
                        SELECT TOP (@take) 
                        PK_PagePrecrop,
                        FK_Page,
                        fileNameCrop,
                        dirnameCrop,
                        FK_node,
                        PolyTextGeometry,
                        PolyTextYoloFormat,
                        FK_LabelDoc,
                        widthPct,
                        heightPct,
                        Points2D,
                        FK_ObjectsLayer,
                        ProcessbyIA,
                        ProcessIAscore,
                        PolyGeometryPoints.STAsBinary() as PolyGeometryPoints
                        FROM [RevistasIA].[docs].[PagePrecrop]
                        WHERE FK_LabelDoc = @fk                   
                        ORDER BY PK_PagePrecrop DESC;";
                    }
                    else if(filter == 2)
                    {
                        squery = @$"
                        SELECT TOP (@take) 
                        PK_PagePrecrop,
                        FK_Page,
                        fileNameCrop,
                        dirnameCrop,
                        FK_node,
                        PolyTextGeometry,
                        PolyTextYoloFormat,
                        FK_LabelDoc,
                        widthPct,
                        heightPct,
                        Points2D,
                        FK_ObjectsLayer,
                        ProcessbyIA,
                        ProcessIAscore,
                        PolyGeometryPoints.STAsBinary() as PolyGeometryPoints
                        FROM [RevistasIA].[docs].[PagePrecrop]
                        WHERE FK_LabelDoc = @fk AND precorteOCR = 1                  
                        ORDER BY PK_PagePrecrop DESC;";                       
                    }
                    else
                    {
                        squery = @$"
                        SELECT TOP (@take) 
                        PK_PagePrecrop,
                        FK_Page,
                        fileNameCrop,
                        dirnameCrop,
                        FK_node,
                        PolyTextGeometry,
                        PolyTextYoloFormat,
                        FK_LabelDoc,
                        widthPct,
                        heightPct,
                        Points2D,
                        FK_ObjectsLayer,
                        ProcessbyIA,
                        ProcessIAscore,
                        PolyGeometryPoints.STAsBinary() as PolyGeometryPoints
                        FROM [RevistasIA].[docs].[PagePrecrop]
                        WHERE FK_LabelDoc = @fk AND ProcessbyIA = {filter} AND precorteOCR = 0                  
                        ORDER BY PK_PagePrecrop DESC;";
                    }
                    lista = cn.Query<PagePrecrop>(squery, new {fk=pk, take}).ToList();
                }
                return lista;

            }
            catch (Exception)
            {
                return null;

            }
        }

        public List<PagePrecrop> GetPagePrecropsByFk(int pk)
        {
            try
            {
                List<PagePrecrop> lista = new List<PagePrecrop>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    //string squery = string.Format("select *  from  [MetaDocs].[dbo].[PagePrecrop] where FK_LabelDoc ={0}", pk);
                    const string squery = @"
                    SELECT *
                    FROM [RevistasIA].[docs].[PagePrecrop]
                    WHERE FK_LabelDoc = @fk;
                    ";

                    lista = cn.Query<PagePrecrop>(squery, new { fk = pk}).ToList();
                }
                return lista;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public sysNode GetNodeByFk(int fK_Node)
        {
            try
            {
                sysNode node = new sysNode();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("SELECT  *  FROM [RevistasIA].[core].[Nodes] where PK_Node={0} ", fK_Node);
                    node = cn.QueryFirst<sysNode>(squery, null, null, 0, CommandType.Text);
                }
                return node;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public List<Categoria> GetCat()
        {
            try
            {
                List<Categoria> list = new List<Categoria>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("SELECT  *  FROM [RevistasIA].[core].[Cats]");
                    list = cn.Query<Categoria>(squery).ToList();
                }
                return list;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public ObjectsLayers GetObjetsLayers(int fK)
        {
            ObjectsLayers objectsLayers = new ObjectsLayers();
            try
            {
               
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("SELECT  *  FROM [RevistasIA].[obj].[ObjectsLayers] where PK_ObjectsLayer={0} ", fK);
                    objectsLayers = cn.QueryFirst<ObjectsLayers>(squery, null, null, 0, CommandType.Text);
                }
                return objectsLayers;
            }
            catch (Exception)
            {

                return objectsLayers;
            }
        }

        public Paginas GetPage(int pk_page)
        {
            Paginas paginas = new Paginas();
            try
            {
                
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("SELECT  *  FROM [RevistasIA].[docs].[Pages] where PK_Page={0} ", pk_page);
                    paginas = cn.QueryFirst<Paginas>(squery, null, null, 0, CommandType.Text);
                }
                return paginas;
            }
            catch (Exception)
            {

                return paginas;
            }
        }

        public LabelDoc GetLabelDocByPK(int PK_LabelDoc)
        {
            LabelDoc ldoc = new LabelDoc();
            try
            {

                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("SELECT  *  FROM [RevistasIA].[docs].[LabelDoc] where PK_LabelDoc={0} ", PK_LabelDoc);
                    ldoc = cn.QueryFirst<LabelDoc>(squery, null, null, 0, CommandType.Text);
                }
                return ldoc;
            }
            catch (Exception)
            {

                return ldoc;
            }
        }

        public List<ObjectMaster> GetObjectsLayersByPkGroup(int fkObjectGroup)
        {
            try
            {
                List<ObjectMaster> lista = new List<ObjectMaster>();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    
                    const string squery = @"
                        SELECT
                        om.PK_ObjectMaster,om.nameObject,
                        COUNT(ol.PK_ObjectsLayer) AS cantidad
                        FROM [RevistasIA].[obj].[ObjectMaster] AS om
                        LEFT JOIN [RevistasIA].[obj].[ObjectsLayers] AS ol
                        ON ol.FK_ObjectMaster = om.PK_ObjectMaster
                        WHERE om.FK_ObjectGroup = @Grupo
                        GROUP BY om.PK_ObjectMaster,om.nameObject
                        ORDER BY cantidad DESC,  om.PK_ObjectMaster, om.nameObject;
                    ";

                    lista = cn.Query<ObjectMaster>(squery, new { Grupo = fkObjectGroup }).ToList();
                }
                return lista;

            }
            catch (Exception)
            {
                return null;

            }
        }

        public LabelDoc GetLabelDocByFkObjMaster(int FK_ObjectMaster)
        {
            try
            {
                LabelDoc ld = new LabelDoc();
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    string squery = string.Format("SELECT  *  FROM [RevistasIA].[docs].[LabelDoc] where FK_ObjectMaster={0} ", FK_ObjectMaster);
                    ld = cn.QueryFirst<LabelDoc>(squery, null, null, 0, CommandType.Text);
                }
                return ld;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public List<PagePrecrop> GetCropsByPoint(int pageId, double x, double y, double HeightTotal)
        {
            try
            {
                var squery = @"
                select p.*
                from [RevistasIA].[docs].[PagePrecrop] p
                cross apply (
                select cast(p.PolyGeometryPoints as geometry).MakeValid() as geom
                ) g
                where p.FK_Page = @PageId
                and g.geom.STContains(geometry::Point(@X,@HeightTotal - @Y, 0)) = 1";

                var param = new
                {
                    PageId = pageId,
                    X = x,
                    Y = y,
                    HeightTotal = HeightTotal
                };
                using (SqlConnection cn = new SqlConnection(conexion))
                {
                    return cn.Query<PagePrecrop>(squery, param).ToList();
                }
            }
            catch (Exception)
            {

                return null;
            }
            
        }

    }
}
