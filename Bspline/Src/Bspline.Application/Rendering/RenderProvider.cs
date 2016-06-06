using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bspline.Core;
using Bspline.Core.AlgorithmResults;
using BsplineKinect.Rendering.Factory;
using BsplineKinect.Rendering.Render;

namespace BsplineKinect.Rendering
{
    /// <summary>
    /// A helper class for <see cref="MainWindow"/> to request <see cref="RendererBase"/>
    /// </summary>
    internal class RenderProvider
    {
        #region Properties
        /// <summary>
        /// Property to hold <see cref="RenderFactory"/>
        /// </summary>
        private RenderFactory RenderFactory { get; set; }

        /// <summary>
        /// Property to hold an actual <see cref="RendererBase"/>
        /// </summary>
        internal RendererBase Renderer { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a helper class for <see cref="MainWindow"/> to request <see cref="RendererBase"/>
        /// </summary>
        /// <param name="renderFactory">a render factory to work with</param>
        /// <param name="defualtRender">a default render to work with</param>
        public RenderProvider(RenderFactory renderFactory,RenderMode defualtRender)
        {
            this.RenderFactory = renderFactory;
            this.Renderer = this.RenderFactory.CreateRender(defualtRender);
        }

        #endregion

        #region Internal

        /// <summary>
        /// Request the render to render <see cref="AlgResult"/>
        /// </summary>
        /// <param name="algResult"></param>
        internal void Render(AlgResult algResult)
        {
            this.Renderer.Render(algResult);
        }

        /// <summary>
        /// Request the render to clear its data
        /// </summary>
        internal void Clear()
        {
            this.Renderer.Reset();
        }


        /// <summary>
        /// Request this object to change <see cref="RendererBase"/>
        /// </summary>
        /// <param name="mode"></param>
        internal void Change(RenderMode mode)
        {
            this.Renderer.Dispose();
            this.Renderer = this.RenderFactory.CreateRender(mode);
        }

        #endregion
    }
}
