using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BsplineKinect
{
    /// <summary>
    /// Interaction logic for UserManual.xaml
    /// </summary>
    public partial class UserManual : Window
    {
        /// <summary>
        /// Field to hold the user manual url
        /// </summary>
        private string _navigationToUrl;

        /// <summary>
        /// Property to hold the navigation url of user manual
        /// </summary>
        public string NavigationToUrl
        {
            get { return _navigationToUrl; }
            set
            {
                _navigationToUrl = value;
                this.Browser.Navigate(this._navigationToUrl);
            }
        }
        /// <summary>
        /// Constructor of interaction logic for UserManual.xaml
        /// </summary>
        public UserManual()
        {
            InitializeComponent();
        }
    }
}
