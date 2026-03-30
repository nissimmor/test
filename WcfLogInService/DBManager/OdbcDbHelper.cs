using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Threading;
using System.Diagnostics;
using MCCMLogger;


namespace WcfLogInService
{
    public class CasheOdbcDbHandler
    {
        string ConnectionString = "";
        string ConnectionName = "";
        public string ExternalOutput = "";
        //bool IsActive = false;
        public const string DefTableName = "Result";
        int CommandTimeout = 10;

        
        public CasheOdbcDbHandler(string ConnectionString, string Name,int commandTimeout = 10)
        {
            this.ConnectionString = ConnectionString;
            this.ConnectionName = Name;
            this.CommandTimeout = commandTimeout;
            MccMLogger.Debug("   -> Initialize connection: " + Name);
            MccMLogger.Debug("   Connection string: " + this.ConnectionString);
            //string ExternalOutput = "";
        }
        public OdbcDataReader ExecuteQuery(OdbcCommand cmnd, out string errMsg)
        {
            OdbcDataReader dr = null;
            errMsg = "";

            if (cmnd == null)
            {
                MccMLogger.Error("ExecuteQuery:: Unable to execute query. Command object is Null");
                ExternalOutput = "Command object is Null";
                return dr;
            }
            using (OdbcConnection _conn = new OdbcConnection(this.ConnectionString))
            {
                try
                {
                    _conn.Open();
                    cmnd.Connection = _conn;
                    cmnd.CommandTimeout = this.CommandTimeout
                    ;                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    dr = cmnd.ExecuteReader(CommandBehavior.CloseConnection);
                    sw.Stop();
                    //MccMLogger.Debug("ExecuteNonQuery:: executing command: " + cmnd.CommandText + " recieved " + dr.RecordsAffected + " records");
                    MccMLogger.Debug("ExecuteNonQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");
                }
                catch (Exception ex)
                {
                    MccMLogger.Error("ExecuteQuery:: Exception during executing command: " + cmnd.CommandText);
                    MccMLogger.Error("  ->Error: " + ex.Message);
                    errMsg = ex.Message;
                }
            }

            return dr;
        }

        public ReturnData ExecQuery(OdbcCommand cmnd)
        {
            string ErrMsg = "";
            this.ExternalOutput = "";
            OdbcDataReader sdr = null;
            ReturnData rt = null;
            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                ErrMsg = "Connection string for: " + this.ConnectionName + " is empty";
                MccMLogger.Error(ErrMsg);
                return BuildError(ErrMsg);
            }
            if (cmnd == null)
            {
                ErrMsg = "Input Command object is null";
                MccMLogger.Error(ErrMsg);
                return BuildError(ErrMsg);
            }
            DateTime now = DateTime.Now;
            using (OdbcConnection _conn = new OdbcConnection(this.ConnectionString))
            {
                try
                {
                    _conn.Open();
                    cmnd.Connection = _conn;
                    cmnd.CommandTimeout = this.CommandTimeout;
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    sdr = cmnd.ExecuteReader(CommandBehavior.CloseConnection);
                    sw.Stop();
                    MccMLogger.Debug("ExecQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");
                }
                catch (Exception ex)
                {
                    MccMLogger.Error("ExecuteQuery:: Exception during executing command: " + cmnd.CommandText);
                    MccMLogger.Error("  ->Error: " + ex.Message);
                    ExternalOutput = ex.Message;
                    return BuildError(ex.Message);
                }

                if (sdr == null)
                {
                    //return BuildError(ErrMsg);
                    return BuildError("SDR object is null");
                }
                
                ActivityStatus status = ActivityStatus.Success;
                int rowsCount = 0;
                DataTable dt = null;
                if (sdr.HasRows)
                {
                    dt = new DataTable(DefTableName);
                    dt.Load(sdr);
                    rowsCount = dt.Rows.Count;
                }
                rt = new ReturnData(new StatusObj(status, now, rowsCount,""),
                    new DataObj(dt));
            }
            return rt;
        }

