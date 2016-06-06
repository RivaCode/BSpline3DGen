using System;
using System.Xml.Linq;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.DataPacket;
using Bspline.Core.Interfaces;
using Bspline.Core.Location;
using Bspline.Model;
using Bspline.WpfUI.DataManagment;
using Bspline.WpfUI.Interfaces;
using Bspline.WpfUI.ModelView;
using System.Collections.Generic;
using System.Linq;

namespace Bspline.WpfUI
{
    /// <summary>
    /// Derived model view manager to hold all communication between <see cref="ModelViewColleague"/>
    /// </summary>
    public class ModelViewMediator : ModelViewBase, IModelViewMediator, IModelDelegate
    {
        /// <summary>
        /// Field to know if in editing mode
        /// </summary>
        private bool _inEditingMode;

        #region Properties

        #region IModelViewMediator Members

        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public bool IsMediaDeviceReady
        {
            get { return this.LogicModel.Media.IsMediaDeviceReady; }
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public bool IsMediaStreaming
        {
            get { return this.LogicModel.Media.IsMediaStreaming; }
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public string MediaStatus
        {
            get { return this.LogicModel.Media.MediaStatus; }
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public double MediaAngle { get { return this.LogicModel.Media.MediaAngle; } }

        #endregion


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public bool InEditingMode
        {
            get { return this._inEditingMode; }
            private set
            {
                this._inEditingMode = value;
                if ( this.EditingDelegate != null )
                {
                    this.EditingDelegate.UpdateEditingMode( this._inEditingMode );
                }
            }
        }

        /// <summary>
        /// Property to delegate editing changes to
        /// </summary>
        private IEditingDelegate EditingDelegate { get; set; }

        /// <summary>
        /// Property to hold the latest algortihm result <see cref="AlgResult"/>
        /// </summary>
        public AlgResult LatestResult
        {
            get { return this.LogicModel.CreatedResult; }
        }

        /// <summary>
        /// Property to hold reference to <see cref="Bspline.Model"/> library
        /// </summary>
        private LogicManager LogicModel { get; set; }

        /// <summary>
        /// Collection of the <see cref="ModelViewColleague"/> to speak with
        /// </summary>
        private List<ModelViewColleague> ModelViewCollection { get; set; }

        /// <summary>
        /// Property to hold application settings
        /// </summary>
        public SettingsNotification AppSettings { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of derived model view manager to hold all communication between <see cref="ModelViewColleague"/>
        /// </summary>
        /// <param name="editingDelegate">editing object to delegate to</param>
        public ModelViewMediator( IEditingDelegate editingDelegate )
        {
            this.EditingDelegate = editingDelegate;
            this.ModelViewCollection = new List<ModelViewColleague>();
            this.LogicModel = new LogicManager( this );
        }

        #endregion

        #region Public

        /// <summary>
        /// Notify to initialize all model views
        /// </summary>
        public void Initialize()
        {
            this.ModelViewCollection.ForEach( mv => mv.InitializeColleague() );
        }

        /// <summary>
        /// Notifies that painting area dimension was changed
        /// </summary>
        /// <param name="width">new width</param>
        /// <param name="height">new height</param>
        public void NotifyDrawingLayoutDimensionChange(double width, double height)
        {
            this.ModelViewCollection.ForEach(mv=> mv.NotifyDrawingDimensionChanged(width,height));
        }

        #region IModelViewMediator Members

        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        /// <param name="modelView"></param>
        public void RegisterModelViewForCommunication( ModelViewColleague modelView )
        {
            if ( !this.ModelViewCollection.Contains( modelView ) )
            {
                this.ModelViewCollection.Add( modelView );
            }
        }

        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void NotifyMediaAngleChanged( double value )
        {
            this.LogicModel.Media.ChangeAngle( (int)value );
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void NotifyRecordingDone()
        {
            this.LogicModel.ProcessData();
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void UpdatePoints( Point3D oldPoint, Point3D newPoint )
        {
            this.LogicModel.UpdatePoints( oldPoint, newPoint );
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void StartRecording( bool isRecording )
        {
            this.LogicModel.Record = isRecording;
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void NotifyNewSessionRequest()
        {
            this.InEditingMode = false;
            LogicModel.ReStart();
            this.DispatchNewRequest();
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public XElement BuildData()
        {
            XElement layoutElement = new XElement( "layout" );


            layoutElement.Add( this.LogicModel.SaveRequest() );
            if ( this.EditingDelegate != null )
            {
                layoutElement.Add( this.EditingDelegate.BuildEditingLayout() );
            }

            return layoutElement;
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void ParseData( XElement layout )
        {
            this.LogicModel.LoadRequest( layout );
            if (this.EditingDelegate!=null)
            {
                this.EditingDelegate.ParseEditingLayout(layout);
            }
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void UpdateSettings( SettingsNotification settingsNotification )
        {
            this.AppSettings = settingsNotification;

            this.LogicModel.ParseSettings( settingsNotification );
            this.ModelViewCollection.ForEach( mv => mv.NotifySettingsChanged() );
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void StartMedia()
        {
            this.LogicModel.Media.StartMedia();
            this.ModelViewCollection.ForEach( mv => mv.NotifyMediaWorkStatusChanged() );
        }


        /// <summary>
        /// <see cref="IModelViewMediator"/>
        /// </summary>
        public void StopMedia()
        {
            this.LogicModel.Media.StopMedia();
            this.ModelViewCollection.ForEach( mv => mv.NotifyMediaWorkStatusChanged() );
        }

        #endregion

        #region IModelDelegate Members


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public void OnMediaHardWareStatusChange()
        {
            this.ModelViewCollection.ForEach( mv => mv.NotifyMediaHardWareStatusChanged() );
        }


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public void OnColorDataPacketReady( ColorDataPacket packet )
        {
            this.ModelViewCollection.ForEach( mv => mv.UpdateVideoStream( packet ) );
        }


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public void OnMotionDataPacketReady( MotionDataPacket motionDataPacket )
        {
            this.ModelViewCollection.ForEach( mv => mv.UpdateTrackingStream( motionDataPacket ) );
        }


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public int[] RequestDrawingResolution()
        {
            int[] dimension = { 640, 480 };
            ModelViewColleague modelView = this.ModelViewCollection.FirstOrDefault( mv => mv.GetDrawingDimension() != null );
            if ( modelView != null )
            {
                dimension = modelView.GetDrawingDimension();
            }
            return dimension;
        }


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public void OnPointsAlgProcessed()
        {
            this.InEditingMode = true;
            this.ModelViewCollection.ForEach( mv => mv.UpdateScreenPaint( this.LogicModel.CreatedResult ) );
        }


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public void OnStartVoiceCommand()
        {
            if ( !this.InEditingMode )
            {
                this.ModelViewCollection.ForEach( mv => mv.NotifyRecordRequest() );
            }
        }


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public void OnStopVoiceCommand()
        {
            if ( !this.InEditingMode )
            {
                this.ModelViewCollection.ForEach( mv => mv.NotifyRecordRequest() );
            }
        }


        /// <summary>
        /// <see cref="IModelDelegate"/>
        /// </summary>
        public void OnNewVoiceCommand()
        {
            this.DispatchNewRequest();
        }


        #endregion

        #endregion

        #region Protected


        /// <summary>
        /// <see cref="DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            if ( this.LogicModel != null )
            {
                this.LogicModel.Dispose();
                this.LogicModel = null;
            }

            this.EditingDelegate = null;

            if ( this.ModelViewCollection != null )
            {
                this.ModelViewCollection.ForEach( mv => mv.Dispose() );
                this.ModelViewCollection.Clear();
                this.ModelViewCollection = null;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Notifies the collection of <see cref="ModelViewColleague"/> that new request was asked
        /// </summary>
        private void DispatchNewRequest()
        {
            this.ModelViewCollection.ForEach( mv => mv.NotifyNewRequest() );
        }

        #endregion
    }
}
