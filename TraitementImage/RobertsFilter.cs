using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    class RobertsFilter : Kernel
    {
        public RobertsFilter(bool vertical)
        {
            Width = Height = 2;
            Values = new int[2, 2];

            if(vertical)
            {
                Values[0, 0] = 1;
                Values[1, 0] = 0;
                Values[0, 1] = 0;
                Values[1, 1] = -1;
            }
            else
            {
                Values[0, 0] = 0;
                Values[1, 0] = -1;
                Values[0, 1] = 1;
                Values[1, 1] = 0;
            }
        }
    }
}
