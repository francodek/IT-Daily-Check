namespace IT_Daily_Check.Models
{
    public class DailyCheck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date_Created { get; set; }
        public string Location { get; set; }
        public string Created_By { get; set; }

        public ICollection<CCTVcheck> CCTVchecks { get; set; }
        public ICollection<DeviceServicecheck> DeviceServicechecks { get; set; }

        public ICollection<InternetServiceSpeedcheck> InternetServiceSpeedchecks { get; set; }
    }
}

