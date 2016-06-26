using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hout.Models.Db
{
    public class TableColumnInfo
    {
        public TableColumnInfo(string name, DbType type, bool canBeNull = true, bool primaryKey = false)
        {
            Name = name;
            Type = type;
            CanBeNull = canBeNull;
            PrimaryKey = primaryKey;
        }
        public string Name { get; set; }
        public DbType Type { get; set; }
        public bool CanBeNull { get; set; }
        public bool PrimaryKey { get; set; }

        public string GetDbTypeName()
        {
            switch (Type)
            {
                case DbType.Blob:
                    return "BLOB";
                case DbType.Integer:
                    return "INTEGER";
                case DbType.Null:
                    return "NULL";
                case DbType.Real:
                    return "REAL";
                case DbType.Text:
                default:
                    return "TEXT";
            }
        }
        public string GetCreateColumnTxt()
        {
            var sb = new StringBuilder(Name);
            sb.Append(' ');
            sb.Append(GetDbTypeName());
            if (PrimaryKey)
            {
                sb.Append(" PRIMARY KEY");
            }
            if (!CanBeNull)
            {
                sb.Append(" NOT NULL");
            }
            return sb.ToString();
        }
    }
}
