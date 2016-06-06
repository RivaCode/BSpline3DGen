using Bspline.Core.Interfaces;
using Bspline.Core.Location;

namespace Bspline.Media.BodyParts
{
    public class RightPosition : IPosition
    {
        /// <summary>
        /// cover the rigth hand position 
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
        public RightPosition( Point3D position )
        {
            this.Position = position;
        }

        #endregion
    }
}