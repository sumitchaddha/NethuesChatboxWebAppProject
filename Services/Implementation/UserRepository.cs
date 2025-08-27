using Microsoft.Extensions.Configuration;
using Nethues_ChatboxWebApp.Models;
using Nethues_ChatboxWebApp.Services.Interface;
using System.Data.SQLite;

namespace Nethues_ChatboxWebApp.Services.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default")!;
        }

        public void CreateTableIfNotExists()
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            string sql = @"CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        PasswordHash TEXT NOT NULL,
                        CreatedUtc TEXT NOT NULL,
                        IsActive INTEGER NOT NULL
                       );

                       CREATE TABLE IF NOT EXISTS UserActions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        Action TEXT NOT NULL,
                        TimestampUtc TEXT NOT NULL,
                        FOREIGN KEY(UserId) REFERENCES Users(Id)
                       );";

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        public User? GetByUsername(string username)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            string sql = "SELECT * FROM Users WHERE Username = @u LIMIT 1;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString()!,
                    PasswordHash = reader["PasswordHash"].ToString()!,
                    CreatedUtc = DateTime.Parse(reader["CreatedUtc"].ToString()!),
                    IsActive = Convert.ToInt32(reader["IsActive"]) == 1,
                    FirstName = reader["FirstName"].ToString()!,
                    LastName = reader["LastName"].ToString()!,
                };
            }
            return null;
        }

        //public void Add(User user)
        //{
        //    using var conn = new SQLiteConnection(_connectionString);
        //    conn.Open();

        //    string sql = @"INSERT INTO Users (Username, PasswordHash, CreatedUtc, IsActive)
        //               VALUES (@u, @p, @c, @a);";

        //    using var cmd = new SQLiteCommand(sql, conn);
        //    cmd.Parameters.AddWithValue("@u", user.Username);
        //    cmd.Parameters.AddWithValue("@p", user.PasswordHash);
        //    cmd.Parameters.AddWithValue("@c", user.CreatedUtc.ToString("o")); // ISO 8601
        //    cmd.Parameters.AddWithValue("@a", user.IsActive ? 1 : 0);
        //    cmd.ExecuteNonQuery();
        //}

        public void LogAction(int userId, string action,string result)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            string sql = @"INSERT INTO UserActions (UserId, Action, TimestampUtc, Result)
                       VALUES (@id, @act, @ts, @r);";

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.Parameters.AddWithValue("@act", action);
            cmd.Parameters.AddWithValue("@r", result);
            cmd.Parameters.AddWithValue("@ts", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public List<object> GetUserActionsHistory(int userId)
        {
            var actions = new List<object>();

            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            using var cmd = new SQLiteCommand("SELECT Action, Result, TimestampUTC FROM UserActions WHERE UserId = @uid ORDER BY TimestampUTC DESC", conn);
            cmd.Parameters.AddWithValue("@uid", userId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                actions.Add(new
                {
                    expression = reader.GetString(0),
                    result = reader.GetString(1),
                    createdAt = reader.GetDateTime(2)
                });
            }

            return actions;
        }

        public User CreateUser(string username, string password, string FirstName, string LastName)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            var cmd = new SQLiteCommand("INSERT INTO Users (Username, Password, DatetimeUTC, IsActive, FirstName, LastName) VALUES (@u, @p, @c, 1, @f,@l); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", hash);
            cmd.Parameters.AddWithValue("@c", DateTime.UtcNow.ToString("o"));
            cmd.Parameters.AddWithValue("@f", FirstName);
            cmd.Parameters.AddWithValue("@l", LastName);
            var id = Convert.ToInt32(cmd.ExecuteScalar());

            return new User { Id = id, Username = username, PasswordHash = hash, CreatedUtc = DateTime.UtcNow, IsActive = true,FirstName = FirstName , LastName = LastName
            };
        }

        public User? ValidateUser(string username, string password)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Username = @u AND IsActive = 1;", conn);
            cmd.Parameters.AddWithValue("@u", username);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            var hash = reader.GetString(2);
            if (!BCrypt.Net.BCrypt.Verify(password, hash)) return null;

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = hash,
                CreatedUtc = DateTime.Parse(reader.GetString(3)),
                IsActive = reader.GetInt32(4) == 1,
                FirstName = reader["FirstName"].ToString()!,
                LastName = reader["LastName"].ToString()!,
            };
        }

        public void SaveRefreshToken(RefreshToken token)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();

            var cmd = new SQLiteCommand("INSERT INTO RefreshTokens (UserId, Token, ExpiresUtc, IsRevoked) VALUES (@u, @t, @e, 0);", conn);
            cmd.Parameters.AddWithValue("@u", token.UserId);
            cmd.Parameters.AddWithValue("@t", token.Token);
            cmd.Parameters.AddWithValue("@e", token.ExpiresUtc.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public User? GetUserById(int userId)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", userId);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    FirstName = reader.GetString(5),
                    LastName = reader.GetString(6)
                };
            }
            return null;
        }

        public RefreshToken? GetRefreshToken(string token)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, UserId, Token, ExpiresUtc, IsRevoked FROM RefreshTokens WHERE Token = @token", conn);
            cmd.Parameters.AddWithValue("@token", token);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new RefreshToken
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Token = reader.GetString(2),
                    ExpiresUtc = reader.GetDateTime(3),
                    IsRevoked = reader.GetBoolean(4)
                };
            }
            return null;
        }

        public void UpdateRefreshToken(int userId)
        {
            using var conn = new SQLiteConnection(_connectionString);
             conn.OpenAsync();

            using var cmd = new SQLiteCommand("UPDATE RefreshTokens SET IsRevoked = 1 WHERE UserId = @id" , conn);
            cmd.Parameters.AddWithValue("@id", userId);
             cmd.ExecuteNonQueryAsync();
        }

        public void UpdatePassword(int userId, string newPasswordHash)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(newPasswordHash);
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Users SET Password = @hash WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }
    }
}
