using System.Collections.Generic;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.DataPacket;

namespace Bspline.Core.Interfaces
{
    public interface IModelDelegate
    {
        /// <summary>
        /// Interface that manage the device connection and it status
        /// </summary>
        void OnMediaHardWareStatusChange();
        void OnColorDataPacketReady( ColorDataPacket packet );
        void OnMotionDataPacketReady( MotionDataPacket motionDataPacket );
        int[] RequestDrawingResolution();
        void OnPointsAlgProcessed();
        void OnStartVoiceCommand();
        void OnStopVoiceCommand();
        void OnNewVoiceCommand();
    }
}
