using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed
{
    public class MeyerStruct
    {
        private List<WatershedPixel> _structure;

        public MeyerStruct(Bitmap image)
        {
            Structure = new List<WatershedPixel>();
            int width = image.Width;
            int height = image.Height;

            //Remplissage du tableau
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int gsv = (image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B) / 3;
                    Structure.Add(new WatershedPixel(x, y, gsv));
                }
            }
            for (int y = 0; y < height; y++)
            {

                int offset = y * width;
                int topOffset = offset + width;
                int bottomOffset = offset - width;
                for (int x = 0; x < width; x++)
                {
                    WatershedPixel currentPixel = (WatershedPixel)Structure.ElementAt(x + offset);

                    if (x + 1 < width)
                    {
                        currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x + 1 + offset));

                        if (y - 1 >= 0)
                            currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x + 1 + bottomOffset));

                        if (y + 1 < height)
                            currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x + 1 + topOffset));
                    }

                    if (x - 1 >= 0)
                    {
                        currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x - 1 + offset));

                        if (y - 1 >= 0)
                            currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x - 1 + bottomOffset));

                        if (y + 1 < height)
                            currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x - 1 + topOffset));
                    }

                    if (y - 1 >= 0)
                        currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x + bottomOffset));

                    if (y + 1 < height)
                        currentPixel.addNeighbour((WatershedPixel)Structure.ElementAt(x + topOffset));
                }
            }

            Structure.Sort();
        }
        public List<WatershedPixel> Structure { get => _structure; set => _structure = value; }
    }
}
