using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaddleSdk
{
    public class Utils
    {
        /// <summary>  
        /// 压缩到指定尺寸  
        /// </summary>  
        /// <param name="oldfile">原文件</param>  
        /// <param name="newfile">新文件</param>  
        public static Bitmap Compression(Bitmap bitmap, Size newSize)
        {
            try
            {
                ImageFormat thisFormat = bitmap.RawFormat;
                Bitmap outBmp = new Bitmap(newSize.Width, newSize.Height);
                Graphics g = Graphics.FromImage(outBmp);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bitmap, new Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);
                g.Dispose();
                return outBmp;
            }
            catch
            {
                return null;
            }
        }
    }
}
