using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TraitementImage
{
    class OperationLibrary
    {
        public static WriteableBitmap ConvertToGrayScale(ZoomBorder border, BitmapImage source, Int32Rect roi)
        {
            WriteableBitmap wb = new WriteableBitmap(source);

            int[] grayPixels = new int[wb.PixelWidth * wb.PixelHeight];
            int widthInBytes = 4 * wb.PixelWidth;
            wb.CopyPixels(grayPixels, widthInBytes, 0);

            //foreach (int x in index)
            for (int x = 0; x < wb.PixelWidth * wb.PixelHeight; x++)
            {
                // get the pixel
                int pixel = grayPixels[x];

                // get the component
                int red = (pixel & 0x00FF0000) >> 16;
                int blue = (pixel & 0x0000FF00) >> 8;
                int green = (pixel & 0x000000FF);

                // get the average
                int average = (byte)((red + blue + green) / 3);

                // assign the gray values keep the alpha
                unchecked
                {
                    grayPixels[x] = (int)((pixel & 0xFF000000) | (average << 16) | (average << 8) | average);
                }
            }

            if (roi.Height > 0 && roi.Width > 0)
            {
                roi = RoiToPixel(border, source, roi);
                wb.WritePixels(roi, grayPixels, widthInBytes, (roi.Y * source.PixelWidth) + roi.X);
            }
            else
                wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), grayPixels, widthInBytes, 0);

            return wb;
        }

        public static WriteableBitmap ConvertToSepia(BitmapImage source, Int32Rect roi)
        {
            WriteableBitmap wb = new WriteableBitmap(source);               // create the WritableBitmap using the source

            int[] grayPixels = new int[wb.PixelWidth * wb.PixelHeight];
            int widthInBytes = 4 * wb.PixelWidth;
            wb.CopyPixels(grayPixels, widthInBytes, 0); 

            // lets use the average algo
            //foreach(int x in index)
            for (int x = 0; x < wb.PixelWidth * wb.PixelHeight; x++)
            {
                // get the pixel
                int pixel = grayPixels[x];

                // get the component
                int red = (pixel & 0x00FF0000) >> 16;
                int blue = (pixel & 0x0000FF00) >> 8;
                int green = (pixel & 0x000000FF);

                // get the average

                int nred = (byte)(red * 0.393 + green * 0.769 + blue * 0.189);
                int ngreen = (byte)(red * 0.349 + green * 0.686 + blue * 0.168);
                int nblue = (byte)(red * 0.272 + green * 0.534 + blue * 0.131);
                if (nred > 255) nred = 255;
                if (nblue > 255) nblue = 255;
                if (ngreen > 255) ngreen = 255;

                // assign the gray values keep the alpha
                unchecked
                {
                    grayPixels[x] = (int)((pixel & 0xFF000000) | (nred << 16) | (ngreen << 8) | nblue);
                }
            }

            return wb;
        }

        public static Int32Rect RoiToPixel(ZoomBorder border, BitmapImage source, Int32Rect roi)
        {
            if (roi == null || roi.Height == 0 || roi.Width == 0)
                return new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight);

            var st = border.GetScaleTransform(border.child);
            var tt = border.GetTranslateTransform(border.child);

            roi.Height = (int)((roi.Height * (source.Height / border.Height)) / st.ScaleY);
            roi.Width = (int)((roi.Width * (source.Width / border.Width)) / st.ScaleX);

            roi.X = (int)((-tt.X + roi.X) * (source.Width / border.Width) / st.ScaleX);
            roi.Y = (int)((-tt.Y + roi.Y) * (source.Height / border.Height) / st.ScaleY);

            return roi;
        }

        public static WriteableBitmap Trim(ZoomBorder border, BitmapImage source, Int32Rect roi)
        {
            WriteableBitmap wb = new WriteableBitmap(source);

            roi = RoiToPixel(border, source, roi);
            int[] newBitmap = new int[wb.PixelWidth * wb.PixelHeight];
            int widthInBytes = 4 * wb.PixelWidth;

            wb.CopyPixels(roi, newBitmap, widthInBytes, 0);

            WriteableBitmap nb = new WriteableBitmap(roi.Width, roi.Height, wb.DpiX, wb.DpiY, wb.Format, wb.Palette);
            nb.WritePixels(new Int32Rect(0, 0, nb.PixelWidth, nb.PixelHeight), newBitmap, widthInBytes, 0);

            return nb;
        }

        public static WriteableBitmap PaletteChange(ZoomBorder border, BitmapImage source, Int32Rect roi, Color col)
        {
            WriteableBitmap wb = new WriteableBitmap(source);
            roi = RoiToPixel(border, source, roi);
            Console.WriteLine("début palette change");

            int[] grayPixels = new int[wb.PixelWidth * wb.PixelHeight];
            int widthInBytes = 4 * wb.PixelWidth;
            wb.CopyPixels(grayPixels, widthInBytes, 0);


            for (int x = 0; x < wb.PixelWidth * wb.PixelHeight; x++)
            {
                // get the pixel
                int pixel = grayPixels[x];

                // get the component
                int red = (pixel & 0x00FF0000) >> 16;
                int blue = (pixel & 0x0000FF00) >> 8;
                int green = (pixel & 0x000000FF);

                red += col.R;
                green += col.G;
                blue += col.B;

                if (red > 255) red = 255;
                if (blue > 255) blue = 255;
                if (green > 255) green = 255;

                // assign the gray values keep the alpha
                unchecked
                {
                    grayPixels[x] = (int)((pixel & 0xFF000000) | (red << 16) | (blue << 8) | green);
                }
            }

            Console.WriteLine("fin palette change");

            wb.WritePixels(roi, grayPixels, widthInBytes, (roi.Y * source.PixelWidth) + roi.X);

            return wb;
        }
    }
}
