using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.Data;

namespace Ctms.Applications.DataFactories
{
    public class BaseFactory
    {
        protected Repository _repository;

        public BaseFactory(Repository repository)
        {
            _repository = repository;
        }
    }
}
