using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleSafeCsChat_Server.SQLPresentable_;

namespace SimpleSafeCsChat_Server
{
    class SQLManager
    {
        string username;
        string password;
        SCSC_SQLite m_scscSQLite;
        public SQLManager(string un, string pwd)
        {
            username = un;
            password = pwd;
        }
        public bool Start(string dbFile)
        {
            m_scscSQLite = new SCSC_SQLite();
            return m_scscSQLite.Connect(dbFile, username, password);
        }

        public bool CreateTables()
        {
            m_scscSQLite.WriteToTable("CREATE TABLE IF NOT EXISTS Users (id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT UNIQUE, password TEXT)");
            m_scscSQLite.WriteToTable("CREATE TABLE IF NOT EXISTS GroupChats (id INTEGER PRIMARY KEY AUTOINCREMENT, groupChatName TEXT, password TEXT)");
            m_scscSQLite.WriteToTable("CREATE TABLE IF NOT EXISTS GroupChatMessages (id INTEGER PRIMARY KEY AUTOINCREMENT, groupChatId INTEGER, messageId INTEGER, message TEXT)");
            m_scscSQLite.WriteToTable("CREATE TABLE IF NOT EXISTS GroupChatParticipants (id INTEGER PRIMARY KEY AUTOINCREMENT, userId INTEGER, chatId INTEGER, lastSeenMessage INTEGER)");
            return true;
        }

        public bool ReadStructureFromSql(ref List<string> structure, string tableName)
        {
            string query = String.Format("SELECT * FROM {0}", tableName);
            structure = m_scscSQLite.ReadFromTable(query, structure.Count());
            return true;
        }

        public bool ReadStructureFromSqlWhere(ref List<List<string>> structure, string tableName, string whereCondition)
        {
            string query = String.Format("SELECT * FROM {0} WHERE {1}", tableName, whereCondition);
            int structureSize = structure[0].Count();
            var outList = m_scscSQLite.ReadFromTable(query, structureSize);
            int itCount = outList.Count / structureSize;
            int i = 0;
            while (i < itCount)
            {
                List<string> tempList = new List<string>();
                for (int j = 0; j < structureSize; j++)
                {
                    tempList.Add(outList[i * structureSize + j]);
                }
                structure.Add(tempList);
                i++;
            }
            return true;
        }

        public bool WriteStructureToSql(SQLPresentable structure)
        {
            string query = structure.InnerFieldsToInsert();
            try
            {
                m_scscSQLite.WriteToTable(query);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
