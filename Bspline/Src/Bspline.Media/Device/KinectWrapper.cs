using Bspline.Core.DataPacket;
using Bspline.Core.Interfaces;
using Bspline.Core.Location;
using Bspline.Media.BodyParts;
using Bspline.Media.Device.Sound;
using Microsoft.Kinect;
using System.Linq;

namespace Bspline.Media.Device
{
    internal class KinectWrapper : MediaWrapperBase
    {

        /// <summary>
        /// Internal class that implement <seealso cref="MediaWrapperBase"/>
        /// check if kinect connected and take info from his sensores
        /// </summary>
        #region Static

        internal static bool IsKinectThere
        {
            get
            {
                return KinectSensor.KinectSensors
                        .FirstOrDefault( sensor => sensor.Status == KinectStatus.Connected ) != null;

            }
        }


        #endregion

        #region Fields
        /// <summary>
        /// the information that we use from the kinect sensorce store in this fileds
        /// </summary>
        private bool _useVoiceCommand;
        private KinectSensor _sensor;
        private bool _isStreaming;
        private Skeleton[] _totalSkeletons;

        #endregion

        #region Properties
        /// <summary>
        /// <seealso cref="KinectAudioHelper"/>
        /// </summary>
        public KinectAudioHelper KinectSoundHelper { get; set; }
        /// <summary>
        /// check if voice commands are in use
        /// </summary>
        public override bool UseVoiceCommand
        {
            set
            {
                this._useVoiceCommand = value;
                if ( this.KinectSoundHelper != null )
                {
                    this.KinectSoundHelper.UseVoiceRecognition = this._useVoiceCommand;
                }
            }
            get { return _useVoiceCommand; }
        }
        /// <summary>
        /// check the sensor for kinect start
        /// </summary>
        private bool IsMediaReadyAndRunning
        {
            get
            {
                return this._sensor != null
                       && this._sensor.IsRunning;
            }
        }
        /// <summary>
        /// get information from Kinect and update the whole system
        /// </summary>
        public override bool IsMediaDeviceReady
        {
            get
            {
                return this.Sensor != null
                       && this.Sensor.Status == KinectStatus.Connected;
            }
        }

        /// <summary>
        /// check if theare connections to streem window
        /// </summary>
        public override bool IsMediaStreaming { get { return this._isStreaming; } }
        /// <summary>
        /// return the status of the kinect
        /// </summary>
        public override string MediaStatus
        {
            get
            {
                return this.IsMediaDeviceReady
                    ? this.Sensor.Status.ToString()
                    : KinectStatus.NotReady.ToString();
            }
        }

        public override double MediaAngle
        {
            get
            {
                double angle = 0;
                if ( this.IsMediaReadyAndRunning )
                {
                    //angle = this.Sensor.ElevationAngle;
                }
                return angle;
            }
        }
        /// <summary>
        /// try to connect to all sensors
        /// </summary>
        internal KinectSensor Sensor
        {
            get
            {
                if ( this._sensor == null )
                {
                    this._sensor = KinectSensor.KinectSensors
                        .FirstOrDefault( sensor => sensor.Status == KinectStatus.Connected );
                }
                return _sensor;
            }
        }
        /// <summary>
        /// points from the skeleton collected by kinect
        /// </summary>
        private Skeleton[] TotalSkeletons
        {
            get
            {
                if ( this._totalSkeletons == null )
                {
                    this._totalSkeletons = new Skeleton[6];
                }
                return _totalSkeletons;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// start the connection and add kinect instance to model delegate
        /// </summary>
        /// <param name="modelDelegate"></param>
        public KinectWrapper( IModelDelegate modelDelegate )
            : base( modelDelegate )
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensorsStatusChanged;
            this._isStreaming = false;
        }

        #endregion

        #region Public
        /// <summary>
        /// check the angel changing and apply it on Kinect tillt motor
        /// </summary>
        /// <param name="value"></param>
        public override void ChangeAngle( int value )
        {
            if ( this.IsMediaReadyAndRunning )
            {
                this.Sensor.ElevationAngle = value;
            }
        }
        /// <summary>
        /// update the system when connection started
        /// </summary>
        public override void StartMedia()
        {
            if ( !this._sensor.IsRunning )
            {
                base.StartMedia();
                this.EnableStreams();
                this.RegisterToStreams();
                this.Sensor.Start();
                this.KinectSoundHelper = new KinectAudioHelper( this );
                this.KinectSoundHelper.Start();
                this.KinectSoundHelper.UseVoiceRecognition = this.UseVoiceCommand;
            }

            this._isStreaming = !this._isStreaming;
        }
        /// <summary>
        /// update the system in the end of communication 
        /// </summary>
        public override void StopMedia()
        {
            if ( this.IsMediaReadyAndRunning )
            {
                if ( this.KinectSoundHelper != null )
                {
                    this.KinectSoundHelper.Dispose();
                    this.KinectSoundHelper = null;
                }
                this.DisableStreams();
                this.UnregisterFromStreams();
                this.Sensor.Stop();
            }

            if ( this._isStreaming )
            {
                this._isStreaming = !this._isStreaming;
            }
        }

        #endregion

        #region Protected
        /// <summary>
        /// <seealso cref="DisposeObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            KinectSensor.KinectSensors.StatusChanged -= KinectSensorsStatusChanged;

            if ( this._sensor != null )
            {
                this.StopMedia();
                this.Sensor.Dispose();
                this._sensor = null;
            }

            this._totalSkeletons = null;
        }

