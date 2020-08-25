using Exir.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatDomain.Services
{
    public interface IThumbnailPersistanceService : IService
    {
        void SaveThumbnail(Guid key, int maxWidth, int maxHeight, byte[] thumbnail);
        byte[] GetThumbnail(Guid key, int maxWidth, int maxHeight);
        void ChangeExtension(Guid key, short extensionTypeCode);
    }
}
