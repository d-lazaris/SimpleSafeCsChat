using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SimpleSafeCsChat_Server
{
    class SCSC_SQLite
    {
        SQLiteConnection m_dbConnection;
        public bool Connect(string dbName, string username, string password)
        {
            m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", dbName));
            try
            {
                m_dbConnection.Open();
            }
            catch (SQLiteException se)
            {
                if (se.ErrorCode == 4060)
                {   
                    return CreateDatabase(dbName, username, password);
                }
                return false;
            }
            return true;
        }
        bool CreateDatabase(string dbName, string username, string password)
        {
            SQLiteConnection.CreateFile(dbName);
            Connect(dbName, username, password);
            return true;
        }
        public bool WriteToTable(string writeQuery)
        {
            //string sql = "create table highscores (name varchar(20), score int)";
            SQLiteCommand command = new SQLiteCommand(writeQuery, m_dbConnection);
            command.ExecuteNonQuery();
            return true;
        }

        //Returns List with numberOfLines*NumberOfColumns strings
        public List<string> ReadFromTable(string readQuery, int numberOfColumns)
        {
            SQLiteCommand command = new SQLiteCommand(readQuery, m_dbConnection);
            List<string> output = new List<string>();
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        output.Add(reader.GetValue(i).ToString().Trim());
                    }
                }
            }
            return output;
        }
    }
}
