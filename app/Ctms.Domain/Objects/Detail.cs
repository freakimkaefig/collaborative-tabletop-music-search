using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctms.Domain.Objects
{
    public class Detail
    {
        private String _city;
        private String _biography;

        public Detail(String city, String biography)
        {
            _city = city;
            _biography = biography;
        }

        public String City { get { return _city; } set { _city = value; } }
        public String Biography { get { return _biography; } set { _biography = value; } }
    }
}
