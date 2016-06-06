using System.Collections.Generic;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.Location;


namespace Bspline.Model.Algorithms
{
    /// <summary>
    /// Abstract class for the algorithm to act as a rendering engine
    /// </summary>
    internal abstract class AlgorithmBase
    {
        /// <summary>
        /// Decides which algorithm to use
        /// </summary>
        public enum Algorithm
        {
            BSpline
        }

        /// <summary>
        /// Create an algorithm enigne depending to <para name="type"/>
        /// </summary>
        /// <param name="type">Algorithm type</param>
        /// <returns></returns>
        public static AlgorithmBase CreateAlgorithm(Algorithm type)
        {   
            return new BSplineAlgorithm();
        }

        /// <summary>
        /// Create curve points according to algorithm engine
        /// </summary>
        /// <param name="knots"></param>
        /// <returns></returns>
        public abstract AlgResult CreateCurvePoints(List<Point3D> knots);

        /// <summary>
        /// Updates curve points of <para name="knots"/>
        /// </summary>
        /// <param name="knots"></param>
        /// <returns></returns>
        public abstract AlgResult UpdateCurvePoints( Matrix<Point3D> knots );

    }
}
