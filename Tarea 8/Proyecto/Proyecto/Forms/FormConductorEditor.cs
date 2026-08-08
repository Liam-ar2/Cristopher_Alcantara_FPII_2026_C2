using System;
using System.Windows.Forms;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormConductorEditor : Form
    {
        private readonly Conductor _conductor;
        private TextBox _txtDocumento, _txtNombre, _txtTelefono;
        private NumericUpDown _numSalario;

        public FormConductorEditor(Conductor conductor = null)
        {
            _conductor = conductor ?? new Conductor();
            InitializeUi();
            CargarConductor();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 460, 380, _conductor.Id == 0 ? "Nuevo Conductor" : "Editar Conductor");
            Tablero.Cabecera(this, _conductor.Id == 0 ? "REGISTRAR CONDUCTOR" : "EDITAR CONDUCTOR",
                "Datos del conductor y salario mensual.");

            int x = 40, y = 120, w = 380;

            Tablero.Etiqueta(this, "Documento de identidad *", x, y + 3, w);
            _txtDocumento = Tablero.CajaTexto(this, x, y + 22, w);
            y += 62;

            Tablero.Etiqueta(this, "Nombre completo *", x, y + 3, w);
            _txtNombre = Tablero.CajaTexto(this, x, y + 22, w);
            y += 62;

            Tablero.Etiqueta(this, "Telefono", x, y + 3, w);
            _txtTelefono = Tablero.CajaTexto(this, x, y + 22, w);
            y += 62;

            Tablero.Etiqueta(this, "Salario mensual (RD$) *", x, y + 3, w);
            _numSalario = Tablero.Numerico(this, x, y + 22, w, 0, 10000000, 2);
            y += 62;

            var nota = new Label
            {
                Text = "El salario se distribuye por km usando el kilometraje mensual del vehiculo.",
                Location = new System.Drawing.Point(x, y + 4),
                Width = w,
                ForeColor = Tablero.Suave,
                Font = new System.Drawing.Font(Tablero.Fuente, 8.5f)
            };
            Controls.Add(nota);

            Tablero.Boton(this, "GUARDAR", x, y + 44, 150, 40).Click += (s, e) => Guardar();
            Tablero.Boton(this, "CANCELAR", x + 170, y + 44, 150, 40, false).Click += (s, e) => Close();
        }

        private void CargarConductor()
        {
            _txtDocumento.Text = _conductor.Documento;
            _txtNombre.Text = _conductor.Nombre;
            _txtTelefono.Text = _conductor.Telefono;
            _numSalario.Value = _conductor.SalarioMensual;
        }

        private void Guardar()
        {
            _conductor.Documento = _txtDocumento.Text.Trim();
            _conductor.Nombre = _txtNombre.Text.Trim();
            _conductor.Telefono = _txtTelefono.Text.Trim();
            _conductor.SalarioMensual = _numSalario.Value;

            var errores = Validaciones.ValidarConductor(_conductor);
            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Datos invalidos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConductorService.Guardar(_conductor);
            DialogResult = DialogResult.OK;
        }
    }
}