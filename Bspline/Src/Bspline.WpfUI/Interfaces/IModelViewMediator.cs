using System.Xml.Linq;
using Bspline.Core;
using Bspline.Core.Interfaces;
using Bspline.WpfUI.DataManagment;
using Bspline.WpfUI.ModelView;

namespace Bspline.WpfUI.Interfaces
{
    /// <summary>
    /// Interface responsible to interact between <see cref="Bspline.WpfUI.ModelView.ModelViewColleague"/>
    /// </summary>
    public interface IModelViewMediator:IMediaControllable
    {
        /// <summary>
        /// Property to read if in editing-mode
        /// </summary>
        bool InEditingMode { get; }

        /// <summary>
        /// Will register <see cref="Bspline.WpfUI.ModelView.ModelViewColleague"/> with in this object
        /// </summary>
        /// <param name="modelView">the colleague to register</param>
        void RegisterModelViewForCommunication(ModelViewColleague modelView);

        /// <summary>
        /// Notifies collection of <see cref="Bspline.WpfUI.ModelView.ModelViewColleague"/> that media angle was changed
        /// </summary>
        /// <param name="value">angle amount</param>
        void NotifyMediaAngleChanged(double value);

        /// <summary>
        /// Notifies collection of <see cref="Bspline.WpfUI.ModelView.ModelViewColleague"/> that recording is done
        /// </summary>
        void NotifyRecordingDone();

        /// <summary>
        /// Start recording
        /// </summary>
        /// <param name="isRecording">record status</param>
        void StartRecording(bool isRecording);

        /// <summary>
        /// Notifies collection of <see cref="Bspline.WpfUI.ModelView.ModelViewColleague"/> that new recording-mode requested
        /// </summary>
        void NotifyNewSessionRequest();

        /// <summary>
        /// Property to hold application settings
        /// </summary>
        SettingsNotification AppSettings { get; }

        /// <summary>
        /// Request application to constructor xml data file which hold all layout related information
        /// </summary>
        /// <returns></returns>
        XElement BuildData();

        /// <summary>
        /// Gives application the xml data to constructor all layout reletaed info
        /// </summary>
        /// <param name="layout"></param>
        void ParseData(XElement layout);

        /// <summary>
        /// Notifies collection of <see cref="Bspline.WpfUI.ModelView.ModelViewColleague"/> that settings was updated
        /// </summary>
        /// <param name="settingsNotification"></param>
        void UpdateSettings(SettingsNotification settingsNotification);
    }
}
