using System;
using System.Drawing;
using System.Windows.Forms;
using Proyecto.Calculators;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormCalculo : Form
    {
        private ComboBox _cmbVehiculo, _cmbConductor, _cmbTipoServicio;
        private NumericUpDown _numDistancia, _numPasajeros, _numCarga, _numHoras, _numPeajes, _numMargen;
        private Label _lblInfoVehiculo;

        public FormCalculo()
        {
            InitializeUi();
            Cargar();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 860, 600, "Calcular Servicio");
            Tablero.Cabecera(this, "CALCULAR COSTO DE SERVICIO",
                "Ingrese los datos del viaje. El sistema le recomendara un precio final.");

            // Panel izquierdo: vehiculo y conductor.
            var p1 = Tablero.Caja(this, 20, 120, 400, 380);
            EtiquetaPanel(p1, "VEHICULO Y CONDUCTOR", 15, 12, true);
            EtiquetaPanel(p1, "Vehiculo:", 15, 44);
            _cmbVehiculo = Tablero.Combo(p1, 15, 64, 370);
            _cmbVehiculo.SelectedIndexChanged += (s, e) => ActualizarInfoVehiculo();

            _lblInfoVehiculo = new Label
            {
                Location = new Point(15, 104),
                Size = new Size(p1.ClientSize.Width - 30, 72),
                Font = new Font(Tablero.Fuente, 9f),
                ForeColor = Tablero.Suave
            };
            p1.Controls.Add(_lblInfoVehiculo);

            EtiquetaPanel(p1, "Conductor:", 15, 186);
            _cmbConductor = Tablero.Combo(p1, 15, 206, 370);
            EtiquetaPanel(p1, "El salario del conductor se reparte entre el kilometraje mensual.", 15, 238);

            // Panel derecho: datos del servicio.
            var p2 = Tablero.Caja(this, 440, 120, 400, 400);
            EtiquetaPanel(p2, "DATOS DEL SERVICIO", 15, 10, true);

            EtiquetaPanel(p2, "Tipo de servicio:", 15, 40);
            _cmbTipoServicio = Tablero.Combo(p2, 15, 60, 360);

            EtiquetaPanel(p2, "Distancia del viaje (km) *:", 15, 96);
            _numDistancia = Tablero.Numerico(p2, 15, 116, 360, 0, 100000, 1);

            EtiquetaPanel(p2, "Cantidad de pasajeros:", 15, 152);
            _numPasajeros = Tablero.Numerico(p2, 15, 172, 360, 0, 5000, 0);

            EtiquetaPanel(p2, "Peso o carga transportada (kg):", 15, 208);
            _numCarga = Tablero.Numerico(p2, 15, 228, 360, 0, 10000000, 0);

            EtiquetaPanel(p2, "Tiempo estimado (horas):", 15, 264);
            _numHoras = Tablero.Numerico(p2, 15, 284, 360, 0, 10000, 1);

            EtiquetaPanel(p2, "Peajes de la ruta (RD$):", 15, 320);
            _numPeajes = Tablero.Numerico(p2, 15, 340, 360, 0, 1000000, 2);

            EtiquetaPanel(p2, "Margen de ganancia (%):", 15, 376);
            _numMargen = Tablero.Numerico(p2, 15, 396, 360, 0, 500, 0);

            // Boton principal.
            Tablero.Boton(this, "C A L C U L A R   S E R V I C I O", 260, 528, 340, 46)
                .Click += (s, e) => CalcularServicio();
        }

        private void EtiquetaPanel(Control padre, string texto, int x, int y, bool negrita = false)
        {
            padre.Controls.Add(new Label
            {
                Text = texto,
                Font = new Font(Tablero.Fuente, negrita ? 10f : 9.2f,
                    negrita ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = negrita ? Tablero.Acento : Tablero.Texto,
                Location = new Point(x, y),
                AutoSize = true
            });
        }

        private void Cargar()
        {
            EnumeracionesUI.LlenarCombo<TipoServicio>(_cmbTipoServicio);

            _cmbVehiculo.Items.Clear();
            foreach (var v in VehiculoService.ObtenerTodos())
                _cmbVehiculo.Items.Add(new ComboItem<Vehiculo>(v, v.Descripcion));
            if (_cmbVehiculo.Items.Count > 0) _cmbVehiculo.SelectedIndex = 0;

            _cmbConductor.Items.Clear();
            _cmbConductor.Items.Add(new ComboItem<Conductor>(null, "(Sin conductor)"));
            foreach (var c in ConductorService.ObtenerTodos())
                _cmbConductor.Items.Add(new ComboItem<Conductor>(c, c.Nombre));
            _cmbConductor.SelectedIndex = 0;

            _numMargen.Value = 20;
            _numDistancia.Value = 20;
            _numPasajeros.Value = 3;
        }

        private Vehiculo VehiculoSeleccionado()
            => (_cmbVehiculo.SelectedItem as ComboItem<Vehiculo>)?.Valor;

        private void ActualizarInfoVehiculo()
        {
            var v = VehiculoSeleccionado();
            if (v == null)
            {
                _lblInfoVehiculo.Text = "No hay vehiculos registrados.\nRegistre uno en Administrar Vehiculos.";
                return;
            }
            _lblInfoVehiculo.Text =
                "  Capacidad: " + v.CapacidadPasajeros + " pasajeros, " + v.CapacidadCargaKg + " kg\n" +
                "  Rendimiento: " + v.RendimientoTexto + "\n" +
                "  Kilometraje mensual: " + v.KmMesTexto;
        }

        private void CalcularServicio()
        {
            var v = VehiculoSeleccionado();
            if (v == null)
            {
                MessageBox.Show("No hay vehiculos registrados.\nRegistre un vehiculo en Administrar Vehiculos.",
                    "Proyecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal distancia = _numDistancia.Value;
            int pasajeros = (int)_numPasajeros.Value;
            decimal carga = _numCarga.Value;

            var errores = Validaciones.ValidarServicio(v, distancia, pasajeros, carga);
            if (errores.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errores), "Datos invalidos",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conductor = (_cmbConductor.SelectedItem as ComboItem<Conductor>)?.Valor;
            var costos = CostoService.ObtenerPorVehiculo(v.Id);
            var cfg = ConfigService.Obtener();

            var servicio = new ServicioTransporte
            {
                VehiculoId = v.Id,
                ConductorId = conductor?.Id,
                TipoServicio = EnumeracionesUI.ValorCombo(_cmbTipoServicio, TipoServicio.Pasajeros),
                DistanciaKm = distancia,
                Pasajeros = pasajeros,
                CargaKg = carga,
                Horas = _numHoras.Value,
                PeajesManualRd = _numPeajes.Value,
                MargenGanancia = _numMargen.Value
            };

            var resultado = CalculadoraCostos.Calcular(
                v, conductor, cfg, costos,
                distancia, pasajeros, carga,
                servicio.Horas, servicio.PeajesManualRd, servicio.MargenGanancia,
                servicio.TipoServicio);

            using (var res = new FormResultado(servicio, resultado, v, conductor))
            {
                res.ShowDialog(this);
            }
        }
    }
}