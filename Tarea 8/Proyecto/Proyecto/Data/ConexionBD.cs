using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Proyecto.Data
{
    /// <summary>
    /// Administra la conexion con la base de datos SQLite local.
    /// La base de datos se guarda en la carpeta Datos (junto al ejecutable).
    /// </summary>
    public static class ConexionBD
    {
        public static string RutaArchivo { get; private set; }
        public static string CadenaConexion { get; private set; }

        public static void Configurar()
        {
            string dir = Path.Combine(Application.StartupPath, "Datos");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            RutaArchivo = Path.Combine(dir, "transporte.db");
            CadenaConexion = "Data Source=" + RutaArchivo + ";Version=3;";
        }

        public static SQLiteConnection Abrir()
        {
            var conexion = new SQLiteConnection(CadenaConexion);
            conexion.Open();
            using (var cmd = conexion.CreateCommand())
            {
                cmd.CommandText = "PRAGMA foreign_keys = ON;";
                cmd.ExecuteNonQuery();
            }
            return conexion;
        }

        public static void EjecutarNoQuery(string sql, params SQLiteParameter[] parametros)
        {
            using (var con = Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddRange(parametros);
                cmd.ExecuteNonQuery();
            }
        }

        public static object EjecutarEscalar(string sql, params SQLiteParameter[] parametros)
        {
            using (var con = Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            {
                cmd.Parameters.AddRange(parametros);
                return cmd.ExecuteScalar();
            }
        }
    }
}