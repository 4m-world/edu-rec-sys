using CodeMatrix.Mepd.Infrastructure.Persistence.Contexts;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CodeMatrix.Mepd.Host.Controllers
{
    [AllowAnonymous]
    public class InfoController : VersionNeutralApiController
    {        
        /// <summary>
        /// Get enviroment details 
        /// </summary>
        /// <returns></returns>
        [Route("/env")]
        [MustHavePermission(RootPermissions.SystemInformation.View)]
        public Dictionary<string, string> Env()
        {
            var vars = Environment.GetEnvironmentVariables();
            var dict = new Dictionary<string, string>();
            foreach (var key in vars.Keys)
            {
                dict[key.ToString()] = vars[key].ToString();
            }

            return dict;
        }

        /// <summary>
        /// Get System information
        /// </summary>
        [Route("/System")]
        [MustHavePermission(RootPermissions.SystemInformation.View)]
        public SystemInfoModel SystemInfo()
        {
            var model = new SystemInfoModel
            {
                ServerLocalTime = DateTime.Now,
                UtcTime = DateTime.UtcNow,
                ServerTimeZone = TimeZoneInfo.Local.StandardName,
                AspNetInfo = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                OperatingSystem = $"{RuntimeInformation.OSDescription} {RuntimeInformation.ProcessArchitecture.ToString().ToLower()}",
            };

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fi = new FileInfo(assembly.Location);
                model.AppDate = fi.LastWriteTime.ToLocalTime();
                model.UsedMemorySize = GetPrivateBytes();
            }
            catch
            {
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var loadedAssembly = new SystemInfoModel.LoadedAssembly
                {
                    FullName = assembly.FullName
                };

                if (!assembly.IsDynamic)
                {
                    try
                    {
                        loadedAssembly.Location = assembly.Location;
                    }
                    catch
                    {

                    }
                }

                model.LoadedAssemblies.Add(loadedAssembly);
            }

            return model;
        }

        private static long GetPrivateBytes()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var process = Process.GetCurrentProcess();
            process.Refresh();

            return process.PrivateMemorySize64;
        }
    }

    public class SystemInfoModel
    {
        public string AspNetInfo { get; set; }
        public string AppVersion { get; set; }
        public DateTime AppDate { get; set; }
        public string OperatingSystem { get; set; }
        public DateTime ServerLocalTime { get; set; }
        public string ServerTimeZone { get; set; }
        public DateTime UtcTime { get; set; }
        public List<LoadedAssembly> LoadedAssemblies { get; set; } = new();
        public long DatabaseSize { get; set; }
        //public string DatabaseSizeString => (DatabaseSize == 0 ? string.Empty : Prettifier.HumanizeBytes(DatabaseSize));
        public long UsedMemorySize { get; set; }
        //public string UsedMemorySizeString => Prettifier.HumanizeBytes(UsedMemorySize);
        public string DataProviderFriendlyName { get; set; }
        public bool ShrinkDatabaseEnabled { get; set; }
        public Dictionary<string, long> MemoryCacheStats { get; set; } = new Dictionary<string, long>();
        public class LoadedAssembly
        {
            public string FullName { get; set; }
            public string Location { get; set; }
        }
    }
}
