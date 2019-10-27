using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementImage
{
    public class GaussianFilter : Kernel
    {
        private double _sigma;
        public GaussianFilter(int w, int h, int[,] v, int c=1) : base(w, h, v, c) { }
        public GaussianFilter(double sigma, int dim)
        {
            Width = dim;
            Height = dim;
            Values = new int[dim, dim];
            Sigma = sigma;
        }

        public GaussianFilter(float si)
        {
            Sigma = si;
            Console.WriteLine("sigma : " + Sigma);
            int dim = ((int)Math.Ceiling(3 * si)) * 2 + 1;
            Console.WriteLine("dim : " + dim);
            Width = dim;
            Height = dim;
            Values = new int[dim, dim];
            init();
        }

        public double Sigma { get => _sigma; set => _sigma = value; }

        private void init()
        {
            double sigma22 = 2 * Sigma * Sigma;
            double sigmaPi2 = (2 * Math.PI * Sigma);
            int total = 0; //pour la normalisation
            double[,] RealMatrix = new double[Width, Width];
            for (int i = 0; i < Width; i++)
            {
                int x = i - (Width / 2);
                for (int j = 0; j < Width; j++)
                {
                    int y = j - Width / 2;
                    RealMatrix[i, j] = (1 / (sigmaPi2 * Sigma)) * Math.Exp((x * x + y * y) / sigma22);
                }
            }

            double div = RealMatrix[0, 0];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    RealMatrix[i, j] /= div;
                    Values[i, j] = (int)RealMatrix[i, j];
                    total += (int) RealMatrix[i, j];
                }
            }

            Coef = total;


        }
    }
}
