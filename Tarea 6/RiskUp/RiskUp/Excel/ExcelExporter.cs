using ClosedXML.Excel;
using RiskUp.Models;

namespace RiskUp.Helpers;


public static class ExcelExporter
{
    public static void ExportarRiesgo(Riesgo riesgo, string rutaArchivo)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Riesgo Mosler");

        ws.Column(1).Width = 28;
        ws.Column(2).Width = 40;

        int fila = 1;

        // Título
        ws.Cell(fila, 1).Value = "RISKUP - EVALUACIÓN DE RIESGO (MÉTODO MOSLER)";
        ws.Range(fila, 1, fila, 2).Merge().Style.Font.SetBold().Font.SetFontSize(14);
        ws.Range(fila, 1, fila, 2).Style.Fill.SetBackgroundColor(XLColor.FromArgb(26, 29, 54));
        ws.Range(fila, 1, fila, 2).Style.Font.SetFontColor(XLColor.White);
        fila += 2;

        void Fila(string etiqueta, object valor, bool negrita = false)
        {
            ws.Cell(fila, 1).Value = etiqueta;
            ws.Cell(fila, 1).Style.Font.SetBold();
            ws.Cell(fila, 2).Value = valor?.ToString() ?? "";
            if (negrita) ws.Cell(fila, 2).Style.Font.SetBold();
            fila++;
        }

        Fila("Usuario Evaluador:", riesgo.UsuarioEvaluador);
        Fila("Nombre del Riesgo:", riesgo.NombreRiesgo);
        Fila("Descripción:", riesgo.Descripcion);
        Fila("Fecha de Registro:", riesgo.FechaRegistro.ToString("dd/MM/yyyy HH:mm"));
        fila++;

        ws.Cell(fila, 1).Value = "CRITERIOS DE EVALUACIÓN";
        ws.Range(fila, 1, fila, 2).Merge().Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        fila++;

        Fila("Función (F):", riesgo.Funcion);
        Fila("Sustitución (S):", riesgo.Sustitucion);
        Fila("Profundidad (D):", riesgo.Profundidad);
        Fila("Extensión (E):", riesgo.Extension);
        Fila("Agresión (A):", riesgo.Agresion);
        Fila("Vulnerabilidad (V):", riesgo.Vulnerabilidad);
        fila++;

        ws.Cell(fila, 1).Value = "RESULTADOS";
        ws.Range(fila, 1, fila, 2).Merge().Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);
        fila++;

        Fila("Importancia (I = F+S+D+E):", riesgo.Importancia);
        Fila("Probabilidad (P = A+V):", riesgo.Probabilidad);
        Fila("Evaluación del Riesgo (ER = I x P):", riesgo.EvaluacionRiesgo, true);
        Fila("Nivel de Riesgo:", riesgo.NivelRiesgo, true);

        var celdaNivel = ws.Cell(fila - 1, 2);
        var color = MoslerCalculator.ObtenerColor(riesgo.NivelRiesgo);
        celdaNivel.Style.Fill.SetBackgroundColor(XLColor.FromArgb(color.R, color.G, color.B));
        celdaNivel.Style.Font.SetFontColor(XLColor.White);

        wb.SaveAs(rutaArchivo);
    }

    public static void ExportarLista(List<Riesgo> riesgos, string rutaArchivo)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Riesgos");

        string[] encabezados =
        {
            "Usuario Evaluador", "Nombre del Riesgo", "Descripción",
            "F", "S", "D", "E", "A", "V",
            "Importancia", "Probabilidad", "Evaluación del Riesgo", "Nivel de Riesgo",
            "Fecha de Registro"
        };

        for (int i = 0; i < encabezados.Length; i++)
        {
            ws.Cell(1, i + 1).Value = encabezados[i];
        }
        ws.Range(1, 1, 1, encabezados.Length).Style.Font.SetBold()
          .Fill.SetBackgroundColor(XLColor.FromArgb(26, 29, 54))
          .Font.SetFontColor(XLColor.White);

        int fila = 2;
        foreach (var r in riesgos)
        {
            ws.Cell(fila, 1).Value = r.UsuarioEvaluador;
            ws.Cell(fila, 2).Value = r.NombreRiesgo;
            ws.Cell(fila, 3).Value = r.Descripcion;
            ws.Cell(fila, 4).Value = r.Funcion;
            ws.Cell(fila, 5).Value = r.Sustitucion;
            ws.Cell(fila, 6).Value = r.Profundidad;
            ws.Cell(fila, 7).Value = r.Extension;
            ws.Cell(fila, 8).Value = r.Agresion;
            ws.Cell(fila, 9).Value = r.Vulnerabilidad;
            ws.Cell(fila, 10).Value = r.Importancia;
            ws.Cell(fila, 11).Value = r.Probabilidad;
            ws.Cell(fila, 12).Value = r.EvaluacionRiesgo;
            ws.Cell(fila, 13).Value = r.NivelRiesgo;
            ws.Cell(fila, 14).Value = r.FechaRegistro.ToString("dd/MM/yyyy HH:mm");
            fila++;
        }

        ws.Columns().AdjustToContents();
        wb.SaveAs(rutaArchivo);
    }
}
