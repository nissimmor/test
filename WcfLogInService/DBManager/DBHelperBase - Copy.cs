using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace DBManager
{
    public class DBHelperBase
    {
        DbConnection Conn = null;
        protected DbCommand Cmnd = null;
        string ConnectionString = "";
        string Caption = "";
        bool ConnectionOpen = false;

        public DBHelperBase(string ConnectionString, string Name)
        {
            this.ConnectionString = ConnectionString;
            this.Caption = Name;
            Logger.Debug("   -> Initialize connection: " + Name);
            Logger.Debug("   Connection string: " + this.ConnectionString);
            string ErrMsg = "";
            OpenConnection(out ErrMsg);
        }

        public bool OpenConnection(out string errMsg)
        {
            bool retval = false;
            errMsg = "";
            if (Conn != null)
            {
                _CloseConnection();
            }
            Logger.Debug("   ->Open Connection to DB...");
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                Logger.Error("   ->Failed to open connecton. connection string is empty");
                return false;
            }

            try
            {
                Conn = new DbConnection(ConnectionString);
                Conn.Open();
                DbCommand cmd = new DbCommand("Select GETDATE()", Conn);
                cmd.ExecuteNonQuery();
                retval = true;
                ConnectionOpen = true;
            }
            catch (Exception ex)
            {
                Logger.Error("OpenConnection:: Failed to open Connection: " + Caption + ". Reason: " + ex.Message);
                Logger.WriteEventLog("Failed to open connection " + Caption +
                                    Environment.NewLine + ex.Message);
                ConnectionOpen = false;
                errMsg = ex.Message;
            }
            return retval;
        }
        public void CloseConnection()
        {
            _CloseConnection();
        }

        public bool ExecuteNonQuery(DbCommand cmnd, out int EffectedRecord, out string errMsg)
        {
            bool retVal = false;
            errMsg = "";
            EffectedRecord = 0;
            if (!CheckConnectionStatus(out errMsg))
            {
                Logger.Error("ExecuteNonQuery:: Unable to execute query due to connection problem");
                return false;
            }

            if (cmnd == null)
            {
                Logger.Error("ExecuteNonQuery:: Unable to execute query. Command object is Null");
                return false;
            }
            try
            {
                cmnd.Connection = Conn;
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                EffectedRecord = cmnd.ExecuteNonQuery();
                sw.Stop();
                Logger.Debug("ExecuteNonQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");

                retVal = true;
            }
            catch (Exception ex)
            {
                Logger.Error("ExecuteNonQuery:: Exception during executing command: " + cmnd.CommandText);
                Logger.Error("  ->Error: " + ex.Message);
                Logger.Error("  ->" + ex.ToString());
                errMsg = ex.Message;
                retVal = false;
            }


            return retVal;
        }
        public DbDataReader ExecuteQuery(DbCommand cmnd, out string errMsg)
        {
            DbDataReader dr = null;
            errMsg = "";
            if (!CheckConnectionStatus(out errMsg))
            {
                Logger.Error("ExecuteQuery:: Unable to execute query due to connection problem");
                return dr;
            }

            if (cmnd == null)
            {
                Logger.Error("ExecuteQuery:: Unable to execute query. Command object is Null");
                return dr;
            }
            try
            {
                cmnd.Connection = Conn;
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                dr = cmnd.ExecuteReader();
                sw.Stop();
                //Logger.Debug("ExecuteNonQuery:: executing command: " + cmnd.CommandText + " recieved " + dr.RecordsAffected + " records");
                Logger.Debug("ExecuteNonQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");

            }
            catch (Exception ex)
            {
                Logger.Error("ExecuteQuery:: Exception during executing command: " + cmnd.CommandText);
                Logger.Error("  ->Error: " + ex.Message);
                errMsg = ex.Message;
                //Logger.Error("  ->" + ex.ToString());
            }


            return dr;
        }
        private void _CloseConnection()
        {
            if (Conn != null)
            {
                if (Conn.State != System.Data.ConnectionState.Closed)
                {
                    try
                    {
                        //Logger.Debug("   ->Connection state before close = " + Conn.State.ToString());
                        Conn.Close();
                    }
                    catch { };
                }
                Conn = null;
                Cmnd = null;
            }
            ConnectionOpen = false;
        }
        private bool CheckConnectionStatus(out string errMsg)
        {
            bool retVal = true;
            errMsg = "";
            if (!ConnectionOpen)
            {
                retVal = OpenConnection(out errMsg);
            }
            return retVal;

        }

        public bool VerifyConnectionStatus(out string errMsg)
        {
            bool retval = false;
            errMsg = "";
            //Logger.Debug("Verify connection to DataBase status...");
            try
            {
                using (DbConnection cn = new DbConnection(ConnectionString))
                {
                    DbCommand cmd = new DbCommand("Select GETDATE()", cn);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    retval = true;
                }


            }
            catch (Exception ex)
            {
                 Logger.Error("Failed to open Connection: " + Caption + ". Reason: " + ex.Message);
                errMsg = ex.Message;
            }
            return retval;
        }

    }
}
