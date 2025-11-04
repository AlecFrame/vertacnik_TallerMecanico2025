using MySqlConnector;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace vertacnik_TallerMecanico2025.Models
{
    public class UsuarioRepo : RepositorioBase
    {
        public UsuarioRepo(IConfiguration configuration) : base(configuration) { }
        public Usuario? ObtenerPorId(int idUsuario)
        {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Usuarios WHERE IdUsuario = @IdUsuario";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Dni = reader["Dni"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                ClaveHash = reader["ClaveHash"].ToString(),
                                Rol = (RolUsuario)Enum.Parse(typeof(RolUsuario), reader["Rol"].ToString()!),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Avatar = reader["Avatar"] != DBNull.Value ? reader["Avatar"].ToString() : null
                            };
                        }
                    }
                }
                connection.Close();
            }

            return usuario;
        }
        public Usuario? ObtenerPorDni(string dni)
        {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Usuarios WHERE Dni = @Dni";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Dni", dni);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Dni = reader["Dni"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                ClaveHash = reader["ClaveHash"].ToString(),
                                Rol = (RolUsuario)Enum.Parse(typeof(RolUsuario), reader["Rol"].ToString()!),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Avatar = reader["Avatar"] != DBNull.Value ? reader["Avatar"].ToString() : null
                            };
                        }
                    }
                }
                connection.Close();
            }

            return usuario;
        }
        public void Alta(Usuario usuario)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Usuarios (Dni, Nombre, Apellido, Email, Telefono, ClaveHash, Rol, Estado, Avatar)
                              VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono, @ClaveHash, @Rol, 1, @Avatar)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Dni", usuario.Dni);
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    command.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol.ToString());
                    command.Parameters.AddWithValue("@Avatar", (object?)usuario.Avatar ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void Alta(Usuario usuario, string clavePlana)
        {
            // Generar el hash de la clave
            usuario.ClaveHash = HashClave(clavePlana);

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Usuarios (Dni, Nombre, Apellido, Email, Telefono, ClaveHash, Rol, Estado, Avatar)
                              VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono, @ClaveHash, @Rol, 1, @Avatar)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Dni", usuario.Dni);
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    command.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol.ToString());
                    command.Parameters.AddWithValue("@Avatar", (object?)usuario.Avatar ?? DBNull.Value);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public string HashClave(string clavePlana)
        {
            // Generar salt aleatorio de 16 bytes
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Derivar la clave con PBKDF2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: clavePlana,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Guardar salt + hash en el mismo campo separados por '.'
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
        public bool VerificarClave(Usuario usuario, string clavePlana)
        {
            var parts = usuario.ClaveHash.Split('.');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: clavePlana,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed == storedHash;
        }
        public void CambiarEstado(int idUsuario, bool nuevoEstado)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE Usuarios SET Estado = @Estado WHERE IdUsuario = @IdUsuario";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void Modificar(Usuario usuario)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = @"UPDATE Usuarios 
                              SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, 
                                  Telefono = @Telefono, Rol = @Rol, Estado = @Estado, Avatar = @Avatar
                              WHERE IdUsuario = @IdUsuario";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                    command.Parameters.AddWithValue("@Rol", usuario.Rol.ToString());
                    command.Parameters.AddWithValue("@Estado", usuario.Estado);
                    command.Parameters.AddWithValue("@Avatar", (object?)usuario.Avatar ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public IList<Usuario> ObtenerTodos()
        {
            IList<Usuario> usuarios = new List<Usuario>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Usuarios";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Dni = reader["Dni"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                ClaveHash = reader["ClaveHash"].ToString(),
                                Rol = (RolUsuario)Enum.Parse(typeof(RolUsuario), reader["Rol"].ToString()!),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Avatar = reader["Avatar"] != DBNull.Value ? reader["Avatar"].ToString() : null
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
                connection.Close();
            }

            return usuarios;
        }
        public Usuario? AuthenticateUser(string email, string password)
        {
            Usuario? usuario = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Usuarios WHERE Email = @Email";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Dni = reader["Dni"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Apellido = reader["Apellido"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                ClaveHash = reader["ClaveHash"].ToString(),
                                Rol = (RolUsuario)Enum.Parse(typeof(RolUsuario), reader["Rol"].ToString()!),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                Avatar = reader["Avatar"] != DBNull.Value ? reader["Avatar"].ToString() : null
                            };

                            // Verificar la clave
                            if (!VerificarClave(usuario, password))
                            {
                                usuario = null; // Clave incorrecta
                            }
                        }
                    }
                }
                connection.Close();
            }

            return usuario;
        }
    }
}