using NUnit.Framework;
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
    }
}
