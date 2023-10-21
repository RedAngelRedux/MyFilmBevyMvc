using Npgsql;

namespace MyFilmBevy.Services
{
    public class ConnectionService
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            string? connectionString;
            connectionString = (string.IsNullOrEmpty(databaseUrl)) 
                ? configuration.GetConnectionString("DefaultConnection")
                : BuildConnectionString(databaseUrl);
            return connectionString;
            //var connectionString = configuration.GetConnectionString("Heroku-Postgres");
            //return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        { 
            //var databaseUri = new Uri("postgres://wxqtzjtswqutka:d03a1168534e58124f8fd021e5112f9124dfefa24bfe8ea8cd9646aea29d4ea6@ec2-54-234-13-16.compute-1.amazonaws.com:5432/dbflefti0ucs4n\r\n");
            //var databaseUri = new Uri("postgres://wxqtzjtswqutka:d03a1168534e58124f8fd021e5112f9124dfefa24bfe8ea8cd9646aea29d4ea6@ec2-54-234-13-16.compute-1.amazonaws.com:5432/dbflefti0ucs4n");
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }
    }
}
