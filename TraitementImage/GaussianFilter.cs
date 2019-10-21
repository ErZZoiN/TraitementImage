using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    public class GaussianFilter : Kernel
    {
        private float _sigma;
        //public GaussianFilter(int w, int h, int[,] v, float c=1) : base(w, h, v, c) { }
        public GaussianFilter(float sigma, int dim)
        {
            Width = dim;
            Height = dim;
            Values = new int[dim, dim];

        }

        public float Sigma { get => _sigma; set => _sigma = value; }
    }
}
