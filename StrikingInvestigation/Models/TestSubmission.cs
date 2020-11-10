using System;

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

        public string AB { get; set; }
    }
}
