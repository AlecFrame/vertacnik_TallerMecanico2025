using MySqlConnector;

namespace vertacnik_TallerMecanico2025.Models
{
    public class PedidoRepo : RepositorioBase
    {

        public readonly UsuarioRepo _usuarioRepo;
        public readonly VehiculoRepo _vehiculoRepo;
        public PedidoRepo(IConfiguration configuration) : base(configuration)
        {
            _usuarioRepo = new UsuarioRepo(configuration);
            _vehiculoRepo = new VehiculoRepo(configuration);
        }

        public void Alta(Pedido pedido)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO pedidos (IdVehiculo, IdUsuario, ObservacionCliente, FechaIngreso, CostoEstimado, Estado) " +
                             "VALUES (@IdVehiculo, @IdUsuario, @ObservacionCliente, @FechaIngreso, 0, @Estado)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdVehiculo", pedido.IdVehiculo);
                    command.Parameters.AddWithValue("@IdUsuario", pedido.IdUsuario);
                    command.Parameters.AddWithValue("@ObservacionCliente", pedido.ObservacionCliente);
                    command.Parameters.AddWithValue("@FechaIngreso", pedido.FechaIngreso);
                    command.Parameters.AddWithValue("@Estado", EstadoPedido.Pendiente);
                    command.ExecuteNonQuery();
                }
            }
        }

        private EstadoPedido GetEstadoPedido(MySqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(ordinal))
                return EstadoPedido.Pendiente;

            try
            {
                // Intentar leer como Int32 primero
                return (EstadoPedido)reader.GetInt32(ordinal);
            }
            catch
            {
                // Si falla, intentar leer como string y hacer parsing
                try
                {
                    string estado = reader.GetString(ordinal);
                    if (Enum.TryParse<EstadoPedido>(estado, true, out var result))
                        return result;
                }
                catch { }
                
                return EstadoPedido.Pendiente; // valor por defecto
            }
        }

        public IList<Pedido> ObtenerTodos()
        {
            IList<Pedido> lista = new List<Pedido>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM pedidos";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pedido pedido = new Pedido
                            {
                                IdPedido = reader.GetInt32("IdPedido"),
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                ObservacionCliente = reader.GetString("ObservacionCliente"),
                                FechaIngreso = reader.GetDateTime("FechaIngreso"),
                                FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion")) ? null : reader.GetDateTime("FechaFinalizacion"),
                                ObservacionFinal = reader.IsDBNull(reader.GetOrdinal("ObservacionFinal")) ? null : reader.GetString("ObservacionFinal"),
                                CostoEstimado = reader.GetDecimal("CostoEstimado"),
                                CostoFinal = reader.IsDBNull(reader.GetOrdinal("CostoFinal")) ? null : reader.GetDecimal("CostoFinal"),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("FechaPago")) ? null : reader.GetDateTime("FechaPago"),
                                Estado = GetEstadoPedido(reader, "Estado")
                            };

                            // Cargar Usuario y Vehículo relacionados
                            pedido.Usuario = _usuarioRepo.ObtenerPorId(pedido.IdUsuario);
                            pedido.Vehiculo = _vehiculoRepo.ObtenerPorId(pedido.IdVehiculo);

                            lista.Add(pedido);
                        }
                    }
                }
            }
            return lista;
        }


        public Pedido? ObtenerPorId(int idPedido)
        {
            Pedido? pedido = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM pedidos WHERE IdPedido = @IdPedido";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPedido", idPedido);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pedido = new Pedido
                            {
                                IdPedido = reader.GetInt32("IdPedido"),
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                ObservacionCliente = reader.GetString("ObservacionCliente"),
                                FechaIngreso = reader.GetDateTime("FechaIngreso"),
                                FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion")) ? null : reader.GetDateTime("FechaFinalizacion"),
                                ObservacionFinal = reader.IsDBNull(reader.GetOrdinal("ObservacionFinal")) ? null : reader.GetString("ObservacionFinal"),
                                CostoEstimado = reader.GetDecimal("CostoEstimado"),
                                CostoFinal = reader.IsDBNull(reader.GetOrdinal("CostoFinal")) ? null : reader.GetDecimal("CostoFinal"),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("FechaPago")) ? null : reader.GetDateTime("FechaPago"),
                                Estado = GetEstadoPedido(reader, "Estado")
                            };

                            // Cargar Usuario y Vehículo relacionados
                            pedido.Usuario = _usuarioRepo.ObtenerPorId(pedido.IdUsuario);
                            pedido.Vehiculo = _vehiculoRepo.ObtenerPorId(pedido.IdVehiculo);
                        }
                    }
                }
            }
            return pedido;
        }

        public void Modificacion(Pedido pedido)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE pedidos SET IdVehiculo = @IdVehiculo, IdUsuario = @IdUsuario, " +
                             "ObservacionCliente = @ObservacionCliente, FechaIngreso = @FechaIngreso, " +
                             "FechaFinalizacion = @FechaFinalizacion, ObservacionFinal = @ObservacionFinal, " +
                             "CostoEstimado = @CostoEstimado, CostoFinal = @CostoFinal, FechaPago = @FechaPago, " +
                             "Estado = @Estado WHERE IdPedido = @IdPedido";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdVehiculo", pedido.IdVehiculo);
                    command.Parameters.AddWithValue("@IdUsuario", pedido.IdUsuario);
                    command.Parameters.AddWithValue("@ObservacionCliente", pedido.ObservacionCliente);
                    command.Parameters.AddWithValue("@FechaIngreso", pedido.FechaIngreso);
                    command.Parameters.AddWithValue("@FechaFinalizacion", (object?)pedido.FechaFinalizacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ObservacionFinal", (object?)pedido.ObservacionFinal ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CostoEstimado", pedido.CostoEstimado);
                    command.Parameters.AddWithValue("@CostoFinal", (object?)pedido.CostoFinal ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaPago", (object?)pedido.FechaPago ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", pedido.Estado);
                    command.Parameters.AddWithValue("@IdPedido", pedido.IdPedido);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CambiarEstado(int idPedido, EstadoPedido nuevoEstado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE pedidos SET Estado = @Estado WHERE IdPedido = @IdPedido";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@IdPedido", idPedido);
                    command.ExecuteNonQuery();
                }
            }
        }

        public (IList<Pedido> lista, int total) ObtenerPaginado(int pagina, int tamanioPagina)
        {
            IList<Pedido> lista = new List<Pedido>();
            int total = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Obtener el total de registros
                string countSql = "SELECT COUNT(*) FROM pedidos";
                using (MySqlCommand countCommand = new MySqlCommand(countSql, connection))
                {
                    total = Convert.ToInt32(countCommand.ExecuteScalar());
                }

                // Obtener los registros paginados
                string sql = "SELECT * FROM pedidos ORDER BY IdPedido LIMIT @Limit OFFSET @Offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Limit", tamanioPagina);
                    command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamanioPagina);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pedido pedido = new Pedido
                            {
                                IdPedido = reader.GetInt32("IdPedido"),
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                ObservacionCliente = reader.GetString("ObservacionCliente"),
                                FechaIngreso = reader.GetDateTime("FechaIngreso"),
                                FechaFinalizacion = reader.IsDBNull(reader.GetOrdinal("FechaFinalizacion")) ? null : reader.GetDateTime("FechaFinalizacion"),
                                ObservacionFinal = reader.IsDBNull(reader.GetOrdinal("ObservacionFinal")) ? null : reader.GetString("ObservacionFinal"),
                                CostoEstimado = reader.GetDecimal("CostoEstimado"),
                                CostoFinal = reader.IsDBNull(reader.GetOrdinal("CostoFinal")) ? null : reader.GetDecimal("CostoFinal"),
                                FechaPago = reader.IsDBNull(reader.GetOrdinal("FechaPago")) ? null : reader.GetDateTime("FechaPago"),
                                Estado = GetEstadoPedido(reader, "Estado")
                            };

                            // Cargar Usuario y Vehículo relacionados
                            pedido.Usuario = _usuarioRepo.ObtenerPorId(pedido.IdUsuario);
                            pedido.Vehiculo = _vehiculoRepo.ObtenerPorId(pedido.IdVehiculo);

                            lista.Add(pedido);
                        }
                    }
                }
            }
            return (lista, total);
        }

        public void ActualizarCostoEstimado(int idPedido)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                ServicioRepo _servicioRepo = new ServicioRepo(configuration);
                // 1. Obtener todos los servicios del pedido
                var servicios = _servicioRepo.ObtenerPorIdPedido(idPedido);

                // 2. Sumar sus costos
                decimal costoTotal = servicios.Sum(s => s.CostoBase);

                // 3. Actualizar el pedido
                string sql = "UPDATE pedidos SET CostoEstimado = @Costo WHERE IdPedido = @IdPedido";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Costo", costoTotal);
                    command.Parameters.AddWithValue("@IdPedido", idPedido);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}