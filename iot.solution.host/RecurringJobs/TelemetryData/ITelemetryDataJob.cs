using Hangfire;

namespace host.iot.solution.RecurringJobs

{
    public interface ITelemetryDataJob 
    {
        [DisableConcurrentExecution(10 * 60)]
        void DailyProcess();

        [DisableConcurrentExecution(10 * 60)]
        void HourlyProcess();
        [DisableConcurrentExecution(10 * 60)]
        void HourlyEmailProcess();
        [DisableConcurrentExecution(10 * 60)]
        void SubscriptionMailProcess();
    }
}