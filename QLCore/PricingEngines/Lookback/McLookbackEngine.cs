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
using System.Linq;

namespace QLCore
{
    //! Lookback option pricing engine using Monte Carlo simulation
    /*!
        \test the correctness of the returned value is tested by
              checking it against analytic results.
    */
    public class MCLookbackEngine<A, R, RNG, S> : McSimulation<SingleVariate, RNG, S>, IGenericEngine
      where A : IPricingEngineArguments, new()
      where R : IPricingEngineResults, new()
      where RNG : IRSG, new()
      where S : IGeneralStatistics, new()
    {
        protected StochasticProcess process_;
        protected int? timeSteps_, timeStepsPerYear_;
        protected int? requiredSamples_, maxSamples_;
        protected double? requiredTolerance_;
        protected bool brownianBridge_;
        protected ulong seed_;
        // constructor
        public MCLookbackEngine(GeneralizedBlackScholesProcess process,
                                 int? timeSteps,
                                 int? timeStepsPerYear,
                                 bool brownianBridge,
                                 bool antitheticVariate,
                                 int? requiredSamples,
                                 double? requiredTolerance,
                                 int? maxSamples,
                                 ulong seed)
           : base(antitheticVariate, false)
        {
            process_ = process;
            timeSteps_ = timeSteps;
            timeStepsPerYear_ = timeStepsPerYear;
            requiredSamples_ = requiredSamples;
            maxSamples_ = maxSamples;
            requiredTolerance_ = requiredTolerance;
            brownianBridge_ = brownianBridge;
            seed_ = seed;

            Utils.QL_REQUIRE(timeSteps != null || timeStepsPerYear != null, () => "no time steps provided");
            Utils.QL_REQUIRE(timeSteps == null || timeStepsPerYear == null, () =>
                      "both time steps and time steps per year were provided");
            if (timeSteps != null)
                Utils.QL_REQUIRE(timeSteps > 0, () => "timeSteps must be positive, " + timeSteps + " not allowed");
            if (timeStepsPerYear != null)
                Utils.QL_REQUIRE(timeStepsPerYear > 0, () =>
               "timeStepsPerYear must be positive, " + timeStepsPerYear + " not allowed");

            process_.registerWith(update);
        }


        public virtual void calculate()
        {
            base.calculate(requiredTolerance_, requiredSamples_, maxSamples_);
            (results_ as OneAssetOption.Results).value = mcModel_.sampleAccumulator().mean();
            if (FastActivator<RNG>.Create().allowsErrorEstimate != 0)
                (results_ as OneAssetOption.Results).errorEstimate = mcModel_.sampleAccumulator().errorEstimate();
        }

        // McSimulation implementation
        protected override TimeGrid timeGrid()
        {
            Date lastExerciseDate = (arguments_ as OneAssetOption.Arguments).exercise.lastDate();
            double t = process_.time(lastExerciseDate);
            if (timeSteps_ != null)
            {
                return new TimeGrid(t, timeSteps_.Value);
            }
            else if (timeStepsPerYear_ != null)
            {
                int steps = (int)(timeStepsPerYear_ * t);
                return new TimeGrid(t, Math.Max(steps, 1));
            }
            else
            {
                Utils.QL_FAIL("time steps not specified");
                return null;
            }
        }

        protected override IPathGenerator<IRNG> pathGenerator()
        {
            int dimensions = process_.factors();
            TimeGrid grid = timeGrid();
            IRNG generator = (IRNG)FastActivator<RNG>.Create().make_sequence_generator(dimensions * (grid.size() - 1), seed_);
            return new PathGenerator<IRNG>(process_, grid, generator, brownianBridge_);
        }

        #region PricingEngine
        protected A arguments_ = FastActivator<A>.Create();
        protected R results_ = FastActivator<R>.Create();

        public IPricingEngineArguments getArguments() { return arguments_; }
        public IPricingEngineResults getResults() { return results_; }
        public void reset() { results_.reset(); }

        #region Observer & Observable
        // observable interface
        private readonly WeakEventSource eventSource = new WeakEventSource();
        public event Callback notifyObserversEvent
        {
            add { eventSource.Subscribe(value); }
            remove { eventSource.Unsubscribe(value); }
        }

        public void registerWith(Callback handler) { notifyObserversEvent += handler; }
        public void unregisterWith(Callback handler) { notifyObserversEvent -= handler; }
        protected void notifyObservers()
        {
            eventSource.Raise();
        }

        public void update() { notifyObservers(); }
        #endregion
        #endregion

        protected override PathPricer<IPath> pathPricer()
        {
            TimeGrid grid = this.timeGrid();
            double discount = (this.process_ as GeneralizedBlackScholesProcess)
                                .riskFreeRate().currentLink().discount(grid.Last());

            ContinuousFixedLookbackOption.Arguments arg1 = this.arguments_ as ContinuousFixedLookbackOption.Arguments;
            ContinuousPartialFixedLookbackOption.Arguments arg2 = this.arguments_ as ContinuousPartialFixedLookbackOption.Arguments;
            ContinuousFloatingLookbackOption.Arguments arg3 = this.arguments_ as ContinuousFloatingLookbackOption.Arguments;
            ContinuousPartialFloatingLookbackOption.Arguments arg4 = this.arguments_ as ContinuousPartialFloatingLookbackOption.Arguments;

            if (arg2 != null)
                return mc_looback_path_pricer(arg2, this.process_ as GeneralizedBlackScholesProcess, discount);
            else if (arg1 != null)
                return mc_looback_path_pricer(arg1, this.process_ as GeneralizedBlackScholesProcess, discount);
            else if (arg4 != null)
                return mc_looback_path_pricer(arg4, this.process_ as GeneralizedBlackScholesProcess, discount);
            else if (arg3 != null)
                return mc_looback_path_pricer(arg3, this.process_ as GeneralizedBlackScholesProcess, discount);
            else
                return null;
        }

        protected PathPricer<IPath> mc_looback_path_pricer(ContinuousFixedLookbackOption.Arguments args,
                                                          GeneralizedBlackScholesProcess process,
                                                          double discount)
        {
            PlainVanillaPayoff payoff = args.payoff as PlainVanillaPayoff;
            Utils.QL_REQUIRE(payoff != null, () => "non-plain payoff given");


            return new LookbackFixedPathPricer(payoff.optionType(),
                                               payoff.strike(),
                                               discount);
        }

        protected PathPricer<IPath> mc_looback_path_pricer(ContinuousPartialFixedLookbackOption.Arguments args,
                                                          GeneralizedBlackScholesProcess process,
                                                          double discount)
        {
            PlainVanillaPayoff payoff = args.payoff as PlainVanillaPayoff;
            Utils.QL_REQUIRE(payoff != null, () => "non-plain payoff given");

            double lookbackStart = process.time(args.lookbackPeriodStart);

            return new LookbackPartialFixedPathPricer(lookbackStart,
                                                    payoff.optionType(),
                                                    payoff.strike(),
                                                    discount);
        }

        protected PathPricer<IPath> mc_looback_path_pricer(ContinuousFloatingLookbackOption.Arguments args,
                                                          GeneralizedBlackScholesProcess process,
                                                          double discount)
        {
            FloatingTypePayoff payoff = args.payoff as FloatingTypePayoff;
            Utils.QL_REQUIRE(payoff != null, () => "non-floating  payoff given");

            return new LookbackFloatingPathPricer(payoff.optionType(),
                                                  discount);
        }

        protected PathPricer<IPath> mc_looback_path_pricer(ContinuousPartialFloatingLookbackOption.Arguments args,
                                                           GeneralizedBlackScholesProcess process,
                                                           double discount)
        {
            FloatingTypePayoff payoff = args.payoff as FloatingTypePayoff;
            Utils.QL_REQUIRE(payoff != null, () => "non-floating  payoff given");

            double lookbackEnd = process.time(args.lookbackPeriodEnd);

            return new LookbackPartialFloatingPathPricer(lookbackEnd,
                                                         payoff.optionType(),
                                                         discount);
        }
    }


