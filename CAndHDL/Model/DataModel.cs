using System;

namespace CAndHDL.Model
{
    /// <summary>
    /// Model for the data
    /// </summary>
    public class DataModel
    {
        /// <summary>Start date</summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>End date</summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>Download all the comics</summary>
        public bool DLAll { get; set; }

        /// <summary>Download all the comics since last download</summary>
        public bool DLSinceLastTime { get; set; }

        /// <summary>Path to comics download</summary>
        public string Path { get; set; }
    }
}
