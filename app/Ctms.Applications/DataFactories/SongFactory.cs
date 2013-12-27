using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Domain.Objects;
using MusicSearch.ResponseObjects;

namespace Ctms.Applications.DataFactories
{
    //Provides methods for CRUD-operations on the database-object playlist
    public class SongFactory : BaseFactory
    {
        public SongFactory()
            : base()
        {
        }
        
        public Song Create(ResponseContainer.Response.Song song)
        {
            Song newSong  = new Song();
            newSong.Title = song.title;

            return newSong;
        }

        public void Delete()
        {
        }
        
    }
}
