using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hout.Models.Device;
using Newtonsoft.Json;

namespace Hout.Models.Db
{
    public partial class Client
    {
        private readonly TableColumnInfo[] _deviceColumns =
        {
            new TableColumnInfo("Id", DbType.Text, false, true),
            new TableColumnInfo("Name", DbType.Text, false),
            new TableColumnInfo("Type", DbType.Text, false),
            new TableColumnInfo("PropertiesJson", DbType.Text)
        };
        private async Task CreateDeviceTable()
        {
            if (!await CheckTableExists("Devices"))
            {
                var createTableSql =
                    $"CREATE TABLE Devices({string.Join(", ", _deviceColumns.Select(c => c.GetCreateColumnTxt()))});";
                _conn.Execute(createTableSql);
            }
        }
        public async Task AddOrUpdate(BaseDevice device)
        {
            var script = GetInsertScript("Devices", _deviceColumns);
            await _conn.ExecuteAsync(script, new
            {
                Id = device.GetId(),
                Name = device.Name,
                Type = device.GetType().AssemblyQualifiedName,
                PropertiesJson = JsonConvert.SerializeObject(device.Properties, JsonSerializerSettings)
            });
        }

        public async Task Remove(BaseDevice device)
        {
            var script = $@"DELETE FROM Devices WHERE Id = @Id";
            await _conn.ExecuteAsync(script, new {Id = device.Id});
        }

        public async Task<IEnumerable<BaseDevice>> GetDevices()
        {
            var recs = await _conn.QueryAsync("SELECT * FROM Devices");
            return recs.Select(GetDeviceFromRec);
        }

        private BaseDevice GetDeviceFromRec(dynamic rec)
        {
            var type = Type.GetType(rec.Type);
            var device = (BaseDevice) Activator.CreateInstance(type);
            device.Name = rec.Name;
            device.Properties = JsonConvert.DeserializeObject<PropertyCollection>(rec.PropertiesJson);
            device.Id = rec.Id;
            return device;
        }
    }
}
