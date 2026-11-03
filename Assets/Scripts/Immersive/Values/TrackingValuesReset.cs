using UnityEngine;

namespace ICOPE.Immersive.Values
{
    /// <summary>Resets the tracking value assets before TrainingRuntime restores stored progress into them.</summary>
    public static class TrackingValuesReset
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void ResetOnPlay()
        {
            TrackingValues.TryLoad()?.Reset();
        }
    }
}
