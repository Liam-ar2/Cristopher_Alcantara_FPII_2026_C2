using RiskUp.Controls;
using RiskUp.Data;
using RiskUp.Helpers;
using RiskUp.Models;

namespace RiskUp.Forms;

public class Fases : Form
{
    private static readonly Color ColorFondo = Color.FromArgb(19, 21, 40);
    private static readonly Color ColorTarjeta = Color.FromArgb(30, 33, 61);
    private static readonly Color ColorBorde = Color.FromArgb(124, 92, 255);
    private static readonly Color ColorAcento = Color.FromArgb(255, 159, 28);
    private static readonly Color ColorTexto = Color.White;
    private static readonly Color ColorTextoSuave = Color.FromArgb(200, 200, 210);

    private readonly Riesgo _riesgo = new();
    private readonly RiesgoRepository _repositorio = new();

    private int _pasoActual = 1;
    private const int TotalPasos = 4;

    private readonly Panel _pnlContenido;
    private readonly Label _lblPaso;
    private readonly Button _btnAtras;
    private readonly Button _btnSiguiente;

    // Controles Paso 1
    private TextBox _txtUsuario = null!;
    private TextBox _txtNombre = null!;
    private TextBox _txtDescripcion = null!;

    // Controles Paso 2
    private readonly Dictionary<string, OrangeSlider> _sliders = new();
    private readonly Dictionary<string, Label> _sliderValores = new();

    // Controles Paso 3
    private Label _lblImportancia = null!;
    private Label _lblProbabilidad = null!;
    private Label _lblEvaluacion = null!;
    private Panel _pnlNivel = null!;
    private Label _lblNivel = null!;

    // Controles Paso 4
    private Label _lblResumen4 = null!;
    private Label _lblEstadoGuardado = null!;

    public Fases()
    {
        Text = "RiskUp - Nueva Evaluación de Riesgo";
        Size = new Size(760, 640);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ColorFondo;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        string rutaIcono = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.ico");
        if (File.Exists(rutaIcono)) Icon = new Icon(rutaIcono);

        // ---------- Encabezado ----------
        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = ColorFondo, Padding = new Padding(20, 10, 20, 10) };
        string rutaLogo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.png");
        if (File.Exists(rutaLogo))
        {
            var pic = new PictureBox
            {
                Image = Image.FromFile(rutaLogo),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(48, 48),
                Location = new Point(20, 11)
            };
            pnlHeader.Controls.Add(pic);
        }
        var lblTituloApp = new Label
        {
            Text = "RiskUp",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(80, 10),
            AutoSize = true
        };
        _lblPaso = new Label
        {
            Text = "Paso 1 de 4: Datos Generales",
            Font = new Font("Segoe UI", 9),
            ForeColor = ColorAcento,
            Location = new Point(80, 36),
            AutoSize = true
        };
 

        pnlHeader.Controls.Add(lblTituloApp);
        pnlHeader.Controls.Add(_lblPaso);

