using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bspline.Core.Interfaces
{
    public interface IMenuDelegate
    {
        /// <summary>
        /// interface that enable or disable menu 
        /// </summary>
        void ShowSettings();
        void ShowUserManual( string userManualUrl );
        void ShowProjectInfo();
    }
}
