namespace iot.solution.entity.Response
{
    public class EntityDetailResponse
    {
        public int EnergyUsage { get; set; }
        public int Temperature { get; set; }
        public int Moisture { get; set; }
        public int Humidity { get; set; }
        public int WaterUsage { get; set; }
        public int TotalDevices { get; set; }
    }
}
