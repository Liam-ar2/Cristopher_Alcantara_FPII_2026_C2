using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculadora
{
    public partial class Form1 : Form
    {
        private double x;
        private double y;
        private double resultado;
        private int operacion;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnmult_Click(object sender, EventArgs e)
        {
            //multiplicacion
            operacion = 3;
            x = Convert.ToDouble(tbDisplay.Text);
            tbDisplay.Text = " ";
        }

        private void btnpunto_Click(object sender, EventArgs e)
        {
            //punto
            tbDisplay.Text = tbDisplay.Text + ".";
        }

        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            //Boton_Limpiar
            tbDisplay.Text = " ";
        }

        private void button24_Click(object sender, EventArgs e)
        {
            //numero 0
            tbDisplay.Text = tbDisplay.Text + "0";
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            //numero 1
            tbDisplay.Text = tbDisplay.Text + "1";
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            //numero 2
            tbDisplay.Text = tbDisplay.Text + "2";
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            //numero 3
            tbDisplay.Text = tbDisplay.Text + "3";
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            //numero 4
            tbDisplay.Text = tbDisplay.Text + "4";
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            //numero 5
            tbDisplay.Text = tbDisplay.Text + "5";
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            //numero 6
            tbDisplay.Text = tbDisplay.Text + "6";
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            //numero 7
            tbDisplay.Text = tbDisplay.Text + "7";
        }

        private void btn8_Click(object sender, EventArgs e)
        {
            //numero 8
            tbDisplay.Text = tbDisplay.Text + "8";
        }

        private void btn9_Click(object sender, EventArgs e)
        {
            //numero 9
            tbDisplay.Text = tbDisplay.Text + "9";
        }

        private void btnsuma_Click(object sender, EventArgs e)
        {
            //suma
            operacion = 1;
            x = Convert.ToDouble(tbDisplay.Text);
            tbDisplay.Text = " ";
        }

        private void tbDisplay_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnraiz_Click(object sender, EventArgs e)
        {
            //raiz
            operacion = 5;
            x = Convert.ToDouble(tbDisplay.Text);
            tbDisplay.Text = " ";
        }

        private void btnigual_Click(object sender, EventArgs e)
        {
            if (operacion != 5)
            {
                y = Convert.ToDouble(tbDisplay.Text);
            }

            switch (operacion)
            {
                case 1:
                    resultado = x + y;
                    break;
                case 2:
                    resultado = x - y;
                    break;
                case 3:
                    resultado = x * y;
                    break;
                case 4:
                    resultado = x / y;
                    break;
                case 5:
                    resultado = Math.Sqrt(x); 
                    break; 
            }
            tbDisplay.Text = resultado.ToString();
        }

        private void btnresta_Click(object sender, EventArgs e)
        {
            //resta
            operacion = 2;
            x = Convert.ToDouble(tbDisplay.Text);
            tbDisplay.Text = " ";
        }

        private void btndiv_Click(object sender, EventArgs e)
        {
            //division
            operacion = 4;
            x = Convert.ToDouble(tbDisplay.Text);
            tbDisplay.Text = " ";
        }
    }
}
