using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace Hout.Models.Db
{
    public partial class Client : IDisposable
    {
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            TypeNameHandling = TypeNameHandling.Auto
        };
        private readonly string connStr;
        private readonly SQLiteConnection _conn;
        public Client()
        {
            var sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = "Hout.db";
            sb.FailIfMissing = false;
            sb.PageSize = 32768;
            sb.ForeignKeys = false;
            sb.UseUTF16Encoding = false;
            sb.Pooling = true;
            sb.JournalMode = SQLiteJournalModeEnum.Wal;
            sb.SyncMode = SynchronizationModes.Normal;
            sb.DateTimeKind = DateTimeKind.Utc;
            sb.DateTimeFormat = SQLiteDateFormats.ISO8601;
            sb.DefaultIsolationLevel = System.Data.IsolationLevel.ReadCommitted;
            connStr = sb.ToString();
            _conn = new SQLiteConnection(connStr);
        }

        public async Task Initialize()
        {
            await _conn.OpenAsync();
        }

        public async Task EnsureTables()
        {
            var tasks = new[] {CreateDeviceTable()};
            await Task.WhenAll(tasks);
        }
        

        private async Task<bool> CheckTableExists(string tableName)
        {
            return
                await
                    _conn.ExecuteScalarAsync<long>(
                        "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@table", new {table = tableName}) > 0;
        }
        protected async Task<SQLiteConnection> Open()
        {
            if (connStr == null)
            {
                throw new ArgumentNullException("Invalid connection string, call Configure to set the connection string.");
            }
            var connection = new SQLiteConnection(connStr);
            await connection.OpenAsync();
            return connection;
        }

        private string GetInsertScript(string table, TableColumnInfo[] columns)
        {
            return $"INSERT OR REPLACE INTO {table}({string.Join(",", columns.Select(c => c.Name))}) VALUES({string.Join(",", columns.Select(c => "@" + c.Name))})";
        }

        public void Dispose()
        {
            _conn.Dispose();
        }
    }
}
