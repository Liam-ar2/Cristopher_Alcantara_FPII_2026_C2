using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Services
{
    public static class VehiculoService
    {
        public static List<Vehiculo> ObtenerTodos()
        {
            var lista = new List<Vehiculo>();
            const string sql = @"SELECT * FROM Vehiculos ORDER BY Marca, Modelo;";
            using (var con = ConexionBD.Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    lista.Add(Leer(rd));
            }
            return lista;
        }

        public static Vehiculo ObtenerPorId(int id)
        {
            const string sql = @"SELECT * FROM Vehiculos WHERE Id = @id;";
            using (var con = ConexionBD.Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var rd = cmd.ExecuteReader())
                    return rd.Read() ? Leer(rd) : null;
            }
        }

        private static Vehiculo Leer(SQLiteDataReader rd)
        {
            return new Vehiculo
            {
                Id = Convert.ToInt32(rd["Id"]),
                Placa = rd["Placa"].ToString(),
                Marca = rd["Marca"].ToString(),
                Modelo = rd["Modelo"].ToString(),
                Tipo = (TipoVehiculo)Convert.ToByte(rd["Tipo"]),
                CapacidadPasajeros = Convert.ToInt32(rd["CapacidadPasajeros"]),
                CapacidadCargaKg = Convert.ToDecimal(rd["CapacidadCargaKg"]),
                RendimientoPorUnidad = Convert.ToDecimal(rd["Rendimiento"]),
                UnidadCombustible = (UnidadCombustible)Convert.ToByte(rd["UnidadCombustible"]),
                KilometrajeMensual = Convert.ToDecimal(rd["KilometrajeMensual"])
            };
        }

        public static void Guardar(Vehiculo v)
        {
            if (v.Id == 0)
            {
                const string sql = @"INSERT INTO Vehiculos
                    (Placa, Marca, Modelo, Tipo, CapacidadPasajeros, CapacidadCargaKg, Rendimiento, UnidadCombustible, KilometrajeMensual)
                    VALUES (@p, @m, @mo, @t, @cp, @cc, @r, @u, @k);";
                ConexionBD.EjecutarNoQuery(sql, Parametros(v));
            }
            else
            {
                const string sql = @"UPDATE Vehiculos SET
                    Placa=@p, Marca=@m, Modelo=@mo, Tipo=@t, CapacidadPasajeros=@cp,
                    CapacidadCargaKg=@cc, Rendimiento=@r, UnidadCombustible=@u, KilometrajeMensual=@k
                    WHERE Id=@id;";
                var pars = new List<SQLiteParameter>(Parametros(v)) { new SQLiteParameter("@id", v.Id) };
                ConexionBD.EjecutarNoQuery(sql, pars.ToArray());
            }
        }

        public static void Eliminar(int id)
        {
            ConexionBD.EjecutarNoQuery("DELETE FROM Vehiculos WHERE Id=@id;",
                new SQLiteParameter("@id", id));
        }

        private static SQLiteParameter[] Parametros(Vehiculo v)
        {
            return new[]
            {
                new SQLiteParameter("@p", v.Placa),
                new SQLiteParameter("@m", v.Marca),
                new SQLiteParameter("@mo", v.Modelo),
                new SQLiteParameter("@t", (int)v.Tipo),
                new SQLiteParameter("@cp", v.CapacidadPasajeros),
                new SQLiteParameter("@cc", (double)v.CapacidadCargaKg),
                new SQLiteParameter("@r", (double)v.RendimientoPorUnidad),
                new SQLiteParameter("@u", (int)v.UnidadCombustible),
                new SQLiteParameter("@k", (double)v.KilometrajeMensual)
            };
        }
    }
}