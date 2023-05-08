using UnityEngine;
using YLRandomizer.Randomizer;

namespace YLRandomizer.Scripts
{
    public class ArchipelagoHeartbeat : MonoBehaviour
    {
        public const float TICK_DELAY_SECONDS = 0.1f;
        public static void CreateNewHeartbeat()
        {
            var heartbeatGameObject = new GameObject();
            heartbeatGameObject.AddComponent<ArchipelagoHeartbeat>();
            UnityEngine.GameObject.DontDestroyOnLoad(heartbeatGameObject);
        }

        void Start()
        {
            InvokeRepeating(nameof(_tick), 0, 0.5f);
        }

        private void _tick()
        {
            ManualSingleton<IRandomizer>.instance?.Tick();
        }
    }
}
