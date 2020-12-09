using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace component.services.logger.viewer.Models.SettingModel
{
    public class SettingModel
    {
        public SettingModel()
        {
            SettingList = new List<Setting>();
            ConnectionList = new List<SelectListItem>();
            ApplicationList = new List<SelectListItem>();
        }
        public IList<Setting> SettingList { get; set; }
        public IList<SelectListItem> ConnectionList { get; set; }
        public IList<SelectListItem> ApplicationList { get; set; }
        public int ConStringId { get; set; }
        public string ApplicationCode { get; set; }
    }

    public class Setting
    {
        public string ComponentConfigId { get; set; }
        public bool Debug { get; set; }
        public bool Error { get; set; }
        public bool Fatal { get; set; }
        public bool Info { get; set; }
        public bool Warn { get; set; }
        public bool SubscribeError { get; set; }
        public bool SubscribeDebug { get; set; }
        public bool OmsLog { get; set; }
        public string Name { get; set; }

        public bool CronJobDebug { get; set; }
        public bool CronJobInfo { get; set; }
        public bool CronJobWarn { get; set; }
        public bool CronJobError { get; set; }
        public bool CronJobFatal { get; set; }
    }
}