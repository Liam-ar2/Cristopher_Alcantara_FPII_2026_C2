using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Services
{
    public static class ServicioService
    {
        /// <summary>Guarda el servicio y su resultado de calculo en la base de datos (historial).</summary>
        public static int Guardar(ServicioTransporte s, ResultadoCalculo r)
        {
            using (var con = ConexionBD.Abrir())
            using (var tx = con.BeginTransaction())
            {
const string sqlServ = @"INSERT INTO Servicios
                    (VehiculoId, ConductorId, Fecha, TipoServicio, DistanciaKm, Pasajeros, CargaKg, Horas, PeajesManual, MargenGanancia)
                    VALUES (@v, @c, @f, @t, @d, @p, @carga, @h, @peaje, @margen);";
                using (var cmd = new SQLiteCommand(sqlServ, con, tx))
                {
                    cmd.Parameters.AddWithValue("@v", s.VehiculoId);
                    cmd.Parameters.AddWithValue("@c", (object)s.ConductorId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@f", s.Fecha.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@t", (int)s.TipoServicio);
                    cmd.Parameters.AddWithValue("@d", (double)s.DistanciaKm);
                    cmd.Parameters.AddWithValue("@p", s.Pasajeros);
                    cmd.Parameters.AddWithValue("@carga", (double)s.CargaKg);
                    cmd.Parameters.AddWithValue("@h", (double)s.Horas);
                    cmd.Parameters.AddWithValue("@peaje", (double)s.PeajesManualRd);
                    cmd.Parameters.AddWithValue("@margen", (double)s.MargenGanancia);
                    cmd.ExecuteNonQuery();
                }

                int servicioId = Convert.ToInt32(new SQLiteCommand("SELECT last_insert_rowid();", con, tx)
                    .ExecuteScalar());

                const string sqlRes = @"INSERT INTO Resultados
                    (ServicioId, DistanciaKm, CombustibleUtilizado, CostoCombustible, CostoMantenimiento, CostoSeguro,
                     CostoConductor, CostoPeajes, CostoOtros, CostoTotalServicio, CostoPorKilometro, CostoPorPasajero,
                     CostoPorCargaKg, Ganancia, PrecioFinal)
                    VALUES (@s, @d, @cu, @cc, @cm, @cs, @cd, @cp, @co, @ct, @porkm, @porpas, @porcarga, @gan, @pre);";
                using (var cmd = new SQLiteCommand(sqlRes, con, tx))
                {
                    cmd.Parameters.AddWithValue("@s", servicioId);
                    cmd.Parameters.AddWithValue("@d", (double)r.DistanciaKm);
                    cmd.Parameters.AddWithValue("@cu", (double)r.CombustibleUtilizado);
                    cmd.Parameters.AddWithValue("@cc", (double)r.CostoCombustible);
                    cmd.Parameters.AddWithValue("@cm", (double)r.CostoMantenimiento);
                    cmd.Parameters.AddWithValue("@cs", (double)r.CostoSeguro);
                    cmd.Parameters.AddWithValue("@cd", (double)r.CostoConductor);
                    cmd.Parameters.AddWithValue("@cp", (double)r.CostoPeajes);
                    cmd.Parameters.AddWithValue("@co", (double)r.CostoOtros);
                    cmd.Parameters.AddWithValue("@ct", (double)r.CostoTotalServicio);
                    cmd.Parameters.AddWithValue("@porkm", (double)r.CostoPorKilometro);
                    cmd.Parameters.AddWithValue("@porpas", (double)r.CostoPorPasajero);
                    cmd.Parameters.AddWithValue("@porcarga", (double)r.CostoPorCargaKg);
                    cmd.Parameters.AddWithValue("@gan", (double)r.Ganancia);
                    cmd.Parameters.AddWithValue("@pre", (double)r.PrecioFinalRecomendado);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return servicioId;
            }
        }

        /// <summary>Devuelve las filas del historial para mostrarlas en DataGridView.</summary>
        public static DataTable ObtenerHistorial()
        {
            const string sql = @"SELECT s.Id, s.Fecha,
                        s.DistanciaKm AS DistanciaKm,
                        s.TipoServicio,
                        v.Marca || ' ' || v.Modelo || ' (' || v.Placa || ')' AS Vehiculo,
                        COALESCE(c.Nombre, '(Sin conductor)') AS Conductor,
                        s.Pasajeros, s.CargaKg, s.Horas, s.MargenGanancia,
                        printf('RD$ %.2f', r.CostoTotalServicio)  AS CostoTotal,
                        printf('RD$ %.2f', r.CostoPorKilometro)   AS CostoPorKm,
                        printf('RD$ %.2f', r.Ganancia)            AS Ganancia,
                        printf('RD$ %.2f', r.PrecioFinal)         AS PrecioFinal
                    FROM Servicios s
                    JOIN Vehiculos v ON v.Id = s.VehiculoId
                    LEFT JOIN Conductores c ON c.Id = s.ConductorId
                    LEFT JOIN Resultados r ON r.ServicioId = s.Id
                    ORDER BY s.Fecha DESC, s.Id DESC;";

            using (var con = ConexionBD.Abrir())
            using (var cmd = new SQLiteCommand(sql, con))
            using (var da = new SQLiteDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static void Eliminar(int servicioId)
        {
            ConexionBD.EjecutarNoQuery("DELETE FROM Servicios WHERE Id=@id;",
                new SQLiteParameter("@id", servicioId));
        }
    }
}