using component.helper.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using System;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;


namespace component.helper
{
    public class UtilityHelper : IUtilityHelper
    {
        public string Mask(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Regex regex = new Regex("[a-zA-Z0-9]");
                return regex.Replace(value, "X");
            }
            return string.Empty;
        }

        public  string IOTResultMessage(List<IoTConnect.Model.ErrorItemModel> message)
        {
            if(message.Count > 0)
            {
                return message[0].Message;
            }
            return "Something Went Wrong!";
        }

       

    }
}
