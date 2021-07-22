using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSanofi
{
    public class TableType
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool AllowNull { get; set; }
        public TableType(string ColumnName, string DataType, bool AllowNull)
        {
            this.ColumnName = ColumnName;
            this.DataType = DataType;
            this.AllowNull = AllowNull;
        }
        public TableType()
        {

        }
    }
}
