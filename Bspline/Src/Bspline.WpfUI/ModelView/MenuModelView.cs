using System;
using System.IO;
using System.Xml.Linq;
using Bspline.Core;
using Bspline.Core.Interfaces;
using Bspline.WpfUI.Commands;
using Bspline.WpfUI.DataManagment;
using Bspline.WpfUI.Interfaces;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace Bspline.WpfUI.ModelView
{
    /// <summary>
    /// Derived model vide to handle the menu area of application
    /// </summary>
    public class MenuModelView : ModelViewColleague
    {
        /// <summary>
        /// Property to hold main window to delegat to <see cref="IMenuDelegate"/>
        /// </summary>
        public IMenuDelegate MenuDelegate { get; set; }

        #region Properties

        #region View bindable


        /// <summary>
        /// Property to hold the load file command <see cref="ICommand"/>
        /// </summary>
        public ICommand LoadCommand
        {
            get { return new RelayCommand(obj => LoadLayout()); }
        }

        /// <summary>
        /// Property to hold the new file command <see cref="ICommand"/>
        /// </summary>
        public ICommand NewCommand
        {
            get
            {
                return new RelayCommand(obj => NewLayout(),
                    obj => this.IsMediaStreaming && this.Mediator.InEditingMode);
            }
        }

        /// <summary>
        /// Property to hold the save file command <see cref="ICommand"/>
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(obj => SaveLayout(),
                    obj => this.Mediator.InEditingMode);
            }
        }

        /// <summary>
        /// Property to hold the save as file command <see cref="ICommand"/>
        /// </summary>
        public ICommand SaveAsCommand
        {
            get
            {
                return new RelayCommand(obj => SaveAsLayout(),
                    obj => this.Mediator.InEditingMode);
            }
        }

        /// <summary>
        /// Property to hold the exit application command <see cref="ICommand"/>
        /// </summary>
        public ICommand ExitCommand
        {
            get { return new RelayCommand(obj => Application.Current.Shutdown()); }
        }

        /// <summary>
        /// Property to hold the open settings window command <see cref="ICommand"/>
        /// </summary>
        public ICommand SettingsCommand
        {
            get { return new RelayCommand(obj => this.ShowSettings()); }
        }


        /// <summary>
        /// Property to hold the open User manual window command <see cref="ICommand"/>
        /// </summary>
        public ICommand UserManualCommand
        {
            get { return new RelayCommand(obj => this.ShowUserManual()); }
        }


        /// <summary>
        /// Property to hold the open info window command <see cref="ICommand"/>
        /// </summary>
        public ICommand ShowInfoCommand
        {
            get { return new RelayCommand(obj => this.ShowProjectInfo()); }
        }

        #endregion

        /// <summary>
        /// Property to hold <see cref="FileManager"/> helper class
        /// </summary>
        private FileManager FileHelper { get; set; }

        #endregion

        #region Constractor

        /// <summary>
        /// Constructor of derived model vide to handle the menu area of application
        /// </summary>
        /// <param name="mediator">mediator to delegate to</param>
        /// <param name="menuDelegate">application window to delegate to</param>
        public MenuModelView(IModelViewMediator mediator, IMenuDelegate menuDelegate)
            : base(mediator)
        {
            this.MenuDelegate = menuDelegate;
            this.FileHelper = new FileManager(this);
        }

        #endregion

        #region Internal

        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override void InitializeColleague()
        {
            this.FileHelper.Initialize();
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        internal override void NotifySettingsChanged()
        {
            this.FileHelper.SaveSettingsToDisk(this.Mediator.AppSettings);
        }

        #endregion

        #region Protected


        /// <summary>
        /// <see cref="DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            if (this.FileHelper != null)
            {
                this.FileHelper.Dispose();
                this.FileHelper = null;
            }
            this.MenuDelegate = null;
        }


        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        protected override void PrepareUiBindedProperties()
        {
            this.OnPropertyChanged(() => this.IsMediaStreaming);
        }

        #endregion

        #region Private

        /// <summary>
        /// Show settings window
        /// </summary>
        private void ShowSettings()
        {
            if (this.MenuDelegate != null)
            {
                this.MenuDelegate.ShowSettings();
            }
        }

        /// <summary>
        /// Show the user manual window
        /// </summary>
        private void ShowUserManual()
        {
            if (this.MenuDelegate != null)
            {
                this.MenuDelegate.ShowUserManual(this.FileHelper.UserManualUrl);
            }
        }

        /// <summary>
        /// Show the info window
        /// </summary>
        private void ShowProjectInfo()
        {
            if (this.MenuDelegate != null)
            {
                this.MenuDelegate.ShowProjectInfo();
            }
        }

        /// <summary>
        /// Load the layout from disk
        /// </summary>
        private void LoadLayout()
        {
            this.FileHelper.Load();
        }

        /// <summary>
        /// Save as the layout to disk
        /// </summary>
        private void SaveAsLayout()
        {
            this.FileHelper.SaveAs();
        }

        /// <summary>
        /// Save the layout to disk
        /// </summary>
        private void SaveLayout()
        {
            this.FileHelper.Save();
        }

        /// <summary>
        /// New layout request
        /// </summary>
        private void NewLayout()
        {
            Mediator.NotifyNewSessionRequest();
        }

        #endregion
    }
}
