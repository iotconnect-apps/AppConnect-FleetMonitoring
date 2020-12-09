using component.services.logger.viewer.Models.SettingModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace component.services.logger.viewer.Controllers
{
    public class SettingController : Controller
    {
        public IConfiguration Configuration { get; }
        public SettingController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GetConnectionString(string environment)
        {
            return Configuration["ConnectionStrings:" + environment];
        }

        // GET: Setting
        public ActionResult Index()
        {
            SettingModel ConfigurationList = new SettingModel();
            ConfigurationList.ConnectionList = ((Common.ConnectionStringName[])Enum.GetValues(typeof(Common.ConnectionStringName))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();

            if (ConfigurationList.ConStringId == 0)
            {
                ConfigurationList.ConStringId = 1;
            }
            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), ConfigurationList.ConStringId);

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetComponentConfigurationSetting");
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = "";
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
                    Setting obj = new Setting();
                    obj.ComponentConfigId = dr["ComponentConfigId"].ToString();
                    obj.Debug = Convert.ToBoolean(Convert.ToString(dr["Debug"]));
                    obj.Error = Convert.ToBoolean(Convert.ToString(dr["Error"]));
                    obj.Fatal = Convert.ToBoolean(Convert.ToString(dr["Fatal"]));
                    obj.Info = Convert.ToBoolean(Convert.ToString(dr["Info"]));
                    obj.Warn = Convert.ToBoolean(Convert.ToString(dr["Warn"]));
                    obj.SubscribeDebug = Convert.ToBoolean(Convert.ToString(dr["SubscribeDebug"]));
                    obj.SubscribeError = Convert.ToBoolean(Convert.ToString(dr["SubscribeError"]));
                    obj.CronJobDebug = Convert.ToBoolean(Convert.ToString(dr["CronJobDebug"]));
                    obj.CronJobInfo = Convert.ToBoolean(Convert.ToString(dr["CronJobInfo"]));
                    obj.CronJobWarn = Convert.ToBoolean(Convert.ToString(dr["CronJobWarn"]));
                    obj.CronJobError = Convert.ToBoolean(Convert.ToString(dr["CronJobError"]));
                    obj.CronJobFatal = Convert.ToBoolean(Convert.ToString(dr["CronJobFatal"]));
                    obj.OmsLog = Convert.ToBoolean(dr["IsOmsLog"].ToString());
                    obj.Name = dr["KeyName"].ToString();
                    ConfigurationList.SettingList.Add(obj);
                }
            }

            ViewBag.Message = "";

            ConfigurationList.ApplicationList = GetApplicationList(ConfigurationList.ConStringId, string.Empty);
            ConfigurationList.ApplicationCode = "All";

            return View(ConfigurationList);
        }

        [HttpPost]
        public ActionResult BindSetting(SettingModel model)
        {
            SettingModel ConfigurationList = new SettingModel();
            ConfigurationList.ConnectionList = ((Common.ConnectionStringName[])Enum.GetValues(typeof(Common.ConnectionStringName))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();
            ConfigurationList.ConStringId = model.ConStringId;

            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), model.ConStringId);

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetComponentConfigurationSetting");
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = model.ApplicationCode;
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
                    Setting obj = new Setting();
                    obj.ComponentConfigId = dr["ComponentConfigId"].ToString();
                    obj.Debug = Convert.ToBoolean(Convert.ToString(dr["Debug"]));
                    obj.Error = Convert.ToBoolean(Convert.ToString(dr["Error"]));
                    obj.Fatal = Convert.ToBoolean(Convert.ToString(dr["Fatal"]));
                    obj.Info = Convert.ToBoolean(Convert.ToString(dr["Info"]));
                    obj.Warn = Convert.ToBoolean(Convert.ToString(dr["Warn"]));
                    obj.SubscribeDebug = Convert.ToBoolean(Convert.ToString(dr["SubscribeDebug"]));
                    obj.SubscribeError = Convert.ToBoolean(Convert.ToString(dr["SubscribeError"]));
                    obj.CronJobDebug = Convert.ToBoolean(Convert.ToString(dr["CronJobDebug"]));
                    obj.CronJobInfo = Convert.ToBoolean(Convert.ToString(dr["CronJobInfo"]));
                    obj.CronJobWarn = Convert.ToBoolean(Convert.ToString(dr["CronJobWarn"]));
                    obj.CronJobError = Convert.ToBoolean(Convert.ToString(dr["CronJobError"]));
                    obj.CronJobFatal = Convert.ToBoolean(Convert.ToString(dr["CronJobFatal"]));
                    obj.Name = dr["KeyName"].ToString();
                    ConfigurationList.SettingList.Add(obj);
                }
            }

            ViewBag.Message = "";

            ConfigurationList.ApplicationList = GetApplicationList(ConfigurationList.ConStringId, string.Empty);

            return View("Index", ConfigurationList);
        }

        [HttpPost]
        public ActionResult Index(SettingModel model)
        {
            SettingModel ConfigurationList = new SettingModel();


            string ConnString = Enum.GetName(typeof(Common.ConnectionStringName), model.ConStringId);

            using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
            {
                SqlCommand cmd = new SqlCommand("GetComponentConfigurationSetting");
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@applicationCode";
                param.Value = string.Equals(model.ApplicationCode, "all", StringComparison.OrdinalIgnoreCase) ? "AllToUpdate" : model.ApplicationCode;
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
                    Setting obj = new Setting();
                    obj.ComponentConfigId = dr["ComponentConfigId"].ToString();
                    obj.Debug = Convert.ToBoolean(dr["Debug"].ToString());
                    obj.Error = Convert.ToBoolean(dr["Error"].ToString());
                    obj.Fatal = Convert.ToBoolean(dr["Fatal"].ToString());
                    obj.Info = Convert.ToBoolean(dr["Info"].ToString());
                    obj.Warn = Convert.ToBoolean(dr["Warn"].ToString());
                    obj.SubscribeDebug = Convert.ToBoolean(dr["SubscribeDebug"].ToString());
                    obj.SubscribeError = Convert.ToBoolean(dr["SubscribeError"].ToString());
                    obj.CronJobDebug = Convert.ToBoolean(Convert.ToString(dr["CronJobDebug"]));
                    obj.CronJobInfo = Convert.ToBoolean(Convert.ToString(dr["CronJobInfo"]));
                    obj.CronJobWarn = Convert.ToBoolean(Convert.ToString(dr["CronJobWarn"]));
                    obj.CronJobError = Convert.ToBoolean(Convert.ToString(dr["CronJobError"]));
                    obj.CronJobFatal = Convert.ToBoolean(Convert.ToString(dr["CronJobFatal"]));
                    obj.Name = dr["KeyName"].ToString();
                    ConfigurationList.SettingList.Add(obj);
                }
            }

            var changedSetting = (from s in model.SettingList
                                  join cs in ConfigurationList.SettingList
                                  on s.ComponentConfigId equals cs.ComponentConfigId
                                  where s.Debug != cs.Debug || s.Error != cs.Error ||
                                  s.Fatal != cs.Fatal && s.Info != cs.Info && s.Warn != cs.Warn ||
                                  s.SubscribeDebug != cs.SubscribeDebug || s.SubscribeError != cs.SubscribeError ||
                                  s.CronJobDebug != cs.CronJobDebug || s.CronJobError != cs.CronJobError ||
                                  s.CronJobFatal != cs.CronJobFatal || s.CronJobInfo != cs.CronJobInfo || s.CronJobWarn != cs.CronJobWarn
                                  select s).ToList();

            foreach (var item in changedSetting)
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString(ConnString)))
                {
                    SqlCommand cmd = new SqlCommand("UpdateComponentConfigurationSetting");
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@id";
                    param.Value = item.ComponentConfigId;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@debug";
                    param.Value = item.Debug;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@error";
                    param.Value = item.Error;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@info";
                    param.Value = item.Info;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@fatal";
                    param.Value = item.Fatal;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@warn";
                    param.Value = item.Warn;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@subscribedebug";
                    param.Value = item.SubscribeDebug;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@subscribeerror";
                    param.Value = item.SubscribeError;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@cronjobdebug";
                    param.Value = item.CronJobDebug;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@cronjoberror";
                    param.Value = item.CronJobError;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@cronjobfatal";
                    param.Value = item.CronJobFatal;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@cronjobinfo";
                    param.Value = item.CronJobInfo;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@cronjobwarn";
                    param.Value = item.CronJobWarn;
                    cmd.Parameters.Add(param);

                    param = new SqlParameter();
                    param.ParameterName = "@omslog";
                    param.Value = item.OmsLog;
                    cmd.Parameters.Add(param);

                    cmd.Connection = conn;
                    conn.Open();

                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            #region change setting update 
            //foreach (var item in model.SettingList)
            //{
            //    foreach (var componentDetail in ConfigurationList.SettingList)
            //    {
            //        if (ConfigurationList.SettingList.Where(t => t.ComponentConfigId == componentDetail.ComponentConfigId && t.Debug == item.Debug && t.Error == item.Error && t.Fatal == item.Fatal && t.Info == item.Info && t.Warn == item.Warn).Count() == 0)
            //        {
            //            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnString].ConnectionString))
            //            {
            //                SqlCommand cmd = new SqlCommand("UpdateComponentConfigurationSetting");
            //                cmd.CommandType = CommandType.StoredProcedure;
            //                SqlParameter param = new SqlParameter();
            //                param.ParameterName = "@id";
            //                param.Value = componentDetail.ComponentConfigId;
            //                cmd.Parameters.Add(param);

            //                param = new SqlParameter();
            //                param.ParameterName = "@debug";
            //                param.Value = item.Debug;
            //                cmd.Parameters.Add(param);

            //                param = new SqlParameter();
            //                param.ParameterName = "@error";
            //                param.Value = item.Error;
            //                cmd.Parameters.Add(param);

            //                param = new SqlParameter();
            //                param.ParameterName = "@info";
            //                param.Value = item.Info;
            //                cmd.Parameters.Add(param);

            //                param = new SqlParameter();
            //                param.ParameterName = "@fatal";
            //                param.Value = item.Fatal;
            //                cmd.Parameters.Add(param);

            //                param = new SqlParameter();
            //                param.ParameterName = "@warn";
            //                param.Value = item.Warn;
            //                cmd.Parameters.Add(param);

            //                //param = new SqlParameter();
            //                //param.ParameterName = "@subscribedebug";
            //                //param.Value = item.Warn;
            //                //cmd.Parameters.Add(param);

            //                //param = new SqlParameter();
            //                //param.ParameterName = "@subscribeerror";
            //                //param.Value = item.Warn;
            //                //cmd.Parameters.Add(param);

            //                cmd.Connection = conn;
            //                conn.Open();

            //                cmd.ExecuteNonQuery();
            //                conn.Close();
            //            }
            //        }
            //    }

            //}
            #endregion

            ViewBag.Message = "Settings updated successfully...";

            model.ConnectionList = ((Common.ConnectionStringName[])Enum.GetValues(typeof(Common.ConnectionStringName))).Select(c => new SelectListItem() { Value = Convert.ToString((int)c), Text = c.ToString() }).ToList();

            model.ApplicationList = GetApplicationList(model.ConStringId, model.ApplicationCode);

            return View(model);
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