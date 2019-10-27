using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    class MeanFilter : Kernel
    {
        public MeanFilter(int dim)
        {
            Width = dim;
            Height = dim;
            Values = new int[dim, dim];
            for(int i=0;i<dim;i++)
            {
                for(int j=0;j<dim;j++)
                {
                    Values[i, j] = 1;
                }
            }
            Coef = (dim * dim);
        }
    }
}
