using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bspline.Core.Interfaces
{
    public interface IEditingDelegate
    {
        /// <summary>
        /// interface that provide the nessary function to edit the canvas
        /// </summary>
        /// <param name="inEditing"></param>
        void UpdateEditingMode( bool inEditing );
        XElement BuildEditingLayout();
        void ParseEditingLayout( XElement xml );
    }
}