        #endregion

        #region Private
        /// <summary>
        /// enableing information stream
        /// </summary>
        private void EnableStreams()
        {
            this.Sensor.DepthStream.Enable();
            this.Sensor.SkeletonStream.Enable();
            this.Sensor.ColorStream.Enable( ColorImageFormat.RgbResolution640x480Fps30 );
        }
        /// <summary>
        /// register sensors to follow them
        /// </summary>
        private void RegisterToStreams()
        {
            this.Sensor.SkeletonFrameReady += SensorSkeletonFrameReady;
            this.Sensor.ColorFrameReady += SensorColorFrameReady;
        }
        /// <summary>
        /// close the streaming
        /// </summary>
        private void DisableStreams()
        {
            this.Sensor.SkeletonStream.Disable();
            this.Sensor.ColorStream.Disable();
        }
        /// <summary>
        /// remove un wanted sensors from the follow list
        /// </summary>
        private void UnregisterFromStreams()
        {
            this.Sensor.SkeletonFrameReady -= SensorSkeletonFrameReady;
            this.Sensor.ColorFrameReady -= SensorColorFrameReady;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageFrame"></param>
        private void SensorColorFrameReadyHandler( ColorImageFrame imageFrame )
        {
            byte[] pixelData = new byte[imageFrame.PixelDataLength];
            imageFrame.CopyPixelDataTo( pixelData );

            ColorDataPacket packet = new ColorDataPacket( pixelData,
                imageFrame.Width,
                imageFrame.Height,
                imageFrame.BytesPerPixel );
            this.ModelDelegate.OnColorDataPacketReady( packet );
        }
        /// <summary>
        /// it is get the skeleton info and get trakink on left and rigth hand only
        /// each set of data for hand 
        /// we convert to 3D Point <seealso cref="3DPoint"/>
        /// and stream this info 
        /// </summary>
        /// <param name="skeletonFrame"></param>
        private void SensorSkeletonFrameReadyHandler( SkeletonFrame skeletonFrame )
        {
            skeletonFrame.CopySkeletonDataTo( this.TotalSkeletons );
            Skeleton trackedSkeleton = this.TotalSkeletons.FirstOrDefault( trackSkeleton =>
                trackSkeleton.TrackingState == SkeletonTrackingState.Tracked );

            if ( trackedSkeleton != null )
            {
                Point3D leftPosition = this.ScaleJoint( trackedSkeleton.Joints[JointType.HandLeft] );
                Point3D rightPosition = this.ScaleJoint( trackedSkeleton.Joints[JointType.HandRight] );

                this.ModelDelegate.OnMotionDataPacketReady( new MotionDataPacket( new LeftPosition( leftPosition ), new RightPosition( rightPosition ) ) );
            }
        }
        /// <summary>
        /// adopte the points to the screen size
        /// </summary>
        /// <param name="joint"></param>
        /// <returns></returns>
        private Point3D ScaleJoint( Joint joint )
        {
            DepthImagePoint point = this.Sensor.CoordinateMapper.MapSkeletonPointToDepthPoint( joint.Position,
                DepthImageFormat.Resolution640x480Fps30 );

            point.X = (int)( ( ( point.X / 1f ) / 640 ) * WidthResolution );
            point.Y = (int)( ( ( point.Y / 1f ) / 480 ) * HeightResolution );

            point.X = point.X < 0 ? 0 : point.X;
            point.Y = point.Y < 0 ? 0 : point.Y;

            return new Point3D( point.X, point.Y, point.Depth );
        }

        #endregion

        #region Events
        /// <summary>
        /// follow the changing in the Kinect status to be able of updating the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KinectSensorsStatusChanged( object sender, StatusChangedEventArgs e )
        {
            if ( e.Status != KinectStatus.Connected )
            {
                this.StopMedia();
            }
            this.OnMediaHardWareStatusChange();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorSkeletonFrameReady( object sender, SkeletonFrameReadyEventArgs e )
        {
            if ( this.IsMediaDeviceReady )
            {
                using ( SkeletonFrame skeletonFrame = e.OpenSkeletonFrame() )
                {
                    if ( skeletonFrame != null )
                    {
                        this.SensorSkeletonFrameReadyHandler( skeletonFrame );
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SensorColorFrameReady( object sender, ColorImageFrameReadyEventArgs e )
        {
            if ( this.IsMediaStreaming )
            {
                using ( ColorImageFrame imageFrame = e.OpenColorImageFrame() )
                {
                    if ( imageFrame != null )
                    {
                        this.SensorColorFrameReadyHandler( imageFrame );
                    }
                }
            }
        }

        #endregion

    }
}
