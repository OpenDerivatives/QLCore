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

using System.Collections.Generic;
using Xunit;
using QLCore;

namespace TestSuite
{

   public class T_Schedule
   {
      void check_dates(Schedule s, List<Date> expected)
      {
         if (s.Count != expected.Count)
         {
            QAssert.Fail("expected " + expected.Count + " dates, " + "found " + s.Count);
         }

         for (int i = 0; i < expected.Count; ++i)
         {
            if (s[i] != expected[i])
            {
               QAssert.Fail("expected " + expected[i] + " at index " + i + ", " + "found " + s[i]);

            }
         }
      }

      [Fact]
      public void testDailySchedule()
      {
         // Testing schedule with daily frequency
         Settings settings = new Settings();
         Date startDate = new Date(17, Month.January, 2012);

         Schedule s = new MakeSchedule(settings).from(startDate).to(startDate + 7)
         .withCalendar(new TARGET())
         .withConvention(BusinessDayConvention.Preceding)
         .withFrequency(Frequency.Daily).value();

         List<Date> expected = new List<Date>(6);
         // The schedule should skip Saturday 21st and Sunday 22rd.
         // Previously, it would adjust them to Friday 20th, resulting
         // in three copies of the same date.
         expected.Add(new Date(17, Month.January, 2012));
         expected.Add(new Date(18, Month.January, 2012));
         expected.Add(new Date(19, Month.January, 2012));
         expected.Add(new Date(20, Month.January, 2012));
         expected.Add(new Date(23, Month.January, 2012));
         expected.Add(new Date(24, Month.January, 2012));

         check_dates(s, expected);
      }

      [Fact]
      public void testEndDateWithEomAdjustment()
      {
         // Testing end date for schedule with end-of-month adjustment
         Settings settings = new Settings();
         Schedule s = new MakeSchedule(settings).from(new Date(30, Month.September, 2009))
         .to(new Date(15, Month.June, 2012))
         .withCalendar(new Japan())
         .withTenor(new Period(6, TimeUnit.Months))
         .withConvention(BusinessDayConvention.Following)
         .withTerminationDateConvention(BusinessDayConvention.Following)
         .forwards()
         .endOfMonth().value();

         List<Date> expected = new List<Date>();
         // The end date is adjusted, so it should also be moved to the end
         // of the month.
         expected.Add(new Date(30, Month.September, 2009));
         expected.Add(new Date(31, Month.March, 2010));
         expected.Add(new Date(30, Month.September, 2010));
         expected.Add(new Date(31, Month.March, 2011));
         expected.Add(new Date(30, Month.September, 2011));
         expected.Add(new Date(30, Month.March, 2012));
         expected.Add(new Date(29, Month.June, 2012));

         check_dates(s, expected);

         // now with unadjusted termination date...
         s = new MakeSchedule(settings).from(new Date(30, Month.September, 2009))
         .to(new Date(15, Month.June, 2012))
         .withCalendar(new Japan())
         .withTenor(new Period(6, TimeUnit.Months))
         .withConvention(BusinessDayConvention.Following)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .forwards()
         .endOfMonth().value();
         // ...which should leave it alone.
         expected[6] = new Date(15, Month.June, 2012);

         check_dates(s, expected);
      }

      [Fact]
      public void testDatesPastEndDateWithEomAdjustment()
      {
         Settings settings = new Settings();
         Schedule s = new MakeSchedule(settings).from(new Date(28, Month.March, 2013))
         .to(new Date(30, Month.March, 2015))
         .withCalendar(new TARGET())
         .withTenor(new Period(1, TimeUnit.Years))
         .withConvention(BusinessDayConvention.Unadjusted)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .forwards()
         .endOfMonth().value();

         List<Date> expected = new List<Date>();
         expected.Add(new Date(31, Month.March, 2013));
         expected.Add(new Date(31, Month.March, 2014));
         // March 31st 2015, coming from the EOM adjustment of March 28th,
         // should be discarded as past the end date.
         expected.Add(new Date(30, Month.March, 2015));

         check_dates(s, expected);
      }

