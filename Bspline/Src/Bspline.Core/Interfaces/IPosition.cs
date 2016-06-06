using Bspline.Core.Location;

namespace Bspline.Core.Interfaces
{
    public interface IPosition
    {
        /// <summary>
        /// Position format from device
        /// </summary>
        Point3D Position { get; }
    }
}
