using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace WbWCF.DataAccess
{
    public class BaseDataAccess
    {
        public static SqlConnection GetConnection()
        {
            string sSqlConnection = "Database=WB;server=localhost;trusted_connection=true";
            SqlConnection sqlConnection = new SqlConnection(sSqlConnection);
            return sqlConnection;
        }
    }
}