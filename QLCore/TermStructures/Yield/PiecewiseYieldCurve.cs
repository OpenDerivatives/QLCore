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

namespace QLCore
{
   // this is an abstract class to give access to all properties and methods of PiecewiseYieldCurve and avoiding generics
   public class PiecewiseYieldCurve : YieldTermStructure, Curve<YieldTermStructure>
   {
      # region new fields: Curve

      public double initialValue() { return _traits_.initialValue(this); }
      public Date initialDate() { return _traits_.initialDate(this); }
      public void registerWith(BootstrapHelper<YieldTermStructure> helper)
      {
      }
      public new bool moving_
      {
         get
         {
            return base.moving_;
         }
         set
         {
            base.moving_ = value;
         }
      }
      public void setTermStructure(BootstrapHelper<YieldTermStructure> helper)
      {
         helper.setTermStructure(this);
      }
      protected ITraits<YieldTermStructure> _traits_ = null;//todo define with the trait for yield curve
      public ITraits<YieldTermStructure> traits_
      {
         get
         {
            return _traits_;
         }
      }
      public double minValueAfter<C>( int i, C c, bool validData, int first ) where C : Curve<YieldTermStructure> { return traits_.minValueAfter( i, c, validData, first ); }
      public double maxValueAfter<C>(int i, C c, bool validData, int first) where C : Curve<YieldTermStructure> { return traits_.maxValueAfter(i, c, validData, first); }
		public double guess<C>( int i, C c, bool validData, int first ) where C : Curve<YieldTermStructure> { return traits_.guess( i, c, validData, first ); }

      # endregion

      #region InterpolatedCurve
      public List<double> times_
      {
          get { return (base_curve as InterpolatedCurve).times(); }
          set { (base_curve as InterpolatedCurve).times_ = value; }
      }
      public List<double> times() { calculate(); return (base_curve as InterpolatedCurve).times(); }

      public List<Date> dates_
      {
          get { return (base_curve as InterpolatedCurve).dates(); }
          set { (base_curve as InterpolatedCurve).dates_ = value; }
      }

      public List<Date> dates() { calculate(); return (base_curve as InterpolatedCurve).dates(); }

      public Date maxDate_
      {
          get { return (base_curve as InterpolatedCurve).maxDate(); }
          set { (base_curve as InterpolatedCurve).maxDate_ = value; }
      }

      public override Date maxDate()
      {
         calculate();
         return (base_curve as InterpolatedCurve).maxDate();
      }

      public List<double> data_
      {
          get { return (base_curve as InterpolatedCurve).data(); }
          set { (base_curve as InterpolatedCurve).data_ = value; }
      }

      public List<double> data() { calculate(); return (base_curve as InterpolatedCurve).data(); }

      public Interpolation interpolation_
      {
          get { return (base_curve as InterpolatedCurve).interpolation_; }
          set { (base_curve as InterpolatedCurve).interpolation_ = value; }
      }
      public IInterpolationFactory interpolator_
      {
          get 
          {
              if (base_curve != null)
                  return (base_curve as InterpolatedCurve).interpolator_;

              else
                  return null;
          }
          set 
          { 
              if (base_curve != null) 
                (base_curve as InterpolatedCurve).interpolator_ = value; 
          }
      }
      
      public Dictionary<Date, double> nodes()
      {
         calculate();
         return (base_curve as InterpolatedCurve).nodes();
      }

      public void setupInterpolation()
      {
         (base_curve as InterpolatedCurve).setupInterpolation();
      }

      public object Clone()
      {
         InterpolatedCurve copy = this.MemberwiseClone() as InterpolatedCurve;
         copy.times_ = new List<double>(times_);
         copy.data_ = new List<double>(data_);
         copy.interpolator_ = interpolator_;
         copy.setupInterpolation();
         (copy as PiecewiseYieldCurve).base_curve = (base_curve as InterpolatedCurve).Clone() as YieldTermStructure;
         return copy;
      }
      #endregion

      #region BootstrapTraits

      public Date initialDate(YieldTermStructure c) { return traits_.initialDate(c); }
      public double initialValue(YieldTermStructure c) { return traits_.initialValue(c); }
      public void updateGuess(List<double> data, double discount, int i) { traits_.updateGuess(data, discount, i); }
      public int maxIterations() { return traits_.maxIterations(); }

      #endregion

      #region Properties

      protected double _accuracy_;
      public double accuracy_
      {
         get
         {
            return _accuracy_;
         }
         set
         {
            _accuracy_ = value;
         }
      }

      protected List<RateHelper> _instruments_ = new List<RateHelper>();
      public List<BootstrapHelper<YieldTermStructure>> instruments_
      {
         get
         {
            //todo edem
            List<BootstrapHelper<YieldTermStructure>> instruments = new List<BootstrapHelper<YieldTermStructure>>();
            _instruments_.ForEach((i, x) => instruments.Add(x));
            return instruments;
         }
      }

      protected IBootStrap<PiecewiseYieldCurve> bootstrap_;
      protected YieldTermStructure base_curve;

      #endregion

      protected internal override double discountImpl(double t)
      {
         calculate();
         return base_curve.discountImpl(t);
      }

      //for iterative and local bootstrap
      public PiecewiseYieldCurve() : base(new Settings()) {}

