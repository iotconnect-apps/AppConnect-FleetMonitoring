namespace iot.solution.entity
{
    public class ActionStatus
    {
        private bool v;

        public ActionStatus()
        {
            Success = false;
            Message = string.Empty;
            Data = null;
        }

        public ActionStatus(bool status)
        {
            Success = status;
        }

        public ActionStatus(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public ActionStatus(bool success, string message, string messageDetail, object result)
        {
            Success = success;
            Message = message;
            Data = result;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
    }
}
