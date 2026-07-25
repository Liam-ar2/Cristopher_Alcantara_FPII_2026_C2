using LiteDB;
using RiskUp.Helpers;

namespace RiskUp.Models;

public class Riesgo
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.Empty;

    
    public string UsuarioEvaluador { get; set; } = string.Empty;
    public string NombreRiesgo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // ----- Fase 2: 
    public int Funcion { get; set; } = 1;         // F
    public int Sustitucion { get; set; } = 1;     // S
    public int Profundidad { get; set; } = 1;     // D
    public int Extension { get; set; } = 1;       // E
    public int Agresion { get; set; } = 1;         // A
    public int Vulnerabilidad { get; set; } = 1;   // V

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // ----- Fase 3: 
    
    [BsonIgnore]
    public int Importancia => Funcion + Sustitucion + Profundidad + Extension;

  
    [BsonIgnore]
    public int Probabilidad => Agresion + Vulnerabilidad;

    
    [BsonIgnore]
    public int EvaluacionRiesgo => Importancia * Probabilidad;

    [BsonIgnore]
    public string NivelRiesgo => MoslerCalculator.ObtenerNivel(EvaluacionRiesgo);
}
