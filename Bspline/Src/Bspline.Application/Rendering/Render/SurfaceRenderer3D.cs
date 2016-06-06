using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Point3D = Bspline.Core.Location.Point3D;

namespace BsplineKinect.Rendering.Render
{

    /// <summary>
    /// Derived class to render <see cref="MainWindow"/> according to 3D
    /// </summary>
    internal class SurfaceRenderer3D : RendererBase
    {
        /// <summary>
        /// Field to hold the 3D geomerty model <see cref="GeometryModel3D"/>
        /// </summary>
        private GeometryModel3D _geometry;
        /// <summary>
        /// Field to hold last position location of surface
        /// </summary>
        private Point _lastPos;

        /// <summary>
        /// Property to hold the screen width factor
        /// </summary>
        private double WidthFactor { get; set; }
        /// <summary>
        /// Property to hold the screen height factor
        /// </summary>
        private double HeightFactor { get; set; }

        #region Constructor

        /// <summary>
        /// Constructor of derived class to render <see cref="MainWindow"/> according to 3D
        /// </summary>
        /// <param name="mainWindow"></param>
        public SurfaceRenderer3D( MainWindow mainWindow )
            : base( mainWindow )
        {

            this.WidthFactor = this.Main.PaintControl.ActualWidth;
            this.HeightFactor = this.Main.PaintControl.ActualHeight;

            this.Main.Paint2D.Visibility = Visibility.Hidden;
            this.Main.Paint3D.Visibility = Visibility.Visible;
        }

        #endregion

        #region Protected

        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void RenderOverride()
        {
            this.Main.Paint3D.Visibility = Visibility.Visible;
            this.Main.Paint3D.MouseDown += PointMouseLeftButtonDown;
            this.Main.Paint3D.MouseMove += PointMouseMove;
            this.Main.Paint3D.MouseUp += PointMouseLeftButtonUp;

            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();

            for ( int j = 0; j < this.AlgInfoResult.KMatrix.Columns; j++ )
            {
                this.AlgInfoResult.KMatrix[0, j].Z = 1;
                this.AlgInfoResult.KMatrix[1, j].Z = 0.3;
                this.AlgInfoResult.KMatrix[2, j].Z = -0.3;
                this.AlgInfoResult.KMatrix[3, j].Z = -1;
            }


            for ( int i = 0; i < this.AlgInfoResult.KMatrix.Columns; i++ )
            {
                for ( int j = 0; j < this.AlgInfoResult.KMatrix.Rows; j++ )
                {
                    Point3D point = this.AlgInfoResult.KMatrix[j,i];
                    mesh.Positions.Add( point.To3DPoint( this.WidthFactor, this.HeightFactor ) );

                    //mesh.Normals.Add( new Vector3D( 0, 0, 1 ) );
                }
            }
            for ( int i = 0; i < mesh.Positions.Count; i += 4 )
            {
                mesh.TriangleIndices.Add( i );
                mesh.TriangleIndices.Add( i + 1 );
                mesh.TriangleIndices.Add( i + 4 );

                mesh.TriangleIndices.Add( i + 1 );
                mesh.TriangleIndices.Add( i + 5 );
                mesh.TriangleIndices.Add( i + 4 );

                mesh.TriangleIndices.Add( i + 1 );
                mesh.TriangleIndices.Add( i + 2 );
                mesh.TriangleIndices.Add( i + 5 );

                mesh.TriangleIndices.Add( i + 2 );
                mesh.TriangleIndices.Add( i + 6 );
                mesh.TriangleIndices.Add( i + 5 );

                mesh.TriangleIndices.Add( i + 2 );
                mesh.TriangleIndices.Add( i + 3 );
                mesh.TriangleIndices.Add( i + 6 );

                mesh.TriangleIndices.Add( i + 3 );
                mesh.TriangleIndices.Add( i + 7 );
                mesh.TriangleIndices.Add( i + 6 );
            }


            _geometry = new GeometryModel3D( mesh, new DiffuseMaterial( Brushes.YellowGreen ) );
       
            _geometry.Transform = new Transform3DGroup();
            this.Main.Group.Children.Add( _geometry );
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void ResetOverride()
        {
            this.Main.Paint3D.Visibility = Visibility.Hidden;
          this.Clear();
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void Clear()
        {
            this.Main.Paint3D.MouseDown -= PointMouseLeftButtonDown;
            this.Main.Paint3D.MouseMove -= PointMouseMove;
            this.Main.Paint3D.MouseUp -= PointMouseLeftButtonUp;
            this.Main.Group.Children.Clear();
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void OnControlPointMouseUp()
        {
            Mouse.Capture(null);
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void OnControlPointMouseDown(object source, MouseButtonEventArgs e)
        {
              if ( e.LeftButton != MouseButtonState.Pressed )
                return;
            Mouse.Capture(this.Main.Paint3D);
            Point pos = Mouse.GetPosition(this.Main.Paint3D);
            _lastPos = new Point(pos.X - this.Main.Paint3D.ActualWidth / 2, this.Main.Paint3D.ActualHeight / 2 - pos.Y);
        }


        /// <summary>
        /// <see cref="RendererBase"/>
        /// </summary>
        protected override void OnControlPointMouseMove(object source, MouseEventArgs e)
        {
            if (this.Main.Paint3D.Equals(Mouse.Captured))
            {
                Point pos = Mouse.GetPosition(this.Main.Paint3D);
                Point actualPos = new Point(pos.X - this.Main.Paint3D.ActualWidth / 2, this.Main.Paint3D.ActualHeight / 2 - pos.Y);
                double dx = actualPos.X - _lastPos.X, dy = actualPos.Y - _lastPos.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0)
                        mouseAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0)
                        mouseAngle += Math.PI;
                    else if (dx > 0 && dy < 0)
                        mouseAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0)
                    mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                else if (dx != 0 && dy == 0)
                    mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                Transform3DGroup group = _geometry.Transform as Transform3DGroup;
                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
                group.Children.Add(new RotateTransform3D(r));

                _lastPos = actualPos;
            }
        }

        #endregion
    }
}
