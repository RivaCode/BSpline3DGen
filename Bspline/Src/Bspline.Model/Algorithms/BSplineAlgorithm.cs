using System;
using System.Collections.Generic;
using System.Linq;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.Location;

namespace Bspline.Model.Algorithms
{
    /// <summary>
    /// Class for the BSpline algorithm to act as a rendering engine
    /// </summary>
    internal class BSplineAlgorithm : AlgorithmBase
    {
        /// <summary>
        /// Create curve points according to algorithm engine
        /// </summary>
        /// <param name="knots"></param>
        /// <returns></returns>
        public override AlgResult CreateCurvePoints(List<Point3D> kvectorPoints)
        {
            AlgResult result = new AlgResult();
            result.KVector.Add(kvectorPoints[0]);

            this.FilterKVectorPointsByTreshHold(kvectorPoints, result);

            const int deltaStep = 25;
            for (int i = 1; i < 4; i++)
            {
                int offset = i * deltaStep;
                foreach (Point3D originalPoint3D in result.KVector)
                {
                    result.KMatrix[i].Add(new Point3D(originalPoint3D.X + offset, originalPoint3D.Y - offset, originalPoint3D.Z));
                }
            }

            this.CurvePointsInner(result);
            return result;
        }

        /// <summary>
        /// Updates curve points of <para name="knots"/>
        /// </summary>
        /// <param name="knots"></param>
        /// <returns></returns>
        public override AlgResult UpdateCurvePoints( Matrix<Point3D> kMatrix )
        {
            AlgResult result = new AlgResult(kMatrix);

            this.CurvePointsInner(result);

            return result;
        }

        /// <summary>
        /// Filter points which are two close according to treshhold
        /// </summary>
        /// <param name="kvectorPoints"></param>
        /// <param name="result"></param>
        private void FilterKVectorPointsByTreshHold(List<Point3D> kvectorPoints, AlgResult result)
        {
            for (int i = 0, j = 0; i < kvectorPoints.Count - 1; i++)
            {
                if (Math.Sqrt(
                    Math.Pow(kvectorPoints[i + 1].X - kvectorPoints[j].X, 2) +
                    Math.Pow(kvectorPoints[i + 1].Y - kvectorPoints[j].Y, 2)
                    ) > 35)
                {
                    result.KVector.Add(kvectorPoints[i + 1]);
                    j = i + 1;
                }
            }

            result.KVector.ForEach(p3D=>p3D.Y+=75);
        }

        /// <summary>
        /// Creates the curve points according to the result
        /// </summary>
        /// <param name="result"></param>
        private void CurvePointsInner(AlgResult result)
        {
            if (result.KVector.Count == 1)
            {
                result.StartCurveVector.Add(new Point3D(
                    (2 * result.KVector[0].X + result.KVector[1].X) / 3,
                    (2 * result.KVector[0].Y + result.KVector[1].Y) / 3,
                    result.KVector[0].Z));

                result.EndCurveVector.Add(new Point3D(
                    2 * result.StartCurveVector[0].X - result.KVector[0].X,
                    2 * result.StartCurveVector[0].Y - result.KVector[0].Y,
                    result.KVector[0].Z));

                return;
            }

            for (int i = 0; i < result.KMatrix.Rows; i++)
            {
                CreateVectors(result,i);
            }
        }

        /// <summary>
        /// Create vector according to <code>vectorIndex</code> and insert it into <code>result</code>
        /// </summary>
        /// <param name="result"></param>
        /// <param name="vectorIndex"></param>
        private void CreateVectors(AlgResult result,int vectorIndex)
        {
            int n = result.KVector.Count - 1;
            double[] rhs = new double[n];

            List<Point3D> adjustedVector = this.CreatedAdjustKnotVector(result.KMatrix[vectorIndex]);
            for (int columnIndex = 0; columnIndex < adjustedVector.Count; columnIndex++)
            {
                result.AdjustedKMatrix[vectorIndex].Add(adjustedVector[columnIndex]);
            }

            // Set right hand side X values
            for (int i = 1; i < n - 1; ++i)
            {
                rhs[i] = 4 * result.AdjustedKMatrix[vectorIndex][i].X + 2 * result.AdjustedKMatrix[vectorIndex][i + 1].X;
            }
            rhs[0] = result.AdjustedKMatrix[vectorIndex][0].X + 2 * result.AdjustedKMatrix[vectorIndex][1].X;
            rhs[n - 1] = (8 * result.AdjustedKMatrix[vectorIndex][n - 1].X + result.AdjustedKMatrix[vectorIndex][n].X) / 2.0;
            // Get first control points X-values
            double[] x = CreateStartCurvePoints(rhs);

            // Set right hand side Y values
            for (int i = 1; i < n - 1; ++i)
            {
                rhs[i] = 4*result.AdjustedKMatrix[vectorIndex][i].Y + 2*result.AdjustedKMatrix[vectorIndex][i + 1].Y;
            }
            rhs[0] = result.AdjustedKMatrix[vectorIndex][0].Y + 2*result.AdjustedKMatrix[vectorIndex][1].Y;
            rhs[n - 1] = (8*result.AdjustedKMatrix[vectorIndex][n - 1].Y + result.AdjustedKMatrix[vectorIndex][n].Y)/2.0;
            // Get first control points Y-values
            double[] y = CreateStartCurvePoints(rhs);


            for (int i = 0; i < n; ++i)
            {
                // First control point
                result.StartCurveMatrix[vectorIndex].Add(new Point3D(x[i], y[i], result.AdjustedKMatrix[vectorIndex][i].Z));
                // Second control point
                result.EndCurveMatrix[vectorIndex].Add(i < n - 1
                    ? new Point3D(2*result.AdjustedKMatrix[vectorIndex][i + 1].X - x[i + 1],
                        2*result.AdjustedKMatrix[vectorIndex][i + 1].Y - y[i + 1], result.AdjustedKMatrix[vectorIndex][i].Z)
                    : new Point3D((result.AdjustedKMatrix[vectorIndex][n].X + x[n - 1])/2, (result.AdjustedKMatrix[vectorIndex][n].Y + y[n - 1])/2,
                        result.AdjustedKMatrix[vectorIndex][i].Z));
            }
        }

        /// <summary>
        /// Create start curve points
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private double[] CreateStartCurvePoints(double[] rhs)
        {
            int n = rhs.Length;
            double[] x = new double[n]; // Solution vector.
            double[] tmp = new double[n]; // Temp workspace.

            double b = 2.0;
            x[0] = rhs[0] / b;
            for (int i = 1; i < n; i++) // Decomposition and forward substitution.
            {
                tmp[i] = 1 / b;
                b = (i < n - 1 ? 4.0 : 3.5) - tmp[i];
                x[i] = (rhs[i] - x[i - 1]) / b;
            }
            for (int i = 1; i < n; i++)
                x[n - i - 1] -= tmp[n - i] * x[n - i]; // Backsubstitution.

            return x;
        }

        /// <summary>
        /// Update the knot vector with adjustment
        /// </summary>
        /// <param name="knotVector"></param>
        /// <returns></returns>
        private List<Point3D> CreatedAdjustKnotVector(List<Point3D> knotVector)
        {
            return BSplineUtility.Instance.AdjustVector(knotVector);
        }
    }
}
