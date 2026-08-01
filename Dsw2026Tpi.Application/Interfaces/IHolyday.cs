using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Interfaces
{
    public interface IHoliday
    {
        bool IsHoliday(DateOnly date);
    }
}
