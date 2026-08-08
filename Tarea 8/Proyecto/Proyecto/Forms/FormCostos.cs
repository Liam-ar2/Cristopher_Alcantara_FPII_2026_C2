using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormCostos : Form
    {
        private ComboBox _cmbVehiculo;
        private NumericUpDown _numPrecio;
        private RadioButton _rbGalon, _rbLitro;
        private DataGridView _grid;
        private Button _btnEditar, _btnEliminar;

        private List<Vehiculo> _vehiculos;

        public FormCostos()
        {
            InitializeUi();
            CargarDatos();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 720, 600, "Administrar Costos");
            Tablero.Cabecera(this, "ADMINISTRAR COSTOS",
                "Registre el precio del combustible y los costos fijos/variables de cada vehiculo.");

            // Fila 1: seleccion de vehiculo.
            Tablero.Etiqueta(this, "Vehiculo:", 24, 102, 100, true);
            _cmbVehiculo = Tablero.Combo(this, 130, 96, 280);
            _cmbVehiculo.SelectedIndexChanged += (s, e) => RecargarCostos();
            Tablero.Boton(this, "NUEVO COSTO", 440, 96, 130, 34).Click += (s, e) => NuevoCosto();

            // Fila 2: precio global del combustible.
            Tablero.Etiqueta(this, "Precio de combustible (RD$):", 24, 148, 240, true);
            _numPrecio = Tablero.Numerico(this, 250, 144, 110, 0, 100000, 2);
            _rbGalon = new RadioButton { Text = "por galon", Location = new System.Drawing.Point(380, 148), AutoSize = true, Checked = true };
            _rbLitro = new RadioButton { Text = "por litro", Location = new System.Drawing.Point(475, 148), AutoSize = true };
            Controls.Add(_rbGalon);
            Controls.Add(_rbLitro);
            Tablero.Boton(this, "GUARDAR PRECIO", 585, 144, 115, 28).Click += (s, e) => GuardarCombustible();

            // Grilla de costos del vehiculo seleccionado.
            _grid = Tablero.Tabla(this, 14, 205, ClientSize.Width - 28, 320);
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", DataPropertyName = "Id", Width = 40 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", DataPropertyName = "Nombre" });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Categoria", HeaderText = "Categoria", DataPropertyName = "Categoria", AutoSizeMode = DataGridViewAutoSizeColumnMode.None, FillWeight = 20 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tipo", HeaderText = "Tipo", DataPropertyName = "Tipo", AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Periodicidad", HeaderText = "Periodicidad", DataPropertyName = "Periodicidad", AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Monto", HeaderText = "Monto", DataPropertyName = "Monto", AutoSizeMode = DataGridViewAutoSizeColumnMode.None });
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Botones de edicion y salida.
            _btnEditar = Tablero.Boton(this, "EDITAR", 14, 486, 110, 36);
            _btnEditar.Click += (s, e) => EditarCosto();
            _btnEliminar = Tablero.Boton(this, "ELIMINAR", 136, 486, 110, 36);
            _btnEliminar.Click += (s, e) => EliminarCosto();
            Tablero.Boton(this, "CERRAR", 258, 486, 110, 36, false).Click += (s, e) => Close();

            _grid.DoubleClick += (s, e) => EditarCosto();
        }

        private void CargarDatos()
        {
            _vehiculos = VehiculoService.ObtenerTodos();
            _cmbVehiculo.Items.Clear();
            foreach (var v in _vehiculos)
                _cmbVehiculo.Items.Add(new ComboItem<Vehiculo>(v, v.Descripcion));
            if (_cmbVehiculo.Items.Count > 0) _cmbVehiculo.SelectedIndex = 0;

            var cfg = ConfigService.Obtener();
            _numPrecio.Value = cfg.PrecioCombustible;
            if (cfg.UnidadCombustible == UnidadCombustible.Litro) _rbLitro.Checked = true;
            else _rbGalon.Checked = true;
        }

        private Vehiculo VehiculoSeleccionado()
        {
            if (_vehiculos == null || _cmbVehiculo.SelectedItem == null) return null;
            return ((ComboItem<Vehiculo>)_cmbVehiculo.SelectedItem).Valor;
        }

        private void RecargarCostos()
        {
            var v = VehiculoSeleccionado();
            if (v == null) { _grid.DataSource = null; return; }

            var items = new List<CostoVehiculo>(CostoService.ObtenerPorVehiculo(v.Id));
            _grid.DataSource = null;
            _grid.DataSource = items;

            foreach (DataGridViewRow fila in _grid.Rows)
            {
                fila.Cells["Categoria"].Value = EnumeracionesUI.Etiqueta(((CostoVehiculo)fila.DataBoundItem).Categoria);
                fila.Cells["Tipo"].Value = EnumeracionesUI.Etiqueta(((CostoVehiculo)fila.DataBoundItem).Tipo);
                fila.Cells["Periodicidad"].Value = EnumeracionesUI.Etiqueta(((CostoVehiculo)fila.DataBoundItem).Periodicidad);
                fila.Cells["Monto"].Value = Formato.Moneda(((CostoVehiculo)fila.DataBoundItem).Monto);
            }
        }

        private void GuardarCombustible()
        {
            var cfg = new ConfiguracionGlobal
            {
                PrecioCombustible = _numPrecio.Value,
                UnidadCombustible = _rbGalon.Checked ? UnidadCombustible.Galon : UnidadCombustible.Litro
            };
            ConfigService.Guardar(cfg);
            MessageBox.Show("Precio de combustible actualizado.", "Proyecto",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void NuevoCosto()
        {
            var v = VehiculoSeleccionado();
            if (v == null)
            {
                MessageBox.Show("Primero debe registrar un vehiculo.", "Proyecto",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var ed = new FormCostoEditor(v))
                if (ed.ShowDialog() == DialogResult.OK) RecargarCostos();
        }

        private void EditarCosto()
        {
            var v = VehiculoSeleccionado();
            var costo = _grid.CurrentRow?.DataBoundItem as CostoVehiculo;
            if (v == null || costo == null)
            {
                MessageBox.Show("Seleccione un costo de la lista.", "Proyecto");
                return;
            }
            using (var ed = new FormCostoEditor(v, costo))
                if (ed.ShowDialog() == DialogResult.OK) RecargarCostos();
        }

        private void EliminarCosto()
        {
            var costo = _grid.CurrentRow?.DataBoundItem as CostoVehiculo;
            if (costo == null)
            {
                MessageBox.Show("Seleccione un costo de la lista.", "Proyecto");
                return;
            }
            if (MessageBox.Show("Desea eliminar el costo seleccionado?", "Confirmacion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CostoService.Eliminar(costo.Id);
                RecargarCostos();
            }
        }
    }
}