namespace StrikingInvestigation.Utilities
{
    public class Device
    {
        public Device()
        {
            DeviceLoad = DeviceLoad.High;
        }

        public DeviceLoad DeviceLoad { get; set; }
    }
}
