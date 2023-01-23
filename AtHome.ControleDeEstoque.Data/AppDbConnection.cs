using System;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Collections.Generic;

namespace AtHome.ControleDeEstoque.Data
{
    class AppDbConnection : IDisposable
    {
        SqlCeConnection _conn;

        public SqlCeConnection Conexao()
        {
            try
            {
                var strConn = ConfigurationManager.ConnectionStrings["ControleDeEstoque"].ToString().Trim();
                _conn = new SqlCeConnection(strConn);
            }
            catch (Exception e)
            {
                throw e;
            }

            return _conn;
        }

        public SqlCeConnection OpenConnection()
        {
            SqlCeConnection cn = Conexao();
            try
            {
                cn.Open();
                return cn;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CloseConnection(SqlCeConnection cn)
        {
            try
            {
                cn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SqlCeDataReader ExecuteQuery(string sql)
        {
            try
            {
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());
                SqlCeDataReader reader = sqlComm.ExecuteReader();
                return reader;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public int ExecuteScalar(string sql)
        {
            try
            {  
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());
                int id = Convert.ToInt32(sqlComm.ExecuteScalar());
                return id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public long ExecuteNonQuery(string sql)
        {
            long regs;

            try
            {
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());
                regs = sqlComm.ExecuteNonQuery();

                return regs;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public long ExecuteNonQuery(string sql, Dictionary<string, dynamic> parameters)
        {
            long regs;

            try
            {
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());

                foreach (var p in parameters)
                {
                    sqlComm.Parameters.AddWithValue(p.Key, p.Value);
                }
                regs = sqlComm.ExecuteNonQuery();

                return regs;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            this.CloseConnection(_conn);
        }
    }

    class AppDbConnectionLog : IDisposable
    {
        SqlCeConnection _conn;

        public SqlCeConnection Conexao()
        {
            try
            {
                var strConn = ConfigurationManager.ConnectionStrings["ControleDeEstoqueLog"].ToString().Trim();
                _conn = new SqlCeConnection(strConn);
            }
            catch (Exception)
            {
            }

            return _conn;
        }

        public SqlCeConnection OpenConnection()
        {
            SqlCeConnection cn = Conexao();
            try
            {
                cn.Open();
                return cn;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CloseConnection(SqlCeConnection cn)
        {
            try
            {
                cn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SqlCeDataReader ExecuteQuery(string sql)
        {
            try
            { 
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());
                SqlCeDataReader reader = sqlComm.ExecuteReader();
                return reader;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int ExecuteScalar(string sql)
        {
            try
            {
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());
                int id = Convert.ToInt32(sqlComm.ExecuteScalar());
                return id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public long ExecuteNonQuery(string sql)
        {
            long regs;

            try
            {
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());
                regs = sqlComm.ExecuteNonQuery();

                return regs;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public long ExecuteNonQuery(string sql, Dictionary<string, dynamic> parameters)
        {
            long regs;

            try
            {
                SqlCeCommand sqlComm = new SqlCeCommand(sql, OpenConnection());

                foreach (var p in parameters)
                {
                    sqlComm.Parameters.AddWithValue(p.Key, p.Value);
                }
                regs = sqlComm.ExecuteNonQuery();

                return regs;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            this.CloseConnection(_conn);
        }
    }
}
