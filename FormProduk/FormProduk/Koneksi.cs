using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormProduk
{
    internal class Koneksi
    {
        public static SqlConnection GetConnection()
        {
            // Ganti SERVER_NAME dengan nama server kamu, biasanya
            // .\SQLEXPRESS atau (localdb)\MSSQLLocalDB
            string connectionString = @"Data Source=DESKTOP-KJM4I64\SQLEXPRESS;Initial Catalog=TokoDR;Integrated Security=True;";
            return new SqlConnection(connectionString);
        }
    }
}

