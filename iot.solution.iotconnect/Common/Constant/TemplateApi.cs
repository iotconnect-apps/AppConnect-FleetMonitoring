namespace IoTConnect.Common.Constant
{
    internal class TemplateApi
    {
        internal const string TemplateLookup = "api/v{0}/device-template/{1}/lookup";
        internal const string AddTemplate = "api/v{0}/device-template";
        internal const string UpdateTemplate = "api/v{0}/device-template/{1}";
        internal const string Templates = "api/v{0}/device-template";
        internal const string Template = "api/v{0}/template/{1}/device";
        internal const string DeleteTemplate = "api/v{0}/device-template/{1}";
        internal const string DataTypes = "api/v{0}/device-template/datatype";
        internal const string QuickAdd = "api/v{0}/device-template/quick";
        internal const string AddAttribute = "api/v{0}/template-attribute";
        internal const string UpdateAttribute = "api/v{0}/template-attribute/{1}";
        internal const string DeleteAttribute = "api/v{0}/template-attribute/{1}";
        internal const string AllAttribute = "api/v{0}/template-attribute/{1}";
        internal const string AddTwin = "api/v{0}/template-setting";
        internal const string UpdateTwin = "api/v{0}/template-setting/{1}";
        internal const string DeleteTwin = "api/v{0}/template-setting/{1}";
        internal const string AllTwin = "api/v{0}/template-setting/{1}";
        internal const string AllDeviceTwin = "api/v{0}/device/{1}/template-setting";
        internal const string AddCommand= "api/v{0}/template-command";
        internal const string UpdateCommand = "api/v{0}/template-command/{1}";
        internal const string DeleteCommand = "api/v{0}/template-command/{1}";
        internal const string AllTemplateCommand = "api/v{0}/template-command/{1}";
        internal const string ExcuteCommand= "api/v{0}/template-command/device/{1}/send";
        internal const string ExcuteCommands = "api/v{0}/template-command/send";
        internal const string TagLookup = "api/v{0}/device-template/{1}/tag-lookup";
    }
}
