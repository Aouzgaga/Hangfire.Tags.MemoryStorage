# Hangfire.Tags.MemoryStorage
Add tags to Hangfire backgroundjobs stored in memory

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
