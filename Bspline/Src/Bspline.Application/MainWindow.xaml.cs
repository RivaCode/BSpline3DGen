using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using Bspline.Core.Interfaces;
using Bspline.Core.Location;
using Bspline.WpfUI;
using Bspline.WpfUI.ModelView;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BsplineKinect.Rendering;
using BsplineKinect.Rendering.Factory;
using BsplineKinect.Rendering.Render;
using Path = System.IO.Path;

namespace BsplineKinect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,
        IPaintingDelegate, IEditingDelegate, IMenuDelegate
    {
        #region Dependency Properties

        #region Use Fill

        /// <summary>
        /// Extending MainWindow with new property
        /// </summary>
        public static readonly DependencyProperty UseFillProperty
            = DependencyProperty.Register( "UseFill", typeof( bool ), typeof( MainWindow ),
                new PropertyMetadata( ChangeCallBack ) );

        /// <summary>
        /// Property to notifiy if layout should use fill
        /// </summary>
        public bool UseFill
        {
            get { return (bool)this.GetValue( UseFillProperty ); }
            set { this.SetValue( UseFillProperty, value ); }
        }

        #endregion

        #region Fill Color


        /// <summary>
        /// Extending MainWindow with new property
        /// </summary>
        public static readonly DependencyProperty FillColorProperty
            = DependencyProperty.Register( "FillColor", typeof( Color ), typeof( MainWindow ),
                new PropertyMetadata( Colors.Black, ChangeCallBack ) );

        /// <summary>
        /// Property to notifiy if layout should use fill color
        /// </summary>
        public Color FillColor
        {
            get { return (Color)this.GetValue( FillColorProperty ); }
            set { this.SetValue( FillColorProperty, value ); }
        }

        #endregion

        #region Stroke Color


        /// <summary>
        /// Extending MainWindow with new property
        /// </summary>
        public static readonly DependencyProperty StrokeColorProperty
            = DependencyProperty.Register( "StrokeColor", typeof( Color ), typeof( MainWindow ),
                new PropertyMetadata( Colors.Black, ChangeCallBack ) );


        /// <summary>
        /// Propety to notify the color of stroke
        /// </summary>
        public Color StrokeColor
        {
            get { return (Color)this.GetValue( StrokeColorProperty ); }
            set { this.SetValue( StrokeColorProperty, value ); }
        }

        #endregion

        #region Stroke Thickness

        /// <summary>
        /// Extending MainWindow with new property
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty
            = DependencyProperty.Register( "StrokeThickness", typeof( int ), typeof( MainWindow ),
                new PropertyMetadata( ChangeCallBack ) );

        /// <summary>
        /// Property to nofity what thickness size should the stroke be
        /// </summary>
        public int StrokeThickness
        {
            get { return (int)this.GetValue( StrokeThicknessProperty ); }
            set { this.SetValue( StrokeThicknessProperty, value ); }
        }

        #endregion

        #region Show Typical Polygon

        /// <summary>
        /// Extending MainWindow with new property
        /// </summary>
        public static readonly DependencyProperty ShowTypicalPolygonProperty
            = DependencyProperty.Register( "ShowTypicalPolygon", typeof( bool ), typeof( MainWindow ),
                new PropertyMetadata( ChangeCallBack ) );

        /// <summary>
        /// Property to notify if to show the typical polygon points
        /// </summary>
        public bool ShowTypicalPolygon
        {
            get { return (bool)this.GetValue( ShowTypicalPolygonProperty ); }
            set { this.SetValue( ShowTypicalPolygonProperty, value ); }
        }

        #endregion

        #region Render mode

        /// <summary>
        /// Extending MainWindow with new property
        /// </summary>
        public static readonly DependencyProperty RenderingModeProperty
            = DependencyProperty.Register( "RenderingMode", typeof( RenderMode ), typeof( MainWindow ),
                new PropertyMetadata( RenderMode.Regular2D, ChangeCallBack ),
                ValidateRenderMode );

        /// <summary>
        /// Poperty to notify in which <see cref="RenderMode"/> the application is
        /// </summary>
        public RenderMode RenderingMode
        {
            get { return (RenderMode)this.GetValue( RenderingModeProperty ); }
            set { this.SetValue( RenderingModeProperty, value ); }
        }

        #endregion

        #region Show Editing Menu


        /// <summary>
        /// Extending MainWindow with new property
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty
            = DependencyProperty.Register( "IsEditing", typeof( bool ), typeof( MainWindow ),
                new PropertyMetadata( false ) );

        /// <summary>
        /// Property to notify if application in editing mode
        /// </summary>
        public bool IsEditing
        {
            get { return (bool)GetValue( IsEditingProperty ); }
            set { SetValue( IsEditingProperty, value ); }
        }

        #endregion

        /// <summary>
        /// New extanded dependency properties change call back
        /// </summary>
        /// <param name="dependencyObject">the new property</param>
        /// <param name="args">the property args</param>
        private static void ChangeCallBack( DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args )
        {
            ( dependencyObject as MainWindow ).DependencyMapping[args.Property.Name]( args.NewValue );
        }

        /// <summary>
        /// New extanded dependency properties validation call back
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ValidateRenderMode( object value )
        {
            RenderMode cName = (RenderMode)value;
            return Enum.GetValues( typeof( RenderMode ) ).Cast<RenderMode>().Any( item => item == cName );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property to hold new dependency property requeried actions upon change
        /// </summary>
        private Dictionary<string, Action<object>> DependencyMapping { get; set; }

        /// <summary>
        /// Property to hold <see cref="ModelViewMediator"/> to delegate to
        /// </summary>
        internal ModelViewMediator Mediator { get; private set; }

        /// <summary>
        /// Property to hold <see cref="RenderProvider"/> as helper class
        /// </summary>
        private RenderProvider RenderingProvider { get; set; }

        #endregion

        /// <summary>
        /// Constructor of interaction logic for MainWindow.xaml
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Mediator = new ModelViewMediator( this );

            ControlModelView controlNotifiableModelView = new ControlModelView( Mediator );
            this.InfoControl.DataContext = controlNotifiableModelView;

            PaintModelView paintNotifiableModelView = new PaintModelView( Mediator, this );
            this.PaintControl.DataContext = paintNotifiableModelView;
            this.GridLowerPanel.DataContext = paintNotifiableModelView;

            MenuModelView menuNotifiableModelView = new MenuModelView( Mediator, this );
            this.MenuControl.DataContext = menuNotifiableModelView;

            this.Mediator.Initialize();

            this.RenderingProvider = new RenderProvider(
                new RenderFactory( this ),
                this.Mediator.AppSettings.DefaultRenderMode );

            this.RegisterDepencencyMapping();
        }

        #region IPaintingDelegate members

        /// <summary>
        /// <see cref="IPaintingDelegate"/>
        /// </summary>
        /// <param name="newPoint"></param>
        public void OnGestureMove( Point3D newPoint )
        {
            Dispatcher.BeginInvoke( (Action)( () =>
            {
                Canvas.SetLeft( TrackingEllipse, newPoint.X );
                Canvas.SetTop( TrackingEllipse, newPoint.Y );
            } ) );
        }

        /// <summary>
        /// <see cref="IPaintingDelegate"/>
        /// </summary>
        /// <param name="result"></param>
        public void OnSurfaceReady( AlgResult result )
        {
            Dispatcher.BeginInvoke( (Action)( () =>
            {
                if ( result != null )
                {
                    this.RenderingProvider.Render( result );
                }
                else
                {
                    MessageBox.Show( "Points data was corrupted, please try again !",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error );
                }
            } ) );
        }

        /// <summary>
        /// <see cref="IPaintingDelegate"/>
        /// </summary>
        public void OnSurfaceClear()
        {
            this.OnMenuItemNewClick();
        }

        #endregion

        #region IEditingDelegate members

        /// <summary>
        /// <see cref="IEditingDelegate"/>
        /// </summary>
        /// <param name="inEditing"></param>
        public void UpdateEditingMode( bool inEditing )
        {
            this.IsEditing = inEditing;

        }

        /// <summary>
        /// <see cref="IEditingDelegate"/>
        /// </summary>
        /// <returns></returns>
        public XElement BuildEditingLayout()
        {
            return new XElement( "editing",
                new XElement( "usefil", this.UseFill.ToString() ),
                new XElement( "fillcolor", this.FillColor.ToString() ),
                new XElement( "strokecolor", this.StrokeColor.ToString() ),
                new XElement( "strokethickness", this.StrokeThickness.ToString( CultureInfo.InvariantCulture ) ),
                new XElement( "showtypicalpolygon", this.ShowTypicalPolygon.ToString() ),
                new XElement( "rendermode", this.RenderingMode.ToString() ) );
        }

        /// <summary>
        /// <see cref="IEditingDelegate"/>
        /// </summary>
        /// <param name="xml"></param>
        public void ParseEditingLayout( XElement xml )
        {
            XElement editing = xml.Element( "editing" );
            if ( editing != null )
            {
                Dispatcher.BeginInvoke( (Action)( () =>
                {
                    this.UseFill = bool.Parse( editing.Element( "usefil" ).Value );
                    this.FillColor = (Color)ColorConverter.ConvertFromString( editing.Element( "fillcolor" ).Value );
                    this.StrokeColor = (Color)ColorConverter.ConvertFromString( editing.Element( "strokecolor" ).Value );
                    this.StrokeThickness = int.Parse( editing.Element( "strokethickness" ).Value );
                    this.ShowTypicalPolygon = bool.Parse( editing.Element( "showtypicalpolygon" ).Value );
                    this.RenderingMode = (RenderMode)Enum.Parse( typeof( RenderMode ), editing.Element( "rendermode" ).Value );
                } ) );
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Registers Depencency mappings actions
        /// </summary>
        private void RegisterDepencencyMapping()
        {
            this.DependencyMapping = new Dictionary<string, Action<object>>
            {
                {
                    BSplineUtility.Instance.NameOf(() => this.UseFill),
                    obj =>
                    {
                        this.RenderingProvider.Renderer.UseFill = (bool) obj;
                        this.RenderingProvider.Renderer.FillBrush = new SolidColorBrush(this.FillColor);
                    }
                },
                {
                    BSplineUtility.Instance.NameOf(() => this.FillColor),
                    obj => this.RenderingProvider.Renderer.FillBrush = new SolidColorBrush((Color) obj)
                },
                {
                    BSplineUtility.Instance.NameOf(() => this.StrokeColor),
                    obj => this.RenderingProvider.Renderer.StrokeBrush = new SolidColorBrush((Color) obj)
                },
                {
                    BSplineUtility.Instance.NameOf(() => this.StrokeThickness),
                    obj =>
                    {
                        this.RenderingProvider.Renderer.StrokeSize = (int) obj;
                        this.RenderingProvider.Renderer.StrokeBrush = new SolidColorBrush(this.StrokeColor);
                    }
                },
                {
                    BSplineUtility.Instance.NameOf(() => this.ShowTypicalPolygon),
                    obj => this.RenderingProvider.Renderer.ShowTypicalPolygon = (bool) obj
                },
                {
                    BSplineUtility.Instance.NameOf(()=>this.RenderingMode),
                    obj =>
                    {
                        this.RenderingProvider.Change((RenderMode)obj);
                        this.RenderingProvider.Renderer.Render(this.Mediator.LatestResult);

                        this.RenderingProvider.Renderer.UseFill = this.UseFill;
                        this.RenderingProvider.Renderer.FillBrush = new SolidColorBrush(this.FillColor);
                        this.RenderingProvider.Renderer.StrokeSize = this.StrokeThickness;
                        this.RenderingProvider.Renderer.StrokeBrush = new SolidColorBrush(this.StrokeColor);
                        this.RenderingProvider.Renderer.ShowTypicalPolygon = this.ShowTypicalPolygon;
                    }
                }
            };
        }

        /// <summary>
        /// Will be called to finilize application before closing
        /// </summary>
        private void OnMainWindowClosed()
        {
            if ( Mediator != null )
            {
                Mediator.Dispose();
            }
            this.Closed -= MainWindowOnClosed;
        }

        /// <summary>
        /// Called when the the tack bar thumb is moved
        /// </summary>
        private void OnThumbDragCompleted()
        {
            ControlModelView notifiableModelView = this.InfoControl.DataContext as ControlModelView;
            if ( notifiableModelView != null )
            {
                ICommand angleChangeCommand = notifiableModelView.AngleChangeCommand;
                if ( angleChangeCommand.CanExecute( null ) )
                {
                    angleChangeCommand.Execute( null );
                }
            }
        }

        /// <summary>
        /// Called when window sizes changes
        /// </summary>
        private void OnMainWindowLayoutUpdate()
        {
            this.Mediator.NotifyDrawingLayoutDimensionChange(
                this.PaintControl.ActualWidth,
                this.PaintControl.ActualHeight );
        }

        private void OnMenuItemNewClick()
        {
            this.RenderingProvider.Clear();
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate the call of window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindowOnClosed( object sender, EventArgs e )
        {
            this.OnMainWindowClosed();
        }


        /// <summary>
        /// Delegate the call of track bar thumb drag is complete
        /// </summary>
        private void Thumb_OnDragCompleted( object sender, DragCompletedEventArgs e )
        {
            this.OnThumbDragCompleted();
        }


        /// <summary>
        /// Delegate the call of window layout is changed
        /// </summary>
        private void PaintControl_OnLayoutUpdated( object sender, EventArgs e )
        {
            this.OnMainWindowLayoutUpdate();
        }


        /// <summary>
        /// Delegate the call of menu item is clicked
        /// </summary>
        private void MenuItem_OnClick( object sender, RoutedEventArgs e )
        {
            this.OnMenuItemNewClick();
        }



        #endregion


        #region IMenuDelegate Members

        public void ShowSettings()
        {
            SettingsModelView notifiableModelView = new SettingsModelView( this.Mediator.AppSettings );
            Settings settingsWindow = new Settings( notifiableModelView );
            settingsWindow.ShowDialog();

            if ( notifiableModelView.SettingsChanged )
            {
                this.Mediator.UpdateSettings( notifiableModelView.NewSettings );
            }
        }

        public void ShowUserManual( string userManualUrl )
        {
            UserManual manual = new UserManual
            {
                NavigationToUrl = userManualUrl
            };
            manual.Show();
        }

        public void ShowProjectInfo()
        {
            MessageBox.Show( string.Format( "Final graduation project: \"Bspline 3D surface generator\"{0}{0}{0}" +
                                          "This project will demonstrate Kinect's ability to process human gesture tracking for creating surfaces{0}{0}" +
                                          "Created by:{0}" +
                                          "Michael Gotfrid{0}" +
                                          "Stas Rivkin{0}", Environment.NewLine ),
                "Project Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information );
        }

        #endregion
    }
}