    //! Monte Carlo Lookback engine factory
    // template <class RNG = PseudoRandom, class S = Statistics>
    public class MakeMCLookbackEngine<A, R, RNG> : MakeMCLookbackEngine<A, R, RNG, Statistics>
        where A : IPricingEngineArguments, new()
        where R : IPricingEngineResults, new()
        where RNG : IRSG, new()
    {
        public MakeMCLookbackEngine(GeneralizedBlackScholesProcess process) : base(process) { }
    }

    public class MakeMCLookbackEngine<A, R, RNG, S>
       where A : IPricingEngineArguments, new()
       where R : IPricingEngineResults, new()
       where RNG : IRSG, new()
       where S : IGeneralStatistics, new()
    {

        public MakeMCLookbackEngine(GeneralizedBlackScholesProcess process)
        {
            process_ = process;
            antithetic_ = false;
            steps_ = null;
            stepsPerYear_ = null;
            samples_ = null;
            maxSamples_ = null;
            tolerance_ = null;
            brownianBridge_ = false;
            seed_ = 0;
        }

        // named parameters
        public MakeMCLookbackEngine<A, R, RNG, S> withSteps(int steps)
        {
            steps_ = steps;
            return this;
        }
        public MakeMCLookbackEngine<A, R, RNG, S> withStepsPerYear(int steps)
        {
            stepsPerYear_ = steps;
            return this;
        }
        public MakeMCLookbackEngine<A, R, RNG, S> withSamples(int samples)
        {
            Utils.QL_REQUIRE(tolerance_ == null, () => "tolerance already set");
            samples_ = samples;
            return this;
        }
        public MakeMCLookbackEngine<A, R, RNG, S> withAbsoluteTolerance(double tolerance)
        {
            Utils.QL_REQUIRE(samples_ == null, () => "number of samples already set");
            Utils.QL_REQUIRE(FastActivator<RNG>.Create().allowsErrorEstimate != 0, () =>
              "chosen random generator policy does not allow an error estimate");
            tolerance_ = tolerance;
            return this;
        }
        public MakeMCLookbackEngine<A, R, RNG, S> withMaxSamples(int samples)
        {
            maxSamples_ = samples;
            return this;
        }
        public MakeMCLookbackEngine<A, R, RNG, S> withSeed(ulong seed)
        {
            seed_ = seed;
            return this;
        }
        public MakeMCLookbackEngine<A, R, RNG, S> withBrownianBridge(bool brownianBridge = true)
        {
            brownianBridge_ = brownianBridge;
            return this;
        }
        public MakeMCLookbackEngine<A, R, RNG, S> withAntitheticVariate(bool b = true)
        {
            antithetic_ = b;
            return this;
        }

        // conversion to pricing engine
        public IPricingEngine value()
        {
            Utils.QL_REQUIRE(steps_ != null || stepsPerYear_ != null, () => "number of steps not given");
            Utils.QL_REQUIRE(steps_ == null || stepsPerYear_ == null, () => "number of steps overspecified");
            return new MCLookbackEngine<A, R, RNG, S>(process_, steps_, stepsPerYear_, brownianBridge_, antithetic_,
                                               samples_, tolerance_, maxSamples_, seed_);
        }

        private GeneralizedBlackScholesProcess process_;
        private bool antithetic_;
        private int? steps_, stepsPerYear_, samples_, maxSamples_;
        private double? tolerance_;
        private bool brownianBridge_;
        private ulong seed_;
    }


