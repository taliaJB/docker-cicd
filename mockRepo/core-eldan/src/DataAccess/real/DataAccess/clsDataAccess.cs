using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace Eldan.DataAccess
{
    public struct Parameter
    {
        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name;
        public object Value;
    }

    public enum enmConnectMode
    {
        ConnectionString,
        AppSettingKey
    }

    public class clsDataAccess
    {
        private const int DEFUALT_TIMEOUT = 30;

        #region Local members
        private string m_ConnectionString;
        private string m_AppSettingKey;
        private int m_Timeout; 
        #endregion

        #region Constructors
        public clsDataAccess()
        {
            m_Timeout = DEFUALT_TIMEOUT;
            m_AppSettingKey = "connString";
            m_ConnectionString = ConfigurationManager.AppSettings[m_AppSettingKey];
        }

        public clsDataAccess(enmConnectMode ConnectMode, string Value)
        {
            m_Timeout = DEFUALT_TIMEOUT;
            switch (ConnectMode)
            {
                case enmConnectMode.ConnectionString:
                    m_AppSettingKey = null;
                    m_ConnectionString = Value;
                    break;
                case enmConnectMode.AppSettingKey:
                    m_AppSettingKey = Value;
                    m_ConnectionString = ConfigurationManager.AppSettings[m_AppSettingKey];
                    break;
                default:
                    break;
            }
        }
        #endregion // Constructors

        #region Properties
        public int Timeout
        {
            get { return m_Timeout; }
            set { m_Timeout = value; }
        }

        public string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        public string AppSettingKey
        {
            get { return m_AppSettingKey; }
        }
        #endregion //Properties

        #region ExecuteSP - no return
        public void ExecuteSP(string SPName)
        {
            ExecuteSP(SPName, null);
        }

        public void ExecuteSP(string SPName, params Parameter[] parameters)
        {
            ExecuteSPds(SPName, parameters);
        }
        #endregion

        #region ExecuteSPdt, SelectQueryDt - return data table
        public DataTable ExecuteSPdt(string SPName)
        {
            return ExecuteSPdt(SPName, null);
        }

        public DataTable ExecuteSPdt(string SPName, params Parameter[] parameters)
        {
            DataSet ds = ExecuteSPds(SPName, parameters);
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable SelectQueryDt(string sqlCmd)
        {
            DataSet ds = SelectQueryDs(sqlCmd);
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }
        #endregion

        #region ExecuteSPXML - return XML
        public string ExecuteSPXML(string SPName)
        {
            return ExecuteSPXML(SPName, null);
        }

        public string ExecuteSPXML(string SPName, params Parameter[] parameters)
        {
            string ResXML = "";
            
            DataSet ds = ExecuteSPds(SPName, parameters);

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ResXML = ResXML + row.Field<string>(0);
                }
            }
            else
                ResXML = null;

            return ResXML;
        }
        #endregion

        #region ExecuteSPds, SelectQueryDs - return data set
        public DataSet ExecuteSPds(string SPName)
        {
            return ExecuteSPds(SPName, null);
        }

        public DataSet ExecuteSPds(string SPName, params Parameter[] parameters)
        {
            DataSet ds = new DataSet();

            // 1. create a command object identifying
            // the stored procedure
            SqlCommand cmd = new SqlCommand(
                SPName);

            cmd.CommandTimeout = m_Timeout;

            // 2. set the command object so it knows
            // to execute a stored procedure
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                // 3. add parameter to command, which
                // will be passed to the stored procedure            
                foreach (Parameter param in parameters)
                {
                    cmd.Parameters.Add(
                        new SqlParameter(param.Name, param.Value ?? DBNull.Value));
                }
            }

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (SqlConnection connection =
                new SqlConnection(m_ConnectionString))
            {
                cmd.Connection = connection;

                //SqlDataAdapter myAdapter = new SqlDataAdapter(queryString, connection);
                SqlDataAdapter myAdapter = new SqlDataAdapter(cmd);

                //fill the dataset with the data by some name say "CustomersTable"
                myAdapter.Fill(ds);
            }

            return ds;
        }

        public DataSet SelectQueryDs(string sqlCmd)
        {
            DataSet ds = new DataSet();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = m_Timeout;

            using (SqlConnection connection = new SqlConnection(m_ConnectionString))
            {
                cmd.CommandText = sqlCmd;
                cmd.Connection = connection;
                SqlDataAdapter myAdapter = new SqlDataAdapter(cmd);
                myAdapter.Fill(ds);
            }

            return ds;
        }
        #endregion

        #region Return scalar
        public T SelectQueryScalar<T>(string sqlCmd)
        {
            DataTable dt = SelectQueryDt(sqlCmd);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0].Field<T>(0);
                }
            }

            return default(T);

        } 
        #endregion



    }
}
