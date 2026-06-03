using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loteria
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void btnnueva_Click(object sender, EventArgs e)
        {
            txtdinero.Text = "";
            txtnumero.Text = "";
            txtprimera.Text = "--";
            txtsegunda.Text = "--";
            txttercera.Text = "--";
            txtresultado.Text = "Ingresa tus datos y presiona jugar";
            MessageBox.Show("¡Datos reiniciados! Ingresa tus datos y presiona jugar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnjugar_Click(object sender, EventArgs e)
        {
            if (txtdinero.Text == "" || txtnumero.Text == "")
            {
                MessageBox.Show("Por favor, ingresa un monto de dinero y un número para jugar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Random GeneradorNum = new Random();
            int primera = GeneradorNum.Next(0, 100);
            int segunda = GeneradorNum.Next(0, 100);
            int tercera = GeneradorNum.Next(0, 100);

            txtprimera.Text = primera.ToString();
            txtsegunda.Text = segunda.ToString();
            txttercera.Text = tercera.ToString();

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
        }

        private void txtdinero_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtnumero_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtnumero.Text.Trim(), out int numero) || numero < 0 || numero > 99)
            {
                MessageBox.Show("Por favor, ingresa un número válido entre 0 y 99.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtnumero.Text = "";            
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

