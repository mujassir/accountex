using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AccountEx.Repositories
{
   

        public class SqlRepository
        {

            public DataTable GetDataTable(string query)
            {
                using (var con = Connection.GetsqlConnection())
                {
                    using (var cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            if (ds.Tables.Count > 0)
                                return ds.Tables[0];
                        }
                    }
                }
                return null;
            }
            public DataTable GetDataTable(string query, List<SqlParameter> parameters)
            {
                using (var con = Connection.GetsqlConnection())
                {
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.StoredProcedure })
                    {
                        foreach (var item in parameters)
                            cmd.Parameters.AddWithValue(item.ParameterName, item.Value);
                        con.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            if (ds.Tables.Count > 0)
                                return ds.Tables[0];
                        }
                    }
                }
                return null;
            }
        public DataSet GetDataSet(string query, List<SqlParameter> parameters)
        {
            using (var con = Connection.GetsqlConnection())
            {
                using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.StoredProcedure })
                {
                    foreach (var item in parameters)
                        cmd.Parameters.AddWithValue(item.ParameterName, item.Value);
                    con.Open();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }

                }
            }
        }
        public DataTable GetDataTable(string query, params SqlParameter[] parameters)
            {
                using (var con = Connection.GetsqlConnection())
                {
                    using (var cmd = new SqlCommand(query, con) { CommandType = CommandType.StoredProcedure })
                    {
                        foreach (var item in parameters)
                            cmd.Parameters.Add(item);
                        con.Open();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            if (ds.Tables.Count > 0)
                                return ds.Tables[0];
                        }
                    }
                }
                return null;
            }

            public SqlParameterCollection GetParams(string spName)
            {
                using (var con = Connection.GetsqlConnection())
                {
                    SqlCommand cmd = new SqlCommand(spName, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlCommandBuilder.DeriveParameters(cmd);
                    return cmd.Parameters;
                }
            }

        }

  
}
