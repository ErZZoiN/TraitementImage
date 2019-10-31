using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed
{
    public class WatershedPixel : IComparable
    {
        public enum label {INIT, MASK, WSHED};

        public static int INIT = -1;
        public static int MASK = -2;
        public static int WSHED = 0;
        public static int FICTIONOUS = -4;

        private int _x;
        private int _y;
        private int _height;
        private int _label;
        private int _depth;
        private List<WatershedPixel> _neightbours;

        public WatershedPixel(int lab)
        {
            Label = lab;
        }

        public WatershedPixel(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
            Label = INIT;
            Depth = 0;
            Neightbours = new List<WatershedPixel>();
        }

        public int Depth { get => _depth; set => _depth = value; }
        public int Label { get => _label; set => _label = value; }
        public int Height { get => _height; set => _height = value; }
        public int Y { get => _y; set => _y = value; }
        public int X { get => _x; set => _x = value; }
        public List<WatershedPixel> Neightbours { get => _neightbours; set => _neightbours = value; }

        public void addNeighbour(WatershedPixel n)
        {
            Neightbours.Add(n);
        }


        public String toString()
        {
            return "(" + Y + "," + Y + "), height : " + Height + ", label : " + Label + ", distance : " + Depth;
        }

        public int CompareTo(object obj)
        {
            WatershedPixel wp = (WatershedPixel)obj;

            if (wp.Height < Height)
                return 1;
            if (wp.Height > Height)
                return -1;
            return 0;
        }

        public Boolean allNeightboursAreWSHED()
        {
            for(int i=0;i<Neightbours.Count;i++)
            {
                WatershedPixel wp = Neightbours.ElementAt(i);

                if (wp.Label != WSHED)
                    return false;
            }

            return true;
        }

        public void initDepth()
        {
            Depth = Math.Abs(Height - Neightbours.ElementAt(0).Height);

            for(int i=1;i<Neightbours.Count;i++)
            {
                if (Math.Abs(Height - Neightbours.ElementAt(i).Height) < Depth)
                    Depth = Math.Abs(Height - Neightbours.ElementAt(i).Height);
            }
        }

        public void setLabelToMASK()
        {
            Label = MASK;
        }
    }
}
