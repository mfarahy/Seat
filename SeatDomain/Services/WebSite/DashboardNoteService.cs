using Exir.Framework.Common;
using Exir.Framework.Common.Caching;
using SeatDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeatDomain.Services
{
    public partial interface IDashboardNoteService : IReadOnlySupportMemoryCachedService<DashboardNote, IDashboardNoteService>
    {
        DashboardNote[] GetActiveNotes();
        void Close(int id);
        DashboardNoteDto[] GetVisibleNotesByCurrentUser();
    }

    [IgnoreT4Template]

    public partial class DashboardNoteService : ReadOnlySupportMemoryCachedService<DashboardNote, IDashboardNoteService>, IDashboardNoteService
    {
        public IDashboardUserNoteService DashboardUserNoteService { get; private set; }
        public DashboardNoteService(IRepository<DashboardNote> repository, IReadOnlyRepository<DashboardNote> readOnlyRepository, IDashboardUserNoteService dashboardUserNoteService) 
            : base(repository, readOnlyRepository)
        {
            DashboardUserNoteService = dashboardUserNoteService;
        }

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public DashboardNote[] GetActiveNotes()
        {
            return GetDefaultQuery()
                .Where(x => (!x.StartDt.HasValue || x.StartDt <= DateTime.Now) &&
                 (!x.EndDt.HasValue || x.EndDt >= DateTime.Now))
                 .ToArray();
        }

        [CacheableDependency(typeof(DashboardUserNote))]

        [Cacheable(CacheName = CacheConstants.InMemoryCache)]

        public DashboardNoteDto[] GetVisibleNotesByCurrentUser()
        {
            var username = Authenticater.Value.CurrentIdentity.Name?.ToLower();

            var notes = This<IDashboardNoteService>().GetActiveNotes();

            var closedNotes = DashboardUserNoteService.GetClosedNoteIds(username);

            List<DashboardNoteDto> result = new List<DashboardNoteDto>();
            foreach (var note in notes)
            {
                if (!closedNotes.Contains(note.Id) &&
                    (String.IsNullOrEmpty(note.Roles) || note.Roles.Split(',').Any(x => Security.Value.AmI(x))))
                {
                    result.Add(new DashboardNoteDto()
                    {
                        Color = note.Color,
                        Id = note.Id,
                        Text = note.Text
                    });
                }
            }

            return result.ToArray();
        }

        [CacheableEntityInvalidator(0)]

        public void Close(int id)
        {
            var username = Authenticater.Value.CurrentIdentity.Name.ToLower();

            if (!DashboardUserNoteService.GetDefaultQuery()
                .Where(x => x.NoteId == id && x.UserName == username)
                .Any())
            {
                DashboardUserNoteService.Save(new DashboardUserNote()
                {
                    UserName = username,
                    NoteId = id,
                    SeenDt = DateTime.Now
                });
            }
        }

    }

}
