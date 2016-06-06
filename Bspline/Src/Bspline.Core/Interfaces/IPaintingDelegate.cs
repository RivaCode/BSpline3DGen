using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.Location;

namespace Bspline.Core.Interfaces
{
    public interface IPaintingDelegate
    {
        /// <summary>
        /// Interface that give the control of point generation and movment
        /// </summary>
        /// <param name="newPoint"></param>
        void OnGestureMove( Point3D newPoint );
        void OnSurfaceClear();
        void OnSurfaceReady( AlgResult algPoints );
    }
}
