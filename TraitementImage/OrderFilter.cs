using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    class OrderFilter : Kernel
    {
        private int _order;
        public OrderFilter(int order)
        {
            Order = order;
            Width = 3;
            Height = 3;
        }

        public int Order { get => _order; set => _order = value; }

        public override Bitmap filter(Bitmap src)
        {
            Bitmap result = new Bitmap(src);

            List<int> listVal = new List<int>();

            for (int i = 0; i < src.Width; i++)
            {
                for (int j = 0; j < src.Height; j++)
                {
                    for (int x = -(Width / 2); x <= (Width / 2); x++)
                    {
                        for (int y = -(Height / 2); y <= (Height / 2); y++)
                        {
                            try
                            {
                                listVal.Add(src.GetPixel(i + x, j + y).R);
                            }
                            catch(ArgumentOutOfRangeException e) { }
                        }
                    }

                    listVal.Sort();
                    int val = src.GetPixel(i, j).R;
                    try
                    {
                        val = listVal.ElementAt(Order);
                    } catch(Exception e) { }
                    Console.WriteLine("i : "+i+"    j : "+j+"    val : "+val);
                    result.SetPixel(i, j, System.Drawing.Color.FromArgb(val, val, val));
                    listVal.Clear();
                }
            }

            return result;
        }
    }
}
