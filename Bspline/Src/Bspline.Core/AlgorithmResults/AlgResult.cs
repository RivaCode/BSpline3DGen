using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bspline.Core.Location;

namespace Bspline.Core.AlgorithmResults
{
    /// <summary>
    /// Class that containe the data of the surface
    /// </summary>
    public class AlgResult
    {
        #region Propertis
        /// <summary>
        /// Property that holds the knot vector for b-spline algorithm
        /// </summary>
        public List<Point3D> KVector
        {
            get { return KMatrix[0]; }
        }

        /// <summary>
        /// list of points with 3D coardinates
        /// </summary>
        public List<Point3D> AdjustedKVector
        {
            get { return this.AdjustedKMatrix[0]; }
        }
        /// <summary>
        /// the property that containe the control points
        /// </summary>
        public List<Point3D> StartCurveVector
        {
            get { return StartCurveMatrix[0]; }

        }
        /// <summary>
        /// the property that give the information of end point to have the pasabilety of closing the curve
        /// </summary>
        public List<Point3D> EndCurveVector
        {
            get { return EndCurveMatrix[0]; }
        }
        /// <summary>
        /// <seealso cref="Matrix.cs"/>
        /// Matrix's that contain list of properties that explained above
        /// </summary>
        public Matrix<Point3D> KMatrix { get; private set; }
        public Matrix<Point3D> AdjustedKMatrix { get; private set; }
        public Matrix<Point3D> StartCurveMatrix { get; private set; }
        public Matrix<Point3D> EndCurveMatrix { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// the information that hold the points for wpf 
        /// </summary>
        /// <param name="kMatrix"></param>
        public AlgResult( Matrix<Point3D> kMatrix )
            : this()
        {
            this.KMatrix = kMatrix;
        }
        /// <summary>
        /// maintain all the properties
        /// </summary>
        public AlgResult()
        {
            this.KMatrix = new Matrix<Point3D>();
            this.AdjustedKMatrix = new Matrix<Point3D>();
            this.StartCurveMatrix = new Matrix<Point3D>();
            this.EndCurveMatrix = new Matrix<Point3D>();
        }

        #endregion


    }
}
