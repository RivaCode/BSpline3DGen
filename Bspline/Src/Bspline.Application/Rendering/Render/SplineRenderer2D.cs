using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Bspline.Core.Location;

namespace BsplineKinect.Rendering.Render
{
    /// <summary>
    /// Derived class to render <see cref="MainWindow"/> according to 2D
    /// </summary>
    internal class SplineRenderer2D : RendererBase
    {
        #region Fields

        protected const double MARKER_SIZE = 10;

        /// <summary>
        /// Fields to hold the last control points X and Y
        /// </summary>
        private double _lastX, _lastY;
        /// <summary>
        /// Field to hold the info of show typical polygon
        /// </summary>
        private bool _showTypicalPolygon;
        /// <summary>
        /// Field to hold the stroke <see cref="Brush"/>
        /// </summary>
        private Brush _strokeBrush;
        /// <summary>
        /// Field to hold the Fill <see cref="Brush"/>
        /// </summary>
        private Brush _fillBrush;
        /// <summary>
        /// Field to hold the stroke size
        /// </summary>
        private int _strokeSize;

        #endregion

        #region Properties

        /// <summary>
        /// Property to hold the start <see cref="Point"/> of control point
        /// </summary>
        private Point StartPoint { get; set; }

        /// <summary>
        /// Property to hold the last <see cref="Point"/> of control point
        /// </summary>
        private Point LastPoint { get; set; }

        /// <summary>
        /// Property to hold the <see cref="Path"/> of our drawing
        /// </summary>
        private Path SplinePath { get; set; }

