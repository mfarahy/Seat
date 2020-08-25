using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml;

namespace SeatWebApp.Security
{
    [SectionName("security")]
    public class SecurityConfig : BaseConfigurationSection<SecurityConfig>
    {
        private ConfigurationProperty _rolesProperty;

        public SecurityConfig()
        {
            _rolesProperty = new ConfigurationProperty(null, typeof(RoleElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        }

        [ConfigurationProperty("", IsDefaultCollection = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(RoleElement), AddItemName = "role", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public RoleElementCollection Roles
        {
            get
            {
                return (RoleElementCollection)base[_rolesProperty];
            }
        }
    }

    public class RoleElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RoleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RoleElement)element).Key;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
    }

    public class RoleElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get
            {
                return (string)(base["key"]);
            }
            set
            {
                base["key"] = value;
            }
        }

        private ConfigurationProperty _opsProperty;

        public RoleElement()
        {
            _opsProperty = new ConfigurationProperty(null, typeof(OperationElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        }

        [ConfigurationProperty("", IsDefaultCollection = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        [ConfigurationCollection(typeof(OperationElement), AddItemName = "op", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public OperationElementCollection Operations
        {
            get
            {
                return (OperationElementCollection)base[_opsProperty];
            }
        }

    }

    public class OperationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new OperationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OperationElement)element).Key;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
    }

    public class OperationElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get
            {
                return (string)(base["key"]);
            }
            set
            {
                base["key"] = value;
            }
        }


    }
}