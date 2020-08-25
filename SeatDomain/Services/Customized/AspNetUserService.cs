

using Exir.Framework.Common;
using SeatDomain.Models;

namespace SeatDomain.Services
{
    [IgnoreT4Template]
    public partial interface IAspNetUserService : ICrudService<AspNetUser>
    {
        AspNetUser FindByName(string username);
        AspNetUser GetCurrent();

    }

}