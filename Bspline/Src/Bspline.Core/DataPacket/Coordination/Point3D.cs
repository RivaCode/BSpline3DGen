using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bspline.Core.Data.Coordination
{
    public class Point3D
    {
        /// <summary>
        /// Point type that we get from the device
        /// </summary>
        #region Porperty
        
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        #endregion

        #region Constructor

        public Point3D(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        
        #endregion
    }
}
