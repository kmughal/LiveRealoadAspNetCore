namespace LiveReloading.Middleware
{
    using LiveReloading.Middleware.Middleware;
    using System.IO;
    using System.Threading.Tasks;
    public class DirectoryWatcher
    {
        public static void StatWatching(LiveReloadingSettings settings)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = string.IsNullOrWhiteSpace(settings.WebProjectPath) ? settings.DefaultProjectPath  : Path.GetFullPath(settings.WebProjectPath);
            watcher.Filter = string.IsNullOrWhiteSpace(settings.FilesToWatchForChange) ? settings.DefaultFilterPattern : settings.FilesToWatchForChange;

            watcher.Changed += NotifyFileChanged;
            watcher.Created += NotifyFileChanged;
            watcher.Deleted += NotifyFileChanged;

            watcher.IncludeSubdirectories = settings.IncludeSubDirectories;
            watcher.EnableRaisingEvents = true;
        }

        static void NotifyFileChanged(object source, FileSystemEventArgs e) => Task.Factory.StartNew(() => InMemoryWebSocketsStore.NotifyChange());
    }
}