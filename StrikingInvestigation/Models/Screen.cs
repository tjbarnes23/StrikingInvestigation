namespace StrikingInvestigation.Models
{
    public class Screen
    {
        public double DiameterScale { get; set; }

        public double XScale { get; set; }

        public int XMargin { get; set; }

        public double YScale { get; set; }

        public int YMargin { get; set; }

        public int GapMin { get; set; }

        public int GapMax { get; set; }

        public int BaseGap { get; set; }

        public bool RunAnimation { get; set; }
    }
}
