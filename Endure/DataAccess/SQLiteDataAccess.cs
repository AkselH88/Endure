using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace Endure.DataAccess
{
    class SQLiteDataAccess
    {
        private readonly static string DBFile = "endure.sqlite";
        private readonly static string ConnectionString = $"Data Source={DBFile};Version=3;";

        public static bool InitDB()
        {
            string path = Directory.GetCurrentDirectory() + "\\" + DBFile;
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());

            if (!files.Contains(path))
            {
                SQLiteConnection.CreateFile(DBFile);

                return false;
            }

            return true;
        }

        public static void CreateTable(string key, string[] colomns)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string table = $"CREATE TABLE '{key}' (";
                for(int i = 0; i < colomns.Length; i++)
                {
                    table += $"{colomns[i]} text";
                    if (colomns[i] == colomns[^1])
                        table += ");";
                    else
                        table += ", ";
                }

                cnn.Execute(table);
            }
        }

        /// <summary>
        /// key represent table
        /// string "action" options ADD, DROP COLUMN, ALTER COLUMN
        /// the action ADD and ALTER COLUMN require a datatype.
        /// datatypes are {}
        /// </summary>
        public static void AlterTable(string key, string action, string colomn, string datatype = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                if(datatype == "")
                    cnn.Execute($"ALTER TABLE '{key}' {action} {colomn};");
                else
                    cnn.Execute($"ALTER TABLE '{key}' {action} {colomn} {datatype};");
            }
        }

        public static List<(string, string)> LoadTable(string key)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var output = cnn.Query<(string, string)>($"SELECT * FROM '{key}';", new DynamicParameters());

                return output.ToList();
            }
        }

        public static List<string> GetTables()
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var output = cnn.Query<string>("SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'; ", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveToTable(string key, string[] values)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string table = $"INSERT INTO '{key}' VALUES (";
                for (int i = 0; i < values.Length; i++)
                {
                    table += $"'{values[i]}'";
                    if (values[i] == values[^1])
                        table += ");";
                    else
                        table += ", ";
                }
                cnn.Execute(table);
            }
        }
    }
}
