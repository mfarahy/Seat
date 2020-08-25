using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Configs
{
    public class ExceptionElement : ConfigurationElement
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

        [ConfigurationProperty("text", IsRequired = true)]
        public string Text
        {
            get
            {
                return (string)base["text"];

            }
            set
            {
                base["text"] = value;
            }
        }

    }

    public class ExceptionElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExceptionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExceptionElement)element).Name;
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
