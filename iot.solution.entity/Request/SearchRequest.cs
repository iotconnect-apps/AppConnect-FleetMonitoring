namespace iot.solution.entity
{
    public class BaseRequest
    {
        public string Guid { get; set; }
        public string CompanyId { get; set; }
        public System.Guid EntityId { get; set; }
        public System.Guid ParentEntityGuid { get; set; }
      
        public string Version { get; set; } = "v1";
        
    }
    public class SearchRequest : BaseRequest
    {
        public System.Guid InvokingUser { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public string SearchText { get; set; }
        public System.DateTime? CurrentDate { get; set; }
        public System.DateTime? StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public string TimeZone { get; set; }
        public System.Guid? FleetGuid { get; set; }
        public System.Guid? DriverGuid { get; set; }
        public string Status { get; set; }
    }
    public class MobileSearchRequest 
    {
      
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public string OrderBy { get; set; }
        public string SearchText { get; set; }       
        public string fleetGuid { get; set; }
        public string entityGuid { get; set; }
    }
    public class SearchResult<T>
    {
        public T Items { get; set; }
        public int Count { get; set; }
    }
}
