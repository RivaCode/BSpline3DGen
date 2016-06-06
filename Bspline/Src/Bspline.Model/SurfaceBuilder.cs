using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.Location;
using Bspline.Model.Algorithms;

namespace Bspline.Model
{
    /// <summary>
    /// Purpose of class to hold current selected algorithm engine
    /// </summary>
    internal class SurfaceBuilder
    {
        #region Fields

        private const string KNOT_MATRIX = "kmatrix";
        private const string KNOT_VECTOR = "kvector";

        #endregion

        /// <summary>
        /// Property which hold the current abstract algorithm engine
        /// </summary>
        private AlgorithmBase Alg { set; get; }

        /// <summary>
        /// Constructor of class to hold current selected algorithm engine
        /// </summary>
        public SurfaceBuilder()
        {
            this.Alg = AlgorithmBase.CreateAlgorithm(AlgorithmBase.Algorithm.BSpline);
        }

        /// <summary>
        /// Builds a surface data embadded in <see cref="Bspline.AlgorithmResults.AlgResult"/>
        /// </summary>
        /// <param name="knots">list of control point to create surface from</param>
        /// <returns></returns>
        public AlgResult BuildSurface(List<Point3D> knots)
        {
            return this.Alg.CreateCurvePoints(knots);
        }

        /// <summary>
        /// Rebuilds a surface data from xml
        /// </summary>
        /// <param name="xmlLayout">surface data</param>
        /// <returns></returns>
        public AlgResult RebuildSurface(XElement xmlLayout)
        {
            Matrix<Point3D> kMatrix = this.ParseLayout( xmlLayout );
            return this.RebuildSurface(kMatrix);
        }

        /// <summary>
        /// Rebuilds a surface data from <see cref="Bspline.AlgorithmResults.Matrix"/>
        /// </summary>
        /// <param name="kMatrix">surface data</param>
        /// <returns></returns>
        public AlgResult RebuildSurface( Matrix<Point3D> kMatrix )
        {
            return this.Alg.UpdateCurvePoints(kMatrix);
        }

        #region Build Data

        /// <summary>
        /// Save current algorithm result in format of <see cref="System.Xml.Linq.XElement"/>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public XElement BuildLayout(AlgResult result)
        {
            return new XElement(KNOT_MATRIX, this.BuildElements(result.KMatrix));
        }

        #endregion

        #region Private

        /// <summary>
        /// Recreates <see cref="Bspline.AlgorithmResults.Matrix"/> from a xml <see cref="System.Xml.Linq.XElement"/>
        /// </summary>
        /// <param name="xmlLayout">xml data</param>
        /// <returns></returns>
        private Matrix<Point3D> ParseLayout( XElement xmlLayout )
        {
            return this.ParseElements(xmlLayout );
        }

        #region Build/Parse Data

        /// <summary>
        /// Recreates <see cref="Bspline.AlgorithmResults.Matrix"/> from a xml <see cref="System.Xml.Linq.XElement"/>
        /// </summary>
        /// <param name="xmlLayout">xml data</param>
        /// <returns></returns>
        private Matrix<Point3D> ParseElements( XElement xmlLayout )
        {
            Matrix<Point3D> matrix = new Matrix<Point3D>();

            List<XElement> queryVectors = (from vector in xmlLayout.Element(KNOT_MATRIX).Elements()
                select vector).ToList();

            for (int i = 0; i < queryVectors.Count; i++)
            {
                matrix[i].AddRange(from pointElement in queryVectors[i].Elements()
                                   select new Point3D(
                                       double.Parse(pointElement.Attribute("x").Value),
                                       double.Parse(pointElement.Attribute("y").Value),
                                       double.Parse(pointElement.Attribute("z").Value)));
            }
            return matrix;
        }

        /// <summary>
        /// Build collection of xml's <see cref="System.Xml.Linq.XElement"/> from <see cref="Bspline.AlgorithmResults.Matrix"/>
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private IEnumerable<XElement> BuildElements( Matrix<Point3D> matrix )
        {
            var query = matrix.Select((vector, index) =>
                new XElement(string.Format("{0}_{1}", KNOT_VECTOR, index.ToString(CultureInfo.InvariantCulture)),
                    this.BuildElements(vector.Value)));
            return query;
        }

        /// <summary>
        /// Build collection of xml's <see cref="System.Xml.Linq.XElement"/> from a collection of <see cref="Bspline.Core.Location.Point3D"/>
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private IEnumerable<XElement> BuildElements(IEnumerable<Point3D> points)
        {
            return from point in points
                   select new XElement("Point",
                       new XAttribute("x", point.X),
                       new XAttribute("y", point.Y),
                       new XAttribute("z", point.Z));
        }

        #endregion

        #endregion
    }
}
