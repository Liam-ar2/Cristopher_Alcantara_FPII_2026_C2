using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormConductores : Form
    {
        private DataGridView _grid;
        private List<Conductor> _conductores;

        public FormConductores()
        {
            InitializeUi();
            Recargar();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 800, 520, "Administrar Conductores");
            Tablero.Cabecera(this, "ADMINISTRAR CONDUCTORES",
                "Registre el personal que conduce los vehiculos.");

            Tablero.Boton(this, "NUEVO", 24, 96, 120, 36).Click += (s, e) => Nuevo();
            Tablero.Boton(this, "EDITAR", 156, 96, 120, 36).Click += (s, e) => Editar();
            Tablero.Boton(this, "ELIMINAR", 288, 96, 120, 36).Click += (s, e) => Eliminar();
            Tablero.Boton(this, "CERRAR", 420, 96, 120, 36, false).Click += (s, e) => Close();

            _grid = Tablero.Tabla(this, 14, 150, ClientSize.Width - 28, ClientSize.Height - 200);
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 40 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Documento", DataPropertyName = "Documento" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "Nombre" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Telefono", DataPropertyName = "Telefono" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Salario mensual", DataPropertyName = "SalarioTexto" });
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            _grid.DoubleClick += (s, e) => Editar();
        }

        private void Recargar()
        {
            _conductores = ConductorService.ObtenerTodos();
            _grid.DataSource = null;
            _grid.DataSource = _conductores;
        }

        private Conductor Seleccionado()
        {
            if (_grid.CurrentRow == null) return null;
            return _grid.CurrentRow.DataBoundItem as Conductor;
        }

        private void Nuevo()
        {
            using (var ed = new FormConductorEditor())
                if (ed.ShowDialog() == DialogResult.OK) Recargar();
        }

        private void Editar()
        {
            var c = Seleccionado();
            if (c == null) { MessageBox.Show("Seleccione un conductor de la lista.", "Proyecto"); return; }
            using (var ed = new FormConductorEditor(c))
                if (ed.ShowDialog() == DialogResult.OK) Recargar();
        }

        private void Eliminar()
        {
            var c = Seleccionado();
            if (c == null) { MessageBox.Show("Seleccione un conductor de la lista.", "Proyecto"); return; }
            if (MessageBox.Show("Desea eliminar al conductor " + c.Nombre + "?", "Confirmacion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ConductorService.Eliminar(c.Id);
                Recargar();
            }
        }
    }
}