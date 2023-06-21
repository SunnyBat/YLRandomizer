using System;
using System.Collections.Generic;
using YLRandomizer.GameAnalysis;

namespace YLRandomizer.Data
{
    public class UserMessages : IUserMessages
    {
        private static readonly TimeSpan ANGLE_UPDATE_TIME = TimeSpan.FromMilliseconds(50);
        private static readonly TimeSpan OBJECTS_UPDATE_TIME = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan MESSAGE_DISPLAY_TIME = TimeSpan.FromSeconds(10);
        private Queue<string> _messages = new Queue<string>();
        private Queue<DateTime> _messageTimes = new Queue<DateTime>();
        private DateTime lastUpdatedAngleTime = DateTime.MinValue;
        private DateTime lastUpdatedObjectsTime = DateTime.MinValue;
        private string closestPagieMessage = "";
        private PlayerDev player;
        private CameraManager camera;
        private PagiePickup[] pagies;

        public string[] GetMessages()
        {
            while (_messages.Count > 0 && DateTime.Now - _messageTimes.Peek() > MESSAGE_DISPLAY_TIME)
            {
                // Dequeue all expired messages; since it's a queue and all messages will be visible
                // for the same amount of time, once we encounter one unexpired message, we can assume
                // that the rest of them in the queue will also be unexpired.
                _messages.Dequeue();
                _messageTimes.Dequeue();
            }
#if DEBUG
            if (DateTime.Now - lastUpdatedObjectsTime > OBJECTS_UPDATE_TIME)
            {
                player = UnityEngine.Object.FindObjectOfType<PlayerDev>();
                camera = UnityEngine.Object.FindObjectOfType<CameraManager>();
                pagies = UnityEngine.Object.FindObjectsOfType<PagiePickup>();
                lastUpdatedObjectsTime = DateTime.Now;
            }
            if (DateTime.Now - lastUpdatedAngleTime > ANGLE_UPDATE_TIME)
            {
                lastUpdatedAngleTime = DateTime.Now; // Update early so we get more consistent times between updates
                var closestPagie = getClosestPagie();
                if (closestPagie != null)
                {
                    var playerCam = camera?.GetCurrentCamera();
                    if (playerCam != null && player != null)
                    {
                        try
                        {
                            var directionVector = closestPagie.gameObject.transform.position - player.transform.position;
                            directionVector.Normalize();
                            var fcType = playerCam.GetType();
                            var fcVar = fcType.GetField("mLookDirection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                            var lookDir = (UnityEngine.Vector3)fcVar.GetValue(playerCam);
                            var angleTo = UnityEngine.Vector3.Angle(directionVector, lookDir);
                            var distanceFromPlayer = UnityEngine.Vector3.Distance(player.transform.position, closestPagie.gameObject.transform.position);
                            closestPagieMessage = $"i={closestPagie.identifier} :: angle={angleTo} :: dist={distanceFromPlayer} :: name={closestPagie.name}";
                        }
                        catch { } // Mostly for when camera is controlled by cutscene, since value we get by reflecting into it (I think that's where it dies...) is invalid
                    }
                    else
                    {
                        closestPagieMessage = "<Unable to calculate pagie data>";
                    }
                }
                else
                {
                    closestPagieMessage = "<No uncollected pagies found>";
                }
            }
            var ret = new List<string>();
            ret.Add(closestPagieMessage);
            ret.Add($"Is in game: {GameState.IsInGame()}");
            ret.AddRange(_messages);
            return ret.ToArray();
#else
            return _messages.ToArray();
#endif
        }

        public void AddMessage(string message)
        {
            _messages.Enqueue(message);
            _messageTimes.Enqueue(DateTime.Now);
        }

        private PagiePickup getClosestPagie()
        {
            try
            {
                var playerPosition = player?.transform.position;
                if (playerPosition != null && pagies?.Length > 0)
                {
                    var closestPagieIndex = -1;
                    float closestDistance = 9999999f;
                    for (int i = 0; i < pagies.Length; i++)
                    {
                        if (pagies[i].GetCollectionStatus() != Savegame.CollectionStatus.Collected)
                        {
                            var distanceAway = UnityEngine.Vector3.Distance((UnityEngine.Vector3) playerPosition, pagies[i].gameObject.transform.position);
                            if (closestDistance > distanceAway)
                            {
                                closestPagieIndex = i;
                                closestDistance = distanceAway;
                            }
                        }
                    }
                    if (closestPagieIndex >= 0)
                    {
                        return pagies[closestPagieIndex];
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
