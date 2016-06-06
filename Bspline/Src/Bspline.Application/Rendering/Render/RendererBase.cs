using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;

namespace BsplineKinect.Rendering.Render
{
    /// <summary>
    /// An Abstract class to allow the <see cref="MainWindow"/> to render the layout according to <see cref="RenderMode"/>
    /// </summary>
    internal abstract class RendererBase : DisposableObject
    {
        #region Properties

        #region Editing
        /// <summary>
        /// Property to hold info if to use fill
        /// </summary>
        internal virtual bool UseFill { get; set; }

        /// <summary>
        /// Property to hold info of what stroke size
        /// </summary>
        internal virtual int StrokeSize { get; set; }

        /// <summary>
        /// Property to hold the fill <see cref="Brush"/>
        /// </summary>
        internal virtual Brush FillBrush { get; set; }
        /// <summary>
        /// Property to hold the Stroke <see cref="Brush"/>
        /// </summary>
        internal virtual Brush StrokeBrush { get; set; }
        /// <summary>
        /// Property to hold the info if to show typical polygon
        /// </summary>
        internal virtual bool ShowTypicalPolygon { get; set; }

        #endregion

        /// <summary>
        /// Property to hold the <see cref="AlgResult"/>
        /// </summary>
        protected AlgResult AlgInfoResult { get; set; }

        /// <summary>
        /// Property to hold the <see cref="MainWindow"/> which to render
        /// </summary>
        protected MainWindow Main { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of abstract class to allow the <see cref="MainWindow"/> to render the layout according to <see cref="RenderMode"/>
        /// </summary>
        /// <param name="main">the main window to render</param>
        protected RendererBase( MainWindow main )
        {
            Main = main;
        }

        #endregion

        #region Public

        /// <summary>
        /// Render the <see cref="MainWindow"/> with the <see cref="AlgInfoResult"/>
        /// </summary>
        /// <param name="result"></param>
        public void Render( AlgResult result )
        {
            this.AlgInfoResult = result;
            this.Main.GridLowerPanel.Visibility = Visibility.Hidden;
            this.Main.PaintControl.Visibility = Visibility.Hidden;


            this.Clear();
            this.RenderOverride();
        }

        /// <summary>
        /// Reset the current object inner data
        /// </summary>
        public void Reset()
        {
            this.Main.PaintControl.Visibility = Visibility.Visible;
            this.Main.GridLowerPanel.Visibility = Visibility.Visible;
            this.ResetOverride();
        }

        #endregion

        #region Protected

        /// <summary>
        /// Will clear the derived render
        /// </summary>
        protected virtual void Clear() { }

        /// <summary>
        /// Actual rendering logic according to dervied class
        /// </summary>
        protected abstract void RenderOverride();

        /// <summary>
        /// Actual reset logic according to derived class
        /// </summary>
        protected abstract void ResetOverride();

        /// <summary>
        /// <see cref="DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            this.Clear();
            this.Main = null;
        }

        /// <summary>
        /// Actual control point when mouse down logic interaction, implemented in derived class
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected virtual void OnControlPointMouseDown( object source, MouseButtonEventArgs e )
        {

        }

        /// <summary>
        /// Actual control point when mouse up logic interaction, implemented in derived class
        /// </summary>
        protected virtual void OnControlPointMouseUp()
        {

        }

        /// <summary>
        /// Actual control point when mouse move logic interaction, implemented in derived class
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected virtual void OnControlPointMouseMove( object source, MouseEventArgs e )
        {

        }

        #endregion

        #region Events - Knot vector move

        /// <summary>
        /// delegates a mouse move callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PointMouseMove( object sender, MouseEventArgs e )
        {
            this.OnControlPointMouseMove( sender, e );
        }

        /// <summary>
        /// delegates a mouse down callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PointMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            this.OnControlPointMouseDown( sender, e );
        }

        /// <summary>
        /// delegate a mouse up callbacks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void PointMouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            this.OnControlPointMouseUp();
        }

        #endregion
    }

    /// <summary>
    /// Extenstion method for <see cref="Bspline.Core.Location.Point3D"/>
    /// </summary>
    internal static class DrawingExtention
    {
        /// <summary>
        /// Convert to <see cref="Point"/> from a <see cref="Bspline.Core.Location.Point3D"/>
        /// </summary>
        /// <param name="source">the point to convert</param>
        /// <returns></returns>
        internal static Point To2DPoint( this Bspline.Core.Location.Point3D source )
        {
            return new Point( source.X, source.Y );
        }

        /// <summary>
        /// Convert to <see cref="Point"/> from a <see cref="Bspline.Core.Location.Point3D"/> using a offset
        /// </summary>
        /// <param name="source">the source point</param>
        /// <param name="offX">x offset</param>
        /// <param name="offY">y offset</param>
        /// <returns></returns>
        internal static Point To2DPoint( this Bspline.Core.Location.Point3D source, int offX, int offY )
        {
            return new Point( source.X + offX, source.Y + offY );
        }

        /// <summary>
        /// Convert to <see cref="Point3D"/> from a <see cref="Bspline.Core.Location.Point3D"/> using a offset
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width">conversion width</param>
        /// <param name="height">conversion height</param>
        /// <returns></returns>
        internal static Point3D To3DPoint( this Bspline.Core.Location.Point3D source, double width, double height )
        {
            Point3D result = new Point3D();
            result.X = ( ( 2 * source.X ) / width ) - 1;
            result.Y = ( ( 2 * source.Y ) / height ) - 1;
            result.Z = source.Z;
            return result;
        }
    }
}
