using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Hangfire.Tags.Dashboard.Monitoring;
using Hangfire.Tags.Storage;

namespace Hangfire.Tags.MemoryStorage
{
    public class MemoryTagsServiceStorage : ObsoleteBaseStorage, ITagsServiceStorage
    {
        public override IEnumerable<TagDto> SearchWeightedTags(JobStorage jobStorage, string tag, string setKey)
        {
            var key = tag == null ? setKey : $"{setKey}:{tag}";
            var connection = GetConnection(jobStorage);
            var values = connection.GetAllItemsFromSet(key);
            return values.GroupBy(s => s).Select(g => new TagDto
            {
                Tag = g.Key,
                Amount = g.Count(),
                Percentage = g.LongCount() / values.Count() * 100
            });
        }
        private Hangfire.MemoryStorage.MemoryStorageConnection GetConnection(JobStorage storage)
        {
            return storage.GetConnection() as Hangfire.MemoryStorage.MemoryStorageConnection;
        }

        public override int GetJobCount(JobStorage jobStorage, string[] tags, string stateName = null)
        {
            var connection = GetConnection(jobStorage);
            return tags.Sum(t => (int)connection.GetSetCount(t));
        }
      
        public override IDictionary<string, int> GetJobStateCount(JobStorage jobStorage, string[] tags, int maxTags = 50)
        {
            var connection = GetConnection(jobStorage);
            return tags
                .SelectMany(t => connection.GetAllItemsFromSet(t))
                .GroupBy(j => connection.GetJobData(j).State)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public override JobList<MatchingJobDto> GetMatchingJobs(JobStorage jobStorage, string[] tags, int from, int count, string stateName = null)
        {
            var monitoringApi = jobStorage.GetMonitoringApi();

            var connection = GetConnection(jobStorage);
            var items = tags.SelectMany(t => connection.GetAllItemsFromSet(t));
            var jobs = items.GroupBy(i => i).Select(g => new { Job = g.Key, JobData = connection.GetJobData(g.Key) }).OrderBy(j => j.Job).AsEnumerable();

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

        public override ITagsTransaction GetTransaction(IWriteOnlyTransaction transaction)
        {
            return new MemoryTagsTransaction();
        }

        public override IEnumerable<string> SearchRelatedTags(JobStorage jobStorage, string tag, string setKey = "tags")
        {
            throw new NotImplementedException();
        }
    }
}
