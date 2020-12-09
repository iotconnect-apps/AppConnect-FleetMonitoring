using System;

namespace iot.solution.entity
{
    public class LookupItem
    {
        private string _Value;
        public string Value { get { return _Value; } set { _Value = value; } }
        public string Text { get; set; }
    }
    public class LookupItemWithTemplateGuid:LookupItem
    {
       public string TemplateGuid { get; set; }
    }
    public class LookupItemWithStatus:LookupItem
    {
        public bool? IsActive { get; set; }
    }
    public class LookupItemWithDescription : LookupItem
    {
        public string Description { get; set; }
    }
    public class AttributeItem
    {
        private string _Value;
        public string Value { get { return _Value; } set { _Value = value; } }
        public string Key { get; set; }
    }
    public class TagLookup
    {
        public string tag { get; set; }
        public bool templateTag { get; set; }
    }
}
