namespace StrikingInvestigation.Models
{
    public class Screen
    {
        public double DiameterScale { get; set; }

        public double XScale { get; set; }

        public int XMargin { get; set; }

        public double YScale { get; set; }

        public int YMargin { get; set; }

        public int BorderWidth  { get; set; }

        public int FontSize { get; set; }

        public int StrokeLabelXOffset { get; set; }

        public int StrokeLabelYOffset { get; set; }

        public int RowStartLabelWidth { get; set; }

        public int RowStartLabelHeight { get; set; }

        public int ChangeLabelXOffset { get; set; }

        public int ChangeLabelYOffset { get; set; }

        public int BaseGap { get; set; }

        public int GapMin { get; set; }

        public int GapMax { get; set; }

        public bool RunAnimation { get; set; }

        public int AnimationDuration { get; set; }
    }
}
