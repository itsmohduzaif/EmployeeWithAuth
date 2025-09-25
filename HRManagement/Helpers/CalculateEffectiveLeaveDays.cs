// This class will calculate effective leave days considering weekends and hours taken in a day(half day leave)
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;

namespace HRManagement.Helpers
{
    public static class CalculateEffectiveLeaveDays
    {
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

        public static decimal GetEffectiveLeaveDays(DateTime StartDate, DateTime EndDate)
        {
            decimal leaveDaysUsed = 0m;

            if (StartDate.Date == EndDate.Date)
            {
                if (StartDate.DayOfWeek == DayOfWeek.Saturday || StartDate.DayOfWeek == DayOfWeek.Sunday) return 0m;
                //var totalHours = (EndDate - StartDate).TotalHours;

                //if (totalHours <= 4)
                //{
                //    return 0.5m; // Half day leave
                //}
                //else
                //{
                //    return 1m; // Full day leave
                //}

                bool isStartDateAHalfDay = CheckIfStartDateIsHalfDay(StartDate);
                if (isStartDateAHalfDay) return 0.5m;

                bool isEndDateAHalfDay = CheckIfEndDateIsHalfDay(EndDate);

                if (isEndDateAHalfDay) return 0.5m;

                return 1m; // Full day leave if neither is half day


            }
            else
            {
                if (StartDate.DayOfWeek != DayOfWeek.Saturday && StartDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    bool isStartDateAHalfDay = CheckIfStartDateIsHalfDay(StartDate);

                    if (isStartDateAHalfDay) leaveDaysUsed += 0.5m;
                    else leaveDaysUsed += 1m;
                }


                if (EndDate.DayOfWeek != DayOfWeek.Saturday && EndDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    bool isEndDateAHalfDay = CheckIfEndDateIsHalfDay(EndDate);

                    if (isEndDateAHalfDay) leaveDaysUsed += 0.5m;
                    else leaveDaysUsed += 1m;
                }

                
                Console.WriteLine($"\n\n\nInitial Leave Days Used (after start and end date calculation): {leaveDaysUsed}\n\n\n");

                for (DateTime date = StartDate.Date.AddDays(1); date < EndDate.Date; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
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
