using System;
using System.Windows.Forms;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormCostoEditor : Form
    {
        private readonly Vehiculo _vehiculo;
        private readonly CostoVehiculo _costo;

        private TextBox _txtNombre;
        private ComboBox _cmbCategoria, _cmbTipo, _cmbPeriodicidad;
        private NumericUpDown _numMonto;

        public FormCostoEditor(Vehiculo vehiculo, CostoVehiculo costo = null)
        {
            _vehiculo = vehiculo;
            _costo = costo ?? new CostoVehiculo { VehiculoId = vehiculo.Id };
            InitializeUi();
            Cargar();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 440, 420, _costo.Id == 0 ? "Nuevo Costo" : "Editar Costo");
            Tablero.Cabecera(this, "REGISTRAR COSTO", "Vehiculo: " + _vehiculo.Descripcion);

            int x = 36, y = 130, w = 370;

            Tablero.Etiqueta(this, "Nombre del costo *", x, y + 3, w);
            _txtNombre = Tablero.CajaTexto(this, x, y + 22, w);
            y += 62;

            Tablero.Etiqueta(this, "Categoria", x, y + 3, 180);
            _cmbCategoria = Tablero.Combo(this, x, y + 22, w);
            EnumeracionesUI.LlenarCombo<CategoriaCosto>(_cmbCategoria);
            y += 62;

            Tablero.Etiqueta(this, "Tipo de costo", x, y + 3, 180);
            _cmbTipo = Tablero.Combo(this, x, y + 22, w);
            EnumeracionesUI.LlenarCombo<TipoCosto>(_cmbTipo);
            _cmbTipo.SelectedIndexChanged += (s, e) => ActualizarPeriodos();
            y += 62;

            Tablero.Etiqueta(this, "Periodicidad", x, y + 3, 180);
            _cmbPeriodicidad = Tablero.Combo(this, x, y + 22, w);
            y += 62;

            Tablero.Etiqueta(this, "Monto (RD$) *", x, y + 3, 180);
            _numMonto = Tablero.Numerico(this, x, y + 22, w, 0, 50000000, 2);
            y += 62;

            Tablero.Boton(this, "GUARDAR", x, y + 40, 170, 40).Click += (s, e) => Guardar();
            Tablero.Boton(this, "CANCELAR", x + 190, y + 40, 170, 40, false).Click += (s, e) => Close();
        }

        private void ActualizarPeriodos()
        {
            var tipo = EnumeracionesUI.ValorCombo(_cmbTipo, TipoCosto.Fijo);
            _cmbPeriodicidad.Items.Clear();
            if (tipo == TipoCosto.Fijo)
            {
                _cmbPeriodicidad.Items.Add(new ComboItem<PeriodicidadCosto>(PeriodicidadCosto.Mensual, "Mensual"));
                _cmbPeriodicidad.Items.Add(new ComboItem<PeriodicidadCosto>(PeriodicidadCosto.Semanal, "Semanal"));
                _cmbPeriodicidad.Items.Add(new ComboItem<PeriodicidadCosto>(PeriodicidadCosto.Diario, "Diario"));
                _cmbPeriodicidad.SelectedIndex = 0;
            }
            else
            {
                _cmbPeriodicidad.Items.Add(new ComboItem<PeriodicidadCosto>(PeriodicidadCosto.PorKilometro, "Por km"));
                _cmbPeriodicidad.SelectedIndex = 0;
            }
        }

        private void Cargar()
        {
            _txtNombre.Text = _costo.Nombre;
            EnumeracionesUI.SeleccionarValor(_cmbCategoria, _costo.Categoria);
            EnumeracionesUI.SeleccionarValor(_cmbTipo, _costo.Tipo);
            ActualizarPeriodos();
            EnumeracionesUI.SeleccionarValor(_cmbPeriodicidad, _costo.Periodicidad);
            _numMonto.Value = _costo.Monto;
        }

        private void Guardar()
        {
            _costo.Nombre = _txtNombre.Text.Trim();
            _costo.Categoria = EnumeracionesUI.ValorCombo(_cmbCategoria, CategoriaCosto.Otros);
            _costo.Tipo = EnumeracionesUI.ValorCombo(_cmbTipo, TipoCosto.Fijo);
            _costo.Periodicidad = EnumeracionesUI.ValorCombo(_cmbPeriodicidad,
                _costo.Tipo == TipoCosto.Fijo ? PeriodicidadCosto.Mensual : PeriodicidadCosto.PorKilometro);
            _costo.Monto = _numMonto.Value;

            var errores = Validaciones.ValidarCosto(_costo);
            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Datos invalidos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CostoService.Guardar(_costo);
            DialogResult = DialogResult.OK;
        }
    }
}