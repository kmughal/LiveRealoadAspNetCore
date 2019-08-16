namespace LiveReloading.Middleware
{
    using System.IO;
    
    public class LiveReloadingSettings
    {
        public string WebProjectPath { get; set; }
        public string FilesToWatchForChange { get; set; }
        public bool IncludeSubDirectories  { get;set;}

        public LiveReloadingSettings()
        {
            WebProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "../LiveRelaoding.Web");
            FilesToWatchForChange = "*.*";
            IncludeSubDirectories = true;
        }

        public string DefaultFilterPattern => "*.*";

        public string DefaultProjectPath => Path.Combine(Directory.GetCurrentDirectory());
    }
}