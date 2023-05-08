using System;
using System.Collections.Generic;

namespace YLRandomizer.Data
{
    public class UserMessages : IUserMessages
    {
        private static readonly TimeSpan MESSAGE_DISPLAY_TIME = TimeSpan.FromSeconds(10);
        private Queue<string> _messages = new Queue<string>();
        private Queue<DateTime> _messageTimes = new Queue<DateTime>();

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
            if (_messages.Count == 0)
            {
                return new string[] { "(DBG: No messages)" };
            }
            return _messages.ToArray();
        }

        public void AddMessage(string message)
        {
            _messages.Enqueue(message);
            _messageTimes.Enqueue(DateTime.Now);
        }
    }
}
