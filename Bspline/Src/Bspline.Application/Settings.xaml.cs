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
using Bspline.Core;
using Bspline.WpfUI.ModelView;

namespace BsplineKinect
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        /// <summary>
        /// Constructor of interaction logic for Settings.xaml
        /// </summary>
       /// <param name="settingsNotifiableModelView">setings model view</param>
        public Settings(SettingsModelView settingsNotifiableModelView)
        {
            InitializeComponent();

            this.DataContext = settingsNotifiableModelView;
        }

        private void ButtonOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
