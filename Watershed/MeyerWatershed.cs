using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed
{
    public class MeyerWatershed
    {
        private int threshold;
        static int HMIN = 0;
        static int HMAX = 256;

        public int watershed(MeyerStruct image)
        {
            Queue<WatershedPixel> queue = new Queue<WatershedPixel>();
            int curlab = 0;
            int heightIndex1 = 0;
            int heightIndex2 = 0;

            for (int h = HMIN; h < HMAX; h++)
            {
                for (int pixelIndex = heightIndex1; pixelIndex < image.Structure.Count; pixelIndex++)
                {
                    WatershedPixel p = image.Structure.ElementAt(pixelIndex);

                    if(p.Height != h)
                    {
                        heightIndex1 = pixelIndex;
                        break;
                    }

                    p.setLabelToMASK();

                    List<WatershedPixel> neighbours = p.Neightbours;

                    for (int i = 0; i < neighbours.Count(); i++)
                    {
                        WatershedPixel q = (WatershedPixel)neighbours.ElementAt(i);

                        if (q.Label >= 0)
                        {/*Initialise queue with neighbours at level h of current basins or watersheds*/
                            p.Depth = 1;
                            queue.Enqueue(p);
                            break;
                        } // end if
                    }
                }
                int curdist = 1;
                queue.Enqueue(new WatershedPixel(WatershedPixel.FICTIONOUS));

                while(true)
                {
                    WatershedPixel p = queue.Dequeue();
                    if (p.Label == WatershedPixel.FICTIONOUS)
                        if (queue.Count==0)
                            break;
                        else
                        {
                            queue.Enqueue(new WatershedPixel(WatershedPixel.FICTIONOUS));
                            curdist++;
                            p = queue.Dequeue();
                        }

                    List<WatershedPixel> neighbours = p.Neightbours;

                    for (int i = 0; i < neighbours.Count; i++)
                    {
                        WatershedPixel q = (WatershedPixel)neighbours.ElementAt(i);

                        if ((q.Depth <= curdist) && (q.Label >= 0))
                        {
                            if (q.Label > 0)
                            {
                                if (p.Label == WatershedPixel.MASK)
                                    p.Label = q.Label;
                                else
                                    if (p.Label != q.Label)
                                    p.Label = WatershedPixel.WSHED;
                            }
                            else
                                if (p.Label == WatershedPixel.MASK)
                                p.Label = WatershedPixel.WSHED;
                        }
                        else
                            if(q.Label == WatershedPixel.MASK && q.Depth ==0)
                        {
                            q.Depth = curdist + 1;
                            queue.Enqueue(q);
                        }
                    }
                }

                for (int pixelIndex = heightIndex2; pixelIndex < image.Structure.Count; pixelIndex++)
                {
                    WatershedPixel p = image.Structure.ElementAt(pixelIndex);

                    if(p.Height != h)
                    {
                        heightIndex2 = pixelIndex;
                        break;
                    }

                    p.Depth = 0;

                    if(p.Label == WatershedPixel.MASK)
                    {
                        curlab++;
                        p.Label = curlab;
                        queue.Enqueue(p);

                        while(queue.Count>0)
                        {
                            WatershedPixel q = queue.Dequeue();

                            List<WatershedPixel> neighbours = p.Neightbours;

                            for (int i = 0; i < neighbours.Count; i++)
                            {
                                WatershedPixel r = neighbours.ElementAt(i);

                                if(r.Label == WatershedPixel.MASK)
                                {
                                    r.Label = curlab;
                                    queue.Enqueue(r);
                                }
                            }
                        }
                    }
                }

            }
            return curlab;
        }
    }
}
