using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class BaseResponse<T>
    {
        public BaseResponse()
        {
            Time = DateTime.Now.ToLongDateString();
            LastSyncDate = "";
        }

        public BaseResponse(bool defalut = false)
        {
            IsSuccess = defalut;
            Message = "";
            Time = DateTime.Now.ToLongDateString();
            LastSyncDate = "";
        }

        public BaseResponse(bool defalut, string message) 
        {
            IsSuccess = defalut;
            Message = message;
            Time = DateTime.Now.ToLongDateString();
            LastSyncDate = "";
        }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public string Time { get; set; }
        public string LastSyncDate { get; set; }
        public T Data { get; set; }

    }
}
