using System;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using Bspline.Core;
using Bspline.WpfUI.ModelView;
using Microsoft.Win32;

namespace Bspline.WpfUI.DataManagment
{
    /// <summary>
    /// Class responsible for all disk interaction
    /// </summary>
    internal class FileManager : DisposableObject
    {
        #region Field

        private const string DEFAULT_EXT = ".xml";
        private const string FILTER = "Xml documents (.xml)|*.xml";
        private const string SETTINGS_DIR = "Settings";
        private const string LAYOUTS_DIR = "Layouts";
        private const string USER_MANUAL_DIR = "UserManual";

        private const string SETTING_FILE = "Settings.xml";

        private const string SETTING_ROOT = "root";

        private const string USER_MANUAL_FILE = "Manual.pdf";

        #endregion

        #region Properties

        /// <summary>
        /// Property hold User manual url
        /// </summary>
        internal string UserManualUrl
        {
            get
            {
                return Path.Combine(Path.Combine(Environment.CurrentDirectory, USER_MANUAL_DIR), USER_MANUAL_FILE);
            }
        }

        /// <summary>
        /// Property hold settings url
        /// </summary>
        private string SettingsUrl
        {
            get
            {
                return Path.Combine(Path.Combine(Environment.CurrentDirectory, SETTINGS_DIR), SETTING_FILE);
            }
        }

        /// <summary>
        /// Property holds the last saved file name
        /// </summary>
        private string LastSaveAsFileName { get; set; }

