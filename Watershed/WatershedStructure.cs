using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed
{
    public class WatershedStructure
    {
        private WatershedPixel[,] _structure;

        public WatershedStructure(Bitmap image)
        {
            Structure = new WatershedPixel[image.Width, image.Height];

            //Remplissage du tableau
            for(int y = 0;y<image.Height;y++)
            {
                for(int x=0;x<image.Width;x++)
                {
                    int gsv = (image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B) / 3;
                    Structure[x, y] = new WatershedPixel(x, y, gsv);
                }
            }

            //Références des voisins
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    WatershedPixel wp = Structure[x, y];

                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (i != 0 || j != 0)
                            {
                                try
                                {
                                    wp.addNeighbour(Structure[x + i, y + j]);
                                }
                                catch (IndexOutOfRangeException e) { }
                            }
                        }
                    }
                    wp.initDepth();
                }
            }
        }
        public WatershedPixel[,] Structure { get => _structure; set => _structure = value; }
    }
}
