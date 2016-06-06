using Bspline.Core.Interfaces;

namespace Bspline.Core.DataPacket
{
    public class MotionDataPacket
    {
        /// <summary>
        /// get the position of two hands from Kinect 
        /// </summary>
        #region Properties
        
        public IPosition LeftPosition { get; private set; }
        public IPosition RightPosition { get; private set; }
        
        #endregion

        #region Constructor
        /// <summary>
        /// set the left and rigth hand position
        /// </summary>
        /// <param name="leftPosition"></param>
        /// <param name="rightPosition"></param>
        public MotionDataPacket(IPosition leftPosition, IPosition rightPosition)
        {
            LeftPosition = leftPosition;
            RightPosition = rightPosition;
        } 

        #endregion
    }
}