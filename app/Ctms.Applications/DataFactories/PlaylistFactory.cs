using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.Data;

namespace Ctms.Applications.DataFactories
{
    //Provides methods for CRUD-operations on the database-object playlist
    public class PlaylistFactory : BaseFactory
    {
        public PlaylistFactory(Repository repository)
            : base(repository)
        {
        }
        /*
        Examples:
        public Playlist Create()
        {
        }

        public void Delete()
        {
        }
        */
    }
}
