namespace Bspline.Core.DataPacket
{
    public class ColorDataPacket : DisposableObject
    {
        /// <summary>
        /// Class that paint each pixel to some color
        /// </summary>
        #region Properties

        public byte[] PixelData { get; private set; }
        /// <summary>
        /// propery that hold Width 
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// property that hold the heigth
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// property that hold stride
        /// </summary>
        public int Stride { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// get array of pixels they width higth and generate the stride
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bytePerPixel"></param>
        public ColorDataPacket( byte[] pixels, int width, int height, int bytePerPixel )
        {
            this.PixelData = pixels;
            this.Width = width;
            this.Height = height;
            this.Stride = this.Width * bytePerPixel;
        }

        #endregion

        #region Protected
        /// <summary>
        /// <seealso cref="DisposableObject.cs"/>
        /// </summary>
        protected override void DisposeInner()
        {
            this.PixelData = null;
        }

        #endregion
    }
}
