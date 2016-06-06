using Bspline.Core.DataPacket;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using SysDrawImage = System.Drawing.Imaging;

namespace Bspline.WpfUI
{
    /// <summary>
    /// Helper class to process bitmaps in order to present live video
    /// </summary>
    internal class BitmapRender
    {
        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The pointer to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Transformes <see cref="Bspline.Core.DataPacket.ColorDataPacket"/> data into a <see cref="System.Drawing.Bitmap"/> asynchronously 
        /// </summary>
        /// <param name="packet">data to transform</param>
        /// <returns></returns>
        public async Task<Bitmap> Render(ColorDataPacket packet)
        {
            return await this.RenderAsync(packet);
        }

        /// <summary>
        /// helper method which uses <see cref="System.Threading.Tasks.Task"/> in order to process <see cref="Bspline.Core.DataPacket.ColorDataPacket"/> data into a <see cref="System.Drawing.Bitmap"/> asynchronously 
        /// </summary>
        /// <param name="packet">data to convert</param>
        /// <returns></returns>
        private async Task<Bitmap> RenderAsync(ColorDataPacket packet)
        {
            return await Task.Run(() =>
            {
                Bitmap bitmap = new Bitmap(packet.Width, packet.Height, SysDrawImage.PixelFormat.Format32bppRgb);

                BitmapData bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, packet.Width, packet.Height),
                    ImageLockMode.WriteOnly, bitmap.PixelFormat);

                IntPtr ptr = bitmapData.Scan0;
                Marshal.Copy(packet.PixelData, 0, ptr, packet.PixelData.Length);

                bitmap.UnlockBits(bitmapData);
                packet.Dispose();

                return bitmap;
            });
        }

        /// <summary>
        /// Converts <see cref="System.Drawing.Bitmap"/> to <see cref="System.Media.Imaging.BitmapSource"/> which can be read by the WPF
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public BitmapSource ToBitmapSource(Bitmap image)
        {
            IntPtr ptr = image.GetHbitmap(); //obtain the Hbitmap

            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ptr); //release the HBitmap
            return bs;
        }
    }
}
