using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormHistorial : Form
    {
        private DataGridView _grid;

        public FormHistorial()
        {
            InitializeUi();
            Recargar();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 960, 560, "Historial de Resultados");
            Tablero.Cabecera(this, "HISTORIAL DE RESULTADOS",
                "Consulte los calculos de servicios guardados anteriormente.");

            Tablero.Boton(this, "ACTUALIZAR", 24, 96, 120, 36).Click += (s, e) => Recargar();
            Tablero.Boton(this, "ELIMINAR", 156, 96, 120, 36).Click += (s, e) => Eliminar();
            Tablero.Boton(this, "CERRAR", 288, 96, 120, 36, false).Click += (s, e) => Close();

            _grid = Tablero.Tabla(this, 14, 150, ClientSize.Width - 28, ClientSize.Height - 200);
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 40 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", DataPropertyName = "Fecha" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Vehiculo", DataPropertyName = "Vehiculo" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Conductor", DataPropertyName = "Conductor" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipo servicio", DataPropertyName = "TipoServicioTexto" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Km", DataPropertyName = "DistanciaKm" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Pasajeros", DataPropertyName = "Pasajeros", Width = 70 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Costo total", DataPropertyName = "CostoTotal" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Costoxkm", DataPropertyName = "CostoPorKm" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ganancia", DataPropertyName = "Ganancia" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Precio final", DataPropertyName = "PrecioFinal" });
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void Recargar()
        {
            var dt = ServicioService.ObtenerHistorial();
            dt.Columns.Add("TipoServicioTexto", typeof(string));
            foreach (DataRow fila in dt.Rows)
            {
                int codigo = Convert.ToInt32(fila["TipoServicio"]);
                fila["TipoServicioTexto"] = Models.EnumeracionesUI.Etiqueta((Models.TipoServicio)codigo);
            }

            _grid.DataSource = null;
            _grid.DataSource = dt;
        }

        private void Eliminar()
        {
            if (_grid.CurrentRow == null) return;
            var cell = _grid.CurrentRow.Cells["Id"];
            if (cell.Value == null || DBNull.Value.Equals(cell.Value)) return;

            int id = Convert.ToInt32(cell.Value);
            if (MessageBox.Show("Desea eliminar este registro del historial?", "Confirmacion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ServicioService.Eliminar(id);
                Recargar();
            }
        }
    }
}