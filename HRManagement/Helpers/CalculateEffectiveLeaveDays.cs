// This class will calculate effective leave days considering weekends and hours taken in a day(half day leave)
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;

namespace HRManagement.Helpers
{
    public static class CalculateEffectiveLeaveDays
    {

        // For now only created a basic logic, will improve it later - once i get more clarity from Malar regarding the requirements
        public static bool IsPublicHoliday(DateTime dateTime)
        {
            //var publicHolidays = new List<DateTime>();

            if (dateTime.Year == 2025)
            {
                var publicHolidays = new List<DateTime>
                {
                    new DateTime(2025, 1, 1),   // New Year's Day
                    new DateTime(2025, 1, 27),  // Israa & Mi’raj (predicted)
                    new DateTime(2025, 3, 30),  // Eid Al Fitr - Day 1 (predicted)
                    new DateTime(2025, 3, 31),  // Eid Al Fitr - Day 2
                    new DateTime(2025, 4, 1),   // Eid Al Fitr - Day 3
                    new DateTime(2025, 6, 5),   // Arafat Day (predicted)
                    new DateTime(2025, 6, 6),   // Eid Al Adha - Day 1
                    new DateTime(2025, 6, 7),   // Eid Al Adha - Day 2
                    new DateTime(2025, 6, 8),   // Eid Al Adha - Day 3
                    new DateTime(2025, 6, 27),  // Hijri New Year (predicted)
                    new DateTime(2025, 9, 5),   // Prophet Muhammad’s Birthday (predicted)
                    new DateTime(2025, 12, 1),  // Commemoration Day
                    new DateTime(2025, 12, 2),  // UAE National Day
                    new DateTime(2025, 12, 3),  // UAE National Day Holiday
                };

                return publicHolidays.Contains(dateTime.Date);
            }
            else if (dateTime.Year == 2026)
            {
                var publicHolidays = new List<DateTime>
                {
                    new DateTime(2026, 1, 1),   // New Year's Day
                    new DateTime(2026, 3, 20),  // Eid Al Fitr - Day 1 (predicted)
                    new DateTime(2026, 3, 21),  // Eid Al Fitr - Day 2
                    new DateTime(2026, 3, 22),  // Eid Al Fitr - Day 3
                    new DateTime(2026, 5, 26),  // Arafat Day (predicted)
                    new DateTime(2026, 5, 27),  // Eid Al Adha - Day 1
                    new DateTime(2026, 5, 28),  // Eid Al Adha - Day 2
                    new DateTime(2026, 5, 29),  // Eid Al Adha - Day 3
                    new DateTime(2026, 6, 17),  // Hijri New Year (predicted)
                    new DateTime(2026, 8, 25),  // Prophet Muhammad’s Birthday (predicted)
                    new DateTime(2026, 12, 1),  // Commemoration Day
                    new DateTime(2026, 12, 2),  // UAE National Day
                    new DateTime(2026, 12, 3),  // UAE National Day Holiday
                };

                return publicHolidays.Contains(dateTime.Date);
            }

            return false; // Default to false if year is not handled
        }


        public static bool CheckIfStartDateIsHalfDay(DateTime dateTime)
        {
            // Assuming half-day is considered if the time is after 1 PM (13:00)
            //TimeSpan time = dateTime.TimeOfDay;

            return dateTime.TimeOfDay >= TimeSpan.FromHours(13);
        }

        public static bool CheckIfEndDateIsHalfDay(DateTime dateTime)
        {
            // Assuming half-day is considered if the time is before 1 PM (13:00)
            return dateTime.TimeOfDay <= TimeSpan.FromHours(13);
        }

        //public static bool IsPublicHolday(DateTime dateTime)
        //{
        //    var publicHolidays = new List<DateTime>
        //    {
        //        new DateTime(dateTime.Year, 1, 1),   // New Year's Day
        //        new DateTime(dateTime.Year, 12, 25), // Christmas Day
        //        // Add more public holidays as needed
        //    }; // Currently 

        //    if (publicHolidays.Contains(dateTime.Date)) return true;

        //    return false;
        //}

        public static decimal GetEffectiveLeaveDays(DateTime StartDate, DateTime EndDate)
        {
            decimal leaveDaysUsed = 0m;

            if (StartDate.Date == EndDate.Date)
            {
                if (StartDate.DayOfWeek == DayOfWeek.Saturday || StartDate.DayOfWeek == DayOfWeek.Sunday || IsPublicHoliday(StartDate)) return 0m;

                bool isStartDateAHalfDay = CheckIfStartDateIsHalfDay(StartDate);
                if (isStartDateAHalfDay) return 0.5m;

                bool isEndDateAHalfDay = CheckIfEndDateIsHalfDay(EndDate);

                if (isEndDateAHalfDay) return 0.5m;

                return 1m; // Full day leave if neither is half day


            }
            else
            {
                if (StartDate.DayOfWeek != DayOfWeek.Saturday && StartDate.DayOfWeek != DayOfWeek.Sunday)
                {// Start date is a working day
                    bool isStartDateAHalfDay = CheckIfStartDateIsHalfDay(StartDate);

                    if (IsPublicHoliday(StartDate)) leaveDaysUsed += 0m;
                    else if (isStartDateAHalfDay) leaveDaysUsed += 0.5m;
                    else leaveDaysUsed += 1m;
                }


                if (EndDate.DayOfWeek != DayOfWeek.Saturday && EndDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    bool isEndDateAHalfDay = CheckIfEndDateIsHalfDay(EndDate);

                    if (IsPublicHoliday(EndDate)) leaveDaysUsed += 0m;
                    else if (isEndDateAHalfDay) leaveDaysUsed += 0.5m;
                    else leaveDaysUsed += 1m;
                }

                
                Console.WriteLine($"\n\n\nInitial Leave Days Used (after start and end date calculation): {leaveDaysUsed}\n\n\n");

                for (DateTime date = StartDate.Date.AddDays(1); date < EndDate.Date; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || IsPublicHoliday(date))
                    {
                        continue; // Skip weekends
                    }

                    leaveDaysUsed += 1m;

                    Console.WriteLine($"date: {date}    leaveDaysUsed:  {leaveDaysUsed}");

    
                }


            }

            Console.WriteLine($"\n\n\nTotal Leave Days Used: {leaveDaysUsed}\n\n\n");
            return leaveDaysUsed;

        } // function GetEffectiveLeaveDays ends

    }
}
