using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Bspline.Core.AlgorithmResults;

namespace BsplineKinect.Rendering.Render
{

    /// <summary>
    /// Derived class to render <see cref="MainWindow"/> according to 2.5D
    /// </summary>
    internal class SplineRenderer2DAdvanced : SplineRenderer2D
    {
        /// <summary>
        /// Constructor of the derived class to render <see cref="MainWindow"/> according to 2D
        /// </summary>
        /// <param name="mainWindow"></param>
        public SplineRenderer2DAdvanced( MainWindow mainWindow )
            : base( mainWindow )
        {
        }

        /// <summary>
        /// <see cref="SplineRenderer2D"/>
        /// </summary>
        protected override void DrawControlShapesOverride()
        {
            Matrix<Rectangle> rectMatrix = new Matrix<Rectangle>();
            Matrix<Line> lineMatrix = new Matrix<Line>();

            for ( int i = 0; i < this.AlgInfoResult.KMatrix.Columns; i++ )
            {
                for ( int j = 0; j < this.AlgInfoResult.KMatrix.Rows; j++ )
                {
                    Rectangle rect = new Rectangle
                    {
                        Stroke = Brushes.Black,
                        Fill = Brushes.Black,
                        Height = MARKER_SIZE,
                        Width = MARKER_SIZE,
                        Tag = new ShapeDegree { In = new List<Line>(), Out = new List<Line>() }
                    };

                    rect.MouseMove += PointMouseMove;
                    rect.MouseLeftButtonDown += PointMouseLeftButtonDown;
                    rect.MouseLeftButtonUp += PointMouseLeftButtonUp;

                    Canvas.SetLeft( rect, this.AlgInfoResult.KMatrix[j, i].X - MARKER_SIZE / 2 );
                    Canvas.SetTop( rect, this.AlgInfoResult.KMatrix[j, i].Y - MARKER_SIZE / 2 );
                    rectMatrix[j].Add( rect );
                }
            }


            for ( int j = 0; j < this.AlgInfoResult.KMatrix.Rows - 1; j++ )
            {

                for ( int i = 0; i < this.AlgInfoResult.KMatrix.Columns - 1; i++ )
                {
                    Line line = new Line
                    {
                        X1 = this.AlgInfoResult.KMatrix[j, i].X,
                        X2 = this.AlgInfoResult.KMatrix[j, i + 1].X,
                        Y1 = this.AlgInfoResult.KMatrix[j, i].Y,
                        Y2 = this.AlgInfoResult.KMatrix[j, i + 1].Y,
                        Stroke = Brushes.Black,
                        StrokeThickness = this.ShowTypicalPolygon ? 0.5 : 0
                    };
                    Line line2 = new Line
                     {
                         X1 = this.AlgInfoResult.KMatrix[j, i].X,
                         X2 = this.AlgInfoResult.KMatrix[j + 1, i].X,
                         Y1 = this.AlgInfoResult.KMatrix[j, i].Y,
                         Y2 = this.AlgInfoResult.KMatrix[j + 1, i].Y,
                         Stroke = Brushes.Black,
                         StrokeThickness = this.ShowTypicalPolygon ? 0.5 : 0
                     };

                    ShapeDegree degree = rectMatrix[j, i].Tag as ShapeDegree;
                    degree.Out.Add( line );
                    degree.Out.Add( line2 );

                    degree = rectMatrix[j, i + 1].Tag as ShapeDegree;
                    degree.In.Add( line );

                    degree = rectMatrix[j + 1, i].Tag as ShapeDegree;
                    degree.In.Add( line2 );

                    this.Main.Paint2D.Children.Add( line );
                    this.Main.Paint2D.Children.Add( line2 );

                }

            }
            for ( int i = 0; i < this.AlgInfoResult.KMatrix.Rows - 1; i++ )
            {
                Line line = new Line
                {
                    X1 = this.AlgInfoResult.KMatrix[i, this.AlgInfoResult.KMatrix.Columns - 1].X,
                    X2 = this.AlgInfoResult.KMatrix[i + 1, this.AlgInfoResult.KMatrix.Columns - 1].X,
                    Y1 = this.AlgInfoResult.KMatrix[i, this.AlgInfoResult.KMatrix.Columns - 1].Y,
                    Y2 = this.AlgInfoResult.KMatrix[i + 1, this.AlgInfoResult.KMatrix.Columns - 1].Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = this.ShowTypicalPolygon ? 0.5 : 0
                };
                this.Main.Paint2D.Children.Add( line );
                ShapeDegree degree = rectMatrix[i, this.AlgInfoResult.KMatrix.Columns - 1].Tag as ShapeDegree;
                degree.Out.Add( line );
                degree = rectMatrix[i + 1, this.AlgInfoResult.KMatrix.Columns - 1].Tag as ShapeDegree;
                degree.In.Add( line );
            }
            for ( int i = 0; i < this.AlgInfoResult.KMatrix.Columns - 1; i++ )
            {
                Line line = new Line
                {
                    X1 = this.AlgInfoResult.KMatrix[this.AlgInfoResult.KMatrix.Rows - 1, i].X,
                    X2 = this.AlgInfoResult.KMatrix[this.AlgInfoResult.KMatrix.Rows - 1, i + 1].X,
                    Y1 = this.AlgInfoResult.KMatrix[this.AlgInfoResult.KMatrix.Rows - 1, i].Y,
                    Y2 = this.AlgInfoResult.KMatrix[this.AlgInfoResult.KMatrix.Rows - 1, i + 1].Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = this.ShowTypicalPolygon ? 0.5 : 0
                };
                this.Main.Paint2D.Children.Add( line );
                ShapeDegree degree = rectMatrix[this.AlgInfoResult.KMatrix.Rows - 1, i].Tag as ShapeDegree;
                degree.Out.Add( line );
                degree = rectMatrix[this.AlgInfoResult.KMatrix.Rows - 1, i + 1].Tag as ShapeDegree;
                degree.In.Add( line );
            }

            foreach ( Rectangle rectangle in rectMatrix.SelectMany( pair => pair.Value ) )
            {
                this.Main.Paint2D.Children.Add( rectangle );
            }

        }

