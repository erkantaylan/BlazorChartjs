using System.Text.RegularExpressions;
using System.Xml.Linq;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests.Regression;

/// <summary>
/// Unreleased: the copyright notice of the projects this code came from.
/// </summary>
/// <remarks>
/// Upstream reused code from ChartJs.Blazor (MIT) in its first commit — a <c>ColorUtil</c> class
/// and <c>IDataset</c> — without the copyright notice the MIT licence requires, and this fork
/// inherited the gap. A notice fails nowhere at runtime, so these tests read <c>LICENSE</c> and
/// the library csproj as text: a <c>LICENSE</c> rewritten without a holder, or a package
/// <c>&lt;Copyright&gt;</c> that names fewer holders than the licence it ships, fails here.
/// </remarks>
public partial class LicenseNoticeTests
{
    private static readonly string License = File.ReadAllText(TestPaths.License);

    [Theory]
    // ChartJs.Blazor, https://github.com/mariusmuntean/ChartJs.Blazor
    [InlineData("Copyright (c) 2019 Marius Muntean")]
    [InlineData("Copyright (c) 2021 Joel L.")]
    // erossini/BlazorChartjs, which this repository forks
    [InlineData("Copyright (c) 2023 Enrico Rossini")]
    public void License_keeps_the_copyright_notice(string line)
    {
        Assert.True(License.Split('\n').Any(l => l.Trim() == line),
            $"LICENSE no longer carries '{line}'. MIT requires the notice of every project this "
            + "code was taken from to be kept in all copies.");
    }

    /// <summary>
    /// The package's <c>&lt;Copyright&gt;</c> is what nuget.org shows; it must name every holder
    /// <c>LICENSE</c> does.
    /// </summary>
    [Fact]
    public void Package_copyright_names_every_license_holder()
    {
        var packageCopyright = XDocument.Load(TestPaths.LibraryProject)
            .Descendants("Copyright")
            .Single()
            .Value;

        var holders = CopyrightLine().Matches(License).Select(m => m.Groups["notice"].Value).ToList();
        Assert.NotEmpty(holders);

        var missing = holders.Where(h => !packageCopyright.Contains(h, StringComparison.Ordinal)).ToList();
        Assert.True(missing.Count == 0,
            $"""
             <Copyright> in src/Erkan.Blazor.Chartjs.csproj does not name every holder in LICENSE:

               {string.Join($"{Environment.NewLine}  ", missing)}
             """);
    }

    [GeneratedRegex(@"^(?<notice>Copyright \(c\) \d{4} .+?)\s*$", RegexOptions.Multiline)]
    private static partial Regex CopyrightLine();
}
