/*
 Copyright (C) 2020 Jean-Camille Tournier (mail@tournierjc.fr)

 This file is part of QLCore Project https://github.com/OpenDerivatives/QLCore

 QLCore is free software: you can redistribute it and/or modify it
 under the terms of the QLCore and QLNet license. You should have received a
 copy of the license along with this program; if not, license is
 available at https://github.com/OpenDerivatives/QLCore/LICENSE.

 QLCore is a forked of QLNet which is a based on QuantLib, a free-software/open-source
 library for financial quantitative analysts and developers - http://quantlib.org/
 The QuantLib license is available online at http://quantlib.org/license.shtml and the
 QLNet license is available online at https://github.com/amaggiulli/QLNet/blob/develop/LICENSE.

 This program is distributed in the hope that it will be useful, but WITHOUT
 ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 FOR A PARTICAR PURPOSE. See the license for more details.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using QLCore;

namespace TestSuite
{

    public class T_CmsSpread
    {
        [Fact]
        public void testFixings()
        {
            TestData d = new TestData();

            SwapIndex cms10y = new EuriborSwapIsdaFixA(new Period(10, TimeUnit.Years), d.yts2, d.yts2);
            SwapIndex cms2y = new EuriborSwapIsdaFixA(new Period(2, TimeUnit.Years), d.yts2, d.yts2);
            SwapSpreadIndex cms10y2y = new SwapSpreadIndex("cms10y2y", cms10y, cms2y);

            Settings.Instance.enforcesTodaysHistoricFixings = false;

            try
            {
                cms10y2y.fixing(d.refDate - 1);
                QAssert.Fail("fixing on refDate did not throwed an error.");
            }
            catch
            {
            }

            try
            {
                cms10y2y.fixing(d.refDate);
            }
            catch
            {
                QAssert.Fail("fixing on refDate throwed an error.");
            }

            QAssert.IsTrue(cms10y2y.fixing(d.refDate) ==
                              cms10y.fixing(d.refDate) - cms2y.fixing(d.refDate));
            cms10y.addFixing(d.refDate, 0.05);
            QAssert.IsTrue(cms10y2y.fixing(d.refDate) ==
                              cms10y.fixing(d.refDate) - cms2y.fixing(d.refDate));
            cms2y.addFixing(d.refDate, 0.04);
            QAssert.IsTrue(cms10y2y.fixing(d.refDate) ==
                              cms10y.fixing(d.refDate) - cms2y.fixing(d.refDate));
            Date futureFixingDate = new TARGET().adjust(d.refDate + new Period(1, TimeUnit.Years));
            QAssert.IsTrue(cms10y2y.fixing(futureFixingDate) ==
                              cms10y.fixing(futureFixingDate) -
                                  cms2y.fixing(futureFixingDate));
            IndexManager.Instance.clearHistories();

            Settings.Instance.enforcesTodaysHistoricFixings = true;
            try
            {
                cms10y2y.fixing(d.refDate);
                QAssert.Fail("fixing on refDate did not throwed an error.");
            }
            catch
            {
            }

            cms10y.addFixing(d.refDate, 0.05);

            try
            {
                cms10y2y.fixing(d.refDate);
                QAssert.Fail("fixing on refDate did not throwed an error.");
            }
            catch
            {
            }
            cms2y.addFixing(d.refDate, 0.04);

            QAssert.IsTrue(cms10y2y.fixing(d.refDate) ==
                              cms10y.fixing(d.refDate) - cms2y.fixing(d.refDate));
            IndexManager.Instance.clearHistories();
        }

        [Fact]
        public void testCouponPricing()
        {
            TestData d = new TestData();
            double tol = 1E-6; // abs tolerance coupon rate

            SwapIndex cms10y = new EuriborSwapIsdaFixA(new Period(10, TimeUnit.Years), d.yts2, d.yts2);
            SwapIndex cms2y = new EuriborSwapIsdaFixA(new Period(2, TimeUnit.Years), d.yts2, d.yts2);
            SwapSpreadIndex cms10y2y = new SwapSpreadIndex("cms10y2y", cms10y, cms2y);

            Date valueDate = cms10y2y.valueDate(d.refDate);
            Date payDate = valueDate + new Period(1, TimeUnit.Years);
            CmsCoupon cpn1a = new CmsCoupon(10000.0, payDate, valueDate, payDate, cms10y.fixingDays(), cms10y,
                                            1.0, 0.0, null, null, new Actual360(), false);
            CmsCoupon cpn1b = new CmsCoupon(10000.0, payDate, valueDate, payDate, cms2y.fixingDays(),
                                            cms2y, 1.0, 0.0, null, null, new Actual360(), false);
            CmsSpreadCoupon cpn1 = new CmsSpreadCoupon(
                                        10000.0, payDate, valueDate, payDate, cms10y2y.fixingDays(),
                                        cms10y2y, 1.0, 0.0, null, null, new Actual360(), false);

            QAssert.IsTrue(cpn1.fixingDate() == d.refDate);
            cpn1a.setPricer(d.cmsPricerLn);
            cpn1b.setPricer(d.cmsPricerLn);
            cpn1.setPricer(d.cmsspPricerLn);
            QAssert.IsTrue(cpn1.rate() == cpn1a.rate() - cpn1b.rate());
            cms10y.addFixing(d.refDate, 0.05);
            QAssert.IsTrue(cpn1.rate() == cpn1a.rate() - cpn1b.rate());
            cms2y.addFixing(d.refDate, 0.03);
            QAssert.IsTrue(cpn1.rate() == cpn1a.rate() - cpn1b.rate());
            IndexManager.Instance.clearHistories();

           CmsCoupon cpn2a = new CmsCoupon(10000.0, new Date(23, Month.February, 2029),
                              new Date(23, Month.February, 2028), new Date(23, Month.February, 2029), 2,
                              cms10y, 1.0, 0.0, null, null, new Actual360(), false);
            CmsCoupon cpn2b = new CmsCoupon(10000.0, new Date(23, Month.February, 2029),
                              new Date(23, Month.February, 2028), new Date(23, Month.February, 2029), 2,
                              cms2y, 1.0, 0.0, null, null, new Actual360(), false);

            CappedFlooredCmsSpreadCoupon plainCpn =
                    new CappedFlooredCmsSpreadCoupon(
                        new Date(23, Month.February, 2029), 10000.0, new Date(23, Month.February, 2028),
                        new Date(23, Month.February, 2029), 2, cms10y2y, 1.0, 0.0, null,
                        null, null, null, new Actual360(), false);
            CappedFlooredCmsSpreadCoupon cappedCpn =
                    new CappedFlooredCmsSpreadCoupon(
                        new Date(23, Month.February, 2029), 10000.0, new Date(23, Month.February, 2028),
                        new Date(23, Month.February, 2029), 2, cms10y2y, 1.0, 0.0, 0.03,
                        null, null, null, new Actual360(), false);
            CappedFlooredCmsSpreadCoupon flooredCpn =
                    new CappedFlooredCmsSpreadCoupon(
                        new Date(23, Month.February, 2029), 10000.0, new Date(23, Month.February, 2028),
                        new Date(23, Month.February, 2029), 2, cms10y2y, 1.0, 0.0, null,
                        0.01, null, null, new Actual360(), false);
            CappedFlooredCmsSpreadCoupon collaredCpn =
                    new CappedFlooredCmsSpreadCoupon(
                        new Date(23, Month.February, 2029), 10000.0, new Date(23, Month.February, 2028),
                        new Date(23, Month.February, 2029), 2, cms10y2y, 1.0, 0.0, 0.03, 0.01,
                        null, null, new Actual360(), false);

            cpn2a.setPricer(d.cmsPricerLn);
            cpn2b.setPricer(d.cmsPricerLn);
            plainCpn.setPricer(d.cmsspPricerLn);
            cappedCpn.setPricer(d.cmsspPricerLn);
            flooredCpn.setPricer(d.cmsspPricerLn);
            collaredCpn.setPricer(d.cmsspPricerLn);

            QAssert.IsTrue(
                Math.Abs(plainCpn.rate() - mcReferenceValue(cpn2a, cpn2b, Double.MaxValue,
                                                             -Double.MaxValue, d.swLn,
                                                             d.correlation.currentLink().value())) <
                tol);
            QAssert.IsTrue(
                Math.Abs(cappedCpn.rate() - mcReferenceValue(cpn2a, cpn2b, 0.03,
                                                              -Double.MaxValue, d.swLn,
                                                              d.correlation.currentLink().value())) <
                tol);
            QAssert.IsTrue(
                Math.Abs(flooredCpn.rate() -
                         mcReferenceValue(cpn2a, cpn2b, Double.MaxValue, 0.01, d.swLn,
                                          d.correlation.currentLink().value())) <

                tol);
            QAssert.IsTrue(
                Math.Abs(collaredCpn.rate() -
                         mcReferenceValue(cpn2a, cpn2b, 0.03, 0.01, d.swLn,
                                          d.correlation.currentLink().value())) <
                tol);

            cpn2a.setPricer(d.cmsPricerSln);
            cpn2b.setPricer(d.cmsPricerSln);
            plainCpn.setPricer(d.cmsspPricerSln);
            cappedCpn.setPricer(d.cmsspPricerSln);
            flooredCpn.setPricer(d.cmsspPricerSln);
            collaredCpn.setPricer(d.cmsspPricerSln);

            QAssert.IsTrue(
                Math.Abs(plainCpn.rate() - mcReferenceValue(cpn2a, cpn2b, Double.MaxValue,
                                                             -Double.MaxValue, d.swSln,
                                                             d.correlation.currentLink().value())) <
                tol);
            QAssert.IsTrue(
                Math.Abs(cappedCpn.rate() - mcReferenceValue(cpn2a, cpn2b, 0.03,
                                                              -Double.MaxValue, d.swSln,
                                                              d.correlation.currentLink().value())) <
                tol);
            QAssert.IsTrue(
                Math.Abs(flooredCpn.rate() -
                         mcReferenceValue(cpn2a, cpn2b, Double.MaxValue, 0.01, d.swSln,
                                          d.correlation.currentLink().value())) <

                tol);
            QAssert.IsTrue(
                Math.Abs(collaredCpn.rate() -
                         mcReferenceValue(cpn2a, cpn2b, 0.03, 0.01, d.swSln,
                                          d.correlation.currentLink().value())) <
                tol);

            cpn2a.setPricer(d.cmsPricerN);
            cpn2b.setPricer(d.cmsPricerN);
            plainCpn.setPricer(d.cmsspPricerN);
            cappedCpn.setPricer(d.cmsspPricerN);
            flooredCpn.setPricer(d.cmsspPricerN);
            collaredCpn.setPricer(d.cmsspPricerN);

            QAssert.IsTrue(
                Math.Abs(plainCpn.rate() - mcReferenceValue(cpn2a, cpn2b, Double.MaxValue,
                                                             -Double.MaxValue, d.swN,
                                                             d.correlation.currentLink().value())) <
                tol);
            QAssert.IsTrue(
                Math.Abs(cappedCpn.rate() - mcReferenceValue(cpn2a, cpn2b, 0.03,
                                                              -Double.MaxValue, d.swN,
                                                              d.correlation.currentLink().value())) <
                tol);
            QAssert.IsTrue(Math.Abs(flooredCpn.rate() -
                                       mcReferenceValue(cpn2a, cpn2b, Double.MaxValue, 0.01,
                                                        d.swN, d.correlation.currentLink().value())) <

                              tol);
            QAssert.IsTrue(Math.Abs(collaredCpn.rate() -
                                       mcReferenceValue(cpn2a, cpn2b, 0.03, 0.01, d.swN,
                                                        d.correlation.currentLink().value())) <
                              tol);
        }

        protected double mcReferenceValue(CmsCoupon cpn1,
                                          CmsCoupon cpn2,
                                          double cap,
                                          double floor,
                                          Handle<SwaptionVolatilityStructure> vol,
                                          double correlation)
        {
            List<double> acc = new List<double>();
            int samples = 1000000;
            Matrix Cov = new Matrix(2, 2);
            Cov[0, 0] = vol.currentLink().blackVariance(cpn1.fixingDate(), cpn1.index().tenor(),
                                           cpn1.indexFixing());
            Cov[1, 1] = vol.currentLink().blackVariance(cpn2.fixingDate(), cpn2.index().tenor(),
                                           cpn2.indexFixing());
            Cov[0, 1] = Cov[1, 0] = Math.Sqrt(Cov[0, 0] * Cov[1, 1]) * correlation;
            Matrix C = MatrixUtilitites.pseudoSqrt(Cov, MatrixUtilitites.SalvagingAlgorithm.None);

            List<double> atmRate = new InitializedList<double>(2), adjRate = new InitializedList<double>(2),
                        volShift = new InitializedList<double>(2);

            Vector avg = new Vector(2);

            atmRate[0] = cpn1.indexFixing();
            atmRate[1] = cpn2.indexFixing();
            adjRate[0] = cpn1.adjustedFixing;
            adjRate[1] = cpn2.adjustedFixing;
            if (vol.currentLink().volatilityType() == VolatilityType.ShiftedLognormal)
            {
                volShift[0] = vol.currentLink().shift(cpn1.fixingDate(), cpn1.index().tenor());
                volShift[1] = vol.currentLink().shift(cpn2.fixingDate(), cpn2.index().tenor());
                avg[0] =
                    Math.Log((adjRate[0] + volShift[0]) / (atmRate[0] + volShift[0])) -
                    0.5 * Cov[0, 0];
                avg[1] =
                    Math.Log((adjRate[1] + volShift[1]) / (atmRate[1] + volShift[1])) -
                    0.5 * Cov[1, 1];
            }
            else
            {
                avg[0] = adjRate[0];
                avg[1] = adjRate[1];
            }

            InverseCumulativeNormal icn = new InverseCumulativeNormal();
            SobolRsg sb_ = new SobolRsg(2, 42);
            Vector w = new Vector(2), z = new Vector(2);
            for (int i = 0; i < samples; ++i)
            {
                List<double> seq = sb_.nextSequence().value;
                w[0] = icn.value(seq[0]);
                w[1] = icn.value(seq[1]);

                z = C * w + avg;
                for (int j = 0; j < 2; ++j)
                {
                    if (vol.currentLink().volatilityType() == VolatilityType.ShiftedLognormal)
                    {
                        z[j] =
                            (atmRate[j] + volShift[j]) * Math.Exp(z[j]) - volShift[j];
                    }
                }
                acc.Add(Math.Min(Math.Max(z[0] - z[1], floor), cap));
            }

            return acc.Average();
        } // mcReferenceValue


        public class TestData
        {
            public TestData()
            {
                refDate = new Date(23, Month.February, 2018);
                Settings.Instance.setEvaluationDate(refDate);

                yts2 = new Handle<YieldTermStructure>(new FlatForward(refDate, 0.02, new Actual365Fixed()));

                swLn = new Handle<SwaptionVolatilityStructure>(new ConstantSwaptionVolatility(refDate, new TARGET(),
                                                               BusinessDayConvention.Following, 0.20, new Actual365Fixed(),
                                                               VolatilityType.ShiftedLognormal, 0.0));
                swSln = new Handle<SwaptionVolatilityStructure>(new ConstantSwaptionVolatility(refDate, new TARGET(),
                                                                BusinessDayConvention.Following, 0.10, new Actual365Fixed(),
                                                                VolatilityType.ShiftedLognormal, 0.01));
                swN = new Handle<SwaptionVolatilityStructure>(new ConstantSwaptionVolatility(refDate, new TARGET(),
                                                              BusinessDayConvention.Following, 0.0075, new Actual365Fixed(),
                                                              VolatilityType.Normal, 0.01));

                reversion = new Handle<Quote>(new SimpleQuote(0.01));
                cmsPricerLn = new LinearTsrPricer(swLn, reversion, yts2);
                cmsPricerSln = new LinearTsrPricer(swSln, reversion, yts2);
                cmsPricerN = new LinearTsrPricer(swN, reversion, yts2);

                correlation = new Handle<Quote>(new SimpleQuote(0.6));
                cmsspPricerLn = new LognormalCmsSpreadPricer(cmsPricerLn, cmsPricerLn, correlation, yts2, 32);
                cmsspPricerSln = new LognormalCmsSpreadPricer(cmsPricerSln, cmsPricerSln, correlation, yts2, 32);
                cmsspPricerN = new LognormalCmsSpreadPricer(cmsPricerN, cmsPricerN, correlation, yts2, 32);
            }

            public SavedSettings backup;
            public Date refDate;
            public Handle<YieldTermStructure> yts2;
            public Handle<SwaptionVolatilityStructure> swLn, swSln, swN;
            public Handle<Quote> reversion, correlation;
            public CmsCouponPricer cmsPricerLn, cmsPricerSln, cmsPricerN;
            public CmsSpreadCouponPricer cmsspPricerLn, cmsspPricerSln, cmsspPricerN;
        }
    }
}
