using IVisionPrecutDocs.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using NetTopologySuite.Geometries;
using DevExpress.Office.DigitalSignatures.Internal;

namespace IVisionPrecutDocs.Data
{
    internal class Inserts
    {
        string cn = new Conexion().conexion;

        //public int InsertPrecrop(PagePrecrop pagePrecrop)
        //{
        //    using (IDbConnection db = new SqlConnection(cn))
        //    {
        //                string sql = @"
        //            SET NOCOUNT ON;

        //            DECLARE @tNew TABLE (NewID BIGINT);

        //            DECLARE @zonaGeometry geometry = NULL;
        //            DECLARE @polyGeometry  geometry = NULL;

        //            IF @PolyTextGeometryPoints IS NOT NULL
        //            BEGIN
        //                SET @zonaGeometry = geometry::STGeomFromText(@PolyTextGeometryPoints, 0);

        //                IF @zonaGeometry.STIsValid() = 0
        //                    SET @zonaGeometry = @zonaGeometry.MakeValid();
        //            END

        //            IF @PolyTextGeometry IS NOT NULL
        //            BEGIN
        //                SET @polyGeometry = geometry::STGeomFromText(@PolyTextGeometry, 0);

        //                IF @polyGeometry.STIsValid() = 0
        //                    SET @polyGeometry = @polyGeometry.MakeValid();
        //            END

        //            INSERT INTO [RevistasIA].[docs].[PagePrecrop]
        //            (
        //                FK_Page, fileNameCrop, dirnameCrop, FK_node,
        //                PolyTextGeometry, PolyGeometry, PolyTextYoloFormat,
        //                FK_LabelDoc, widthPct, heightPct, Points2D,
        //                FK_ObjectsLayer, ProcessbyIA, ProcessIAscore,
        //                PolyGeometryPoints, PolyTextGeometryPoints,
        //                PrecropPointsValidation, IsProcessIaTextExtracted, OCRText
        //                   )
        //                    OUTPUT INSERTED.PK_PagePrecrop INTO @tNew(NewID)
        //                    VALUES
        //                    (
        //                        @FK_Page,
        //                        @fileNameCrop,
        //                        @dirnameCrop,
        //                        @FK_node,
        //                        @PolyTextGeometry,
        //                        @polyGeometry,
        //                        @PolyTextYoloFormat,
        //                        @FK_LabelDoc,
        //                        @widthPct,
        //                        @heightPct,
        //                        @Points2D,
        //                        @FK_ObjectsLayer,
        //                        @ProcessbyIA,
        //                        @ProcessIAscore,
        //                        @zonaGeometry,
        //                        @PolyTextGeometryPoints,
        //                        @PrecropPointsValidation,
        //                        0,     -- Se mantiene en 0 por ahora
        //                        NULL   -- Aún no se extrae el texto OCR             
        //                    );

        //                    DECLARE @NewID BIGINT;
        //                    SELECT TOP (1) @NewID = NewID FROM @tNew;

        //                    -- Retorna el ID generado
        //                    SELECT CAST(@NewID AS INT) AS NewID;
        //                ";

        //        // Ejecuta y lee el nuevo ID usando Dapper
        //        int nuevoId = db.QuerySingle<int>(sql, new
        //        {
        //            pagePrecrop.FK_Page,
        //            pagePrecrop.fileNameCrop,
        //            pagePrecrop.dirnameCrop,
        //            pagePrecrop.FK_node,
        //            pagePrecrop.PolyTextGeometry,                   
        //            pagePrecrop.PolyTextYoloFormat,
        //            pagePrecrop.FK_LabelDoc,
        //            pagePrecrop.widthPct,
        //            pagePrecrop.heightPct,
        //            pagePrecrop.Points2D,
        //            pagePrecrop.FK_ObjectsLayer,
        //            pagePrecrop.ProcessbyIA,
        //            pagePrecrop.ProcessIAscore,
        //            pagePrecrop.PolyTextGeometryPoints,
        //            pagePrecrop.PrecropPointsValidation
        //});

        //    pagePrecrop.PK_PagePrecrop = nuevoId;
        //    return nuevoId;
        //}
        //}

