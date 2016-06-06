
namespace Bspline.Core.Location
{
    public class Point3D
    {
        /// <summary>
        /// Class that give format to the device points
        /// </summary>
        #region Properties

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// save X Y Z coardinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3D( double x, double y, double z )
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        #endregion

    }
}
