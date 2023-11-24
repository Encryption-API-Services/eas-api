using System;

namespace Models.TrialPeriod
{
    public class TrialPeriodRequest
    {
        public string UserId { get; set; }
        public string Route { get; set; }
        public DateTime CreateTime { get; set; }
    }
}