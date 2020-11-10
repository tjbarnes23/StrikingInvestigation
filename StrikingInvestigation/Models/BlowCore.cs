namespace StrikingInvestigation.Models
{
    public class BlowCore
    {
        // Constructor not used for this class because Json deserializer needs parameterless constructor

        public string Bell { get; set; }

        public string ChangeStr { get; set; }

        public int Gap { get; set; }

        // AltGap is used when two gaps need to be tracked -- e.g. if audio and visual strikes are to occur
        // at different times
        public int AltGap { get; set; }

        // To signify that test bell formatting should be applied to this blow
        public bool IsTestBell { get; set; }

        // To signify that special formatting should be applied to this blow
        public bool IsHighlighted { get; set; }
    }
}
