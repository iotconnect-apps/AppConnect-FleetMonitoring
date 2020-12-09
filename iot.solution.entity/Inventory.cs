using System;
using System.Collections.Generic;
using System.Text;

namespace iot.solution.entity
{
    public class Inventory
    {
        public Guid Guid { get; set; }
        public Guid CompanyGuid { get; set; }
        public Guid EntityGuid { get; set; }
        public Guid? DeviceGuid { get; set; }
        public Guid? ProductGuid { get; set; }
        public Guid? ProductTypeGuid { get; set; }
        public DateTime? RefillDateTime { get; set; }
        public long? Quantity { get; set; }
        public Guid? DeviceItemGuid { get; set; }
        public string Action { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClearShelf { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
       
    }

    public class InventoryDetail:Inventory
    {
        public string CompanyName { get; set; }
        public string EntityName { get; set; }
        public string DeviceName { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ShelfId { get; set; }
        public long? AvailableQty { get; set; }
        public long? Capacity { get; set; }
    }

    public class InventoryHistoryDetail
    {
        public string EntityName { get; set; }
        public string DeviceName { get; set; }
        public string UniqueId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ShelfId { get; set; }
        public string Operation { get; set; }
        public string RefillQuantity { get; set; }
        public DateTime RefillDate { get; set; }
        public List<RefillDetail> refillDetails { get; set; }
    }
    public class RefillDetail
    {
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string RefillQuantity { get; set; }
        public DateTime RefillDate { get; set; }
    }
}
