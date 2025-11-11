using MySqlConnector;

namespace vertacnik_TallerMecanico2025.Models
{
    public class RepuestoRepo : RepositorioBase
    {
        public RepuestoRepo(IConfiguration configuration) : base(configuration) { }

        public void Alta(Repuesto repuesto)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO repuestos (Nombre, Descripcion, CostoRepuestoBase, Estado) " +
                             "VALUES (@Nombre, @Descripcion, @CostoRepuestoBase, 1)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", repuesto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                    command.Parameters.AddWithValue("@CostoRepuestoBase", repuesto.CostoRepuestoBase);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IList<Repuesto> ObtenerTodos()
        {
            IList<Repuesto> lista = new List<Repuesto>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM repuestos";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Repuesto repuesto = new Repuesto
                            {
                                IdRepuesto = reader.GetInt32("IdRepuesto"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoRepuestoBase = reader.GetDecimal("CostoRepuestoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(repuesto);
                        }
                    }
                }
            }
            return lista;
        }

        public Repuesto? ObtenerPorId(int idRepuesto)
        {
            Repuesto? repuesto = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM repuestos WHERE IdRepuesto = @IdRepuesto";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdRepuesto", idRepuesto);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            repuesto = new Repuesto
                            {
                                IdRepuesto = reader.GetInt32("IdRepuesto"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoRepuestoBase = reader.GetDecimal("CostoRepuestoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                        }
                    }
                }
            }
            return repuesto;
        }

        public void Modificacion(Repuesto repuesto)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE repuestos SET Nombre = @Nombre, Descripcion = @Descripcion, " +
                             "CostoRepuestoBase = @CostoRepuestoBase WHERE IdRepuesto = @IdRepuesto";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", repuesto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", repuesto.Descripcion);
                    command.Parameters.AddWithValue("@CostoRepuestoBase", repuesto.CostoRepuestoBase);
                    command.Parameters.AddWithValue("@IdRepuesto", repuesto.IdRepuesto);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void CambiarEstado(int idRepuesto, bool nuevoEstado)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE repuestos SET Estado = @Estado WHERE IdRepuesto = @IdRepuesto";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@IdRepuesto", idRepuesto);
                    command.ExecuteNonQuery();
                }
            }
        }

        public (IList<Repuesto> lista, int total) ObtenerPaginado(int pagina, int tamanioPagina)
        {
            IList<Repuesto> lista = new List<Repuesto>();
            int total = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Obtener el total de registros
                string countSql = "SELECT COUNT(*) FROM repuestos";
                using (MySqlCommand countCommand = new MySqlCommand(countSql, connection))
                {
                    total = Convert.ToInt32(countCommand.ExecuteScalar());
                }

                // Obtener los registros paginados
                string sql = "SELECT * FROM repuestos LIMIT @Offset, @TamanioPagina";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    int offset = (pagina - 1) * tamanioPagina;
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@TamanioPagina", tamanioPagina);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Repuesto repuesto = new Repuesto
                            {
                                IdRepuesto = reader.GetInt32("IdRepuesto"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoRepuestoBase = reader.GetDecimal("CostoRepuestoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(repuesto);
                        }
                    }
                }
            }
            return (lista, total);
        }

        public IList<Repuesto> BuscarPorNombre(string nombreParcial)
        {
            IList<Repuesto> lista = new List<Repuesto>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM repuestos WHERE Nombre LIKE @NombreParcial";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NombreParcial", "%" + nombreParcial + "%");
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Repuesto repuesto = new Repuesto
                            {
                                IdRepuesto = reader.GetInt32("IdRepuesto"),
                                Nombre = reader.GetString("Nombre"),
                                Descripcion = reader.GetString("Descripcion"),
                                CostoRepuestoBase = reader.GetDecimal("CostoRepuestoBase"),
                                Estado = reader.GetBoolean("Estado")
                            };
                            lista.Add(repuesto);
                        }
                    }
                }
            }
            return lista;
        }
    }
}