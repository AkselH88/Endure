using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Endure.DataAccess
{
    class SQLiteDataAccess
    {
        private readonly static string DBFile = "endure.sqlite";
        private readonly static string ConnectionString = $"Data Source={DBFile};Version=3;";

        public static bool InitDB(string DataBase = "")
        {
            string file = (DataBase == string.Empty) ? DBFile : DataBase;
            string path = Directory.GetCurrentDirectory() + "\\" + file;
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());

            if (!files.Contains(path))
            {
                SQLiteConnection.CreateFile(file);

                return false;
            }

            return true;
        }

        private static string GetConnectionString(string DataBase)
        {
            return DataBase == string.Empty ? ConnectionString : $"Data Source={DataBase};Version=3;";
        }

        public static void CreateTable(string table, List<string> colomns, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"CREATE TABLE '{table}' (";
                for (int i = 0; i < colomns.Count; i++)
                {
                    command += $"\"{colomns[i]}\" text";
                    if (i == colomns.Count - 1)
                        command += ");";
                    else
                        command += ", ";
                }

                cnn.Execute(command);
                Debug.WriteLine(command);
            }
        }

        public static void RemoveTable(string table, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                cnn.Execute($"DROP TABLE '{table}';");
            }
        }

        public static List<List<string>> LoadTable(string table, List<string> colomns, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                List<List<string>> asColomns = new List<List<string>>();
                List<List<string>> asRows = new List<List<string>>();
                foreach (string colomn in colomns)
                {
                    var output = cnn.Query<string>($"SELECT \"{colomn}\" FROM '{table}';", new DynamicParameters());

                    asColomns.Add(output.ToList());
                }

                for (int i = 0; i < asColomns[0].Count; i++)
                {
                    asRows.Add(new List<string>());
                    for (int j = 0; j < asColomns.Count; j++)
                    {
                        asRows[i].Add((asColomns[j][i] == null) ? "" : asColomns[j][i]);
                    }
                }

                return asRows;
            }
        }

        public static List<List<string>> LoadTableByColomn(string table, List<string> colomns, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                List<List<string>> asColomns = new List<List<string>>();
                foreach (string colomn in colomns)
                {
                    var output = cnn.Query<string>($"SELECT \"{colomn}\" FROM '{table}';", new DynamicParameters());

                    asColomns.Add(output.ToList());
                }

                return asColomns;
            }
        }

        public static string LoadValueFromTable(string table, string colomn, string keyColomn, string keyValue, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"SELECT \"{colomn}\" FROM '{table}' WHERE \"{keyColomn}\" = '{keyValue}';";

                var output = cnn.Query<string>(command);

                return output.Single();
            }
        }

        public static void RemoveRow(string table, string keyColomn, string keyValue, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"DELETE FROM '{table}' WHERE \"{keyColomn}\" = '{keyValue}';";

                cnn.Execute(command);
            }
        }

        public static void RemoveRow(string table, string keyColomn1, string keyValue1, string keyColomn2, string keyValue2, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"DELETE FROM '{table}' WHERE \"{keyColomn1}\" = '{keyValue1}' AND \"{keyColomn2}\" = '{keyValue2}';";

                cnn.Execute(command);
            }
        }

        public static List<string> GetTables(string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                var output = cnn.Query<string>("SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%';", new DynamicParameters());
                //Debug.WriteLine(output.ToString());
                return output.ToList();
            }
        }

        public static List<string> GetColomns(string table, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                var output = cnn.Query<string>($"SELECT name FROM PRAGMA_TABLE_INFO('{table}');", new DynamicParameters());
                //Debug.WriteLine(output.ToString());
                return output.ToList();
            }
        }

        public static void AddColomn(string table, string colomn, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"ALTER TABLE '{table}' ADD \"{colomn}\" text;";
                cnn.Execute(command);
                Debug.WriteLine(command);
            }
        }

        public static void RemoveColomn(string table, string colomn, string db = "")
        {
            List<string> colomns = GetColomns(table, db);
            colomns.Remove(colomn);
            List<List<string>> values = LoadTable(table, colomns, db);

            RemoveTable(table, db);
            CreateTable(table, colomns, db);

            foreach (List<string> row in values)
            {
                SaveToTable(table, row, db);
            }

            // Can not use this matod in SQLite need a work around
            /*
            using (IDbConnection cnn = new SQLiteConnection(ConnectionString))
            {
                string command = $"ALTER TABLE '{table}' DROP COLOMN \"{colomn}\";";
                cnn.Execute(command);
                Debug.WriteLine(command);
            }*/
        }

        public static void SaveToTable(string table, List<string> values, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"INSERT INTO '{table}' VALUES (";
                for (int i = 0; i < values.Count; i++)
                {
                    command += $"'{values[i]}'";
                    if (i == values.Count - 1)
                        command += ");";
                    else
                        command += ", ";
                }
                cnn.Execute(command);
                Debug.WriteLine(command);
            }
        }

        public static void UpdateTable(string table, string colomn, string value, string keyColomn, string keyValue, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"UPDATE '{table}' SET";

                command += $" \"{colomn}\" = '{value}'";

                command += $" WHERE \"{keyColomn}\" = '{keyValue}';";

                cnn.Execute(command);
                Debug.WriteLine(command);
            }
        }

        public static void UpdateTable(string table, string colomn, string value, string keyColomn1, string keyValue1, string keyColomn2, string keyValue2, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"UPDATE '{table}' SET";

                command += $" \"{colomn}\" = '{value}'";

                command += $" WHERE \"{keyColomn1}\" = '{keyValue1}' AND \"{keyColomn2}\" = '{keyValue2}';";

                cnn.Execute(command);
                Debug.WriteLine(command);
            }
        }

        public static void SaveToTableAtSpesifiedRow(string table, List<string> colomns, List<string> values, string db = "")
        {
            using (IDbConnection cnn = new SQLiteConnection(GetConnectionString(db)))
            {
                string command = $"UPDATE '{table}' SET";

                for (int i = 1; i < colomns.Count; i++)
                {
                    command += $" \"{colomns[i]}\" = '{values[i]}'";
                    if (i != colomns.Count - 1)
                        command += ",";
                }

                command += $" WHERE \"{colomns.First()}\" = '{values.First()}';";

                cnn.Execute(command);
                Debug.WriteLine(command);
            }
        }
    }
}
