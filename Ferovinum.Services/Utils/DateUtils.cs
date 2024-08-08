using Ferovinum.Domain;

namespace Ferovinum.Services.Utils
{
    public static class DateUtils
    {
        public static IQueryable<Transaction> ApplyDatesOptionally(this IQueryable<Transaction> entities, DateTime? from, DateTime? to)
        {
            if (from != null)
            {
                entities = entities.Where(tr => tr.Timestamp >= from);
            }
            if (to != null)
            {
                entities = entities.Where(tr => tr.Timestamp <= to);
            }
            return entities;
        }

        public static IQueryable<Transaction> ApplyDateOptionally(this IQueryable<Transaction> entities, DateTime? date)
        {
            if (date != null)
            {
                entities = entities.Where(tr => tr.Timestamp <= date);
            }
            return entities;
        }

        /// <summary>
        /// Returns the difference between dates in months.
        /// </summary>
        /// <param name="current">First considered date.</param>
        /// <param name="another">Second considered date.</param>
        /// <returns>The number of full months between the given dates.</returns>
        public static int DifferenceInMonths(this DateTime current, DateTime another)
        {
            DateTime previous, next;
            if (current > another)
            {
                previous = another;
                next = current;
            }
            else
            {
                previous = current;
                next = another;
            }

            return
                (next.Year - previous.Year) * 12     // multiply the difference in years by 12 months
              + next.Month - previous.Month          // add difference in months
              + (previous.Day <= next.Day ? 0 : -1); // if the day of the next date has not reached the day of the previous one, then the last month has not yet ended
        }
    }
}
