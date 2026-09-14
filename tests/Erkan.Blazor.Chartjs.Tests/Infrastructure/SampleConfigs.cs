using Erkan.Blazor.Chartjs.Enums;
using Erkan.Blazor.Chartjs.Interfaces;
using Erkan.Blazor.Chartjs.Models;
using Erkan.Blazor.Chartjs.Models.Bar;
using Erkan.Blazor.Chartjs.Models.Bubble;
using Erkan.Blazor.Chartjs.Models.Common;
using Erkan.Blazor.Chartjs.Models.Doughnut;
using Erkan.Blazor.Chartjs.Models.Line;
using Erkan.Blazor.Chartjs.Models.Pie;
using Erkan.Blazor.Chartjs.Models.Polar;
using Erkan.Blazor.Chartjs.Models.Radar;
using Erkan.Blazor.Chartjs.Models.Scatter;

namespace Erkan.Blazor.Chartjs.Tests.Infrastructure;

/// <summary>
/// The configurations every snapshot and hygiene test is built from.
/// </summary>
/// <remarks>
/// Three shapes per chart type, each answering a different question.
/// <list type="bullet">
///   <item><c>Empty</c> — <c>new XChartConfig()</c> and nothing else. Whatever this emits, the
///   models emit unprompted.</item>
///   <item><c>Minimal</c> — the least a working chart needs: labels, one dataset, and an
///   options object. This is where a stray <c>[]</c>, a bare <c>null</c> or a leaked
///   wrapper-internal marker shows up, because nothing here asked for any of them.</item>
///   <item><c>Rich</c> — what a real consumer sets: scales, legend and its title, title and subtitle, the tooltip's styling,
///   placement and layout, datalabels, zoom and the chart-level layout, sizing, colour, hover and events options,
///   exercised as widely as each chart type's options class allows, plus an
///   <c>ExtraOptions</c> entry on every class that has the bag. Each of those entries is a key
///   Chart.js really reads, because the key check validates them like any other, and none
///   repeats a key a typed property already writes.</item>
/// </list>
/// </remarks>
public static class SampleConfigs
{
    public const string Bar = "bar";
    public const string Bubble = "bubble";
    public const string Doughnut = "doughnut";
    public const string Line = "line";
    public const string Pie = "pie";
    public const string Polar = "polarArea";
    public const string Radar = "radar";
    public const string Scatter = "scatter";

    public static readonly IReadOnlyList<string> Kinds =
        [Bar, Bubble, Doughnut, Line, Pie, Polar, Radar, Scatter];

