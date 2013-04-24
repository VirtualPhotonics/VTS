using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.MonteCarlo.PhaseFunctions
{
    [TestFixture]
    public class MuellerMatrixTests
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [Test]
        public void validate_constructor()
        {
            List <double> theta = new List<double>();
            theta.Add(0.0); 
            theta.Add(Math.PI);
            double[] st11 = new[] { 0.0, 0.0 };
            double[] s12 = new[] { 0.0, 1.0 };
            double[] s13 = new[] { 0.0, 2.0 };
            double[] s14 = new[] { 0.0, 3.0 };
            double[] s21 = new[] { 0.0, 4.0 };
            double[] s22 = new[] { 0.0, 5.0 };
            double[] s23 = new[] { 0.0, 6.0 };
            double[] s24 = new[] { 0.0, 7.0 };
            double[] s31 = new[] { 0.0, 8.0 };
            double[] s32 = new[] { 0.0, 9.0 };
            double[] s33 = new[] { 0.0, 10.0 };
            double[] s34 = new[] { 0.0, 11.0 };
            double[] s41 = new[] { 0.0, 12.0 };
            double[] s42 = new[] { 0.0, 13.0 };
            double[] s43 = new[] { 0.0, 14.0 };
            double[] s44 = new[] { 0.0, 15.0 };
            MuellerMatrix m = new MuellerMatrix(theta, st11, s12, s13, s14, s21, s22, s23, s24, s31, s32, s33, s34, s41, s42, s43, s44);
            Assert.IsTrue(m.Theta.Equals(theta));
            Assert.IsTrue(m.St11.Equals(st11) && m.S12.Equals(s12) && m.S13.Equals(s13) && m.S14.Equals(s14) && m.S21.Equals(s21) && m.S22.Equals(s22) && m.S23.Equals(s23) && m.S24.Equals(s24) && m.S31.Equals(s31) && m.S32.Equals(s32) && m.S33.Equals(s33) && m.S34.Equals(s34) && m.S41.Equals(s41) && m.S42.Equals(s42) && m.S43.Equals(s43) && m.S44.Equals(s44));
        }

        /// <summary>
        /// Test to see if ReturnIndexAtThetaValue is correct.
        /// </summary>
        [Test]
        public void validated_ReturnIndexAtThetaValue()
        {
            MuellerMatrix m = new MuellerMatrix();
            m.Theta.Add(1.0);
            m.Theta.Add(2.0);
            m.Theta.Add(3.0);
            m.Theta.Add(4.0);
            m.Theta.Add(5.0);
            int test1, test2, test3, test4, test5, test6, test7, test8;
            test1 = m.ReturnIndexAtThetaValue(1.0);
            test2 = m.ReturnIndexAtThetaValue(2.0);
            test3 = m.ReturnIndexAtThetaValue(3.0);
            test4 = m.ReturnIndexAtThetaValue(4.0);
            test5 = m.ReturnIndexAtThetaValue(5.0);
            test6 = m.ReturnIndexAtThetaValue(3.5);
            test7 = m.ReturnIndexAtThetaValue(0.5);
            test8 = m.ReturnIndexAtThetaValue(4.99);
            


            Assert.AreEqual(1,test1);
            Assert.AreEqual(2, test2);
            Assert.AreEqual(3, test3);
            Assert.AreEqual(4, test4);
            Assert.AreEqual(5, test5);
            Assert.AreEqual(3, test6);
            Assert.AreEqual(0, test7);
            Assert.AreEqual(5, test8);


            //MuellerMatrix m2 = new MuellerMatrix();
            //m2.Theta = new[] { 0, Math.PI };
            //test7 = m2.ReturnIndexAtThetaValue(Math.PI);
            //Assert.AreEqual(1, test7);
        }

        /// <summary>
        /// Tests to validate if multiplication with this MuellerMatrix is correct.
        /// </summary>
        /// 

        [Test]
        public void validate_MultiplyByVector()
        {
            MuellerMatrix m = new MuellerMatrix();
            StokesVector v = new StokesVector(1, 0.5, 0.7, 0.6);
            m.MultiplyByVector(v, 0.0);
            Assert.AreEqual(v.S0, 0.25);
            Assert.AreEqual(v.S1, -0.25);
            Assert.AreEqual(v.S2, 0);
            Assert.AreEqual(v.S3, 0);
        }
        
    }
}
