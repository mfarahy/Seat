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
            public interface ITrade{
    
     long TradePK{
      get;
      set;
      }
    
     long InsCode{
      get;
      set;
      }
    
     System.DateTime DateTime{
      get;
      set;
      }
    
     int Number{
      get;
      set;
      }
    
     int Quantity{
      get;
      set;
      }
    
     decimal Price{
      get;
      set;
      }
    
     int Unknown1{
      get;
      set;
      }
    
    }// interface
    
    [Table("Trades")]
    
[Serializer(typeof(ProtobufSerializer))]
    
[ProtoContract]
    
[Exir.Framework.Common.Caching.CacheableKey( new string[]{nameof(TradePK)} )]
    
[Entity]
    
[Serializable]
    
[DataContract(IsReference = true)]
    
public partial class Trade:IEntityBase, ITrade,IObjectWithChangeTracker, INotifyPropertyChanged, ICloneable
    {
        #region Primitive Properties
    [Key]
    
[Required]
    
	[ProtoMember(1)]
    
    [DataMember]
    
    public virtual long TradePK
        {
            get { return _tradePK; }
            set
            {
                if (_tradePK != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && !Typing.IsEquals(_tradePK,Typing.GetDefaultValue(typeof(long))))
                    {
                        throw new InvalidOperationException("The property 'TradePK' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _tradePK = value;
                    OnPropertyChanged("TradePK");
                }
            }
        }
        protected long _tradePK;
    [Required]
    
	[ProtoMember(2)]
    
    [DataMember]
    
    public virtual long InsCode
        {
            get { return _insCode; }
            set
            {
                if (_insCode != value)
                {
                    ChangeTracker.RecordOriginalValue("InsCode", _insCode);
                        if (!ChangeTracker.OriginalValues.ContainsKey("InsCode"))
                            ChangeTracker.RecordOriginalValue("InsCode", _insCode);
                            
                    _insCode = value;
                    OnPropertyChanged("InsCode");
                }
            }
        }
        protected long _insCode;
    [Required]
    
	[ProtoMember(3)]
    
    [DataMember]
    
    public virtual System.DateTime DateTime
        {
            get { return _dateTime; }
            set
            {
                if (_dateTime != value)
                {
                    ChangeTracker.RecordOriginalValue("DateTime", _dateTime);
                        if (!ChangeTracker.OriginalValues.ContainsKey("DateTime"))
                            ChangeTracker.RecordOriginalValue("DateTime", _dateTime);
                            
                    _dateTime = value;
                    OnPropertyChanged("DateTime");
                }
            }
        }
        protected System.DateTime _dateTime;
    [Required]
    
	[ProtoMember(4)]
    
    [DataMember]
    
    public virtual int Number
        {
            get { return _number; }
            set
            {
                if (_number != value)
                {
                    ChangeTracker.RecordOriginalValue("Number", _number);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Number"))
                            ChangeTracker.RecordOriginalValue("Number", _number);
                            
                    _number = value;
                    OnPropertyChanged("Number");
                }
            }
        }
        protected int _number;
    [Required]
    
	[ProtoMember(5)]
    
    [DataMember]
    
    public virtual int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    ChangeTracker.RecordOriginalValue("Quantity", _quantity);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Quantity"))
                            ChangeTracker.RecordOriginalValue("Quantity", _quantity);
                            
                    _quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }
        protected int _quantity;
    [Required]
    
	[ProtoMember(6)]
    
    [DataMember]
    
    public virtual decimal Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    ChangeTracker.RecordOriginalValue("Price", _price);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Price"))
                            ChangeTracker.RecordOriginalValue("Price", _price);
                            
                    _price = value;
                    OnPropertyChanged("Price");
                }
            }
        }
        protected decimal _price;
    [Required]
    
	[ProtoMember(7)]
    
    [DataMember]
    
    public virtual int Unknown1
        {
            get { return _unknown1; }
            set
            {
                if (_unknown1 != value)
                {
                    ChangeTracker.RecordOriginalValue("Unknown1", _unknown1);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Unknown1"))
                            ChangeTracker.RecordOriginalValue("Unknown1", _unknown1);
                            
                    _unknown1 = value;
                    OnPropertyChanged("Unknown1");
                }
            }
        }
        protected int _unknown1;

        #endregion

    
    		public virtual object Clone(){
    			return Clone(new Dictionary<object,object>(),true);
    		}
    
    public   virtual object Clone(Dictionary<object,object> clonedObjects,bool includePrimaryKey)
    {
        Trade cloned = new Trade();
                clonedObjects.Add(this, cloned);
        				if(includePrimaryKey)
    				     cloned._tradePK=this._tradePK;
             cloned._insCode=this._insCode;
             cloned._dateTime=this._dateTime;
             cloned._number=this._number;
             cloned._quantity=this._quantity;
             cloned._price=this._price;
             cloned._unknown1=this._unknown1;
            
    
        return cloned;
    }
    
    
    public virtual long ComputeHashCode()
    	{
    		var sb = new System.Text.StringBuilder();
    
    						sb.Append(this._tradePK.ToString());
    					// TradePK
    			sb.Append("`");
    
    						sb.Append(this._insCode.ToString());
    					// InsCode
    			sb.Append("`");
    
    						sb.Append(this._dateTime.ToString());
    					// DateTime
    			sb.Append("`");
    
    						sb.Append(this._number.ToString());
    					// Number
    			sb.Append("`");
    
    						sb.Append(this._quantity.ToString());
    					// Quantity
    			sb.Append("`");
    
    						sb.Append(this._price.ToString());
    					// Price
    			sb.Append("`");
    
    						sb.Append(this._unknown1.ToString());
    					// Unknown1
    			sb.Append("`");
    
    		
    		return sb.ToString().ToLower().FarmhashCode64();
    }
            public virtual T CreateService<T>() 
    				 where T : IService
    		         {
                if (ObjectRegistry.ContainsObject("MMP-ServiceFactory"))
                {
                    var service_factory = (IServiceFactory)ObjectRegistry.GetObject("MMP-ServiceFactory",true);
                    return (T)service_factory.CreateByModel<Trade>();
                }
                else
                   {
    					return (T)StaticServiceFactory.CreateByModel<Trade>();			   
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
    
        Trade casted_other = other as Trade;
    
    	if(casted_other==null) {
    		checked_objects.Remove(this);
    		return false;
    	}
    
             if(!Typing.IsEquals(this.TradePK,casted_other.TradePK))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.InsCode,casted_other.InsCode))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.DateTime,casted_other.DateTime))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Number,casted_other.Number))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Quantity,casted_other.Quantity))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Price,casted_other.Price))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Unknown1,casted_other.Unknown1))
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
          yield return new BaseField{Name= "TradePK", Kind = FieldKinds.Primitive,PropertyType =typeof(long),
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
            yield return new BaseField{Name= "TradePK", Kind = FieldKinds.PrimaryKey,PropertyType =typeof(long) ,Nullable=false,TargetType =null,
    	IsIdentity=true
        
         };
              yield return new BaseField{Name= "InsCode", Kind = FieldKinds.Primitive,PropertyType =typeof(long) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "DateTime", Kind = FieldKinds.Primitive,PropertyType =typeof(System.DateTime) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Number", Kind = FieldKinds.Primitive,PropertyType =typeof(int) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Quantity", Kind = FieldKinds.Primitive,PropertyType =typeof(int) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Price", Kind = FieldKinds.Primitive,PropertyType =typeof(decimal) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Unknown1", Kind = FieldKinds.Primitive,PropertyType =typeof(int) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
          	
        }
    
    
    
    private static IEnumerable<IField> GetNavigationFields()
    {
                return Enumerable.Empty<IField>();
                 
    }
    
    public virtual Expression GetPrimaryKeyPrediacate()
    {
        var vTradePK=(long)this.GetValue("TradePK");
        Expression<Func<Trade,bool>> exp = p => p.TradePK == vTradePK ;
        return exp;        
    
    }
    
    public virtual object GetValue(string propertyName)
    {
        object retVal = null;
        switch(propertyName.ToLower())
        {
            case "tradepk":
                retVal = this.TradePK;
                break;
            case "inscode":
                retVal = this.InsCode;
                break;
            case "datetime":
                retVal = this.DateTime;
                break;
            case "number":
                retVal = this.Number;
                break;
            case "quantity":
                retVal = this.Quantity;
                break;
            case "price":
                retVal = this.Price;
                break;
            case "unknown1":
                retVal = this.Unknown1;
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
    			
    			return _tradePK!=default(long);
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
            case "tradepk":
                this.TradePK =(long)value;
            break;
            case "inscode":
                this.InsCode =(long)value;
            break;
            case "datetime":
                this.DateTime =(System.DateTime)value;
            break;
            case "number":
                this.Number =(int)value;
            break;
            case "quantity":
                this.Quantity =(int)value;
            break;
            case "price":
                this.Price =(decimal)value;
            break;
            case "unknown1":
                this.Unknown1 =(int)value;
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