    public static TheoryData<string> AllKinds
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (var kind in Kinds) data.Add(kind);
            return data;
        }
    }

    private static readonly List<string> Labels = ["Jan", "Feb", "Mar", "Apr"];

    // ------------------------------------------------------------------ empty

    public static IChartConfig Empty(string kind) => kind switch
    {
        Bar => new BarChartConfig(),
        Bubble => new BubbleChartConfig(),
        Doughnut => new DoughnutChartConfig(),
        Line => new LineChartConfig(),
        Pie => new PieChartConfig(),
        Polar => new PolarChartConfig(),
        Radar => new RadarChartConfig(),
        Scatter => new ScatterChartConfig(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "unknown chart type"),
    };

    // ---------------------------------------------------------------- minimal

    public static IChartConfig Minimal(string kind)
    {
        switch (kind)
        {
            case Bar:
                return new BarChartConfig
                {
                    Options = new Options(),
                    Data = new BarData
                    {
                        Labels = Labels,
                        Datasets = [new BarDataset { Label = "Revenue", Data = [10, 20, 30, 40] }],
                    },
                };

            case Bubble:
                return new BubbleChartConfig
                {
                    Options = new Options(),
                    Data = new BubbleData
                    {
                        Labels = Labels,
                        Datasets =
                        [
                            new BubbleDataset
                            {
                                Label = "Clusters",
                                Data = [new BubbleCoords { X = 10, Y = 20, R = 5 }],
                            },
                        ],
                    },
                };

            case Doughnut:
                return new DoughnutChartConfig
                {
                    Options = new Options(),
                    Data = new DoughnutData
                    {
                        Labels = Labels,
                        Datasets = [new DoughnutDataset { Label = "Share", Data = [10, 20, 30, 40] }],
                    },
                };

            case Line:
                return new LineChartConfig
                {
                    Options = new Options(),
                    Data = new LineData
                    {
                        Labels = Labels,
                        Datasets = [new LineDataset { Label = "Latency", Data = [10, 20, 30, 40] }],
                    },
                };

            case Pie:
                return new PieChartConfig
                {
                    Options = new PieOptions(),
                    Data = new PieData
                    {
                        Labels = Labels,
                        Datasets = [new PieDataset { Label = "Share", Data = [10, 20, 30, 40] }],
                    },
                };

            case Polar:
                return new PolarChartConfig
                {
                    Options = new Options(),
                    Data = new PolarData
                    {
                        Labels = Labels,
                        Datasets = [new PolarDataset { Label = "Coverage", Data = [10, 20, 30, 40] }],
                    },
                };

            case Radar:
                return new RadarChartConfig
                {
                    Options = new RadarOptions(),
                    Data = new RadarData
                    {
                        Labels = Labels,
                        Datasets = [new RadarDataset { Label = "Skills", Data = [10, 20, 30, 40] }],
                    },
                };

            case Scatter:
                return new ScatterChartConfig
                {
                    Options = new Options(),
                    Data = new ScatterData
                    {
                        Labels = Labels,
                        Datasets =
                        [
                            new ScatterDataset
                            {
                                Label = "Samples",
                                Data = [new ScatterXYValue { X = 1.5m, Y = 2.5m }],
                            },
                        ],
                    },
                };

            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, "unknown chart type");
        }
    }

    // ------------------------------------------------------------------- rich

    public static IChartConfig Rich(string kind) => kind switch
    {
        Bar => RichBar(),
        Bubble => RichBubble(),
        Doughnut => RichDoughnut(),
        Line => RichLine(),
        Pie => RichPie(),
        Polar => RichPolar(),
        Radar => RichRadar(),
        Scatter => RichScatter(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "unknown chart type"),
    };

    /// <summary>Legend, title, subtitle, tooltip and datalabels, configured the way an app that themes its charts would.</summary>
    /// <param name="title">The title text.</param>
    /// <param name="colors">The built-in colors plugin's options, or <c>null</c> to leave it unconfigured.</param>
    private static Plugins StyledPlugins(string title, Colors? colors = null) => new()
    {
        Colors = colors,
        Legend = new Legend
        {
            Display = true,
            Position = LegendPosition.Bottom,
            Align = Align.Center,
            Reverse = false,
            RTL = false,
            TextDirection = TextDirection.LTR,
            FullSize = true,
            Labels = new LegendLabels
            {
                Color = "#1f2933",
                Font = new Font { Family = "Inter", Size = 13, Style = "normal", Weight = "500", LineHeight = "1.2" },
                BoxWidth = 18,
                BoxHeight = 18,
                Padding = 12,
                UsePointStyle = true,
                PointStyle = PointStyle.Circle,
                PointStyleWidth = 10,
                TextAlign = "left",
                UseBorderRadius = true,
                BorderRadius = 4,
            },
            MaxWidth = 480,
            MaxHeight = 64,
            Title = new LegendTitle
            {
                Display = true,
                Text = "Series",
                Position = Align.Start,
                Color = "#3e4c59",
                Font = new Font { Size = 12, Weight = "600" },
                Padding = new TitlePadding(4),
            },
            // maxHeight is typed on Legend now; the legend's layout weight is not
            ExtraOptions = new() { ["weight"] = 1000 },
        },
        Title = new Title
        {
            Display = true,
            Text = title,
            Align = Align.Start,
            Position = Position.Top,
            Color = "#0b1f33",
            FullSize = true,
            Font = new Font { Family = "Inter", Size = 18, Weight = "700" },
            Padding = new TitlePadding { Top = 8, Bottom = 16 },
            ExtraOptions = new() { ["weight"] = 2000 },
        },
        Subtitle = new Title
        {
            Display = true,
            Text = "Sample data",
            Align = Align.Start,
            Position = Position.Top,
            Color = "#52606d",
            FullSize = true,
            Font = new Font { Size = 13, Style = "italic" },
            Padding = new TitlePadding { Bottom = 12 },
            ExtraOptions = new() { ["weight"] = 1500 },
        },
        Tooltip = new Tooltip
        {
            Enabled = true,
            Mode = InteractionMode.Index,
            Intersect = false,
            Axis = AxisInteractions.X,
            IncludeInvisible = false,
            Position = TooltipPosition.Nearest,
            XAlign = TooltipXAlign.Left,
            YAlign = TooltipYAlign.Center,
            BackgroundColor = "rgba(11,31,51,0.92)",
            TitleColor = "#ffffff",
            TitleFont = new Font { Size = 14, Weight = "600" },
            TitleAlign = TextAlign.Center,
            TitleSpacing = 4,
            TitleMarginBottom = 8,
            BodyColor = "#dbe4ee",
            BodyFont = new Font { Size = 13 },
            BodyAlign = TextAlign.Left,
            BodySpacing = 4,
            FooterColor = "#9fb3c8",
            FooterFont = new Font { Size = 12, Style = "italic" },
            FooterAlign = TextAlign.Right,
            FooterSpacing = 0,
            FooterMarginTop = 10,
            Padding = new Padding(10, 12, 10, 12),
            CaretPadding = 6,
            CaretSize = 8,
            CornerRadius = 4,
            BorderColor = "#334e68",
            BorderWidth = 1,
            MultiKeyBackground = "#102a43",
            DisplayColors = true,
            BoxWidth = 10,
            BoxHeight = 10,
            BoxPadding = 4,
            UsePointStyle = true,
            RTL = false,
            TextDirection = TextDirection.LTR,
            // cornerRadius and displayColors are typed on Tooltip now; the tooltip's own animation is not
            ExtraOptions = new() { ["animation"] = new { duration = 150, easing = "easeOutQuad" } },
        },
        DataLabels = new DataLabels
        {
            Align = DatalabelsAlign.End,
            Anchor = DatalabelsAnchor.End,
            BackgroundColor = "#ffffff",
            BorderColor = "#334e68",
            BorderRadius = 4,
            BorderWidth = 1,
            Clamp = true,
            Clip = false,
            Color = "#102a43",
            Font = new Font { Size = 11, Weight = "600" },
            Offset = 6,
            Opacity = 0.9m,
            Padding = new Padding { Top = 2, Right = 4, Bottom = 2, Left = 4 },
            Rotation = 0,
            TextAlign = DatalabelsTextAlign.Center,
            TextStrokeColor = "#ffffff",
            TextStrokeWidth = 1,
            textShadowBlur = 2,
            TextShadowColor = "rgba(0,0,0,0.25)",
        },
        // plugins.subtitle is typed now (Subtitle above); the filler plugin's options are not
        ExtraOptions = new()
        {
            ["filler"] = new { propagate = true },
        },
    };

    /// <summary>A cartesian x/y pair covering ticks, grid, the Chart.js 4 border object and a title.</summary>
    private static Dictionary<string, Axis> CartesianScales() => new()
    {
        [Scales.XAxisId] = new Axis
        {
            Type = "category",
            Display = true,
            Position = Position.Bottom,
            Grid = new Grid { Display = true, Color = "#e4e7eb", DrawOnChartArea = false, DrawTicks = true },
            Border = new Border { Display = true, Color = "#9aa5b1", Width = 1, Dash = [4, 4], DashOffset = 2, Z = 1 },
            Ticks = new Ticks
            {
                Color = "#52606d",
                Font = new Font { Size = 12 },
                AutoSkip = true,
                MaxRotation = 45,
                MinRotation = 0,
                MaxTicksLimit = 8,
                // CrossAlign lives in Erkan.Blazor.Chartjs.Models, not alongside its eleven siblings
                CrossAlign = global::Erkan.Blazor.Chartjs.Models.CrossAlign.Near,
            },
            Title = new AxesTitle { Display = true, Text = "Month", Color = "#3e4c59", Align = Align.Center, Font = new Font { Size = 13 } },
        },
        [Scales.YAxisId] = new Axis
        {
            Type = "linear",
            Display = true,
            Position = Position.Left,
            BeginAtZero = true,
            Min = 0,
            Max = 100,
            SuggestedMin = 0,
            SuggestedMax = 120,
            Stacked = false,
            Grid = new Grid { Display = true, Color = "#f0f4f8", DrawOnChartArea = true },
            Border = new Border { Display = false },
            Ticks = new Ticks { Color = "#52606d", StepSize = 20, MaxTicksLimit = 6 },
            Title = new AxesTitle { Display = true, Text = "Requests", Align = Align.Center },
            ExtraOptions = new() { ["grace"] = "5%", ["reverse"] = false },
        },
    };

    /// <summary>Pan, wheel, pinch, drag and limits — the whole plugin surface the wrapper exposes.</summary>
    private static Zoom FullZoom() => new()
    {
        Mode = "xy",
        OverScaleMode = "y",
        Pan = new Pan { Enabled = true, Mode = "x", ModifierKey = "shift", Threshold = 0, OverScaleMode = "y" },
        ZoomOptions = new ZoomOptions
        {
            ScaleMode = "xy",
            Wheel = new Wheel { Enabled = true, Speed = 0.05m, ModifierKey = "ctrl" },
            Pinch = new Pinch { Enabled = true },
            Drag = new Drag
            {
                Enabled = true,
                BackgroundColor = "rgba(54,162,235,0.3)",
                BorderColor = "rgb(54,162,235)",
                BorderWidth = 1,
                ModifierKey = "alt",
                Threshold = 0,
            },
        },
        Limits = new Limits
        {
            // "original" and a number take different JSON shapes; both are exercised
            X = new ScaleLimits { Min = "original", Max = "original", MinRange = 2 },
            Y = new ScaleLimits { Min = "0", Max = "200", MinRange = 10 },
        },
    };

    private static BarChartConfig RichBar() => new()
    {
        Data = new BarData
        {
            Labels = Labels,
            Datasets =
            [
                new BarDataset
                {
                    Label = "Revenue",
                    Data = [10, 20, 30, 40],
                    BackgroundColor = ["#3ebd93", "#65d6ad", "#8eedc7", "#c6f7e2"],
                    BorderColor = ["#199473"],
                    BorderWidth = 2,
                    HoverBackgroundColor = ["#147d64"],
                    Fill = true,
                    Stack = "primary",
                    Order = 1,
                    DataLabels = new DataLabels { Color = "#102a43", Offset = 0, Clamp = false },
                    ExtraOptions = new() { ["borderRadius"] = 6, ["borderSkipped"] = false },
                },
            ],
        },
        // every chart-level option; the other rich configs each take a subset
        Options = new Options
        {
            Responsive = true,
            MaintainAspectRatio = false,
            AspectRatio = 2,
            ResizeDelay = 0,
            DevicePixelRatio = 2,
            Color = "#243b53",
            BackgroundColor = "rgba(62,189,147,0.2)",
            BorderColor = "#199473",
            Layout = new Layout { AutoPadding = false, Padding = new Padding(8, 16, 8, 16) },
            Hover = new Interaction
            {
                Mode = InteractionMode.Index,
                Axis = AxisInteractions.X,
                Intersect = false,
                IncludeInvisible = true,
            },
            Events = ["mousemove", "mouseout", "click"],
            IndexAxis = Axes.X,
            Locale = "tr-TR",
            Animation = true,
            Animations = new Animations
            {
                Colors = true,
                X = false,
                Tension = new Tension { Duration = 1000, Easing = "linear", From = 1, To = 0, Loop = true, Delay = 0 },
            },
            Interaction = new Interaction
            {
                Mode = InteractionMode.Index,
                Axis = AxisInteractions.X,
                Intersect = false,
                IncludeInvisible = false,
            },
            Elements = new Elements { Line = new Line { BorderColor = "#334e68", BorderWidth = 2 } },
            Scales = CartesianScales(),
            Plugins = StyledPlugins("Revenue by month", new Colors { Enabled = true, ForceOverride = true }),
            RegisterPlugins = ["chartjs-plugin-autocolors"],
            // options.layout is typed now (Options.Layout above); transitions still is not
            ExtraOptions = new()
            {
                ["transitions"] = new { active = new { animation = new { duration = 0 } } },
            },
        },
    };

    private static BubbleChartConfig RichBubble()
    {
        var plugins = StyledPlugins("Cluster density", new Colors { Enabled = true });
        plugins.Zoom = FullZoom();
        return new BubbleChartConfig
        {
            Data = new BubbleData
            {
                Labels = Labels,
                Datasets =
                [
                    new BubbleDataset
                    {
                        Label = "Clusters",
                        BackgroundColor = "rgba(101,214,173,0.7)",
                        ExtraOptions = new() { ["borderColor"] = "#199473", ["borderWidth"] = 1 },
                        Data =
                        [
                            new BubbleCoords { X = 10, Y = 20, R = 5 },
                            new BubbleCoords { X = 0, Y = 0, R = 0 },
                        ],
                    },
                ],
            },
            Options = new Options
            {
                // null on both hands the decision back to Chart.js: neither key is written
                Responsive = null,
                MaintainAspectRatio = null,
                DevicePixelRatio = 1.5,
                ResizeDelay = 100,
                Scales = CartesianScales(),
                Plugins = plugins,
            },
        };
    }

    private static DoughnutChartConfig RichDoughnut() => new()
    {
        Data = new DoughnutData
        {
            Labels = Labels,
            Datasets =
            [
                new DoughnutDataset
                {
                    Label = "Share",
                    Data = [10, 20, 30, 40],
                    BackgroundColor = ["#ef4e4e", "#f9703e", "#f7c948", "#3ebd93"],
                    BorderWidth = 0,
                    HoverOffset = 12,
                },
            ],
        },
        Options = new Options
        {
            Responsive = true,
            MaintainAspectRatio = true,
            AspectRatio = 1,
            Color = "#334e68",
            Layout = new Layout { Padding = new Padding(12) },
            Plugins = StyledPlugins("Traffic share", new Colors { Enabled = false }),
        },
    };

    private static LineChartConfig RichLine()
    {
        var plugins = StyledPlugins("Latency over time");
        plugins.Zoom = FullZoom();
        // a two-line title: the lines replace the single Text StyledPlugins set
        plugins.Title!.TextLines = ["Latency over time", "p99 and errors"];

        var scales = CartesianScales();
        scales["x"] = new Axis
        {
            Type = "time",
            Display = true,
            Position = Position.Bottom,
            Time = new AxesTime
            {
                Unit = TimeUnit.Day,
                MinUnit = TimeUnit.Hour,
                Round = TimeUnit.Day,
                TooltipFormat = "DD/MM/YYYY",
                IsoWeekday = 1,
                DisplayFormats = new AxesTimeFormats { Day = "DD MMM", Hour = "HH:mm", Month = "MMM YYYY" },
            },
            // 'source' belongs to ticks, not to time: Chart.js reads scales[].ticks.source
            Ticks = new Ticks { Source = "auto", Color = "#52606d", AutoSkip = true },
            Title = new AxesTitle { Display = true, Text = "Time" },
        };
        scales["y2"] = new Axis
        {
            Type = "linear",
            Position = Position.Right,
            Display = true,
            Grid = new Grid { DrawOnChartArea = false },
            Ticks = new Ticks { Color = "#829ab1" },
        };

        return new LineChartConfig
        {
            Data = new LineData
            {
                Labels = Labels,
                Datasets =
                [
                    new LineDataset
                    {
                        Label = "p99",
                        Data = [10, 20, 30, 40],
                        BackgroundColor = "rgba(62,189,147,0.2)",
                        BorderColor = "#199473",
                        BorderWidth = 2,
                        BorderDash = [5, 2.5m],
                        BorderDashOffset = 0,
                        BorderCapStyle = BorderCapStyle.Round,
                        BorderJoinStyle = BorderJoinStyle.Bevel,
                        Clip = 4,
                        Fill = false,
                        Tension = 0,
                        PointRadius = 3,
                        PointStyle = PointStyle.Circle,
                        PointBackgroundColor = ["#199473", "#3ebd93", "#e12d39", "#199473"],
                        PointBorderColor = ["#ffffff"],
                        PointBorderWidth = 1,
                        PointHitRadius = 6,
                        PointRotation = 0,
                        PointHoverRadius = 5,
                        PointHoverBackgroundColor = ["#0b1f33"],
                        PointHoverBorderColor = ["#ffffff"],
                        PointHoverBorderWidth = 2,
                        HoverBackgroundColor = "rgba(62,189,147,0.4)",
                        HoverBorderColor = "#147d64",
                        HoverBorderWidth = 3,
                        CubicInterpolationMode = CubicInterpolationMode.Default,
                        // StepMode lives in ...Models.Common.StringEnums, again unlike its siblings
                        StepMode = global::Erkan.Blazor.Chartjs.Models.Common.StringEnums.StepMode.False,
                        SpanGaps = false,
                        ShowLine = true,
                        DrawActiveElementsOnTop = false,
                        XAxisId = Scales.XAxisId,
                        YAxisId = Scales.YAxisId,
                        Order = 1,
                    },
                    new LineDataset
                    {
                        Label = "errors",
                        Data = [1, 2, 3, 4],
                        BorderColor = "#e12d39",
                        YAxisId = Scales.Y2AxisId,
                        Tension = 0.4m,
                        Fill = true,
                        SpanGaps = true,
                        StepMode = global::Erkan.Blazor.Chartjs.Models.Common.StringEnums.StepMode.Middle,
                        Stack = "errors",
                        // spanGaps and borderDash are typed on LineDataset now; these two are not
                        ExtraOptions = new() { ["hoverBorderDash"] = new[] { 6, 3 }, ["hidden"] = false },
                    },
                ],
            },
            Options = new Options
            {
                Responsive = true,
                Scales = scales,
                Plugins = plugins,
                Interaction = new Interaction { Mode = InteractionMode.Nearest, Intersect = false },
                // hover overrides interaction for hover alone; what it leaves unset falls back to it
                Hover = new Interaction { Intersect = true },
                Events = ["mousemove", "mouseout", "click", "touchstart", "touchmove"],
                Layout = new Layout { Padding = new Padding { Top = 24 } },
            },
        };
    }

    private static PieChartConfig RichPie() => new()
    {
        Data = new PieData
        {
            Labels = Labels,
            Datasets =
            [
                new PieDataset
                {
                    Label = "Share",
                    Data = [10, 20, 30, 40],
                    BackgroundColor = ["#2bb0ed", "#5ed0fa", "#81defd", "#b3ecff"],
                    BorderWidth = 1,
                    HoverOffset = 8,
                },
            ],
        },
        Options = new PieOptions
        {
            Responsive = true,
            Circumference = 360,
            Rotation = 0,
            ResizeDelay = 250,
            Layout = new Layout { AutoPadding = true },
            Plugins = StyledPlugins("Share of voice", new Colors { ForceOverride = false }),
        },
    };

    private static PolarChartConfig RichPolar() => new()
    {
        Data = new PolarData
        {
            Labels = Labels,
            Datasets =
            [
                new PolarDataset
                {
                    Label = "Coverage",
                    Data = [10, 20, 30, 40],
                    BackgroundColor = ["#da127d", "#e8368f", "#f364a2", "#ff8cba"],
                    BorderWidth = 1,
                },
            ],
        },
        Options = new Options
        {
            Responsive = true,
            BorderColor = "#ffffff",
            Plugins = StyledPlugins("Coverage by area"),
        },
    };

    /// <remarks>
    /// Radar is the odd one out: <see cref="RadarOptions"/> is a separate class carrying only
    /// elements, scales, responsive and maintainAspectRatio — it has no Plugins property at
    /// all, so a radar chart's legend and title can only be given through
    /// <see cref="RadarOptions.ExtraOptions"/>, which is what this configuration does.
    /// </remarks>
    private static RadarChartConfig RichRadar() => new()
    {
        Data = new RadarData
        {
            Labels = Labels,
            Datasets =
            [
                new RadarDataset
                {
                    Label = "Skills",
                    Data = [10, 20, 30, 40],
                    BackgroundColor = "rgba(101,31,255,0.2)",
                    BorderColor = "#5a2ebf",
                    BorderWidth = 2,
                    Fill = false,
                    PointBackgroundColor = "#5a2ebf",
                    PointBorderColor = "#ffffff",
                    PointHoverBackgroundColor = "#ffffff",
                    PointHoverBorderColor = "#5a2ebf",
                },
            ],
        },
        Options = new RadarOptions
        {
            Responsive = true,
            MaintainAspectRatio = false,
            Elements = new RadarOptionsElements { Line = new RadarOptionsElementsLine { BorderWidth = 0 } },
            Scales = new RadarOptionsScales
            {
                R = new RadarOptionsScalesRadius { BeginAtZero = true, Min = 0, Max = 50 },
            },
            // RadarOptions has no Plugins property; the bag is the only way to a radar legend
            ExtraOptions = new()
            {
                ["plugins"] = new { legend = new { position = "right" }, title = new { display = true, text = "Skills" } },
            },
        },
    };

    private static ScatterChartConfig RichScatter()
    {
        var plugins = StyledPlugins("Samples", new Colors { ForceOverride = true });
        plugins.Zoom = FullZoom();
        // the tooltip describes the one sample under the pointer, and clearing XAlign/YAlign
        // hands the caret sides back to Chart.js: neither key is written
        plugins.Tooltip!.Mode = InteractionMode.Point;
        plugins.Tooltip.Intersect = true;
        plugins.Tooltip.Axis = AxisInteractions.XY;
        plugins.Tooltip.XAlign = null;
        plugins.Tooltip.YAlign = null;
        plugins.Tooltip.DisplayColors = false;
        return new ScatterChartConfig
        {
            Data = new ScatterData
            {
                Labels = Labels,
                Datasets =
                [
                    new ScatterDataset
                    {
                        Label = "Samples",
                        BackgroundColor = "#f0b429",
                        BorderColor = "#b44d12",
                        BorderWidth = 1,
                        PointRadius = 4,
                        PointHitRadius = 6,
                        PointStyle = PointStyle.Triangle,
                        ShowLine = false,
                        Tension = 0,
                        YAxisId = Scales.YAxisId,
                        ExtraOptions = new() { ["hidden"] = false, ["pointHoverRadius"] = 8 },
                        Data =
                        [
                            new ScatterXYValue { X = 1.5m, Y = 2.5m },
                            new ScatterXYValue { X = 0m, Y = 0m },
                        ],
                    },
                ],
            },
            Options = new Options
            {
                Responsive = true,
                MaintainAspectRatio = true,
                AspectRatio = 1.5,
                Hover = new Interaction { Mode = InteractionMode.Point, Axis = AxisInteractions.XY },
                Events = ["click"],
                Scales = CartesianScales(),
                Plugins = plugins,
            },
        };
    }
}
