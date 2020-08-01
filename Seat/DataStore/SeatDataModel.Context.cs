﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using Exir.Framework.Common;
using Exir.Framework.DataAccess;
using Exir.Framework.DataAccess.EntityFramework;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using Seat.Models;

namespace Seat.DataStore
{
    public partial class SeatEntitiesDbContext : EntityDbContext
    {
    	    	public SeatEntitiesDbContext(string databaseName, string connectionString)
        			:base(new SeatEntitiesObjectContext(databaseName,connectionString),true)
        	{
        	Configuration.ValidateOnSaveEnabled = false;
        	}
    }
    
    public partial class SeatEntitiesObjectContext : ObjectContextBase
    {
        public const string ConnectionString = "name=SeatEntities";
        public const string ContainerName = "SeatEntities";
    
        #region Constructors
    
              public SeatEntitiesObjectContext(string databaseName, string connectionStringName)
                    : base(databaseName, CreateEntityConnection(connectionStringName, typeof(SeatEntitiesObjectContext).Assembly), ContainerName)
            {
                Initialize();
            }
    
    
        private void Initialize()
        {
            // Creating proxies requires the use of the ProxyDataContractResolver and
            // may allow lazy loading which can expand the loaded graph during serialization.
            ContextOptions.ProxyCreationEnabled = false;
            ObjectMaterialized += new ObjectMaterializedEventHandler(HandleObjectMaterialized);
        }
    
        private void HandleObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity as IObjectWithChangeTracker;
            if (entity != null)
            {
                bool changeTrackingEnabled = entity.ChangeTracker.ChangeTrackingEnabled;
                try
                {
                    entity.MarkAsUnchanged();
                }
                finally
                {
                    entity.ChangeTracker.ChangeTrackingEnabled = changeTrackingEnabled;
                }
                this.StoreReferenceKeyValues(entity);
            }
        }
    
        #endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<ClientType> ClientTypes
        {
            get { return _clientTypes  ?? (_clientTypes = CreateObjectSet<ClientType>("SeatEntities.ClientTypes")); }
        }
        private ObjectSet<ClientType> _clientTypes;
    
        public ObjectSet<Message> Messages
        {
            get { return _messages  ?? (_messages = CreateObjectSet<Message>("SeatEntities.Messages")); }
        }
        private ObjectSet<Message> _messages;
    
        public ObjectSet<CodalMessage> CodalMessages
        {
            get { return _codalMessages  ?? (_codalMessages = CreateObjectSet<CodalMessage>("SeatEntities.CodalMessages")); }
        }
        private ObjectSet<CodalMessage> _codalMessages;
    
        public ObjectSet<Instrument> Instruments
        {
            get { return _instruments  ?? (_instruments = CreateObjectSet<Instrument>("SeatEntities.Instruments")); }
        }
        private ObjectSet<Instrument> _instruments;
    
        public ObjectSet<History> Histories
        {
            get { return _histories  ?? (_histories = CreateObjectSet<History>("SeatEntities.Histories")); }
        }
        private ObjectSet<History> _histories;
    
        public ObjectSet<BestLimit> BestLimits
        {
            get { return _bestLimits  ?? (_bestLimits = CreateObjectSet<BestLimit>("SeatEntities.BestLimits")); }
        }
        private ObjectSet<BestLimit> _bestLimits;
    
        public ObjectSet<Trade> Trades
        {
            get { return _trades  ?? (_trades = CreateObjectSet<Trade>("SeatEntities.Trades")); }
        }
        private ObjectSet<Trade> _trades;
    
        public ObjectSet<ShareHolderChange> ShareHolderChanges
        {
            get { return _shareHolderChanges  ?? (_shareHolderChanges = CreateObjectSet<ShareHolderChange>("SeatEntities.ShareHolderChanges")); }
        }
        private ObjectSet<ShareHolderChange> _shareHolderChanges;

        #endregion

    }
}
