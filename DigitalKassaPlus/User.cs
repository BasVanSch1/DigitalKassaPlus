using DigitalKassaPlus.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalKassaPlus
{
    internal abstract class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public bool IsActive { get; set; }

        private readonly SingletonDAL dbManager = SingletonDAL.Instance;

        public User(int _id, string _name, string _phone, string _email, bool _isactive)
        {
            Id = _id;
            Name = _name;
            Phone = _phone;
            Email = _email;
            IsActive = _isactive;
        }

        public int? GetPin()
        {
            int? pin = dbManager.GetPinFromUser(Id);

            return pin;
        }   
    }
}