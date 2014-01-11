using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.Managers;
using MusicSearch.SearchObjects;

namespace MusicSearch
{
    public class Start
    {
        public static void Main(String[] args)
        {
            var selectionManager = new SelectionManager();
            var type = new KeywordType();
            type.Type = KeywordType.Types.Artist;
            selectionManager.SetSelection(type.Type);
            var options = selectionManager.LoadNextOptions();


            type.Type = KeywordType.Types.Title;
            selectionManager.SetSelection(type.Type);
            options = selectionManager.LoadNextOptions();
        }
    }
}
