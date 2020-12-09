using iot.solution.entity.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace iot.solution.entity
{
    //Facility
    public class Entity
    {
        public Guid Guid { get; set; }
       // public Guid CompanyGuid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public Guid? ParentEntityGuid { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public Guid? StateGuid { get; set; }
        public Guid? CountryGuid { get; set; }
        public string Image { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        //public int TotalUsers{ get; set; }
        public List<EntityWiseDeviceResponse> Devices { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
    public class EntityDetail:Entity
    {
       
        public int TotalSubEntities { get; set; }
        public int TotalDevices { get; set; }
        public int TotalAlerts { get; set; }
        public int TotalConnected { get; set; }
        public int TotalDisconnected { get; set; } 
    }
    public class EntityListRequest :ListRequest
    {
        public string parentEntityGuid { get; set; } = "";
        public DateTime? currentDate { get; set; } = null;
        public string timeZone { get; set; } = "";
    }
    public class EntityListItem
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool? IsActive { get; set; }
        public int TotalSubEntities { get; set; }
        public int TotalDevices { get; set; }
        public int TotalAlerts { get; set; }
        public int TotalConnected { get; set; }
        public int TotalDisconnected { get; set; }
    }
    public class EntityWithCounts : Entity
    {
        public int TotalDevices { get; set; }
        public int TotalOnConnectedDevices { get; set; }
        public int TotalOffDevices { get; set; }
        public int TotalDisconnectedDevices { get; set; }
        public int TotalEneryGenerated { get; set; }
        public int TotalFuelUsed { get; set; }
       
    }

    [XmlRoot(ElementName = "devices")]
    public class devices
    {
        [XmlElement("device")]
        public List<device> devicelist { get; set; }
    }
    [Serializable]
    public class device
    {
        [XmlElement("guid")]
        public Guid guid { get; set; }
        [XmlElement("name")]
        public string name { get; set; }
        [XmlElement("uniqueId")]
        public string uniqueId { get; set; }
        [XmlElement("entityGuid")]
        public Guid entityGuid { get; set; }
        [XmlElement("capacity")]
        public long capacity { get; set; }
        [XmlElement("availableQty")]
        public long availableQty { get; set; }
        [XmlElement("thresholdRate")]
        public string thresholdRate { get; set; }
        [XmlElement("color")]
        public string color { get; set; }
       
        public bool IsConnected { get; set; }
    }

}
