using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    public class Kernel
    {
        private int _width;
        private int _height;
        private float _coef;
        private int[,] _values;

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public int[,] Values { get => _values; set => _values = value; }
        public float Coef { get => _coef; set => _coef = value; }

        /*public Kernel(int w, int h, int[,] v, float c=1)
        {
            Width = w;
            Height = h;
            Values = v;
            Coef = c;
        }*/

        public Bitmap filter(Bitmap src)
        {
            Bitmap result = new Bitmap(src);
            for(int i=0;i<src.Width;i++)
            {
                for(int j=0;j<src.Height;j++)
                {
                    int newval = 0;
                    for(int x=-(Width/2);x<(Width/2);x++)
                    {
                        for(int y=-(Height/2);y<(Height/2);x++)
                        {
                            newval += src.GetPixel(i + x, j + y).R * Values[x, y];
                        }
                    }
                    newval = (int)(newval * Coef);
                    result.SetPixel(i, j, System.Drawing.Color.FromArgb(newval, newval, newval));
                }
            }

            return result;
        }
    }
}
