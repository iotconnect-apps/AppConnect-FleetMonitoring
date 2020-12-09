using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class ErrorMessageResponse
    {
        public string code { get; set; }
        public string msg { get; set; }
    }
    public class SaveCompanyResponse
    {
        public string PaymentTransactionId { get; set; }
        public int TransactionStatus { get; set; }
        public string ErrorMessage { get; set; }
        public string BillingId { get; set; }
        public string IoTConnectCompanyGuid { get; set; }
        public string Code { get; set; }
        public string Msg { get; set; }
    }
}
