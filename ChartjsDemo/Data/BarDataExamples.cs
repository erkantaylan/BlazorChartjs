using System.Diagnostics;

namespace ChartjsDemo.Data;

public static class BarDataExamples
{
    public static List<string> SimpleBarText = new List<string>() { "January", "February", "March", "April", "May", "June", "July" };
    public static List<DataItem> SimpleBar = new List<DataItem>() {
            new DataItem() { Name = "January", Value = 65 },
            new DataItem() { Name = "February", Value = 59 },
            new DataItem() { Name = "March", Value = 80 },
            new DataItem() { Name = "April", Value = 81 },
            new DataItem() { Name = "May", Value = 56 },
            new DataItem() { Name = "June", Value = 55 },
            new DataItem() { Name = "July", Value = 40 }
        };

    public static List<string> GroupedLabels = new List<string>() { "1900", "1950", "1999", "2050" };
    public static List<decimal?> Grouped1 = new List<decimal?>() { 133, 221, 783, 2478 };
    public static List<decimal?> Grouped2 = new List<decimal?>() { 408, 547, 675, 734 };

    public static List<string> CallbackLabels = new List<string>() { "Q1", "Q2", "Q3", "Q4" };
    public static List<decimal?> CallbackValues = new List<decimal?> { 50000, 60000, 70000, 1800000 };

    public static List<string> StylingLabels = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
    public static List<decimal?> StylingTarget = new List<decimal?>() { 50, 55, 60, 60, 65, 70 };
    public static List<decimal?> StylingActual = new List<decimal?>() { 42, 58, 51, 63, 60, 74 };

    public static List<decimal?> StackOnline = new List<decimal?>() { 18, 22, 25, 31, 28, 35 };
    public static List<decimal?> StackStore = new List<decimal?>() { 30, 26, 28, 24, 27, 22 };
    public static List<decimal?> StackWholesale = new List<decimal?>() { 12, 15, 11, 14, 16, 18 };

    public static List<string> SkipNullLabels = new List<string>() { "North", "East", "South", "West" };
    public static List<decimal?> SkipNullThisYear = new List<decimal?>() { 34, 41, 29, 38 };
    public static List<decimal?> SkipNullLastYear = new List<decimal?>() { 30, null, 31, null };
}