# Hangfire.Tags.MemoryStorage
Add tags (https://github.com/face-it/Hangfire.Tags) to Hangfire backgroundjobs stored in memory (https://github.com/perrich/Hangfire.MemoryStorage).

In .NET Core's Startup.cs:
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddHangfire(config =>
    {
        config.UseMemoryStorage();
        config.UseTagsWithMemory();
    });
}
```

Otherwise,
```c#
GlobalConfiguration.Configuration
    .UseTagsWithMemory()
    .UseMemoryStorage("connectionSting");
```
