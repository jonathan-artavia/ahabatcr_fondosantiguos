using Fondos_Antiguos.Localization;
using Fondos_Antiguos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Fondos_Antiguos
{
    internal sealed class DataConnection
    {
        #region Singleton
        private static readonly Lazy<DataConnection> lazy =
            new Lazy<DataConnection>
                (() => new DataConnection());

        public static DataConnection Instance { get { return lazy.Value; } }

        private DataConnection()
        {
            this._connectionCache = new MemoryCache(new MemoryCacheOptions() { CompactionPercentage = 0.8 });
        }
        #endregion

        #region Fields
        FaIdentityOptions _options;
        MemoryCache _connectionCache;
        ApplicationUser anonUser { get; set; }
        #endregion

        #region Public
        public void SetOptions(FaIdentityOptions options)
        {
            this._options = options;
            anonUser = new ApplicationUser("anon");
            anonUser.PasswordHash = DataSecurity.Hash("anon");
            anonUser.Id = "BE75D823412D4DA9AEC6236C6CD73BF8";
            anonUser.IdUsuario = 1;
        }

        #region ExecuteScalar methods
        public object ExecuteScalar(string text, Dictionary<string, object> parameters, HttpContextBase httpContext)
            => this.ExecuteScalar(text, parameters?.Select(s => new MySqlParameter(s.Key, s.Value)), httpContext);

        public object ExecuteScalar(string text, Dictionary<string, object> parameters, IUser user)
            => this.ExecuteScalar(text, parameters?.Select(s => new MySqlParameter(s.Key, s.Value)), user);

        public object ExecuteScalar(string text, IEnumerable<MySqlParameter> parameters, HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            MySqlConnection connection = _connectionCache.GetOrCreate((httpContext?.User?.Identity.GetUserId()) ?? this.anonUser.Id, new Func<ICacheEntry, MySqlConnection>((x) =>
            {
                return new MySqlConnection(this._options.GetConnectionString());
            }));

            connection.Open();

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = System.Data.CommandType.Text;
            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            object result = cmd.ExecuteScalar();
            connection.Close();

            return result;
        }

        public object ExecuteScalar(string text, IEnumerable<MySqlParameter> parameters, IUser user)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            MySqlConnection connection = _connectionCache.GetOrCreate((user?.Id) ?? this.anonUser.Id, new Func<ICacheEntry, MySqlConnection>((x) =>
            {
                return new MySqlConnection(this._options.GetConnectionString());
            }));

            connection.Open();

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = System.Data.CommandType.Text;
            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            object result = cmd.ExecuteScalar();
            connection.Close();

            return result;
        }
        #endregion

        #region ExecuteNonQuery methods
        public int ExecuteNonQuery(string text, Dictionary<string, object> parameters, HttpContextBase httpContext)
            => this.ExecuteNonQuery(text, parameters.Select(s => new MySqlParameter(s.Key, s.Value)), httpContext);

        public int ExecuteNonQuery(string text, Dictionary<string, object> parameters, IUser user)
            => this.ExecuteNonQuery(text, parameters.Select(s => new MySqlParameter(s.Key, s.Value)), user);

        public int ExecuteNonQuery(string text, IEnumerable<MySqlParameter> parameters, IUser user)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            MySqlConnection connection = _connectionCache.GetOrCreate((user?.Id) ?? this.anonUser.Id, new Func<ICacheEntry, MySqlConnection>((x) =>
            {
                return new MySqlConnection(this._options.GetConnectionString());
            }));

            connection.Open();

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = System.Data.CommandType.Text;
            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            int result = cmd.ExecuteNonQuery();
            connection.Close();

            return result;
        }

        public int ExecuteNonQuery(string text, IEnumerable<MySqlParameter> parameters, HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            MySqlConnection connection = _connectionCache.GetOrCreate((httpContext?.User?.Identity.GetUserId()) ?? this.anonUser.Id, new Func<ICacheEntry, MySqlConnection>((x) =>
            {
                return new MySqlConnection(this._options.GetConnectionString());
            }));

            connection.Open();

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = text;
            cmd.CommandType = System.Data.CommandType.Text;
            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            int result = cmd.ExecuteNonQuery();
            connection.Close();

            return result;
        }
        #endregion

        #region ExecuteQuery methods
        public IDataReader ExecuteQuery(string text, Dictionary<string, object> parameters, HttpContextBase httpContext)
            => this.ExecuteQuery(text, parameters?.Select(s => new MySqlParameter(s.Key, s.Value)), httpContext);

        public IDataReader ExecuteQuery(string text, Dictionary<string, object> parameters, IUser user)
            => this.ExecuteQuery(text, parameters?.Select(s => new MySqlParameter(s.Key, s.Value)), user);

        public IDataReader ExecuteQuery(string text, Dictionary<string, object> parameters, HttpContextBase httpContext, long page = 0)
            => this.ExecuteQuery(text, parameters?.Select(s => new MySqlParameter(s.Key, s.Value)), httpContext, page);

        public IDataReader ExecuteQuery(string text, Dictionary<string, object> parameters, IUser user, long page = 0)
            => this.ExecuteQuery(text, parameters?.Select(s => new MySqlParameter(s.Key, s.Value)), user, page);

        public IDataReader ExecuteQuery(string text, IEnumerable<MySqlParameter> parameters, HttpContextBase httpContext, long page = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            MySqlConnection connection = _connectionCache.GetOrCreate((httpContext?.User?.Identity.GetUserId()) ?? this.anonUser.Id, new Func<ICacheEntry, MySqlConnection>((x) =>
            {
                return new MySqlConnection(this._options.GetConnectionString());
            }));

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            MySqlCommand cmd = connection.CreateCommand();
            if (page > 0)
                cmd.CommandText = string.Format($"{text}\r\n{SqlResource.SqlPagingFormat}", FaViewOptions.RegistrosPorPagina, (page != 1 ? (" OFFSET " + (page * FaViewOptions.RegistrosPorPagina - FaViewOptions.RegistrosPorPagina + 1).ToString()) : " OFFSET 0"));
            else
                cmd.CommandText = text;
            cmd.CommandType = System.Data.CommandType.Text;
            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            MySqlDataReader result = (MySqlDataReader)cmd.ExecuteReader();

            DataSet ds = new DataSet();
            ds.Tables.Add("result");
            ds.Tables[0].Load(result, LoadOption.OverwriteChanges);

            connection.Close();
            return ds.CreateDataReader();
        }

        public IDataReader ExecuteQuery(string text, IEnumerable<MySqlParameter> parameters, IUser user, long page = 0)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            MySqlConnection connection = _connectionCache.GetOrCreate((user?.Id) ?? this.anonUser.Id, new Func<ICacheEntry, MySqlConnection>((x) =>
            {
                return new MySqlConnection(this._options.GetConnectionString());
            }));

            connection.Open();

            MySqlCommand cmd = connection.CreateCommand();
            if (page > 0)
                cmd.CommandText = string.Format($"{text}\r\n{SqlResource.SqlPagingFormat}", FaViewOptions.RegistrosPorPagina, (page != 1 ? (" OFFSET " + (page * FaViewOptions.RegistrosPorPagina - FaViewOptions.RegistrosPorPagina + 1).ToString()) : " OFFSET 0"));
            else
                cmd.CommandText = text;
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.AddRange(parameters.ToArray());

            MySqlDataReader result = (MySqlDataReader)cmd.ExecuteReader();

            DataSet ds = new DataSet();
            ds.Tables.Add("result");
            ds.Tables[0].Load(result, LoadOption.OverwriteChanges);

            connection.Close();
            return ds.CreateDataReader();
        }
        #endregion


        public void ClearConnection(IUser user)
        {
            if (user != null)
                this._connectionCache.Remove(user.Id);
        }
        #endregion
    }
}