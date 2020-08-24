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

namespace QLCore
{
   // Armijo line search.
   //
   // (see Polak, Algorithms and consistent approximations, Optimization,
   // volume 124 of Applied Mathematical Sciences, Springer-Verlag, NY,
   // 1997)
   //
   public class ArmijoLineSearch : LineSearch
   {
      //! Default constructor
      public ArmijoLineSearch(double eps, double alpha) : this(eps, alpha, 0.65)
      {}

      public ArmijoLineSearch(double eps) : this(eps, 0.05, 0.65)
      {}

      public ArmijoLineSearch() : this(1e-8, 0.05, 0.65)
      {}

      public ArmijoLineSearch(double eps, double alpha, double beta)
         : base(eps)
      {
         alpha_ = alpha;
         beta_ = beta;
      }

      //! Perform line search
      public override double value(Problem P, ref EndCriteria.Type ecType, EndCriteria endCriteria, double t_ini)
      {
         Constraint constraint = P.constraint();
         succeed_ = true;
         bool maxIter = false;
         double qtold;
         double t = t_ini;
         int loopNumber = 0;

         double q0 = P.functionValue();
         double qp0 = P.gradientNormValue();

         qt_ = q0;
         qpt_ = (gradient_.Count == 0) ? qp0 : -Vector.DotProduct(gradient_, searchDirection_);

         // Initialize gradient
         gradient_ = new Vector(P.currentValue().Count);
         // Compute new point
         xtd_ = P.currentValue().Clone();
         t = update(ref xtd_, searchDirection_, t, constraint);
         // Compute function value at the new point
         qt_ = P.value(xtd_);

         // Enter in the loop if the criterion is not satisfied
         if ((qt_ - q0) > -alpha_ * t * qpt_)
         {
            do
            {
               loopNumber++;
               // Decrease step
               t *= beta_;
               // Store old value of the function
               qtold = qt_;
               // New point value
               xtd_ = P.currentValue();
               t = update(ref xtd_, searchDirection_, t, constraint);

               // Compute function value at the new point
               qt_ = P.value(xtd_);
               P.gradient(ref gradient_, xtd_);
               // and it squared norm
               maxIter = endCriteria.checkMaxIterations(loopNumber, ref ecType);
            }
            while ((((qt_ - q0) > (-alpha_ * t * qpt_)) || ((qtold - q0) <= (-alpha_ * t * qpt_ / beta_))) &&
                   (!maxIter));
         }

         if (maxIter)
            succeed_ = false;

         // Compute new gradient
         P.gradient(ref gradient_, xtd_);
         // and it squared norm
         qpt_ = Vector.DotProduct(gradient_, gradient_);

         // Return new step value
         return t;
      }

      private double alpha_;
      private double beta_;
   }
}
