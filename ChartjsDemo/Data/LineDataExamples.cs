using Erkan.Blazor.Chartjs.Models.Common;

namespace ChartjsDemo.Data
{
    public static class LineDataExamples
    {
        public static List<string> SimpleLineText = new List<string>() { "January", "February", "March", "April", "May", "June", "July" };
        public static List<decimal?> SimpleLine = new List<decimal?>() { 65, 59, 80, 81, 86, 55, 40 };
        public static List<decimal?> SimpleLine2 = new List<decimal?>() { 33, 25, 35, 51, 54, 76, 60 };
        public static List<decimal?> SimpleLine3 = new List<decimal?>() { 53, 91, 39, 61, 39, 87, 23 };

        // stepped line
        public static List<string> StepLineText = new List<string>() { "Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6" };
        public static List<decimal?> StepLine = new List<decimal?>() { 65, 59, 80, 81, 86, 55, 40 };

        // step modes compared
        public static List<string> StepModesText = new List<string>() { "Day 1", "Day 2", "Day 3", "Day 4", "Day 5", "Day 6" };
        public static List<decimal?> StepModes = new List<decimal?>() { 10, 30, 20, 40, 25, 35 };

        // dashes, cap and join styles, per-point colours
        public static List<string> StylingLineText = new List<string>() { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        public static List<decimal?> StylingTarget = new List<decimal?>() { 60, 60, 60, 60, 60, 60, 60 };
        public static List<decimal?> StylingForecast = new List<decimal?>() { 55, 62, 61, 68, 57, 63, 70 };
        public static List<decimal?> StylingActual = new List<decimal?>() { 52, 64, 58, 71, 49, 66, 75 };

        // gaps in the data
        public static List<string> GapLineText = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static List<decimal?> GapLine = new List<decimal?>() { 12, 19, null, 24, 30, null, null, 41, 38 };

        // custom code
        public static List<string> CustomLineText = new List<string>() { "January", "February", "March", "April", "May", "June" };
        public static List<decimal?> CustomLine = new List<decimal?>() { 60, 80, 81, 56, 55, 40 };

        // multi axes
        public static List<string> MultiAxesLineText = new List<string>() { 
            "January;2015", "February;2015;Y", "March;2015", 
            "January;2016", "February;2016;Y", "March;2016" };
        public static List<decimal?> MultiAxesLine = new List<decimal?>() { 12, 19, 3, 5, 2, 3 };

        public static List<decimal?> BreakLine = new List<decimal?>() { 0, 20, 20, 60, 60, 120, null, 180, 120, 125, 105, 110, 170 };
        public static List<string> BreakLineText = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };
    }
}