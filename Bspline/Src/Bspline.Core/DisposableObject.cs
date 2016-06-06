using System;

namespace Bspline.Core
{
    public class DisposableObject : IDisposable
    {

        /// <summary>
        /// Class that give dispose option to clean the memory before end of use
        /// </summary>
        #region Properties

        protected bool IsDisposed { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// clean the memory and remove the 
        /// <seealso cref="DisposableObject"/>
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose( false );
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// maintaine the members for dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        #endregion

        #region Protected
        /// <summary>
        /// get result if clean or not
        /// </summary>
        /// <param name="dispose"></param>
        protected void Dispose( bool dispose )
        {
            if ( !dispose || this.IsDisposed )
            {
                return;
            }

            this.DisposeInner();
            this.IsDisposed = true;
        }

        protected virtual void DisposeInner()
        {

        }

        #endregion
    }
}
