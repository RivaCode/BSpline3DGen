
namespace Bspline.Core.Interfaces
{
    public interface IMediaControllable : IMediaStatus
    {
        /// <summary>
        /// interface that start or stop media device
        /// </summary>
        void StartMedia();

        void StopMedia();
    }
}
