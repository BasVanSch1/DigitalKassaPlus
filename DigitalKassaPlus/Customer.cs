using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal class Customer : IEquatable<Customer>
    {
        public int Code { get; private set; }
        public string Name { get; private set; }
        public DateTime BirthDate { get; private set; }
        public string Phone {  get; private set; }
        public string Email { get; private set; }
        public int Points { get; private set; }
        public CustomerCard Card { get; private set; }

        public Customer(int _code, string _name, DateTime _birthdate, string _phone, string _email, int _points, CustomerCard _card)
        {
            Code = _code;
            Name = _name;
            BirthDate = _birthdate;
            Phone = _phone;
            Email = _email;
            Points = _points;
            Card = _card;
        }

        public void AddPoints(int _points)
        {
            Points += _points;
        }

        public void RemovePoints(int _points)
        {
            if (Points <= _points)
            {
                Points = 0;
                return;
            }

            Points -= _points;
        }

        public bool ChangeCard(CustomerCard card)
        {
            if (Card == card)
            {
                return false;
            }

            Card = card;
            return true;

        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Customer);
        }

        public bool Equals(Customer other)
        {
            return !(other is null) &&
                   Code == other.Code;
        }

        public override int GetHashCode()
        {
            return -434485196 + Code.GetHashCode();
        }

        public static bool operator ==(Customer left, Customer right)
        {
            return EqualityComparer<Customer>.Default.Equals(left, right);
        }

        public static bool operator !=(Customer left, Customer right)
        {
            return !(left == right);
        }
    }
}
