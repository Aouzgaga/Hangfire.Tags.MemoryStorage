using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Hangfire.Tags.Dashboard.Monitoring;
using Hangfire.Tags.Storage;

namespace Hangfire.Tags.MemoryStorage
{
    public class MemoryTagsServiceStorage : ITagsServiceStorage
    {
        public MemoryTagsServiceStorage()
        {
        }

        public IEnumerable<TagDto> SearchWeightedTags(string tag, string setKey)
        {
            var storage = GetStorage();
            var key = tag == null ? setKey : $"{setKey}:{tag}";
            var values = storage.GetAllItemsFromSet(key);
            return values.GroupBy(s => s).Select(g => new TagDto
            {
                Tag = g.Key,
                Amount = g.Count(),
                Percentage = g.LongCount() / values.Count() * 100
            });
        }

        private Hangfire.MemoryStorage.MemoryStorageConnection GetStorage()
        {
            return JobStorage.Current.GetConnection() as Hangfire.MemoryStorage.MemoryStorageConnection;
        }

        public IEnumerable<string> SearchTags(string tag, string setKey)
        {
            throw new NotSupportedException();
        }

        public int GetJobCount(string[] tags, string stateName = null)
        {
            var storage = GetStorage();
            return tags.Sum(t => (int)storage.GetSetCount(t));
        }

        public IDictionary<string, int> GetJobStateCount(string[] tags, int maxTags = 50)
        {
            var storage = GetStorage();
            return tags
                .SelectMany(t => storage.GetAllItemsFromSet(t))
                .GroupBy(j => storage.GetJobData(j).State)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public JobList<MatchingJobDto> GetMatchingJobs(string[] tags, int from, int count, string stateName = null)
        {
            var storage = GetStorage();
            var monitoringApi = JobStorage.Current.GetMonitoringApi();

            var items = tags.SelectMany(t => storage.GetAllItemsFromSet(t));
            var jobs = items.GroupBy(i => i).Select(g => new { Job = g.Key, JobData = storage.GetJobData(g.Key) }).OrderBy(j => j.Job).AsEnumerable();

            if (!string.IsNullOrEmpty(stateName))
            {
                jobs = jobs.Where(j => j.JobData.State == stateName);
            }

            var paginated = jobs.Skip(from).Take(count);
            var dtos = paginated.ToDictionary(j => j.Job, j => new MatchingJobDto
            {
                Job = j.JobData.Job,
                State = j.JobData.State,
                CreatedAt = j.JobData.CreatedAt,
                ResultAt = monitoringApi.JobDetails(j.Job).History.Where(h => h.StateName == j.JobData.State).OrderByDescending(h => h.CreatedAt).First()?.CreatedAt
            });

            return new JobList<MatchingJobDto>(dtos);
        }

        public ITagsTransaction GetTransaction(IWriteOnlyTransaction transaction)
        {
            return new MemoryTagsTransaction();
        }
    }
}
