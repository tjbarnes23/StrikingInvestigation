using System;
using StrikingInvestigation.Utilities;

namespace StrikingInvestigation.Models
{
    public class TestSubmission
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime TestDate { get; set; }

        public string TestType { get; set; }

        public int TestId { get; set; }

        public int Gap { get; set; }

        public ABResult ABResult { get; set; }
    }
}
