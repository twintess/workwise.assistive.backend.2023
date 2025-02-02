using db.mrds.net;
using Microsoft.Data.SqlClient;
using System.Data;
using utils.mrds.net;
using workwise.assistive.backend.Model;

namespace workwise.assistive.backend.Service
{
    public class PopupService
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        private readonly string _connectionString;
        public PopupService(ILogger<UserService> logger)
        {
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DB");
        }

        public IEnumerable<PopupScheduleResponse> GetPopupSchedule(DateTime from, DateTime to)
        {
            var result = new List<PopupScheduleResponse>();

            try
            {
                DataTable? scheduleTable;
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = new SqlCommand("GetPopupSchedule", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Parameters =
                        {
                            new SqlParameter("from", from),
                            new SqlParameter("to", to)
                        }
                    };

                    var dbm = new DBMasterClass(new DatabaseParameter(connection.ConnectionString, ConnectionType.NET));
                    scheduleTable = dbm.GetDataTable(query);
                }

                if (scheduleTable != null)
                {
                    foreach (DataRow row in scheduleTable.Rows)
                    {
                        var item = new PopupScheduleResponse()
                        {
                            Id = row.Field<int>("EventID"),
                            Title = DbUtils.CheckString(row.Field<string>("Name")),
                            Start = row.Field<DateTime>("ExecuteDate").ToString("yyyy-MM-dd HH:mm:ss"),
                            End = row.Field<DateTime>("ExecuteDate").ToString("yyyy-MM-dd HH:mm:ss"),
                            Enabled = row.Field<bool>("Enabled")
                        };

                        result.Add(item);
                    }
                }

                return result;
            }
            catch(Exception exc)
            {
                _logger.LogError(exc.ToString());
                return result;
            }
        }

        public IEnumerable<PopupDetailsResponse> GetPopupList(string username)
        {
            var result = new List<PopupDetailsResponse>();

            try
            {
                DataTable? scheduleTable;
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = new SqlCommand("GetPopupList", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Parameters =
                        {
                            new SqlParameter("@authorLogin", username)
                        }
                    };

                    var dbm = new DBMasterClass(new DatabaseParameter(connection.ConnectionString, ConnectionType.NET));
                    scheduleTable = dbm.GetDataTable(query);
                }

                if (scheduleTable != null)
                {
                    foreach (DataRow row in scheduleTable.Rows)
                    {
                        var item = new PopupDetailsResponse()
                        {
                            EventId = row.Field<int>("EventID"),
                            Name = DbUtils.CheckString(row.Field<string>("Name")),
                            Enabled = row.Field<bool>("Enabled"),
                            CreatorFirstName = DbUtils.CheckString(row.Field<string>("creatorFirstName")),
                            CreatorLastName = DbUtils.CheckString(row.Field<string>("creatorLastName")),
                            CreatorUsername= DbUtils.CheckString(row.Field<string>("creatorUsername")),
                            InsertDate = row.Field<DateTime>("InsertDate").ToString("dd-MM-yyyy, HH:mm:ss")
                        };

                        result.Add(item);
                    }
                }

                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());
                return result;
            }
        }
    }
}
