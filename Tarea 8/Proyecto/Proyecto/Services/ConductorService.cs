using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Services
{
    public static class ConductorService
    {
        public static List<Conductor> ObtenerTodos()
        {
            var lista = new List<Conductor>();
            const string sql = @"SELECT * FROM Conductores ORDER BY Nombre;";
            using (var con = ConexionBD.Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    lista.Add(Leer(rd));
            }
            return lista;
        }

        public static Conductor ObtenerPorId(int id)
        {
            const string sql = @"SELECT * FROM Conductores WHERE Id = @id;";
            using (var con = ConexionBD.Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var rd = cmd.ExecuteReader())
                    return rd.Read() ? Leer(rd) : null;
            }
        }

        private static Conductor Leer(SQLiteDataReader rd)
        {
            return new Conductor
            {
                Id = Convert.ToInt32(rd["Id"]),
                Documento = rd["Documento"].ToString(),
                Nombre = rd["Nombre"].ToString(),
                Telefono = rd["Telefono"].ToString(),
                SalarioMensual = Convert.ToDecimal(rd["SalarioMensual"])
            };
        }

        public static void Guardar(Conductor c)
        {
            if (c.Id == 0)
            {
                ConexionBD.EjecutarNoQuery(
                    @"INSERT INTO Conductores (Documento, Nombre, Telefono, SalarioMensual)
                      VALUES (@d, @n, @t, @s);", Parametros(c));
            }
            else
            {
                var pars = new List<SQLiteParameter>(Parametros(c)) { new SQLiteParameter("@id", c.Id) };
                ConexionBD.EjecutarNoQuery(
                    @"UPDATE Conductores SET Documento=@d, Nombre=@n, Telefono=@t, SalarioMensual=@s
                      WHERE Id=@id;", pars.ToArray());
            }
        }

        public static void Eliminar(int id)
        {
            ConexionBD.EjecutarNoQuery("DELETE FROM Conductores WHERE Id=@id;",
                new SQLiteParameter("@id", id));
        }

        private static SQLiteParameter[] Parametros(Conductor c)
        {
            return new[]
            {
                new SQLiteParameter("@d", c.Documento),
                new SQLiteParameter("@n", c.Nombre),
                new SQLiteParameter("@t", c.Telefono ?? ""),
                new SQLiteParameter("@s", (double)c.SalarioMensual)
            };
        }
    }
}