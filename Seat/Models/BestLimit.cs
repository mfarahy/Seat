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

namespace Seat.Models
{
            public interface IBestLimit{
    
     long BestLimitPK{
      get;
      set;
      }
    
     long InsCode{
      get;
      set;
      }
    
     int Row{
      get;
      set;
      }
    
     int Buy_Count{
      get;
      set;
      }
    
     long Buy_Volume{
      get;
      set;
      }
    
     decimal Buy_Price{
      get;
      set;
      }
    
     decimal Sell_Price{
      get;
      set;
      }
    
     long Sell_Volume{
      get;
      set;
      }
    
     int Sell_Count{
      get;
      set;
      }
    
     System.DateTime DateTime{
      get;
      set;
      }
    
    Instrument Instrument{get;set;}
    }// interface
    
    [Table("BestLimits")]
    [Serializer(typeof(ProtobufSerializer))]
    [ProtoContract]
    [Exir.Framework.Common.Caching.CacheableKey( new string[]{nameof(BestLimitPK)} )]
    [Entity]
    [Serializable]
    [DataContract(IsReference = true)]
    [KnownType(typeof(Instrument))]
    public partial class BestLimit:IEntityBase, IBestLimit,IObjectWithChangeTracker, INotifyPropertyChanged, ICloneable
    {
        #region Primitive Properties
    [Key]
    [Required]
    	[ProtoMember(1)]
        [DataMember]
        public virtual long BestLimitPK
        {
            get { return _bestLimitPK; }
            set
            {
                if (_bestLimitPK != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && !Typing.IsEquals(_bestLimitPK,Typing.GetDefaultValue(typeof(long))))
                    {
                        throw new InvalidOperationException("The property 'BestLimitPK' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _bestLimitPK = value;
                    OnPropertyChanged("BestLimitPK");
                }
            }
        }
        protected long _bestLimitPK;
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
                            
                    if (!IsDeserializing)
                    {
                        if (Instrument != null && Instrument.InsCode != value)
                        {
                            Instrument = null;
                        }
                    }
                    _insCode = value;
                    OnPropertyChanged("InsCode");
                }
            }
        }
        protected long _insCode;
    [Required]
    	[ProtoMember(3)]
        [DataMember]
        public virtual int Row
        {
            get { return _row; }
            set
            {
                if (_row != value)
                {
                    ChangeTracker.RecordOriginalValue("Row", _row);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Row"))
                            ChangeTracker.RecordOriginalValue("Row", _row);
                            
                    _row = value;
                    OnPropertyChanged("Row");
                }
            }
        }
        protected int _row;
    [Required]
    	[ProtoMember(4)]
        [DataMember]
        public virtual int Buy_Count
        {
            get { return _buy_Count; }
            set
            {
                if (_buy_Count != value)
                {
                    ChangeTracker.RecordOriginalValue("Buy_Count", _buy_Count);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Buy_Count"))
                            ChangeTracker.RecordOriginalValue("Buy_Count", _buy_Count);
                            
                    _buy_Count = value;
                    OnPropertyChanged("Buy_Count");
                }
            }
        }
        protected int _buy_Count;
    [Required]
    	[ProtoMember(5)]
        [DataMember]
        public virtual long Buy_Volume
        {
            get { return _buy_Volume; }
            set
            {
                if (_buy_Volume != value)
                {
                    ChangeTracker.RecordOriginalValue("Buy_Volume", _buy_Volume);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Buy_Volume"))
                            ChangeTracker.RecordOriginalValue("Buy_Volume", _buy_Volume);
                            
                    _buy_Volume = value;
                    OnPropertyChanged("Buy_Volume");
                }
            }
        }
        protected long _buy_Volume;
    [Required]
    	[ProtoMember(6)]
        [DataMember]
        public virtual decimal Buy_Price
        {
            get { return _buy_Price; }
            set
            {
                if (_buy_Price != value)
                {
                    ChangeTracker.RecordOriginalValue("Buy_Price", _buy_Price);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Buy_Price"))
                            ChangeTracker.RecordOriginalValue("Buy_Price", _buy_Price);
                            
                    _buy_Price = value;
                    OnPropertyChanged("Buy_Price");
                }
            }
        }
        protected decimal _buy_Price;
    [Required]
    	[ProtoMember(7)]
        [DataMember]
        public virtual decimal Sell_Price
        {
            get { return _sell_Price; }
            set
            {
                if (_sell_Price != value)
                {
                    ChangeTracker.RecordOriginalValue("Sell_Price", _sell_Price);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Sell_Price"))
                            ChangeTracker.RecordOriginalValue("Sell_Price", _sell_Price);
                            
                    _sell_Price = value;
                    OnPropertyChanged("Sell_Price");
                }
            }
        }
        protected decimal _sell_Price;
    [Required]
    	[ProtoMember(8)]
        [DataMember]
        public virtual long Sell_Volume
        {
            get { return _sell_Volume; }
            set
            {
                if (_sell_Volume != value)
                {
                    ChangeTracker.RecordOriginalValue("Sell_Volume", _sell_Volume);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Sell_Volume"))
                            ChangeTracker.RecordOriginalValue("Sell_Volume", _sell_Volume);
                            
                    _sell_Volume = value;
                    OnPropertyChanged("Sell_Volume");
                }
            }
        }
        protected long _sell_Volume;
    [Required]
    	[ProtoMember(9)]
        [DataMember]
        public virtual int Sell_Count
        {
            get { return _sell_Count; }
            set
            {
                if (_sell_Count != value)
                {
                    ChangeTracker.RecordOriginalValue("Sell_Count", _sell_Count);
                        if (!ChangeTracker.OriginalValues.ContainsKey("Sell_Count"))
                            ChangeTracker.RecordOriginalValue("Sell_Count", _sell_Count);
                            
                    _sell_Count = value;
                    OnPropertyChanged("Sell_Count");
                }
            }
        }
        protected int _sell_Count;
    [Required]
    	[ProtoMember(10)]
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

        #endregion

        #region Navigation Properties
    
     	[ProtoMember(11,AsReference =true)]
    [ForeignKey("InsCode")]
       [DataMember]
        public  Instrument Instrument
        {
            get { return _instrument; }
            set
            {
                if (!ReferenceEquals(_instrument, value))
                {
                    var previousValue = _instrument;
                    _instrument = value;
                    FixupInstrument(previousValue);
                    OnNavigationPropertyChanged("Instrument");
                }
            }
        }
        protected Instrument _instrument;

        #endregion

    
    		public virtual object Clone(){
    			return Clone(new Dictionary<object,object>(),true);
    		}
    
    public   virtual object Clone(Dictionary<object,object> clonedObjects,bool includePrimaryKey)
    {
        BestLimit cloned = new BestLimit();
                clonedObjects.Add(this, cloned);
        				if(includePrimaryKey)
    				     cloned._bestLimitPK=this._bestLimitPK;
             cloned._insCode=this._insCode;
             cloned._row=this._row;
             cloned._buy_Count=this._buy_Count;
             cloned._buy_Volume=this._buy_Volume;
             cloned._buy_Price=this._buy_Price;
             cloned._sell_Price=this._sell_Price;
             cloned._sell_Volume=this._sell_Volume;
             cloned._sell_Count=this._sell_Count;
             cloned._dateTime=this._dateTime;
        if(Instrument!=null) {
    if (!clonedObjects.ContainsKey(Instrument))
    cloned.Instrument=(Instrument)Instrument.Clone(clonedObjects,includePrimaryKey);
    else
                    cloned.Instrument = (Instrument)clonedObjects[Instrument];
                }
        
    
        return cloned;
    }
    
    
    public virtual long ComputeHashCode()
    	{
    		var sb = new System.Text.StringBuilder();
    
    						sb.Append(this._bestLimitPK.ToString());
    					// BestLimitPK
    			sb.Append("`");
    
    						sb.Append(this._insCode.ToString());
    					// InsCode
    			sb.Append("`");
    
    						sb.Append(this._row.ToString());
    					// Row
    			sb.Append("`");
    
    						sb.Append(this._buy_Count.ToString());
    					// Buy_Count
    			sb.Append("`");
    
    						sb.Append(this._buy_Volume.ToString());
    					// Buy_Volume
    			sb.Append("`");
    
    						sb.Append(this._buy_Price.ToString());
    					// Buy_Price
    			sb.Append("`");
    
    						sb.Append(this._sell_Price.ToString());
    					// Sell_Price
    			sb.Append("`");
    
    						sb.Append(this._sell_Volume.ToString());
    					// Sell_Volume
    			sb.Append("`");
    
    						sb.Append(this._sell_Count.ToString());
    					// Sell_Count
    			sb.Append("`");
    
    						sb.Append(this._dateTime.ToString());
    					// DateTime
    			sb.Append("`");
    
    		
    		return sb.ToString().ToLower().FarmhashCode64();
    }
            public virtual T CreateService<T>() 
    				 where T : IService
    		         {
                if (ObjectRegistry.ContainsObject("SeatServiceFactory"))
                {
                    var service_factory = (IServiceFactory)ObjectRegistry.GetObject("SeatServiceFactory",true);
                    return (T)service_factory.CreateByModel<BestLimit>();
                }
                else
                   {
    					return (T)StaticServiceFactory.CreateByModel<BestLimit>();			   
    				}
            }
    	protected const int LAST_PROTOBUF_MEMBER_INDEX=12;
    
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
    
           if(_instrument!=null && !reseted_objects.Contains(_instrument)) _instrument.ResetChanges(reseted_objects);
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
    
        BestLimit casted_other = other as BestLimit;
    
    	if(casted_other==null) {
    		checked_objects.Remove(this);
    		return false;
    	}
    
             if(!Typing.IsEquals(this.BestLimitPK,casted_other.BestLimitPK))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.InsCode,casted_other.InsCode))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Row,casted_other.Row))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Buy_Count,casted_other.Buy_Count))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Buy_Volume,casted_other.Buy_Volume))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Buy_Price,casted_other.Buy_Price))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Sell_Price,casted_other.Sell_Price))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Sell_Volume,casted_other.Sell_Volume))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.Sell_Count,casted_other.Sell_Count))
    {
    		checked_objects.Remove(this);
    		return false;
    	}         if(!Typing.IsEquals(this.DateTime,casted_other.DateTime))
    {
    		checked_objects.Remove(this);
    		return false;
    	}    if(this.Instrument!= null) {
    if(!this.Instrument.Equals(casted_other.Instrument,checked_objects))
    {
    		checked_objects.Remove(this);
    		return false;
    	}}
    else
    if(casted_other.Instrument!= null) 
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
          yield return new BaseField{Name= "BestLimitPK", Kind = FieldKinds.Primitive,PropertyType =typeof(long),
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
            yield return new BaseField{Name= "BestLimitPK", Kind = FieldKinds.PrimaryKey,PropertyType =typeof(long) ,Nullable=false,TargetType =null,
    	IsIdentity=true
        
         };
              yield return new BaseField{Name= "InsCode", Kind = FieldKinds.Primitive,PropertyType =typeof(long) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Row", Kind = FieldKinds.Primitive,PropertyType =typeof(int) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Buy_Count", Kind = FieldKinds.Primitive,PropertyType =typeof(int) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Buy_Volume", Kind = FieldKinds.Primitive,PropertyType =typeof(long) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Buy_Price", Kind = FieldKinds.Primitive,PropertyType =typeof(decimal) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Sell_Price", Kind = FieldKinds.Primitive,PropertyType =typeof(decimal) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Sell_Volume", Kind = FieldKinds.Primitive,PropertyType =typeof(long) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "Sell_Count", Kind = FieldKinds.Primitive,PropertyType =typeof(int) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
              yield return new BaseField{Name= "DateTime", Kind = FieldKinds.Primitive,PropertyType =typeof(System.DateTime) ,Nullable=false,TargetType =null,
    	IsIdentity=null
        
         };
          	
        }
    
    
    
    private static IEnumerable<IField> GetNavigationFields()
    {
    
    
                
            yield return new BaseField{Name= "Instrument", Kind = FieldKinds.Navigational,PropertyType =typeof(Instrument),TargetType =typeof(Instrument)
            ,DependentProperty="InsCode",
    IsCollection=false
    ,FromEndMultiplicity = Multiplicities.Many
    ,ToEndMultiplicity = Multiplicities.One
            };
    }
    
    public virtual Expression GetPrimaryKeyPrediacate()
    {
        var vBestLimitPK=(long)this.GetValue("BestLimitPK");
        Expression<Func<BestLimit,bool>> exp = p => p.BestLimitPK == vBestLimitPK ;
        return exp;        
    
    }
    
    public virtual object GetValue(string propertyName)
    {
        object retVal = null;
        switch(propertyName.ToLower())
        {
            case "bestlimitpk":
                retVal = this.BestLimitPK;
                break;
            case "inscode":
                retVal = this.InsCode;
                break;
            case "row":
                retVal = this.Row;
                break;
            case "buy_count":
                retVal = this.Buy_Count;
                break;
            case "buy_volume":
                retVal = this.Buy_Volume;
                break;
            case "buy_price":
                retVal = this.Buy_Price;
                break;
            case "sell_price":
                retVal = this.Sell_Price;
                break;
            case "sell_volume":
                retVal = this.Sell_Volume;
                break;
            case "sell_count":
                retVal = this.Sell_Count;
                break;
            case "datetime":
                retVal = this.DateTime;
                break;
        	
            case "instrument":
                   retVal= this.Instrument;
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
    			
    			return _bestLimitPK!=default(long);
            }
      
     public virtual void StartTracking()
            {
    		if(ChangeTracker.ChangeTrackingEnabled) return;
    		            ChangeTracker.ChangeTrackingEnabled = true;
                				if(Instrument!=null) Instrument.StartTracking();
                            }
    
            public virtual void StopTracking()
            {
    		         ChangeTracker.ChangeTrackingEnabled = false;
                       }
    
    public virtual bool SetValue(string propertyName, object value)
    {
        switch(propertyName.ToLower())
        {
            case "bestlimitpk":
                this.BestLimitPK =(long)value;
            break;
            case "inscode":
                this.InsCode =(long)value;
            break;
            case "row":
                this.Row =(int)value;
            break;
            case "buy_count":
                this.Buy_Count =(int)value;
            break;
            case "buy_volume":
                this.Buy_Volume =(long)value;
            break;
            case "buy_price":
                this.Buy_Price =(decimal)value;
            break;
            case "sell_price":
                this.Sell_Price =(decimal)value;
            break;
            case "sell_volume":
                this.Sell_Volume =(long)value;
            break;
            case "sell_count":
                this.Sell_Count =(int)value;
            break;
            case "datetime":
                this.DateTime =(System.DateTime)value;
            break;
    	
            case "instrument":
             this.Instrument =( Instrument) value;
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
    
        // This entity type is the dependent end in at least one association that performs cascade deletes.
        // This event handler will process notifications that occur when the principal end is deleted.
        internal void HandleCascadeDelete(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
            }
        }
    
        protected virtual void ClearNavigationProperties()
        {
            Instrument = null;
        }

        #endregion

        #region Association Fixup
    
        private void FixupInstrument(Instrument previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.BestLimits.Contains(this))
            {
                previousValue.BestLimits.Remove(this);
            }
    
            if (Instrument != null)
            {
                if (!Instrument.BestLimits.Contains(this))
                {
                    Instrument.BestLimits.Add(this);
                }
    
                InsCode = Instrument.InsCode;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Instrument")
                    && (ChangeTracker.OriginalValues["Instrument"] == Instrument))
                {
                    ChangeTracker.OriginalValues.Remove("Instrument");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Instrument", previousValue);
                }
                if (Instrument != null && !Instrument.ChangeTracker.ChangeTrackingEnabled)
                {
                    Instrument.StartTracking();
                }
            }
        }

        #endregion

    }
}
