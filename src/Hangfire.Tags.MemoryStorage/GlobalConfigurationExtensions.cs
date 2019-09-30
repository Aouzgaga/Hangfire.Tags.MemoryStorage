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
        public static IGlobalConfiguration UseTagsWithMemory(this IGlobalConfiguration configuration, TagsOptions options = null)
        {
            options = options ?? new TagsOptions();

            options.Storage = new MemoryTagsServiceStorage();
            var config = configuration.UseTags(options);
            return config;
        }
    }
}
