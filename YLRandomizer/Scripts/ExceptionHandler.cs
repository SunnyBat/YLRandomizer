using UnityEngine;

namespace YLRandomizer.Scripts
{
    public class ExceptionHandler : MonoBehaviour
    {
        public string output = "";
        public string stack = "";

        void OnEnable()
        {
            Application.logMessageReceivedThreaded += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceivedThreaded -= HandleLog;
        }

        private void HandleLog(string condition, string stackTrace, UnityEngine.LogType type)
        {
            output = condition;
            stack = stackTrace;
        }
    }
}
