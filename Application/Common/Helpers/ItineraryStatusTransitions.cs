using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Enums;

namespace TouRest.Application.Common.Helpers
{
    public static class ItineraryStatusTransitions
    {
        private static readonly Dictionary<ItineraryStatus, ItineraryStatus[]> AllowedTransitions = new()
        {
            { ItineraryStatus.Draft,    new[] { ItineraryStatus.Active                            } },
            { ItineraryStatus.Active,   new[] { ItineraryStatus.Inactive, ItineraryStatus.Draft   } },
            { ItineraryStatus.Inactive, Array.Empty<ItineraryStatus>()                              },
        };

        public static bool CanTransition(ItineraryStatus current, ItineraryStatus next)
            => AllowedTransitions.TryGetValue(current, out var allowed) && allowed.Contains(next);
    }
}
