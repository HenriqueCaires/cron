using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cron
{
    public interface ICronSchedule
    {
        bool IsValid(string expression);
        bool IsTime(DateTime date_time);
    }

    public class CronSchedule : ICronSchedule
    {
        #region Readonly Class Members

        readonly static Regex DividedRegex = new Regex(@"(\*/\d+)");
        readonly static Regex RangeRegex = new Regex(@"(\d+\-\d+)\/?(\d+)?");
        readonly static Regex WildRegex = new Regex(@"(\*)");
        readonly static Regex ListRegex = new Regex(@"(((\d+,)*\d+)+)");
        readonly static Regex ValidationRegex = new Regex(DividedRegex + "|" + RangeRegex + "|" + WildRegex + "|" + ListRegex);

        #endregion

        #region Private Instance Members

        private readonly string _expression;
        public List<int> Minutes;
        public List<int> Hours;
        public List<int> DaysOfMonth;
        public List<int> Months;
        public List<int> DaysOfWeek;

        #endregion

        #region Public Constructors

        public CronSchedule()
        {
        }

        public CronSchedule(string expressions)
        {
            _expression = expressions;
            Generate();
        }

        #endregion

        #region Public Methods

        private bool IsValid()
        {
            return IsValid(_expression);
        }

        public bool IsValid(string expression)
        {
            var matches = ValidationRegex.Matches(expression);
            return matches.Count > 0;
        }

        public bool IsTime(DateTime date_time)
        {
            return Minutes.Contains(date_time.Minute) &&
                   Hours.Contains(date_time.Hour) &&
                   DaysOfMonth.Contains(date_time.Day) &&
                   Months.Contains(date_time.Month) &&
                   DaysOfWeek.Contains((int)date_time.DayOfWeek);
        }

        private void Generate()
        {
            if (!IsValid()) return;

            var matches = ValidationRegex.Matches(this._expression);

            GenerateMinutes(matches[0].ToString());

            if (matches.Count > 1)
                GenerateHours(matches[1].ToString());
            else
                GenerateHours("*");

            if (matches.Count > 2)
                GenerateDaysOfMonth(matches[2].ToString());
            else
                GenerateDaysOfMonth("*");

            if (matches.Count > 3)
                GenerateMonths(matches[3].ToString());
            else
                GenerateMonths("*");

            if (matches.Count > 4)
                GenerateDaysOfWeek(matches[4].ToString());
            else
                GenerateDaysOfWeek("*");
        }

        private void GenerateMinutes(string match)
        {
            Minutes = GenerateValues(match, 0, 60);
        }

        private void GenerateHours(string match)
        {
            Hours = GenerateValues(match, 0, 24);
        }

        private void GenerateDaysOfMonth(string match)
        {
            DaysOfMonth = GenerateValues(match, 1, 32);
        }

        private void GenerateMonths(string match)
        {
            Months = GenerateValues(match, 1, 13);
        }

        private void GenerateDaysOfWeek(string match)
        {
            DaysOfWeek = GenerateValues(match, 0, 7);
        }

        private List<int> GenerateValues(string configuration, int start, int max)
        {
            if (DividedRegex.IsMatch(configuration)) return DividedArray(configuration, start, max);
            if (RangeRegex.IsMatch(configuration)) return RangeList(configuration);
            if (WildRegex.IsMatch(configuration)) return WildList(configuration, start, max);
            if (ListRegex.IsMatch(configuration)) return ListList(configuration);

            return new List<int>();
        }

        private List<int> DividedArray(string configuration, int start, int max)
        {
            if (!DividedRegex.IsMatch(configuration))
                return new List<int>();

            var ret = new List<int>();
            var split = configuration.Split("/".ToCharArray());
            var divisor = int.Parse(split[1]);

            for (int i = start; i < max; ++i)
                if (i % divisor == 0)
                    ret.Add(i);

            return ret;
        }

        private List<int> RangeList(string configuration)
        {
            if (!RangeRegex.IsMatch(configuration))
                return new List<int>();

            var ret = new List<int>();
            var split = configuration.Split("-".ToCharArray());
            var start = int.Parse(split[0]);
            var end = 0;
            if (split[1].Contains("/"))
            {
                split = split[1].Split("/".ToCharArray());
                end = int.Parse(split[0]);
                int divisor = int.Parse(split[1]);

                for (int i = start; i < end; ++i)
                    if (i % divisor == 0)
                        ret.Add(i);
                return ret;
            }
            else
                end = int.Parse(split[1]);

            for (int i = start; i <= end; ++i)
                ret.Add(i);

            return ret;
        }

        private List<int> WildList(string configuration, int start, int max)
        {
            if (!WildRegex.IsMatch(configuration))
                return new List<int>();

            var ret = new List<int>();

            for (var i = start; i < max; ++i)
                ret.Add(i);

            return ret;
        }

        private List<int> ListList(string configuration)
        {
            if (!ListRegex.IsMatch(configuration))
                return new List<int>();

            return configuration.Split(",".ToCharArray()).Select(s => int.Parse(s)).ToList();
        }

        #endregion
    }
}
