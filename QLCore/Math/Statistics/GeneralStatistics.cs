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
using System.Linq;

namespace QLCore
{
   public interface IGeneralStatistics
   {
      int samples();
      double mean();
      double min();
      double max();
      double standardDeviation();
      double variance();
      double skewness();
      double kurtosis();
      double percentile(double percent);
      double weightSum();
      double errorEstimate();
      List<KeyValuePair<double, double>> data();

      void reset();
      void add
         (double value, double weight);
      void addSequence(List<double> data, List<double> weight);

      KeyValuePair<double, int> expectationValue(Func<KeyValuePair<double, double>, double> f,
                                                 Func<KeyValuePair<double, double>, bool> inRange);
   }

   //! Statistics tool
   /*! This class accumulates a set of data and returns their
       statistics (e.g: mean, variance, skewness, kurtosis,
       error estimation, percentile, etc.) based on the empirical
       distribution (no gaussian assumption)

       It doesn't suffer the numerical instability problem of
       IncrementalStatistics. The downside is that it stores all
       samples, thus increasing the memory requirements.
   */
   public class GeneralStatistics : IGeneralStatistics
   {
      private List<KeyValuePair<double, double>> samples_;
      //! number of samples collected
      public int samples() { return samples_.Count; }
      //! collected data
      public List<KeyValuePair<double, double>> data() { return samples_; }

      private bool sorted_;
      private double? mean_ = null, weightSum_ = null, variance_ = null, skewness_ = null, kurtosis_ = null;


      public GeneralStatistics() { reset(); }


      /*! returns the error estimate on the mean value, defined as
          \f$ \epsilon = \sigma/\sqrt{N}. \f$ */
      public double errorEstimate() { return Math.Sqrt(variance() / samples()); }

      /*! returns the minimum sample value */
      public double min()
      {
         Utils.QL_REQUIRE(samples() > 0, () => "empty sample set");
         return samples_.Min(x => x.Key);
      }

      /*! returns the maximum sample value */
      public double max()
      {
         Utils.QL_REQUIRE(samples() > 0, () => "empty sample set");
         return samples_.Max(x => x.Key);
      }


      //! adds a datum to the set, possibly with a weight
      public void add
         (double value) { add(value, 1); }
      public void add
         (double value, double weight)
      {
         Utils.QL_REQUIRE(weight >= 0.0, () => "negative weight not allowed");
         samples_.Add(new KeyValuePair<double, double>(value, weight));

         sorted_ = false;
         mean_ = weightSum_ = variance_ = skewness_ = kurtosis_ = null;
      }

      //! resets the data to a null set
      public void reset()
      {
         samples_ = new List<KeyValuePair<double, double>>();

         sorted_ = true;
         mean_ = weightSum_ = variance_ = skewness_ = kurtosis_ = null;
      }

      //! sort the data set in increasing order
      public void sort()
      {
         if (!sorted_)
         {
            samples_.Sort((x, y) => x.Key.CompareTo(y.Key));
            sorted_ = true;
         }
      }


      //! sum of data weights
      public double weightSum()
      {
         if (weightSum_ == null)
            weightSum_ = samples_.Sum<KeyValuePair<double, double>>(x => x.Value);
         return weightSum_.GetValueOrDefault();
      }

      /*! returns the mean, defined as
          \f[ \langle x \rangle = \frac{\sum w_i x_i}{\sum w_i}. \f] */
      public double mean()
      {
         if (mean_ == null)
         {
            int N = samples();
            Utils.QL_REQUIRE(samples() > 0, () => "empty sample set");
            // eat our own dog food
            mean_ = expectationValue(x => x.Key * x.Value, x => true).Key;
         }
         return mean_.GetValueOrDefault();
      }

      /*! returns the standard deviation \f$ \sigma \f$, defined as the
      square root of the variance. */
      public double standardDeviation() { return Math.Sqrt(variance()); }

      /*! returns the variance, defined as
          \f[ \sigma^2 = \frac{N}{N-1} \left\langle \left(
              x-\langle x \rangle \right)^2 \right\rangle. \f] */
      public double variance()
      {
         if (variance_ == null)
         {
            int N = samples();
            Utils.QL_REQUIRE(N > 1, () => "sample number <=1, unsufficient");
            // Subtract the mean and square. Repeat on the whole range.
            // Hopefully, the whole thing will be inlined in a single loop.
            double s2 = expectationValue(x => Math.Pow(x.Key * x.Value - mean(), 2), x => true).Key;
            variance_ = s2 * N / (N - 1.0);
         }
         return variance_.GetValueOrDefault();
      }