        public int InsertPrecrop(PagePrecrop pagePrecrop)
        {
            using (IDbConnection db = new SqlConnection(cn))
            {
                string sql = @"
                SET NOCOUNT ON;

                DECLARE @tNew TABLE (NewID BIGINT);

                DECLARE @zonaGeometry geometry = NULL;
                DECLARE @polyGeometry  geometry = NULL;

                IF @PolyTextGeometryPoints IS NOT NULL
                BEGIN
                    SET @zonaGeometry = geometry::STGeomFromText(@PolyTextGeometryPoints, 0);

                    IF @zonaGeometry.STIsValid() = 0
                        SET @zonaGeometry = @zonaGeometry.MakeValid();
                END

                IF @PolyTextGeometry IS NOT NULL
                BEGIN
                    SET @polyGeometry = geometry::STGeomFromText(@PolyTextGeometry, 0);

                    IF @polyGeometry.STIsValid() = 0
                        SET @polyGeometry = @polyGeometry.MakeValid();
                END

                INSERT INTO [RevistasIA].[docs].[PagePrecrop]
                (
                    FK_Page, fileNameCrop, dirnameCrop, FK_node,
                    PolyTextGeometry, PolyGeometry, PolyTextYoloFormat,
                    FK_LabelDoc, widthPct, heightPct, Points2D,
                    FK_ObjectsLayer, ProcessbyIA, ProcessIAscore,
                    PolyGeometryPoints, PolyTextGeometryPoints,
                    PrecropPointsValidation, IsProcessIaTextExtracted, OCRText,
                    dateinclude -- <-- 1. Agregamos la columna aquí
                )
                OUTPUT INSERTED.PK_PagePrecrop INTO @tNew(NewID)
                VALUES
                (
                    @FK_Page,
                    @fileNameCrop,
                    @dirnameCrop,
                    @FK_node,
                    @PolyTextGeometry,
                    @polyGeometry,
                    @PolyTextYoloFormat,
                    @FK_LabelDoc,
                    @widthPct,
                    @heightPct,
                    @Points2D,
                    @FK_ObjectsLayer,
                    @ProcessbyIA,
                    @ProcessIAscore,
                    @zonaGeometry,
                    @PolyTextGeometryPoints,
                    @PrecropPointsValidation,
                    0,      -- Se mantiene en 0 por ahora
                    NULL,   -- Aún no se extrae el texto OCR               
                    GETDATE() -- <-- 2. Insertamos la fecha y hora actual del servidor
                );

                DECLARE @NewID BIGINT;
                SELECT TOP (1) @NewID = NewID FROM @tNew;

                

                DECLARE @TextoEncontrado NVARCHAR(MAX) = NULL;

                IF @zonaGeometry IS NOT NULL
                BEGIN
                    ;WITH PI AS
                    (
                        SELECT
                            simpleWord,
                            geometry::STGeomFromText(PolywraptextString, 0) AS g
                        FROM [RevistasIA].[docs].[PageIndex]
                        WHERE FK_Page = @FK_Page
                          AND PolywraptextString IS NOT NULL
                    )
                    SELECT
                        @TextoEncontrado = STRING_AGG(CONVERT(NVARCHAR(MAX), simpleWord), N' ')
                    FROM PI
                    WHERE g IS NOT NULL
                      AND g.STIsValid() = 1
                      AND @zonaGeometry.Filter(g) = 1
                      AND @zonaGeometry.STIntersects(g) = 1
                    OPTION (RECOMPILE);
                END

                UPDATE [RevistasIA].[docs].[PagePrecrop]
                SET OCRText = @TextoEncontrado
                WHERE PK_PagePrecrop = @NewID;

                SELECT CAST(@NewID AS INT) AS NewID;
        ";

                // Ejecuta y lee el nuevo ID usando Dapper
                int nuevoId = db.QuerySingle<int>(sql, new
                {
                    pagePrecrop.FK_Page,
                    pagePrecrop.fileNameCrop,
                    pagePrecrop.dirnameCrop,
                    pagePrecrop.FK_node,
                    pagePrecrop.PolyTextGeometry,
                    pagePrecrop.PolyTextYoloFormat,
                    pagePrecrop.FK_LabelDoc,
                    pagePrecrop.widthPct,
                    pagePrecrop.heightPct,
                    pagePrecrop.Points2D,
                    pagePrecrop.FK_ObjectsLayer,
                    pagePrecrop.ProcessbyIA,
                    pagePrecrop.ProcessIAscore,
                    pagePrecrop.PolyTextGeometryPoints,
                    pagePrecrop.PrecropPointsValidation
                });

                pagePrecrop.PK_PagePrecrop = nuevoId;
                return nuevoId;
            }
        }

