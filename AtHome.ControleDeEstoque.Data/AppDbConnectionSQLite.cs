using System;
using System.Configuration;
using System.Data.SQLite;

namespace AtHome.ControleDeEstoque.Data
{
    public class AppDbConnectionSQLite : IDisposable
    {
        public SQLiteConnection Conexao()
        {
            try
            {
                //string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
                string path = @"C:\AtHome\Gestão de Estoque\ControleDeEstoque\AtHome.ControleDeEstoque\BancoSqLite";
                string nome_banco = "BancoSqLiteGestaoEstoque.db";

                DbConn.Connection = new SQLiteConnection("Data Source=" + path + @"\" + nome_banco + "; version=3; New=False; Compress=True;");
            }
            catch (Exception e)
            {
                throw e;
            }   
            return DbConn.Connection;
        }

        public SQLiteConnection OpenConnection()
        {
            Conexao();
            try
            {
                //_conn.Open();
                return DbConn.Connection;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (DbConn.Connection.State == System.Data.ConnectionState.Open)
                {
                    DbConn.Connection.Close();
                    DbConn.Connection.Dispose();
                    SQLiteConnection.ClearAllPools();
                    DbConn.Connection = null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public SQLiteDataReader ExecuteQuery(string sql)
        {
            SQLiteCommand sqlComm = new SQLiteCommand(sql, OpenConnection());
            sqlComm.Connection.Open();

            SQLiteDataReader reader = sqlComm.ExecuteReader();

            sqlComm.Dispose();
            
            return reader;
        }

        public int ExecuteScalar(string sql)
        {
            int id = 0;
            try
            {
                SQLiteCommand sqlComm = new SQLiteCommand(sql, OpenConnection());
                sqlComm.Connection.Open();

                id = Convert.ToInt32(sqlComm.ExecuteScalar());

                sqlComm.Dispose();
            }
            catch(Exception e)
            {
                throw e;
            }

            return id;
        }

        public long ExecuteNonQuery(string sql)
        {
            long regs;

            try
            {
                SQLiteCommand sqlComm = new SQLiteCommand(sql, OpenConnection());
                sqlComm.Connection.Open();

                regs = sqlComm.ExecuteNonQuery();

                sqlComm.Dispose();
                
                return regs;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            this.CloseConnection();
        }
    }
}
