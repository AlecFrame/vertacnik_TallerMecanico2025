using MySqlConnector;

namespace vertacnik_TallerMecanico2025.Models
{
    public class DetalleRepuestoRepo : RepositorioBase
    {

        public readonly RepuestoRepo _repuestoRepo;
        public readonly ServicioRepo _servicioRepo;
        public DetalleRepuestoRepo(IConfiguration configuration) : base(configuration)
        {
            _repuestoRepo = new RepuestoRepo(configuration);
            _servicioRepo = new ServicioRepo(configuration);
        }

        public void Alta(DetalleRepuesto detalleRepuesto)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO detalles_repuestos (IdRepuesto, IdServicio, Cantidad, CostoTotal) " +
                             "VALUES (@IdRepuesto, @IdServicio, @Cantidad, @CostoTotal)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdRepuesto", detalleRepuesto.IdRepuesto);
                    command.Parameters.AddWithValue("@IdServicio", detalleRepuesto.IdServicio);
                    command.Parameters.AddWithValue("@Cantidad", detalleRepuesto.Cantidad);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IList<DetalleRepuesto> ObtenerPorId(int idServicio)
        {
            IList<DetalleRepuesto> lista = new List<DetalleRepuesto>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM detalles_repuestos WHERE IdServicio = @IdServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdServicio", idServicio);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DetalleRepuesto detalleRepuesto = new DetalleRepuesto
                            {
                                IdDetalleRepuesto = reader.GetInt32("IdDetalleRepuesto"),
                                IdRepuesto = reader.GetInt32("IdRepuesto"),
                                IdServicio = reader.GetInt32("IdServicio"),
                                Cantidad = reader.GetInt32("Cantidad")
                            };

                            // Cargar entidades asociadas
                            detalleRepuesto.Repuesto = _repuestoRepo.ObtenerPorId(detalleRepuesto.IdRepuesto);
                            detalleRepuesto.Servicio = _servicioRepo.ObtenerPorId(detalleRepuesto.IdServicio);

                            lista.Add(detalleRepuesto);
                        }
                    }
                }
            }
            return lista;
        }

        public void Modificacion(DetalleRepuesto detalleRepuesto)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE detalles_repuestos SET IdRepuesto = @IdRepuesto, IdServicio = @IdServicio, " +
                             "Cantidad = @Cantidad WHERE IdDetalleRepuesto = @IdDetalleRepuesto";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdRepuesto", detalleRepuesto.IdRepuesto);
                    command.Parameters.AddWithValue("@IdServicio", detalleRepuesto.IdServicio);
                    command.Parameters.AddWithValue("@Cantidad", detalleRepuesto.Cantidad);
                    command.Parameters.AddWithValue("@IdDetalleRepuesto", detalleRepuesto.IdDetalleRepuesto);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Baja(int idDetalleRepuesto)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM detalles_repuestos WHERE IdDetalleRepuesto = @IdDetalleRepuesto";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdDetalleRepuesto", idDetalleRepuesto);
                    command.ExecuteNonQuery();
                }
            }
        }

        public (IList<DetalleRepuesto> lista, int total) ObtenerPaginado(int pagina, int tamanioPagina)
        {
            IList<DetalleRepuesto> lista = new List<DetalleRepuesto>();
            int total = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Obtener el total de registros
                string countSql = "SELECT COUNT(*) FROM detalles_repuestos";
                using (MySqlCommand countCommand = new MySqlCommand(countSql, connection))
                {
                    total = Convert.ToInt32(countCommand.ExecuteScalar());
                }

                // Obtener los registros paginados
                string sql = "SELECT * FROM detalles_repuestos LIMIT @Offset, @TamanioPagina";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamanioPagina);
                    command.Parameters.AddWithValue("@TamanioPagina", tamanioPagina);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DetalleRepuesto detalleRepuesto = new DetalleRepuesto
                            {
                                IdDetalleRepuesto = reader.GetInt32("IdDetalleRepuesto"),
                                IdRepuesto = reader.GetInt32("IdRepuesto"),
                                IdServicio = reader.GetInt32("IdServicio"),
                                Cantidad = reader.GetInt32("Cantidad")
                            };

                            // Cargar entidades asociadas
                            detalleRepuesto.Repuesto = _repuestoRepo.ObtenerPorId(detalleRepuesto.IdRepuesto);
                            detalleRepuesto.Servicio = _servicioRepo.ObtenerPorId(detalleRepuesto.IdServicio);

                            lista.Add(detalleRepuesto);
                        }
                    }
                }
            }
            return (lista, total);
        }

        public decimal CalcularCostoTotal(int idRepuesto, int cantidad)
        {
            Repuesto? repuesto = _repuestoRepo.ObtenerPorId(idRepuesto);
            if (repuesto == null)
            {
                throw new Exception("Repuesto no encontrado");
            }
            return repuesto.CostoRepuestoBase * cantidad;
        }
    }
}