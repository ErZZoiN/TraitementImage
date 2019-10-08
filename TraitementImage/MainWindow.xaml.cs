﻿using System;
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

namespace TraitementImage
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _workspace;
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

        private void Scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                BitmapResult = new WriteableBitmap(ToBitmapImage(OperationLibrary.Scale(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork)), scaleX.Value, scaleY.Value)));
                applyChange();
            }
            catch (ArgumentNullException) { };
        }

        private void applyChange()
        {
            resultImage.Source = null;
            resultImage.Source = BitmapResult;
            workImage.Source = null;
            workImage.Source = BitmapWork;

            workHistogram.Source = ToBitmapImage(OperationLibrary.createHistogram(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapWork))));
            resultHistogram.Source = ToBitmapImage(OperationLibrary.createHistogram(BitmapImage2Bitmap(ConvertWriteableBitmapToBitmapImage(BitmapResult))));
        }
    }
}
