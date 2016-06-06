using Bspline.Core.DataPacket;

namespace Bspline.Model.Interfaces
{
    /// <summary>
    /// Interface which will delegates filtered results to <see cref="Bspline.Model.LogicManager"/>
    /// </summary>
    interface ILogicDelegate
    {
        void OnMotionDataPacketFiltered(MotionDataPacket dataPacket);
    }
}
