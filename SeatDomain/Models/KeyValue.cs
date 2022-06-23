//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using Exir.Framework.Common;
using System.Linq.Expressions;
using System.Linq;
using ProtoBuf;
using System.ComponentModel.DataAnnotations.Schema;
using Exir.Framework.Common.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SeatDomain.Models
{
            public interface IKeyValue{
    
     long KeyValuePK{
      get;
      set;
      }
    
     string SetKey{
      get;
      set;
      }
    
     System.Guid RowKey{
      get;
      set;
      }
    
     string Path{
      get;
      set;
      }
    
     string Key{
      get;
      set;
      }
    
     string Value{
      get;
      set;
      }
    
     byte Type{
      get;
      set;
      }
    
    }// interface
    
    [Table("KeyValues")]
    [Serializer(typeof(ProtobufSerializer))]
    [ProtoContract]
    [Exir.Framework.Common.Caching.CacheableKey( new string[]{nameof(KeyValuePK)} )]
    [Entity]
    [Serializable]
    [DataContract(IsReference = true)]
    public partial class KeyValue:IEntityBase, IKeyValue,IObjectWithChangeTracker, INotifyPropertyChanged, ICloneable
    {
        #region Primitive Properties
    [Key]
    [Required]
    	[ProtoMember(1)]
        [DataMember]
        public virtual long KeyValuePK
        {
            get { return _keyValuePK; }
            set
            {
                if (_keyValuePK != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && !Typing.IsEquals(_keyValuePK,Typing.GetDefaultValue(typeof(long))))
                    {
                        throw new InvalidOperationException("The property 'KeyValuePK' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _keyValuePK = value;
                    OnPropertyChanged("KeyValuePK");
                }
            }
        }
        protected long _keyValuePK;
    [MaxLength(50)]
    [Required]
    	[ProtoMember(2)]
        [DataMember]
        public virtual string SetKey
        {
            get { return _setKey; }
            set
            {
                if (_setKey != value)
                {
                    ChangeTracker.RecordOriginalValue("SetKey", _setKey);
                        if (!ChangeTracker.OriginalValues.ContainsKey("SetKey"))
                            ChangeTracker.RecordOriginalValue("SetKey", _setKey);
                            
                    _setKey = value;
                    OnPropertyChanged("SetKey");
                }
            }
        }
        protected string _setKey;
    [Required]
    	[ProtoMember(3)]
        [DataMember]
        public virtual System.Guid RowKey
        {
            get { return _rowKey; }
            set
            {
                if (_rowKey != value)
                {
                    ChangeTracker.RecordOriginalValue("RowKey", _rowKey);
                        if (!ChangeTracker.OriginalValues.ContainsKey("RowKey"))
                            ChangeTracker.RecordOriginalValue("RowKey", _rowKey);
                            
                    _rowKey = value;
                    OnPropertyChanged("RowKey");
                }
            }
        }
        protected System.Guid _rowKey;
    [MaxLength(1000)]
    	[ProtoMember(4)]
        [DataMember]
        public virtual string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    ChangeTracker.RecordOriginalValue("Path", _path);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Path"))
                            ChangeTracker.RecordOriginalValue("Path", _path);
                            
                    _path = value;
                    OnPropertyChanged("Path");
                }
            }
        }
        protected string _path;
    [MaxLength(50)]
    [Required]
    	[ProtoMember(5)]
        [DataMember]
        public virtual string Key
        {
            get { return _key; }
            set
            {
                if (_key != value)
                {
                    ChangeTracker.RecordOriginalValue("Key", _key);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Key"))
                            ChangeTracker.RecordOriginalValue("Key", _key);
                            
                    _key = value;
                    OnPropertyChanged("Key");
                }
            }
        }
        protected string _key;
    	[ProtoMember(6)]
        [DataMember]
        public virtual string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    ChangeTracker.RecordOriginalValue("Value", _value);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Value"))
                            ChangeTracker.RecordOriginalValue("Value", _value);
                            
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }
        }
        protected string _value;
    [Required]
    	[ProtoMember(7)]
        [DataMember]
        public virtual byte Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    ChangeTracker.RecordOriginalValue("Type", _type);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Type"))
                            ChangeTracker.RecordOriginalValue("Type", _type);
                            
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        protected byte _type;

        #endregion

    
    		public virtual object Clone(){
    			return Clone(new Dictionary<object,object>(),true);
    		}
    
    public   virtual object Clone(Dictionary<object,object> clonedObjects,bool includePrimaryKey)
    {
        KeyValue cloned = new KeyValue();
                clonedObjects.Add(this, cloned);
        				if(includePrimaryKey)
    				     cloned._keyValuePK=this._keyValuePK;
             cloned._setKey=this._setKey;
             cloned._rowKey=this._rowKey;
             cloned._path=this._path;
             cloned._key=this._key;
             cloned._value=this._value;
             cloned._type=this._type;
            
    
        return cloned;
    }
    
    
    public virtual long ComputeHashCode()
    	{
    		var sb = new System.Text.StringBuilder();
    
    						sb.Append(this._keyValuePK.ToString());
    					// KeyValuePK
    			sb.Append("`");
    
    						sb.Append(this._setKey.ToString());
    					// SetKey
    			sb.Append("`");
    
    						sb.Append(this._rowKey.ToString());
    					// RowKey
    			sb.Append("`");
    
    						sb.Append(this._path?.ToString());
    						// Path
    			sb.Append("`");
    
    						sb.Append(this._key.ToString());
    					// Key
    			sb.Append("`");
    
    						sb.Append(this._value?.ToString());
    						// Value
    			sb.Append("`");
    
    						sb.Append(this._type.ToString());
    					// Type
    			sb.Append("`");
    
    		
    		return sb.ToString().ToLower().FarmhashCode64();
    }
            public virtual T CreateService<T>() 
    				 where T : IService
    		         {
                if (ObjectRegistry.ContainsObject("MMP-ServiceFactory"))
                {
                    var service_factory = (IServiceFactory)ObjectRegistry.GetObject("MMP-ServiceFactory",true);
                    return (T)service_factory.CreateByModel<KeyValue>();
                }
                else
                   {
    					return (T)StaticServiceFactory.CreateByModel<KeyValue>();			   
    				}
            }
    	protected const int LAST_PROTOBUF_MEMBER_INDEX=8;
    
    public virtual void ResetChanges(bool includeNavigations=false)
    {
        if(!includeNavigations){
            ChangeTracker.ResetChanges();
            return;
        }
    
        ResetChanges(new List<object>(), includeNavigations);
    }
    public virtual void ResetChanges(List<object> reseted_objects, bool includeNavigations=false)
    {
        if(reseted_objects.Contains(this)) return;
    
        ChangeTracker.ResetChanges();
         
        reseted_objects.Add(this);
    
       }
    
    public virtual bool Equals(IEntityBase obj)
        {
    		bool result= this.Equals(obj, new List<object>());
    
    	return result;
    }   
    
    public virtual bool Equals(object other,List<object> checked_objects)
    {
    	if(checked_objects.Contains(this)) return true;
    	
    	checked_objects.Add(this);
    
        KeyValue casted_other = other as KeyValue;
    
    	if(casted_other==null) {
    		checked_objects.Remove(this);
    		return false;
    	}
    
             if(!Typing.IsEquals(this.KeyValuePK,casted_other.KeyValuePK))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.SetKey,casted_other.SetKey))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.RowKey,casted_other.RowKey))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Path,casted_other.Path))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Key,casted_other.Key))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Value,casted_other.Value))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Type,casted_other.Type))
    {
    		checked_objects.Remove(this);
    		return false;
    	}    
    checked_objects.Remove(this);
    
    return true;
    }
    
        #region Metadata section
            [NotMapped]
            [ProtoIgnore]
            [Newtonsoft.Json.JsonIgnore]
            [Jil.JilDirective(Ignore =true)]
    	    [System.Xml.Serialization.XmlIgnore]
            public virtual IEnumerable<IField> PrimaryKeys { get{ return GetPrimaryKeys(); } }
    
        public virtual IEnumerable<IField> GetFields ()
        {
                var array1 = GetPrimitiveFields();
                var array2 = GetNavigationFields();
                if(array1.Any() && array2.Any()) return array1.Union(array2);
                else if(array1.Any()) return array1;
                else if(array2.Any()) return array2;
                return null;
        }
    
        public virtual IEnumerable<IField> GetPrimaryKeys()
    {
          yield return new BaseField{Name= "KeyValuePK", Kind = FieldKinds.Primitive,PropertyType =typeof(long),
    	IsIdentity=true
    	 };
    }
    
    public virtual Type GetFieldType(string propertyName)
    {
        var field = GetFields().Where(x => x.Name == propertyName).FirstOrDefault();
        return field != null ? field.PropertyType: null;
    }
    
    public virtual bool HasField(string propertyName)
    {
        var field = GetFields().Where(x => x.Name == propertyName).FirstOrDefault();
        return field != null;
    }
    
    
        private static IEnumerable<IField> GetPrimitiveFields()
        {
            yield return new BaseField{Name= "KeyValuePK", Kind = FieldKinds.PrimaryKey,PropertyType =typeof(long) ,Nullable=false,TargetType =null,
    	IsIdentity=true
        
         };
              yield return new BaseField{Name= "SetKey", Kind = FieldKinds.Primitive,PropertyType =typeof(string) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        ,Size=50
    	,Unicode=false
    	,FixedLength=false
    	,DefaultValue=""
        
         };
              yield return new BaseField{Name= "RowKey", Kind = FieldKinds.Primitive,PropertyType =typeof(System.Guid) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Path", Kind = FieldKinds.Primitive,PropertyType =typeof(string) ,Nullable=true,TargetType =null,
    	IsIdentity=null
        ,Size=1000
    	,Unicode=false
    	,FixedLength=false
    	,DefaultValue=""
        
         };
              yield return new BaseField{Name= "Key", Kind = FieldKinds.Primitive,PropertyType =typeof(string) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        ,Size=50
    	,Unicode=false
    	,FixedLength=false
    	,DefaultValue=""
        
         };
              yield return new BaseField{Name= "Value", Kind = FieldKinds.Primitive,PropertyType =typeof(string) ,Nullable=true,TargetType =null,
    	IsIdentity=null
        ,Size=0
    	,Unicode=true
    	,FixedLength=false
    	,DefaultValue=""
        
         };
              yield return new BaseField{Name= "Type", Kind = FieldKinds.Primitive,PropertyType =typeof(byte) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
          	
        }
    
    
    
    private static IEnumerable<IField> GetNavigationFields()
    {
                return Enumerable.Empty<IField>();
                 
    }
    
    public virtual Expression GetPrimaryKeyPrediacate()
    {
        var vKeyValuePK=(long)this.GetValue("KeyValuePK");
        Expression<Func<KeyValue,bool>> exp = p => p.KeyValuePK == vKeyValuePK ;
        return exp;        
    
    }
    
    public virtual object GetValue(string propertyName)
    {
        object retVal = null;
        switch(propertyName.ToLower())
        {
            case "keyvaluepk":
                retVal = this.KeyValuePK;
                break;
            case "setkey":
                retVal = this.SetKey;
                break;
            case "rowkey":
                retVal = this.RowKey;
                break;
            case "path":
                retVal = this.Path;
                break;
            case "key":
                retVal = this.Key;
                break;
            case "value":
                retVal = this.Value;
                break;
            case "type":
                retVal = this.Type;
                break;
        	
                        default:
    							retVal=null;
    			            break;
        }
        return retVal;
    }
    
    		private Dictionary<string,object> __tags;
    		public virtual void AddTag(string key, object value)
            {
                if(__tags==null)__tags=new Dictionary<string,object>();
    			if(!__tags.ContainsKey(key)) __tags.Add(key,value);
            }  
    
    		public virtual object GetTag(string key)
            {
                if(__tags==null)return null;
    			if(!__tags.ContainsKey(key))return null;
    			return __tags[key];
            }
    
            public virtual object RemoveTag(string key)
            {
                if (__tags == null) return null;
                if (__tags.ContainsKey(key)){
    			var result= __tags[key];
                    __tags.Remove(key);
    				return result;
    			}
    			return  null;
            }
            public virtual bool HasKey()
            {
    			
    			return _keyValuePK!=default(long);
            }
      
     public virtual void StartTracking()
            {
    		if(ChangeTracker.ChangeTrackingEnabled) return;
    		            ChangeTracker.ChangeTrackingEnabled = true;
                        }
    
            public virtual void StopTracking()
            {
    		         ChangeTracker.ChangeTrackingEnabled = false;
                       }
    
    public virtual bool SetValue(string propertyName, object value)
    {
        switch(propertyName.ToLower())
        {
            case "keyvaluepk":
                this.KeyValuePK =(long)value;
            break;
            case "setkey":
                this.SetKey =(string)value;
            break;
            case "rowkey":
                this.RowKey =(System.Guid)value;
            break;
            case "path":
                this.Path =(string)value;
            break;
            case "key":
                this.Key =(string)value;
            break;
            case "value":
                this.Value =(string)value;
            break;
            case "type":
                this.Type =(byte)value;
            break;
    	
                    default:
                                   return Exir.Framework.Common.Fasterflect.PropertyExtensions.TrySetPropertyValue(this, propertyName, value);
    			            break;
       }   
       return true;
    }
            
        #endregion

        #region ChangeTracking
    
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
     	[NonSerialized]
       private ObjectChangeTracker _changeTracker;
    	  [NotMapped]
            [ProtoIgnore]
            [Newtonsoft.Json.JsonIgnore]
            [Jil.JilDirective(Ignore =true)]
        [System.Xml.Serialization.XmlIgnore]
        public virtual ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                              _propertyChanged += new PropertyChangedEventHandler(delegate(object sender, PropertyChangedEventArgs e)
                        {
                        if(ChangeTracker.ChangeTrackingEnabled)
                            ChangeTracker.RecordPropertyChange(e.PropertyName);
                        });
          }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
           
    
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    	[NotMapped]
        protected bool IsDeserializing { get; private set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        protected virtual void ClearNavigationProperties()
        {
        }

        #endregion

    }
}