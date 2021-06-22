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

using Xunit;
using QLCore;

namespace TestSuite
{

   public class T_Money
   {
      [Fact]
      public void testNone()
      {
         Currency EUR = new EURCurrency();
         Settings settings = new Settings();
         Money m1 = 50000.0 * EUR;
         Money m2 = 100000.0 * EUR;
         Money m3 = 500000.0 * EUR;

         Money.conversionType = Money.ConversionType.NoConversion;

         Money calculated = m1 * 3.0 + 2.5 * m2 - m3 / 5.0;
         double x = m1.value * 3.0 + 2.5 * m2.value - m3.value / 5.0;
         Money expected = new Money(settings, x, EUR);

         if (calculated != expected)
            QAssert.Fail("Wrong result: expected: " + expected + " calculated: " + calculated);
      }

      [Fact]
      public void testBaseCurrency()
      {
         Settings settings =  new Settings();
         Currency EUR = new EURCurrency(), GBP = new GBPCurrency(), USD = new USDCurrency();

         Money m1 = 50000.0 * GBP;
         Money m2 = 100000.0 * EUR;
         Money m3 = 500000.0 * USD;

         ExchangeRateManager.Instance.clear();
         ExchangeRate eur_usd = new ExchangeRate(EUR, USD, 1.2042);
         ExchangeRate eur_gbp = new ExchangeRate(EUR, GBP, 0.6612);
         ExchangeRateManager.Instance.add(eur_usd);
         ExchangeRateManager.Instance.add(eur_gbp);

         Money.conversionType = Money.ConversionType.BaseCurrencyConversion;
         Money.baseCurrency = EUR;

         Money calculated = m1 * 3.0 + 2.5 * m2 - m3 / 5.0;

         Rounding round = Money.baseCurrency.rounding;
         double x = round.Round(m1.value * 3.0 / eur_gbp.rate) + 2.5 * m2.value
                    - round.Round(m3.value / (5.0 * eur_usd.rate));
         Money expected = new Money(settings, x, EUR);

         Money.conversionType = Money.ConversionType.NoConversion;

         if (calculated != expected)
         {
            QAssert.Fail("Wrong result: expected: " + expected + "calculated: " + calculated);
         }
      }

      [Fact]
      public void testAutomated()
      {
         Settings settings = new Settings();
         Currency EUR = new EURCurrency(), GBP = new GBPCurrency(), USD = new USDCurrency();

         Money m1 = 50000.0 * GBP;
         Money m2 = 100000.0 * EUR;
         Money m3 = 500000.0 * USD;

         ExchangeRateManager.Instance.clear();
         ExchangeRate eur_usd = new ExchangeRate(EUR, USD, 1.2042);
         ExchangeRate eur_gbp = new ExchangeRate(EUR, GBP, 0.6612);
         ExchangeRateManager.Instance.add(eur_usd);
         ExchangeRateManager.Instance.add(eur_gbp);

         Money.conversionType = Money.ConversionType.AutomatedConversion;

         Money calculated = (m1 * 3.0 + 2.5 * m2) - m3 / 5.0;

         Rounding round = m1.currency.rounding;
         double x = m1.value * 3.0 + round.Round(2.5 * m2.value * eur_gbp.rate)
                    - round.Round((m3.value / 5.0) * eur_gbp.rate / eur_usd.rate);
         Money expected = new Money(settings, x, GBP);

         Money.conversionType = Money.ConversionType.NoConversion;

         if (calculated != expected)
         {
            QAssert.Fail("Wrong result: " + "expected: " + expected + " calculated: " + calculated);
         }
      }

   }
}