    public class LookbackFixedPathPricer : PathPricer<IPath>
    {
        public LookbackFixedPathPricer(Option.Type type,
                                       double strike,
                                       double discount)
        {
            payoff_ = new PlainVanillaPayoff(type, strike);
            discount_ = discount;

            Utils.QL_REQUIRE(strike >= 0.0,
                             () => "strike less than zero not allowed");

        }
        public double value(IPath path)
        {
            Utils.QL_REQUIRE(!(path as Path).empty(), () => "the path cannot be empty");

            double underlying;
            switch (payoff_.optionType())
            {
                case Option.Type.Put:
                    underlying = (path as Path).values().Min();
                    break;
                case Option.Type.Call:
                    underlying = (path as Path).values().Max();
                    break;
                default:
                    underlying = 0.0;
                    Utils.QL_FAIL("unknown option type");
                    break;
            }

            return payoff_.value(underlying) * discount_;
        }

        protected PlainVanillaPayoff payoff_;
        protected double discount_;
    }

    public class LookbackPartialFixedPathPricer : PathPricer<IPath>
    {
        public LookbackPartialFixedPathPricer(double lookbackStart,
                                         Option.Type type,
                                         double strike,
                                         double discount)
        {
            lookbackStart_ = lookbackStart;
            payoff_ = new PlainVanillaPayoff(type, strike);
            discount_ = discount;

            Utils.QL_REQUIRE(strike >= 0.0,
                             () => "strike less than zero not allowed");
        }
        public double value(IPath path)
        {
            Utils.QL_REQUIRE(!(path as Path).empty(), () => "the path cannot be empty");

            TimeGrid doubleGrid = (path as Path).timeGrid();
            int startIndex = doubleGrid.closestIndex(lookbackStart_);
            double underlying;
            switch (payoff_.optionType())
            {
                case Option.Type.Put:
                    underlying = (path as Path).values().GetRange(startIndex, (path as Path).values().Count - startIndex).Min();
                    break;
                case Option.Type.Call:
                    underlying = (path as Path).values().GetRange(startIndex, (path as Path).values().Count - startIndex).Max();
                    break;
                default:
                    underlying = 0.0;
                    Utils.QL_FAIL("unknown option type");
                    break;
            }

            return payoff_.value(underlying) * discount_;
        }

