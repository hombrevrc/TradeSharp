using System;
using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using TradeSharp.Util;
using TradeSharp.Util.ComplexMath;

namespace TradeSharp.Test.ComplexMath
{
    [TestFixture]
    public class FourierTransformTest
    {
        [Test]
        public void TestOneSignal()
        {
            var data = new double[4096];

            for (var i = 0; i < data.Length; i++)
            {
                var alpha = i*180/System.Math.PI;
                var signal = System.Math.Cos(alpha) + 100;
                data[i] = signal;
            }

            var ampls = FourierTransform.Transform(data);
        }

        [Test]
        public void Matrix()
        {
            var random = new Random();
            const int w = 60, h = 40;
            var matrix = new float[50][];
            for (var y = 0; y < h; y++)
            {
                matrix[y] = new float[w];
                for (var x = 0; x < w; x++)
                {
                    matrix[y][x] = random.Next(100) < 2 ? random.Next(10, 50) : 0;
                    //if (y == 20 && x == 30) matrix[y][x] = 40;
                }
            }
            matrix = ProcessMatrix(matrix);
            PrintMatrix(matrix);
        }

        private static float[][] ProcessMatrix(float[][] matrix)
        {
            var signals = new List<Cortege2<Point, float>>();
            const int h = 40;
            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < matrix[y].Length; x++)
                {
                    var val = matrix[y][x];
                    if (val != 0) signals.Add(new Cortege2<Point, float>(new Point(x, y), val));                    
                }
            }

            var newMatrix = new float[h][];
            for (var y = 0; y < h; y++)
            {
                newMatrix[y] = new float[matrix[y].Length];
                for (var x = 0; x < matrix[y].Length; x++)
                {
                    var power = 0f;
                    foreach (var signal in signals)
                    {
                        var range = (float) ((x - signal.a.X)*(x - signal.a.X) + (y - signal.a.Y)*(y - signal.a.Y));
                        power += signal.b / range;
                    }
                    newMatrix[y][x] = power;
                }
            }
            return newMatrix;
        }

        private static void PrintMatrix(float [][] matrix)
        {
            var fieldStr = "";
            const int h = 40;
            for (var y = 0; y < h; y++)
            {
                var line = "";
                for (var x = 0; x < matrix[y].Length; x++)
                {
                    var val = matrix[y][x];
                    var chr = val < 1 ? '.' : val < 10 ? 'o' : val < 25 ? 'O' : '#';
                    line += chr;
                }
                line += Environment.NewLine;
                fieldStr += line;
            }
        }
    }
}