        /// <summary>
        /// <see cref="SplineRenderer2D"/>
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<PathFigure> PreparePathFigures()
        {
            List<PathFigure> figures = new List<PathFigure>();

            for ( int i = 0; i < this.AlgInfoResult.KMatrix.Rows; i++ )
            {
                Point[] points = this.AlgInfoResult.AdjustedKMatrix[i].Select( p3D => p3D.To2DPoint() ).ToArray();

                Point[] cp1 = this.AlgInfoResult.StartCurveMatrix[i].Select( p3D => p3D.To2DPoint() ).ToArray();
                Point[] cp2 = this.AlgInfoResult.EndCurveMatrix[i].Select( p3D => p3D.To2DPoint() ).ToArray();

                PathSegmentCollection lines = new PathSegmentCollection();
                for ( int pointsIndex = 0; pointsIndex < cp1.Length; ++pointsIndex )
                {
                    lines.Add( new BezierSegment( cp1[pointsIndex], cp2[pointsIndex], points[pointsIndex + 1], true ) );
                }

                figures.Add( new PathFigure( points[0], lines, false ) );
            }
            
            for (int i = 0; i < this.AlgInfoResult.AdjustedKMatrix.Columns; i++)
            {
                List<Point> column =new List<Point>();
                for (int j = 0; j < this.AlgInfoResult.AdjustedKMatrix.Rows; j++)
                {
                   column.Add(this.AlgInfoResult.AdjustedKMatrix[j,i].To2DPoint()); 
                }

                PathSegmentCollection innerLines = new PathSegmentCollection();
                foreach ( Point t in column )
                {
                    innerLines.Add( new LineSegment( t, true ) );
                }

                figures.Add( new PathFigure( column[0], innerLines, false ) );
            }

            return figures;
        }
    }
}
