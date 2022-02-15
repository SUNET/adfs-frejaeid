using ADFSFreja.Application.Interfaces;
using ADFSFreja.Application.Settings;
using ADFSFreja.Application.Utils;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ADFSFreja.Application.Services
{
    public class PersonServiceSql : IPersonService
    {
        private SqlSettings _sqlSettings;
        public PersonServiceSql(SqlSettings settings)
        {
            _sqlSettings = settings;
        }
        public string GetCivicNumber(string uid)
        {
            var civicNumber = "";
            Log.WriteEntry("Proceeding with GetCivicNumber from SQL ", EventLogEntryType.Information, 335);
            if (uid.Contains("@"))
            {
                uid = uid.Substring(0, uid.IndexOf('@'));
            }
            try
            {
                foreach (var s in _sqlSettings.Settings)
                {
                    civicNumber = Execute(s.ConnectionString, s.Command, uid);
                    if (civicNumber != null)
                    {
                        return civicNumber;
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteEntry("Error looking up civicnuber for user, error " + e.Message, EventLogEntryType.Error, 335);
            }
            
            return civicNumber;
        }
        private string Execute(string conn, string command, string uid)
        {
            var ret = "";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = GetConnection(conn);
            cmd.CommandText = command;
            cmd.Parameters.AddWithValue("uid", uid);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                ret = reader.GetString(0);
            }
            else
            {
                ret = null;
            }
            reader.Close();
            return ret;
        }

        private SqlConnection GetConnection(string connStr)
        {
            var conn = new SqlConnection();
            conn.ConnectionString = connStr;
            conn.Open();
            return conn;
        }
    }
}