      [Fact]
      public void testDatesSameAsEndDateWithEomAdjustment()
      {
         // Testing that next-to-last date same as end date is removed...
         Settings settings = new Settings();
         Schedule s = new MakeSchedule(settings).from(new Date(28, Month.March, 2013))
         .to(new Date(31, Month.March, 2015))
         .withCalendar(new TARGET())
         .withTenor(new Period(1, TimeUnit.Years))
         .withConvention(BusinessDayConvention.Unadjusted)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .forwards()
         .endOfMonth()
         .value();

         List<Date> expected = new List<Date>(3);
         expected.Add(new Date(31, Month.March, 2013));
         expected.Add(new Date(31, Month.March, 2014));
         // March 31st 2015, coming from the EOM adjustment of March 28th,
         // should be discarded as the same as the end date.
         expected.Add(new Date(31, Month.March, 2015));

         check_dates(s, expected);

         // also, the last period should be regular.
         if (!s.isRegular(2))
            QAssert.Fail("last period should be regular");
      }


      [Fact]
      public void testForwardDatesWithEomAdjustment()
      {
         // Testing that the last date is not adjusted for EOM when termination date convention is unadjusted
         Settings settings = new Settings();
         Schedule s = new MakeSchedule(settings).from(new Date(31, Month.August, 1996))
         .to(new Date(15, Month.September, 1997))
         .withCalendar(new UnitedStates(UnitedStates.Market.GovernmentBond))
         .withTenor(new Period(6, TimeUnit.Months))
         .withConvention(BusinessDayConvention.Unadjusted)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .forwards()
         .endOfMonth().value();

         List<Date> expected = new List<Date>();
         expected.Add(new Date(31, Month.August, 1996));
         expected.Add(new Date(28, Month.February, 1997));
         expected.Add(new Date(31, Month.August, 1997));
         expected.Add(new Date(15, Month.September, 1997));

         check_dates(s, expected);
      }

      [Fact]
      public void testBackwardDatesWithEomAdjustment()
      {
         // Testing that the first date is not adjusted for EOM going backward when termination date convention is unadjusted
         Settings settings = new Settings();
         Schedule s = new MakeSchedule(settings).from(new Date(22, Month.August, 1996))
         .to(new Date(31, Month.August, 1997))
         .withCalendar(new UnitedStates(UnitedStates.Market.GovernmentBond))
         .withTenor(new Period(6, TimeUnit.Months))
         .withConvention(BusinessDayConvention.Unadjusted)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .backwards()
         .endOfMonth().value();

         List<Date> expected = new List<Date>();
         expected.Add(new Date(22, Month.August, 1996));
         expected.Add(new Date(31, Month.August, 1996));
         expected.Add(new Date(28, Month.February, 1997));
         expected.Add(new Date(31, Month.August, 1997));

         check_dates(s, expected);
      }

      [Fact]
      public void testDoubleFirstDateWithEomAdjustment()
      {
         // Testing that the first date is not duplicated due to EOM convention when going backwards
         Settings settings = new Settings();
         Schedule s = new MakeSchedule(settings).from(new Date(22, Month.August, 1996))
         .to(new Date(31, Month.August, 1997))
         .withCalendar(new UnitedStates(UnitedStates.Market.GovernmentBond))
         .withTenor(new Period(6, TimeUnit.Months))
         .withConvention(BusinessDayConvention.Following)
         .withTerminationDateConvention(BusinessDayConvention.Following)
         .backwards()
         .endOfMonth().value();

         List<Date> expected = new List<Date>();
         expected.Add(new Date(30, Month.August, 1996));
         expected.Add(new Date(28, Month.February, 1997));
         expected.Add(new Date(29, Month.August, 1997));

         check_dates(s, expected);
      }

