using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ICOPE.Immersive.Tracking
{
    /// <summary>Runs queued async work one item at a time, in call order. A failing item is logged and does not stop the queue.</summary>
    public class SerialTaskQueue
    {
        Task tail = Task.CompletedTask;

        public void Enqueue(Func<Task> work)
        {
            tail = Run(tail, work);
        }

        static async Task Run(Task previous, Func<Task> work)
        {
            await previous;

            try
            {
                await work();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}
