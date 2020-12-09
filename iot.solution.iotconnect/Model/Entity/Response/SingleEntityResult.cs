using System;

namespace IoTConnect.Model
{
    /// <summary>
    /// Single Entity result.
    /// </summary>
    public class SingleEntityResult
    {
        /// <summary>
        /// Returns Guid.
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// Returns Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Returns parent Entity Guid.
        /// </summary>
        public string ParentEntityGuid { get; set; }
        /// <summary>
        /// Returns Description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Returns Child Entity Lable
        /// </summary>
        public string ChildEntityLable { get; set; }
        /// <summary>
        /// Returns Company Guid
        /// </summary>
        public string CompanyGuid { get; set; }
        /// <summary>
        /// Returns Entity deleted Or not.
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Returns Created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Returns Created by.
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Returns Updated Date.
        /// </summary>
        public DateTime UpdatedDate { get; set; }
        /// <summary>
        /// Returns Update By.
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Returns Address.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Returns City.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Returns State Guid.
        /// </summary>
        public string StateGuid { get; set; }
        /// <summary>
        /// Returns Country Guid.
        /// </summary>
        public string CountryGuid { get; set; }
        /// <summary>
        /// Returns Time Zone Guid.
        /// </summary>
        public string TimezoneGuid { get; set; }
        /// <summary>
        /// Returns Default User Guid.
        /// </summary>
        public string DefaultuserGuid { get; set; }
        /// <summary>
        /// Returns zip Code.
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Returns Address2.
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// Returns Entity Info.
        /// </summary>
        public object EntityInfo { get; set; }
    }
}