        public int InsertLabelDoc(int Fkcat, string nameLabel, double widthPct, double heightPct, int fkObjMaster, int fkObjGroup)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(cn))
                {
                    const string sql = @"
                    INSERT INTO [RevistasIA].[docs].[LabelDoc] ([FK_cat], [namelabel], [widthPct], [heightPct], [FK_ObjectMaster], [FK_ObjectGroup]) 
                    OUTPUT INSERTED.PK_LabelDoc
                    VALUES (@FK_cat, @namelabel, @widthPct, @heightPct, @FK_ObjectMaster, @FK_ObjectGroup)
                    ";
                    int nuevoId = db.QuerySingle<int>(sql, new
                    {
                        FK_cat = Fkcat,
                        namelabel = nameLabel,
                        widthPct = widthPct,
                        heightPct = heightPct,
                        FK_ObjectMaster = fkObjMaster,
                        FK_ObjectGroup = fkObjGroup
                    });
                    return nuevoId;
                }
            }
            catch (Exception)
            {
                return 0;
            }           
        }

        public int InsertDoc(string dateInclude, int fkCat, int fkNode)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(cn))
                {
                    const string sql = @"
                    INSERT INTO [RevistasIA].[docs].[Docs] ([dateCreated], [dateInclude], [FK_cat], [FK_Node]) 
                    OUTPUT INSERTED.[Pk_Doc]
                    VALUES (GETDATE(), @dateInclude, @fkCat, @fkNode)
                    ";
                    int nuevoId = db.QuerySingle<int>(sql, new
                    {
                        dateInclude = dateInclude,
                        fkCat = fkCat,
                        fkNode = fkNode
                    });
                    return nuevoId;
                }

            }
            catch (Exception)
            {
                return 0;
            }
            
        }

        public int InsertPage(int fkdoc, int pagenumbert, int fkNode, string fileNameFullJPG, 
            string dirNameFullJPG, string fileNameMiniJPG, int PageheightPX, int PagewidthPX, 
            int dpi, int taskPageOcr, string OcrText, int taskPagePrecutbyIA,int PageheightPoints, int PagewidthPoints)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(cn))
                {
                    const string sql = @"
                    INSERT INTO [RevistasIA].[docs].[Pages] (
                    [FK_doc],
                    [dateCreated],
                    [pagenumber],
                    [FK_Node],
                    [fileNameFullJPG],
                    [dirNameFullJPG],
                    [fileNameMiniJPG],
                    [PageheightPX],
                    [PagewidthPX],
                    [dpi],
                    [taskPageOcr],
                    [OcrText],
                    [taskPagePrecutbyIA],
                    [PageheightPoints],
                    [PagewidthPoints]
                    )
                    OUTPUT INSERTED.PK_Page
                    VALUES (
                    @fkdoc,
                    GETDATE(),
                    @pagenumbert,
                    @fkNode,
                    @fileNameFullJPG,
                    @dirNameFullJPG,
                    @fileNameMiniJPG,
                    @PageheightPX,
                    @PagewidthPX,
                    @dpi,
                    @taskPageOcr,
                    @OcrText,
                    @taskPagePrecutbyIA,
                    @PageheightPoints,
                    @PagewidthPoints)
                    ";
                    return db.QuerySingle<int>(sql, new
                    {
                        fkdoc = fkdoc,
                        pagenumbert = pagenumbert,
                        fkNode = fkNode,
                        fileNameFullJPG = fileNameFullJPG,
                        dirNameFullJPG = dirNameFullJPG,
                        fileNameMiniJPG = fileNameMiniJPG,
                        PageheightPX = PageheightPX,
                        PagewidthPX = PagewidthPX,
                        dpi = dpi,
                        taskPageOcr = taskPageOcr,
                        OcrText = OcrText,
                        taskPagePrecutbyIA = taskPagePrecutbyIA,
                        PageheightPoints = PageheightPoints,
                        PagewidthPoints = PagewidthPoints
                    });
                    //return true;
                }

            }
            catch (Exception)
            {
                return 0;
            }

        }

        public void InsertCat(string nameCat)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(cn))
                {
                    const string sql = @"
                    INSERT INTO [RevistasIA].[core].[Cats] ([Cat],[dateCreated]) 
                    VALUES (@nameCat,GETDATE())
                    ";
                    int nuevoId = db.QuerySingle<int>(sql, new
                    {
                        nameCat = nameCat
                    });
                }
            }
            catch (Exception)
            {
            }
        }

        public int InsertObjetcMaster(string name, string detail, int fkobGroup, int active = 1, int rgb = -256)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(cn))
                {
                    const string sql = @"
                    INSERT INTO [RevistasIA].[obj].[ObjectMaster] ([nameObject], [detail], [FK_ObjectGroup], [active], [rgb]) 
                    OUTPUT INSERTED.PK_ObjectMaster
                    VALUES (@nameObject, @detail, @FK_ObjectGroup, @active, @rgb)
                    ";
                    int nuevoId = db.QuerySingle<int>(sql, new
                    {
                        nameObject = name,
                        detail = detail,
                        FK_ObjectGroup = fkobGroup,
                        active = active,
                        rgb = rgb
                    });
                    return nuevoId;
                }
                    
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int InsertObjectsLayers(string dirObject,
        int FK_ObjectMaster, string dirObjectMini,
        string TextYoloScale, string Points2D, int FK_Node)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(cn))
                {
                    const string sql = @"
                    INSERT INTO [RevistasIA].[obj].[ObjectsLayers] ([dirObject], [FK_ObjectMaster], 
                    [dirObjectMini], [TextYoloScale], [Points2D], [FK_Node]) 
                    OUTPUT INSERTED.PK_ObjectsLayer
                    VALUES (@dirObject, @FK_ObjectMaster, @dirObjectMini,
                    @TextYoloScale, @Points2D, @FK_Node)
                    ";
                    int nuevoId = db.QuerySingle<int>(sql, new
                    {
                        dirObject = dirObject,
                        //fileName = fileName,
                        FK_ObjectMaster = FK_ObjectMaster,
                        dirObjectMini = dirObjectMini,
                        //fileNameMini = fileNameMini,
                        TextYoloScale = TextYoloScale,
                        Points2D = Points2D,
                        FK_Node = FK_Node
                    });
                    return nuevoId;
                }

            }
            catch (Exception)
            {
                return 0;
            }
        }
    }   
}
