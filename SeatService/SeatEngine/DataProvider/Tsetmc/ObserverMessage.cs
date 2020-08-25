using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatService.SeatServiceEngine.DataProvider.Tsetmc
{
    public class ObserverMessage : IEquatable<ObserverMessage>
    {
        public string Subject { get; set; }
        public DateTime MessageDt { get; set; }
        public string Description { get; set; }
        public List<long> RelativeInstances { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ObserverMessage another)
            {
                return another.Subject == Subject && another.MessageDt == MessageDt && another.Description == Description;
            }
            return false;
        }

        public bool Equals(ObserverMessage other)
        {
            return other != null &&
                   Subject == other.Subject &&
                   MessageDt == other.MessageDt &&
                   Description == other.Description;
        }

        public override int GetHashCode()
        {
            var hashCode = 803565732;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Subject);
            hashCode = hashCode * -1521134295 + MessageDt.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            return hashCode;
        }

        public static bool operator ==(ObserverMessage left, ObserverMessage right)
        {
            return EqualityComparer<ObserverMessage>.Default.Equals(left, right);
        }

        public static bool operator !=(ObserverMessage left, ObserverMessage right)
        {
            return !(left == right);
        }
    }
}
