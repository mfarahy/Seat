
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/10/2017 14:48:10
-- Generated from EDMX file: C:\Users\rasooli\Source\Workspaces\Framework\Source\Workbox\45\Exir.Workbox\Dal\WorkboxModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [KMTDevelop2];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Attachments_WorkflowInstances_WorkflowInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbAttachments] DROP CONSTRAINT [FK_Attachments_WorkflowInstances_WorkflowInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_Comments_WorkflowInstances_WorkflowInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbComments] DROP CONSTRAINT [FK_Comments_WorkflowInstances_WorkflowInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_Folders_Folders_Parent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbFolders] DROP CONSTRAINT [FK_Folders_Folders_Parent];
GO
IF OBJECT_ID(N'[dbo].[FK_Labels_Labels_Parent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbLabels] DROP CONSTRAINT [FK_Labels_Labels_Parent];
GO
IF OBJECT_ID(N'[dbo].[FK_Labels_Workboxs_Workbox]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbLabels] DROP CONSTRAINT [FK_Labels_Workboxs_Workbox];
GO
IF OBJECT_ID(N'[dbo].[FK_Entries_Entries_Prev]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntries] DROP CONSTRAINT [FK_Entries_Entries_Prev];
GO
IF OBJECT_ID(N'[dbo].[FK_Entries_Workboxs_Workbox]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntries] DROP CONSTRAINT [FK_Entries_Workboxs_Workbox];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkflowAssignments_WorkflowSystems_System]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbWorkflowAssignments] DROP CONSTRAINT [FK_WorkflowAssignments_WorkflowSystems_System];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkboxMacroPersonWorkbox]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbWorkboxMacros] DROP CONSTRAINT [FK_WorkboxMacroPersonWorkbox];
GO
IF OBJECT_ID(N'[dbo].[FK_WbAllowedReferentWorkflowSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbAllowedReferents] DROP CONSTRAINT [FK_WbAllowedReferentWorkflowSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_WbDisallowedReferentWorkflowSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbDisallowedReferents] DROP CONSTRAINT [FK_WbDisallowedReferentWorkflowSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkboxMacroMacro]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbWorkboxMacros] DROP CONSTRAINT [FK_WorkboxMacroMacro];
GO
IF OBJECT_ID(N'[dbo].[FK_WbReferItemTemplates_WbReferTypes_ReferType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbReferItemTemplates] DROP CONSTRAINT [FK_WbReferItemTemplates_WbReferTypes_ReferType];
GO
IF OBJECT_ID(N'[dbo].[FK_ReferTypeWorkflowSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbReferTypes] DROP CONSTRAINT [FK_ReferTypeWorkflowSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_ReferTemplateWorkflowSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbReferTemplates] DROP CONSTRAINT [FK_ReferTemplateWorkflowSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_JobAssignmentInstance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbJobs] DROP CONSTRAINT [FK_JobAssignmentInstance];
GO
IF OBJECT_ID(N'[dbo].[FK_JobUrgency]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbJobs] DROP CONSTRAINT [FK_JobUrgency];
GO
IF OBJECT_ID(N'[dbo].[FK_JobWorkflowSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbJobs] DROP CONSTRAINT [FK_JobWorkflowSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkflowAssignmentJob]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbJobs] DROP CONSTRAINT [FK_WorkflowAssignmentJob];
GO
IF OBJECT_ID(N'[dbo].[FK_EntryJob]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntries] DROP CONSTRAINT [FK_EntryJob];
GO
IF OBJECT_ID(N'[dbo].[FK_JobPreview]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbJobs] DROP CONSTRAINT [FK_JobPreview];
GO
IF OBJECT_ID(N'[dbo].[FK_EntryEntryExtended]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntries] DROP CONSTRAINT [FK_EntryEntryExtended];
GO
IF OBJECT_ID(N'[dbo].[FK_EntryAssignmentInstance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntries] DROP CONSTRAINT [FK_EntryAssignmentInstance];
GO
IF OBJECT_ID(N'[dbo].[FK_AssignmentInstanceWorkflowAssignment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbAssignmentInstances] DROP CONSTRAINT [FK_AssignmentInstanceWorkflowAssignment];
GO
IF OBJECT_ID(N'[dbo].[FK_AssignmentInstanceWorkflowSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbAssignmentInstances] DROP CONSTRAINT [FK_AssignmentInstanceWorkflowSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_CommentEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbComments] DROP CONSTRAINT [FK_CommentEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_EntryWorkflowSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntries] DROP CONSTRAINT [FK_EntryWorkflowSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkflowAssignmentDefinitionFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbWorkflowAssignments] DROP CONSTRAINT [FK_WorkflowAssignmentDefinitionFile];
GO
IF OBJECT_ID(N'[dbo].[FK_AssignmentInstanceDefinitionFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbAssignmentInstances] DROP CONSTRAINT [FK_AssignmentInstanceDefinitionFile];
GO
IF OBJECT_ID(N'[dbo].[FK_EntryWorkflowAssignment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntries] DROP CONSTRAINT [FK_EntryWorkflowAssignment];
GO
IF OBJECT_ID(N'[dbo].[FK_ReferItemTemplateReferTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbReferItemTemplates] DROP CONSTRAINT [FK_ReferItemTemplateReferTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_EntryExtendedAssignmentInstance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbEntryExtendeds] DROP CONSTRAINT [FK_EntryExtendedAssignmentInstance];
GO
IF OBJECT_ID(N'[dbo].[FK_ReferTypeLabel]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbReferTypes] DROP CONSTRAINT [FK_ReferTypeLabel];
GO
IF OBJECT_ID(N'[dbo].[FK_BranchCommandJob]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WbBranchCommands] DROP CONSTRAINT [FK_BranchCommandJob];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[WbAttachments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbAttachments];
GO
IF OBJECT_ID(N'[dbo].[WbComments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbComments];
GO
IF OBJECT_ID(N'[dbo].[WbCommentTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbCommentTemplates];
GO
IF OBJECT_ID(N'[dbo].[WbFolders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbFolders];
GO
IF OBJECT_ID(N'[dbo].[WbLabels]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbLabels];
GO
IF OBJECT_ID(N'[dbo].[WbMessages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbMessages];
GO
IF OBJECT_ID(N'[dbo].[WbUrgencies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbUrgencies];
GO
IF OBJECT_ID(N'[dbo].[WbJobs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbJobs];
GO
IF OBJECT_ID(N'[dbo].[WbEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbEntries];
GO
IF OBJECT_ID(N'[dbo].[WbPersonWorkboxes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbPersonWorkboxes];
GO
IF OBJECT_ID(N'[dbo].[WbWorkflowAssignments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbWorkflowAssignments];
GO
IF OBJECT_ID(N'[dbo].[WbAssignmentInstances]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbAssignmentInstances];
GO
IF OBJECT_ID(N'[dbo].[WbWorkflowSystems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbWorkflowSystems];
GO
IF OBJECT_ID(N'[dbo].[WbSubstitutionExceptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbSubstitutionExceptions];
GO
IF OBJECT_ID(N'[dbo].[WbSubstitutions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbSubstitutions];
GO
IF OBJECT_ID(N'[dbo].[WbMacros]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbMacros];
GO
IF OBJECT_ID(N'[dbo].[WbPreviews]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbPreviews];
GO
IF OBJECT_ID(N'[dbo].[WbScheduledMessages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbScheduledMessages];
GO
IF OBJECT_ID(N'[dbo].[WbWorkboxMacros]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbWorkboxMacros];
GO
IF OBJECT_ID(N'[dbo].[WbAllowedReferents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbAllowedReferents];
GO
IF OBJECT_ID(N'[dbo].[WbDisallowedReferents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbDisallowedReferents];
GO
IF OBJECT_ID(N'[dbo].[WbRefDescTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbRefDescTemplates];
GO
IF OBJECT_ID(N'[dbo].[WbReferItemTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbReferItemTemplates];
GO
IF OBJECT_ID(N'[dbo].[WbReferTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbReferTemplates];
GO
IF OBJECT_ID(N'[dbo].[WbReferTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbReferTypes];
GO
IF OBJECT_ID(N'[dbo].[WbEntryExtendeds]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbEntryExtendeds];
GO
IF OBJECT_ID(N'[dbo].[WbDefinitionFiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbDefinitionFiles];
GO

IF OBJECT_ID(N'[dbo].[WbBranchCommands]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WbBranchCommands];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'WbAttachments'
CREATE TABLE [dbo].[WbAttachments] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(50)  NULL,
    [Description] nvarchar(1024)  NULL,
    [AttachedFile] varbinary(max)  NOT NULL,
    [Group] nvarchar(50)  NULL,
    [InstanceId] bigint  NOT NULL
);
GO

-- Creating table 'WbComments'
CREATE TABLE [dbo].[WbComments] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(250)  NULL,
    [Dt] datetime  NOT NULL,
    [Group] nvarchar(50)  NULL,
    [WriterId] bigint  NOT NULL,
    [InstanceId] bigint  NOT NULL,
    [IsGlobal] bit  NULL,
    [CommentType] int  NULL,
    [EntryId] bigint  NOT NULL
);
GO

-- Creating table 'WbCommentTemplates'
CREATE TABLE [dbo].[WbCommentTemplates] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Caption] nvarchar(50)  NULL,
    [Comment] nvarchar(3000)  NULL,
    [OwenerId] bigint  NOT NULL
);
GO

-- Creating table 'WbFolders'
CREATE TABLE [dbo].[WbFolders] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Code] nvarchar(10)  NOT NULL,
    [LevelOrder] int  NOT NULL,
    [WorkboxFilter] nvarchar(3000)  NULL,
    [IsInactive] bit  NOT NULL,
    [IsVolatile] bit  NOT NULL,
    [MustOpen] bit  NOT NULL,
    [IsDefault] bit  NOT NULL,
    [IsPersisted] bit  NOT NULL,
    [BackgroundColor] nvarchar(6)  NULL,
    [ForegroundColor] nvarchar(6)  NULL,
    [ParentId] bigint  NULL,
    [AllSelectAction] int  NOT NULL,
    [ZeroSelectAction] int  NOT NULL,
    [IsLeaf] bit  NOT NULL
);
GO

-- Creating table 'WbLabels'
CREATE TABLE [dbo].[WbLabels] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [LatinName] nvarchar(50)  NULL,
    [BackgroundColor] nvarchar(6)  NULL,
    [ForegroundColor] nvarchar(6)  NULL,
    [ParentId] bigint  NULL,
    [PersonWorkboxId] bigint  NOT NULL,
    [LevelOrder] int  NULL,
    [IsInactive] bit  NULL,
    [IsLeaf] bit  NOT NULL,
    [IsPublic] bit  NOT NULL
);
GO

-- Creating table 'WbMessages'
CREATE TABLE [dbo].[WbMessages] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Subject] nvarchar(50)  NULL,
    [Body] nvarchar(max)  NULL,
    [SendDt] datetime  NULL,
    [RegDt] datetime  NOT NULL,
    [Attachments] varbinary(max)  NULL,
    [SenderId] bigint  NOT NULL
);
GO

-- Creating table 'WbUrgencies'
CREATE TABLE [dbo].[WbUrgencies] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [BgColor] nvarchar(6)  NULL,
    [FgColor] nvarchar(6)  NULL,
    [Level] int  NOT NULL,
    [IsSystematic] bit  NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'WbJobs'
CREATE TABLE [dbo].[WbJobs] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RefSettings] nvarchar(3000)  NULL,
    [WorkReport] nvarchar(512)  NULL,
    [Parameters] nvarchar(2048)  NULL,
    [VisibleFields] nvarchar(3000)  NULL,
    [EditableFields] nvarchar(3000)  NULL,
    [HiddenFields] nvarchar(3000)  NULL,
    [DefaultValues] nvarchar(2048)  NULL,
    [FormName] varchar(50)  NULL,
    [FormCaption] nvarchar(50)  NULL,
    [PlayerName] varchar(20)  NOT NULL,
    [Description] nvarchar(500)  NULL,
    [Subject] nvarchar(200)  NOT NULL,
    [EntityId] nvarchar(100)  NULL,
    [LockAfterNthSave] int  NOT NULL,
    [LastSaveDt] datetime  NULL,
    [StrMetadata] nvarchar(1000)  NULL,
    [Tags] nvarchar(100)  NULL,
    [CurrentStationCaption] nvarchar(100)  NULL,
    [CurrentStationId] nvarchar(60)  NULL,
    [Group] nvarchar(25)  NULL,
    [SpinDt] datetime  NULL,
    [CustomSpinnable] bit  NULL,
    [ReturnSpinnable] bit  NULL,
    [SpinnerId] bigint  NULL,
    [IsSaved] bit  NOT NULL,
    [IsSpinned] bit  NOT NULL,
    [IsReadOnly] bit  NOT NULL,
    [IsAssistant] bit  NOT NULL,
    [KindOfAssistance] int  NOT NULL,
    [TypeOfStationExit] int  NULL,
    [CurrentTrackNodeUid] nvarchar(60)  NULL,
    [InstanceId] bigint  NULL,
    [UrgencyId] bigint  NULL,
    [SystemId] bigint  NULL,
    [AssignmentId] bigint  NULL,
    [PreviewId] bigint  NULL
);
GO

-- Creating table 'WbEntries'
CREATE TABLE [dbo].[WbEntries] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Row_Version] binary(8)  NULL,
    [EntryDt] datetime  NULL,
    [ReadDt] datetime  NULL,
    [IsRead] bit  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [IsRejected] bit  NOT NULL,
    [IsReferRejected] bit  NOT NULL,
    [IsLocked] bit  NOT NULL,
    [IsReferred] bit  NOT NULL,
    [IsSaver] bit  NOT NULL,
    [Referable] bit  NOT NULL,
    [Rejectable] bit  NOT NULL,
    [Spinnable] bit  NOT NULL,
    [Commentable] bit  NOT NULL,
    [Attachable] bit  NOT NULL,
    [Labels] nvarchar(110)  NULL,
    [SubsequentCount] int  NOT NULL,
    [Deadline] int  NULL,
    [ReferDesc] nvarchar(150)  NULL,
    [IsRejectEntry] bit  NULL,
    [IsHiddenRefer] bit  NOT NULL,
    [WorkReportStatus] int  NULL,
    [IsHiddenDesc] bit  NULL,
    [SenderName] nvarchar(50)  NULL,
    [RecieverName] nvarchar(50)  NULL,
    [StopTime] int  NULL,
    [IsArchived] bit  NOT NULL,
    [IsObsolete] bit  NOT NULL,
    [RecieverId] bigint  NOT NULL,
    [SenderId] bigint  NOT NULL,
    [PrevId] bigint  NULL,
    [PersonWorkboxId] bigint  NOT NULL,
    [JobId] bigint  NOT NULL,
    [ExtendedId] bigint  NULL,
    [InstanceId] bigint  NULL,
    [SystemId] bigint  NULL,
    [AssignmentId] bigint  NULL,
    [IsConfirmed] bit  NOT NULL
);
GO

-- Creating table 'WbPersonWorkboxes'
CREATE TABLE [dbo].[WbPersonWorkboxes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [AllowViewSlaveWorkboxes] bit  NOT NULL,
    [PersonId] bigint  NULL,
    [ActiveDefaultSubstitution] bit  NULL,
    [VisibleWorkboxMemIds] nvarchar(50)  NULL,
    [OwenerId] bigint  NOT NULL
);
GO

-- Creating table 'WbWorkflowAssignments'
CREATE TABLE [dbo].[WbWorkflowAssignments] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL,
    [Code] varchar(50)  NULL,
    [AssignmentDt] datetime  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [Count] int  NULL,
    [ModelTypeName] varchar(200)  NULL,
    [AssemblyName] varchar(200)  NULL,
    [ExecConditionSpel] nvarchar(2048)  NULL,
    [Description] nvarchar(1024)  NULL,
    [IsActive] bit  NOT NULL,
    [SaveLogLevel] int  NOT NULL,
    [GroupAttachmentsType] int  NOT NULL,
    [GroupCommentsType] int  NOT NULL,
    [ArchiveWhenSpin] bit  NOT NULL,
    [Responsible] bigint  NULL,
    [SystemId] bigint  NOT NULL,
    [RemoveSimilar] int  NOT NULL,
    [ActionWhenSpin] int  NOT NULL,
    [RevalSubject] bit  NULL,
    [DefaultPlayerName] varchar(20)  NOT NULL,
    [DefaultFormName] varchar(50)  NULL,
    [DefinitionFileId] int  NOT NULL
);
GO

-- Creating table 'WbAssignmentInstances'
CREATE TABLE [dbo].[WbAssignmentInstances] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [StartDt] datetime  NOT NULL,
    [EntityId] nvarchar(50)  NOT NULL,
    [Wfiid] uniqueidentifier  NOT NULL,
    [Starter] bigint  NOT NULL,
    [AssignmentId] bigint  NOT NULL,
    [DefaultFormName] varchar(50)  NULL,
    [DefaultPlayerName] varchar(50)  NOT NULL,
    [SystemId] bigint  NOT NULL,
    [GroupAttachmentsType] int  NOT NULL,
    [GroupCommentsType] int  NOT NULL,
    [SystemCode] nvarchar(20)  NOT NULL,
    [DefinitionFileId] int  NOT NULL,
    [HasAttachment] bit  NOT NULL,
    [HasComment] bit  NOT NULL,
    [ParrentInstanceId] bigint  NULL
);
GO

-- Creating table 'WbWorkflowSystems'
CREATE TABLE [dbo].[WbWorkflowSystems] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Caption] nvarchar(50)  NOT NULL,
    [Code] nvarchar(20)  NOT NULL,
    [Description] nvarchar(500)  NULL,
    [SettingForm] nvarchar(50)  NULL
);
GO

-- Creating table 'WbSubstitutionExceptions'
CREATE TABLE [dbo].[WbSubstitutionExceptions] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [StartDt] datetime  NOT NULL,
    [EndDt] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [Referable] bit  NOT NULL,
    [Spinnable] bit  NOT NULL,
    [Confirmable] bit  NOT NULL,
    [Commentable] bit  NOT NULL,
    [Attachable] bit  NOT NULL,
    [Editable] bit  NOT NULL,
    [HiddenReferVisible] bit  NOT NULL,
    [RefTakeback] bit  NOT NULL,
    [WfTakeback] bit  NOT NULL,
    [Visible] bit  NOT NULL,
    [SubstituteId] bigint  NOT NULL,
    [ExceptionUserId] bigint  NULL,
    [ExceptionUnitId] bigint  NULL
);
GO

-- Creating table 'WbSubstitutions'
CREATE TABLE [dbo].[WbSubstitutions] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [StartDt] datetime  NOT NULL,
    [EndDt] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [Referable] bit  NOT NULL,
    [Spinnable] bit  NOT NULL,
    [Confirmable] bit  NOT NULL,
    [Commentable] bit  NOT NULL,
    [Attachable] bit  NOT NULL,
    [HiddenReferVisible] bit  NOT NULL,
    [RefTakeback] bit  NOT NULL,
    [WfTakeback] bit  NOT NULL,
    [Editable] bit  NOT NULL,
    [AbsentUserId] bigint  NULL,
    [AbsentUnitId] bigint  NULL,
    [SubstituteUserId] bigint  NOT NULL
);
GO

-- Creating table 'WbMacros'
CREATE TABLE [dbo].[WbMacros] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Description] nvarchar(500)  NULL,
    [TypeName] nvarchar(500)  NOT NULL
);
GO

-- Creating table 'WbPreviews'
CREATE TABLE [dbo].[WbPreviews] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [TypeName] nvarchar(50)  NOT NULL,
    [EntityId] nvarchar(20)  NOT NULL,
    [HtmlPreview] nvarchar(3000)  NULL
);
GO

-- Creating table 'WbScheduledMessages'
CREATE TABLE [dbo].[WbScheduledMessages] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Subject] nvarchar(50)  NULL,
    [Body] nvarchar(max)  NULL,
    [SendDt] datetime  NULL,
    [RegDt] datetime  NOT NULL,
    [Attachments] varbinary(max)  NULL,
    [SenderId] bigint  NOT NULL
);
GO

-- Creating table 'WbWorkboxMacros'
CREATE TABLE [dbo].[WbWorkboxMacros] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [IsActive] bit  NOT NULL,
    [Description] nvarchar(500)  NULL,
    [MacroConfig] nvarchar(3000)  NULL,
    [WorkboxId] bigint  NOT NULL,
    [MacroId] bigint  NOT NULL,
    [Metadata] nvarchar(3000)  NOT NULL,
    [Criteria] nvarchar(3000)  NOT NULL
);
GO

-- Creating table 'WbAllowedReferents'
CREATE TABLE [dbo].[WbAllowedReferents] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [StartDt] datetime  NOT NULL,
    [EndDt] datetime  NOT NULL,
    [AllowedIds] nvarchar(3000)  NOT NULL,
    [OwenerUserId] bigint  NULL,
    [OwenerUnitId] bigint  NULL,
    [SystemId] bigint  NOT NULL
);
GO

-- Creating table 'WbDisallowedReferents'
CREATE TABLE [dbo].[WbDisallowedReferents] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [StartDt] datetime  NOT NULL,
    [EndDt] datetime  NOT NULL,
    [DisallowedIds] nvarchar(3000)  NOT NULL,
    [OwenerUserId] bigint  NULL,
    [OwenerUnitId] bigint  NULL,
    [SystemId] bigint  NOT NULL
);
GO

-- Creating table 'WbRefDescTemplates'
CREATE TABLE [dbo].[WbRefDescTemplates] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Caption] nvarchar(50)  NULL,
    [Description] nvarchar(3000)  NULL,
    [OwenerId] bigint  NOT NULL
);
GO

-- Creating table 'WbReferItemTemplates'
CREATE TABLE [dbo].[WbReferItemTemplates] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [TargetUnitId] bigint  NULL,
    [TargetUserId] bigint  NULL,
    [ReferTypeId] bigint  NULL,
    [ReferTemplateId] bigint  NOT NULL
);
GO

-- Creating table 'WbReferTemplates'
CREATE TABLE [dbo].[WbReferTemplates] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [OwenerId] bigint  NOT NULL,
    [SystemId] bigint  NOT NULL,
    [AllowRepetitive] bit  NULL,
    [IncludeSubUnits] bit  NULL,
    [Comment] nvarchar(500)  NULL
);
GO

-- Creating table 'WbReferTypes'
CREATE TABLE [dbo].[WbReferTypes] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(50)  NOT NULL,
    [Spinnable] bit  NOT NULL,
    [Commentable] bit  NOT NULL,
    [Attachable] bit  NOT NULL,
    [Referable] bit  NOT NULL,
    [Rejectable] bit  NOT NULL,
    [IsHiddenRefer] bit  NOT NULL,
    [SystemId] bigint  NULL,
    [IsLock] bit  NOT NULL,
    [RequireWorkReport] bit  NOT NULL,
    [Deadline] int  NULL,
    [LabelId] bigint  NOT NULL
);
GO

-- Creating table 'WbEntryExtendeds'
CREATE TABLE [dbo].[WbEntryExtendeds] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [RefSettings] nvarchar(max)  NULL,
    [WorkReport] nvarchar(max)  NULL,
    [InstanceId] bigint  NULL
);
GO

-- Creating table 'WbDefinitionFiles'
CREATE TABLE [dbo].[WbDefinitionFiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(120)  NOT NULL,
    [Xml] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'WbWorkboxJobs'

CREATE VIEW WbWorkboxJobs
AS
SELECT 
    PB.OwenerId AS  [Workbox_OwenerID] ,
    E.[Id] ,
    [Row_Version],
    [EntryDt] ,
    [ReadDt] ,
    [IsRead],
    [IsDeleted] ,
    [IsRejected] ,
    [IsReferRejected],
    [IsLocked] ,
    [IsReferred] ,
    [IsSaver] ,
    [Referable] ,
    [Rejectable] ,
    [Spinnable] ,
    [Commentable] ,
    [Attachable],
    [Labels] ,
    [SubsequentCount] ,
    [Deadline] ,
    [ReferDesc] ,
    [IsRejectEntry] ,
    [IsHiddenRefer] ,
    [WorkReportStatus] ,
    [IsHiddenDesc] ,
    [SenderName] ,
    [RecieverName] ,
    [StopTime] ,
    [IsArchived] ,
    [IsObsolete] ,
    [RecieverId] ,
    [SenderId] ,
    [PrevId] ,
    [PersonWorkboxId] ,
    [JobId],
    [ExtendedId] ,
    E.[InstanceId] ,
    E.[SystemId] ,
    [Subject],
    J.[Description] AS [JDescription] ,
    [CustomSpinnable] ,
    [ReturnSpinnable] ,
    [FormCaption] ,
    [FormName] ,
    [LastSaveDt] ,
    [Tags] ,
    [StrMetadata] ,
    [CurrentStationCaption] ,
    [CurrentStationId] ,
    [CurrentTrackNodeUid] ,
    [Group] ,
    [SpinnerId] ,
    [UrgencyId],
    [IsSaved],
    [LockAfterNthSave] ,
    [PlayerName] ,
    J.[EntityId] ,
    [IsAssistant],
    [KindOfAssistance],
    [IsReadOnly],
    [IsSpinned],
    [SpinDt] ,
    [PreviewId] ,
    E.[AssignmentId],
    [IsConfirmed] ,
    [Parameters] ,
    [HasAttachment] ,
    [HasComment] ,
    [GroupAttachmentsType] ,
    [GroupCommentsType] ,
    [WorkReport]
FROM [dbo].[WbEntries] AS E
INNER JOIN [dbo].[WbPersonWorkboxes] AS PB ON E.PersonWorkboxId = PB.Id
LEFT OUTER JOIN [dbo].[WbJobs] AS J ON J.EntityId = E.Id
LEFT OUTER JOIN [dbo].[WbAssignmentInstances] AS AI ON E.AssignmentId = AI.Id

GO

-- Creating table 'WbBranchCommands'
CREATE TABLE [dbo].[WbBranchCommands] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [CommandName] varchar(100)  NOT NULL,
    [Description] nvarchar(1000)  NULL,
    [TargetUserIds] varchar(500)  NULL,
    [JobId] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'WbAttachments'
ALTER TABLE [dbo].[WbAttachments]
ADD CONSTRAINT [PK_WbAttachments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbComments'
ALTER TABLE [dbo].[WbComments]
ADD CONSTRAINT [PK_WbComments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbCommentTemplates'
ALTER TABLE [dbo].[WbCommentTemplates]
ADD CONSTRAINT [PK_WbCommentTemplates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbFolders'
ALTER TABLE [dbo].[WbFolders]
ADD CONSTRAINT [PK_WbFolders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbLabels'
ALTER TABLE [dbo].[WbLabels]
ADD CONSTRAINT [PK_WbLabels]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbMessages'
ALTER TABLE [dbo].[WbMessages]
ADD CONSTRAINT [PK_WbMessages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbUrgencies'
ALTER TABLE [dbo].[WbUrgencies]
ADD CONSTRAINT [PK_WbUrgencies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbJobs'
ALTER TABLE [dbo].[WbJobs]
ADD CONSTRAINT [PK_WbJobs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [PK_WbEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbPersonWorkboxes'
ALTER TABLE [dbo].[WbPersonWorkboxes]
ADD CONSTRAINT [PK_WbPersonWorkboxes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbWorkflowAssignments'
ALTER TABLE [dbo].[WbWorkflowAssignments]
ADD CONSTRAINT [PK_WbWorkflowAssignments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbAssignmentInstances'
ALTER TABLE [dbo].[WbAssignmentInstances]
ADD CONSTRAINT [PK_WbAssignmentInstances]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbWorkflowSystems'
ALTER TABLE [dbo].[WbWorkflowSystems]
ADD CONSTRAINT [PK_WbWorkflowSystems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbSubstitutionExceptions'
ALTER TABLE [dbo].[WbSubstitutionExceptions]
ADD CONSTRAINT [PK_WbSubstitutionExceptions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbSubstitutions'
ALTER TABLE [dbo].[WbSubstitutions]
ADD CONSTRAINT [PK_WbSubstitutions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbMacros'
ALTER TABLE [dbo].[WbMacros]
ADD CONSTRAINT [PK_WbMacros]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbPreviews'
ALTER TABLE [dbo].[WbPreviews]
ADD CONSTRAINT [PK_WbPreviews]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbScheduledMessages'
ALTER TABLE [dbo].[WbScheduledMessages]
ADD CONSTRAINT [PK_WbScheduledMessages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbWorkboxMacros'
ALTER TABLE [dbo].[WbWorkboxMacros]
ADD CONSTRAINT [PK_WbWorkboxMacros]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbAllowedReferents'
ALTER TABLE [dbo].[WbAllowedReferents]
ADD CONSTRAINT [PK_WbAllowedReferents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbDisallowedReferents'
ALTER TABLE [dbo].[WbDisallowedReferents]
ADD CONSTRAINT [PK_WbDisallowedReferents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbRefDescTemplates'
ALTER TABLE [dbo].[WbRefDescTemplates]
ADD CONSTRAINT [PK_WbRefDescTemplates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbReferItemTemplates'
ALTER TABLE [dbo].[WbReferItemTemplates]
ADD CONSTRAINT [PK_WbReferItemTemplates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbReferTemplates'
ALTER TABLE [dbo].[WbReferTemplates]
ADD CONSTRAINT [PK_WbReferTemplates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbReferTypes'
ALTER TABLE [dbo].[WbReferTypes]
ADD CONSTRAINT [PK_WbReferTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbEntryExtendeds'
ALTER TABLE [dbo].[WbEntryExtendeds]
ADD CONSTRAINT [PK_WbEntryExtendeds]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WbDefinitionFiles'
ALTER TABLE [dbo].[WbDefinitionFiles]
ADD CONSTRAINT [PK_WbDefinitionFiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO


-- Creating primary key on [Id] in table 'WbBranchCommands'
ALTER TABLE [dbo].[WbBranchCommands]
ADD CONSTRAINT [PK_WbBranchCommands]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [InstanceId] in table 'WbAttachments'
ALTER TABLE [dbo].[WbAttachments]
ADD CONSTRAINT [FK_Attachments_WorkflowInstances_WorkflowInfo]
    FOREIGN KEY ([InstanceId])
    REFERENCES [dbo].[WbAssignmentInstances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Attachments_WorkflowInstances_WorkflowInfo'
CREATE INDEX [IX_FK_Attachments_WorkflowInstances_WorkflowInfo]
ON [dbo].[WbAttachments]
    ([InstanceId]);
GO

-- Creating foreign key on [InstanceId] in table 'WbComments'
ALTER TABLE [dbo].[WbComments]
ADD CONSTRAINT [FK_Comments_WorkflowInstances_WorkflowInfo]
    FOREIGN KEY ([InstanceId])
    REFERENCES [dbo].[WbAssignmentInstances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Comments_WorkflowInstances_WorkflowInfo'
CREATE INDEX [IX_FK_Comments_WorkflowInstances_WorkflowInfo]
ON [dbo].[WbComments]
    ([InstanceId]);
GO

-- Creating foreign key on [ParentId] in table 'WbFolders'
ALTER TABLE [dbo].[WbFolders]
ADD CONSTRAINT [FK_Folders_Folders_Parent]
    FOREIGN KEY ([ParentId])
    REFERENCES [dbo].[WbFolders]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Folders_Folders_Parent'
CREATE INDEX [IX_FK_Folders_Folders_Parent]
ON [dbo].[WbFolders]
    ([ParentId]);
GO

-- Creating foreign key on [ParentId] in table 'WbLabels'
ALTER TABLE [dbo].[WbLabels]
ADD CONSTRAINT [FK_Labels_Labels_Parent]
    FOREIGN KEY ([ParentId])
    REFERENCES [dbo].[WbLabels]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Labels_Labels_Parent'
CREATE INDEX [IX_FK_Labels_Labels_Parent]
ON [dbo].[WbLabels]
    ([ParentId]);
GO

-- Creating foreign key on [PersonWorkboxId] in table 'WbLabels'
ALTER TABLE [dbo].[WbLabels]
ADD CONSTRAINT [FK_Labels_Workboxs_Workbox]
    FOREIGN KEY ([PersonWorkboxId])
    REFERENCES [dbo].[WbPersonWorkboxes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Labels_Workboxs_Workbox'
CREATE INDEX [IX_FK_Labels_Workboxs_Workbox]
ON [dbo].[WbLabels]
    ([PersonWorkboxId]);
GO

-- Creating foreign key on [PrevId] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [FK_Entries_Entries_Prev]
    FOREIGN KEY ([PrevId])
    REFERENCES [dbo].[WbEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entries_Entries_Prev'
CREATE INDEX [IX_FK_Entries_Entries_Prev]
ON [dbo].[WbEntries]
    ([PrevId]);
GO

-- Creating foreign key on [PersonWorkboxId] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [FK_Entries_Workboxs_Workbox]
    FOREIGN KEY ([PersonWorkboxId])
    REFERENCES [dbo].[WbPersonWorkboxes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entries_Workboxs_Workbox'
CREATE INDEX [IX_FK_Entries_Workboxs_Workbox]
ON [dbo].[WbEntries]
    ([PersonWorkboxId]);
GO

-- Creating foreign key on [SystemId] in table 'WbWorkflowAssignments'
ALTER TABLE [dbo].[WbWorkflowAssignments]
ADD CONSTRAINT [FK_WorkflowAssignments_WorkflowSystems_System]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkflowAssignments_WorkflowSystems_System'
CREATE INDEX [IX_FK_WorkflowAssignments_WorkflowSystems_System]
ON [dbo].[WbWorkflowAssignments]
    ([SystemId]);
GO

-- Creating foreign key on [WorkboxId] in table 'WbWorkboxMacros'
ALTER TABLE [dbo].[WbWorkboxMacros]
ADD CONSTRAINT [FK_WorkboxMacroPersonWorkbox]
    FOREIGN KEY ([WorkboxId])
    REFERENCES [dbo].[WbPersonWorkboxes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkboxMacroPersonWorkbox'
CREATE INDEX [IX_FK_WorkboxMacroPersonWorkbox]
ON [dbo].[WbWorkboxMacros]
    ([WorkboxId]);
GO

-- Creating foreign key on [SystemId] in table 'WbAllowedReferents'
ALTER TABLE [dbo].[WbAllowedReferents]
ADD CONSTRAINT [FK_WbAllowedReferentWorkflowSystem]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WbAllowedReferentWorkflowSystem'
CREATE INDEX [IX_FK_WbAllowedReferentWorkflowSystem]
ON [dbo].[WbAllowedReferents]
    ([SystemId]);
GO

-- Creating foreign key on [SystemId] in table 'WbDisallowedReferents'
ALTER TABLE [dbo].[WbDisallowedReferents]
ADD CONSTRAINT [FK_WbDisallowedReferentWorkflowSystem]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WbDisallowedReferentWorkflowSystem'
CREATE INDEX [IX_FK_WbDisallowedReferentWorkflowSystem]
ON [dbo].[WbDisallowedReferents]
    ([SystemId]);
GO

-- Creating foreign key on [MacroId] in table 'WbWorkboxMacros'
ALTER TABLE [dbo].[WbWorkboxMacros]
ADD CONSTRAINT [FK_WorkboxMacroMacro]
    FOREIGN KEY ([MacroId])
    REFERENCES [dbo].[WbMacros]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkboxMacroMacro'
CREATE INDEX [IX_FK_WorkboxMacroMacro]
ON [dbo].[WbWorkboxMacros]
    ([MacroId]);
GO

-- Creating foreign key on [ReferTypeId] in table 'WbReferItemTemplates'
ALTER TABLE [dbo].[WbReferItemTemplates]
ADD CONSTRAINT [FK_WbReferItemTemplates_WbReferTypes_ReferType]
    FOREIGN KEY ([ReferTypeId])
    REFERENCES [dbo].[WbReferTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WbReferItemTemplates_WbReferTypes_ReferType'
CREATE INDEX [IX_FK_WbReferItemTemplates_WbReferTypes_ReferType]
ON [dbo].[WbReferItemTemplates]
    ([ReferTypeId]);
GO

-- Creating foreign key on [SystemId] in table 'WbReferTypes'
ALTER TABLE [dbo].[WbReferTypes]
ADD CONSTRAINT [FK_ReferTypeWorkflowSystem]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReferTypeWorkflowSystem'
CREATE INDEX [IX_FK_ReferTypeWorkflowSystem]
ON [dbo].[WbReferTypes]
    ([SystemId]);
GO

-- Creating foreign key on [SystemId] in table 'WbReferTemplates'
ALTER TABLE [dbo].[WbReferTemplates]
ADD CONSTRAINT [FK_ReferTemplateWorkflowSystem]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReferTemplateWorkflowSystem'
CREATE INDEX [IX_FK_ReferTemplateWorkflowSystem]
ON [dbo].[WbReferTemplates]
    ([SystemId]);
GO

-- Creating foreign key on [InstanceId] in table 'WbJobs'
ALTER TABLE [dbo].[WbJobs]
ADD CONSTRAINT [FK_JobAssignmentInstance]
    FOREIGN KEY ([InstanceId])
    REFERENCES [dbo].[WbAssignmentInstances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JobAssignmentInstance'
CREATE INDEX [IX_FK_JobAssignmentInstance]
ON [dbo].[WbJobs]
    ([InstanceId]);
GO

-- Creating foreign key on [UrgencyId] in table 'WbJobs'
ALTER TABLE [dbo].[WbJobs]
ADD CONSTRAINT [FK_JobUrgency]
    FOREIGN KEY ([UrgencyId])
    REFERENCES [dbo].[WbUrgencies]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JobUrgency'
CREATE INDEX [IX_FK_JobUrgency]
ON [dbo].[WbJobs]
    ([UrgencyId]);
GO

-- Creating foreign key on [SystemId] in table 'WbJobs'
ALTER TABLE [dbo].[WbJobs]
ADD CONSTRAINT [FK_JobWorkflowSystem]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JobWorkflowSystem'
CREATE INDEX [IX_FK_JobWorkflowSystem]
ON [dbo].[WbJobs]
    ([SystemId]);
GO

-- Creating foreign key on [AssignmentId] in table 'WbJobs'
ALTER TABLE [dbo].[WbJobs]
ADD CONSTRAINT [FK_WorkflowAssignmentJob]
    FOREIGN KEY ([AssignmentId])
    REFERENCES [dbo].[WbWorkflowAssignments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkflowAssignmentJob'
CREATE INDEX [IX_FK_WorkflowAssignmentJob]
ON [dbo].[WbJobs]
    ([AssignmentId]);
GO

-- Creating foreign key on [JobId] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [FK_EntryJob]
    FOREIGN KEY ([JobId])
    REFERENCES [dbo].[WbJobs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntryJob'
CREATE INDEX [IX_FK_EntryJob]
ON [dbo].[WbEntries]
    ([JobId]);
GO

-- Creating foreign key on [PreviewId] in table 'WbJobs'
ALTER TABLE [dbo].[WbJobs]
ADD CONSTRAINT [FK_JobPreview]
    FOREIGN KEY ([PreviewId])
    REFERENCES [dbo].[WbPreviews]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JobPreview'
CREATE INDEX [IX_FK_JobPreview]
ON [dbo].[WbJobs]
    ([PreviewId]);
GO

-- Creating foreign key on [ExtendedId] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [FK_EntryEntryExtended]
    FOREIGN KEY ([ExtendedId])
    REFERENCES [dbo].[WbEntryExtendeds]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntryEntryExtended'
CREATE INDEX [IX_FK_EntryEntryExtended]
ON [dbo].[WbEntries]
    ([ExtendedId]);
GO

-- Creating foreign key on [InstanceId] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [FK_EntryAssignmentInstance]
    FOREIGN KEY ([InstanceId])
    REFERENCES [dbo].[WbAssignmentInstances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntryAssignmentInstance'
CREATE INDEX [IX_FK_EntryAssignmentInstance]
ON [dbo].[WbEntries]
    ([InstanceId]);
GO

-- Creating foreign key on [AssignmentId] in table 'WbAssignmentInstances'
ALTER TABLE [dbo].[WbAssignmentInstances]
ADD CONSTRAINT [FK_AssignmentInstanceWorkflowAssignment]
    FOREIGN KEY ([AssignmentId])
    REFERENCES [dbo].[WbWorkflowAssignments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AssignmentInstanceWorkflowAssignment'
CREATE INDEX [IX_FK_AssignmentInstanceWorkflowAssignment]
ON [dbo].[WbAssignmentInstances]
    ([AssignmentId]);
GO

-- Creating foreign key on [SystemId] in table 'WbAssignmentInstances'
ALTER TABLE [dbo].[WbAssignmentInstances]
ADD CONSTRAINT [FK_AssignmentInstanceWorkflowSystem]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AssignmentInstanceWorkflowSystem'
CREATE INDEX [IX_FK_AssignmentInstanceWorkflowSystem]
ON [dbo].[WbAssignmentInstances]
    ([SystemId]);
GO

-- Creating foreign key on [EntryId] in table 'WbComments'
ALTER TABLE [dbo].[WbComments]
ADD CONSTRAINT [FK_CommentEntry]
    FOREIGN KEY ([EntryId])
    REFERENCES [dbo].[WbEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CommentEntry'
CREATE INDEX [IX_FK_CommentEntry]
ON [dbo].[WbComments]
    ([EntryId]);
GO

-- Creating foreign key on [SystemId] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [FK_EntryWorkflowSystem]
    FOREIGN KEY ([SystemId])
    REFERENCES [dbo].[WbWorkflowSystems]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntryWorkflowSystem'
CREATE INDEX [IX_FK_EntryWorkflowSystem]
ON [dbo].[WbEntries]
    ([SystemId]);
GO

-- Creating foreign key on [DefinitionFileId] in table 'WbWorkflowAssignments'
ALTER TABLE [dbo].[WbWorkflowAssignments]
ADD CONSTRAINT [FK_WorkflowAssignmentDefinitionFile]
    FOREIGN KEY ([DefinitionFileId])
    REFERENCES [dbo].[WbDefinitionFiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkflowAssignmentDefinitionFile'
CREATE INDEX [IX_FK_WorkflowAssignmentDefinitionFile]
ON [dbo].[WbWorkflowAssignments]
    ([DefinitionFileId]);
GO

-- Creating foreign key on [DefinitionFileId] in table 'WbAssignmentInstances'
ALTER TABLE [dbo].[WbAssignmentInstances]
ADD CONSTRAINT [FK_AssignmentInstanceDefinitionFile]
    FOREIGN KEY ([DefinitionFileId])
    REFERENCES [dbo].[WbDefinitionFiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AssignmentInstanceDefinitionFile'
CREATE INDEX [IX_FK_AssignmentInstanceDefinitionFile]
ON [dbo].[WbAssignmentInstances]
    ([DefinitionFileId]);
GO

-- Creating foreign key on [AssignmentId] in table 'WbEntries'
ALTER TABLE [dbo].[WbEntries]
ADD CONSTRAINT [FK_EntryWorkflowAssignment]
    FOREIGN KEY ([AssignmentId])
    REFERENCES [dbo].[WbWorkflowAssignments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntryWorkflowAssignment'
CREATE INDEX [IX_FK_EntryWorkflowAssignment]
ON [dbo].[WbEntries]
    ([AssignmentId]);
GO

-- Creating foreign key on [ReferTemplateId] in table 'WbReferItemTemplates'
ALTER TABLE [dbo].[WbReferItemTemplates]
ADD CONSTRAINT [FK_ReferItemTemplateReferTemplate]
    FOREIGN KEY ([ReferTemplateId])
    REFERENCES [dbo].[WbReferTemplates]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReferItemTemplateReferTemplate'
CREATE INDEX [IX_FK_ReferItemTemplateReferTemplate]
ON [dbo].[WbReferItemTemplates]
    ([ReferTemplateId]);
GO

-- Creating foreign key on [InstanceId] in table 'WbEntryExtendeds'
ALTER TABLE [dbo].[WbEntryExtendeds]
ADD CONSTRAINT [FK_EntryExtendedAssignmentInstance]
    FOREIGN KEY ([InstanceId])
    REFERENCES [dbo].[WbAssignmentInstances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntryExtendedAssignmentInstance'
CREATE INDEX [IX_FK_EntryExtendedAssignmentInstance]
ON [dbo].[WbEntryExtendeds]
    ([InstanceId]);
GO

-- Creating foreign key on [LabelId] in table 'WbReferTypes'
ALTER TABLE [dbo].[WbReferTypes]
ADD CONSTRAINT [FK_ReferTypeLabel]
    FOREIGN KEY ([LabelId])
    REFERENCES [dbo].[WbLabels]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReferTypeLabel'
CREATE INDEX [IX_FK_ReferTypeLabel]
ON [dbo].[WbReferTypes]
    ([LabelId]);
GO

-- Creating foreign key on [JobId] in table 'WbBranchCommands'
ALTER TABLE [dbo].[WbBranchCommands]
ADD CONSTRAINT [FK_BranchCommandJob]
    FOREIGN KEY ([JobId])
    REFERENCES [dbo].[WbJobs]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BranchCommandJob'
CREATE INDEX [IX_FK_BranchCommandJob]
ON [dbo].[WbBranchCommands]
    ([JobId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------