      /*! returns the skewness, defined as
          \f[ \frac{N^2}{(N-1)(N-2)} \frac{\left\langle \left(
              x-\langle x \rangle \right)^3 \right\rangle}{\sigma^3}. \f]
          The above evaluates to 0 for a Gaussian distribution.
      */
      public double skewness()
      {
         if (skewness_ == null)
         {
            int N = samples();
            Utils.QL_REQUIRE(N > 2, () => "sample number <=2, unsufficient");

            double x = expectationValue(y => Math.Pow(y.Key * y.Value - mean(), 3), y => true).Key;
            double sigma = standardDeviation();

            skewness_ = (x / Math.Pow(sigma, 3)) * (N / (N - 1.0)) * (N / (N - 2.0));
         }
         return skewness_.GetValueOrDefault();
      }

      /*! returns the excess kurtosis
          The above evaluates to 0 for a Gaussian distribution.
      */
      public double kurtosis()
      {
         if (kurtosis_ == null)
         {
            int N = samples();
            Utils.QL_REQUIRE(N > 3, () => "sample number <=3, unsufficient");

            double x = expectationValue(y => Math.Pow(y.Key * y.Value - mean(), 4), y => true).Key;
            double sigma2 = variance();

            double c1 = (N / (N - 1.0)) * (N / (N - 2.0)) * ((N + 1.0) / (N - 3.0));
            double c2 = 3.0 * ((N - 1.0) / (N - 2.0)) * ((N - 1.0) / (N - 3.0));

            kurtosis_ = c1 * (x / (sigma2 * sigma2)) - c2;
         }
         return kurtosis_.GetValueOrDefault();
      }

      /*! Expectation value of a function \f$ f \f$ on a given range \f$ \mathcal{R} \f$, i.e.,

          The range is passed as a boolean function returning
          <tt>true</tt> if the argument belongs to the range
          or <tt>false</tt> otherwise.

          The function returns a pair made of the result and the number of observations in the given range. */
      public KeyValuePair<double, int> expectationValue(Func<KeyValuePair<double, double>, double> f,
                                                        Func<KeyValuePair<double, double>, bool> inRange)
      {
         double num = 0.0, den = 0.0;
         int N = 0;

         foreach (KeyValuePair<double, double> x in samples_.Where<KeyValuePair<double, double>>(x => inRange(x)))
         {
            num += f(x) * x.Value;
            den += x.Value;
            N += 1;
         }

         if (N == 0)
            return new KeyValuePair<double, int>(0, 0);
         return new KeyValuePair<double, int>(num / den, N);
      }

      /*! \f$ y \f$-th percentile, defined as the value \f$ \bar{x} \f$
          \pre \f$ y \f$ must be in the range \f$ (0-1]. \f$
      */
      public double percentile(double percent)
      {

         Utils.QL_REQUIRE(percent > 0.0 && percent <= 1.0, () => "percentile (" + percent + ") must be in (0.0, 1.0]");

         double sampleWeight = weightSum();
         Utils.QL_REQUIRE(sampleWeight > 0, () => "empty sample set");

         sort();

         double integral = 0, target = percent * sampleWeight;
         int pos = samples_.Count(x => { integral += x.Value; return integral < target; });
         return samples_[pos].Key;
      }

      /*! \f$ y \f$-th top percentile, defined as the value
          \pre \f$ y \f$ must be in the range \f$ (0-1]. \f$
      */
      public double topPercentile(double percent)
      {
         Utils.QL_REQUIRE(percent > 0.0 && percent <= 1.0, () => "percentile (" + percent + ") must be in (0.0, 1.0]");

         double sampleWeight = weightSum();
         Utils.QL_REQUIRE(sampleWeight > 0, () => "empty sample set");

         sort();

         double integral = 0, target = 1 - percent * sampleWeight;
         int pos = samples_.Count(x => { integral += x.Value; return integral < target; });
         return samples_[pos].Key;
      }

      //! adds a sequence of data to the set, with default weight
      public void addSequence(List<double> list)
      {
         foreach (double v in list)
            add
               (v, 1);
      }
      //! adds a sequence of data to the set, each with its weight
      public void addSequence(List<double> data, List<double> weight)
      {
         for (int i = 0; i < data.Count; i++)
            add
               (data[i], weight[i]);
      }
   }
}
