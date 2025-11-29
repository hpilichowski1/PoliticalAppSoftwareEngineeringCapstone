using System;

namespace PoliticalApp.Models
{
    public class Representative
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;       // e.g. "Senator", "Representative"
        public string Party { get; set; } = string.Empty;       // "Democrat", "Republican", etc.
        public string District { get; set; } = string.Empty;    // e.g. "FL-2" or "FL"
        public string Bio { get; set; } = string.Empty;

        // Stats used in your XAML
        public double ConsistencyScore { get; set; }            // placeholder (can map from votes later)
        public int YearsInOffice { get; set; }
        public string Level { get; set; } = "State";            // used for your "Level" filter
    }
}
