namespace IVisionPrecutDocs
{
    partial class DetailsPrecut
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureEditPrecut = new DevExpress.XtraEditors.PictureEdit();
            this.pictureEditObj = new DevExpress.XtraEditors.PictureEdit();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblnp = new System.Windows.Forms.Label();
            this.lblFechaPage = new System.Windows.Forms.Label();
            this.lblNumPage = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.sbtnMove = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditPrecut.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditObj.Properties)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureEditPrecut, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureEditObj, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.sbtnMove, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(702, 456);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureEditPrecut
            // 
            this.pictureEditPrecut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureEditPrecut.Location = new System.Drawing.Point(3, 58);
            this.pictureEditPrecut.Name = "pictureEditPrecut";
            this.pictureEditPrecut.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEditPrecut.Size = new System.Drawing.Size(345, 365);
            this.pictureEditPrecut.TabIndex = 0;
            // 
            // pictureEditObj
            // 
            this.pictureEditObj.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureEditObj.Location = new System.Drawing.Point(354, 58);
            this.pictureEditObj.Name = "pictureEditObj";
            this.pictureEditObj.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
            this.pictureEditObj.Size = new System.Drawing.Size(345, 365);
            this.pictureEditObj.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblNumPage, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblFecha, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblnp, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblFechaPage, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(345, 49);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // lblFecha
            // 
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(5, 5);
            this.lblFecha.Margin = new System.Windows.Forms.Padding(5);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(40, 13);
            this.lblFecha.TabIndex = 0;
            this.lblFecha.Text = "Fecha:";
            // 
            // lblnp
            // 
            this.lblnp.AutoSize = true;
            this.lblnp.Location = new System.Drawing.Point(5, 29);
            this.lblnp.Margin = new System.Windows.Forms.Padding(5);
            this.lblnp.Name = "lblnp";
            this.lblnp.Size = new System.Drawing.Size(77, 13);
            this.lblnp.TabIndex = 1;
            this.lblnp.Text = "Numero de pg:";
            // 
            // lblFechaPage
            // 
            this.lblFechaPage.AutoSize = true;
            this.lblFechaPage.Location = new System.Drawing.Point(95, 5);
            this.lblFechaPage.Margin = new System.Windows.Forms.Padding(5);
            this.lblFechaPage.Name = "lblFechaPage";
            this.lblFechaPage.Size = new System.Drawing.Size(35, 13);
            this.lblFechaPage.TabIndex = 2;
            this.lblFechaPage.Text = "label1";
            // 
            // lblNumPage
            // 
            this.lblNumPage.AutoSize = true;
            this.lblNumPage.Location = new System.Drawing.Point(95, 29);
            this.lblNumPage.Margin = new System.Windows.Forms.Padding(5);
            this.lblNumPage.Name = "lblNumPage";
            this.lblNumPage.Size = new System.Drawing.Size(35, 13);
            this.lblNumPage.TabIndex = 3;
            this.lblNumPage.Text = "label2";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(354, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(345, 49);
            this.tableLayoutPanel3.TabIndex = 3;
            // 
            // sbtnMove
            // 
            this.sbtnMove.Location = new System.Drawing.Point(3, 429);
            this.sbtnMove.Name = "sbtnMove";
            this.sbtnMove.Size = new System.Drawing.Size(75, 23);
            this.sbtnMove.TabIndex = 4;
            this.sbtnMove.Text = "Mover";
            this.sbtnMove.Click += new System.EventHandler(this.sbtnMove_Click);
            // 
            // DetailsPrecut
            // 
            this.ClientSize = new System.Drawing.Size(702, 456);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DetailsPrecut";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditPrecut.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEditObj.Properties)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.PictureEdit pictureEditPrecut;
        private DevExpress.XtraEditors.PictureEdit pictureEditObj;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblnp;
        private System.Windows.Forms.Label lblNumPage;
        private System.Windows.Forms.Label lblFechaPage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private DevExpress.XtraEditors.SimpleButton sbtnMove;
    }
}
