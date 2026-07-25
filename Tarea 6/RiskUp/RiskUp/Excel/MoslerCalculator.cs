namespace RiskUp.Helpers;

public static class MoslerCalculator
{
    public static string ObtenerNivel(int evaluacionRiesgo)
    {
        return evaluacionRiesgo switch
        {
            <= 40 => "Muy Bajo",
            <= 80 => "Bajo",
            <= 120 => "Medio",
            <= 160 => "Alto",
            _ => "Muy Alto"
        };
    }

    public static Color ObtenerColor(string nivel)
    {
        return nivel switch
        {
            "Muy Bajo" => Color.FromArgb(46, 204, 113),
            "Bajo" => Color.FromArgb(155, 216, 89),
            "Medio" => Color.FromArgb(255, 193, 7),
            "Alto" => Color.FromArgb(255, 127, 42),
            _ => Color.FromArgb(220, 53, 69)
        };
    }
}
