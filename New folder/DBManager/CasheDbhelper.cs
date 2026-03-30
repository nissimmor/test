using System;
using System.Data;
using InterSystems.Data.CacheClient;
using System.Diagnostics;
using System.Threading;
using MCCMLogger;

namespace WcfLogInService
{
    public class CasheDbHandler
    {
        string ConnectionString = "";
        string ConnectionName = "";
        public string ExternalOutput = "";
        //bool IsActive = false;
        public const string DefTableName = "Result";
        int CommandTimeout = 10;


        public CasheDbHandler(string ConnectionString, string Name, int commandTimeout = 10)
        {
            this.ConnectionString = ConnectionString;
            this.ConnectionName = Name;
            this.CommandTimeout = commandTimeout;
            //MccMLogger.Debug("   -> Initialize connection: " + Name);
            //MccMLogger.Debug("   Connection string: " + this.ConnectionString);
            //string ExternalOutput = "";
        }
        public CacheDataReader ExecuteQuery(CacheCommand cmnd, out string errMsg)
        {
            CacheDataReader dr = null;
            errMsg = "";

            if (cmnd == null)
            {
                MccMLogger.Error("ExecuteQuery:: Unable to execute query. Command object is Null");
                ExternalOutput = "Command object is Null";
                return dr;
            }
            using (CacheConnection _conn = new CacheConnection(this.ConnectionString))
            {
                try
                {
                    _conn.Open();
                    cmnd.Connection = _conn;
                    cmnd.CommandTimeout = this.CommandTimeout
                    ; System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
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

        /* public ReturnData ExecQuery(CacheCommand cmnd)
         {
             string ErrMsg = "";
             this.ExternalOutput = "";
             CacheDataReader sdr = null;
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
             using (CacheConnection _conn = new CacheConnection(this.ConnectionString))
             {
                 try
                 {
                     _conn.Open();
                     cmnd.Connection = _conn;
                     cmnd.CommandTimeout = this.CommandTimeout;
                     System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                     sdr = cmnd.ExecuteReader(CommandBehavior.CloseConnection);
                     //sdr = cmnd.ExecuteReader();
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
                     using (CacheDataAdapter Adapter = new CacheDataAdapter(cmnd.CommandText, _conn))
                     {
                         Adapter.Fill(dt);
                        
                     }
                     rowsCount = dt.Rows.Count;
                 }

                 //if (sdr.HasRows) // not working with cashe client
                 //{
                 //    dt = new DataTable(DefTableName);
                 //    dt.Load(sdr);
                 //      rowsCount = dt.Rows.Count;
                 //}
                 
                 rt = new ReturnData(new StatusObj(status, now, rowsCount,""),
                     new DataObj(dt));
             }
             return rt;
         }*/

        public ReturnData ExecQuery(CacheCommand cmnd)
        {
            string ErrMsg = "";
            this.ExternalOutput = "";
            ReturnData rt = null;

            ActivityStatus status = ActivityStatus.Fail;
            int rowsCount = 0;
            DataTable dt = null;

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
            using (CacheConnection _conn = new CacheConnection(this.ConnectionString))
            {
                try
                {
                    _conn.Open();
                    cmnd.Connection = _conn;
                    cmnd.CommandTimeout = this.CommandTimeout;
                    System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                    dt = new DataTable(DefTableName);
                    using (CacheDataAdapter Adapter = new CacheDataAdapter(cmnd))
                    {
                        Adapter.Fill(dt);
                        status = ActivityStatus.Success;
                    }

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

                if (dt == null)
                {
                    return BuildError("Table object is null");
                }

                if (dt.Rows.Count > 0)
                {
                    rowsCount = dt.Rows.Count;
                }
                rt = new ReturnData(new StatusObj(status, now, rowsCount, ""),
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
            try
            {
                using (CacheConnection _conn = new CacheConnection(this.ConnectionString))
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

            }
            catch (Exception ex)
            {
                mm = ex.Message;
                MccMLogger.Error("  Failed to connect DB: " + ex.Message);
            }
            ErrMsg = mm;
            if ((retVal == false) && string.IsNullOrWhiteSpace(mm)) // timeout without error
            {
                MccMLogger.Error("  Connect timeout for " + ConnectionName);
            }


            return retVal;
        }

        public ReturnData ExecNonQuery(CacheCommand cmnd)
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
            using (CacheConnection _conn = new CacheConnection(this.ConnectionString))
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
                rt = new ReturnData(new StatusObj(status, now, rowsCount, ""),
                    new DataObj(null));
            }
            return rt;
        }

        public bool ExecNonQuery(CacheCommand cmnd, out string ErrMsg)
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
            using (CacheConnection _conn = new CacheConnection(this.ConnectionString))
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
    }
}
