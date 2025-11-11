using MySqlConnector;

namespace vertacnik_TallerMecanico2025.Models
{
    public class VehiculoRepo : RepositorioBase
    {
        private readonly ClienteRepo _clienteRepo;
        public VehiculoRepo(IConfiguration configuration) : base(configuration)
        {
            _clienteRepo = new ClienteRepo(configuration);
        }

        public IList<Vehiculo> ObtenerTodos()
        {
            IList<Vehiculo> lista = new List<Vehiculo>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM vehiculos";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vehiculo vehiculo = new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                IdCliente = reader.GetInt32("IdCliente"),
                                Patente = reader.GetString("Patente"),
                                Marca = reader.GetString("Marca"),
                                Modelo = reader.GetString("Modelo"),
                                Anio = reader.GetInt32("Anio"),
                                Color = reader.GetString("Color"),
                                Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? null : reader.GetString("Foto"),
                                EnElTaller = reader.GetBoolean("EnElTaller")
                            };

                            vehiculo.Cliente = _clienteRepo.ObtenerPorId(vehiculo.IdCliente);
                            
                            lista.Add(vehiculo);
                        }
                    }
                }
            }
            return lista;
        }

        public void Alta(Vehiculo vehiculo)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO vehiculos (IdCliente, Patente, Marca, Modelo, Anio, Color, Foto, EnElTaller) " +
                             "VALUES (@IdCliente, @Patente, @Marca, @Modelo, @Anio, @Color, @Foto, @EnElTaller)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", vehiculo.IdCliente);
                    command.Parameters.AddWithValue("@Patente", vehiculo.Patente);
                    command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
                    command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
                    command.Parameters.AddWithValue("@Anio", vehiculo.Anio);
                    command.Parameters.AddWithValue("@Color", vehiculo.Color);
                    command.Parameters.AddWithValue("@Foto", (object?)vehiculo.Foto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EnElTaller", vehiculo.EnElTaller);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CambiarEstado(int idVehiculo, bool nuevoEstado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE vehiculos SET EnElTaller = @EnElTaller WHERE IdVehiculo = @IdVehiculo";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@EnElTaller", nuevoEstado);
                    command.Parameters.AddWithValue("@IdVehiculo", idVehiculo);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Modificacion(Vehiculo vehiculo)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE vehiculos SET IdCliente = @IdCliente, Patente = @Patente, Marca = @Marca, " +
                             "Modelo = @Modelo, Anio = @Anio, Color = @Color, Foto = @Foto, EnElTaller = @EnElTaller " +
                             "WHERE IdVehiculo = @IdVehiculo";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", vehiculo.IdCliente);
                    command.Parameters.AddWithValue("@Patente", vehiculo.Patente);
                    command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
                    command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
                    command.Parameters.AddWithValue("@Anio", vehiculo.Anio);
                    command.Parameters.AddWithValue("@Color", vehiculo.Color);
                    command.Parameters.AddWithValue("@Foto", (object?)vehiculo.Foto ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EnElTaller", vehiculo.EnElTaller);
                    command.Parameters.AddWithValue("@IdVehiculo", vehiculo.IdVehiculo);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Vehiculo? ObtenerPorId(int idVehiculo)
        {
            Vehiculo? vehiculo = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM vehiculos WHERE IdVehiculo = @IdVehiculo";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdVehiculo", idVehiculo);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vehiculo = new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                IdCliente = reader.GetInt32("IdCliente"),
                                Patente = reader.GetString("Patente"),
                                Marca = reader.GetString("Marca"),
                                Modelo = reader.GetString("Modelo"),
                                Anio = reader.GetInt32("Anio"),
                                Color = reader.GetString("Color"),
                                Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? null : reader.GetString("Foto"),
                                EnElTaller = reader.GetBoolean("EnElTaller")
                            };

                            vehiculo.Cliente = _clienteRepo.ObtenerPorId(vehiculo.IdCliente);
                        }
                    }
                }
            }
            return vehiculo;
        }

        public IList<Vehiculo> ObtenerPorCliente(int idCliente)
        {
            IList<Vehiculo> lista = new List<Vehiculo>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM vehiculos WHERE IdCliente = @IdCliente";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vehiculo vehiculo = new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                IdCliente = reader.GetInt32("IdCliente"),
                                Patente = reader.GetString("Patente"),
                                Marca = reader.GetString("Marca"),
                                Modelo = reader.GetString("Modelo"),
                                Anio = reader.GetInt32("Anio"),
                                Color = reader.GetString("Color"),
                                Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? null : reader.GetString("Foto"),
                                EnElTaller = reader.GetBoolean("EnElTaller")
                            };

                            vehiculo.Cliente = _clienteRepo.ObtenerPorId(vehiculo.IdCliente);

                            lista.Add(vehiculo);
                        }
                    }
                }
            }
            return lista;
        }

        public (IList<Vehiculo> lista, int total) ObtenerPaginado(int pagina, int tamanioPagina)
        {
            IList<Vehiculo> lista = new List<Vehiculo>();
            int total = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Obtener el total de registros
                string countSql = "SELECT COUNT(*) FROM vehiculos";
                using (MySqlCommand countCommand = new MySqlCommand(countSql, connection))
                {
                    total = Convert.ToInt32(countCommand.ExecuteScalar());
                }

                // Obtener los registros paginados
                string sql = "SELECT * FROM vehiculos LIMIT @Offset, @TamanioPagina";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    int offset = (pagina - 1) * tamanioPagina;
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@TamanioPagina", tamanioPagina);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vehiculo vehiculo = new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                IdCliente = reader.GetInt32("IdCliente"),
                                Patente = reader.GetString("Patente"),
                                Marca = reader.GetString("Marca"),
                                Modelo = reader.GetString("Modelo"),
                                Anio = reader.GetInt32("Anio"),
                                Color = reader.GetString("Color"),
                                Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? null : reader.GetString("Foto"),
                                EnElTaller = reader.GetBoolean("EnElTaller")
                            };

                            vehiculo.Cliente = _clienteRepo.ObtenerPorId(vehiculo.IdCliente);

                            lista.Add(vehiculo);
                        }
                    }
                }
            }

            return (lista, total);
        }

        public IList<Vehiculo> BuscarPorPatenteMarcaModeloColorYFiltrarCliente(int idCliente, string query)
        {
            IList<Vehiculo> lista = new List<Vehiculo>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"SELECT * FROM vehiculos
                       WHERE IdCliente = @IdCliente
                       AND (Patente LIKE @Query OR Marca LIKE @Query OR Modelo LIKE @Query OR Color LIKE @Query)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    command.Parameters.AddWithValue("@Query", "%" + query + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vehiculo vehiculo = new Vehiculo
                            {
                                IdVehiculo = reader.GetInt32("IdVehiculo"),
                                IdCliente = reader.GetInt32("IdCliente"),
                                Patente = reader.GetString("Patente"),
                                Marca = reader.GetString("Marca"),
                                Modelo = reader.GetString("Modelo"),
                                Anio = reader.GetInt32("Anio"),
                                Color = reader.GetString("Color"),
                                Foto = reader.IsDBNull(reader.GetOrdinal("Foto")) ? null : reader.GetString("Foto"),
                                EnElTaller = reader.GetBoolean("EnElTaller")
                            };

                            //vehiculo.Cliente = _clienteRepo.ObtenerPorId(vehiculo.IdCliente);

                            lista.Add(vehiculo);
                        }
                    }
                }
            }
            return lista;
        }
    }
}