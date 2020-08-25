using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Configs
{
    public class QueueElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]

        public string Name
        {
            get
            {
                return (string)base["name"];

            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("capacity", IsRequired = true)]

        public int Capacity
        {
            get
            {
                return (int)base["capacity"];

            }
            set
            {
                base["capacity"] = value;
            }
        }

    
    }

    public class QueueElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new QueueElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((QueueElement)element).Name;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
    }

}
