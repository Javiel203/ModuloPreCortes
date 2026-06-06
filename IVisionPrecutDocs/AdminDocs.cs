using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using IVisionDrawSelection;
using IVisionPrecutDocs.Data;
using IVisionPrecutDocs.Entities;
using IVisionPrecutDocs.Proceso;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IVisionPrecutDocs
{
    public partial class AdminDocs : Form
    {
        private ObservableCollection<Categoria> CatsObservable;
        private readonly ContextMenuStrip rowMenuGrid1 = new ContextMenuStrip();
        int idUpdate = 0;
        public AdminDocs()
        {
            InitializeComponent();
            this.Load += AdminDocs_Load;
            ToolStripMenuItem mEdit = new ToolStripMenuItem("Editar", null, OnEditarClick);
            ToolStripMenuItem mAdd = new ToolStripMenuItem("Agregar Documento", null, OnAgregarDocClick);
            ToolStripMenuItem mDrop = new ToolStripMenuItem("Eliminar", null, OnEliminarClick);        
            rowMenuGrid1.Items.AddRange(new ToolStripItem[] { mEdit, mAdd, mDrop,  new ToolStripSeparator() });
            gridView1.PopupMenuShowing += GridView1_PopupMenuShowing;
        }

        private void AdminDocs_Load(object sender, EventArgs e)
        {
            
            FormatGridControl1();

        }

        private void OnEditarClick(object sender, EventArgs e)
        {
            var main = gridControl1.MainView as ColumnView;
            Categoria cat = main.GetRow(main.FocusedRowHandle) as Categoria;
            txtcat.Text = cat.Cat;
            btnAgregar.Text = "Actualizar";
            idUpdate = cat.PK_cat;
        }

        private void OnAgregarDocClick(object sender, EventArgs e)
        {
            addDocs add = new addDocs();
            var main = gridControl1.MainView as ColumnView;
            Categoria cat = main.GetRow(main.FocusedRowHandle) as Categoria;
            add.fk_cat = cat.PK_cat;
            add.nameCat = cat.Cat;
            add.Show();
        }

        private void OnEliminarClick(object sender, EventArgs e)
        {
            var main = gridControl1.MainView as ColumnView;
            Categoria cat = main.GetRow(main.FocusedRowHandle) as Categoria;
            new Delete().DeleteCategoriaById(cat.PK_cat);
            FormatGridControl1();

            //MessageBox.Show(cat.PK_cat.ToString());
        }



        private void GridView1_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow || e.HitInfo.InRowCell)
            {
                var view = (GridView)sender;
                view.FocusedRowHandle = e.HitInfo.RowHandle;
                e.Allow = false;
                rowMenuGrid1.Show(gridControl1, e.Point);
            }
        }

        public void FormatGridControl1()
        {
            List<Categoria> list = new Consults().GetCat();
            CatsObservable = new ObservableCollection<Categoria>(list);
            gridControl1.DataSource = CatsObservable;
            gridView1.PopulateColumns();
            gridView1.Columns["PK_cat"].Visible = false;
            gridView1.Columns["Cat"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["Cat"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["Cat"].OptionsColumn.AllowSize = false;
            //gridView1.Columns["Categoria"].Width = 250;
            gridView1.Columns["Cat"].Caption = "Nombre de Categoria";           
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtcat.Text))
            {
                if (btnAgregar.Text =="Actualizar" && idUpdate != 0)
                {
                    new Updats().UpdateCatPk(idUpdate,txtcat.Text);
                    idUpdate = 0;
                    txtcat.Clear();
                    btnAgregar.Text = "Agregar";
                }
                else
                {
                    new Inserts().InsertCat(txtcat.Text);
                    txtcat.Clear();
                }
                    
                FormatGridControl1();
            }
        }
    }
}
