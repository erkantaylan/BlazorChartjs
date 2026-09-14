using System.Text.Json;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Tests.Infrastructure;

namespace Erkan.Blazor.Chartjs.Tests;

/// <summary>
/// The subtitle, multi-line title text, the legend's title and the legend's size limits:
/// <see cref="Plugins.Subtitle"/>, <see cref="Title.TextLines"/>, <see cref="LegendTitle"/>,
/// <see cref="Legend.MaxWidth"/> and <see cref="Legend.MaxHeight"/>.
/// </summary>
/// <remarks>
/// The snapshots record what the rich configs emit, and the key validation proves every key exists
/// somewhere in Chart.js. Neither says that the subtitle and the title write the same keys one
/// level apart, that <c>Text</c> and <c>TextLines</c> never both reach the one <c>text</c> key, or
/// that the legend title's <c>position</c> is an alignment — so those are asserted here.
/// </remarks>
public class TitleAndLegendOptionsTests
{
    private static readonly OptionPathSet ChartJsPaths = ChartJsKeyList.Current.Paths;

    private static Title EveryTitleOptionSet() => new()
    {
        Align = Align.End,
        Color = "#102a43",
        Display = true,
        FullSize = false,
        Position = Position.Bottom,
        Text = "Q3",
        Padding = new TitlePadding(2, 6),
        Font = new Font { Size = 11 },
    };

    // ------------------------------------------------------------------ subtitle

    /// <summary>
    /// Chart.js's subtitle plugin reads exactly the title's options, so the subtitle is a
    /// <see cref="Title"/>, and every key it writes lands under <c>plugins.subtitle</c>.
    /// </summary>
    [Fact]
    public void Subtitle_is_a_Title_written_to_plugins_subtitle()
    {
        Assert.Equal(typeof(Title), typeof(Plugins).GetProperty(nameof(Plugins.Subtitle))!.PropertyType);

        var nodes = JsonPaths.Enumerate(new Options { Plugins = new Plugins { Legend = null, Subtitle = EveryTitleOptionSet() } })
            .Where(node => node.Path.StartsWith("plugins.", StringComparison.Ordinal))
            .ToList();

        Assert.NotEmpty(nodes);
        foreach (var node in nodes)
        {
            Assert.StartsWith("plugins.subtitle", node.Path, StringComparison.Ordinal);
            Assert.True(ChartJsPaths.Contains($"options.{node.Path}"), $"options.{node.Path} is not a Chart.js 4.5.1 option.");
        }
    }

    /// <summary>The same settings write the same object, whether they are the title or the subtitle.</summary>
    [Fact]
    public void Title_and_subtitle_write_the_same_object()
    {
        using var document = ChartJson.SerializeToDocument(new Plugins
        {
            Legend = null,
            Title = EveryTitleOptionSet(),
            Subtitle = EveryTitleOptionSet(),
        });

        Assert.Equal(
            document.RootElement.GetProperty("title").GetRawText(),
            document.RootElement.GetProperty("subtitle").GetRawText());
    }

    [Fact]
    public void An_unset_subtitle_writes_no_key()
    {
        Assert.DoesNotContain("subtitle", ChartJson.Serialize(new Plugins()), StringComparison.Ordinal);
    }

    // ---------------------------------------------------------------------- text

    [Fact]
    public void Text_writes_a_string()
    {
        Assert.Equal("""{"text":"Revenue"}""", ChartJson.Serialize(new Title { Text = "Revenue" }));
    }

    [Fact]
    public void TextLines_writes_an_array_with_one_entry_per_line()
    {
        var title = new Title { TextLines = ["Revenue", "by month"] };

        Assert.Equal("""{"text":["Revenue","by month"]}""", ChartJson.Serialize(title));
        Assert.Null(title.Text);
    }

    /// <summary>
    /// <c>Text</c> and <c>TextLines</c> are two ways to set the one <c>text</c> key. Assigning either
    /// clears the other, so the object never holds two answers and the key is never written twice.
    /// </summary>
    [Fact]
    public void Assigning_one_form_of_the_text_clears_the_other()
    {
        var title = new Title { Text = "Revenue", TextLines = ["Revenue", "by month"] };
        Assert.Null(title.Text);
        Assert.Equal(["Revenue", "by month"], title.TextLines);
        Assert.Equal("""{"text":["Revenue","by month"]}""", ChartJson.Serialize(title));

        title.Text = "Costs";
        Assert.Null(title.TextLines);
        Assert.Equal("""{"text":"Costs"}""", ChartJson.Serialize(title));
    }

    /// <summary>Clearing one form is not assigning a text, so it leaves the other alone.</summary>
    [Fact]
    public void Assigning_null_to_one_form_leaves_the_other()
    {
        var single = new Title { Text = "Revenue" };
        single.TextLines = null;
        Assert.Equal("Revenue", single.Text);

        var lines = new Title { TextLines = ["Revenue", "by month"] };
        lines.Text = null;
        Assert.Equal(["Revenue", "by month"], lines.TextLines);
    }

    [Fact]
    public void An_untouched_Title_writes_an_empty_object()
    {
        Assert.Equal("{}", ChartJson.Serialize(new Title()));
    }

    [Fact]
    public void A_multi_line_subtitle_is_written_as_an_array()
    {
        var paths = JsonPaths.Map(new Plugins { Legend = null, Subtitle = new Title { TextLines = ["one", "two"] } });

        Assert.Equal("""["one","two"]""", paths["subtitle.text"].GetRawText());
    }

