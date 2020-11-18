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

        public static List<List<string>> LoadTable(string key, List<string> colomns)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                List<List<string>> asColomns = new List<List<string>>();
                List<List<string>> asRows = new List<List<string>>();
                foreach (string colomn in colomns)
                {
                    var output = cnn.Query<string>($"SELECT {colomn} FROM '{key}';", new DynamicParameters());

                    asColomns.Add(output.ToList());
                    
                }

                for (int i = 0; i < asColomns[0].Count; i++)
                {
                    asRows.Add(new List<string>());
                    for (int j = 0; j < asColomns.Count; j++)
                    {
                        asRows[i].Add(asColomns[j][i]);
                    }
                }

                return asRows;
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

        public static List<string> GetColomns(string table)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                var output = cnn.Query<string>($"SELECT name FROM PRAGMA_TABLE_INFO('{table}');", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveToTable(string key, List<string> values)
        {
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string table = $"INSERT INTO '{key}' VALUES (";
                for (int i = 0; i < values.Count; i++)
                {
                    table += $"'{values[i]}'";
                    if (i == values.Count -1)
                        table += ");";
                    else
                        table += ", ";
                }
                cnn.Execute(table);
            }
        }
    }
}
