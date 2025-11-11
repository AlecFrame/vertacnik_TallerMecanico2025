using MySqlConnector;

namespace vertacnik_TallerMecanico2025.Models
{
    public class ServicioRepo : RepositorioBase
    {
        private readonly TipoServicioRepo _tipoServicioRepo;
        private readonly PedidoRepo _pedidoRepo;
        private readonly UsuarioRepo _usuarioRepo;
        public ServicioRepo(IConfiguration configuration) : base(configuration)
        {
            _tipoServicioRepo = new TipoServicioRepo(configuration);
            _pedidoRepo = new PedidoRepo(configuration);
            _usuarioRepo = new UsuarioRepo(configuration);
        }

        public void Alta(Servicio servicio)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO servicios (IdPedido, IdUsuario, IdTipoServicio, Fecha, Descripcion, CostoBase, Estado) " +
                             "VALUES (@IdPedido, @IdUsuario, @IdTipoServicio, @Fecha, @Descripcion, @CostoBase, 1)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPedido", servicio.IdPedido);
                    command.Parameters.AddWithValue("@IdUsuario", servicio.IdUsuario);
                    command.Parameters.AddWithValue("@IdTipoServicio", servicio.IdTipoServicio);
                    command.Parameters.AddWithValue("@Fecha", servicio.Fecha);
                    command.Parameters.AddWithValue("@Descripcion", servicio.Descripcion);
                    command.Parameters.AddWithValue("@CostoBase", servicio.CostoBase);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IList<Servicio> ObtenerTodos()
        {
            IList<Servicio> lista = new List<Servicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM servicios";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Servicio servicio = new Servicio
                            {
                                IdServicio = reader.GetInt32("IdServicio"),
                                IdPedido = reader.GetInt32("IdPedido"),
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoBase = reader.GetDecimal("CostoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };

                            // Cargar entidades relacionadas
                            servicio.TipoServicio = _tipoServicioRepo.ObtenerPorId(servicio.IdTipoServicio);
                            servicio.Pedido = _pedidoRepo.ObtenerPorId(servicio.IdPedido);
                            servicio.Usuario = _usuarioRepo.ObtenerPorId(servicio.IdUsuario);

                            lista.Add(servicio);
                        }
                    }
                }
            }
            return lista;
        }

        public void CambiarEstado(int idServicio, bool nuevoEstado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE servicios SET Estado = @Estado WHERE IdServicio = @IdServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@IdServicio", idServicio);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Servicio? ObtenerPorId(int idServicio)
        {
            Servicio? servicio = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM servicios WHERE IdServicio = @IdServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdServicio", idServicio);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            servicio = new Servicio
                            {
                                IdServicio = reader.GetInt32("IdServicio"),
                                IdPedido = reader.GetInt32("IdPedido"),
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoBase = reader.GetDecimal("CostoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };

                            // Cargar entidades relacionadas
                            servicio.TipoServicio = _tipoServicioRepo.ObtenerPorId(servicio.IdTipoServicio);
                            servicio.Pedido = _pedidoRepo.ObtenerPorId(servicio.IdPedido);
                            servicio.Usuario = _usuarioRepo.ObtenerPorId(servicio.IdUsuario);
                        }
                    }
                }
            }
            return servicio;
        }

        public void Modificacion(Servicio servicio)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE servicios SET IdPedido = @IdPedido, IdUsuario = @IdUsuario, " +
                             "IdTipoServicio = @IdTipoServicio, Fecha = @Fecha, Descripcion = @Descripcion, " +
                             "CostoBase = @CostoBase WHERE IdServicio = @IdServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPedido", servicio.IdPedido);
                    command.Parameters.AddWithValue("@IdUsuario", servicio.IdUsuario);
                    command.Parameters.AddWithValue("@IdTipoServicio", servicio.IdTipoServicio);
                    command.Parameters.AddWithValue("@Fecha", servicio.Fecha);
                    command.Parameters.AddWithValue("@Descripcion", servicio.Descripcion);
                    command.Parameters.AddWithValue("@CostoBase", servicio.CostoBase);
                    command.Parameters.AddWithValue("@IdServicio", servicio.IdServicio);
                    command.ExecuteNonQuery();
                }
            }
        }

        public (IList<Servicio> lista, int total) ObtenerPaginado(int pagina, int tamanioPagina)
        {
            IList<Servicio> lista = new List<Servicio>();
            int total = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Obtener el total de registros
                string countSql = "SELECT COUNT(*) FROM servicios";
                using (MySqlCommand countCommand = new MySqlCommand(countSql, connection))
                {
                    total = Convert.ToInt32(countCommand.ExecuteScalar());
                }

                // Obtener los registros paginados
                string sql = "SELECT * FROM servicios LIMIT @Offset, @TamanioPagina";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamanioPagina);
                    command.Parameters.AddWithValue("@TamanioPagina", tamanioPagina);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Servicio servicio = new Servicio
                            {
                                IdServicio = reader.GetInt32("IdServicio"),
                                IdPedido = reader.GetInt32("IdPedido"),
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoBase = reader.GetDecimal("CostoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };

                            // Cargar entidades relacionadas
                            servicio.TipoServicio = _tipoServicioRepo.ObtenerPorId(servicio.IdTipoServicio);
                            servicio.Pedido = _pedidoRepo.ObtenerPorId(servicio.IdPedido);
                            servicio.Usuario = _usuarioRepo.ObtenerPorId(servicio.IdUsuario);

                            lista.Add(servicio);
                        }
                    }
                }
            }
            return (lista, total);
        }

        public IList<Servicio> ObtenerPorIdPedido(int idPedido)
        {
            IList<Servicio> lista = new List<Servicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM servicios WHERE IdPedido = @IdPedido";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPedido", idPedido);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Servicio servicio = new Servicio
                            {
                                IdServicio = reader.GetInt32("IdServicio"),
                                IdPedido = reader.GetInt32("IdPedido"),
                                IdUsuario = reader.GetInt32("IdUsuario"),
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoBase = reader.GetDecimal("CostoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };

                            // Cargar entidades relacionadas
                            servicio.TipoServicio = _tipoServicioRepo.ObtenerPorId(servicio.IdTipoServicio);
                            servicio.Pedido = _pedidoRepo.ObtenerPorId(servicio.IdPedido);
                            servicio.Usuario = _usuarioRepo.ObtenerPorId(servicio.IdUsuario);

                            lista.Add(servicio);
                        }
                    }
                }
            }
            return lista;
        }

        public void ActualizarCostoBase(int idServicio)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // 1. Obtener el tipo de servicio y su costo base
                var servicio = ObtenerPorId(idServicio);
                if (servicio == null) return;

                decimal costoTipo = servicio.TipoServicio.CostoTipoServicioBase;

                DetalleRepuestoRepo _detalleRepuestoRepo = new DetalleRepuestoRepo(configuration);
                // 2. Calcular el costo de repuestos asociados
                var detalles = _detalleRepuestoRepo.ObtenerPorId(idServicio);
                decimal costoRepuestos = detalles.Sum(d => d.Cantidad * d.Repuesto.CostoRepuestoBase);

                // 3. Calcular total y actualizar en BD
                decimal costoTotal = costoTipo + costoRepuestos;

                string sql = "UPDATE servicios SET CostoBase = @Costo WHERE IdServicio = @IdServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Costo", costoTotal);
                    command.Parameters.AddWithValue("@IdServicio", idServicio);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}