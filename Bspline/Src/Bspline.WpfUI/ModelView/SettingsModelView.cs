using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Bspline.Core;
using Bspline.WpfUI.Commands;

namespace Bspline.WpfUI.ModelView
{
    /// <summary>
    /// Class to hold window interaction of application settings
    /// </summary>
    public class SettingsModelView : ModelViewBase
    {
        #region Properties
        /// <summary>
        /// Property to hold info if settings is valid
        /// </summary>
        private bool SettingsDataIsValid { get; set; }

        /// <summary>
        /// Property to hold info if settings is changes
        /// </summary>
        public bool SettingsChanged
        {
            get { return this.SettingsDataIsValid; }
        }

        #region View binded


        /// <summary>
        /// Property to hold the save settings window command <see cref="ICommand"/>
        /// </summary>
        public ICommand SaveCommand
        {
            get { return new RelayCommand(obj => this.SettingsDataIsValid = true); }
        }

        /// <summary>
        /// Property to hold show coordinate info
        /// </summary>
        public bool ShowCoordinate { get; set; }

        /// <summary>
        /// property to hold number of points to filter
        /// </summary>
        public int NumOfPointToFilter { get; set; }

        /// <summary>
        /// Property to hold the rendering mode for application to start with
        /// </summary>
        public RenderMode SelectedRenderMode { get; set; }

        /// <summary>
        /// Property to hold info if to use vocie command
        /// </summary>
        public bool UseVoiceCommands { get; set; }

        #endregion

        /// <summary>
        /// Property to hold the rendering mode
        /// </summary>
        public RenderMode Mode { get { return SelectedRenderMode; } }

        /// <summary>
        /// Property to hold new settings
        /// </summary>
        public SettingsNotification NewSettings
        {
            get { return new SettingsNotification(this.Mode, 
                this.NumOfPointToFilter, 
                this.ShowCoordinate,
                this.UseVoiceCommands); }
        }
        
        #endregion


        /// <summary>
        /// Constructor of class to hold window interaction of application settings
        /// </summary>
        /// <param name="settingsNotification"></param>
        public SettingsModelView(SettingsNotification settingsNotification)
        {
            this.ShowCoordinate = settingsNotification.ShowCoordinates;
            this.NumOfPointToFilter = settingsNotification.PointsToFilter;
            this.UseVoiceCommands = settingsNotification.UseVoiceCommands;

            this.SelectedRenderMode = settingsNotification.DefaultRenderMode;
        }
    }
}
