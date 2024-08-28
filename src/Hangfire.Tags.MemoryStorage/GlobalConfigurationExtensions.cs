using Hangfire.Tags.Dashboard;

namespace Hangfire.Tags.MemoryStorage
{
    /// <summary>
    /// Provides extension methods to setup Hangfire.Tags
    /// </summary>
    public static class GlobalConfigurationExtensions
    {
        /// <summary>
        /// Configures Hangfire to use Tags.
        /// </summary>
        /// <param name="configuration">Global configuration</param>
        /// <param name="options">Options for tags</param>
        /// <returns></returns>
        public static IGlobalConfiguration UseTagsWithMemory(this IGlobalConfiguration configuration, TagsOptions options = null, JobStorage jobStorage = null)
        {
            options = options ?? new TagsOptions();

            var storage = new MemoryTagsServiceStorage();
            (jobStorage ?? JobStorage.Current).Register(options, storage);

            var config = configuration.UseTags(options);
            return config;
        }
    }
}
