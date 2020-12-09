using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
    public class AddEntityModel
    {
        /// <summary>
        /// Add Entity.
        /// </summary>
        public AddEntityModel()
        {
             //UserInfo = new AddUserInfo(); 
            CustomETPlaceHolders = new CustomETPlaceHolder();
        }
        /// <summary>
        /// Entity Name. IotConnect will throw an error if Name is already exist. 
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        /// <summary>
        /// (Optional). Entity Description.        
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Parent Entity Guid.
        /// </summary>
        [Required(ErrorMessage = "ParentEntityGuid is required")]
        public string ParentEntityGuid { get; set; }
        /// <summary>
        /// (Optional) Child Entity Lable.
        /// </summary>
        public string ChildEntityLable { get; set; }
        /// <summary>
        /// Address.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// (Optional) Address2.
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// City.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// State Guid.Refer IotConnect.Comman.Master.State() to get list of states.
        /// </summary>
        public string StateGuid { get; set; }
        /// <summary>
        /// Country Guid. Refer IotConnect.Comman.Master.Country() to get list of countries.
        /// </summary>
        public string CountryGuid { get; set; }
        /// <summary>
        /// (Optional) Zip Code.
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// Time Zone Guid. Refer IotConnect.Comman.Master.TimeZones() to get list of timezones.
        /// </summary>
        public string TimezoneGuid { get; set; }
        /// <summary>
        /// Owner Info. If you do not want to create owner pass null.
        /// </summary>
        [JsonProperty("UserInfo")]
        public AddOwnerInfo OwnerInfo { get; set; }
        /// <summary>
        /// CustomETPlace Holder.
        /// </summary>
        public CustomETPlaceHolder CustomETPlaceHolders { get; set; }
    }
     
    public class AddOwnerInfo
    {
        public AddOwnerInfo()
        {
            Properties = new List<AddProperty>();
        }
        /// <summary>
        /// Owner First Name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Owner last Name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// (Optional) UserId
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// Time Zone Guid. Refer IotConnect.Comman.Master.TimeZones() to get list of timezones.
        /// </summary>
        public string TimezoneGuid { get; set; }
        /// <summary>
        /// Role Guid. Refer IotConnect.User.AllUserRole() to get list of roles.
        /// </summary>
        public string RoleGuid { get; set; }
        /// <summary>
        /// (Optional) Contact No.
        /// </summary>
        public string ContactNo { get; set; }
        /// <summary>
        /// Properties.
        /// </summary>
        public List<AddProperty> Properties { get; set; }
        /// <summary>
        /// Entity Status.
        /// </summary>
        public bool IsActive { get; set; }
    }
    /// <summary>
    /// Add Property
    /// </summary>
    public class AddProperty
    {
        /// <summary>
        /// Property key.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Property Value.
        /// </summary>
        public string Value { get; set; }
    }

    public class CustomETPlaceHolder { }
}
