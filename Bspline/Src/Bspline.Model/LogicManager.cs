using System.Collections.Generic;
using System.Xml.Linq;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.DataPacket;
using Bspline.Core.Interfaces;
using Bspline.Core.Location;
using Bspline.Media;
using Bspline.Model.Interfaces;

namespace Bspline.Model
{
    /// <summary>
    /// Class responsible for all data processing
    /// </summary>
    public class LogicManager : DisposableObject, IModelDelegate, ILogicDelegate
    {
        /// <summary>
        /// Field will hold the rendering engine
        /// </summary>
        private SurfaceBuilder _builder;

        #region Properties

        /// <summary>
        /// Property which logic manager delegate calls to it from self and from <see cref="Bspline.Media"/>
        /// </summary>
        private IModelDelegate ModelDelegate { get; set; }

        /// <summary>
        /// Property which hold our media device
        /// </summary>
        public MediaWrapperBase Media { get; private set; }

        /// <summary>
        /// Property which hold our NoiseFilter helper
        /// </summary>
        private NoiseFilter NoiseFilter { get; set; }

        /// <summary>
        /// Property which holds our filtered collected points
        /// </summary>
        private List<IPosition> AlgPoints { get; set; }

        /// <summary>
        /// Property which hold our created alg result to algorithm engine
        /// </summary>
        public AlgResult CreatedResult { get; private set; }

        /// <summary>
        /// Property to notify if application in record mode
        /// </summary>
        public bool Record { get; set; }

        /// <summary>
        /// Propety which hold the rendering engine 
        /// </summary>
        private SurfaceBuilder Builder
        {
            get
            {
                if (_builder == null)
                {
                    _builder = new SurfaceBuilder();
                }
                return _builder;
            }
        }

        #endregion

        #region Constructor
        
        /// <summary>
        /// Constructor for class which is responsible for all data processing
        /// </summary>
        /// <param name="modelDelegate">interface to which the this object will delegate calls</param>
        public LogicManager(IModelDelegate modelDelegate)
        {
            this.ModelDelegate = modelDelegate;
            this.NoiseFilter = new NoiseFilter(this);
            this.Media = MediaWrapperBase.CreateMediaWrapper(this);
            this.AlgPoints = new List<IPosition>();
        }

        #endregion

        #region Public

        #region IModelDelegate Members

        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        /// <param name="packet">holds video information</param>
        public void OnColorDataPacketReady(ColorDataPacket packet)
        {
           this.ModelDelegate.OnColorDataPacketReady(packet);
        }

        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        public void OnMotionDataPacketReady(MotionDataPacket motionDataPacket)
        {
            this.NoiseFilter.Filter(motionDataPacket);

        }

        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        public void OnMediaHardWareStatusChange()
        {
            this.ModelDelegate.OnMediaHardWareStatusChange();
        }
        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        public int[] RequestDrawingResolution()
        {
            return this.ModelDelegate.RequestDrawingResolution();
        }

        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        public void OnPointsAlgProcessed()
        {
        }

        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        public void OnStartVoiceCommand()
        {
            this.ModelDelegate.OnStartVoiceCommand();
        }

        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        public void OnStopVoiceCommand()
        {
            this.ModelDelegate.OnStopVoiceCommand();
        }

        /// <summary>
        /// <see cref="Bspline.Core.Interfaces.IModelDelegate"/>
        /// </summary>
        public void OnNewVoiceCommand()
        {
            this.ModelDelegate.OnNewVoiceCommand();
        }

        #endregion
        
        #region ILogicDelegate Members

        /// <summary>
        /// <see cref="Bspline.Model.Interfaces.ILogicDelegate"/>
        /// </summary>
        public void OnMotionDataPacketFiltered(MotionDataPacket dataPacket)
        {
            this.ModelDelegate.OnMotionDataPacketReady(dataPacket);
            if (this.Record)
            {
                this.AlgPoints.Add(dataPacket.RightPosition);
            }
        }

        #endregion

        /// <summary>
        /// Parse settings packet from UI
        /// </summary>
        /// <param name="settings">current settings to parse</param>
        public void ParseSettings(SettingsNotification settings)
        {
            this.NoiseFilter.NumberOfPointsToFilter = settings.PointsToFilter;
            this.Media.UseVoiceCommand = settings.UseVoiceCommands;
        }

        /// <summary>
        /// Save current algorithm result in format of <see cref="System.Xml.Linq.XElement"/>
        /// </summary>
        /// <returns>xml format of algorithm result</returns>
        public XElement SaveRequest()
        {
            return this.Builder.BuildLayout(this.CreatedResult);
        }

        /// <summary>
        /// Load algorithm result from xml
        /// </summary>
        /// <param name="layout">xml format of algorithm result</param>
        public void LoadRequest(XElement layout)
        {
            this.CreatedResult = this.Builder.RebuildSurface(layout);
            ModelDelegate.OnPointsAlgProcessed();
        }

        /// <summary>
        /// Will process collected points and pass notification to UI when algorithm result is ready
        /// </summary>
        public void ProcessData()
        {
            this.NoiseFilter.Reset();
            this.CreatedResult = this.Builder.BuildSurface(this.AlgPoints.ConvertAll(hand => hand.Position));
            ModelDelegate.OnPointsAlgProcessed();

        }

        /// <summary>
        /// Will update algorithm result according to <code>newPoint</code> and will notify UI
        /// </summary>
        /// <param name="oldPoint">saved point in algorithm result</param>
        /// <param name="newPoint">new point modified by UI</param>
        public void UpdatePoints(Point3D oldPoint, Point3D newPoint)
        {
            Point3D foundPoint = this.CreatedResult.KMatrix.Find(p3D => p3D.X.CompareTo(oldPoint.X) == 0
                                                                          && p3D.Y.CompareTo(oldPoint.Y) == 0);
            if (foundPoint != null)
            {
                foundPoint.X = newPoint.X;
                foundPoint.Y = newPoint.Y;
                foundPoint.Z = newPoint.Z;
            }
            this.CreatedResult = this.Builder.RebuildSurface(this.CreatedResult.KMatrix);
            ModelDelegate.OnPointsAlgProcessed();
        }

        /// <summary>
        /// Will cause the <see cref="Bspline.Model.LogicManager"/> to reset it's inner cache
        /// </summary>
        public void ReStart()
        {
            this.NoiseFilter.Reset();
            this.AlgPoints.Clear();
            this.Record = false;
            this.CreatedResult = null;
        }
        #endregion
         
        #region Protected

        /// <summary>
        /// <see cref="Bspline.Core.DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            if (this.Media != null)
            {
                this.Media.Dispose();
                this.Media = null;
            }
            if (this.NoiseFilter != null)
            {
                this.NoiseFilter.Dispose();
                this.NoiseFilter = null;
            }
        }

        #endregion
    }
}
