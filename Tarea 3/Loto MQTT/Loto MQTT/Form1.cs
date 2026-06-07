using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Text.Json;

namespace Loto_MQTT
{
    public partial class Form1 : Form
    {

        // ── Cliente MQTT ──
        IMqttClient mqttClient;

        // Tópicos
        private const string TOPIC_RESULTADOS = "loteria/resultados";
        private const string TOPIC_APUESTAS = "loteria/apuestas";
        private const string TOPIC_GANADORES = "loteria/ganadores/";

        // Apuestas recibidas de clientes remotos
        private readonly List<ApuestaMsg> _apuestas = new List<ApuestaMsg>();
        private readonly object _lock = new object();

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await ConectarMqttAsync();
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mqttClient?.IsConnected == true)
                await mqttClient.DisconnectAsync();
        }

        private async Task ConectarMqttAsync()
        {
            var opciones = new MqttClientOptionsBuilder()
                .WithClientId("LotteriaServidor")
                .WithTcpServer("broker.hivemq.com", 1883)
                .WithCleanSession()
                .Build();

            mqttClient = new MqttFactory().CreateMqttClient();
            mqttClient.ApplicationMessageReceivedAsync += OnMensajeRecibidoAsync;

            try
            {
                await mqttClient.ConnectAsync(opciones);

                await mqttClient.SubscribeAsync(
                    new MqttTopicFilterBuilder()
                        .WithTopic(TOPIC_APUESTAS)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                        .Build());

                MessageBox.Show("Conectado al broker MQTT en localhost:1883",
                                "MQTT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo conectar al broker MQTT:\n{ex.Message}\n\n" +
                   "Asegúrate de que MQTTX tenga el broker activo en el puerto 1883.",
                   "Error MQTT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ///////  RECIBIR APUESTA DE UN CLIENTE REMOTO
        private Task OnMensajeRecibidoAsync(MqttApplicationMessageReceivedEventArgs args)
        {
            if (args.ApplicationMessage.Topic != TOPIC_APUESTAS)
                return Task.CompletedTask;

            try
            {
                string json = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment.ToArray());
                var apuesta = JsonSerializer.Deserialize<ApuestaMsg>(json);
                if (apuesta == null) return Task.CompletedTask;

                lock (_lock)
                {
                    _apuestas.RemoveAll(a => a.ClienteId == apuesta.ClienteId);
                    _apuestas.Add(apuesta);
                }

                BeginInvoke(new Action(() =>
                {
                    MessageBox.Show(
                        $"Apuesta recibida:\nCliente: {apuesta.ClienteId}\nNúmero: {apuesta.Numero}\nMonto: {apuesta.Monto:N2} RD$",
                        "Nueva apuesta MQTT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            catch { }

            return Task.CompletedTask;
        }



        //////  PUBLICAR MENSAJE MQTT
        private async Task PublicarAsync(string topico, string payload)
        {
            if (mqttClient?.IsConnected != true) return;

            var mensaje = new MqttApplicationMessageBuilder()
                .WithTopic(topico)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .Build();

            await mqttClient.PublishAsync(mensaje);
        }

        private void btnnueva_Click(object sender, EventArgs e)
        {
            txtnombre.Text = "";
            txtdinero.Text = "";
            txtnumero.Text = "";
            txtprimera.Text = "--";
            txtsegunda.Text = "--";
            txttercera.Text = "--";
            txtresultado.Text = "Ingresa tus datos y presiona jugar";

            MessageBox.Show("¡Datos reiniciados! Ingresa tus datos y presiona jugar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnjugar_Click(object sender, EventArgs e)
        {
            if (txtdinero.Text == "" || txtnumero.Text == "" || txtnombre.Text == "")
            {
                MessageBox.Show("Por favor, ingresa un monto de dinero, un número y tu nombre para jugar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //////////////////////////////////////////////////////////////////
            // DATOS DEL JUGADOR
            string nombre = txtnombre.Text.Trim();

            double monto =
                Convert.ToDouble(txtdinero.Text);

            int numero =
                Convert.ToInt32(txtnumero.Text);

            // CREAR APUESTA MQTT
            var apuesta = new ApuestaMsg
            {
                ClienteId = nombre,
                Numero = numero,
                Monto = monto
            };

            // CONVERTIR A JSON
            string jsonApuesta =
                JsonSerializer.Serialize(apuesta);

            // PUBLICAR APUESTA
            await PublicarAsync(
                TOPIC_APUESTAS,
                jsonApuesta);
            //////////////////////////////////////////////////////////////


            Random GeneradorNum = new Random();
            int primera = GeneradorNum.Next(0, 100);
            int segunda = GeneradorNum.Next(0, 100);
            int tercera = GeneradorNum.Next(0, 100);

            txtprimera.Text = primera.ToString();
            txtsegunda.Text = segunda.ToString();
            txttercera.Text = tercera.ToString();


            //// Publicar resultados por MQTT  (usa ResultadoMsg de Resultado.cs)
            var resultado = new ResultadoMsg
            {
                Primera = primera,
                Segunda = segunda,
                Tercera = tercera,
                Hora = DateTime.Now.ToString("HH:mm:ss")
            };
            await PublicarAsync(TOPIC_RESULTADOS, JsonSerializer.Serialize(resultado));

            double CantidadDin = Convert.ToDouble(txtdinero.Text);
            int Numero = Convert.ToInt32(txtnumero.Text);

            if (Numero == primera)
            {
                double premio = CantidadDin * 1000;
                txtresultado.Text = $"¡Ganaste! Premio: {premio} RD$";
                txtprimera.BackColor = Color.Gold;
            }
            else if (Numero == segunda)
            {
                double premio = CantidadDin * 100;
                txtresultado.Text = $"¡Segundo lugar! Premio: {premio} RD$";
                txtsegunda.BackColor = Color.Silver;
            }
            else if (Numero == tercera)
            {
                double premio = CantidadDin * 10;
                txtresultado.Text = $"¡Tercer lugar! Premio: {premio} RD$";
                txttercera.BackColor = Color.FromArgb(205, 127, 50);
            }
            else
            {
                txtresultado.Text = "¡Lo siento! No ganaste esta vez.";
                txtprimera.BackColor = Color.FromArgb(30, 35, 52);
                txtsegunda.BackColor = Color.FromArgb(30, 35, 52);
                txttercera.BackColor = Color.FromArgb(30, 35, 52);

            }


            // Evaluar apuestas remotas y notificar a cada cliente
            List<ApuestaMsg> copia;
            lock (_lock) { copia = new List<ApuestaMsg>(_apuestas); }

            foreach (var apuestaRemota in copia)
                await EvaluarApuestaRemotaAsync(apuestaRemota, primera, segunda, tercera);

        }


        /////  EVALUAR APUESTA REMOTA Y PUBLICAR RESULTADO
        private async Task EvaluarApuestaRemotaAsync(ApuestaMsg apuestaRemota,
                                                      int primera, int segunda, int tercera)
        {
            // Usa GanadorMsg de Ganador.cs
            GanadorMsg msg;

            if (apuestaRemota.Numero == primera)
                msg = new GanadorMsg
                {
                    ClientId = apuestaRemota.ClienteId,
                    Numero = apuestaRemota.Numero,
                    Monto = apuestaRemota.Monto,
                    Premio = apuestaRemota.Monto * 1000,
                    Lugar = "Primera",
                    Mensaje = $"¡Ganaste! Primer lugar. Premio: {apuestaRemota.Monto * 1000:N2} RD$"
                };
            else if (apuestaRemota.Numero == segunda)
                msg = new GanadorMsg
                {
                    ClientId = apuestaRemota.ClienteId,
                    Numero = apuestaRemota.Numero,
                    Monto = apuestaRemota.Monto,
                    Premio = apuestaRemota.Monto * 100,
                    Lugar = "Segunda",
                    Mensaje = $"¡Segundo lugar! Premio: {apuestaRemota.Monto * 100:N2} RD$"
                };
            else if (apuestaRemota.Numero == tercera)
                msg = new GanadorMsg
                {
                    ClientId = apuestaRemota.ClienteId,
                    Numero = apuestaRemota.Numero,
                    Monto = apuestaRemota.Monto,
                    Premio = apuestaRemota.Monto * 10,
                    Lugar = "Tercera",
                    Mensaje = $"¡Tercer lugar! Premio: {apuestaRemota.Monto * 10:N2} RD$"
                };
            else
                msg = new GanadorMsg
                {
                    ClientId = apuestaRemota.ClienteId,
                    Numero = apuestaRemota.Numero,
                    Monto = apuestaRemota.Monto,
                    Premio = 0,
                    Lugar = "Ninguna",
                    Mensaje = "Lo siento, no ganaste esta vez."
                };

            await PublicarAsync(TOPIC_GANADORES + apuestaRemota.ClienteId, JsonSerializer.Serialize(msg));
        }

        private void txtnumero_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtnumero.Text.Trim(), out int numero) || numero < 0 || numero > 99)
            {
                MessageBox.Show("Por favor, ingresa un número válido entre 0 y 99.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtnumero.Text = "";
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}