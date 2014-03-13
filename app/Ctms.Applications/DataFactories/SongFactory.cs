using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Domain.Objects;
using MusicSearch.ResponseObjects;
using Ctms.Applications.Data;

namespace Ctms.Applications.DataFactories
{
    //Provides methods for CRUD-operations on the database-object playlist
    public class SongFactory : BaseFactory
    {
        public SongFactory(Repository repository)
            : base(repository)
        {
        }
        
        public Song Create(ResponseContainer.ResponseObj.Song song)
        {
            Song newSong  = new Song();
            newSong.Title = song.Title;
            newSong.Artist = song.Artist_Name;

            return newSong;
        }

        public void Delete()
        {
        }
        
    }
}
