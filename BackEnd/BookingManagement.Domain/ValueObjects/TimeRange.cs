namespace BookingManagement.Domain.ValueObjects
{
    public readonly record struct TimeRange
    {
        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime Start { get; }

        public DateTime End { get; }
    }
}
