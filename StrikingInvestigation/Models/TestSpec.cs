using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Models
{
    public class TestSpec
    {
        public int BrowserWidth { get; set; }
        
        public int BrowserHeight { get; set; }

        public DeviceLoad DeviceLoad { get; set; }

        public int Stage { get; set; }

        public int TenorWeight { get; set; }

        public int NumRows { get; set; }

        public int ErrorType { get; set; }

        public int ErrorSize { get; set; }

        public int TestBellLoc { get; set; }

        public int SelectedTest { get; set; }

        public bool ShowGaps { get; set; }

        public string SaveLabel { get; set; }

        public bool SpinnerSaving { get; set; }

        public bool Saved { get; set; }

        public double DiameterScale { get; set; }

        public double XScale { get; set; }

        public double YScale { get; set; }

        public int BorderWidth { get; set; }

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

        public bool AHasErrors { get; set; }

        public string SubmitLabel1 { get; set; }

        public string SubmitLabel2 { get; set; }

        public string SubmitLabel3 { get; set; }

        public bool SpinnerSubmitting1 { get; set; }

        public bool SpinnerSubmitting2 { get; set; }

        public bool SpinnerSubmitting3 { get; set; }

        public bool Submitted1 { get; set; }

        public bool Submitted2 { get; set; }

        public bool Submitted3 { get; set; }

        public bool ResultSound { get; set; }
        
        public string ResultSource { get; set; }
        
        public bool ResultEntered { get; set; }
    }
}
