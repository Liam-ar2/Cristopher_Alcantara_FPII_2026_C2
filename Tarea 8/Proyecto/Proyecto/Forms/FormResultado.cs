using System;
using System.Drawing;
using System.Windows.Forms;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormResultado : Form
    {
        private readonly ServicioTransporte _servicio;
        private readonly ResultadoCalculo _r;
        private readonly Vehiculo _vehiculo;
        private readonly Conductor _conductor;

        private Button _btnGuardar;

        public FormResultado(ServicioTransporte servicio, ResultadoCalculo resultado,
            Vehiculo vehiculo, Conductor conductor)
        {
            _servicio = servicio;
            _r = resultado;
            _vehiculo = vehiculo;
            _conductor = conductor;
            InitializeUi();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 700, 640, "Resultado del Calculo");
            Tablero.Cabecera(this, "RESULTADO DEL CALCULO", null);

            var info = new Label
            {
                Location = new Point(26, 92),
                Size = new Size(ClientSize.Width - 52, 40),
                Font = new Font(Tablero.Fuente, 10.5f, FontStyle.Bold),
                ForeColor = Tablero.Texto,
                Text = _vehiculo.Descripcion + "   |   " +
                       EnumeracionesUI.Etiqueta(_servicio.TipoServicio) +
                       "   |   " + _servicio.Fecha.ToString("g")
            };
            Controls.Add(info);

            // Tabla resumen.
            var grid = new DataGridView { Location = new Point(20, 140), Size = new Size(ClientSize.Width - 40, 380) };
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.ColumnHeadersVisible = false;
            grid.Columns.Add("concepto", "Concepto");
            grid.Columns.Add("valor", "Valor");
            grid.Columns["concepto"].Width = 320;
            grid.Columns["valor"].Width = 180;
            grid.Columns["concepto"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.Columns["valor"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            grid.DefaultCellStyle.Font = new Font(Tablero.Fuente, 9.5f);
            grid.EnableHeadersVisualStyles = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ClearSelection();
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Color.FromArgb(230, 233, 238);
            Controls.Add(grid);

            AgregarFila(grid, "Distancia del viaje", Formato.Kilometros(_r.DistanciaKm));
            AgregarFila(grid, "Combustible utilizado",
                _r.CombustibleUtilizado.ToString("0.00") + " " + EnumeracionesUI.Etiqueta(_vehiculo.UnidadCombustible).ToLower() + "s");
            AgregarFila(grid, "Costo de combustible", Formato.Moneda(_r.CostoCombustible));
            AgregarFila(grid, "Costo de mantenimiento", Formato.Moneda(_r.CostoMantenimiento));
            AgregarFila(grid, "Costo del seguro", Formato.Moneda(_r.CostoSeguro));
            AgregarFila(grid, "Costo del conductor",
                _conductor == null ? "RD$ 0.00" : Formato.Moneda(_r.CostoConductor) + " (" + _conductor.Nombre + ")");
            AgregarFila(grid, "Costo de peajes", Formato.Moneda(_r.CostoPeajes));
            AgregarFila(grid, "Otros costos", Formato.Moneda(_r.CostoOtros));
            AgregarFila(grid, "COSTO TOTAL DEL SERVICIO", Formato.Moneda(_r.CostoTotalServicio), true);

            if (_r.DistanciaKm > 0)
            {
                AgregarFila(grid, "  Costo por kilometraje", Formato.Moneda(_r.CostoPorKilometro));
                AgregarFila(grid, "  Precio final por km",
                    Formato.Moneda(_r.PrecioFinalRecomendado / _r.DistanciaKm));
            }
            if (_servicio.Pasajeros > 0)
                AgregarFila(grid, "  Costo por pasajero", Formato.Moneda(_r.CostoPorPasajero) +
                    " (" + _servicio.Pasajeros + " pasajeros)");
            if (_servicio.CargaKg > 0)
                AgregarFila(grid, "  Costo por kg de carga", Formato.Moneda(_r.CostoPorCargaKg));
            if (_servicio.Horas > 0)
                AgregarFila(grid, "  Costo por hora",
                    Formato.Moneda(_r.CostoTotalServicio / _servicio.Horas));
            AgregarFila(grid, "Margen de ganancia", _servicio.MargenGanancia + "%");
            AgregarFila(grid, "Ganancia estimada (" + _servicio.MargenGanancia + "%)", Formato.Moneda(_r.Ganancia));

            // Precio final grande.
            var precio = new Label
            {
                Location = new Point(20, ClientSize.Height - 110),
                AutoSize = true,
                Font = new Font(Tablero.Fuente, 20f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 80),
                Text = "PRECIO FINAL RECOMENDADO:  " + Formato.Moneda(_r.PrecioFinalRecomendado)
            };
            Controls.Add(precio);

            // Botones.
            _btnGuardar = Tablero.Boton(this, "GUARDAR EN HISTORIAL", 20, ClientSize.Height - 58, 200, 36);
            _btnGuardar.Click += (s, e) => Guardar();
            Tablero.Boton(this, "NUEVO CALCULO", 240, ClientSize.Height - 58, 150, 36, false).Click += (s, e) => Close();
            Tablero.Boton(this, "CERRAR", 410, ClientSize.Height - 58, 100, 36, false).Click += (s, e) => Close();
        }

        private void AgregarFila(DataGridView g, string concepto, string valor, bool enfatizar = false)
        {
            int idx = g.Rows.Add(concepto, valor);
            var celda = g.Rows[idx].Cells[0];
            celda.Style.Font = new Font(Tablero.Fuente, enfatizar ? 10f : 9.5f, enfatizar ? FontStyle.Bold : FontStyle.Regular);
            g.Rows[idx].Cells[1].Style.Font = new Font(Tablero.Fuente, 9.5f, enfatizar ? FontStyle.Bold : FontStyle.Regular);
            if (enfatizar)
            {
                g.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(235, 245, 235);
                g.Rows[idx].Cells[1].Style.ForeColor = Color.FromArgb(0, 130, 60);
            }
        }

        private void Guardar()
        {
            ServicioService.Guardar(_servicio, _r);
            _btnGuardar.Enabled = false;
            _btnGuardar.Text = "GUARDADO";
            MessageBox.Show("El calculo ha sido guardado en el historial.", "Proyecto",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}