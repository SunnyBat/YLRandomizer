using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YLRandomizer.GameAnalysis
{
    /// <summary>
    /// Used to track specific pieces of data that normally come from Archipelago. For example,
    /// the Trowza purchases normally come from Archipelago's unlocked locations list. However,
    /// this doesn't actually update until the location is sent then the server responds and
    /// confirms that it has been sent. Because of this, there is a variable amount of time
    /// where the shop can have incorrect data, and this time just so happens to line up with
    /// the refresh call to the shop interface.
    /// 
    /// This aims to fix issues like this by temporarily tracking this data internally until
    /// Archipelago sends the location to us.
    /// </summary>
    class IntermediateActionTracker
    {
        private static HashSet<long> checkedLocationsList = new HashSet<long>();

        public static bool HasLocationBeenCheckedLocally(long locationId)
        {
            return checkedLocationsList.Contains(locationId);
        }

        public static void AddLocallyCheckedLocation(long locationId)
        {
            checkedLocationsList.Add(locationId);
        }

        public static void RemoveLocallyCheckedLocation(long locationId)
        {
            checkedLocationsList.Remove(locationId);
        }
    }
}
