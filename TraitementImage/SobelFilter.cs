using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    class SobelFilter : Kernel
    {
        public SobelFilter(bool vertical)
        {
            Width = Height = 3;
            Values = new int[3, 3];
            Coef = 4;

            for (int i = 0; i < Width; i++)
            {
                int val = i - 1;
                for (int j = 0; j < Width; j++)
                {
                    if (vertical)
                        Values[i, j] = val + val*(j % 2);
                    else
                        Values[j, i] = val + val*(j % 2);
                }
            }
        }
    }
}
