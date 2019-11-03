using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    class KirschFilter : Kernel
    {
        public KirschFilter(int num)
        {
            Width = Height = 3;
            Coef = 1;
            Values = new int[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Values[i, j] = -3;

            Values[1, 1] = 0;

            switch (num)
            {
                case 1:
                    Values[0, 0] = 5;
                    Values[0, 1] = 5;
                    Values[0, 2] = 5;
                    break;
                case 2:
                    Values[0, 0] = 5;
                    Values[0, 1] = 5;
                    Values[1, 0] = 5;
                    break;
                case 3:
                    Values[0, 0] = 5;
                    Values[1, 0] = 5;
                    Values[2, 0] = 5;
                    break;
                case 4:
                    Values[2, 0] = 5;
                    Values[2, 1] = 5;
                    Values[1, 0] = 5;
                    break;
                case 5:
                    Values[2, 0] = 5;
                    Values[2, 1] = 5;
                    Values[2, 2] = 5;
                    break;
                case 6:
                    Values[2, 2] = 5;
                    Values[2, 1] = 5;
                    Values[1, 2] = 5;
                    break;
                case 7:
                    Values[0, 2] = 5;
                    Values[1, 2] = 5;
                    Values[2, 2] = 5;
                    break;
                case 8:
                    Values[0, 2] = 5;
                    Values[1, 2] = 5;
                    Values[0, 1] = 5;
                    break;
            }
        }
    }
}
