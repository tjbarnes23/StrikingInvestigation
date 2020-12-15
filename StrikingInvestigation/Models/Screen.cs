namespace StrikingInvestigation.Models
{
    public class Screen
    {
        public bool IsA { get; set; }

        public int XMargin { get; set; }

        public int YMargin { get; set; }

        public bool PlayDisabled { get; set; }

        public string PlayLabel { get; set; }

        public bool SpinnerPlaying { get; set; }

        public bool RunAnimation { get; set; }

        public int AnimationDuration { get; set; }
    }
}
