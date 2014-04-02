using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.Services
{
    //Provides entities (data from database and files). Just getters, no setters, but add to/remove from list is possible.
    [Export]
    public class EntityService
    {
    }
}
