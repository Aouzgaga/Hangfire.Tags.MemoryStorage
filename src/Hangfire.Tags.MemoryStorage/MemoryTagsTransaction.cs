using System;
using Hangfire.Tags.Storage;

namespace Hangfire.Tags.MemoryStorage
{
    internal class MemoryTagsTransaction : ITagsTransaction
    {

        public MemoryTagsTransaction()
        {
        }

        public void ExpireSetValue(string key, string value, TimeSpan expireIn)
        {
        }

        public void PersistSetValue(string key, string value)
        {
        }
    }
}
