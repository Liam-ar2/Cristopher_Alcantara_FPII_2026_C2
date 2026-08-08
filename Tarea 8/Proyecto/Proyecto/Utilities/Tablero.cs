using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Proyecto.Utilities
{
    /// <summary>Fabrica de controles con estilo comun para todas las ventanas.</summary>
    public static class Tablero
    {
        public const string Fuente = "Segoe UI";
        public static readonly Color Fondo = Color.FromArgb(244, 246, 249);
        public static readonly Color Tarjeta = Color.White;
        public static readonly Color Acento = Color.FromArgb(24, 92, 215);
        public static readonly Color AcentoOscuro = Color.FromArgb(18, 70, 165);
        public static readonly Color Texto = Color.FromArgb(33, 37, 41);
        public static readonly Color Suave = Color.FromArgb(110, 118, 130);
        public static readonly Color Borde = Color.FromArgb(206, 212, 218);

        public static void EstiloForm(Form f, int ancho, int alto, string titulo)
        {
            f.ClientSize = new Size(ancho, alto);
            f.Text = titulo;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.FormBorderStyle = FormBorderStyle.FixedSingle;
            f.MaximizeBox = false;
            f.BackColor = Fondo;
            f.Font = new Font(Fuente, 9.5f);
            f.Name = titulo + "_App";
        }

        public static Label Cabecera(Control padre, string titulo, string subtitulo = null)
        {
            var l = new Label
            {
                Text = titulo,
                Font = new Font(Fuente, 20f, FontStyle.Bold),
                ForeColor = Texto,
                Location = new Point(24, 18),
                AutoSize = false,
                Size = new Size(padre.ClientSize.Width - 48, 40)
            };
            if (!string.IsNullOrEmpty(subtitulo))
            {
                var sub = new Label
                {
                    Text = subtitulo,
                    Font = new Font(Fuente, 10f),
                    ForeColor = Suave,
                    Location = new Point(26, 60),
                    AutoSize = false,
                    Size = new Size(padre.ClientSize.Width - 52, 20)
                };
                padre.Controls.Add(sub);
            }
            padre.Controls.Add(l);
            return l;
        }

        public static Label Etiqueta(Control padre, string texto, int x, int y, int ancho, bool negrita = false)
        {
            var l = new Label
            {
                Text = texto,
                Font = new Font(Fuente, negrita ? 9.8f : 9.5f, negrita ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Texto,
                Location = new Point(x, y),
                AutoSize = false,
                Size = new Size(ancho, 18),
                TextAlign = ContentAlignment.MiddleLeft
            };
            padre.Controls.Add(l);
            return l;
        }

        public static TextBox CajaTexto(Control padre, int x, int y, int ancho, int alto = 26)
        {
            var t = new TextBox
            {
                Font = new Font(Fuente, 10f),
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                BorderStyle = BorderStyle.FixedSingle
            };
            padre.Controls.Add(t);
            return t;
        }

        public static NumericUpDown Numerico(Control padre, int x, int y, int ancho,
            decimal min, decimal max, int decimales = 0, decimal incremento = 1)
        {
            var n = new NumericUpDown
            {
                Font = new Font(Fuente, 10f),
                Location = new Point(x, y),
                Size = new Size(ancho, 26),
                Minimum = min,
                Maximum = max,
                DecimalPlaces = decimales,
                Increment = incremento,
                ThousandsSeparator = true
            };
            padre.Controls.Add(n);
            return n;
        }

        public static ComboBox Combo(Control padre, int x, int y, int ancho)
        {
            var c = new ComboBox
            {
                Font = new Font(Fuente, 10f),
                Location = new Point(x, y),
                Size = new Size(ancho, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            padre.Controls.Add(c);
            return c;
        }

        public static Button Boton(Control padre, string texto, int x, int y, int ancho, int alto, bool primario = true)
        {
            var b = new Button
            {
                Text = texto,
                Font = new Font(Fuente, 10f, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                Cursor = Cursors.Hand
            };
            if (primario)
            {
                b.BackColor = Acento;
                b.ForeColor = Color.White;
                b.FlatAppearance.BorderColor = Acento;
                b.FlatAppearance.MouseOverBackColor = AcentoOscuro;
            }
            else
            {
                b.BackColor = Color.White;
                b.ForeColor = Texto;
                b.FlatAppearance.BorderColor = Borde;
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 242, 245);
            }
            padre.Controls.Add(b);
            return b;
        }

        public static DataGridView Tabla(Control padre, int x, int y, int ancho, int alto)
        {
            var g = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false,
                Font = new Font(Fuente, 9.5f)
            };
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 242, 245);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Texto;
            g.ColumnHeadersDefaultCellStyle.Font = new Font(Fuente, 9.5f, FontStyle.Bold);
            g.ColumnHeadersHeight = 34;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 250);
            g.DefaultCellStyle.SelectionForeColor = Texto;
            g.GridColor = Borde;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 251);
            padre.Controls.Add(g);
            return g;
        }

        public static Panel Caja(Control padre, int x, int y, int ancho, int alto)
        {
            var p = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            padre.Controls.Add(p);
            return p;
        }
    }
}