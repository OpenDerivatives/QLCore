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
using System.Text;

namespace QLCore
{
    public class t_remapper
    {
        public Func<double,double> f;
        public double T;
        public t_remapper(Func<double, double> f, double T)
        {
            this.f = f;
            this.T = T;
        }
        public double value(double x)
        {
            double arg = (x + 1.0) * T / 2.0;
            return f(arg);
        }
    }
    public class DefaultDensityStructure : DefaultProbabilityTermStructure
    {
        public DefaultDensityStructure(
            Settings settings,
            DayCounter dayCounter = null,
            List<Handle<Quote>> jumps = null,
            List<Date> jumpDates = null)
            : base (settings, dayCounter, jumps, jumpDates)
        {}
        public DefaultDensityStructure(
            Settings settings,
            Date referenceDate,
            Calendar cal = null,
            DayCounter dayCounter = null,
            List<Handle<Quote>> jumps = null,
            List<Date> jumpDates = null)
            : base(settings, referenceDate, cal, dayCounter, jumps, jumpDates)
        { }
        public DefaultDensityStructure(
            Settings settings,
            int settlementDays,
            Calendar cal,
            DayCounter dayCounter = null,
            List<Handle<Quote> > jumps = null,
            List<Date> jumpDates = null)
            : base (settings, settlementDays, cal, dayCounter, jumps, jumpDates)
        { }
        protected internal override double survivalProbabilityImpl(double t)
        {
            GaussChebyshevIntegration integral = new GaussChebyshevIntegration(48);
            t_remapper remap_t = new t_remapper(this.defaultDensityImpl, t);
            // the Gauss-Chebyshev quadratures integrate over [-1,1],
            // hence the remapping (and the Jacobian term t/2)
            double P = 1.0 - integral.value(remap_t.value) * t / 2.0;
            //Utils.QL_ENSURE(P >= 0.0, "negative survival probability");
            return Math.Max(P, 0.0);
        }
        protected internal override double defaultDensityImpl(double t)
        {
            throw new NotImplementedException();
        }
        protected internal override double hazardRateImpl(double t)
        {
            throw new NotImplementedException();
        }
        public override Date maxDate() { return null; }
    }
}
