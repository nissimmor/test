using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCCMLogger;

namespace WcfLogInService
{
    public class DataObj
    {
       DataTable _dt = null;

        public DataObj( DataTable dataTable)
        {
            _dt = dataTable;
        }
       
        public DataTable Data
        { 
            get
            {
                return _dt;
            }
        }
        
        public string GetField (string FieldName)
        {
            if (_dt == null)
            {
                throw new ArgumentNullException("Data object is null");
            }
            if (_dt.Rows.Count >1) // not relevant
            {
                MccMLogger.Error("DataObj::getField works only only datatable which contain one record only. ");
                return "";
            }
            if(_dt.Columns.Contains(FieldName))
            {
                return _dt.Rows[0][FieldName].ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetField (int FieldID)
        {
            if (_dt == null)
            {
                throw new ArgumentNullException("Data object is null");
            }
            if (_dt.Rows.Count >1) // not relevant
            {
                MccMLogger.Error("DataObj::getField works only only datatable which contain one record only. ");
                return "";
            }
            if(_dt.Columns.Count > FieldID) // field ID exist
            {
                return _dt.Rows[0][FieldID].ToString();
            }
            else
            {
                return "";
            }
        }

         public string GetField (int RecordID, string FieldName)
        {
            if (_dt == null)
            {
                throw new ArgumentNullException("Data object is null");
            }
            if (_dt.Rows.Count < RecordID) // not relevant
            {
                MccMLogger.Error("DataObj::record #" + RecordID.ToString() + " doesnt exist in table");
                return "";
            }
            if(_dt.Columns.Contains(FieldName))
            {
                return _dt.Rows[RecordID][FieldName].ToString();
            }
            else
            {
                return "";
            }
        }
        public void printData()
        {
            if (_dt == null)
            {
                   MccMLogger.Error("DataObj::printData - Data object is has no results");
                return ;
            }
            /*  remove Column list
              StringBuilder sb = new StringBuilder("print data object" + Environment.NewLine + "Columns: ")  ;
             int ii = 0;
             foreach (DataColumn c in _dt.Columns)
             {
                 if (ii > 0)
                         sb.Append(", ");
                 sb.Append(c.ColumnName + "(" + c.DataType.ToString() + ")" );
                 ii++;
             }
             */
            StringBuilder sb = new StringBuilder("Recieve Data object:");
            int j = 1;
            foreach (DataRow d in _dt.Rows)
            {
                sb.Append( Environment.NewLine + "Row #"+ j.ToString() + ": ");
                for (int i = 0; i < _dt.Columns.Count; i++)
                {
                    if (i > 0)
                        sb.Append(" ; ");
                    sb.Append(d[i].ToString());
                }
                j++;
            }
            MccMLogger.Debug(sb.ToString());
            
        }
    }

    public enum ActivityStatus
    {
        Unknown = 0,
        Success = 1,
        Fail    = 2
    }


    public enum CampaignType
    {
        Preview = 1,
        Massive = 2,
        Callback= 4,
        Abandon = 8,
        Proxy   = 16
    }
    public class StatusObj
    {
        private ActivityStatus _status;
        private DateTime _ExecutionTime;
        private int _RowsCount;
        private string _Description;

        public StatusObj()
        {
            _status = ActivityStatus.Unknown;
            _RowsCount = 0;
            _Description = "";
        }

        public StatusObj (ActivityStatus status, DateTime ExecutionTime, int RowsCount, string Description)
        {
            this._status = status;
            this._ExecutionTime = ExecutionTime;
            this._RowsCount = RowsCount;
            this._Description = Description;
        }
        public ActivityStatus Status
        {
            get
            {
                return _status;
            }
        }
        public DateTime ExecutionTime
        {
            get
            {
                return _ExecutionTime;
            }
        }
        public int RowsCount
        {
            get
            {
                return _RowsCount;
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
        }

    }

    public class ReturnData
    {
        StatusObj _status;
        DataObj _data;

        public ReturnData( StatusObj status, DataObj data)
        {
            this._status = status;
            this._data = data;
        }

        public ReturnData()
        {
            this._status = null;
            this._data = null;;
        }
        
        public StatusObj StatusInfo
        {
            get
            {
                return this._status;
            }
        }

        public DataObj DataInfo
        {
            get
            {
                return this._data;
            }
        }

        public void SetError( string ErrMsg)
        {
            this._status = new StatusObj(ActivityStatus.Fail,
                                        DateTime.Now,
                                        0,
                                        ErrMsg);
        }

    }
   
}
