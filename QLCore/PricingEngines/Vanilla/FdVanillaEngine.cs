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

namespace QLCore
{
   //! Finite-differences pricing engine for BSM one asset options
   /*! The name is a misnomer as this is a base class for any finite difference scheme.  Its main job is to handle grid layout.

       \ingroup vanillaengines
   */
   public class FDVanillaEngine
   {
      protected GeneralizedBlackScholesProcess process_;
      protected int timeSteps_, gridPoints_;
      protected bool timeDependent_;
      protected Date exerciseDate_;
      protected Payoff payoff_;
      protected TridiagonalOperator finiteDifferenceOperator_;
      public SampledCurve intrinsicValues_ { get; set; }

      protected List<BoundaryCondition<IOperator>> BCs_;
      // temporaries
      protected double sMin_, center_, sMax_;

      // temporaries
      const double safetyZoneFactor_ = 1.1;

      // required for generics and template iheritance
      public FDVanillaEngine() { }
      // this should be defined as new in each deriving class which use template iheritance
      // in order to return a proper class to wrap
      public virtual FDVanillaEngine factory(GeneralizedBlackScholesProcess process,
                                             int timeSteps, int gridPoints, bool timeDependent)
      {
         return new FDVanillaEngine(process, timeSteps, gridPoints, timeDependent);
      }

      public FDVanillaEngine(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent)
      {
         process_ = process;
         timeSteps_ = timeSteps;
         gridPoints_ = gridPoints;
         timeDependent_ = timeDependent;
         intrinsicValues_ = new SampledCurve(gridPoints);
         BCs_ = new InitializedList<BoundaryCondition<IOperator>>(2);
      }

      public Vector grid() { return intrinsicValues_.grid(); }

      protected virtual void setGridLimits()
      {
         setGridLimits(process_.stateVariable().link.value(), getResidualTime());
         ensureStrikeInGrid();
      }

      protected void setGridLimits(double center, double t)
      {
         Utils.QL_REQUIRE(center > 0.0, () => "negative or null underlying given");
         Utils.QL_REQUIRE(t > 0.0, () => "negative or zero residual time");
         center_ = center;
         int newGridPoints = safeGridPoints(gridPoints_, t);
         if (newGridPoints > intrinsicValues_.size())
         {
            intrinsicValues_ = new SampledCurve(newGridPoints);
         }

         double volSqrtTime = Math.Sqrt(process_.blackVolatility().link.blackVariance(t, center_));

         // the prefactor fine tunes performance at small volatilities
         double prefactor = 1.0 + 0.02 / volSqrtTime;
         double minMaxFactor = Math.Exp(4.0 * prefactor * volSqrtTime);
         sMin_ = center_ / minMaxFactor; // underlying grid min value
         sMax_ = center_ * minMaxFactor; // underlying grid max value
      }

      public void ensureStrikeInGrid()
      {
         // ensure strike is included in the grid
         StrikedTypePayoff striked_payoff = payoff_ as StrikedTypePayoff;
         if (striked_payoff == null)
            return;

         double requiredGridValue = striked_payoff.strike();

         if (sMin_ > requiredGridValue / safetyZoneFactor_)
         {
            sMin_ = requiredGridValue / safetyZoneFactor_;
            // enforce central placement of the underlying
            sMax_ = center_ / (sMin_ / center_);
         }
         if (sMax_ < requiredGridValue * safetyZoneFactor_)
         {
            sMax_ = requiredGridValue * safetyZoneFactor_;
            // enforce central placement of the underlying
            sMin_ = center_ / (sMax_ / center_);
         }
      }

      protected void initializeInitialCondition()
      {
         intrinsicValues_.setLogGrid(sMin_, sMax_);
         intrinsicValues_.sample(payoff_.value);
      }

      protected void initializeOperator()
      {
         finiteDifferenceOperator_ = OperatorFactory.getOperator(process_, intrinsicValues_.grid(),
                                                                 getResidualTime(), timeDependent_);
      }

      protected void initializeBoundaryConditions()
      {
         BCs_[0] = new NeumannBC(intrinsicValues_.value(1) - intrinsicValues_.value(0), NeumannBC.Side.Lower);
         BCs_[1] = new NeumannBC(intrinsicValues_.value(intrinsicValues_.size() - 1) -
                                 intrinsicValues_.value(intrinsicValues_.size() - 2),
                                 NeumannBC.Side.Upper);
      }

      public double getResidualTime()
      {
         return process_.time(exerciseDate_);
      }

      // safety check to be sure we have enough grid points.
      private int safeGridPoints(int gridPoints, double residualTime)
      {
         const int minGridPoints = 10;
         const int minGridPointsPerYear = 2;
         return Math.Max(gridPoints,
                         residualTime > 1 ?
                         (int)(minGridPoints + (residualTime - 1.0) * minGridPointsPerYear)
                         : minGridPoints);
      }

      public virtual void setupArguments(IPricingEngineArguments a)
      {
         OneAssetOption.Arguments args = a as OneAssetOption.Arguments;
         Utils.QL_REQUIRE(args != null, () => "incorrect argument type");

         exerciseDate_ = args.exercise.lastDate();
         payoff_ = args.payoff;
      }
      public virtual void calculate(IPricingEngineResults r) { throw new NotSupportedException(); }
      public virtual void update()
      {
         if (process_ != null)
            process_.update();
      }
   }


   // this is the interface to allow generic use of FDAmericanEngine and FDShoutEngine
   // those engines are shortcuts to FDEngineAdapter
   public interface IFDEngine : IPricingEngine
   {
      IFDEngine factory(GeneralizedBlackScholesProcess process, int timeSteps = 100, int gridPoints = 100);
   }

   public class FDEngineAdapter<Base, Engine> : FDVanillaEngine, IGenericEngine
      where Base : FDConditionEngineTemplate, new ()
      where Engine : IGenericEngine, new ()
   {

      // a wrap-up of base engine
      Base optionBase;

      // required for generics
      public FDEngineAdapter() { }

      //public FDEngineAdapter(GeneralizedBlackScholesProcess process, Size timeSteps=100, Size gridPoints=100, bool timeDependent = false)
      public FDEngineAdapter(GeneralizedBlackScholesProcess process, int timeSteps, int gridPoints, bool timeDependent)
      {
         optionBase = (Base)FastActivator<Base>.Create().factory(process, timeSteps, gridPoints, timeDependent);
      }

      public void calculate()
      {
         optionBase.setupArguments(getArguments());
         optionBase.calculate(getResults());
      }


      #region IGenericEngine wrap-up
      // we do not need to register with the wrapped engine because all we need is containers for parameters and results
      protected IGenericEngine engine_ = FastActivator<Engine>.Create();

      public IPricingEngineArguments getArguments() { return engine_.getArguments(); }
      public IPricingEngineResults getResults() { return engine_.getResults(); }
      public void reset() { engine_.reset(); }
      public override void update()
      {
         optionBase.update();
         base.update();
      }
      #endregion
   }
}
