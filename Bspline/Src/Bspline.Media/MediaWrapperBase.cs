using Bspline.Core;
using Bspline.Core.Interfaces;
using Bspline.Media.Device;

namespace Bspline.Media
{
    public abstract class MediaWrapperBase : DisposableObject, IMediaControllable
    {
        /// <summary>
        /// class that implement "IMediaControllable" interface
        /// <seealso cref="IMediaControllable"/>
        /// </summary>
        /// <param name="modelDelegate"></param>
        /// <returns></returns>
        #region Static

        public static MediaWrapperBase CreateMediaWrapper( IModelDelegate modelDelegate )
        {
            MediaWrapperBase wrapper = null;
            if ( KinectWrapper.IsKinectThere )
            {
                wrapper = new KinectWrapper( modelDelegate );
            }
            else
            {
                wrapper = new KinectDummy( modelDelegate );
            }
            return wrapper;
        }

        #endregion

        #region Properties
        /// <summary>
        /// connection delegate to the model
        /// </summary>
        internal IModelDelegate ModelDelegate { get; private set; }
        /// <summary>
        /// containe the wigth of the screen
        /// </summary>
        protected int WidthResolution { get; set; }
        /// <summary>
        /// containe the heigth of the screen "work space"
        /// </summary>
        protected int HeightResolution { get; set; }
        /// <summary>
        /// follow the the media status
        /// </summary>
        public abstract bool IsMediaStreaming { get; }
        public abstract bool IsMediaDeviceReady { get; }
        public abstract string MediaStatus { get; }
        public abstract double MediaAngle { get; }
        /// <summary>
        /// give the information if you use voice command or no
        /// </summary>
        public virtual bool UseVoiceCommand { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// crete new instance and give the delegate to contact with model
        /// </summary>
        /// <param name="modelDelegate"></param>
        protected MediaWrapperBase( IModelDelegate modelDelegate )
        {
            this.ModelDelegate = modelDelegate;
        }

        #endregion

        #region Public
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public abstract void ChangeAngle( int value );
        /// <summary>
        /// start the connection with the media
        /// </summary>
        public virtual void StartMedia()
        {
            int[] dimension = this.ModelDelegate.RequestDrawingResolution();
            this.WidthResolution = dimension[0];
            this.HeightResolution = dimension[1];
        }

        public abstract void StopMedia();

        #endregion

        #region Protected
        /// <summary>
        /// <seealso cref="DisposableObject"/>
        /// </summary>
        protected void OnMediaHardWareStatusChange()
        {
            this.ModelDelegate.OnMediaHardWareStatusChange();
        }

        #endregion

    }
}