        protected double lookbackStart_;
        protected PlainVanillaPayoff payoff_;
        protected double discount_;
    }

    public class LookbackFloatingPathPricer : PathPricer<IPath>
    {
        public LookbackFloatingPathPricer(Option.Type type,
                                          double discount)
        {
            payoff_ = new FloatingTypePayoff(type);
            discount_ = discount;
        }

        public double value(IPath path)
        {
            Utils.QL_REQUIRE(!(path as Path).empty(), () => "the path cannot be empty");

            double strike;
            double terminalPrice = (path as Path).back();
            switch (payoff_.optionType())
            {
                case Option.Type.Call:
                    strike = (path as Path).values().Min();
                    break;
                case Option.Type.Put:
                    strike = (path as Path).values().Max();
                    break;
                default:
                    strike = 0.0;
                    Utils.QL_FAIL("unknown option type");
                    break;
            }

            return payoff_.value(terminalPrice, strike) * discount_;
        }

        protected FloatingTypePayoff payoff_;
        protected double discount_;
    }

    public class LookbackPartialFloatingPathPricer : PathPricer<IPath>
    {
        public LookbackPartialFloatingPathPricer(double lookbackEnd,
                                                 Option.Type type,
                                                 double discount)
        {
            lookbackEnd_ = lookbackEnd;
            payoff_ = new FloatingTypePayoff(type);
            discount_ = discount;
        }

        public double value(IPath path)
        {
            Utils.QL_REQUIRE(!(path as Path).empty(), () => "the path cannot be empty");

            TimeGrid doubleGrid = (path as Path).timeGrid();
            int endIndex = doubleGrid.closestIndex(lookbackEnd_);
            double terminalPrice = (path as Path).back();
            double strike;

            switch (payoff_.optionType())
            {
                case Option.Type.Call:
                    strike = (path as Path).values().GetRange(0, endIndex).Min();
                    break;
                case Option.Type.Put:
                    strike = (path as Path).values().GetRange(0, endIndex).Max();
                    break;
                default:
                    strike = 0.0;
                    Utils.QL_FAIL("unknown option type");
                    break;
            }

            return payoff_.value(terminalPrice, strike) * discount_;
        }

        protected double lookbackEnd_;
        protected FloatingTypePayoff payoff_;
        protected double discount_;
    }
}