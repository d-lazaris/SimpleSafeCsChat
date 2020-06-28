using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server.SQLPresentable_
{
    public abstract class SQLPresentable
    {
        public string TableName;

        public int columnsCount;

        public Dictionary<string, string> columns;

        public List<List<string>> Rows;

        public string ConstructTable()
        {
            string columnsAsLine = "";
            foreach (var item in columns)
            {
                columnsAsLine += item.Key + " " + item.Value;
            }
            return String.Format("CREATE TABLE IF NOT EXISTS {0}({1})", TableName, columnsAsLine);
        }

        public abstract void RowsToInnerFields();

        public abstract string InnerFieldsToInsert();
    }
}
