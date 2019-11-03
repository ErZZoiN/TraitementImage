using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Controls.Primitives;
using System.Drawing;
using System.Drawing.Imaging;
using Watershed;

namespace TraitementImage
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _workspace;
        private int tt;
        private int histonoff = 1;
        private WriteableBitmap _bitmapresult;
        private WriteableBitmap _bitmapwork;
        private Int32Rect _rect;
        private System.Windows.Shapes.Rectangle _roi;
        public static string defdir = "C:\\Users\\mrpar_000\\source\\repos\\TraitementImage\\TraitementImage\\workspace";

        public MainWindow()
        {
            Workspace = defdir;
            InitializeComponent();
            borderWork.SendRoi += BorderWork_SendRoi;
            borderWork.ResetRoi += BorderWork_resetRoi;
            Roi = new System.Windows.Shapes.Rectangle();
            canvaWork.Children.Add(Roi);
            Roi.Fill = new SolidColorBrush() { Color = Colors.Red, Opacity = 0.30f };
        }

        private void BorderWork_resetRoi()
        {
            Roi.Height = 0;
            Roi.Width = 0;

            Rect = new Int32Rect(0, 0, 0, 0);        }

        private void BorderWork_SendRoi(Int32Rect roi)
        {
            if(roi.Height<0)
            {
                roi.Height = -roi.Height;
                roi.Y = roi.Y - roi.Height;
            }
            if(roi.Width<0)
            {
                roi.Width = -roi.Width;
                roi.X = roi.X - roi.Width;
            }
            Rect = new Int32Rect()
            {
                X = roi.X,
                Y = roi.Y,
                Height = roi.Height,
                Width = roi.Width
            };
            Roi.Height = roi.Height;
            Roi.Width = roi.Width;
            Canvas.SetLeft(Roi, roi.X);
            Canvas.SetTop(Roi, roi.Y);
        }

        public string Workspace { get => _workspace; set => _workspace = value; }
        public WriteableBitmap BitmapResult { get => _bitmapresult; set => _bitmapresult = value; }
        public WriteableBitmap BitmapWork { get => _bitmapwork; set => _bitmapwork = value; }
        public System.Windows.Shapes.Rectangle Roi { get => _roi; set => _roi = value; }
        public Int32Rect Rect { get => _rect; set => _rect = value; }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                InitialDirectory = Workspace
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string SelectedFile = dialog.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(SelectedFile);
                bitmap.EndInit();
                BitmapWork = new WriteableBitmap(bitmap);
                BitmapResult = new WriteableBitmap(bitmap);
                workImage.Source = BitmapWork;
                resultImage.Source = BitmapResult;

                workHistogram.Source = ToBitmapImage(OperationLibrary.createHistogram(BitmapImage2Bitmap(bitmap)));
                resultHistogram.Source = ToBitmapImage(OperationLibrary.createHistogram(BitmapImage2Bitmap(bitmap)));
            }
        }

        private void ResultToWork_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage tmp = ConvertWriteableBitmapToBitmapImage(BitmapResult);
            BitmapWork = new WriteableBitmap(tmp);
            applyChange();
        }


        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void BlackAndWhite_Click(object sender, RoutedEventArgs e)
        {
            BitmapResult = OperationLibrary.ConvertToGrayScale(borderWork, ConvertWriteableBitmapToBitmapImage(BitmapWork), Rect);
            applyChange();
        }

        private void Sepia_Click(object sender, RoutedEventArgs e)
        {
            BitmapResult = OperationLibrary.ConvertToSepia(borderWork, ConvertWriteableBitmapToBitmapImage(BitmapWork), Rect);
            applyChange();
        }

        private void Clip_Click(object sender, RoutedEventArgs e)
        {
            BitmapResult = OperationLibrary.Trim(borderWork, ConvertWriteableBitmapToBitmapImage(BitmapWork), Rect);
            applyChange();
        }

        private void Palette_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            BitmapResult = OperationLibrary.PaletteChange(borderWork, ConvertWriteableBitmapToBitmapImage(BitmapWork), Rect, e.NewValue.Value);
            applyChange();
        }

        private void Scale_ValueChanged(object sender, EventArgs e)
        {
            /*try
            {
                BitmapResult = new WriteableBitmap(ToBitmapImage(OperationLibrary.Scale(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)), scaleX.Value, scaleY.Value)));
                applyChange();
                if (tt == 0)
                {
                    textScaleX.Text = scaleX.Value.ToString();
                    textScaleY.Text = scaleY.Value.ToString();
                    tt = 1;
                }
                else
                    tt = 0;
            }
            catch (ArgumentNullException) { };*/

            BitmapResult = new WriteableBitmap(ToBitmapImage(OperationLibrary.Scale(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)),float.Parse(scaleX.Text), float.Parse(scaleY.Text))));
            applyChange();
        }

        private void applyChange()
        {
            resultImage.Source = null;
            resultImage.Source = BitmapResult;
            workImage.Source = null;
            workImage.Source = BitmapWork;

            if (histonoff == 1)
            {
                workHistogram.Source = ToBitmapImage(OperationLibrary.createHistogram(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork))));
                resultHistogram.Source = ToBitmapImage(OperationLibrary.createHistogram(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapResult))));
            }
        }

        private void Threshold_Click(object sender, RoutedEventArgs e)
        {
            ThreshholdSelector ts = new ThreshholdSelector(nbrTresh.Value.Value, this);
            ts.Show();
        }

        private void Equalization_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapResult = new WriteableBitmap(ToBitmapImage(OperationLibrary.HistEq(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)))));
                applyChange();
            }
            catch (ArgumentNullException) { };
        }

        public void multiThreshHold(Thresh[] ttable)
        {
            WriteableBitmap tmp = new WriteableBitmap(BitmapWork);
            foreach(Thresh t in ttable)
            {
                try
                {
                    tmp = new WriteableBitmap(ToBitmapImage(OperationLibrary.Threshhold(t, BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(tmp)))));
                }
                catch (ArgumentNullException ex) { Console.WriteLine("exception multithreshold : " + ex.Message); };
            }
            BitmapResult = tmp;
            applyChange();
        }

        private void TextScaleY_TextChanged(object sender, TextChangedEventArgs e)
        {
           /* try
            {
                if (tt == 0)
                {
                    scaleX.Value = Int32.Parse(textScaleX.Text);
                    scaleY.Value = Int32.Parse(textScaleY.Text);
                    tt = 1;
                }
                else
                    tt = 0;
            }
            catch (NullReferenceException) { }
            catch (FormatException) { }*/
        }

        private void HistOnOff_Click(object sender, RoutedEventArgs e)
        {
            if (histonoff == 1)
            {
                histonoff = 0;
                histOnOff.Content = "Histogramme : off";
            }
            else
            {
                histonoff = 1;
                histOnOff.Content = "Histogramme : on";
            }
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            Kernel k;
            if(listFilter.SelectedValue.ToString() == "Gauss")
            {
                k = new GaussianFilter(float.Parse(dimFilter.Text));
            }
            else if(listFilter.SelectedValue.ToString() == "Laplace")
            {
                k = new LaplacianFilter(Int32.Parse(dimFilter.Text) == 0);
            }
            else if(listFilter.SelectedValue.ToString() == "Ordre")
            {
                k = new OrderFilter(Int32.Parse(dimFilter.Text));
            }
            else if(listFilter.SelectedValue.ToString() == "Moyen")
            {
                k = new MeanFilter(Int32.Parse(dimFilter.Text));
            }
            else //Alors on calcul le gradient, la methode est differente
            {
                Kernel k2;
                if (listFilter.SelectedValue.ToString() == "Sobel")
                {
                    k = new SobelFilter(true);
                    k2 = new SobelFilter(false);
                }
                else if (listFilter.SelectedValue.ToString() == "Prewitt")
                {
                    k = new PrewittFilter(true);
                    k2 = new PrewittFilter(false);
                }
                else if (listFilter.SelectedValue.ToString() == "Roberts")
                {
                    k = new RobertsFilter(true);
                    k2 = new RobertsFilter(false);
                }
                else return;

                Bitmap verticalDerive = k.filter(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)));
                Bitmap horizontalDerive = k2.filter(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)));
                BitmapResult = new WriteableBitmap(ToBitmapImage(OperationLibrary.Gradient(horizontalDerive, verticalDerive)));

                applyChange();
                return;
            }

            BitmapResult = new WriteableBitmap(ToBitmapImage(k.filter(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)))));

            applyChange();
        }

        private void listFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(listFilter.SelectedValue.ToString());
            dimFilter.IsEnabled = true;
            dimLabel.Content = "dim : ";
            if (listFilter.SelectedValue.ToString() == "Gauss")
            {
                dimLabel.Content = "Sigma : ";
            }
            else if (listFilter.SelectedValue.ToString() == "Laplace")
            {
                dimLabel.Content = "Diag ? ";
            }
            else if (listFilter.SelectedValue.ToString() == "Ordre")
            {
                dimLabel.Content = "Ordre : ";
            }
            else if (listFilter.SelectedValue.ToString() != "Moyen")
                dimFilter.IsEnabled = false;
        }

        private void WatershedButton_Click(object sender, RoutedEventArgs e)
        {
            WatershedAlgorithm algo = new WatershedAlgorithm(winPick.Value.Value);
            WatershedStructure imagestruct = new WatershedStructure(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)));
            int nbLabs = algo.Watershed(imagestruct);
            int[] grayPixels = new int[BitmapWork.PixelWidth * BitmapWork.PixelHeight];
            List<int> listGray = new List<int>();
            int widthInBytes = 4 * BitmapWork.PixelWidth;

            Console.WriteLine(nbLabs);
            foreach (WatershedPixel wp in imagestruct.Structure)
            {
                float f = ((float)16777216) / (float)nbLabs;
                float f2 = f * (float)(wp.Label - 1);
                int tmp = (int)f2; //Valeur en niveau de gris
                unchecked
                {
                    grayPixels[wp.X + wp.Y * BitmapResult.PixelWidth] = (int)((0xFF000000)| tmp);
                }
            }
            BitmapResult.WritePixels(new Int32Rect(0, 0, BitmapWork.PixelWidth, BitmapWork.PixelHeight), grayPixels, widthInBytes, 0);
        }

        private void MeyerButton_Click(object sender, RoutedEventArgs e)
        {
            MeyerWatershed algo = new MeyerWatershed();
            MeyerStruct imagestruct = new MeyerStruct(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)));
            int nbLabs = algo.watershed(imagestruct);
            int[] grayPixels = new int[BitmapWork.PixelWidth * BitmapWork.PixelHeight];
            List<int> listGray = new List<int>();
            int widthInBytes = 4 * BitmapWork.PixelWidth;

            Console.WriteLine(nbLabs);
            foreach (WatershedPixel wp in imagestruct.Structure)
            {
                int tmp;
                if(wp.Label == WatershedPixel.WSHED && !(wp.allNeightboursAreWSHED()))
                {
                    tmp = 255;
                }
                else
                {
                    float f = ((float)16777216) / (float)nbLabs;
                    float f2 = f * (float)(wp.Label - 1);
                    tmp = (int)f2; //Valeur en niveau de gris
                }

                unchecked
                {
                    grayPixels[wp.X + wp.Y * BitmapResult.PixelWidth] = (int)((0xFF000000) | tmp);
                }
            }
            BitmapResult.WritePixels(new Int32Rect(0, 0, BitmapWork.PixelWidth, BitmapWork.PixelHeight), grayPixels, widthInBytes, 0);
        }
    }
}
