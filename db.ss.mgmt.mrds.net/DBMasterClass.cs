using Microsoft.Data.SqlClient;
using System.Data;

namespace db.mrds.net
{
    public class DBMasterClass
    {
        private readonly DatabaseParameter _databaseParameter;

        public DBMasterClass(DatabaseParameter databaseParameter)
        {
            _databaseParameter = databaseParameter;
        }

        public string SelectOneValue(SqlCommand Query)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }

                using var DBConn = new SqlConnection(_databaseParameter.GeConnectionString());
                DBConn.Open();

                if (DBConn.State == ConnectionState.Open)
                {
                    var SelectValue = Query;
                    SelectValue.Connection = DBConn;
                    SelectValue.CommandTimeout = 7200;
                    object ReturnedValue;
                    ReturnedValue = SelectValue.ExecuteScalar();
                    SelectValue.Dispose();
                    if (ReturnedValue != null)
                    {
                        return ReturnedValue.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                throw;
            }
        }

        public string InsertUpdateOneValueWithTVP(SqlCommand sqlCommand)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }
                switch (_databaseParameter.GetConnectionType())
                {
                    case ConnectionType.NET:
                        {
                            using var DBConn = new SqlConnection(_databaseParameter.GeConnectionString());
                            DBConn.Open();

                            if (DBConn.State == ConnectionState.Open)
                            {
                                var InsertUpdateValue = sqlCommand;
                                InsertUpdateValue.Connection = DBConn;
                                InsertUpdateValue.CommandTimeout = 7200;
                                object? ReturnedValue = null;
                                ReturnedValue = InsertUpdateValue.ExecuteScalar();
                                InsertUpdateValue.Dispose();
                                if (ReturnedValue != null)
                                {
                                    return ReturnedValue.ToString();
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }
                            break;
                        }
                }
                return string.Empty;
            }
            catch
            {
                throw;
            }
        }

        public string ExecNonQuery(SqlCommand Query)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }

                using var DBConn = new SqlConnection(_databaseParameter.GeConnectionString());
                DBConn.Open();

                if (DBConn.State == ConnectionState.Open)
                {
                    var InsertUpdateValue = Query;
                    InsertUpdateValue.CommandTimeout = 7200;
                    InsertUpdateValue.Connection = DBConn;
                    object? ReturnedValue = null;
                    ReturnedValue = InsertUpdateValue.ExecuteNonQuery();
                    InsertUpdateValue.Dispose();
                    if (ReturnedValue != null)
                    {
                        return ReturnedValue.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                return string.Empty;
            }
            catch
            {
                throw;
            }
        }


        public string InserUpdateOneValue(SqlCommand Query)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }

                using var DBConn = new SqlConnection(_databaseParameter.GeConnectionString());
                DBConn.Open();

                if (DBConn.State == ConnectionState.Open)
                {
                    var InsertUpdateValue = Query;
                    InsertUpdateValue.CommandTimeout = 7200;
                    InsertUpdateValue.Connection = DBConn;
                    object? ReturnedValue = null;
                    ReturnedValue = InsertUpdateValue.ExecuteScalar();
                    InsertUpdateValue.Dispose();
                    if (ReturnedValue != null)
                    {
                        return ReturnedValue.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                return string.Empty;
            }
            catch
            {
                throw;
            }
        }

        public string InserUpdateOneValueInTransaction(SqlCommand Query)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }

                if (Query.Connection.State == ConnectionState.Open)
                {
                    var InsertUpdateValue = Query;
                    InsertUpdateValue.CommandTimeout = 7200;
                    object? ReturnedValue = InsertUpdateValue.ExecuteScalar();

                    if (ReturnedValue != null)
                    {
                        return ReturnedValue.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    throw new ArgumentException("Connection is not opened");
                }
            }
            catch
            {
                throw;
            }
        }

        public DataTable? GetDataTableTVP(SqlCommand sqlCommand)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }
                switch (_databaseParameter.GetConnectionType())
                {
                    case ConnectionType.NET:
                        {
                            using var DBConn = new SqlConnection(_databaseParameter.GeConnectionString());
                            DBConn.Open();

                            if (DBConn.State == ConnectionState.Open)
                            {

                                var gDT = sqlCommand;
                                gDT.CommandTimeout = 7200;
                                gDT.Connection = DBConn;
                                var ODA = new SqlDataAdapter(gDT);
                                if (ODA != null)
                                {
                                    var ResultTable = new DataTable("ResultTable");
                                    ODA.Fill(ResultTable);
                                    ODA.Dispose();
                                    return ResultTable;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
            catch
            {
                throw;
            }
        }



        public DataTable? GetDataTable(SqlCommand Query)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }
                using var DBConn = new SqlConnection(_databaseParameter.GeConnectionString());

                DBConn.Open();

                if (DBConn.State == ConnectionState.Open)
                {
                    Query.CommandTimeout = 7200;
                    Query.Connection = DBConn;

                    var ODA = new SqlDataAdapter(Query);

                    if (ODA != null)
                    {
                        var ResultTable = new DataTable("ResultTable");
                        ODA.Fill(ResultTable);
                        ODA.Dispose();
                        return ResultTable;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public DataTable? GetDataTableReport(SqlCommand Query, int Timeout)
        {
            try
            {
                if (!CheckDBParam())
                {
                    throw new NullReferenceException("Before you use DBMasterClass please fill DBParam object with correct information.");
                }
                using var DBConn = new SqlConnection(_databaseParameter.GeConnectionString());
                DBConn.Open();

                if (DBConn.State == ConnectionState.Open)
                {
                    Query.CommandTimeout = Timeout;
                    Query.Connection = DBConn;

                    var ODA = new SqlDataAdapter(Query);

                    if (ODA != null)
                    {
                        var ResultTable = new DataTable("ResultTable");
                        ODA.Fill(ResultTable);
                        ODA.Dispose();
                        return ResultTable;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        private bool CheckDBParam()
        {
            if (_databaseParameter == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}