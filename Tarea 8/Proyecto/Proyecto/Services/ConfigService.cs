using System;
using System.Data.SQLite;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Services
{
    public static class ConfigService
    {
        public static ConfiguracionGlobal Obtener()
        {
            var cfg = new ConfiguracionGlobal
            {
                PrecioCombustible = 0m,
                UnidadCombustible = UnidadCombustible.Galon
            };

            using (var con = ConexionBD.Abrir())
            using (var cmd = new SQLiteCommand("SELECT Clave, Valor FROM Configuracion;", con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    string clave = rd["Clave"].ToString();
                    string valor = rd["Valor"].ToString();
                    if (clave == "PrecioCombustible")
                    {
                        if (decimal.TryParse(valor, out decimal precio))
                            cfg.PrecioCombustible = precio;
                    }
                    else if (clave == "UnidadCombustible")
                    {
                        if (int.TryParse(valor, out int unidad))
                            cfg.UnidadCombustible = (UnidadCombustible)unidad;
                    }
                }
            }
            return cfg;
        }

        public static void Guardar(ConfiguracionGlobal cfg)
        {
            ConexionBD.EjecutarNoQuery(
                @"INSERT INTO Configuracion (Clave, Valor) VALUES ('PrecioCombustible', @v)
                  ON CONFLICT(Clave) DO UPDATE SET Valor = @v;",
                new SQLiteParameter("@v", cfg.PrecioCombustible.ToString()));

            ConexionBD.EjecutarNoQuery(
                @"INSERT INTO Configuracion (Clave, Valor) VALUES ('UnidadCombustible', @v)
                  ON CONFLICT(Clave) DO UPDATE SET Valor = @v;",
                new SQLiteParameter("@v", ((int)cfg.UnidadCombustible).ToString()));
        }
    }
}