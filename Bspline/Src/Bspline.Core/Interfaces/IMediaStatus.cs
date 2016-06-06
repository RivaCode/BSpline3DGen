
namespace Bspline.Core.Interfaces
{
    public interface IMediaStatus
    {
        /// <summary>
        /// interface that any device sholde implement to get it status
        /// </summary>
        bool IsMediaStreaming { get; }
        bool IsMediaDeviceReady { get; }
        string MediaStatus { get; }
        double MediaAngle { get; }
    }
}