        /// <summary>
        /// Property of <see cref="Bspline.WpfUI.ModelViews.MenuModelView"/> to which this object will delegate to
        /// </summary>
        private MenuModelView MenuModelView { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for class responsible for all disk interaction
        /// </summary>
        /// <param name="menuModelView"><see cref="Bspline.WpfUI.ModelViews.MenuModelView"/> to which this object will delegate to</param>
        public FileManager(MenuModelView menuModelView)
        {
            this.MenuModelView = menuModelView;
        }

        #endregion

        #region Internal

        /// <summary>
        /// Intialize all file interaction
        /// </summary>
        internal void Initialize()
        {
            this.LastSaveAsFileName = string.Empty;

            this.PrepareLayout();
            this.PrepareSettings();
        }

        /// <summary>
        /// loads settings from disk and pass a <see cref="Bspline.Core.SettingsNotification"/> which hold application settings data
        /// </summary>
        /// <returns></returns>
        internal SettingsNotification RetriveSettingsFromDisk()
        {
            XElement settingsElement = ParseSettings(this.SettingsUrl);
            return new SettingsNotification(settingsElement);
        }

        /// <summary>
        /// Save settings to the disk
        /// </summary>
        /// <param name="settingsNotification">settings data</param>
        internal void SaveSettingsToDisk(SettingsNotification settingsNotification)
        {
            XElement settingsElement = this.CreateSettingsElement(settingsNotification);
            this.SaveElement(settingsElement, this.SettingsUrl);
        }

        /// <summary>
        /// Layout "Save as" interaction
        /// </summary>
        internal void SaveAs()
        {
            this.PrepareLayout();
            DialogResult result = this.ShowDialog(() => new SaveFileDialog(), LAYOUTS_DIR);

            if (result.Result)
            {
                this.LastSaveAsFileName = result.SelectedFileName;
                this.SaveLayout();
            }
        }

        /// <summary>
        /// Layout "Save" interaction
        /// </summary>
        internal void Save()
        {
            if (string.IsNullOrEmpty(this.LastSaveAsFileName))
            {
                this.SaveAs();
            }
            else
            {
                this.SaveLayout();
            }
        }

        /// <summary>
        /// Layout "Load" interaction
        /// </summary>
        internal void Load()
        {
            DialogResult result = this.ShowDialog(() => new OpenFileDialog(), LAYOUTS_DIR);

            if (result.Result)
            {
                this.LastSaveAsFileName = result.SelectedFileName;

                XElement layout = XElement.Load(this.LastSaveAsFileName);
                this.MenuModelView.Mediator.ParseData(layout);
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// <see cref="Bspline.Core.DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            this.MenuModelView = null;
        }

        #endregion

        #region Private

        /// <summary>
        /// Will show the required <see cref="Microsoft.Win32.FileDialog"/> dialog for specific directory location
        /// </summary>
        /// <param name="dialogCreator">function which will create our specific dialog</param>
        /// <param name="dirName">name of directory to open into</param>
        /// <returns></returns>
        private DialogResult ShowDialog(Func<FileDialog> dialogCreator, string dirName)
        {
            FileDialog dialog = dialogCreator();

            dialog.FileName = string.Empty;
            dialog.InitialDirectory = Path.Combine(Environment.CurrentDirectory, dirName);
            dialog.DefaultExt = FileManager.DEFAULT_EXT; // Default file extension
            dialog.Filter = FileManager.FILTER; // Filter files by extension 

            bool? result = dialog.ShowDialog();
            return new DialogResult
            {
                Result = result != null && result.Value,
                SelectedFileName = dialog.FileName
            };
        }

        /// <summary>
        /// Save current layout to disk
        /// </summary>
        private void SaveLayout()
        {
            XElement layout = this.MenuModelView.Mediator.BuildData();
            this.SaveElement(layout, this.LastSaveAsFileName);
        }

        /// <summary>
        /// Create layout directory in case doesn't exists yet
        /// </summary>
        private void PrepareLayout()
        {
            this.CreateDirIfNeeded(Path.Combine(Environment.CurrentDirectory, LAYOUTS_DIR));
        }

        /// <summary>
        /// Create settings directory in case doesn't exists yet
        /// </summary>
        private void PrepareSettings()
        {
            string settingDir = Path.Combine(Environment.CurrentDirectory, SETTINGS_DIR);
            this.CreateDirIfNeeded(settingDir);

            SettingsNotification settingsNotification = this.RetriveSettingsFromDisk();
            this.MenuModelView.Mediator.UpdateSettings(settingsNotification);
        }

        /// <summary>
        /// Reads the settings from disk
        /// </summary>
        /// <param name="settingFile">settings file name</param>
        /// <returns>settings as xml</returns>
        private XElement ParseSettings(string settingFile)
        {
            XElement settingsElement;
            if (File.Exists(settingFile))
            {
                settingsElement = XElement.Load(settingFile);
            }
            else
            {
                settingsElement = this.CreateSettingsElement(new SettingsNotification(RenderMode.Regular2D, 3, true, true));
                this.SaveElement(settingsElement, settingFile);
            }

            return settingsElement;
        }

        /// <summary>
        /// Save any <see cref="System.Xml.Linq.XElement"/> element into specific location
        /// </summary>
        /// <param name="element">xml data</param>
        /// <param name="location">file location</param>
        private void SaveElement(XElement element, string location)
        {
            element.Save(location);
        }

        private XElement CreateSettingsElement(SettingsNotification settings)
        {
            XElement settingsElement = new XElement(SETTING_ROOT,
                new XElement(SettingsNotification.RENDER_MODE, settings.DefaultRenderMode.ToString()),
                new XElement(SettingsNotification.NUM_TO_FILTER, settings.PointsToFilter),
                new XElement(SettingsNotification.SHOW_COORDINATES, settings.ShowCoordinates),
                new XElement(SettingsNotification.USE_VOICE_COMMANDS, settings.UseVoiceCommands));

            return settingsElement;
        }

        /// <summary>
        /// Create a directory at specific location 
        /// </summary>
        /// <param name="fullPath">location of directory</param>
        private void CreateDirIfNeeded(string fullPath)
        {
            BSplineUtility.Instance.CreateDir(fullPath);
        }

        #endregion

        #region Inner class - Helper
        /// <summary>
        /// Nested helper class, which hold the dialog results
        /// </summary>
        private class DialogResult
        {
            public bool Result { get; set; }
            public string SelectedFileName { get; set; }
        }

        #endregion
    }
}
