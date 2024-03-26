using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class CustomerCard : IEquatable<CustomerCard>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }

        public CustomerCard(int _id, bool _isActive) 
        {
            Id = _id;
            IsActive = _isActive;
        }

        public CustomerCard(int _id) :
            this(_id, true)
        { }

        public void SetActive(bool state)
        {
            IsActive = state;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CustomerCard);
        }

        public bool Equals(CustomerCard other)
        {
            return !(other is null) &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hashCode = -55384001;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CustomerCard left, CustomerCard right)
        {
            return EqualityComparer<CustomerCard>.Default.Equals(left, right);
        }

        public static bool operator !=(CustomerCard left, CustomerCard right)
        {
            return !(left == right);
        }
    }
}
