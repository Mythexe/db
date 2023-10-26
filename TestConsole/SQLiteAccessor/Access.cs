using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteAccessor
{
    public class Access<T>
    {
        string filepath;
        string table;
        public Access(string filepath, string table)
        {
            this.filepath = filepath;
            this.table = table; 
        }
         private static string ConnectionStringBuilder(string filepath)
        {
            var connes = new SQLiteConnectionStringBuilder
            {
                DataSource = filepath,                
            };
            return connes.ToString();
        }
        public List<T> LoadDatasfromFile()
        {
            try
            {
                using (IDbConnection conn = new SQLiteConnection(ConnectionStringBuilder(filepath)))
                {
                    var output = conn.Query<T>($"select * from {table}", new DynamicParameters());
                    return output.ToList();
                }
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
        }
        public void SaveDatatoFile(T file, params string[] tags)
        {
            string command = $"insert into {table} (FirstName";
            //for(int i = 0; i < tags.Length; i++) 
            //{
            //    command += tags[i];
            //    if (i < tags.Length - 1)
            //    {
            //        command += ", ";
            //    }
            //}
            command += ") values (";
            for (int i = 0; i < tags.Length; i++)
            {
                command += "@";
                command += tags[i];
                if (i < tags.Length - 1)
                {
                    command += ", ";
                }
            }
            command += ")";

            using (IDbConnection conn = new SQLiteConnection(ConnectionStringBuilder(filepath)))
            {
                
                var output = conn.Execute(command, file);
            }
           
        }

        public void DeleteDataformFile(string tag, string value)
        {
            string command = $"delete from {table} where {tag} = '{value}'";


            using (IDbConnection conn = new SQLiteConnection(filepath))
            {
                var output = conn.Execute(command);
            }
        }
    }
}
