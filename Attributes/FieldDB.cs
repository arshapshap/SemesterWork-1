using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class FieldDB : Attribute
    {
        public string ColumnName;
        public bool IsCyrillic;
        public FieldDB(string columnName, bool isCyrillic = false)
        {
            ColumnName = columnName;
            IsCyrillic = isCyrillic;
        }
    }
}
