using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Configs
{
    [SectionName("jobService")]
    public class JobServiceConfig : BaseConfigurationSection<JobServiceConfig>
    {
        private ConfigurationProperty _ruleProeprty;

        [ConfigurationProperty("secret_key", IsRequired = false)]
        public string SecretKey { get { return (string)(base["secret_key"]); } set { base["secret_key"] = value; } }

        [ConfigurationProperty("delay", IsRequired = false)]
        public int DelayTime { get { return (int)(base["delay"]); } set { base["delay"] = value; } }

        [ConfigurationProperty("debug", IsRequired = false)]
        public bool Debug { get { return (bool)(base["debug"]); } set { base["debug"] = value; } }

        [ConfigurationProperty("server", IsRequired = false)]
        public string Server { get { return (string)(base["server"]); } set { base["server"] = value; } }

        [ConfigurationProperty("retryCount", IsRequired = false, DefaultValue = 7)]
        public int RetryCount { get { return (int)(base["retryCount"]); } set { base["retryCount"] = value; } }

        public JobServiceConfig()
        {
            _ruleProeprty = new ConfigurationProperty(null, typeof(QueueElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        }


        [ConfigurationProperty("", IsDefaultCollection = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(QueueElement), AddItemName = "queue", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public QueueElementCollection Queue
        {
            get
            {
                return (QueueElementCollection)base[_ruleProeprty];
            }
        }

        [ConfigurationProperty("rerunables")]
        [ConfigurationCollection(typeof(ExceptionElement), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public ExceptionElementCollection Rerunables
        {
            get
            {
                return (ExceptionElementCollection)base["rerunables"];
            }
        }


        [ConfigurationProperty("negligibles")]
        [ConfigurationCollection(typeof(ExceptionElement), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public ExceptionElementCollection Negligibles
        {
            get
            {
                return (ExceptionElementCollection)base["negligibles"];
            }
        }

        [ConfigurationProperty("waitTime", DefaultValue = 5)]
        public int PeriodicallyWaitTime { get { return (int)(base["waitTime"]); } set { base["waitTime"] = value; } }

        public int GetQueueCapacity(string key)
        {
            foreach (QueueElement q in Queue)
            {
                if (q.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    return q.Capacity;
            }

            if (key.Equals(Constants.BackstageJobs.Queue.Default, StringComparison.CurrentCultureIgnoreCase))
                return 1;

            return GetQueueCapacity(Constants.BackstageJobs.Queue.Default);
        }

        internal bool IsRerunable(Exception exception)
        {
            if (Rerunables == null || Rerunables.Count == 0 || exception == null) return false;

            var text = exception.SerializeToString();
            foreach (ExceptionElement rerunable in Rerunables)
            {
                if (String.IsNullOrWhiteSpace(rerunable.Text)) continue;
                bool found = true;
                int lastIndex = 0;
                foreach (var part in rerunable.Text.Split('|'))
                {
                    lastIndex = text.IndexOf(part, lastIndex);
                    if (lastIndex < 0)
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return true;
            }

            return false;
        }

        public bool IsNegligible(Exception exception)
        {
            if (Negligibles == null || Negligibles.Count == 0 || exception == null) return false;

            var text = exception.SerializeToString();
            foreach (ExceptionElement negi in Negligibles)
            {
                if (String.IsNullOrWhiteSpace(negi.Text)) continue;
                bool found = true;
                int lastIndex = 0;
                foreach (var part in negi.Text.Split('|'))
                {
                    lastIndex = text.IndexOf(part, lastIndex);
                    if (lastIndex < 0)
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return true;
            }

            return false;
        }
    }
}
