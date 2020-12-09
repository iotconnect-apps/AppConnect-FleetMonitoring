using IoTConnect.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IoTConnect.Common
{
    internal static class Helper
    {
        public static object GetPropertyValue(this object T, string PropName)
        {
            return T.GetType().GetProperty(PropName) == null ? null : T.GetType().GetProperty(PropName).GetValue(T, null);
        }

        public static string GetDescription(this Enum e)
        {
            var attribute =
                e.GetType()
                    .GetTypeInfo()
                    .GetMember(e.ToString())
                    .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .SingleOrDefault()
                    as DescriptionAttribute;

            return attribute?.Description ?? e.ToString();
        }

        public static List<ErrorItemModel> ValidateObject(this object model)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            List<ErrorItemModel> errorItemModels = new List<ErrorItemModel>();
            ValidationContext context = new ValidationContext(model, null, null);

            if (!Validator.TryValidateObject(model, context, errors, true))
            {
                for (int i = 0; i < errors?.Count; i++)
                {
                    ErrorItemModel errorItemModel = new ErrorItemModel();
                    errorItemModel.Message = errors[i].ErrorMessage;
                    errorItemModel.Param = errors[i].MemberNames.FirstOrDefault();
                    errorItemModels.Add(errorItemModel);
                }
                return errorItemModels;
            }
            return errorItemModels;
        }
    }
}