        public bool CheckConnectivity(int timeout_ms, out string ErrMsg)
        {
            ErrMsg = "";
            string mm = "";
            bool retVal = false;
            MccMLogger.Debug("  Check connecivity of connection: " + ConnectionName + " ...");
            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                ErrMsg = "Connection string is empty";
                return false;
            }
            // We'll use a Stopwatch here for simplicity. A comparison to a stored DateTime.Now value could also be used
		    Stopwatch sw = new Stopwatch();
		    //bool connectSuccess = false;
            using (OdbcConnection _conn = new OdbcConnection(this.ConnectionString))
            {
                // Try to open the connection, if anything goes wrong, make sure we set connectSuccess = false
                Thread t = new Thread(delegate()
                {
                    try
                    {
                        sw.Start();
                        _conn.Open();
                        retVal = true;
                    }
                    catch (Exception ex)
                    {
                        mm = ex.Message;
                        MccMLogger.Error("  Failed to connect DB: " + ex.Message);
                    }
                });

                // Make sure it's marked as a background thread so it'll get cleaned up automatically
                t.IsBackground = true;
                t.Start();

                // Keep trying to join the thread until we either succeed or the timeout value has been exceeded
                while (timeout_ms > sw.ElapsedMilliseconds)
                    if (t.Join(1))
                        break;
            }
            ErrMsg = mm;
            if ((retVal== false) && string.IsNullOrWhiteSpace(mm)) // timeout without error
            {
                MccMLogger.Error("  Connect timeout for " + ConnectionName);
            }
            return retVal;
        }
       
        public ReturnData ExecNonQuery(OdbcCommand cmnd)
        {
            string ErrMsg = "";
            this.ExternalOutput = "";
            ReturnData rt = null;
            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                ErrMsg = "Connection string for: " + this.ConnectionName + " is empty";
                MccMLogger.Error(ErrMsg);
                return BuildError(ErrMsg);
            }
            if (cmnd == null)
            {
                ErrMsg = "Input Command object is null";
                MccMLogger.Error(ErrMsg);
                return BuildError(ErrMsg);
            }
            int rowsCount = 0;
            DateTime now = DateTime.Now;
            using (OdbcConnection _conn = new OdbcConnection(this.ConnectionString))
            {
                try
                {
                    _conn.Open();
                    cmnd.Connection = _conn;
                    cmnd.CommandTimeout = this.CommandTimeout;
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    rowsCount = cmnd.ExecuteNonQuery();
                    sw.Stop();
                    MccMLogger.Debug("ExecNonQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");
                }
                catch (Exception ex)
                {
                    MccMLogger.Error("ExecuteNonQuery:: Exception during executing command: " + cmnd.CommandText);
                    MccMLogger.Error("  ->Error: " + ex.Message);
                    ExternalOutput = ex.Message;
                    return BuildError(ex.Message);
                }

                ActivityStatus status = ActivityStatus.Success;
                rt = new ReturnData(new StatusObj(status, now, rowsCount,""),
                    new DataObj(null));
            }
            return rt;
        }

        public bool ExecNonQuery(OdbcCommand cmnd, out string ErrMsg)
        {
            ErrMsg = "";
            this.ExternalOutput = "";

            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                ErrMsg = "Connection string for: " + this.ConnectionName + " is empty";
                MccMLogger.Error(ErrMsg);
                return false;
            }
            if (cmnd == null)
            {
                ErrMsg = "Input Command object is null";
                MccMLogger.Error(ErrMsg);
                return false;
            }
            int rowsCount = 0;
            DateTime now = DateTime.Now;
            using (OdbcConnection _conn = new OdbcConnection(this.ConnectionString))
            {
                try
                {
                    _conn.Open();
                    cmnd.Connection = _conn;
                    cmnd.CommandTimeout = this.CommandTimeout;
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                    rowsCount = cmnd.ExecuteNonQuery();
                    sw.Stop();
                    MccMLogger.Debug("ExecNonQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");
                }
                catch (Exception ex)
                {
                    MccMLogger.Error("ExecuteNonQuery:: Exception during executing command: " + cmnd.CommandText);
                    MccMLogger.Error("  ->Error: " + ex.Message);
                    ExternalOutput = ex.Message;
                    return false;
                }
            }
            return true;
        }

       

        private ReturnData BuildError(string ErrMsg)
        {
            ReturnData rt = new ReturnData();
            rt.SetError(ErrMsg);
            return rt;
        }
    
       /* public bool OpenConnection(out string errMsg)
        {
            bool retval = false;
            errMsg = "";
            if (Conn != null)
            {
                _CloseConnection();
            }
            MccMLogger.Debug("   ->Open Connection to DB...");
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                MccMLogger.Error("   ->Failed to open connecton. connection string is empty");
                ExternalOutput = "connection string is empty";
                return false;
            }

            try
            {
                Conn = new  OdbcConnection(ConnectionString);
                Conn.Open();
                //OdbcCommand cmd = new  OdbcCommand("Select GETDATE()", Conn);
                //cmd.ExecuteNonQuery();
                retval = true;
                ConnectionOpen = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("OpenConnection:: Failed to open Connection ... Reason: " + ex.Message);
                ExternalOutput = "Failed to open Connection ... Reason: " + ex.Message;
                ConnectionOpen = false;
                errMsg = ex.Message;
            }
            return retval;
        }*/
       /* public void CloseConnection()
        {
            _CloseConnection();
        }*/

        /*public bool ExecuteNonQuery(OdbcCommand cmnd, out int EffectedRecord, out string errMsg)
        {
            bool retVal = false;
            errMsg = "";
            EffectedRecord = 0;
            if (!CheckConnectionStatus(out errMsg))
            {
                MccMLogger.Error("ExecuteNonQuery:: Unable to execute query due to connection problem");
                ExternalOutput = Environment.NewLine + "Unable to execute query due to connection problem"  + Environment.NewLine;
                return false;
            }

            if (cmnd == null)
            {
                MccMLogger.Error("ExecuteNonQuery:: Unable to execute query. Command object is Null");
                return false;
            }
            try
            {
                cmnd.Connection = Conn;
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                EffectedRecord = cmnd.ExecuteNonQuery();
                sw.Stop();
                ExternalOutput = Environment.NewLine + " ... Executing command: " + cmnd.CommandText  + Environment.NewLine;

                MccMLogger.Debug("ExecuteNonQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");

                retVal = true;
            }
            catch (Exception ex)
            {
                MccMLogger.Error("ExecuteNonQuery:: Exception during executing command: " + cmnd.CommandText);
                MccMLogger.Error("  ->Error: " + ex.Message);
                //MccMLogger.Error("  ->" + ex.ToString());
                ExternalOutput = Environment.NewLine + "Failed to execute command: " + cmnd.CommandText;
                ExternalOutput += Environment.NewLine  + "Error: " + ex.Message + Environment.NewLine;

                retVal = false;
            }


            return retVal;
        }*/
        /*public OdbcDataReader ExecuteQuery(OdbcCommand cmnd, out string errMsg)
        {
            OdbcDataReader dr = null;
            errMsg = "";
            if (!CheckConnectionStatus(out errMsg))
            {
                MccMLogger.Error("ExecuteQuery:: Unable to execute query due to connection problem");
                ExternalOutput = "Unable to execute query due to connection problem" + Environment.NewLine;
                return dr;
            }

            if (cmnd == null)
            {
                MccMLogger.Error("ExecuteQuery:: Unable to execute query. Command object is Null");
                return dr;
            }
            try
            {
                cmnd.Connection = Conn;
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                dr = cmnd.ExecuteReader();
                sw.Stop();
                //MccMLogger.Debug("ExecuteNonQuery:: executing command: " + cmnd.CommandText + " recieved " + dr.RecordsAffected + " records");
                MccMLogger.Debug("ExecuteQuery:: executing command: " + cmnd.CommandText + " took " + sw.ElapsedMilliseconds.ToString() + "ms.");

            }
            catch (Exception ex)
            {
                MccMLogger.Error("ExecuteQuery:: Exception during executing command: " + cmnd.CommandText);
                MccMLogger.Error("  ->Error: " + ex.Message);
                //MccMLogger.Error("  ->" + ex.ToString());
                ExternalOutput = Environment.NewLine + "Failed to execute command: " + cmnd.CommandText;
                ExternalOutput += Environment.NewLine + "Error: " + ex.Message + Environment.NewLine;
            }


            return dr;
        }*/

       /* private void _CloseConnection()
        {
            if (Conn != null)
            {
                if (Conn.State != System.Data.ConnectionState.Closed)
                {
                    try
                    {
                        //MccMLogger.Debug("   ->Connection state before close = " + Conn.State.ToString());
                        Conn.Close();
                    }
                    catch { };
                }
                Conn = null;
                Cmnd = null;
            }
            ConnectionOpen = false;
        }*/
        /*private bool CheckConnectionStatus(out string ErrMsg)
        {
            bool retVal = true;
            ErrMsg = "";
            if (!ConnectionOpen)
            {
                retVal = OpenConnection(out ErrMsg);
            }
            return retVal;

        }*/

        /*public bool VerifyConnectionStatus()
        {

            bool retval = false;
            try
            {
                using (OdbcConnection cn = new OdbcConnection(ConnectionString))
                {
                    OdbcCommand cmd = new OdbcCommand("Select GETDATE()", cn);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    retval = true;
                }


            }
            catch (Exception ex)
            {
                MccMLogger.Error("Failed to open Connection: " + Caption + ". Reason: " + ex.Message);
            }
            return retval;
        }
         * */


    }
}
