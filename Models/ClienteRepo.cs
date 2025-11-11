using MySqlConnector;

namespace vertacnik_TallerMecanico2025.Models
{
    public class ClienteRepo : RepositorioBase
    {
        public ClienteRepo(IConfiguration configuration) : base(configuration) { }

        public IList<Cliente> ObtenerTodos()
        {
            IList<Cliente> lista = new List<Cliente>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM clientes";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente
                            {
                                IdCliente = reader.GetInt32("IdCliente"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(cliente);
                        }
                    }
                }
            }
            return lista;
        }

        public void Alta(Cliente cliente)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO clientes (Dni, Nombre, Apellido, Email, Telefono, Estado) " +
                             "VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono, 1)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Dni", cliente.Dni);
                    command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                    command.Parameters.AddWithValue("@Email", cliente.Email);
                    command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CambiarEstado(int idCliente, bool nuevoEstado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE clientes SET Estado = @Estado WHERE IdCliente = @IdCliente";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Modificacion(Cliente cliente)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE clientes SET Dni = @Dni, Nombre = @Nombre, Apellido = @Apellido, " +
                             "Telefono = @Telefono, Email = @Email WHERE IdCliente = @IdCliente";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Dni", cliente.Dni);
                    command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                    command.Parameters.AddWithValue("@Email", cliente.Email);
                    command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    command.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Cliente? ObtenerPorId(int idCliente)
        {
            Cliente? cliente = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT IdCliente, Dni, Nombre, Apellido, Email, Telefono, Estado " +
                             "FROM clientes WHERE IdCliente = @IdCliente";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", idCliente);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente
                            {
                                IdCliente = reader.GetInt32("IdCliente"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),
                                Estado = reader.GetBoolean("Estado")
                            };
                        }
                    }
                }
            }
            return cliente;
        }

        public (IList<Cliente> lista, int total) ObtenerPaginado(int pagina, int tamanioPagina)
        {
            IList<Cliente> lista = new List<Cliente>();
            int total = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // 1 Contar total de registros
                string countSql = "SELECT COUNT(*) FROM clientes";
                using (MySqlCommand countCmd = new MySqlCommand(countSql, connection))
                {
                    total = Convert.ToInt32(countCmd.ExecuteScalar());
                }

                // 2 Traer sólo los de la página actual
                int offset = (pagina - 1) * tamanioPagina;
                string sql = "SELECT * FROM clientes ORDER BY IdCliente ASC LIMIT @Limit OFFSET @Offset";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Limit", tamanioPagina);
                    command.Parameters.AddWithValue("@Offset", offset);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente
                            {
                                IdCliente = reader.GetInt32("IdCliente"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(cliente);
                        }
                    }
                }
            }

            return (lista, total);
        }

        public IList<Cliente> BuscarPorNombreODni(string query)
        {
            IList<Cliente> lista = new List<Cliente>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM clientes " +
                             "WHERE Nombre LIKE @Query OR Apellido LIKE @Query OR Dni LIKE @Query";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Query", $"%{query}%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente
                            {
                                IdCliente = reader.GetInt32("IdCliente"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(cliente);
                        }
                    }
                }
            }
            return lista;
        }

    }
}