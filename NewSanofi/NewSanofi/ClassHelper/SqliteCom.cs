using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewSanofi
{
    public class SqliteCom
    {
        public SQLiteConnection sql_con;
        public SQLiteCommand sql_cmd;
        public SQLiteDataAdapter DB;
        public DataSet DS = new DataSet();
        public DataTable DT = new DataTable();
        private static SqliteCom _sqlite;
        public static SqliteCom Instance
        {
            get
            {
                if (_sqlite == null)
                {
                    _sqlite = new SqliteCom();
                }

                return _sqlite;

            }
        }
        public string databasename;
        private SqliteCom()
        {

        }
        public SqliteCom(string dbname)
        {
            databasename = dbname + ".db";
        }
        private void CreateDatabase()
        {
            if (!System.IO.File.Exists(Path.Combine(MainWindow.currentDirectory, databasename)))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile(Path.Combine(MainWindow.currentDirectory, databasename));
            }
        }


        private void SetConnection()
        {
            CreateDatabase();
            sql_con = new SQLiteConnection("Data Source=" + databasename + ";Version=3;New=False;Compress=True;");
        }
        public DataTable LoadTable(string table)
        {
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = "PRAGMA synchronize = OFF;PRAGMA jorunal_mode = MEMORY; ";
            sql_cmd.ExecuteNonQuery();

            string CommandText = "select * from " + table;
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            new SQLiteCommandBuilder(DB);

            return DT;
        }
        public DataTable LoadTable(string table, string database)
        {
            this.databasename = database;
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = "PRAGMA synchronize = OFF;PRAGMA jorunal_mode = MEMORY; ";
            sql_cmd.ExecuteNonQuery();

            string CommandText = "select * from " + table;
            DB = new SQLiteDataAdapter(CommandText, sql_con);
            DS.Reset();
            DB.Fill(DS);
            DT = DS.Tables[0];
            new SQLiteCommandBuilder(DB);

            return DT;
        }
       
        

        public void CreateTable(string TableName, List<TableType> tableParam)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"CREATE TABLE IF NOT EXISTS [" + TableName + "]([ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,");
            foreach (var table in tableParam)
            {
                sb.Append(table.ColumnName + " " + table.DataType + " " + (table.AllowNull ? "NULL" : "NOT NULL"));
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            //sb.Append("CREATE TABLE IF NOT EXISTS [Errors]([ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, [Name] NVARCHAR(50) NULL)");
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = sb.ToString();
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        public void UpdateTable(string TableName, List<string> parameterName, List<string> value, string keyCondition, string valueCondition)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"UPDATE " + TableName + " SET ");
            for (int i = 0; i < parameterName.Count; i++)
            {
                sb.Append(parameterName[i] + " ='" + value[i] + "',");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" WHERE " + keyCondition + "='" + valueCondition + "';");
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = sb.ToString();
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        public void CloneTable(string oriTable, string cloneTable)
        {
            string command = @"CREATE TABLE " + cloneTable + " AS Select * From " + oriTable;
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = command;
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }
        public void DeleteTable(string tableName)
        {
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = string.Format("DROP TABLE {0}", tableName);
            try
            {
                sql_cmd.ExecuteNonQuery();
            }
            catch { }
            sql_con.Close();
        }

        public void DeleteAllRows(string tableName)
        {
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = string.Format("Delete from {0}", tableName);
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        public void DeleteRows(string tableName,string ParamName,string ParamValue)
        {
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = string.Format("Delete from {0} Where {1} = '{2}'", tableName,ParamName,ParamValue);
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        public void DeleteAllTables()
        {
            List<string> nameTables = GetAllNameTable();
            foreach (var name in nameTables)
            {
                DeleteTable(name);
            }
        }
        public List<string> GetAllNameTable()
        {
            List<string> tables = new List<string>();
            SetConnection();
            sql_con.Open();
            DataTable dt = sql_con.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(tablename);
            }
            sql_con.Close();
            return tables;
        }
        public DataTable GetSchemaTable(string tableName)
        {
            SetConnection();
            sql_con.Open();
            //string[] restrictions = new string[4];
            //restrictions[2] = tableName;
            //DataTable dt = sql_con.GetSchema("Tables", restrictions);
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText = "Select * From " + tableName;
            var reader = sql_cmd.ExecuteReader(CommandBehavior.SchemaOnly);
            DataTable dt = reader.GetSchemaTable();
            sql_con.Close();

            return dt;
        }
        public void InsertRow(string table, List<string> param)
        {
            try
            {

                SetConnection();
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Insert into {0} values(", table));
                sb.Append("null,");
                foreach (var p in param)
                {
                    sb.Append(string.Format("'{0}',", p));
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                sql_cmd.CommandText = sb.ToString();
                sql_cmd.ExecuteNonQuery();
                sql_con.Close();
            }
            catch (Exception ex) { }
        }
    }
}
