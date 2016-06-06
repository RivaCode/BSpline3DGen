using System;
using Bspline.Core;
using BsplineKinect.Rendering.Render;

namespace BsplineKinect.Rendering.Factory
{
    /// <summary>
    /// Helper class to provide an a <see cref="RendererBase"/>
    /// </summary>
    internal class RenderFactory : DisposableObject
    {
        /// <summary>
        /// Property to hold the main window which the <see cref="RendererBase"/> will render
        /// </summary>
        private MainWindow MainWindow { get; set; }

        /// <summary>
        /// Construcotor of helper class to provide an a <see cref="RendererBase"/>
        /// </summary>
        /// <param name="mainWindow">application main window</param>
        public RenderFactory( MainWindow mainWindow )
        {
            MainWindow = mainWindow;
        }

        #region Internal

        /// <summary>
        /// Create a <see cref="RendererBase"/> instance according to <see cref="RenderMode"/>
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        internal RendererBase CreateRender( RenderMode mode )
        {
            RendererBase rendererBase = null;

            switch ( mode )
            {
                case RenderMode.Regular2D:
                    rendererBase = this.Create2DRenderer();
                    break;
                case RenderMode.Advanced2D:
                    rendererBase = this.CreateAdvanced2DRenderer();
                    break;
                case RenderMode.Surface3D:
                    rendererBase = this.Create3DRender();
                    break;
                default:
                    throw new ArgumentOutOfRangeException( "mode" );
            }

            return rendererBase;
        }

        #endregion

        #region Protected
        /// <summary>
        /// <see cref="DisposableObject"/>
        /// </summary>
        protected override void DisposeInner()
        {
            this.MainWindow = null;
        }

        #endregion

        #region Private

        /// <summary>
        /// Create a <see cref="SplineRenderer2D"/>
        /// </summary>
        /// <returns></returns>
        private RendererBase Create2DRenderer()
        {
            return new SplineRenderer2D( this.MainWindow );
        }

        /// <summary>
        /// Create a <see cref="SplineRenderer2DAdvanced"/>
        /// </summary>
        /// <returns></returns>
        private RendererBase CreateAdvanced2DRenderer()
        {
            return new SplineRenderer2DAdvanced( this.MainWindow );
        }

        /// <summary>
        /// Create a <see cref="SurfaceRenderer3D"/>
        /// </summary>
        /// <returns></returns>
        private RendererBase Create3DRender()
        {
            return new SurfaceRenderer3D( this.MainWindow );
        }

        #endregion
    }
}