        // ---------- Contenido ----------
        _pnlContenido = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo, Padding = new Padding(24) };

        // ---------- Pie (navegación) ----------
        var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = ColorFondo };
        _btnAtras = CrearBoton("Atrás", ColorTarjeta);
        _btnAtras.Location = new Point(20, 15);
        _btnAtras.Click += (s, e) => IrAPaso(_pasoActual - 1);

        _btnSiguiente = CrearBoton("Siguiente →", ColorAcento);
        _btnSiguiente.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnSiguiente.Click += BtnSiguiente_Click;

        var btnCancelar = CrearBoton("Cancelar", ColorTarjeta);
        btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancelar.Click += (s, e) => Close();

        pnlFooter.Controls.Add(_btnAtras);
        pnlFooter.Controls.Add(_btnSiguiente);
        pnlFooter.Controls.Add(btnCancelar);
        pnlFooter.Resize += (s, e) =>
        {
            _btnSiguiente.Location = new Point(pnlFooter.Width - _btnSiguiente.Width - 20, 15);
            btnCancelar.Location = new Point(pnlFooter.Width - _btnSiguiente.Width - btnCancelar.Width - 32, 15);
        };

        Controls.Add(_pnlContenido);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);

        IrAPaso(1);
    }

    private Button CrearBoton(string texto, Color color)
    {
        return new Button
        {
            Text = texto,
            Width = 150,
            Height = 40,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            FlatAppearance = { BorderSize = 0 },
            Cursor = Cursors.Hand
        };
    }

    // ============================================================
    //  Navegación entre pasos
    // ============================================================

    private void BtnSiguiente_Click(object? sender, EventArgs e)
    {
        if (_pasoActual == 1 && !ValidarPaso1()) return;

        if (_pasoActual == 1) GuardarDatosPaso1();
        if (_pasoActual == 2) GuardarDatosPaso2();

        if (_pasoActual == TotalPasos)
        {
            Close(); // "Finalizar"
            return;
        }

        IrAPaso(_pasoActual + 1);
    }

    private void IrAPaso(int paso)
    {
        if (paso < 1 || paso > TotalPasos) return;
        _pasoActual = paso;

        _pnlContenido.Controls.Clear();
        _pnlContenido.Controls.Add(ConstruirPaso(paso));

        string[] nombres = { "Datos Generales", "Criterios de Evaluación", "Resultados del Método Mosler", "Guardar y Exportar" };
        _lblPaso.Text = $"Paso {paso} de {TotalPasos}: {nombres[paso - 1]}";

        _btnAtras.Enabled = paso > 1;
        _btnSiguiente.Text = paso == TotalPasos ? "Finalizar" : "Siguiente →";
    }

    private Control ConstruirPaso(int paso) => paso switch
    {
        1 => ConstruirPaso1(),
        2 => ConstruirPaso2(),
        3 => ConstruirPaso3(),
        4 => ConstruirPaso4(),
        _ => new Panel()
    };

   
    //  PASO 1: Datos generales
    

    private Control ConstruirPaso1()
    {
        var raiz = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo };

        var lblUsuario = EtiquetaCampo("Usuario Evaluador", 0);
        _txtUsuario = CajaTexto(28);
        _txtUsuario.Text = _riesgo.UsuarioEvaluador;

        var lblNombre = EtiquetaCampo("Nombre del Riesgo", 70);
        _txtNombre = CajaTexto(98);
        _txtNombre.Text = _riesgo.NombreRiesgo;

        var lblDescripcion = EtiquetaCampo("Descripción", 140);
        _txtDescripcion = CajaTexto(168);
        _txtDescripcion.Multiline = true;
        _txtDescripcion.Height = 140;
        _txtDescripcion.Text = _riesgo.Descripcion;

        raiz.Controls.Add(lblUsuario);
        raiz.Controls.Add(_txtUsuario);
        raiz.Controls.Add(lblNombre);
        raiz.Controls.Add(_txtNombre);
        raiz.Controls.Add(lblDescripcion);
        raiz.Controls.Add(_txtDescripcion);

        return raiz;
    }

    private Label EtiquetaCampo(string texto, int top)
    {
        return new Label
        {
            Text = texto,
            Location = new Point(0, top),
            AutoSize = true,
            ForeColor = ColorTextoSuave,
            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
    }

    private TextBox CajaTexto(int top)
    {
        return new TextBox
        {
            Location = new Point(0, top),
            Width = 660,
            Font = new Font("Segoe UI", 10),
            BackColor = ColorTarjeta,
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
    }

    private bool ValidarPaso1()
    {
        if (string.IsNullOrWhiteSpace(_txtUsuario.Text))
        {
            MostrarAviso("Ingrese el nombre del usuario evaluador.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(_txtNombre.Text))
        {
            MostrarAviso("Ingrese el nombre del riesgo.");
            return false;
        }
        return true;
    }

    private void GuardarDatosPaso1()
    {
        _riesgo.UsuarioEvaluador = _txtUsuario.Text.Trim();
        _riesgo.NombreRiesgo = _txtNombre.Text.Trim();
        _riesgo.Descripcion = _txtDescripcion.Text.Trim();
    }

    private void MostrarAviso(string mensaje) =>
        MessageBox.Show(this, mensaje, "RiskUp", MessageBoxButtons.OK, MessageBoxIcon.Warning);


    //  PASO 2: Criterios de evaluación (sliders 1 a 5)
 

    private Control ConstruirPaso2()
    {
        _sliders.Clear();
        _sliderValores.Clear();

        var tarjeta = new Panel { Dock = DockStyle.Fill, BackColor = ColorTarjeta };
        const int OffsetX = 24;
        const int OffsetY = 24;

        var lblTitulo = new Label
        {
            Text = "Criterios de Evaluación",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(OffsetX, OffsetY)
        };
        var lblAyuda = new Label
        {
            Text = "Arrastre cada control para calificar del 1 (mínimo) al 5 (máximo).",
            Font = new Font("Segoe UI", 9),
            ForeColor = ColorTextoSuave,
            AutoSize = true,
            Location = new Point(OffsetX, OffsetY + 34)
        };

        tarjeta.Controls.Add(lblTitulo);
        tarjeta.Controls.Add(lblAyuda);

        (string clave, string etiqueta, int valorActual)[] criterios =
        {
            ("F", "Función (F)", _riesgo.Funcion),
            ("S", "Sustitución (S)", _riesgo.Sustitucion),
            ("D", "Profundidad (D)", _riesgo.Profundidad),
            ("E", "Extensión (E)", _riesgo.Extension),
            ("A", "Agresión (A)", _riesgo.Agresion),
            ("V", "Vulnerabilidad (V)", _riesgo.Vulnerabilidad),
        };

        int top = OffsetY + 66;
        foreach (var (clave, etiqueta, valorActual) in criterios)
        {
            var lbl = new Label
            {
                Text = etiqueta,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(OffsetX, top + 6)
            };

            var slider = new OrangeSlider
            {
                Minimum = 1,
                Maximum = 5,
                Value = valorActual,
                Location = new Point(OffsetX + 220, top),
                Width = 340,
                Height = 24
            };

            var lblValor = new Label
            {
                Text = valorActual.ToString(),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = ColorAcento,
                AutoSize = true,
                Location = new Point(OffsetX + 580, top + 3)
            };

            slider.ValueChanged += (s, e) => lblValor.Text = slider.Value.ToString();

            tarjeta.Controls.Add(lbl);
            tarjeta.Controls.Add(slider);
            tarjeta.Controls.Add(lblValor);

            _sliders[clave] = slider;
            _sliderValores[clave] = lblValor;

            top += 52;
        }

        return tarjeta;
    }

    private void GuardarDatosPaso2()
    {
        _riesgo.Funcion = _sliders["F"].Value;
        _riesgo.Sustitucion = _sliders["S"].Value;
        _riesgo.Profundidad = _sliders["D"].Value;
        _riesgo.Extension = _sliders["E"].Value;
        _riesgo.Agresion = _sliders["A"].Value;
        _riesgo.Vulnerabilidad = _sliders["V"].Value;
    }

 
    //  PASO 3: Resultados del método Mosler
   

    private Control ConstruirPaso3()
    {
        var raiz = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo };

        var lblTitulo = new Label
        {
            Text = _riesgo.NombreRiesgo,
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        var lblEvaluador = new Label
        {
            Text = $"Evaluador: {_riesgo.UsuarioEvaluador}",
            Font = new Font("Segoe UI", 9),
            ForeColor = ColorTextoSuave,
            AutoSize = true,
            Location = new Point(0, 32)
        };

        var tabla = new Panel
        {
            Location = new Point(0, 70),
            Width = 660,
            Height = 260,
            BackColor = ColorTarjeta
        };

        string[,] filas =
        {
            { "Función (F)", _riesgo.Funcion.ToString() },
            { "Sustitución (S)", _riesgo.Sustitucion.ToString() },
            { "Profundidad (D)", _riesgo.Profundidad.ToString() },
            { "Extensión (E)", _riesgo.Extension.ToString() },
            { "Agresión (A)", _riesgo.Agresion.ToString() },
            { "Vulnerabilidad (V)", _riesgo.Vulnerabilidad.ToString() },
        };

        int filaTop = 16;
        for (int i = 0; i < filas.GetLength(0); i++)
        {
            tabla.Controls.Add(new Label
            {
                Text = filas[i, 0],
                ForeColor = ColorTextoSuave,
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, filaTop),
                AutoSize = true
            });
            tabla.Controls.Add(new Label
            {
                Text = filas[i, 1],
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(300, filaTop),
                AutoSize = true
            });
            filaTop += 26;
        }

        _lblImportancia = ResultadoLabel($"Importancia (I = F+S+D+E): {_riesgo.Importancia}", filaTop + 10);
        _lblProbabilidad = ResultadoLabel($"Probabilidad (P = A+V): {_riesgo.Probabilidad}", filaTop + 36);
        _lblEvaluacion = ResultadoLabel($"Evaluación del Riesgo (ER = I x P): {_riesgo.EvaluacionRiesgo}", filaTop + 62);

        tabla.Controls.Add(_lblImportancia);
        tabla.Controls.Add(_lblProbabilidad);
        tabla.Controls.Add(_lblEvaluacion);

        _pnlNivel = new Panel
        {
            Location = new Point(0, 350),
            Width = 660,
            Height = 70,
            BackColor = MoslerCalculator.ObtenerColor(_riesgo.NivelRiesgo)
        };
        _lblNivel = new Label
        {
            Text = $"NIVEL DE RIESGO: {_riesgo.NivelRiesgo.ToUpper()}",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        _pnlNivel.Controls.Add(_lblNivel);

        raiz.Controls.Add(lblTitulo);
        raiz.Controls.Add(lblEvaluador);
        raiz.Controls.Add(tabla);
        raiz.Controls.Add(_pnlNivel);

        return raiz;
    }

    private Label ResultadoLabel(string texto, int top)
    {
        return new Label
        {
            Text = texto,
            ForeColor = ColorAcento,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Location = new Point(20, top),
            AutoSize = true
        };
    }

  
    //  PASO 4: Guardar en LiteDB y Exportar a Excel
   

    private Control ConstruirPaso4()
    {
        var raiz = new Panel { Dock = DockStyle.Fill, BackColor = ColorFondo };

        var lblTitulo = new Label
        {
            Text = "Confirmar y Guardar",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(0, 0)
        };

        var btnGuardar = CrearBoton("💾 Guardar en Base de Datos", ColorAcento);
        btnGuardar.Width = 260;
        btnGuardar.Location = new Point(0, 110);
        btnGuardar.Click += (s, e) =>
        {
            GuardarDatosPaso2(); // asegura últimos valores si el usuario regresó
            _repositorio.Guardar(_riesgo);
            _lblEstadoGuardado.Text = "✔ Riesgo guardado correctamente en riskup.db";
            _lblEstadoGuardado.ForeColor = Color.FromArgb(46, 204, 113);
        };

        var btnExportar = CrearBoton("📊 Exportar a Excel", ColorTarjeta);
        btnExportar.Width = 260;
        btnExportar.Location = new Point(280, 110);
        btnExportar.Click += BtnExportarExcel_Click;

        _lblEstadoGuardado = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            AutoSize = true,
            Location = new Point(0, 170)
        };

        raiz.Controls.Add(lblTitulo);
        raiz.Controls.Add(_lblResumen4);
        raiz.Controls.Add(btnGuardar);
        raiz.Controls.Add(btnExportar);
        raiz.Controls.Add(_lblEstadoGuardado);

        return raiz;
    }

    private void BtnExportarExcel_Click(object? sender, EventArgs e)
    {
        using var dialogo = new SaveFileDialog
        {
            Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
            FileName = $"Riesgo_{_riesgo.NombreRiesgo}_{DateTime.Now:yyyyMMdd}.xlsx"
        };

        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                ExcelExporter.ExportarRiesgo(_riesgo, dialogo.FileName);
                MessageBox.Show(this, "El riesgo se exportó correctamente a Excel.", "RiskUp",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Ocurrió un error al exportar: {ex.Message}", "RiskUp",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _repositorio.Dispose();
        base.OnFormClosed(e);
    }
}
