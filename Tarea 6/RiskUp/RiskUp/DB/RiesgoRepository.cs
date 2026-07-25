using LiteDB;
using RiskUp.Models;

namespace RiskUp.Data;

public class RiesgoRepository : IDisposable
{
    private const string NombreArchivoDb = "riskup.db";
    private const string NombreColeccion = "riesgos";

    private readonly LiteDatabase _db;
    private readonly ILiteCollection<Riesgo> _coleccion;

    public RiesgoRepository()
    {
        
        string carpeta = AppDomain.CurrentDomain.BaseDirectory;
        string ruta = Path.Combine(carpeta, NombreArchivoDb);

        _db = new LiteDatabase(ruta);
        _coleccion = _db.GetCollection<Riesgo>(NombreColeccion);
        _coleccion.EnsureIndex(x => x.NombreRiesgo);
        _coleccion.EnsureIndex(x => x.FechaRegistro);
    }

    public Riesgo Guardar(Riesgo riesgo)
    {
        if (riesgo.Id == ObjectId.Empty)
        {
            riesgo.Id = ObjectId.NewObjectId();
            _coleccion.Insert(riesgo);
        }
        else
        {
            _coleccion.Update(riesgo);
        }
        return riesgo;
    }

    public List<Riesgo> ObtenerTodos()
    {
        return _coleccion.FindAll()
                          .OrderByDescending(r => r.FechaRegistro)
                          .ToList();
    }

    public Riesgo? ObtenerPorId(ObjectId id) => _coleccion.FindById(id);

    public bool Eliminar(ObjectId id) => _coleccion.Delete(id);

    public void Dispose()
    {
        _db?.Dispose();
        GC.SuppressFinalize(this);
    }
}
