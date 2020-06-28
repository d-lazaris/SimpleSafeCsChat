using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server.SQLPresentable_
{
    class SQLPresentable_GroupChats : SQLPresentable
    {
        public int Id;
        public string chatName;
        public string password;
        public SQLPresentable_GroupChats()
        {
            TableName = "GroupChats";
            columns = new Dictionary<string, string>();
            //id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE, password TEXT)
            columns.Add("id", "INTEGER PRIMARY KEY AUTOINCREMENT");
            columns.Add("groupChatName", "TEXT UNIQUE");
            columns.Add("password", "TEXT");
        }
        public override void RowsToInnerFields()
        {

        }

        public override string InnerFieldsToInsert()
        {
            string fields = "";
            string values = "'" + chatName + "' " + password + "'";
            foreach (KeyValuePair<string, string> kvp in columns)
            {
                fields += kvp.Key + ", ";
            }
            fields = fields.Substring(0, fields.Length - 2);
            values = values.Substring(0, values.Length - 2);
            return String.Format("INSERT INTO {0}({1}) VALUES({2})", TableName, fields, values);
        }
    }
}