        #region Editing Properties
        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        internal override Brush FillBrush
        {
            get { return this._fillBrush; }
            set
            {
                this._fillBrush = value;
                this.SplinePath.Fill = this.UseFill ? this._fillBrush : null;
            }
        }

        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        internal override Brush StrokeBrush
        {
            get { return _strokeBrush; }
            set
            {
                _strokeBrush = value;
                this.SplinePath.Stroke = this._strokeBrush;
            }
        }
        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        internal override int StrokeSize
        {
            get { return this._strokeSize; }
            set
            {
                _strokeSize = value;
                this.SplinePath.StrokeThickness = this._strokeSize;
            }
        }
        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        internal override bool ShowTypicalPolygon
        {
            get { return _showTypicalPolygon; }
            set
            {
                this._showTypicalPolygon = value;
                int thicknes = this._showTypicalPolygon ? 1 : 0;
                foreach ( Line line in this.Main.Paint2D.Children.OfType<Line>() )
                {
                    line.StrokeThickness = thicknes;
                }
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the derived class to render <see cref="MainWindow"/> according to 2D
        /// </summary>
        /// <param name="mainWindow"></param>
        public SplineRenderer2D( MainWindow mainWindow )
            : base( mainWindow )
        {
        }

        #endregion

        #region Protected

        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected sealed override void RenderOverride()
        {
            this.Main.Paint2D.Visibility = Visibility.Visible;
            IEnumerable<PathFigure> figures = this.PreparePathFigures();
            PathGeometry geometry = new PathGeometry( figures ) { FillRule = FillRule.Nonzero };

            this.SplinePath = new Path
            {
                Data = geometry,
                Fill = this.UseFill ? this.FillBrush : null,
                Stroke = this.StrokeBrush,
                StrokeThickness = this.StrokeSize
            };

            this.Main.Paint2D.Children.Add( this.SplinePath );

            this.DrawControlShapesOverride();
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void ResetOverride()
        {
            this.Main.Paint2D.Visibility= Visibility.Hidden;
            this.Clear();
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void Clear()
        {
            foreach ( var chiledRec in this.Main.Paint2D.Children.OfType<Rectangle>() )
            {
                chiledRec.MouseMove -= PointMouseMove;
                chiledRec.MouseLeftButtonDown -= PointMouseLeftButtonDown;
                chiledRec.MouseLeftButtonUp -= PointMouseLeftButtonUp;
            }

            this.Main.Paint2D.Children.Clear();
        }

        /// <summary>
        /// Draw our control points into our <see cref="SplinePath"/>
        /// </summary>
        protected virtual void DrawControlShapesOverride()
        {

            List<Point> typicalPolygonPoints = this.AlgInfoResult.KVector.Select( p3D => p3D.To2DPoint() ).ToList();

            /*************************/

            List<Line> createdLines = new List<Line>();
            for ( int i = 0; i < typicalPolygonPoints.Count - 1; i++ )
            {
                Line line = new Line
                {
                    X1 = typicalPolygonPoints[i].X,
                    X2 = typicalPolygonPoints[i + 1].X,
                    Y1 = typicalPolygonPoints[i].Y,
                    Y2 = typicalPolygonPoints[i + 1].Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = this.ShowTypicalPolygon ? 1 : 0
                };
                createdLines.Add( line );
                this.Main.Paint2D.Children.Add(line);
            }


            /*************************/

            List<Rectangle> createdRectangle = new List<Rectangle>();
            foreach ( Point point in typicalPolygonPoints )
            {
                Rectangle rect = new Rectangle
                {
                    Stroke = Brushes.Black,
                    Fill = Brushes.Black,
                    Height = MARKER_SIZE,
                    Width = MARKER_SIZE
                };

                rect.MouseMove += PointMouseMove;
                rect.MouseLeftButtonDown += PointMouseLeftButtonDown;
                rect.MouseLeftButtonUp += PointMouseLeftButtonUp;

                Canvas.SetLeft( rect, point.X - MARKER_SIZE / 2 );
                Canvas.SetTop( rect, point.Y - MARKER_SIZE / 2 );

                this.Main.Paint2D.Children.Add( rect );
                createdRectangle.Add( rect );
            }

            /*************************/
            /* Connect points with lines  */
            /*************************/

            for ( int i = 1; i < createdRectangle.Count - 1; i++ )
            {
                createdRectangle[i].Tag = new ShapeDegree
                {
                    In = new List<Line> { createdLines[i - 1] },
                    Out = new List<Line> { createdLines[i] }
                };
            }

            /*************************/
            /* Add line to first point      */
            /*************************/

            createdRectangle[0].Tag = new ShapeDegree
            {
                Out = new List<Line> { createdLines[0] }
            };

            /*************************/
            /* Add line to last point       */
            /*************************/

            createdRectangle[createdRectangle.Count - 1].Tag = new ShapeDegree
            {
                In = new List<Line> { createdLines[createdLines.Count - 1] }
            };
        }

        /// <summary>
        /// Preparese our curve inside a collection of <see cref="PathFigure"/>
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<PathFigure> PreparePathFigures()
        {
            Point[] points = this.AlgInfoResult.AdjustedKMatrix[0].Select( p3D => p3D.To2DPoint() ).ToArray();
            Point[] cp1 = this.AlgInfoResult.StartCurveMatrix[0].Select( p3D => p3D.To2DPoint() ).ToArray();
            Point[] cp2 = this.AlgInfoResult.EndCurveMatrix[0].Select( p3D => p3D.To2DPoint() ).ToArray();

            PathSegmentCollection lines = new PathSegmentCollection();
            for ( int i = 0; i < cp1.Length; ++i )
            {
                lines.Add( new BezierSegment( cp1[i], cp2[i], points[i + 1], true ) );
            }

            PathFigure f = new PathFigure( points[0], lines, false );

            return new[] { f };
        }

        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnControlPointMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle source = sender as Rectangle;
            Mouse.Capture( source );
            this._lastX = Canvas.GetLeft( source );
            this._lastY = Canvas.GetTop( source );
            this.StartPoint = new Point( this._lastX + MARKER_SIZE / 2, this._lastY + MARKER_SIZE / 2 );
            this.LastPoint = e.GetPosition(this.Main.Paint2D);
        }

        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void OnControlPointMouseUp()
        {
            Mouse.Capture( null );
            this.Main.Mediator.UpdatePoints( new Point3D( StartPoint.X, StartPoint.Y, 0 ), new Point3D( _lastX, _lastY, 0 ) );
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void OnControlPointMouseMove( object sender, MouseEventArgs e )
        {
            Rectangle source = sender as Rectangle;
            if (source.Equals(Mouse.Captured) )
            {
                double x = e.GetPosition(Main.Paint2D).X;
                double y = e.GetPosition(Main.Paint2D).Y;

                this._lastX += x - this.LastPoint.X;
                Canvas.SetLeft( source, this._lastX );
                this._lastY += y - this.LastPoint.Y;
                Canvas.SetTop( source, this._lastY );
                this.LastPoint = new Point( x, y );

                ShapeDegree shapeDegree = source.Tag as ShapeDegree;
                if ( shapeDegree.Out != null )
                {

                    shapeDegree.Out.ForEach( line =>
                    {
                        line.X1 = this._lastX;
                        line.Y1 = this._lastY;
                    } );
                }

                if ( shapeDegree.In != null )
                {

                    shapeDegree.In.ForEach( line =>
                    {
                        line.X2 = this._lastX;
                        line.Y2 = this._lastY;
                    } );
                }

                this.FixLastXY();
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Fix last X and Y according to screen resolution
        /// </summary>
        private void FixLastXY()
        {
            const int xBorder = 10;
            if ( this._lastX < 0 )
            {
                this._lastX = 0;
            }
            else if ( this._lastX > this.Main.Paint2D.ActualWidth - xBorder )
            {
                this._lastX = this.Main.Paint2D.ActualWidth - xBorder;
            }

            const int yBorder = 10;
            if ( this._lastY < yBorder )
            {
                this._lastY = yBorder;
            }
            else if (this._lastY > this.Main.Paint2D.ActualHeight - yBorder)
            {
                this._lastY = this.Main.Paint2D.ActualHeight - yBorder;
            }
        }

        #endregion

        #region Nested class
        /// <summary>
        /// A helper class to act as direct-graph object
        /// </summary>
        protected class ShapeDegree
        {
            /// <summary>
            /// Property to hold to lines which come out from <see cref="Shape"/>
            /// </summary>
            public List<Line> Out { get; set; }

            /// <summary>
            /// Property to hold to lines which come in to <see cref="Shape"/>
            /// </summary>
            public List<Line> In { get; set; }
        }
        #endregion
    }
}
