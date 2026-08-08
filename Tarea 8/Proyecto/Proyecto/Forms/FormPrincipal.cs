using System;
using System.Drawing;
using System.Windows.Forms;
using Proyecto.Data;
using Proyecto.Models;
using Proyecto.Services;
using Proyecto.Utilities;

namespace Proyecto.Forms
{
    public class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeUi();
        }

        private void InitializeUi()
        {
            Tablero.EstiloForm(this, 980, 640, "Sistema de Costos de Transporte");

            // Barra superior con el nombre del sistema.
            var marco = new Panel
            {
                BackColor = Tablero.Acento,
                Dock = DockStyle.Top,
                Height = 112
            };
            marco.Controls.Add(new Label
            {
                Text = "SISTEMA DE CALCULO DE COSTOS DE TRANSPORTE",
                ForeColor = Color.White,
                Font = new Font(Tablero.Fuente, 17f, FontStyle.Bold),
                Location = new Point(34, 20),
                AutoSize = true
            });
            marco.Controls.Add(new Label
            {
                Text = "Calcula el costo por kilometraje de un vehiculo y el precio justo de cada servicio.",
                ForeColor = Color.FromArgb(215, 225, 240),
                Font = new Font(Tablero.Fuente, 10.5f),
                Location = new Point(36, 58),
                AutoSize = true
            });
            Controls.Add(marco);

            // Menu de navegacion.
            CrearBoton("CALCULAR SERVICIO", "Determine el precio de un transporte", 140, AbrirCalculo);
            CrearBoton("ADMINISTRAR VEHICULOS", "Registre y mantenga su flota", 216, AbrirVehiculos);
            CrearBoton("ADMINISTRAR CONDUCTORES", "Personal a cargo de los servicios", 292, AbrirConductores);
            CrearBoton("ADMINISTRAR COSTOS", "Combustible, seguro, salario y mas", 368, AbrirCostos);
            CrearBoton("HISTORIAL DE RESULTADOS", "Consulte calculos anteriores", 444, AbrirHistorial);

            var botonSalir = Tablero.Boton(this, "SALIR", 420, 480, 200, 40, false);
            botonSalir.Click += (s, e) => Application.Exit();

            // Panel informativo.
            var info = new GroupBox
            {
                Text = "	INFORMACION GENERAL",
                Font = new Font(Tablero.Fuente, 10f, FontStyle.Bold),
                ForeColor = Tablero.Acento,
                Location = new Point(420, 168),
                Size = new Size(520, 290),
                BackColor = Color.White
            };
            lblInfo = CrearInfoLbl(info);
            Controls.Add(info);

            // Pie de ventana.
            Controls.Add(new Label
            {
                Text = "Moneda: Pesos Dominicanos (RD$)   |   C# Windows Forms + SQLite",
                Location = new Point(40, 600),
                Font = new Font(Tablero.Fuente, 9f),
                ForeColor = Tablero.Suave,
                AutoSize = true
            });
        }

        private Label CrearInfoLbl(Control padre)
        {
            var l = new Label
            {
                Location = new Point(16, 38),
                AutoSize = false,
                Width = padre.ClientSize.Width - 32,
                Height = 210,
                Font = new Font(Tablero.Fuente, 9.5f),
                ForeColor = Tablero.Texto,
                Text = ""
            };
            padre.Controls.Add(l);
            return l;
        }

        private Label lblInfo;

        private void ActualizarInformacion()
        {
            var cfg = ConfigService.Obtener();
            lblInfo.Text =
                "Base de datos local :  " + ConexionBD.RutaArchivo + "\n\n" +
                "Precio del combustible:  " + Formato.Moneda(cfg.PrecioCombustible) +
                " por " + EnumeracionesUI.Etiqueta(cfg.UnidadCombustible).ToLower() + "\n\n" +
                "Vehiculos registrados:  " + VehiculoService.ObtenerTodos().Count + "\n\n" +
                "Conductores registrados:  " + ConductorService.ObtenerTodos().Count;
        }

        private void CrearBoton(string titulo, string detalle, int y, EventHandler alHacerClic)
        {
            var b = new Button
            {
                Text = titulo + "\n" + detalle,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(Tablero.Fuente, 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(30, y),
                Size = new Size(330, 70),
                Cursor = Cursors.Hand,
                BackColor = Color.White,
                ForeColor = Tablero.Texto,
                FlatAppearance = { BorderColor = Tablero.Borde, MouseOverBackColor = Color.FromArgb(228, 236, 250) },
                Padding = new Padding(14, 0, 0, 0)
            };
            b.Click += alHacerClic;
            Controls.Add(b);
        }

        private void Abrir(Form f)
        {
            f.ShowDialog();
            ActualizarInformacion();
        }

        private void AbrirCalculo(object s, EventArgs e) => Abrir(new FormCalculo());
        private void AbrirVehiculos(object s, EventArgs e) => Abrir(new FormVehiculos());
        private void AbrirConductores(object s, EventArgs e) => Abrir(new FormConductores());
        private void AbrirCostos(object s, EventArgs e) => Abrir(new FormCostos());
        private void AbrirHistorial(object s, EventArgs e) => Abrir(new FormHistorial());
    }
}