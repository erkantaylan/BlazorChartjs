namespace Erkan.Blazor.Chartjs.Models.Common
{
    /// <summary>
    /// Zoom. Serialized as <c>plugins.zoom</c>.
    /// </summary>
    public class Zoom
    {
        private string? _mode;
        private bool _modeSet;
        private string? _overScaleMode;
        private bool _overScaleModeSet;
        private ZoomOptions? _zoomOptions;

        /// <summary>
        /// The instance the pushed values were last written into, and the values that
        /// were written. Together they tell a value this class put into
        /// <see cref="ZoomOptions"/> apart from one the caller set there, so the
        /// caller's value is never overwritten and the result does not depend on the
        /// order the properties are assigned in.
        /// </summary>
        private ZoomOptions? _pushTarget;
        private string? _pushedMode;
        private string? _pushedOverScaleMode;

        /// <summary>
        /// Gets or sets the limits.
        /// </summary>
        /// <value>
        /// The limits.
        /// </value>
        [JsonPropertyName("limits")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Limits? Limits { get; set; }

        /// <summary>
        /// Gets or sets the zoom direction.
        /// </summary>
        /// <value>
        /// The mode. Values: x, y, xy.
        /// </value>
        /// <remarks>
        /// The plugin reads the mode from <c>plugins.zoom.zoom.mode</c>, so this value is
        /// written into <see cref="ZoomOptions"/> rather than next to it. A mode set on
        /// <see cref="ZoomOptions"/> directly wins, whichever order the two are assigned in.
        /// The getter reports the value that will actually be serialized.
        /// </remarks>
        [JsonIgnore]
        public string? Mode
        {
            get => _zoomOptions != null ? _zoomOptions.Mode : _mode;
            set
            {
                _mode = value;
                _modeSet = true;
                PushNestedZoomOptions();
            }
        }

        /// <summary>
        /// Gets or sets the over scale mode.
        /// </summary>
        /// <value>
        /// The over scale mode. Values: x, y, xy.
        /// </value>
        /// <remarks>
        /// Like <see cref="Mode"/>, the plugin reads this from <c>plugins.zoom.zoom</c>,
        /// a value set on <see cref="ZoomOptions"/> directly wins, and the result does not
        /// depend on the order the properties are assigned in.
        /// </remarks>
        [JsonIgnore]
        public string? OverScaleMode
        {
            get => _zoomOptions != null ? _zoomOptions.OverScaleMode : _overScaleMode;
            set
            {
                _overScaleMode = value;
                _overScaleModeSet = true;
                PushNestedZoomOptions();
            }
        }

        /// <summary>
        /// Gets or sets the pan.
        /// </summary>
        /// <value>
        /// The pan.
        /// </value>
        [JsonPropertyName("pan")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pan? Pan { get; set; }

        /// <summary>
        /// Gets or sets the zoom options.
        /// </summary>
        /// <value>
        /// The zoom options.
        /// </value>
        [JsonPropertyName("zoom")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ZoomOptions? ZoomOptions
        {
            get => _zoomOptions;
            set
            {
                _zoomOptions = value;
                PushNestedZoomOptions();
            }
        }

        /// <summary>
        /// Copies the flat <see cref="Mode"/> / <see cref="OverScaleMode"/> values into the
        /// nested zoom options the plugin actually reads, without overwriting anything the
        /// caller set there directly.
        /// </summary>
        /// <remarks>
        /// Runs from all three setters, because an object initializer may assign them in
        /// any order. A value is (re)written when the target slot is empty or still holds
        /// what this instance last put there; a value the caller set on
        /// <see cref="ZoomOptions"/> itself is left alone. That makes the outcome
        /// order-independent: <c>new Zoom { Mode = "x", ZoomOptions = o }</c> and
        /// <c>new Zoom { ZoomOptions = o, Mode = "x" }</c> serialize identically.
        /// </remarks>
        private void PushNestedZoomOptions()
        {
            if (!_modeSet && !_overScaleModeSet)
                return;

            if (_zoomOptions == null)
            {
                // Nothing to write: do not materialize an empty "zoom": {} object.
                if (_mode == null && _overScaleMode == null)
                    return;

                _zoomOptions = new ZoomOptions();
            }

            var ownsTarget = ReferenceEquals(_pushTarget, _zoomOptions);

            if (_modeSet && (_zoomOptions.Mode == null || (ownsTarget && _zoomOptions.Mode == _pushedMode)))
            {
                _zoomOptions.Mode = _mode;
                _pushedMode = _mode;
            }

            if (_overScaleModeSet
                && (_zoomOptions.OverScaleMode == null || (ownsTarget && _zoomOptions.OverScaleMode == _pushedOverScaleMode)))
            {
                _zoomOptions.OverScaleMode = _overScaleMode;
                _pushedOverScaleMode = _overScaleMode;
            }

            _pushTarget = _zoomOptions;
        }
    }
}
