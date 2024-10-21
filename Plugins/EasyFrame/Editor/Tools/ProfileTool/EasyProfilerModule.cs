using Unity.Profiling;
using Unity.Profiling.Editor;

namespace ProfileTool
{
    [System.Serializable]
    [ProfilerModuleMetadata("Easy Details")]
    public class EasyProfilerModule: ProfilerModule
    {
        private static readonly ProfilerCounterDescriptor[] Descriptors = new ProfilerCounterDescriptor[]
        {
            new ProfilerCounterDescriptor("Reprents Count", ProfilerCategory.Loading)
        };

        private static readonly string[] AutoEnabledCategoryNames = new string[]
        {
            ProfilerCategory.Memory.Name,
        };
        
#if UNITY_2022_2_OR_NEWER
        public override ProfilerModuleViewController CreateDetailsViewController()
        {
            return new AddressablesProfilerViewController(ProfilerWindow);
        }
#else
        public override ProfilerModuleViewController CreateDetailsViewController()
        {
            return new EasyProfilerModuleViewController(ProfilerWindow);
        }
#endif
        public EasyProfilerModule() : base(Descriptors, ProfilerModuleChartType.Line, AutoEnabledCategoryNames)
        {
           
        }
        
    }
}