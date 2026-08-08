using System;
using System.Data.SQLite;
using Proyecto.Models;

namespace Proyecto.Data
{
    /// <summary>
    /// Crea las tablas de la base de datos y registra datos de ejemplo
    /// la primera vez que se ejecuta el programa.
    /// </summary>
    public static class InicializadorBD
    {
        public static void Iniciar()
        {
            ConexionBD.Configurar();
            CrearTablas();
            RegistrarEjemplo();
        }

        private static void CrearTablas()
        {
            const string script = @"
CREATE TABLE IF NOT EXISTS Vehiculos (
    Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
    Placa               TEXT    NOT NULL,
    Marca               TEXT    NOT NULL,
    Modelo              TEXT    NOT NULL,
    Tipo                INTEGER NOT NULL,
    CapacidadPasajeros  INTEGER NOT NULL,
    CapacidadCargaKg    REAL    NOT NULL,
    Rendimiento         REAL    NOT NULL,
    UnidadCombustible   INTEGER NOT NULL,
    KilometrajeMensual  REAL    NOT NULL
);

CREATE TABLE IF NOT EXISTS Conductores (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
    Documento      TEXT    NOT NULL,
    Nombre         TEXT    NOT NULL,
    Telefono       TEXT,
    SalarioMensual REAL    NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS CostosVehiculo (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    VehiculoId  INTEGER NOT NULL REFERENCES Vehiculos(Id) ON DELETE CASCADE,
    Nombre      TEXT    NOT NULL,
    Categoria   INTEGER NOT NULL,
    Tipo        INTEGER NOT NULL,
    Periodicidad INTEGER NOT NULL,
    Monto       REAL    NOT NULL
);

CREATE TABLE IF NOT EXISTS Configuracion (
    Clave TEXT PRIMARY KEY,
    Valor TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Servicios (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    VehiculoId    INTEGER NOT NULL REFERENCES Vehiculos(Id),
    ConductorId   INTEGER REFERENCES Conductores(Id),
    Fecha         TEXT    NOT NULL,
    TipoServicio  INTEGER NOT NULL,
    DistanciaKm   REAL    NOT NULL,
    Pasajeros     INTEGER NOT NULL DEFAULT 0,
    CargaKg       REAL    NOT NULL DEFAULT 0,
    Horas         REAL    NOT NULL DEFAULT 0,
    PeajesManual  REAL    NOT NULL DEFAULT 0,
    MargenGanancia REAL   NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Resultados (
    Id                   INTEGER PRIMARY KEY AUTOINCREMENT,
    ServicioId           INTEGER NOT NULL REFERENCES Servicios(Id) ON DELETE CASCADE,
    DistanciaKm          REAL,
    CombustibleUtilizado REAL,
    CostoCombustible     REAL,
    CostoMantenimiento   REAL,
    CostoSeguro          REAL,
    CostoConductor       REAL,
    CostoPeajes          REAL,
    CostoOtros           REAL,
    CostoTotalServicio   REAL,
    CostoPorKilometro    REAL,
    CostoPorPasajero     REAL,
    CostoPorCargaKg      REAL,
    Ganancia             REAL,
    PrecioFinal          REAL
);
";
            using (var con = ConexionBD.Abrir())
            {
                var ordenes = script.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var orden in ordenes)
                {
                    if (string.IsNullOrWhiteSpace(orden)) continue;
                    using (var cmd = new SQLiteCommand(orden, con))
                        cmd.ExecuteNonQuery();
                }
            }
        }

        private static void RegistrarEjemplo()
        {
            object existe = ConexionBD.EjecutarEscalar("SELECT COUNT(*) FROM Vehiculos;");
            if (existe != null && Convert.ToInt32(existe) > 0) return;

            InsertarVehiculo("A123456", "Toyota", "Corolla", (int)TipoVehiculo.Automovil, 4, 400, 22m, (int)UnidadCombustible.Galon, 2200m);
            InsertarVehiculo("B765432", "Hyundai", "Tucson", (int)TipoVehiculo.SUV, 5, 600, 18m, (int)UnidadCombustible.Galon, 1800m);
            InsertarVehiculo("C987654", "Mercedes", "Sprinter", (int)TipoVehiculo.Minibus, 19, 1500, 14m, (int)UnidadCombustible.Galon, 2500m);

            InsertarConductor("001-1234567-8", "Juan Perez", "809-555-0101", 30000m);
            InsertarConductor("001-8765432-1", "Maria Rodriguez", "809-555-0202", 28000m);

            InsertarCosto(1, "Mantenimiento basico", (int)CategoriaCosto.Mantenimiento, (int)TipoCosto.Variable, (int)PeriodicidadCosto.PorKilometro, 1.20m);
            InsertarCosto(1, "Seguro vehicular", (int)CategoriaCosto.Seguro, (int)TipoCosto.Fijo, (int)PeriodicidadCosto.Mensual, 8200m);
            InsertarCosto(1, "Parqueo y lavado", (int)CategoriaCosto.Otros, (int)TipoCosto.Fijo, (int)PeriodicidadCosto.Mensual, 1500m);

            InsertarCosto(2, "Mantenimiento basico", (int)CategoriaCosto.Mantenimiento, (int)TipoCosto.Variable, (int)PeriodicidadCosto.PorKilometro, 1.50m);
            InsertarCosto(2, "Seguro vehicular", (int)CategoriaCosto.Seguro, (int)TipoCosto.Fijo, (int)PeriodicidadCosto.Mensual, 9800m);

            InsertarCosto(3, "Mantenimiento basico", (int)CategoriaCosto.Mantenimiento, (int)TipoCosto.Variable, (int)PeriodicidadCosto.PorKilometro, 2.20m);
            InsertarCosto(3, "Seguro vehicular", (int)CategoriaCosto.Seguro, (int)TipoCosto.Fijo, (int)PeriodicidadCosto.Mensual, 15000m);

            InsertarConfig("PrecioCombustible", "210.00");
            InsertarConfig("UnidadCombustible", ((int)UnidadCombustible.Galon).ToString());
        }

        private static int InsertarVehiculo(string placa, string marca, string modelo, int tipo,
            int pasajeros, decimal carga, decimal rendimiento, int unidad, decimal kmMes)
        {
            const string sql = @"INSERT INTO Vehiculos
                (Placa, Marca, Modelo, Tipo, CapacidadPasajeros, CapacidadCargaKg, Rendimiento, UnidadCombustible, KilometrajeMensual)
                VALUES (@p, @m, @mo, @t, @cp, @cc, @r, @u, @k);";
            ConexionBD.EjecutarNoQuery(sql,
                new SQLiteParameter("@p", placa), new SQLiteParameter("@m", marca),
                new SQLiteParameter("@mo", modelo), new SQLiteParameter("@t", tipo),
                new SQLiteParameter("@cp", pasajeros), new SQLiteParameter("@cc", (double)carga),
                new SQLiteParameter("@r", (double)rendimiento), new SQLiteParameter("@u", unidad),
                new SQLiteParameter("@k", (double)kmMes));
            return Convert.ToInt32(ConexionBD.EjecutarEscalar("SELECT last_insert_rowid();"));
        }

        private static int InsertarConductor(string documento, string nombre, string telefono, decimal salario)
        {
            const string sql = @"INSERT INTO Conductores (Documento, Nombre, Telefono, SalarioMensual)
                VALUES (@d, @n, @t, @s);";
            ConexionBD.EjecutarNoQuery(sql,
                new SQLiteParameter("@d", documento), new SQLiteParameter("@n", nombre),
                new SQLiteParameter("@t", telefono), new SQLiteParameter("@s", (double)salario));
            return Convert.ToInt32(ConexionBD.EjecutarEscalar("SELECT last_insert_rowid();"));
        }

        private static void InsertarCosto(int vehiculoId, string nombre, int categoria, int tipo, int periodicidad, decimal monto)
        {
            const string sql = @"INSERT INTO CostosVehiculo (VehiculoId, Nombre, Categoria, Tipo, Periodicidad, Monto)
                VALUES (@v, @n, @c, @t, @p, @m);";
            ConexionBD.EjecutarNoQuery(sql,
                new SQLiteParameter("@v", vehiculoId), new SQLiteParameter("@n", nombre),
                new SQLiteParameter("@c", categoria), new SQLiteParameter("@t", tipo),
                new SQLiteParameter("@p", periodicidad), new SQLiteParameter("@m", (double)monto));
        }

        private static void InsertarConfig(string clave, string valor)
        {
            ConexionBD.EjecutarNoQuery("INSERT INTO Configuracion (Clave, Valor) VALUES (@c, @v);",
                new SQLiteParameter("@c", clave), new SQLiteParameter("@v", valor));
        }
    }
}