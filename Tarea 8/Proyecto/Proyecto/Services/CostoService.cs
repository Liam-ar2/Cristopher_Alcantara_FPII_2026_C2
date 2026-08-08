using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Services
{
    public static class CostoService
    {
        public static List<CostoVehiculo> ObtenerPorVehiculo(int vehiculoId)
        {
            var lista = new List<CostoVehiculo>();
            const string sql = @"SELECT * FROM CostosVehiculo WHERE VehiculoId = @v ORDER BY Categoria, Nombre;";
            using (var con = ConexionBD.Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@v", vehiculoId);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        lista.Add(Leer(rd));
                }
            }
            return lista;
        }

        private static CostoVehiculo Leer(SQLiteDataReader rd)
        {
            return new CostoVehiculo
            {
                Id = Convert.ToInt32(rd["Id"]),
                VehiculoId = Convert.ToInt32(rd["VehiculoId"]),
                Nombre = rd["Nombre"].ToString(),
                Categoria = (CategoriaCosto)Convert.ToByte(rd["Categoria"]),
                Tipo = (TipoCosto)Convert.ToByte(rd["Tipo"]),
                Periodicidad = (PeriodicidadCosto)Convert.ToByte(rd["Periodicidad"]),
                Monto = Convert.ToDecimal(rd["Monto"])
            };
        }

        public static void Guardar(CostoVehiculo costo)
        {
            if (costo.Id == 0)
            {
                ConexionBD.EjecutarNoQuery(
                    @"INSERT INTO CostosVehiculo (VehiculoId, Nombre, Categoria, Tipo, Periodicidad, Monto)
                      VALUES (@v, @n, @c, @t, @p, @m);", Parametros(costo));
            }
            else
            {
                var pars = new List<SQLiteParameter>(Parametros(costo)) { new SQLiteParameter("@id", costo.Id) };
                ConexionBD.EjecutarNoQuery(
                    @"UPDATE CostosVehiculo SET Nombre=@n, Categoria=@c, Tipo=@t, Periodicidad=@p, Monto=@m
                      WHERE Id=@id;", pars.ToArray());
            }
        }

        public static void Eliminar(int id)
        {
            ConexionBD.EjecutarNoQuery("DELETE FROM CostosVehiculo WHERE Id=@id;",
                new SQLiteParameter("@id", id));
        }

        private static SQLiteParameter[] Parametros(CostoVehiculo c)
        {
            return new[]
            {
                new SQLiteParameter("@v", c.VehiculoId),
                new SQLiteParameter("@n", c.Nombre),
                new SQLiteParameter("@c", (int)c.Categoria),
                new SQLiteParameter("@t", (int)c.Tipo),
                new SQLiteParameter("@p", (int)c.Periodicidad),
                new SQLiteParameter("@m", (double)c.Monto)
            };
        }
    }
}