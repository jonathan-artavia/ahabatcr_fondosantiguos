using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fondos_Antiguos
{
    public class FaIdentityOptions
    {
        public string Server { get; set; }
        public uint Port { get; set; }

        public string UserID { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public string GetConnectionString()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(); ;
            builder.Server = this.Server;
            builder.Port = this.Port;
            builder.SslMode = MySqlSslMode.Preferred;
            builder.UserID = this.UserID;
            builder.Password = this.Password;
            builder.Database = this.Database;
            builder.ConvertZeroDateTime = true;
            return builder.ToString();
        }
    }
}
