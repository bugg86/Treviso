﻿namespace Treviso.Domain.Data.Models;

public class TimerInfo
{
    public ScheduleStatus ScheduleStatus { get; set; } = null!;

    public bool IsPastDue { get; set; }
}