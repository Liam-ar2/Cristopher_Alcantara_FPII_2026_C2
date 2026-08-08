using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormVehiculoEditor : Form
    {
        private readonly Vehiculo _vehiculo;

        private TextBox _txtPlaca, _txtMarca, _txtModelo;
        private ComboBox _cmbTipo;
        private NumericUpDown _numPasajeros, _numCarga, _numRendimiento, _numKmMes;
        private RadioButton _rbGalon, _rbLitro;

        public FormVehiculoEditor(Vehiculo vehiculo = null)
        {
            _vehiculo = vehiculo ?? new Vehiculo();
            InitializeUi();
            CargarVehiculo();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 600, 630, _vehiculo.Id == 0 ? "Nuevo Vehiculo" : "Editar Vehiculo");
            Tablero.Cabecera(this, _vehiculo.Id == 0 ? "REGISTRAR VEHICULO" : "EDITAR VEHICULO",
                "Complete los datos del vehiculo. Los campos marcados con * son obligatorios.");

            int xl = 40, xr = 320, w = 210, paso = 74, y = 118;

            // Columna izquierda.
            Tablero.Etiqueta(this, "Placa *", xl, y + 3, w);
            _txtPlaca = Tablero.CajaTexto(this, xl, y + 22, w);
            y += paso;

            Tablero.Etiqueta(this, "Marca *", xl, y + 3, w);
            _txtMarca = Tablero.CajaTexto(this, xl, y + 22, w);
            y += paso;

            Tablero.Etiqueta(this, "Modelo *", xl, y + 3, w);
            _txtModelo = Tablero.CajaTexto(this, xl, y + 22, w);
            y += paso;

            Tablero.Etiqueta(this, "Tipo de vehiculo", xl, y + 3, w);
            _cmbTipo = Tablero.Combo(this, xl, y + 22, w);
            EnumeracionesUI.LlenarCombo<TipoVehiculo>(_cmbTipo);
            y += paso;

            Tablero.Etiqueta(this, "Capacidad de pasajeros *", xl, y + 3, w);
            _numPasajeros = Tablero.Numerico(this, xl, y + 22, w, 0, 2000, 0);

            // Columna derecha.
            y = 118;
            Tablero.Etiqueta(this, "Capacidad de carga (kg)", xr, y + 3, w);
            _numCarga = Tablero.Numerico(this, xr, y + 22, w, 0, 1000000, 0);
            y += paso;

            Tablero.Etiqueta(this, "Rendimiento (km/unidad) *", xr, y + 3, w);
            _numRendimiento = Tablero.Numerico(this, xr, y + 22, w, 0, 1000, 2, 0.5m);
            y += paso;

            Tablero.Etiqueta(this, "Unidad de combustible", xr, y + 3, w);
            _rbGalon = new RadioButton
            {
                Text = "Por galon",
                Location = new System.Drawing.Point(xr, y + 20),
                AutoSize = true,
                Checked = true,
                Font = new System.Drawing.Font(Tablero.Fuente, 9.5f)
            };
            _rbLitro = new RadioButton
            {
                Text = "Por litro",
                Location = new System.Drawing.Point(xr + 100, y + 20),
                AutoSize = true,
                Font = new System.Drawing.Font(Tablero.Fuente, 9.5f)
            };
            Controls.Add(_rbGalon);
            Controls.Add(_rbLitro);
            y += paso;

            Tablero.Etiqueta(this, "Kilometraje mensual (km)*", xr, y + 3, w);
            _numKmMes = Tablero.Numerico(this, xr, y + 22, w, 0, 2000000, 0, 100);

            // Nota.
            var nota = new Label
            {
                Text = "El kilometraje mensual se usa para distribuir los costos fijos (seguro, salario) por km.",
                Location = new System.Drawing.Point(40, 476),
                Width = 480,
                ForeColor = Tablero.Suave,
                Font = new System.Drawing.Font(Tablero.Fuente, 8.5f)
            };
            Controls.Add(nota);

            Tablero.Boton(this, "GUARDAR", 120, 505, 150, 40).Click += (s, e) => Guardar();
            Tablero.Boton(this, "CANCELAR", 290, 505, 150, 40, false).Click += (s, e) => Close();
        }

        private void CargarVehiculo()
        {
            _txtPlaca.Text = _vehiculo.Placa;
            _txtMarca.Text = _vehiculo.Marca;
            _txtModelo.Text = _vehiculo.Modelo;
            EnumeracionesUI.SeleccionarValor(_cmbTipo, _vehiculo.Tipo);
            _numPasajeros.Value = _vehiculo.CapacidadPasajeros;
            _numCarga.Value = _vehiculo.CapacidadCargaKg;
            _numRendimiento.Value = _vehiculo.RendimientoPorUnidad;
            if (_vehiculo.UnidadCombustible == UnidadCombustible.Litro) _rbLitro.Checked = true;
            else _rbGalon.Checked = true;
            _numKmMes.Value = _vehiculo.KilometrajeMensual;
        }

        private void Guardar()
        {
            _vehiculo.Placa = _txtPlaca.Text.Trim();
            _vehiculo.Marca = _txtMarca.Text.Trim();
            _vehiculo.Modelo = _txtModelo.Text.Trim();
            _vehiculo.Tipo = EnumeracionesUI.ValorCombo(_cmbTipo, TipoVehiculo.Automovil);
            _vehiculo.CapacidadPasajeros = (int)_numPasajeros.Value;
            _vehiculo.CapacidadCargaKg = _numCarga.Value;
            _vehiculo.RendimientoPorUnidad = _numRendimiento.Value;
            _vehiculo.UnidadCombustible = _rbGalon.Checked ? UnidadCombustible.Galon : UnidadCombustible.Litro;
            _vehiculo.KilometrajeMensual = _numKmMes.Value;

            var errores = Validaciones.ValidarVehiculo(_vehiculo);
            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Datos invalidos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            VehiculoService.Guardar(_vehiculo);
            DialogResult = DialogResult.OK;
        }
    }
}