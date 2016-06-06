

using System;
using System.Timers;
using Bspline.Core.DataPacket;
using Bspline.Core.Interfaces;
using Bspline.Core.Location;
using Bspline.Media.BodyParts;

namespace Bspline.Media.Device
{
    public class KinectDummy : MediaWrapperBase
    {

        /// <summary>
        /// this class builded to sumulate meddia
        /// <seealso cref="MediaWrapperBase"/>
        /// so if the Kinect dosent connected we have option for simulate the connection
        /// </summary>
        #region Fields

        private bool _isStreaming;

        #endregion

        #region Properties
        /// <summary>
        /// it only implement points stream
        /// and it genarate randomly points to the stream
        /// </summary>
        private Random PointGenX { get; set; }
        private Random PointGenY { get; set; }
        private Random PointGenZ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private Random RanMultipler { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private Timer PointsGenTimer { get; set; }
        /// <summary>
        /// simulate streaming
        /// </summary>
        public override bool IsMediaStreaming
        {
            get { return this._isStreaming; }
        }
        /// <summary>
        /// simulate device ready
        /// </summary>
        public override bool IsMediaDeviceReady
        {
            get { return true; }
        }
        /// <summary>
        /// update the status for bunding
        /// </summary>
        public override string MediaStatus
        {
            get { return "Dummy..."; }
        }
        /// <summary>
        /// follow the angles and save it always to 0
        /// </summary>
        public override double MediaAngle
        {
            get { return 0; ; }
        }


        #endregion

        #region Constructor
        /// <summary>
        /// Kinect dummy it is simulator for the system if we don't have kinect
        /// </summary>
        /// <param name="modelDelegate"></param>
        public KinectDummy( IModelDelegate modelDelegate )
            : base( modelDelegate )
        {
            this._isStreaming = false;
            this.PointsGenTimer = new Timer { Enabled = false, Interval = 250 };
            this.PointsGenTimer.Elapsed += PointsGenTimer_Elapsed;
            this.PointGenX = new Random( 20 );
            this.PointGenY = new Random( 20 );
            this.PointGenZ = new Random( 0 );
            this.RanMultipler = new Random( 1 );
        }

        #endregion

        #region Public
        /// <summary>
        /// empty
        /// </summary>
        /// <param name="value"></param>
        public override void ChangeAngle( int value )
        {

        }
        /// <summary>
        /// start media
        /// </summary>
        public override void StartMedia()
        {
            this._isStreaming = true;
            this.PointsGenTimer.Start();
            base.StartMedia();
        }
        /// <summary>
        /// stop media
        /// </summary>
        public override void StopMedia()
        {
            this.PointsGenTimer.Stop();
            this._isStreaming = false;
        }

        #endregion

        #region Events
        /// <summary>
        /// point generator rondamly inside the canvas window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointsGenTimer_Elapsed( object sender, ElapsedEventArgs e )
        {
            double x = this.PointGenX.NextDouble() * 640 * this.RanMultipler.NextDouble();
            double y = this.PointGenY.NextDouble() * 480 * this.RanMultipler.NextDouble();
            double z = this.PointGenZ.NextDouble() * 10;

            x = ( ( ( x / 1f ) / 640 ) * WidthResolution );
            y = ( ( ( y / 1f ) / 480 ) * HeightResolution );

            Point3D point = new Point3D( x, y, z );

            if ( this._isStreaming )
            {

                this.ModelDelegate.OnMotionDataPacketReady( new MotionDataPacket( new LeftPosition( point ), new RightPosition( point ) ) );

            }
        }

        #endregion
    }
}