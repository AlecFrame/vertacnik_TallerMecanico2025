using MySqlConnector;

namespace vertacnik_TallerMecanico2025.Models
{
    public class TipoServicioRepo : RepositorioBase
    {
        public TipoServicioRepo(IConfiguration configuration) : base(configuration) { }

        public void Alta(TipoServicio tipoServicio)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO tiposervicios (Nombre, Descripcion, CostoTipoServicioBase, Estado) " +
                             "VALUES (@Nombre, @Descripcion, @CostoTipoServicioBase, 1)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", tipoServicio.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", tipoServicio.Descripcion);
                    command.Parameters.AddWithValue("@CostoTipoServicioBase", tipoServicio.CostoTipoServicioBase);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IList<TipoServicio> ObtenerTodos()
        {
            IList<TipoServicio> lista = new List<TipoServicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM tiposervicios";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TipoServicio tipoServicio = new TipoServicio
                            {
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoTipoServicioBase = reader.GetDecimal("CostoTipoServicioBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(tipoServicio);
                        }
                    }
                }
            }
            return lista;
        }

        public void CambiarEstado(int idTipoServicio, bool nuevoEstado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE tiposervicios SET Estado = @Estado WHERE IdTipoServicio = @IdTipoServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@IdTipoServicio", idTipoServicio);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Modificacion(TipoServicio tipoServicio)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE tiposervicios SET Nombre = @Nombre, Descripcion = @Descripcion, " +
                             "CostoTipoServicioBase = @CostoTipoServicioBase " +
                             "WHERE IdTipoServicio = @IdTipoServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", tipoServicio.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", tipoServicio.Descripcion);
                    command.Parameters.AddWithValue("@CostoTipoServicioBase", tipoServicio.CostoTipoServicioBase);
                    command.Parameters.AddWithValue("@IdTipoServicio", tipoServicio.IdTipoServicio);
                    command.ExecuteNonQuery();
                }
            }
        }

        public TipoServicio? ObtenerPorId(int idTipoServicio)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM tiposervicios WHERE IdTipoServicio = @IdTipoServicio";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdTipoServicio", idTipoServicio);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TipoServicio tipoServicio = new TipoServicio
                            {
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoTipoServicioBase = reader.GetDecimal("CostoTipoServicioBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            return tipoServicio;
                        }
                    }
                }
            }
            return null;
        }

        public (IList<TipoServicio> lista, int total) ObtenerPaginado(int pagina, int tamanioPagina)
        {
            IList<TipoServicio> lista = new List<TipoServicio>();
            int total = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Obtener el total de registros
                string countSql = "SELECT COUNT(*) FROM tiposervicios";
                using (MySqlCommand countCommand = new MySqlCommand(countSql, connection))
                {
                    total = Convert.ToInt32(countCommand.ExecuteScalar());
                }

                // Obtener los registros paginados
                string sql = "SELECT * FROM tiposervicios LIMIT @Offset, @TamanioPagina";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    int offset = (pagina - 1) * tamanioPagina;
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@TamanioPagina", tamanioPagina);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TipoServicio tipoServicio = new TipoServicio
                            {
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoTipoServicioBase = reader.GetDecimal("CostoTipoServicioBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(tipoServicio);
                        }
                    }
                }
            }
            return (lista, total);
        }

        public IList<TipoServicio> BuscarPorNombreYDescripcion(string query)
        {
            IList<TipoServicio> lista = new List<TipoServicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM tiposervicios WHERE Estado = 1 AND (Nombre LIKE @Query OR Descripcion LIKE @Query)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Query", "%" + query + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TipoServicio tipoServicio = new TipoServicio
                            {
                                IdTipoServicio = reader.GetInt32("IdTipoServicio"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoTipoServicioBase = reader.GetDecimal("CostoTipoServicioBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(tipoServicio);
                        }
                    }
                }
            }
            return lista;
        }
    }
}