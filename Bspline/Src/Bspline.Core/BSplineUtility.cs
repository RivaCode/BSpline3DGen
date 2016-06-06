using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bspline.Core.Location;

namespace Bspline.Core
{
    public class BSplineUtility
    {
        /// <summary>
        /// Class that hold number of usefull utilites for the project
        /// </summary>
        private static BSplineUtility _instance;
        private static readonly object Lock = new object();

        public static BSplineUtility Instance
        {
            get
            {
                if ( _instance == null )
                {
                    lock ( Lock )
                    {
                        if ( _instance == null )
                        {
                            _instance = new BSplineUtility();
                        }
                    }
                }
                return _instance;
            }
        }

        private BSplineUtility() { }
        /// <summary>
        /// Utillity that give the member name 
        /// </summary>
        /// <typeparam name="TName"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string NameOf<TName>( Expression<Func<TName>> expression )
        {
            return ( (MemberExpression)expression.Body ).Member.Name;
        }
        /// <summary>
        /// creating new directory and give the path of it
        /// </summary>
        /// <param name="fullPath"></param>
        public void CreateDir( string fullPath )
        {
            if ( !Directory.Exists( fullPath ) )
            {
                Directory.CreateDirectory( fullPath );
            }
        }
        /// <summary>
        /// give the value of any Enum of the system
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public IEnumerable<TEnum> GetEnumValues<TEnum>()
        {
            return Enum.GetValues( typeof( TEnum ) ).Cast<TEnum>();
        }
        /// <summary>
        /// get element value
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public string GetElementValue( XElement xml, string element )
        {
            string result = string.Empty;
            var xElement = xml.Element( element );
            if ( xElement != null )
            {
                result = xElement.Value;
            }
            return result;
        }
        /// <summary>
        /// provide the full image source help to see the update image location
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public string GetFullImageSource( string imageName )
        {
            return string.Format( "pack://application:,,,/BsplineKinect;component/Images/{0}.png", imageName );
        }
        /// <summary>
        /// generate 3 more points to give the 2.5D visualy
        ///
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public List<Point3D> AdjustVector( List<Point3D> vector )
        {
            const double t = .45;

            List<Point3D> adjusted = new List<Point3D>( vector.Select( p3D => new Point3D( p3D.X, p3D.Y, p3D.Z ) ) );
            for ( int i = 1; i < vector.Count - 1; i++ )
            {

                double dx1 = this.FindMiddle( vector[i].X, vector[i - 1].X, t );
                double dY1 = this.FindMiddle( vector[i].Y, vector[i - 1].Y, t );

                double dx2 = this.FindMiddle( vector[i + 1].X, vector[i].X, t );
                double dY2 = this.FindMiddle( vector[i + 1].Y, vector[i].Y, t );

                double dx = this.FindMiddle( dx1, dx2, t );
                double dY = this.FindMiddle( dY1, dY2, t );

                adjusted[i].X = dx;
                adjusted[i].Y = dY;
            }

            return adjusted;
        }
        /// <summary>
        /// Find mmiddel distance between two (x,y) points
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public double FindMiddle( double start, double end, double factor )
        {
            double value = Math.Min( start, end ) +
                             ( ( Math.Abs( start - end ) + 1 ) * factor );

            return value;
        }
    }
}
