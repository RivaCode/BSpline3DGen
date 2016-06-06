using System.Windows.Threading;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.DataPacket;
using Bspline.WpfUI.Commands;
using Bspline.WpfUI.Interfaces;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bspline.WpfUI.ModelView
{
    /// <summary>
    /// Derived model vide to handle the control area of application
    /// </summary>
    public class ControlModelView : ModelViewColleague
    {
        #region Fields

        private const string STREAMING = "Streaming";
        private const string NO_STREAM = "No Stream...";

        private BitmapSource _videoImageSource;
        private BitmapRender _bitmapHelper;
        private string _streamLabel;

        #endregion

        #region Properties

        /// <summary>
        /// Property which assosiated with WPF dispatcher <see cref="Dispatcher"/> which update the streaming property 
        /// </summary>
        private DispatcherTimer StreamTimer { get; set; }

        /// <summary>
        /// Property which hold <see cref="BitmapRender"/> to help render video
        /// </summary>
        private BitmapRender BitmapHelper
        {
            get
            {
                if ( this._bitmapHelper == null )
                {
                    this._bitmapHelper = new BitmapRender();
                }
                return this._bitmapHelper;
            }
        }

        #region Binded To View

        /// <summary>
        /// Property to hold rendered video
        /// </summary>
        public BitmapSource VideoImageSource
        {
            get { return _videoImageSource; }
            private set
            {
                _videoImageSource = value;
                this.OnPropertyChanged( () => this.VideoImageSource );
            }
        }

        /// <summary>
        /// Property to hold the connection toggling <see cref="ICommand"/>
        /// </summary>
        public ICommand ToggleStreamCommand
        {
            get { return new RelayCommand( obj => ToggleMedia(), null ); }
        }

        /// <summary>
        /// Property to hold the angle change <see cref="ICommand"/>
        /// </summary>
        public ICommand AngleChangeCommand
        {
            get
            {
                return new RelayCommand(
                    obj => this.Mediator.NotifyMediaAngleChanged( this.MediaAngle ),
                    obj => this.IsMediaDeviceReady );
            }
        }

        /// <summary>
        /// Property to hold streaming prompt
        /// </summary>
        public string StreamLabel
        {
            get { return _streamLabel; }
            private set
            {
                _streamLabel = value;
                this.OnPropertyChanged( () => this.StreamLabel );
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Costructor derived model vide to handle the control area of application
        /// </summary>
        /// <param name="mediator">mediator to delegate to</param>
        public ControlModelView( IModelViewMediator mediator )
            : base( mediator )
        {

        }

        #endregion

        #region Internal

        /// <summary>
        /// Async method to handle video data packet and produce video bitmap
        /// </summary>
        /// <param name="packet"></param>
        internal override async void UpdateVideoStream( ColorDataPacket packet )
        {
            if ( this.IsDisposed )
            {
                return;
            }

            try
            {
                Bitmap result = await this.BitmapHelper.Render( packet );
                this.PostToUiThread( () =>
                {
                    if ( !this.IsDisposed && this.IsMediaStreaming )
                    {
                        this.VideoImageSource = this.BitmapHelper.ToBitmapSource( result );
                    }
                } );
            }
            catch ( TaskCanceledException )
            {
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        protected override void PrepareColleagueInner()
        {
            this.StreamTimer = new DispatcherTimer { Interval = new TimeSpan( 0, 0, 1 ), IsEnabled = false };
            this.StreamTimer.Tick += StreamTimerTick;
        }

        /// <summary>
        /// <see cref="ModelViewColleague"/>
        /// </summary>
        protected override void PrepareUiBindedProperties()
        {
            if ( this.IsMediaStreaming )
            {
                this.IconWidth = 50;
                this.IconMargin = new Thickness( 0 );
                this.ImageSource = "stop";
                this.StreamLabel = STREAMING;
                this.StreamTimer.Start();
            }
            else
            {
                this.IconWidth = 100;
                this.IconMargin = new Thickness( 12, 0, 0, 0 );
                this.ImageSource = "play";
                this.StreamLabel = NO_STREAM;
                this.VideoImageSource = null;
                this.StreamTimer.Stop();
            }
            this.MediaAngle = this.Mediator.MediaAngle;

            base.PrepareUiBindedProperties();
        }

        /// <summary>
        /// <see cref="DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            if ( this.StreamTimer != null )
            {
                this.StreamTimer.Stop();
                this.StreamTimer.Tick -= StreamTimerTick;
                this.StreamTimer = null;
            }

            this._videoImageSource = null;
            this._bitmapHelper = null;
        }

        #endregion

        #region Private

        /// <summary>
        /// Process the toggle media async
        /// </summary>
        private async void ToggleMedia()
        {
            await ToggleMediaAsync();
        }

        /// <summary>
        /// Process the toggle media async
        /// </summary>
        /// <returns></returns>
        private async Task ToggleMediaAsync()
        {
            await Task.Run( () =>
            {
                if ( this.IsMediaStreaming )
                {
                    this.Mediator.StopMedia();
                }
                else
                {
                    this.Mediator.StartMedia();
                }
            } );
        }

        private void OnTimerTick()
        {
            this.StreamLabel = string.Format( "{0}.", this.StreamLabel );
            if ( this.StreamLabel.EndsWith( "........", StringComparison.OrdinalIgnoreCase ) )
            {
                this.StreamLabel = this.StreamLabel.TrimEnd( new[] { '.' } );
                this.StreamLabel = string.Format( "{0}.", this.StreamLabel );
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// The <see cref="Dispatcher"/> onTick method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StreamTimerTick( object sender, EventArgs e )
        {
            if ( this.StreamLabel.StartsWith( STREAMING, StringComparison.OrdinalIgnoreCase ) )
            {
                this.OnTimerTick();
            }
        }

        #endregion
    }
}
