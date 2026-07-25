using RiskUp.Data;
using RiskUp.Helpers;
using RiskUp.Models;

namespace RiskUp.Forms;

public class RiesgosGuardadosForm : Form
{
    private static readonly Color ColorFondo = Color.FromArgb(19, 21, 40);
    private static readonly Color ColorTarjeta = Color.FromArgb(30, 33, 61);
    private static readonly Color ColorAcento = Color.FromArgb(255, 159, 28);

    private readonly RiesgoRepository _repositorio = new();
    private readonly DataGridView _grid;
    private List<Riesgo> _riesgos = new();

    public RiesgosGuardadosForm()
    {
        Text = "RiskUp - Riesgos Guardados";
        Size = new Size(920, 560);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = ColorFondo;

        string rutaIcono = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.ico");
        if (File.Exists(rutaIcono)) Icon = new Icon(rutaIcono);

        var pnlHeader = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = ColorFondo };
        var lblTitulo = new Label
        {
            Text = "Riesgos Registrados",
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(20, 12),
            AutoSize = true
        };
        pnlHeader.Controls.Add(lblTitulo);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = ColorTarjeta,
            ForeColor = Color.Black,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };

        var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = ColorFondo };
        var btnExportarTodo = CrearBoton("📊 Exportar Todo a Excel", ColorAcento);
        btnExportarTodo.Location = new Point(20, 10);
        btnExportarTodo.Click += BtnExportarTodo_Click;

        var btnExportarSeleccionado = CrearBoton("📄 Exportar Seleccionado", ColorTarjeta);
        btnExportarSeleccionado.Location = new Point(240, 10);
        btnExportarSeleccionado.Click += BtnExportarSeleccionado_Click;

        var btnEliminar = CrearBoton("🗑 Eliminar", ColorTarjeta);
        btnEliminar.Location = new Point(460, 10);
        btnEliminar.Click += BtnEliminar_Click;

        var btnCerrar = CrearBoton("Cerrar", ColorTarjeta);
        btnCerrar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCerrar.Location = new Point(920 - 170, 10);
        btnCerrar.Click += (s, e) => Close();

        pnlFooter.Controls.Add(btnExportarTodo);
        pnlFooter.Controls.Add(btnExportarSeleccionado);
        pnlFooter.Controls.Add(btnEliminar);
        pnlFooter.Controls.Add(btnCerrar);
        pnlFooter.Resize += (s, e) => btnCerrar.Location = new Point(pnlFooter.Width - 170, 10);

        Controls.Add(_grid);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);

        CargarDatos();
    }

    private Button CrearBoton(string texto, Color color)
    {
        return new Button
        {
            Text = texto,
            Width = 200,
            Height = 40,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            FlatAppearance = { BorderSize = 0 },
            Cursor = Cursors.Hand
        };
    }

    private void CargarDatos()
    {
        _riesgos = _repositorio.ObtenerTodos();

        var vista = _riesgos.Select(r => new
        {
            r.NombreRiesgo,
            r.UsuarioEvaluador,
            r.Importancia,
            r.Probabilidad,
            EvaluacionRiesgo = r.EvaluacionRiesgo,
            r.NivelRiesgo,
            Fecha = r.FechaRegistro.ToString("dd/MM/yyyy HH:mm")
        }).ToList();

        _grid.DataSource = vista;

        if (_grid.Columns.Contains("NombreRiesgo")) _grid.Columns["NombreRiesgo"].HeaderText = "Riesgo";
        if (_grid.Columns.Contains("UsuarioEvaluador")) _grid.Columns["UsuarioEvaluador"].HeaderText = "Evaluador";
        if (_grid.Columns.Contains("EvaluacionRiesgo")) _grid.Columns["EvaluacionRiesgo"].HeaderText = "ER";
        if (_grid.Columns.Contains("NivelRiesgo")) _grid.Columns["NivelRiesgo"].HeaderText = "Nivel";
    }

    private Riesgo? ObtenerSeleccionado()
    {
        if (_grid.SelectedRows.Count == 0) return null;
        int indice = _grid.SelectedRows[0].Index;
        if (indice < 0 || indice >= _riesgos.Count) return null;
        return _riesgos[indice];
    }

    private void BtnExportarTodo_Click(object? sender, EventArgs e)
    {
        if (_riesgos.Count == 0)
        {
            MessageBox.Show(this, "No hay riesgos registrados para exportar.", "RiskUp",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialogo = new SaveFileDialog
        {
            Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
            FileName = $"RiskUp_Riesgos_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                ExcelExporter.ExportarLista(_riesgos, dialogo.FileName);
                MessageBox.Show(this, "Listado exportado correctamente.", "RiskUp",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al exportar: {ex.Message}", "RiskUp",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnExportarSeleccionado_Click(object? sender, EventArgs e)
    {
        var riesgo = ObtenerSeleccionado();
        if (riesgo is null)
        {
            MessageBox.Show(this, "Seleccione un riesgo de la lista.", "RiskUp",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dialogo = new SaveFileDialog
        {
            Filter = "Archivo de Excel (*.xlsx)|*.xlsx",
            FileName = $"Riesgo_{riesgo.NombreRiesgo}_{DateTime.Now:yyyyMMdd}.xlsx"
        };
        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                ExcelExporter.ExportarRiesgo(riesgo, dialogo.FileName);
                MessageBox.Show(this, "Riesgo exportado correctamente.", "RiskUp",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error al exportar: {ex.Message}", "RiskUp",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        var riesgo = ObtenerSeleccionado();
        if (riesgo is null)
        {
            MessageBox.Show(this, "Seleccione un riesgo de la lista.", "RiskUp",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirmacion = MessageBox.Show(this, $"¿Eliminar el riesgo \"{riesgo.NombreRiesgo}\"?", "RiskUp",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirmacion == DialogResult.Yes)
        {
            _repositorio.Eliminar(riesgo.Id);
            CargarDatos();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _repositorio.Dispose();
        base.OnFormClosed(e);
    }
}