    /// <summary>
    /// <c>text</c> is written by a private <c>[JsonInclude]</c> property. The declared-key walk must
    /// still see it, or <c>plugins.title.text</c> would leave the exhaustive key check silently.
    /// </summary>
    [Fact]
    public void The_text_key_stays_on_the_declared_surface()
    {
        var owners = ModelGraph.AllKeys
            .Where(key => key.Path is "options.plugins.title.text" or "options.plugins.subtitle.text")
            .Select(key => key.Property)
            .ToList();

        Assert.Equal(2, owners.Count);
        Assert.All(owners, property => Assert.Equal(typeof(Title), property.DeclaringType));
    }

    // -------------------------------------------------------------- legend title

    private static LegendTitle EveryLegendTitleOptionSet() => new()
    {
        Color = "#3e4c59",
        Display = true,
        Font = new Font { Size = 12 },
        Padding = new TitlePadding(3, 5),
        Position = Align.End,
        Text = "Series",
    };

    public static TheoryData<string, string> LegendTitleKeys => new()
    {
        { "color", "\"#3e4c59\"" },
        { "display", "true" },
        { "font.size", "12" },
        { "padding.top", "3" },
        { "padding.bottom", "5" },
        { "position", "\"end\"" },
        { "text", "\"Series\"" },
    };

    [Theory]
    [MemberData(nameof(LegendTitleKeys))]
    public void Each_legend_title_option_writes_its_value_at_the_path_ChartJs_reads(string path, string json)
    {
        var paths = JsonPaths.Map(new Options { Plugins = new Plugins { Legend = new Legend { Title = EveryLegendTitleOptionSet() } } });
        var key = $"plugins.legend.title.{path}";

        Assert.True(paths.TryGetValue(key, out var written), $"options.{key} was not written.");
        Assert.Equal(json, written.GetRawText());
        Assert.True(ChartJsPaths.Contains($"options.{key}"), $"options.{key} is not a Chart.js 4.5.1 option.");
    }

    /// <summary>The theory above covers every property: one that is added later without a row fails here.</summary>
    [Fact]
    public void Every_key_the_fully_set_legend_title_writes_has_a_row()
    {
        var expected = LegendTitleKeys.Select(row => (string)row[0]).ToHashSet(StringComparer.Ordinal);
        var written = JsonPaths.Map(EveryLegendTitleOptionSet()).Keys
            .Where(k => k is not ("font" or "padding"))
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(expected.Order(StringComparer.Ordinal), written.Order(StringComparer.Ordinal));
    }

    /// <summary>
    /// Chart.js names the key <c>position</c> but places the title with <c>_alignStartEnd</c>, so it
    /// takes <c>start</c>, <c>center</c> and <c>end</c> — the values <see cref="Align"/> carries.
    /// </summary>
    [Fact]
    public void The_legend_title_position_is_an_alignment()
    {
        Assert.Equal(typeof(Align), typeof(LegendTitle).GetProperty(nameof(LegendTitle.Position))!.PropertyType);

        Assert.Equal(
            ["start", "center", "end"],
            new[] { Align.Start, Align.Center, Align.End }.Select(a => new LegendTitle { Position = a }.PositionString));
    }

    [Fact]
    public void The_legend_title_padding_is_the_vertical_pair()
    {
        Assert.Equal(typeof(TitlePadding), typeof(LegendTitle).GetProperty(nameof(LegendTitle.Padding))!.PropertyType);

        var paths = JsonPaths.Map(new LegendTitle { Padding = new TitlePadding(6) });
        Assert.Equal(6, paths["padding.top"].GetInt32());
        Assert.Equal(6, paths["padding.bottom"].GetInt32());
    }

    [Fact]
    public void An_untouched_LegendTitle_writes_an_empty_object()
    {
        Assert.Equal("{}", ChartJson.Serialize(new LegendTitle()));
    }

    /// <summary><c>Display = false</c> is how a legend title that a theme turned on is switched back off.</summary>
    [Fact]
    public void A_hidden_legend_title_writes_display_false()
    {
        Assert.Equal("""{"display":false}""", ChartJson.Serialize(new LegendTitle { Display = false }));
    }

    // ------------------------------------------------------------- legend limits

    [Fact]
    public void MaxWidth_and_MaxHeight_are_written_on_the_legend_itself()
    {
        var paths = JsonPaths.Map(new Options { Plugins = new Plugins { Legend = new Legend { MaxWidth = 320, MaxHeight = 48 } } });

        Assert.Equal(320, paths["plugins.legend.maxWidth"].GetInt32());
        Assert.Equal(48, paths["plugins.legend.maxHeight"].GetInt32());
        Assert.True(ChartJsPaths.Contains("options.plugins.legend.maxWidth"));
        Assert.True(ChartJsPaths.Contains("options.plugins.legend.maxHeight"));
    }

    [Fact]
    public void MaxWidth_and_MaxHeight_are_nullable_and_omitted_when_unset()
    {
        Assert.Equal(typeof(int?), typeof(Legend).GetProperty(nameof(Legend.MaxWidth))!.PropertyType);
        Assert.Equal(typeof(int?), typeof(Legend).GetProperty(nameof(Legend.MaxHeight))!.PropertyType);

        var json = ChartJson.Serialize(new Legend());
        Assert.DoesNotContain("maxWidth", json, StringComparison.Ordinal);
        Assert.DoesNotContain("maxHeight", json, StringComparison.Ordinal);
    }

    [Fact]
    public void A_legend_title_is_written_under_the_legend_and_not_the_chart_title()
    {
        using var document = ChartJson.SerializeToDocument(new Plugins { Legend = new Legend { Title = new LegendTitle { Text = "Series" } } });

        Assert.Equal(JsonValueKind.Object, document.RootElement.GetProperty("legend").GetProperty("title").ValueKind);
        Assert.False(document.RootElement.TryGetProperty("title", out _));
    }
}
