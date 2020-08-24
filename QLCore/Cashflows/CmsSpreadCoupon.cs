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
    //! CMS spread coupon class
    /*! \warning This class does not perform any date adjustment,
                 i.e., the start and end date passed upon construction
                 should be already rolled to a business day.
    */
    public class CmsSpreadCoupon : FloatingRateCoupon
    {
        new protected SwapSpreadIndex index_;

        // need by CashFlowVectors
        public CmsSpreadCoupon() { }

        public CmsSpreadCoupon(double nominal,
                         Date paymentDate,
                         Date startDate,
                         Date endDate,
                         int fixingDays,
                         SwapSpreadIndex index,
                         double gearing = 1.0,
                         double spread = 0.0,
                         Date refPeriodStart = null,
                         Date refPeriodEnd = null,
                         DayCounter dayCounter = null,
                         bool isInArrears = false,
                         Date exCouponDate = null)
           : base(paymentDate, nominal, startDate, endDate, fixingDays, index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears)
        {
            index_ = index;

            index_.registerWith(update);
        }

        // Inspectors
        public SwapSpreadIndex swapSpreadIndex()
        {
            return index_;
        }

        // Factory - for Leg generators
        public override CashFlow factory(double nominal, Date paymentDate, Date startDate, Date endDate, int fixingDays,
                     InterestRateIndex index, double gearing, double spread,
                     Date refPeriodStart, Date refPeriodEnd, DayCounter dayCounter, bool isInArrears)
        {
            return new CmsSpreadCoupon(nominal, paymentDate, startDate, endDate, fixingDays,
                       (SwapSpreadIndex)index, gearing, spread, refPeriodStart, refPeriodEnd, dayCounter, isInArrears);
        }

    }

    public class CappedFlooredCmsSpreadCoupon : CappedFlooredCoupon
    {
        public CappedFlooredCmsSpreadCoupon() { }

        public CappedFlooredCmsSpreadCoupon(
                  Date paymentDate,
                  double nominal,
                  Date startDate,
                  Date endDate,
                  int fixingDays,
                  SwapSpreadIndex index,
                  double gearing = 1.0,
                  double spread = 0.0,
                  double? cap = null,
                  double? floor = null,
                  Date refPeriodStart = null,
                  Date refPeriodEnd = null,
                  DayCounter dayCounter = null,
                  bool isInArrears = false,
                  Date exCouponDate = null)
        : base(new CmsSpreadCoupon(nominal, paymentDate, startDate, endDate, fixingDays,
                      index, gearing, spread, refPeriodStart, refPeriodEnd,
                      dayCounter, isInArrears, exCouponDate), cap, floor)
        { }

    }


    //! helper class building a sequence of capped/floored cms-rate coupons
    public class CmsSpreadLeg : FloatingLegBase
    {
        public CmsSpreadLeg(Schedule schedule, SwapSpreadIndex swapIndex)
        {
            schedule_ = schedule;
            index_ = swapIndex;
            paymentAdjustment_ = BusinessDayConvention.Following;
            inArrears_ = false;
            zeroPayments_ = false;
        }

        public override List<CashFlow> value()
        {
            return CashFlowVectors.FloatingLeg<SwapSpreadIndex, CmsSpreadCoupon, CappedFlooredCmsSpreadCoupon>(
                notionals_, schedule_, index_ as SwapSpreadIndex, paymentDayCounter_, paymentAdjustment_, fixingDays_, gearings_, spreads_, caps_, floors_, inArrears_, zeroPayments_);
        }
    }

    //! base pricer for vanilla CMS spread coupons
    public class CmsSpreadCouponPricer : FloatingRateCouponPricer
    {
        public CmsSpreadCouponPricer(Handle<Quote> correlation = null)
        {
            correlation_ = correlation;
            correlation_.registerWith(update);
        }

        public Handle<Quote> correlation() { return correlation_; }

        public void setCorrelation(Handle<Quote> correlation = null)
        {
            correlation_.unregisterWith(update);
            correlation_ = correlation;
            correlation_.registerWith(update);
            update();
        }

        public override double swapletPrice()
        {
            throw new NotImplementedException();
        }

        public override double swapletRate()
        {
            throw new NotImplementedException();
        }

        public override double capletPrice(double effectiveCap)
        {
            throw new NotImplementedException();
        }

        public override double capletRate(double effectiveCap)
        {
            throw new NotImplementedException();
        }

        public override double floorletPrice(double effectiveFloor)
        {
            throw new NotImplementedException();
        }

        public override double floorletRate(double effectiveFloor)
        {
            throw new NotImplementedException();
        }

        public override void initialize(FloatingRateCoupon coupon)
        {
            throw new NotImplementedException();
        }

        public override double optionletPrice(Option.Type optionType, double effStrike)
        {
            throw new NotImplementedException();
        }

        protected Handle<Quote> correlation_;
    }

    public class LognormalCmsSpreadPricer : CmsSpreadCouponPricer
    {
        public LognormalCmsSpreadPricer(
            CmsCouponPricer cmsPricer1,
            CmsCouponPricer cmsPricer2,
            Handle<Quote> correlation,
            Handle<YieldTermStructure> couponDiscountCurve = null,
            int integrationPoints = 16,
            VolatilityType volatilityType = VolatilityType.None,
            double? shift1 = null,
            double? shift2 = null)
            : base(correlation)
        {
            correlation_.registerWith(update);
            cmsPricer1_ = cmsPricer1;
            cmsPricer2_ = cmsPricer2;

            couponDiscountCurve_ = couponDiscountCurve;

            if (couponDiscountCurve_ != null && !couponDiscountCurve_.empty())
                couponDiscountCurve_.registerWith(update);

            cmsPricer1_.registerWith(update);
            cmsPricer2_.registerWith(update);

            Utils.QL_REQUIRE(integrationPoints >= 4,
                       () => "at least 4 integration points should be used ("
                           + integrationPoints + ")");
            integrator_ = new GaussHermiteIntegration(integrationPoints);

            cnd_ = new CumulativeNormalDistribution(0.0, 1.0);

            if (volatilityType == VolatilityType.None)
            {
                Utils.QL_REQUIRE(shift1 == null && shift2 == null,
                           () => "if volatility type is inherited, no shifts should be specified");
                inheritedVolatilityType_ = true;
                volType_ = cmsPricer1.swaptionVolatility().currentLink().volatilityType();
            }
            else
            {
                shift1_ = shift1 == null ? 0.0 : shift1.Value;
                shift2_ = shift2 == null ? 0.0 : shift2.Value;
                inheritedVolatilityType_ = false;
                volType_ = volatilityType;
            }
        }

        /* */
        public override double swapletPrice()
        {
            return gearing_ * coupon_.accrualPeriod() * discount_ *
                           (gearing1_ * adjustedFixing1_ + gearing2_ * adjustedFixing2_) +
                       spreadLegValue_;
        }

        public override double swapletRate()
        {
            return swapletPrice() / (coupon_.accrualPeriod() * discount_);
        }

        public override double capletPrice(double effectiveCap)
        {
            // caplet is equivalent to call option on fixing
            if (fixingDate_ <= today_)
            {
                // the fixing is determined
                double Rs = Math.Max(
                    coupon_.index().fixing(fixingDate_) - effectiveCap, 0.0);
                double price = gearing_ * Rs * coupon_.accrualPeriod() * discount_;
                return price;
            }
            else
            {
                double capletPrice = optionletPrice(Option.Type.Call, effectiveCap);
                return gearing_ * capletPrice;
            }
        }

        public override double capletRate(double effectiveCap)
        {
            return capletPrice(effectiveCap) /
                      (coupon_.accrualPeriod() * discount_);
        }

        public override double floorletPrice(double effectiveFloor)
        {
            // floorlet is equivalent to put option on fixing
            if (fixingDate_ <= today_)
            {
                // the fixing is determined
                double Rs = Math.Max(effectiveFloor - coupon_.index().fixing(fixingDate_), 
                                           0.0);
                double price = gearing_ * Rs * coupon_.accrualPeriod() * discount_;
                return price;
            }
            else
            {
                double floorletPrice = optionletPrice(Option.Type.Put, effectiveFloor);
                return gearing_ * floorletPrice;
            }
        }

        public override double floorletRate(double effectiveFloor)
        {
            return floorletPrice(effectiveFloor) /
                       (coupon_.accrualPeriod() * discount_);
        }

        public override void initialize(FloatingRateCoupon coupon)
        {
            coupon_ = coupon as CmsSpreadCoupon;
            Utils.QL_REQUIRE(coupon_ != null, () => "CMS spread coupon needed");
            index_ = coupon_.swapSpreadIndex();
            gearing_ = coupon_.gearing();
            spread_ = coupon_.spread();

            fixingDate_ = coupon_.fixingDate();
            paymentDate_ = coupon_.date();

            // if no coupon discount curve is given just use the discounting curve
            // from the _first_ swap index.
            // for double calculation this curve cancels out in the computation, so
            // e.g. the discounting
            // swap engine will produce correct results, even if the
            // couponDiscountCurve is not set here.
            // only the price member function in this class will be dependent on the
            // coupon discount curve.

            today_ = Settings.Instance.evaluationDate();

            if (couponDiscountCurve_.empty())
                couponDiscountCurve_ =
                    index_.swapIndex1().exogenousDiscount()
                        ? index_.swapIndex1().discountingTermStructure()
                        : index_.swapIndex1().forwardingTermStructure();

            discount_ = paymentDate_ > couponDiscountCurve_.currentLink().referenceDate()
                            ? couponDiscountCurve_.currentLink().discount(paymentDate_)
                            : 1.0;

            spreadLegValue_ = spread_ * coupon_.accrualPeriod() * discount_;

            gearing1_ = index_.gearing1();
            gearing2_ = index_.gearing2();

            Utils.QL_REQUIRE(gearing1_ > 0.0 && gearing2_ < 0.0,
                       () => "gearing1 (" + gearing1_
                                    + ") should be positive while gearing2 ("
                                    + gearing2_ + ") should be negative");

            c1_ = new CmsCoupon(
                coupon_.nominal(), coupon_.date(), coupon_.accrualStartDate(),
                coupon_.accrualEndDate(), coupon_.fixingDays,
                index_.swapIndex1(), 1.0, 0.0, coupon_.referencePeriodStart,
                coupon_.referencePeriodEnd, coupon_.dayCounter(),
                coupon_.isInArrears());

            c2_ = new CmsCoupon(
                coupon_.nominal(), coupon_.date(), coupon_.accrualStartDate(),
                coupon_.accrualEndDate(), coupon_.fixingDays,
                index_.swapIndex2(), 1.0, 0.0, coupon_.referencePeriodStart,
                coupon_.referencePeriodEnd, coupon_.dayCounter(),
                coupon_.isInArrears());

            c1_.setPricer(cmsPricer1_);
            c2_.setPricer(cmsPricer2_);

            if (fixingDate_ > today_)
            {

                fixingTime_ = cmsPricer1_.swaptionVolatility().currentLink().timeFromReference(
                    fixingDate_);

                swapRate1_ = c1_.indexFixing();
                swapRate2_ = c2_.indexFixing();

                adjustedFixing1_ = c1_.adjustedFixing;
                adjustedFixing2_ = c2_.adjustedFixing;

                SwaptionVolatilityStructure swvol = cmsPricer1_.swaptionVolatility();
                SwaptionVolatilityCube swcub = swvol as SwaptionVolatilityCube;

                if (inheritedVolatilityType_ && volType_ == VolatilityType.ShiftedLognormal)
                {
                    shift1_ =
                        swvol.shift(fixingDate_, index_.swapIndex1().tenor());
                    shift2_ =
                        swvol.shift(fixingDate_, index_.swapIndex2().tenor());
                }

                if (swcub == null)
                {
                    // not a cube, just an atm surface given, so we can
                    // not easily convert volatilities and just forbid it
                    Utils.QL_REQUIRE(inheritedVolatilityType_,
                               () => "if only an atm surface is given, the volatility type must be inherited");
                    vol1_ = swvol.volatility(
                        fixingDate_, index_.swapIndex1().tenor(), swapRate1_);
                    vol2_ = swvol.volatility(
                        fixingDate_, index_.swapIndex2().tenor(), swapRate2_);
                }
                else
                {
                    vol1_ = swcub.smileSection(fixingDate_,
                                                index_.swapIndex1().tenor())
                                .volatility(swapRate1_, volType_, shift1_);
                    vol2_ = swcub.smileSection(fixingDate_,
                                                index_.swapIndex2().tenor())
                                .volatility(swapRate2_, volType_, shift2_);
                }

                if (volType_ == VolatilityType.ShiftedLognormal)
                {
                    mu1_ = 1.0 / fixingTime_ * Math.Log((adjustedFixing1_ + shift1_) /
                                                        (swapRate1_ + shift1_));
                    mu2_ = 1.0 / fixingTime_ * Math.Log((adjustedFixing2_ + shift2_) /
                                                        (swapRate2_ + shift2_));
                }
                // for the normal volatility case we do not need the drifts
                // but rather use adjusted doubles directly in the integrand

                rho_ = Math.Max(Math.Min(correlation().currentLink().value(), 0.9999),
                                -0.9999); // avoid division by zero in integrand
            }
            else
            {
                // fixing is in the past or today
                adjustedFixing1_ = c1_.indexFixing();
                adjustedFixing2_ = c2_.indexFixing();
            }
        }

        public class integrand_f
        {
            protected LognormalCmsSpreadPricer pricer_;
            public integrand_f(LognormalCmsSpreadPricer pricer)
            {
                pricer_ = pricer;
            }
            public double value(double x)
            {
                return pricer_.integrand(x);
            }
        }

        public override double optionletPrice(Option.Type optionType, double strike)
        {

            // this method is only called for future fixings
            optionType_ = optionType;
            phi_ = optionType == Option.Type.Call ? 1.0 : -1.0;
            double res = 0.0;
            if (volType_ == VolatilityType.ShiftedLognormal)
            {
                // (shifted) lognormal volatility
                if (strike >= 0.0)
                {
                    a_ = gearing1_;
                    b_ = gearing2_;
                    s1_ = swapRate1_ + shift1_;
                    s2_ = swapRate2_ + shift2_;
                    m1_ = mu1_;
                    m2_ = mu2_;
                    v1_ = vol1_;
                    v2_ = vol2_;
                    k_ = strike + gearing1_ * shift1_ + gearing2_ * shift2_;
                }
                else
                {
                    a_ = -gearing2_;
                    b_ = -gearing1_;
                    s1_ = swapRate2_ + shift1_;
                    s2_ = swapRate1_ + shift2_;
                    m1_ = mu2_;
                    m2_ = mu1_;
                    v1_ = vol2_;
                    v2_ = vol1_;
                    k_ = -strike - gearing1_ * shift1_ - gearing2_ * shift2_;
                    res += phi_ * (gearing1_ * adjustedFixing1_ +
                                   gearing2_ * adjustedFixing2_ - strike);
                }
                res +=
                    1.0 / Math.Sqrt(Math.PI) * integrator_.value(new integrand_f(this).value);
            }
            else
            {
                // normal volatility
                double forward = gearing1_ * adjustedFixing1_ +
                    gearing2_ * adjustedFixing2_;
                double stddev =
                    Math.Sqrt(fixingTime_ *
                              (gearing1_ * gearing1_ * vol1_ * vol1_ +
                               gearing2_ * gearing2_ * vol2_ * vol2_ +
                               2.0 * gearing1_ * gearing2_ * rho_ * vol1_ * vol2_));
                res =
                    Utils.bachelierBlackFormula(optionType_, strike, forward, stddev, 1.0);
            }
            return res * discount_ * coupon_.accrualPeriod();
        }

        public double integrand(double x)
        {
            // this is Brigo, 13.16.2 with x = v / sqrt(2)

            double v = Math.Sqrt(2.0) * x;
            double h =
                k_ - b_ * s2_ * Math.Exp((m2_ - 0.5 * v2_ * v2_) * fixingTime_ +
                                         v2_ * Math.Sqrt(fixingTime_) * v);
            double phi1, phi2;
            phi1 = cnd_.value(
                phi_ * (Math.Log(a_ * s1_ / h) +
                        (m1_ + (0.5 - rho_ * rho_) * v1_ * v1_) * fixingTime_ +
                        rho_ * v1_ * Math.Sqrt(fixingTime_) * v) /
                (v1_ * Math.Sqrt(fixingTime_ * (1.0 - rho_ * rho_))));
            phi2 = cnd_.value(
                phi_ * (Math.Log(a_ * s1_ / h) +
                        (m1_ - 0.5 * v1_ * v1_) * fixingTime_ +
                        rho_ * v1_ * Math.Sqrt(fixingTime_) * v) /
                (v1_ * Math.Sqrt(fixingTime_ * (1.0 - rho_ * rho_))));
            double f = a_ * phi_ * s1_ *
                         Math.Exp(m1_ * fixingTime_ -
                                  0.5 * rho_ * rho_ * v1_ * v1_ * fixingTime_ +
                                  rho_ * v1_ * Math.Sqrt(fixingTime_) * v) *
                         phi1 -
                     phi_ * h * phi2;
            return Math.Exp(-x * x) * f;
        }

        public double integrand_normal(double x)
        {
            // this is http://ssrn.com/abstract=2686998, 3.20 with x = s / sqrt(2)
            double s = Math.Sqrt(2.0) * x;

            double beta =
                phi_ *
                (gearing1_ * adjustedFixing1_ + gearing2_ * adjustedFixing2_ - k_ +
                 Math.Sqrt(fixingTime_) *
                     (rho_ * gearing1_ * vol1_ + gearing2_ * vol2_) * s);
            double f =
                Utils.close_enough(alpha_, 0.0)
                    ? Math.Max(beta, 0.0)
                    : psi_ * alpha_ / (Math.Sqrt(Math.PI) * Math.Sqrt(2.0)) *
                              Math.Exp(-beta * beta / (2.0 * alpha_ * alpha_)) +
                          beta * (1.0 - cnd_.value(-psi_ * beta / alpha_));
            return Math.Exp(-x * x) * f;
        }

        protected CmsCouponPricer cmsPricer1_, cmsPricer2_;
        protected Handle<YieldTermStructure> couponDiscountCurve_;
        protected CmsSpreadCoupon coupon_;
        protected Date today_, fixingDate_, paymentDate_;
        protected double fixingTime_;
        protected double gearing_, spread_;
        protected double spreadLegValue_;
        protected double discount_;
        protected SwapSpreadIndex index_;
        protected CumulativeNormalDistribution cnd_;
        protected GaussianQuadrature integrator_;
        protected double swapRate1_, swapRate2_, gearing1_, gearing2_;
        protected double adjustedFixing1_, adjustedFixing2_;
        protected double vol1_, vol2_;
        protected double mu1_, mu2_;
        protected double rho_;
        protected bool inheritedVolatilityType_;
        protected VolatilityType volType_;
        protected double shift1_, shift2_;
        protected double phi_, a_, b_, s1_, s2_, m1_, m2_, v1_, v2_, k_;
        protected double alpha_, psi_;
        protected Option.Type optionType_;
        protected CmsCoupon c1_, c2_;
    }
}
