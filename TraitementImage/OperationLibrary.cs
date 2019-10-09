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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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

        public static WriteableBitmap ConvertToSepia(ZoomBorder border, BitmapImage source, Int32Rect roi)
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

            if (roi.Height > 0 && roi.Width > 0)
            {
                roi = RoiToPixel(border, source, roi);
                wb.WritePixels(roi, grayPixels, widthInBytes, (roi.Y * source.PixelWidth) + roi.X);
            }
            else
                wb.WritePixels(new Int32Rect(0, 0, wb.PixelWidth, wb.PixelHeight), grayPixels, widthInBytes, 0);

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

        public static WriteableBitmap PaletteChange(ZoomBorder border, BitmapImage source, Int32Rect roi, System.Windows.Media.Color col)
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
                int green = (pixel & 0x0000FF00) >> 8;
                int blue = (pixel & 0x000000FF);

                red += col.R;
                green += col.G;
                blue += col.B;

                if (red > 255) red = 255;
                if (blue > 255) blue = 255;
                if (green > 255) green = 255;

                // assign the gray values keep the alpha
                unchecked
                {
                    grayPixels[x] = (int)((pixel & 0xFF000000) | (red << 16) | (green << 8) | blue);
                }
            }

            Console.WriteLine("fin palette change");

            wb.WritePixels(roi, grayPixels, widthInBytes, (roi.Y * source.PixelWidth) + roi.X);

            return wb;
        }

        public static float Lerp(float s, float e, float t)
        {
            return s + (e - s) * t;
        }

        public static float Blerp(float c00, float c10, float c01, float c11, float tx, float ty)
        {
            return Lerp(Lerp(c00, c10, tx), Lerp(c01, c11, tx), ty);
        }

        public static Bitmap Scale(Bitmap self, double scaleX, double scaleY)
        {
            int newWidth = (int)(self.Width * scaleX);
            int newHeight = (int)(self.Height * scaleY);
            Bitmap newImage = new Bitmap(newWidth, newHeight, self.PixelFormat);

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    float gx = ((float)x) / newWidth * (self.Width - 1);
                    float gy = ((float)y) / newHeight * (self.Height - 1);
                    int gxi = (int)gx;
                    int gyi = (int)gy;
                    System.Drawing.Color c00 = self.GetPixel(gxi, gyi);
                    System.Drawing.Color c10 = self.GetPixel(gxi + 1, gyi);
                    System.Drawing.Color c01 = self.GetPixel(gxi, gyi + 1);
                    System.Drawing.Color c11 = self.GetPixel(gxi + 1, gyi + 1);

                    int red = (int)Blerp(c00.R, c10.R, c01.R, c11.R, gx - gxi, gy - gyi);
                    int green = (int)Blerp(c00.G, c10.G, c01.G, c11.G, gx - gxi, gy - gyi);
                    int blue = (int)Blerp(c00.B, c10.B, c01.B, c11.B, gx - gxi, gy - gyi);
                    System.Drawing.Color rgb = System.Drawing.Color.FromArgb(red, green, blue);
                    newImage.SetPixel(x, y, rgb);
                }
            }

            return newImage;
        }

        public static Bitmap createHistogram(Bitmap bmp)
        {
            int[] histogram_r = new int[256];
            float max = 0;

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int Value = (bmp.GetPixel(i, j).R + bmp.GetPixel(i, j).G + bmp.GetPixel(i, j).B)/3;
                    histogram_r[Value]++;
                    if (max < histogram_r[Value])
                        max = histogram_r[Value];
                }
            }

            int histHeight = 128;
            Bitmap img = new Bitmap(256, histHeight + 10);
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < histogram_r.Length; i++)
                {
                    float pct = histogram_r[i] / max;   // What percentage of the max is this value?
                    g.DrawLine(Pens.Black,
                        new System.Drawing.Point(i, img.Height - 5),
                        new System.Drawing.Point(i, img.Height - 5 - (int)(pct * histHeight))  // Use that percentage of the height
                        );
                }
            }
            return img;
        }

        public static Bitmap Threshhold(int min, int max,int val, Bitmap bmp) //Les seuils doivent arriver triés
        {
            Bitmap ret = new Bitmap(bmp);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    int valeur = (bmp.GetPixel(i, j).R + bmp.GetPixel(i, j).G + bmp.GetPixel(i, j).B) / 3;
                    if(valeur<max && valeur>min)
                    {
                        ret.SetPixel(i, j, System.Drawing.Color.FromArgb(val, val, val));
                    }
                }
            }

            return ret;
        }

        public static Bitmap HistEq(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;

            Bitmap res = new Bitmap(img);

            int[] hist = new int[256];

            for(int i =0;i<w;i++)
            {
                for(int j=0;j<h;j++)
                {
                    hist[img.GetPixel(i, j).B]++;
                }
            }

            int[] cumulatedHistogram = new int[256];
            cumulatedHistogram[0] = hist[0];
            for(int i =1;i<256;i++)
            {
                cumulatedHistogram[i] = cumulatedHistogram[i - 1] + hist[i];
            }

            float[] arr = new float[256];
            for(int i =0;i<256;i++)
            {
                arr[i] = (float)((cumulatedHistogram[i] * 255.0) / (float)(w*h));
            }

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int nVal = (int)arr[img.GetPixel(i, j).B];
                    res.SetPixel(x, y, System.Drawing.Color.FromArgb(nVal, nVal, nVal));
                }
            }

            return res;
        }
    }
}
