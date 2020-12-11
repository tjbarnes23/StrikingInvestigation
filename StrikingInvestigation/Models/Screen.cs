namespace StrikingInvestigation.Models
{
    public class Screen
    {
        public int SelectedTest { get; set; }

        public bool ShowGaps { get; set; }

        public string SaveLabel { get; set; }

        public bool SpinnerSaving { get; set; }

        public bool Saved { get; set; }

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

        public bool ControlsDisabled { get; set; }

        public bool TenorWeightDisabled { get; set; }

        public bool PlayDisabled { get; set; }

        public bool PlayADisabled { get; set; }

        public bool PlayBDisabled { get; set; }

        public string PlayLabel { get; set; }

        public string PlayALabel { get; set; }

        public string PlayBLabel { get; set; }

        public bool SpinnerPlaying { get; set; }

        public bool SpinnerPlayingA { get; set; }

        public bool SpinnerPlayingB { get; set; }

        public string SubmitLabel { get; set; }

        public bool SpinnerSubmitting { get; set; }

        public bool Submitted { get; set; }

        public bool RunAnimation { get; set; }

        public int AnimationDuration { get; set; }
    }
}