      // two constructors to forward down the ctor chain
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, Calendar cal, DayCounter dc,
                                 List<Handle<Quote>> jumps = null, List<Date> jumpDates = null) 
         : base(settings, referenceDate, cal, dc, jumps, jumpDates)
      {}
      public PiecewiseYieldCurve(Settings settings, int settlementDays, Calendar cal, DayCounter dc,
                                 List<Handle<Quote>> jumps = null, List<Date> jumpDates = null)
         : base(settings, settlementDays, cal, dc, jumps, jumpDates)
      {}
   }

   public class PiecewiseYieldCurve<Traits, Interpolator, BootStrap> : PiecewiseYieldCurve
      where Traits : ITraits<YieldTermStructure>, new()
      where Interpolator : class, IInterpolationFactory, new()
      where BootStrap : IBootStrap<PiecewiseYieldCurve>, new()
   {

      #region Constructors
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments, DayCounter dayCounter)
         : this(settings, referenceDate, instruments, dayCounter, new List<Handle<Quote>>(), new List<Date>(),
                1.0e-12, FastActivator<Interpolator>.Create(), FastActivator<BootStrap>.Create()) { }
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps, List<Date> jumpDates)
         : this(settings, referenceDate, instruments, dayCounter, jumps, jumpDates, 1.0e-12, FastActivator<Interpolator>.Create(),
                FastActivator<BootStrap>.Create()) { }
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps,
                                 List<Date> jumpDates, double accuracy)
         : this(settings, referenceDate, instruments, dayCounter, jumps, jumpDates, accuracy, FastActivator<Interpolator>.Create(),
                FastActivator<BootStrap>.Create()) { }
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps,
                                 List<Date> jumpDates, double accuracy, Interpolator i)
         : this(settings, referenceDate, instruments, dayCounter, jumps, jumpDates, accuracy, i, FastActivator<BootStrap>.Create()) { }
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps, List<Date> jumpDates,
                                 double accuracy, Interpolator i, BootStrap bootstrap)
         : base(settings, referenceDate, new Calendar(), dayCounter, jumps, jumpDates)
      {

         _traits_ = FastActivator<Traits>.Create();
         base_curve = _traits_.factory<Interpolator>(this.settings(), referenceDate, dayCounter, jumps, jumpDates, i);
         _instruments_ = instruments;
         accuracy_ = accuracy;
         interpolator_ = i;
         bootstrap_ = bootstrap;

         bootstrap_.setup(this);
      }

      public PiecewiseYieldCurve(Settings settings, int settlementDays, Calendar calendar, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps, List<Date> jumpDates, double accuracy)
         : this(settings, settlementDays, calendar, instruments, dayCounter, jumps, jumpDates, accuracy,
                FastActivator<Interpolator>.Create(), FastActivator<BootStrap>.Create()) { }
      public PiecewiseYieldCurve(Settings settings, int settlementDays, Calendar calendar, List<RateHelper> instruments,
                                 DayCounter dayCounter,  List<Handle<Quote>> jumps, List<Date> jumpDates, double accuracy,
                                 Interpolator i, BootStrap bootstrap)
         : base(settings, settlementDays, calendar, dayCounter, jumps, jumpDates)
      {
         _traits_ = FastActivator<Traits>.Create();
         base_curve = _traits_.factory<Interpolator>(this.settings(), settlementDays, calendar, dayCounter, jumps, jumpDates, i);
         _instruments_ = instruments;
         accuracy_ = accuracy;
         interpolator_ = i;
         bootstrap_ = bootstrap;

         bootstrap_.setup(this);
      }
      #endregion

      // observer interface
      public override void update()
      {
         base_curve.update();
         base.update();
         // LazyObject::update();        // we do it in the TermStructure
      }

      protected override void performCalculations()
      {
         // just delegate to the bootstrapper
         bootstrap_.calculate();
      }
   }

   // Allows for optional 3rd generic parameter defaulted to IterativeBootstrap
   public class PiecewiseYieldCurve<Traits, Interpolator> : PiecewiseYieldCurve<Traits, Interpolator, IterativeBootstrapForYield>
      where Traits : ITraits<YieldTermStructure>, new()
      where Interpolator : class, IInterpolationFactory, new()
   {

      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments, DayCounter dayCounter)
         : base(settings, referenceDate, instruments, dayCounter) { }
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps, List<Date> jumpDates)
         : base(settings, referenceDate, instruments, dayCounter, jumps, jumpDates) { }
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps, List<Date> jumpDates, double accuracy)
         : base(settings, referenceDate, instruments, dayCounter, jumps, jumpDates, accuracy) { }
      public PiecewiseYieldCurve(Settings settings, Date referenceDate, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps, List<Date> jumpDates, double accuracy, Interpolator i)
         : base(settings, referenceDate, instruments, dayCounter, jumps, jumpDates, accuracy, i) { }

      public PiecewiseYieldCurve(Settings settings, int settlementDays, Calendar calendar, List<RateHelper> instruments,
                                 DayCounter dayCounter)
         : this(settings, settlementDays, calendar, instruments, dayCounter, new List<Handle<Quote>>(), new List<Date>(), 1.0e-12) { }

      public PiecewiseYieldCurve(Settings settings, int settlementDays, Calendar calendar, List<RateHelper> instruments,
                                 DayCounter dayCounter, List<Handle<Quote>> jumps, List<Date> jumpDates, double accuracy)
         : base(settings, settlementDays, calendar, instruments, dayCounter, jumps, jumpDates, accuracy) { }
   }
}
