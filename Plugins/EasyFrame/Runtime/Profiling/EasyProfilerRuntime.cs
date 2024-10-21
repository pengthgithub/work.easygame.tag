using Unity.Profiling;
using UnityEngine.Profiling;

namespace Easy
{
#if UNITY_EDITOR  
    internal static class EasyProfilerRuntime
    {
        private static ProfilerCounterValue<int> SceneLoadCounter = new ProfilerCounterValue<int>(ProfilerCategory.Loading, "Reprents Count", ProfilerMarkerDataUnit.Count);
        public static void Initialise()
        {
            SceneLoadCounter.Value = 0;
        }

        public static void ReprentCreate()
        {
            SceneLoadCounter.Value += 1;
        }
        public static void ReprentRemove()
        {
            SceneLoadCounter.Value -= 1;
        }
    }
#endif
}