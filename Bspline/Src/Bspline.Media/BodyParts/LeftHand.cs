using Bspline.Core.Interfaces;
using Bspline.Core.Location;

namespace Bspline.Media.BodyParts
{
    public class LeftPosition : IPosition
    {
        /// <summary>
        /// cover the left hand position 
        /// <seealso cref="IPosition"/>
        /// </summary>
        #region Properties

        public Point3D Position { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// store the last position
        /// </summary>
        /// <param name="position"></param>
        public LeftPosition( Point3D position )
        {
            this.Position = position;
        }

        #endregion

    }
}