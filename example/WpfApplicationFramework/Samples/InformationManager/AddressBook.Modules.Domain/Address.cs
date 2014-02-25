using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.Common.Domain;

namespace Waf.InformationManager.AddressBook.Modules.Domain
{
    public class Address : ValidationModel
    {
        private string street;
        private string city;
        private string state;
        private string postalCode;
        private string country;


        public string Street
        {
            get { return street; }
            set
            {
                if (street != value)
                {
                    street = value;
                    RaisePropertyChanged("Street");
                }
            }
        }

        public string City
        {
            get { return city; }
            set
            {
                if (city != value)
                {
                    city = value;
                    RaisePropertyChanged("City");
                }
            }
        }

        public string State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    RaisePropertyChanged("State");
                }
            }
        }

        public string PostalCode
        {
            get { return postalCode; }
            set
            {
                if (postalCode != value)
                {
                    postalCode = value;
                    RaisePropertyChanged("PostalCode");
                }
            }
        }

        public string Country
        {
            get { return country; }
            set
            {
                if (country != value)
                {
                    country = value;
                    RaisePropertyChanged("Country");
                }
            }
        }
    }
}
