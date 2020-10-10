using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace Fondos_Antiguos.Models
{
    public class FaIdentityUser : IdentityUser, IUser
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        public FaIdentityUser() : this(null)
        {
            
        }

        /// <summary>
        /// Constructor that takes user name as argument
        /// </summary>
        /// <param name="userName"></param>
        public FaIdentityUser(string userName)
            : base(userName)
        {
            this.UserName = userName;
            this.FechaIngreso = DateTime.Now;
        }

        /// <summary>
        /// User ID, in the database is the IdUsuario
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Auto Increment ID column
        /// </summary>
        public long IdUsuario { get; set; }

        /// <summary>
        /// Roles's column IDRol
        /// </summary>
        public string IdRol { get; set; }

        /// <summary>
        /// Columna FechaIngreso
        /// </summary>
        public DateTime FechaIngreso { get; set; }

        /// <summary>
        /// Columna ReqCambioCntrña
        /// </summary>
        public bool ReqCambioContraseña { get; set; }
    }
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : FaIdentityUser
    {
        public ApplicationUser() : base()
        {

        }
        public ApplicationUser(string userName) : base(userName)
        {
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class IdentityRolPermit
    {
        public long ID { get; set; }
        /// <summary>
        /// Direccion de View
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// ID tipo string para identificar roles en ASP.Identity
        /// </summary>
        public string IdRol { get; set; }

        /// <summary>
        /// 0 = NA, 1 = Todas permitidas, 2 = Ninguna Permitida
        /// </summary>
        public byte TodasLasVistas { get; set; }

        public void Fill(IDataReader row)
        {
            this.ID = Convert.IsDBNull(row["ID"]) ? 0 : Convert.ToInt64(row["ID"]);
            this.ViewPath = Convert.IsDBNull(row["View"]) ? null : ViewUtil.ObtenerNombreDeView(row["View"].ToString());
            this.IdRol = Convert.IsDBNull(row["IdRol"]) ? null : row["IdRol"].ToString();
            this.TodasLasVistas = Convert.IsDBNull(row["All"]) ? (byte)0 : Convert.ToByte(row["All"]);
        }
    }

    public class FaApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public FaApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            
        }

        public FaApplicationDbContext(string nameOrConnectionString) : base(nameOrConnectionString, false)
        {

        }

        static FaApplicationDbContext()
        {
            Database.SetInitializer(new MySqlInitializer());
        }

        public static FaApplicationDbContext Create()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            builder.Database = System.Configuration.ConfigurationManager.AppSettings["Database"];
            
            return new FaApplicationDbContext(builder.ToString());
        }
    }

    public class MySqlInitializer : IDatabaseInitializer<FaApplicationDbContext>
    {
        public void InitializeDatabase(FaApplicationDbContext context)
        {
#if DEBUG
            if (!context.Database.Exists())
            {
                // if database did not exist before - create it
                context.Database.Create();
            }
            else
            {
                if (context.Database.Connection is MySqlConnection _mysqlConn)
                {
                    if (_mysqlConn.State != System.Data.ConnectionState.Open)
                        _mysqlConn.Open();
                    _mysqlConn.ChangeDatabase(System.Configuration.ConfigurationManager.AppSettings["Database"].ToString());
                }
                // query to check if MigrationHistory table is present in the database 
                var migrationHistoryTableExists = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<int>(
                  $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{System.Configuration.ConfigurationManager.AppSettings["Database"]}' AND table_name = '__MigrationHistory'");

                // if MigrationHistory table is not there (which is the case first time we run) - create it
                if (migrationHistoryTableExists.FirstOrDefault() == 0)
                {
                    //context.Database.Delete();
                    context.Database.Create();
                }
            }
#endif
        }
    }
}