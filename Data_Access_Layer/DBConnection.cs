using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Data_Access_Layer
{
    class DBConnection
    {
        private string _connectionstring;

        private System.Data.SqlClient.SqlConnection _connection;
        private System.Data.SqlClient.SqlTransaction _transaction;

        public string Connectionstring
        {
            get { return _connectionstring; }
            set { _connectionstring = value; }
        }


        public System.Data.SqlClient.SqlConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public System.Data.SqlClient.SqlTransaction Transaction
        {
            get { return _transaction; }
            set { _transaction = value; }
        }

        public DBConnection()
        {
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            string strConnectionstring = Convert.ToString(config.AppSettings.Settings["ConnectionString"].Value);
            if (!string.IsNullOrEmpty(strConnectionstring))
                Connectionstring = strConnectionstring;
        }

        public bool OpenConnection()
        {
            try
            {
                if (string.IsNullOrEmpty(Connectionstring))
                    throw new Exception("Connection not available.");
                Connection = new System.Data.SqlClient.SqlConnection(Connectionstring);
                Connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                if (Connection == null)
                    throw new Exception("Connection not available.");

                Connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool BeginTransaction(string transactionName)
        {
            try
            {
                if (Connection == null)
                    throw new Exception("Connection not available.");

                Transaction = Connection.BeginTransaction(transactionName);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CommitTransaction()
        {
            try
            {
                if (Connection == null)
                    throw new Exception("Connection not available.");

                if (Transaction == null)
                    throw new Exception("Transaction not available.");

                Transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool RollBackTransaction()
        {
            try
            {
                if (Connection == null)
                    throw new Exception("Connection not available.");

                if (Transaction == null)
                    throw new Exception("Transaction not available.");

                Transaction.Rollback();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        #region Transactional Methods

        internal string ExecuteScalar(string Query, Dictionary<string, object> Parameters = null)
        {
            string result = string.Empty;
            try
            {
                if (Connection == null)
                    throw new Exception("Connection is not available.");

                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(Query, _connection, _transaction);
                if (Parameters != null)
                    foreach (KeyValuePair<string, object> obj in Parameters)
                        cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(obj.Key, obj.Value));

                result = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        internal bool ExecuteNonQuery(string Query, Dictionary<string, object> Parameters = null)
        {
            if (_connection == null)
                throw new Exception("Connection is not available.");
            if (_transaction == null)
                throw new Exception("Transaction is not available.");

            int result;
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(Query, _connection, _transaction);
            if (Parameters != null)
                foreach (KeyValuePair<string, object> obj in Parameters)
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(obj.Key, obj.Value));

            result = cmd.ExecuteNonQuery();
            cmd = null;
            return result > 0 ? true : false;
        }

        internal System.Data.DataTable ExecuteQuery(string query, Dictionary<string, object> Parameters = null)
        {
            if (_connection == null)
                throw new Exception("Connection is not available.");

            System.Data.DataTable dtDetails = new System.Data.DataTable();
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(query, _connection);
            cmd.CommandTimeout = 0;

            if (Parameters != null)
                foreach (KeyValuePair<string, object> obj in Parameters)
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(obj.Key, obj.Value));

            System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter(cmd);
            adp.Fill(dtDetails);
            adp = null;
            cmd = null;
            return dtDetails;
        }


        internal bool SPExecuteNonQuery(string ProcedureName, Dictionary<string, object> Parameters)
        {
            if (_connection == null)
                throw new Exception("Connection is not available.");
            if (_transaction == null)
                throw new Exception("Transaction is not available.");

            int result;
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = Connection;
            cmd.Transaction = Transaction;
            cmd.CommandTimeout = 0;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = ProcedureName;
            if (Parameters != null)
                foreach (KeyValuePair<string, object> obj in Parameters)
                    cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter(obj.Key, obj.Value));
            result = cmd.ExecuteNonQuery();
            cmd = null;
            return result > 0 ? true : false;
        }
        #endregion

    }
}
