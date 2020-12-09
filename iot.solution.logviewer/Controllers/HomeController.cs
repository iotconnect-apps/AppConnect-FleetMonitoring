using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using component.services.logger.viewer.Models.LoggerModel;
using Microsoft.Extensions.Configuration;

namespace component.services.logger.viewer.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuration { get; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GetConnectionString(string environment)
        {
            return Configuration["ConnectionStrings:" + environment];
        }

        public ActionResult Index(int connId = 0)
        {
            LoggerModel List = new LoggerModel();
            if (connId == 0)
            {
                List.ConStringId = 1;
            }
            else
            {
                List.ConStringId = connId;
            }
            List.HoursList = ((Common.HoursBeforeEnum[])Enum.GetValues(typeof(Common.HoursBeforeEnum))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();
            List.ConnectionList = ((Common.ConnectionStringName[])Enum.GetValues(typeof(Common.ConnectionStringName))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();
            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), List.ConStringId);
            List.Hours = (int)Common.HoursBeforeEnum.OneHours;
            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetLoggerCount");
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@hours";
                param.Value = 1;
                cmd.Parameters.Add(param);

                cmd.Connection = conn;
                cmd.CommandTimeout = 120;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    if (Convert.ToInt32(dr["SeverityCount"].ToString()) > 0)
                    {
                        Loggers obj = new Loggers();
                        obj.Logger = dr["Logger"].ToString();
                        obj.LoggerCount = Convert.ToInt32(dr["LoggerCount"].ToString());

                        if (List.LoggerList.Where(t => t.Logger == obj.Logger).Count() == 0)
                        {
                            List.LoggerList.Add(obj);
                        }
                        else
                        {
                            List.LoggerList.Where(t => t.Logger == obj.Logger).ToList()[0].LoggerCount += Convert.ToInt32(dr["SeverityCount"].ToString());
                        }
                        severities Objs = new severities();
                        Objs.Logger = dr["Logger"].ToString();
                        Objs.Severity = dr["Severity"].ToString();
                        Objs.SeverityCount = Convert.ToInt32(dr["SeverityCount"].ToString());
                        List.SeverityList.Add(Objs);
                    }
                }
            }

            List.ApplicationList = GetApplicationList(List.ConStringId, List.ApplicationCode);

            return View(List);
        }

        [HttpPost]
        public ActionResult Index(LoggerModel Model)
        {
            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), Model.ConStringId);

            LoggerModel List = new LoggerModel();
            List.HoursList = ((Common.HoursBeforeEnum[])Enum.GetValues(typeof(Common.HoursBeforeEnum))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();
            List.ConnectionList = ((Common.ConnectionStringName[])Enum.GetValues(typeof(Common.ConnectionStringName))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetLoggerCount");
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@hours";
                param.Value = Model.Hours;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = Model.ApplicationCode ?? string.Empty;
                cmd.Parameters.Add(param);

                cmd.Connection = conn;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    if (Convert.ToInt32(dr["SeverityCount"].ToString()) > 0)
                    {
                        Loggers obj = new Loggers();
                        obj.Logger = dr["Logger"].ToString();
                        obj.LoggerCount = Convert.ToInt32(dr["LoggerCount"].ToString());

                        if (List.LoggerList.Where(t => t.Logger == obj.Logger).Count() == 0)
                        {
                            List.LoggerList.Add(obj);
                        }
                        else
                        {
                            List.LoggerList.Where(t => t.Logger == obj.Logger).ToList()[0].LoggerCount += Convert.ToInt32(dr["SeverityCount"].ToString());
                        }
                        severities Objs = new severities();
                        Objs.Logger = dr["Logger"].ToString();
                        Objs.Severity = dr["Severity"].ToString();
                        Objs.SeverityCount = Convert.ToInt32(dr["SeverityCount"].ToString());
                        List.SeverityList.Add(Objs);
                    }
                }
            }

            List.ApplicationList = GetApplicationList(Model.ConStringId, Model.ApplicationCode);

            return View(List);
        }

        public ActionResult ErrorList(string logger, string severity, int hours = 0, int connId = 0)
        {
            string applicationCode = logger.Split('_')[0].Trim();
            // logger = logger.Split('_')[1].Trim();

            ErrorModel lstErrors = new ErrorModel();
            lstErrors.Logger = logger;
            lstErrors.ConStringId = connId;
            lstErrors.Severity = severity;
            lstErrors.Hours = hours;
            lstErrors.HoursList = ((Common.HoursBeforeEnum[])Enum.GetValues(typeof(Common.HoursBeforeEnum))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();

            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), lstErrors.ConStringId);

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetSeverityListByLogger");

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@logger";
                param.Value = logger;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = applicationCode;
                cmd.Parameters.Add(param);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Connection = conn;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = dr["Severity"].ToString();
                    item.Value = dr["Severity"].ToString();
                    lstErrors.SeverityList.Add(item);
                }
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetErrorList");

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@logger";
                param.Value = logger;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@severity";
                param.Value = severity;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@hours";
                param.Value = hours;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = applicationCode;
                cmd.Parameters.Add(param);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Connection = conn;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    Error Objerror = new Error();
                    Objerror.LogId = dr["LogId"].ToString();
                    Objerror.ApplicationCode = dr["ApplicationCode"].ToString();
                    Objerror.ComponentStatus = dr["ComponentStatus"].ToString();
                    Objerror.ErrorCode = dr["ErrorCode"].ToString();
                    Objerror.Exception = dr["Exception"].ToString();
                    Objerror.LogFile = dr["LogFile"].ToString();
                    Objerror.Identity = dr["Identity"].ToString();
                    Objerror.LogDate = Convert.ToDateTime(dr["LogDate"].ToString());
                    Objerror.Logger = dr["Logger"].ToString();
                    Objerror.Message = dr["Message"].ToString();
                    Objerror.MessageData = dr["MessageData"].ToString();
                    Objerror.Method = dr["Method"].ToString();
                    Objerror.Severity = dr["Severity"].ToString();
                    Objerror.StackTrace = dr["StackTrace"].ToString();
                    lstErrors.ErrorList.Add(Objerror);
                }
            }

            lstErrors.ApplicationList = GetApplicationList(lstErrors.ConStringId, applicationCode);
            lstErrors.ApplicationCode = applicationCode;

            return View(lstErrors);
        }

        [HttpPost]
        public ActionResult ErrorList(ErrorModel model)
        {
            ErrorModel lstErrors = new ErrorModel();
            lstErrors.Logger = model.Logger;
            lstErrors.Severity = model.Severity;
            lstErrors.Hours = model.Hours;
            lstErrors.ConStringId = model.ConStringId;
            lstErrors.HoursList = ((Common.HoursBeforeEnum[])Enum.GetValues(typeof(Common.HoursBeforeEnum))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();

            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), lstErrors.ConStringId);

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetSeverityListByLogger");

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@logger";
                param.Value = model.Logger;//.Split('_')[1].Trim();
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = model.ApplicationCode;
                cmd.Parameters.Add(param);


                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = dr["Severity"].ToString();
                    item.Value = dr["Severity"].ToString();
                    lstErrors.SeverityList.Add(item);
                }
            }

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetErrorList");

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@logger";
                param.Value = model.Logger;//.Split('_')[1].Trim();
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@severity";
                param.Value = model.Severity;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@hours";
                param.Value = model.Hours;
                cmd.Parameters.Add(param);

                param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = model.ApplicationCode;
                cmd.Parameters.Add(param);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    Error Objerror = new Error();
                    Objerror.LogId = dr["LogId"].ToString();
                    Objerror.ApplicationCode = dr["ApplicationCode"].ToString();
                    Objerror.ComponentStatus = dr["ComponentStatus"].ToString();
                    Objerror.ErrorCode = dr["ErrorCode"].ToString();
                    Objerror.Exception = dr["Exception"].ToString();
                    Objerror.LogFile = dr["LogFile"].ToString();
                    Objerror.Identity = dr["Identity"].ToString();
                    Objerror.LogDate = Convert.ToDateTime(dr["LogDate"].ToString());
                    Objerror.Logger = dr["Logger"].ToString();
                    Objerror.Message = dr["Message"].ToString();
                    Objerror.MessageData = dr["MessageData"].ToString();
                    Objerror.Method = dr["Method"].ToString();
                    Objerror.Severity = dr["Severity"].ToString();
                    Objerror.StackTrace = dr["StackTrace"].ToString();
                    lstErrors.ErrorList.Add(Objerror);
                }
            }

            lstErrors.ApplicationList = GetApplicationList(lstErrors.ConStringId, model.ApplicationCode);
            lstErrors.ApplicationCode = model.ApplicationCode;

            return View(lstErrors);
        }

        [HttpGet]
        public ActionResult GetDetailError(string ErrorId, int ConnId)
        {
            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), ConnId);

            Error model = new Error();

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetErrorDetail");

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Id";
                param.Value = ErrorId;
                cmd.Parameters.Add(param);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    model.LogId = dr["LogId"].ToString();
                    model.ApplicationCode = dr["ApplicationCode"].ToString();
                    model.ComponentStatus = dr["ComponentStatus"].ToString();
                    model.ErrorCode = dr["ErrorCode"].ToString();
                    model.Exception = dr["Exception"].ToString();
                    model.LogFile = dr["LogFile"].ToString();
                    model.Identity = dr["Identity"].ToString();
                    model.LogDate = Convert.ToDateTime(dr["LogDate"].ToString());
                    model.Logger = dr["Logger"].ToString();
                    model.Message = dr["Message"].ToString();
                    model.MessageData = dr["MessageData"].ToString();
                    model.Method = dr["Method"].ToString();
                    model.Severity = dr["Severity"].ToString();
                    model.StackTrace = dr["StackTrace"].ToString();
                }
            }

            return PartialView("_GetDetailError", model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        private IList<SelectListItem> GetApplicationList(int conStringId, string applicationCode = "")
        {
            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), conStringId);
            List<SelectListItem> listItems = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetApplicationList");
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Connection = conn;
                conn.Open();
                SqlDataAdapter MyDataAdapter = new SqlDataAdapter();
                MyDataAdapter.SelectCommand = cmd;
                DataSet oDataset = new DataSet();
                DataTable oDatatable = new DataTable();
                cmd.ExecuteNonQuery();
                MyDataAdapter.Fill(oDataset);
                oDatatable = oDataset.Tables[0];
                conn.Close();

                foreach (DataRow dr in oDatatable.Rows)
                {
                    if (!listItems.Any(t => t.Value == dr["ApplicationCode"].ToString()))
                    {
                        listItems.Add(new SelectListItem
                        {
                            Text = dr["ApplicationCode"].ToString(),
                            Value = dr["ApplicationCode"].ToString(),
                            Selected = applicationCode != string.Empty && applicationCode != null ? string.Equals(applicationCode, dr["ApplicationCode"].ToString(), StringComparison.OrdinalIgnoreCase) : false
                        });
                    }
                }

                bool IsApplicationSelected = listItems.Any(x => x.Selected);
                listItems.Insert(0, new SelectListItem { Text = "All", Value = "All", Selected = !IsApplicationSelected });
            }

            return listItems;
        }
    }
}
