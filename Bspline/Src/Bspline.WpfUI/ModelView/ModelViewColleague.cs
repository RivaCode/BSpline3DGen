using System.Collections.Generic;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.DataPacket;
using Bspline.Core.Interfaces;
using Bspline.WpfUI.DataManagment;
using Bspline.WpfUI.Interfaces;
using System;
using System.Reflection;
using System.Windows;

namespace Bspline.WpfUI.ModelView
{
    /// <summary>
    /// Abstract class for Modelviews to drive from, <see cref="Bspline.WpfUI.Interfaces.IModelViewMediator"/>
    /// </summary>
    public abstract class ModelViewColleague : ModelViewBase, IMediaStatus
    {
        #region Fields

        /// <summary>
        /// Field to hold icon height
        /// </summary>
        private int _iconHeight;
        /// <summary>
        /// Field to hold icon width
        /// </summary>
        private int _iconWidth;
        /// <summary>
        /// Field to hold icon margin <see cref="System.Windows.Thickness"/>
        /// </summary>
        private Thickness _iconMargin;
        /// <summary>
        /// Field to hold image source
        /// </summary>
        private string _imageSource;
        /// <summary>
        /// Field to hold media status
        /// </summary>
        private string _mediaStatus;
        /// <summary>
        /// Field to hold info if media device is ready
        /// </summary>
        private bool _isMediaDeviceReady;

        /// <summary>
        /// Field to hold the current angle of media device
        /// </summary>
        private double _mediaAngle;

        #endregion

        #region Properties

        /// <summary>
        /// Property to hold the mediator <see cref="Bspline.WpfUI.Interfaces.IModelViewMediator"/>
        /// </summary>
        internal IModelViewMediator Mediator { get; set; }

        /// <summary>
        /// Property to hold info if the media device is streaming
        /// </summary>
        public bool IsMediaStreaming
        {
            get { return this.Mediator.IsMediaStreaming; }
        }

        #region View Binded

        /// <summary>
        /// Property to hold image source
        /// </summary>
        public string ImageSource
        {
            get { return _imageSource; }
            protected set
            {
                _imageSource = BSplineUtility.Instance.GetFullImageSource(value);
                this.OnPropertyChanged(() => this.ImageSource);
            }
        }

        /// <summary>
        /// Property to hold icon height
        /// </summary>
        public int IconHeight
        {
            get { return this._iconHeight; }
            protected set
            {
                this._iconHeight = value;
                this.OnPropertyChanged(() => this.IconHeight);
            }
        }

        /// <summary>
        /// Propety to hold icon width
        /// </summary>
        public int IconWidth
        {
            get { return _iconWidth; }
            protected set
            {
                _iconWidth = value;
                this.OnPropertyChanged(() => this.IconWidth);
            }
        }

        public Thickness IconMargin
        {
            get { return _iconMargin; }
            protected set
            {
                _iconMargin = value;
                this.OnPropertyChanged(() => this.IconMargin);
            }
        }

        /// <summary>
        /// Propety to hold media status
        /// </summary>
        public string MediaStatus
        {
            get { return _mediaStatus; }
            protected set
            {
                _mediaStatus = value;
                this.OnPropertyChanged(() => this.MediaStatus);
            }
        }

        /// <summary>
        /// Property to hold media device angle
        /// </summary>
        public double MediaAngle
        {
            get { return _mediaAngle; }
            protected set
            {
                _mediaAngle = value;
                this.OnPropertyChanged(() => this.MediaAngle);
            }
        }

        /// <summary>
        /// Property to hold info if the media device is ready
        /// </summary>
        public bool IsMediaDeviceReady
        {
            get { return _isMediaDeviceReady; }
            protected set
            {
                _isMediaDeviceReady = value;
                this.OnPropertyChanged(() => this.IsMediaDeviceReady);
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of class for Modelviews to drive from, <see cref="Bspline.WpfUI.Interfaces.IModelViewMediator"/>
        /// </summary>
        /// <param name="mediator">mediator to delegate to</param>
        protected ModelViewColleague(IModelViewMediator mediator)
        {
            this.Mediator = mediator;
            this.Mediator.RegisterModelViewForCommunication(this);
            this.PrepareColleague();
            this.UpdateUi();
        }

        #endregion

        #region Public

        /// <summary>
        /// Update that media work status changed
        /// </summary>
        internal virtual void NotifyMediaWorkStatusChanged()
        {
            this.UpdateUi();
        }

        /// <summary>
        /// Update that media hardware status changed
        /// </summary>
        internal virtual void NotifyMediaHardWareStatusChanged()
        {
            this.UpdateUi();
        }

        /// <summary>
        /// Update upon video stream 
        /// </summary>
        /// <param name="packet">image data</param>
        internal virtual void UpdateVideoStream(ColorDataPacket packet)
        {

        }

        /// <summary>
        /// Update upon motion stream
        /// </summary>
        /// <param name="motionDataPacket">motion data</param>
        internal virtual void UpdateTrackingStream(MotionDataPacket motionDataPacket)
        {
        }


        /// <summary>
        /// Requests drawing dimension
        /// </summary>
        /// <returns></returns>
        internal virtual int[] GetDrawingDimension()
        {
            return null;
        }

        /// <summary>
        /// Update the screen painting
        /// </summary>
        /// <param name="result"></param>
        internal virtual void UpdateScreenPaint(AlgResult result)
        {

        }

        /// <summary>
        /// Update that record was requested
        /// </summary>
        internal virtual void NotifyRecordRequest()
        {

        }

        /// <summary>
        /// Update that new record was requested
        /// </summary>
        internal virtual void NotifyNewRequest()
        {

        }

        /// <summary>
        /// Initialize derived class
        /// </summary>
        internal virtual void InitializeColleague()
        {

        }

        /// <summary>
        /// Update that settings was changed
        /// </summary>
        internal virtual void NotifySettingsChanged()
        {

        }

        /// <summary>
        /// Update that screen dimension was changed
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal virtual void NotifyDrawingDimensionChanged(double width, double height)
        {
        }
        #endregion

        #region Protected


        /// <summary>
        /// UpdateUi is thread safe for the UI thread
        /// </summary>
        protected void UpdateUi()
        {
            this.PostToUiThread(PrepareUiBindedProperties);
        }

        /// <summary>
        /// Delegates <see cref="Action"/> to UI thread
        /// </summary>
        /// <param name="action"></param>
        protected void PostToUiThread(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(action);
            }
        }

        /// <summary>
        /// Prepares the derived class to update properties
        /// </summary>
        protected virtual void PrepareUiBindedProperties()
        {
            this.MediaStatus = this.Mediator.MediaStatus;
            this.IsMediaDeviceReady = this.Mediator.IsMediaDeviceReady;
        }

        /// <summary>
        /// Prepare the derived class for changes
        /// </summary>
        protected virtual void PrepareColleagueInner()
        {

        }

        /// <summary>
        /// <see cref="Bspline.Core.DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            this.Mediator = null;
        }

        #endregion

        #region Private

        /// <summary>
        /// will call the derived class the <see cref="PrepareColleagueInner"/> method
        /// </summary>
        private void PrepareColleague()
        {
            this.PrepareColleagueInner();
        }

        #endregion
    }
}