      [Fact]
      public void testCDS2015Convention()
      {
         // Testing CDS2015 semi-annual rolling convention
         //From September 20th 2016 to March 19th 2017 of the next Year,
         //end date is December 20th 2021 for a 5 year Swap
         Settings settings = new Settings();
         Schedule s1 = new  MakeSchedule(settings).from(new Date(12, Month.December, 2016))
         .to(new Date(12, Month.December, 2016) + new Period(5, TimeUnit.Years))
         .withCalendar(new WeekendsOnly())
         .withTenor(new Period(3, TimeUnit.Months))
         .withConvention(BusinessDayConvention.ModifiedFollowing)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .withRule(DateGeneration.Rule.CDS2015).value();
         QAssert.IsTrue(s1.startDate() == new Date(20, Month.September, 2016));
         QAssert.IsTrue(s1.endDate() == new Date(20, Month.December, 2021));
         Schedule s2 = new MakeSchedule(settings).from(new Date(1, Month.March, 2017))
         .to(new Date(1, Month.March, 2017) + new Period(5, TimeUnit.Years))
         .withCalendar(new WeekendsOnly())
         .withTenor(new Period(3, TimeUnit.Months))
         .withConvention(BusinessDayConvention.ModifiedFollowing)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .withRule(DateGeneration.Rule.CDS2015).value();
         QAssert.IsTrue(s2.startDate() == new Date(20, Month.December, 2016));
         QAssert.IsTrue(s2.endDate() == new Date(20, Month.December, 2021));
         //From March 20th 2017 to September 19th 2017
         //end date is June 20th 2022 for a 5 year Swap
         Schedule s3 = new MakeSchedule(settings).from(new Date(20, Month.March, 2017))
         .to(new Date(20, Month.March, 2017) + new Period(5, TimeUnit.Years))
         .withCalendar(new WeekendsOnly())
         .withTenor(new Period(3, TimeUnit.Months))
         .withConvention(BusinessDayConvention.ModifiedFollowing)
         .withTerminationDateConvention(BusinessDayConvention.Unadjusted)
         .withRule(DateGeneration.Rule.CDS2015).value();
         QAssert.IsTrue(s3.startDate() == new Date(20, Month.March, 2017));
         QAssert.IsTrue(s3.endDate() == new Date(20, Month.June, 2022));

      }

      [Fact]
      public void testDateConstructor()
      {
         // Testing the constructor taking a vector of dates and possibly additional meta information
         Settings settings = new Settings();

         List<Date> dates = new List<Date>();
         dates.Add(new Date(16, Month.May, 2015));
         dates.Add(new Date(18, Month.May, 2015));
         dates.Add(new Date(18, Month.May, 2016));
         dates.Add(new Date(31, Month.December, 2017));

         // schedule without any additional information
         Schedule schedule1 = new Schedule(settings, dates);
         if (schedule1.Count != dates.Count)
            QAssert.Fail("schedule1 has size " + schedule1.Count + ", expected " + dates.Count);
         for (int i = 0; i < dates.Count; ++i)
            if (schedule1[i] != dates[i])
               QAssert.Fail("schedule1 has " + schedule1[i] + " at position " + i + ", expected " + dates[i]);
         if (schedule1.calendar() != new NullCalendar())
            QAssert.Fail("schedule1 has calendar " + schedule1.calendar().name() + ", expected null calendar");
         if (schedule1.businessDayConvention() != BusinessDayConvention.Unadjusted)
            QAssert.Fail("schedule1 has convention " + schedule1.businessDayConvention() + ", expected unadjusted");

         // schedule with metadata
         List<bool> regular = new List<bool>();
         regular.Add(false);
         regular.Add(true);
         regular.Add(false);

         Schedule schedule2 = new Schedule(settings, dates, new TARGET(), BusinessDayConvention.Following, BusinessDayConvention.ModifiedPreceding, new Period(1, TimeUnit.Years),
                                           DateGeneration.Rule.Backward, true, regular);
         for (int i = 1; i < dates.Count; ++i)
            if (schedule2.isRegular(i) != regular[i - 1])
               QAssert.Fail("schedule2 has a " + (schedule2.isRegular(i) ? "regular" : "irregular") + " period at position " + i + ", expected " + (regular[i - 1] ? "regular" : "irregular"));
         if (schedule2.calendar() != new TARGET())
            QAssert.Fail("schedule1 has calendar " + schedule2.calendar().name() + ", expected TARGET");
         if (schedule2.businessDayConvention() != BusinessDayConvention.Following)
            QAssert.Fail("schedule2 has convention " + schedule2.businessDayConvention() + ", expected Following");
         if (schedule2.terminationDateBusinessDayConvention() != BusinessDayConvention.ModifiedPreceding)
            QAssert.Fail("schedule2 has convention " + schedule2.terminationDateBusinessDayConvention() + ", expected Modified Preceding");
         if (schedule2.tenor() != new Period(1, TimeUnit.Years))
            QAssert.Fail("schedule2 has tenor " + schedule2.tenor() + ", expected 1Y");
         if (schedule2.rule() != DateGeneration.Rule.Backward)
            QAssert.Fail("schedule2 has rule " + schedule2.rule() + ", expected Backward");
         if (schedule2.endOfMonth() != true)
            QAssert.Fail("schedule2 has end of month flag false, expected true");
      }
   }
}
