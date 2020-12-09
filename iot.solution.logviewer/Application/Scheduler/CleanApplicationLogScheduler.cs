using component.logger.data.log.Repositories;
using component.services.logger.viewer.Application.AppSetting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace component.services.logger.viewer.Application.Scheduler
{
    public class CleanApplicationLogScheduler : SchedulerService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CleanApplicationLogScheduler"/> class.
        /// </summary>
        public CleanApplicationLogScheduler()
        {
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                JobCleanApplicationLog();
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }

        /// <summary>
        /// Jobs the clean application log.
        /// </summary>
        private async void JobCleanApplicationLog()
        {
            string days = ServiceAppSetting.Instance.GetAppSettingByKey("ApplicationLogKeepDays");
            LogManagementRepository logManagementRepository = new LogManagementRepository();
            bool dataResponse = logManagementRepository.CleanApplicationLog(string.IsNullOrWhiteSpace(days) ? 7 : Convert.ToInt32(days));
        }
    }
}
