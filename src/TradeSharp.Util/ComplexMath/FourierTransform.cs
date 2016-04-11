using System;

namespace TradeSharp.Util.ComplexMath
{
    public static class FourierTransform
    {
        public static double[] Transform(double[] x)
        {
            double[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new double[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                double[] x_even = new double[N / 2];
                double[] x_odd = new double[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                double[] X_even = Transform(x_even);
                double[] X_odd = Transform(x_odd);
                X = new double[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + FuncW(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - FuncW(i, N) * X_odd[i];
                }
            }
            return X;
        }

        private static double FuncW(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return Math.Cos(arg);
        }
    }
}
