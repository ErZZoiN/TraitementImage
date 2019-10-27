using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    class LaplacianFilter : Kernel
    {
        public LaplacianFilter(bool diagon)
        {
            Width = 3;
            Height = 3;
            Values = new int[3, 3];

            if (!diagon)
            {
                Values[0, 1] = 1;
                Values[2, 1] = 1;
                Values[1, 0] = 1;
                Values[1, 2] = 1;
                Values[1, 1] = -4;
            }
            else
            {
                for(int i=0;i<Width;i++)
                {
                    for(int j=0;j<Width;j++)
                    {
                        Values[i, j] = 1;
                    }
                }
                Values[1, 1] = -8;
            }
        }
    }
}
