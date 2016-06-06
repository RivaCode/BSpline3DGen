using System;
using System.Xml.Linq;

namespace Bspline.Core
{
    public class SettingsNotification
    {
        /// <summary>
        /// Class that contain the settings
        /// </summary>
        public const string RENDER_MODE = "render_mode";
        public const string NUM_TO_FILTER = "filter_points";
        public const string SHOW_COORDINATES = "show_coor";
        public const string USE_VOICE_COMMANDS = "voice_command";
        /// <summary>
        /// containe the default mode of the surface
        /// </summary>
        public RenderMode DefaultRenderMode { get; private set; }
        /// <summary>
        /// number of points that we chose to filter
        /// </summary>
        public int PointsToFilter { get; private set; }
        /// <summary>
        /// triger that decide to use or not use voice comand
        /// </summary>
        public bool UseVoiceCommands { get; private set; }
        /// <summary>
        /// enable or disable coardinates display
        /// </summary>
        public bool ShowCoordinates { get; private set; }
        /// <summary>
        /// contain the default setting 
        /// </summary>
        /// <param name="settingsElement"></param>
        public SettingsNotification( XElement settingsElement )
        {
            string value = BSplineUtility.Instance.GetElementValue( settingsElement, RENDER_MODE );

            RenderMode render;
            this.DefaultRenderMode = Enum.TryParse( value, out render ) ? render : RenderMode.Regular2D;

            value = BSplineUtility.Instance.GetElementValue( settingsElement, NUM_TO_FILTER );
            int points;
            this.PointsToFilter = int.TryParse( value, out points ) ? points : 3;

            value = BSplineUtility.Instance.GetElementValue( settingsElement, SHOW_COORDINATES );
            bool show;
            this.ShowCoordinates = !bool.TryParse( value, out show ) || show;

            value = BSplineUtility.Instance.GetElementValue( settingsElement, USE_VOICE_COMMANDS );
            this.UseVoiceCommands = !bool.TryParse( value, out show ) || show;
        }
        /// <summary>
        /// one more way to bulide default settings
        /// </summary>
        /// <param name="defaultRenderMode"></param>
        /// <param name="pointsToFilter"></param>
        /// <param name="showCoordinates"></param>
        /// <param name="useVoicCommands"></param>

        public SettingsNotification( RenderMode defaultRenderMode,
            int pointsToFilter,
            bool showCoordinates,
            bool useVoicCommands )
        {
            this.DefaultRenderMode = defaultRenderMode;
            this.PointsToFilter = pointsToFilter;
            this.ShowCoordinates = showCoordinates;
            this.UseVoiceCommands = useVoicCommands;
        }
    }
}
