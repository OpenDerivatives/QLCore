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

/*! \file fdmblackscholesmesher.cpp
    \brief 1-d mesher for the Black-Scholes process (in ln(S))
*/

namespace QLCore
{
    public class FdmBlackScholesMultiStrikeMesher : Fdm1dMesher
    {
        public FdmBlackScholesMultiStrikeMesher(int size,
                                                 GeneralizedBlackScholesProcess process,
                                                 double maturity,
                                                 List<double> strikes,
                                                 double eps = 0.0001,
                                                 double scaleFactor = 1.5,
                                                 Pair<double?, double?> cPoint
                                                            = null)
            : base(size)
        {
            double spot = process.x0();
            Utils.QL_REQUIRE(spot > 0.0, () => "negative or null underlying given");

            double d = process.dividendYield().currentLink().discount(maturity)
                                 / process.riskFreeRate().currentLink().discount(maturity);
            double minStrike = strikes.Min();
            double maxStrike = strikes.Max();

            double Fmin = spot * spot / maxStrike * d;
            double Fmax = spot * spot / minStrike * d;

            Utils.QL_REQUIRE(Fmin > 0.0, () => "negative forward given");

            // Set the grid boundaries
            double normInvEps = new InverseCumulativeNormal().value(1 - eps);
            double sigmaSqrtTmin
                = process.blackVolatility().currentLink().blackVol(maturity, minStrike)
                                                            * Math.Sqrt(maturity);
            double sigmaSqrtTmax
                = process.blackVolatility().currentLink().blackVol(maturity, maxStrike)
                                                            * Math.Sqrt(maturity);

            double xMin
                = Math.Min(0.8 * Math.Log(0.8 * spot * spot / maxStrike),
                           Math.Log(Fmin) - sigmaSqrtTmin * normInvEps * scaleFactor
                                          - sigmaSqrtTmin * sigmaSqrtTmin / 2.0);
            double xMax
                = Math.Max(1.2 * Math.Log(0.8 * spot * spot / minStrike),
                           Math.Log(Fmax) + sigmaSqrtTmax * normInvEps * scaleFactor
                                          - sigmaSqrtTmax * sigmaSqrtTmax / 2.0);

            Fdm1dMesher helper;
            if (cPoint.first != null
                && Math.Log(cPoint.first.Value) >= xMin && Math.Log(cPoint.first.Value) <= xMax)
            {

                helper = new Concentrating1dMesher(xMin, xMax, size,
                        new Pair<double?, double?>(Math.Log(cPoint.first.Value), cPoint.second));
            }
            else
            {
                helper = new Uniform1dMesher(xMin, xMax, size);

            }

            locations_ = helper.locations();
            for (int i = 0; i < locations_.Count; ++i)
            {
                dplus_[i] = helper.dplus(i);
                dminus_[i] = helper.dminus(i);
            }
        }
    }
}
