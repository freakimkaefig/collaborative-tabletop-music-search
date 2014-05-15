using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.Common.Domain;

namespace Waf.InformationManager.AddressBook.Modules.Domain
{
    public class Contact : ValidationModel
    {
        private readonly Address address;
        private string firstname;
        private string lastname;
        private string company;
        private string email;
        private string phone;


        public Contact()
        {
            this.address = new Address();
        }


        public string Firstname
        {
            get { return firstname; }
            set
            {
                if (firstname != value)
                {
                    firstname = value;
                    RaisePropertyChanged("Firstname");
                }
            }
        }

        public string Lastname
        {
            get { return lastname; }
            set
            {
                if (lastname != value)
                {
                    lastname = value;
                    RaisePropertyChanged("Lastname");
                }
            }
        }

        public string Company
        {
            get { return company; }
            set
            {
                if (company != value)
                {
                    company = value;
                    RaisePropertyChanged("Company");
                }
            }
        }

        [EmailValidation]
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    RaisePropertyChanged("Email");
                }
            }
        }

        public string Phone
        {
            get { return phone; }
            set
            {
                if (phone != value)
                {
                    phone = value;
                    RaisePropertyChanged("Phone");
                }
            }
        }

        public Address Address { get { return address; } }
    }
}
