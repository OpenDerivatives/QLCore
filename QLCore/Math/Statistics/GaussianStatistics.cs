﻿/*
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

namespace QLCore
{
   //! Statistics tool for gaussian-assumption risk measures
   /*! This class wraps a somewhat generic statistic tool and adds
       a number of gaussian risk measures (e.g.: value-at-risk, expected
       shortfall, etc.) based on the mean and variance provided by
       the underlying statistic tool.
   */
   public class GenericGaussianStatistics<Stat> : IGeneralStatistics where Stat : IGeneralStatistics, new ()
   {
      public GenericGaussianStatistics() { }
      public GenericGaussianStatistics(Stat s)
      {
         impl_ = s;
      }

      #region wrap-up Stat
      protected Stat impl_ = FastActivator<Stat>.Create();

      public int samples() { return impl_.samples(); }
      public double mean() { return impl_.mean(); }
      public double min() { return impl_.min(); }
      public double max() { return impl_.max(); }
      public double standardDeviation() { return impl_.standardDeviation(); }
      public double variance() { return impl_.variance(); }
      public double skewness() { return impl_.skewness(); }
      public double kurtosis() { return impl_.kurtosis(); }
      public double percentile(double percent) { return impl_.percentile(percent); }
      public double weightSum() { return impl_.weightSum(); }
      public double errorEstimate() { return impl_.errorEstimate(); }
      public List<KeyValuePair<double, double>> data() { return impl_.data(); }

      public void reset() { impl_.reset(); }
      public void add
         (double value, double weight) { impl_.add(value, weight); }
      public void addSequence(List<double> data, List<double> weight) { impl_.addSequence(data, weight); }

      public KeyValuePair<double, int> expectationValue(Func<KeyValuePair<double, double>, double> f,
                                                        Func<KeyValuePair<double, double>, bool> inRange)
      {
         return impl_.expectationValue(f, inRange);
      }
      #endregion


      // Gaussian risk measures
      /*! returns the downside variance
      */
      public double gaussianDownsideVariance() { return gaussianRegret(0.0); }

      /*! returns the downside deviation, defined as the square root of the downside variance. */
      public double gaussianDownsideDeviation() { return Math.Sqrt(gaussianDownsideVariance()); }

      /*! returns the variance of observations below target
          See Dembo, Freeman "The Rules Of Risk", Wiley (2001)
      */
      public double gaussianRegret(double target)
      {
         double m = this.mean();
         double std = this.standardDeviation();
         double variance = std * std;
         CumulativeNormalDistribution gIntegral = new CumulativeNormalDistribution(m, std);
         NormalDistribution g = new NormalDistribution(m, std);
         double firstTerm = variance + m * m - 2.0 * target * m + target * target;
         double alfa = gIntegral.value(target);
         double secondTerm = m - target;
         double beta = variance * g.value(target);
         double result = alfa * firstTerm - beta * secondTerm;
         return result / alfa;
      }

      /*! gaussian-assumption y-th percentile
      */
      /*! \pre percentile must be in range (0%-100%) extremes excluded */
      public double gaussianPercentile(double percentile)
      {
         Utils.QL_REQUIRE(percentile > 0.0 && percentile < 1.0, () => "percentile (" + percentile + ") must be in (0.0, 1.0)");

         InverseCumulativeNormal gInverse = new InverseCumulativeNormal(mean(), standardDeviation());
         return gInverse.value(percentile);
      }
      public double gaussianTopPercentile(double percentile) { return gaussianPercentile(1.0 - percentile); }

      //! gaussian-assumption Potential-Upside at a given percentile
      public double gaussianPotentialUpside(double percentile)
      {
         Utils.QL_REQUIRE(percentile<1.0 && percentile >= 0.9, () => "percentile (" + percentile + ") out of range [0.9, 1)");

         double result = gaussianPercentile(percentile);
         // potential upside must be a gain, i.e., floored at 0.0
         return Math.Max(result, 0.0);
      }

      //! gaussian-assumption Value-At-Risk at a given percentile
      public double gaussianValueAtRisk(double percentile)
      {
         Utils.QL_REQUIRE(percentile < 1.0 && percentile >= 0.9, () => "percentile (" + percentile + ") out of range [0.9, 1)");

         double result = gaussianPercentile(1.0 - percentile);
         // VAR must be a loss
         // this means that it has to be MIN(dist(1.0-percentile), 0.0)
         // VAR must also be a positive quantity, so -MIN(*)
         return -Math.Min(result, 0.0);
      }

      //! gaussian-assumption Expected Shortfall at a given percentile
      /*! Assuming a gaussian distribution it
          returns the expected loss in case that the loss exceeded
          a VaR threshold,

          that is the average of observations below the
          given percentile \f$ p \f$.
          Also know as conditional value-at-risk.

          See Artzner, Delbaen, Eber and Heath,
          "Coherent measures of risk", Mathematical Finance 9 (1999)
      */
      public double gaussianExpectedShortfall(double percentile)
      {
         Utils.QL_REQUIRE(percentile < 1.0 && percentile >= 0.9, () => "percentile (" + percentile + ") out of range [0.9, 1)");

         double m = this.mean();
         double std = this.standardDeviation();
         InverseCumulativeNormal gInverse = new InverseCumulativeNormal(m, std);
         double var = gInverse.value(1.0 - percentile);
         NormalDistribution g = new NormalDistribution(m, std);
         double result = m - std * std * g.value(var) / (1.0 - percentile);
         // expectedShortfall must be a loss
         // this means that it has to be MIN(result, 0.0)
         // expectedShortfall must also be a positive quantity, so -MIN(*)
         return -Math.Min(result, 0.0);
      }

      //! gaussian-assumption Shortfall (observations below target)
      public double gaussianShortfall(double target)
      {
         CumulativeNormalDistribution gIntegral = new CumulativeNormalDistribution(this.mean(), this.standardDeviation());
         return gIntegral.value(target);
      }

      //! gaussian-assumption Average Shortfall (averaged shortfallness)
      public double gaussianAverageShortfall(double target)
      {
         double m = this.mean();
         double std = this.standardDeviation();
         CumulativeNormalDistribution gIntegral = new CumulativeNormalDistribution(m, std);
         NormalDistribution g = new NormalDistribution(m, std);
         return ((target - m) + std * std * g.value(target) / gIntegral.value(target));
      }
   }

   //! default gaussian statistic tool
   public class GaussianStatistics : GenericGaussianStatistics<GeneralStatistics> { }


   //! Helper class for precomputed distributions
   public class StatsHolder : IGeneralStatistics
   {
      private double mean_, standardDeviation_;
      public double mean() { return mean_; }
      public double standardDeviation() { return standardDeviation_; }

      public StatsHolder() { } // required for generics
      public StatsHolder(double mean, double standardDeviation)
      {
         mean_ = mean;
         standardDeviation_ = standardDeviation;
      }

      #region IGeneralStatistics
      public int samples() { throw new NotSupportedException(); }
      public double min() { throw new NotSupportedException(); }
      public double max() { throw new NotSupportedException(); }
      public double variance() { throw new NotSupportedException(); }
      public double skewness() { throw new NotSupportedException(); }
      public double kurtosis() { throw new NotSupportedException(); }
      public double percentile(double percent) { throw new NotSupportedException(); }
      public double weightSum() { throw new NotSupportedException(); }
      public double errorEstimate() { throw new NotSupportedException(); }
      public List<KeyValuePair<double, double>> data() { throw new NotSupportedException(); }

      public void reset() { throw new NotSupportedException(); }
      public void add
         (double value, double weight) { throw new NotSupportedException(); }
      public void addSequence(List<double> data, List<double> weight) { throw new NotSupportedException(); }

      public KeyValuePair<double, int> expectationValue(Func<KeyValuePair<double, double>, double> f,
                                                        Func<KeyValuePair<double, double>, bool> inRange)
      {
         throw new NotSupportedException();
      }
      #endregion
   }
}
