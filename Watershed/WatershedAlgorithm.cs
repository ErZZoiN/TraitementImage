using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed
{
    public class WatershedAlgorithm
    {
        public struct StreamRet
        {
            public List<WatershedPixel> stream;
            public WatershedPixel.label lab;
        }

        public StreamRet Stream(WatershedStructure image, WatershedPixel currentPixel)
        {
            List<WatershedPixel> stream = new List<WatershedPixel>();
            stream.Add(currentPixel);

            List<WatershedPixel> unexploredBottom = new List<WatershedPixel>();
            unexploredBottom.Add(currentPixel);

            while(unexploredBottom.Count>0)
            {
                WatershedPixel y = unexploredBottom.ElementAt(0);
                unexploredBottom.RemoveAt(0);
                bool breadth_first = true;

                while(breadth_first && ())

            }

            return null;
        }

        public WatershedPixel closestNeightbours(WatershedPixel y, List<WatershedPixel> stream)
        {
            foreach (WatershedPixel wp in y.Neightbours)
            {
                if (stream.Exists(wp) == null && Math.Abs(wp.Height - y.Height) == y.Depth)
                    return wp;
            }

            return null;
        }
    }
}
