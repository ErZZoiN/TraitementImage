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
            public List<WatershedPixel> Stream;
            public int Lab;
        }

        public int Watershed(WatershedStructure image)
        {
            int nbLabs = 0;
            foreach(WatershedPixel wp in image.Structure)
            {
                if(wp.Label == WatershedPixel.LAB_INIT)
                {
                    StreamRet ret = Stream(image, wp);
                    if(ret.Lab==WatershedPixel.LAB_INIT) //Alors on n'est pas tombé sur un pixel déjà labellé i.e le stream est un inf-stream.
                    {
                        nbLabs++;
                        foreach(WatershedPixel wp2 in ret.Stream) //On initialise le label des pixels de l'inf-stream à un nouveau label
                        {
                            wp2.Label = nbLabs;
                        }
                    }
                    else //Alors le Label retourné par le stream est un label existant, celui de l'inf-stream que le stream de notre point a rejoint.
                    {
                        foreach(WatershedPixel wp2 in ret.Stream)
                        {
                            wp2.Label = ret.Lab;
                        }
                    }

                }
            }
            return nbLabs;
        }

        //Représente l'écoulement d'une goutte d'eau depuis le pixel donné
        public StreamRet Stream(WatershedStructure image, WatershedPixel currentPixel)
        {
            List<WatershedPixel> stream = new List<WatershedPixel>(); //Le stream induit par la goutte d'eau coulant de notre point.
            stream.Add(currentPixel);

            List<WatershedPixel> unexploredBottom = new List<WatershedPixel>(); //L'ensemble des points d'altitude minimum du stream
            unexploredBottom.Add(currentPixel);

            while(unexploredBottom.Count>0) //Tant qu'il y a des points de plus basse altitude
            {
                WatershedPixel y = unexploredBottom.ElementAt(0);
                unexploredBottom.RemoveAt(0);
                bool breadth_first = true;

                WatershedPixel bufferPix = closestNeightbours(y, stream);
                while(breadth_first && (bufferPix!=null))
                {
                    if (bufferPix.Label != WatershedPixel.LAB_INIT) //Alors bufferPix est déjà labellé, et c'est le label de notre currentPixel
                        return new StreamRet()
                        {
                            Stream = stream,
                            Lab = bufferPix.Label
                        };
                    
                    if(bufferPix.Depth < y.Depth) //Alors bufferpix est sous le point y, et on progresse à partir de ce point.
                    {
                        stream.Add(bufferPix);
                        unexploredBottom = new List<WatershedPixel>();
                        unexploredBottom.Add(bufferPix);
                        breadth_first = false;
                    }
                    else //bufferPix = y, on est sur un 'plateau'
                    {
                        stream.Add(bufferPix);
                        unexploredBottom.Add(bufferPix);
                    }
                    bufferPix = closestNeightbours(y, stream);
                }

            }


            return new StreamRet() //On est arrivé a un minimum local
            {
                Stream=stream,
                Lab=WatershedPixel.LAB_INIT
            };
        }

        public WatershedPixel closestNeightbours(WatershedPixel y, List<WatershedPixel> stream) //Vers quel pixel ma goutte va-t-elle tomber
        {
            foreach (WatershedPixel wp in y.Neightbours)
            {
                if (stream.IndexOf(wp) == -1 && Math.Abs(wp.Height - y.Height) == y.Depth)
                    return wp;
            }

            return null;
        }
    }
}
