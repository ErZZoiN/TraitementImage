using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TraitementImage
{
    public class ZoomBorder : Border
    {
        public UIElement child = null;
        private Point origin;
        private Point start;
        private Int32Rect _roi;

        public delegate void sendRect(Int32Rect roi);
        public delegate void resetRect();
        public event resetRect ResetRoi;
        public event sendRect SendRoi;

        public TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        public ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        public override UIElement Child
        {
            get { return base.Child; }
            set
            {
                if (value != null && value != Child)
                    Initialize(value);
                base.Child = value;
            }
        }

        public Int32Rect Roi { get => _roi; set => _roi = value; }

        public void Initialize(UIElement element)
        {
            child = element;
            if (child != null)
            {
                TransformGroup group = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                group.Children.Add(st);
                TranslateTransform tt = new TranslateTransform();
                group.Children.Add(tt);
                child.RenderTransform = group;
                child.RenderTransformOrigin = new Point(0.0, 0.0);
                MouseWheel += Child_MouseWheel;
                MouseLeftButtonDown += Child_MouseLeftButtonDown;
                MouseLeftButtonUp += Child_MouseLeftButtonUp;
                MouseMove += Child_MouseMove;
                MouseRightButtonDown += new MouseButtonEventHandler(
                  Child_MouseRightButtonDown);
                MouseRightButtonUp += new MouseButtonEventHandler(Child_MouseRightButtonUp);
            }
        }

        public void Reset()
        {
            if (child != null)
            {
                // reset zoom
                ScaleTransform st = GetScaleTransform(child);
                st.ScaleX = 1.0;
                st.ScaleY = 1.0;

                // reset pan
                TranslateTransform tt = GetTranslateTransform(child);
                tt.X = 0.0;
                tt.Y = 0.0;
            }
        }

        #region Child Events

        private void Child_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (child != null)
            {
                ScaleTransform st = GetScaleTransform(child);
                TranslateTransform tt = GetTranslateTransform(child);

                double zoom = e.Delta > 0 ? .2 : -.2;
                if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                    return;

                Point relative = e.GetPosition(child);
                double absoluteX;
                double absoluteY;

                absoluteX = relative.X * st.ScaleX + tt.X;
                absoluteY = relative.Y * st.ScaleY + tt.Y;

                st.ScaleX += zoom;
                st.ScaleY += zoom;

                tt.X = absoluteX - relative.X * st.ScaleX;
                tt.Y = absoluteY - relative.Y * st.ScaleY;
            }
        }

        private void Child_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                try
                {
                    ResetRoi();
                }
                catch (NullReferenceException)
                {
                }
                TranslateTransform tt = GetTranslateTransform(child);
                start = e.GetPosition(this);
                origin = new Point(tt.X, tt.Y);
                Cursor = Cursors.Hand;
                child.CaptureMouse();
            }
        }

        private void Child_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
        }

        void Child_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                TranslateTransform tt = GetTranslateTransform(child);
                origin = new Point(tt.X, tt.Y);
                start = e.GetPosition(this);
                Int32Rect rectBuffer = new Int32Rect
                {
                    X = (int)start.X,
                    Y = (int)start.Y,
                    Height = 0,
                    Width = 0
                };
                Roi = rectBuffer;
                Cursor = Cursors.Cross;
                child.CaptureMouse();
            }
        }

        void Child_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (child != null)
            {
                child.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
                Console.WriteLine(Roi.ToString());
            }
        }

        private void Child_MouseMove(object sender, MouseEventArgs e)
        {
            if (child != null)
            {
                if (child.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
                {
                    TranslateTransform tt = GetTranslateTransform(child);
                    Vector v = start - e.GetPosition(this);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                }
                else if (child.IsMouseCaptured && e.RightButton == MouseButtonState.Pressed && e.LeftButton == MouseButtonState.Released)
                {
                    TranslateTransform tt = GetTranslateTransform(child);
                    Vector v = (Vector)e.GetPosition(this);
                    Int32Rect rectBuffer = Roi;
                    rectBuffer.Width = -(int)(start.X - v.X);
                    rectBuffer.Height = -(int)(start.Y - v.Y);
                    Roi = rectBuffer;
                    try
                    {
                        SendRoi(Roi);
                    }
                    catch (NullReferenceException) //Si pas d'image loadée
                    { }
                }
            }
        }

        #endregion
    }
}
