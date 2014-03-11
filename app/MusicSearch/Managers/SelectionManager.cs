using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.SearchObjects;

namespace MusicSearch.Managers
{
    public class SelectionManager
    {
        //public List<SearchOption> NextOptions { get; set; }
        //public List<List<SearchOption>> LastOptions { get; set; }

        public KeywordContainer SelectOptions;
        private KeywordType.Types mSelectOption;

        public SelectionManager()
        {
            Reset();

            //Test
            //SetSelection(KeywordType.Types.Artist);

            SelectOptions = new KeywordContainer()
            {
                Container = new Dictionary<KeywordType.Types, List<Keyword>>()
                {
                    {
                        KeywordType.Types.Artist, 
                        new List<Keyword>()
                        {
                            new Artist("Katy Perry"),
                            new Artist("Joss Stone"),
                            new Artist("Hansi Hinterseer"),
                            new Artist("Gregory Porter"),
                            new Artist("Rakede")
                        }
                    },
                    {
                        KeywordType.Types.Title, 
                        new List<Keyword>()
                        {
                            new Title("Fireworks"),
                            new Title("Bangarang"),
                            new Title("Alle meine Entchen"),
                            new Title("Show me how to live"),
                            new Title("Angels")
                        }
                    }
                }
            };
        }

        public void SetSelection(KeywordType.Types selectOption)
        {
            mSelectOption = selectOption;
        }
        /*
        public List<SearchOption> LoadNextOptions()
        {
            //Clear all old options from the last selection
            NextOptions.Clear();

            //
            var optionContainer = SelectOptions.Container[mSelectOption];
            foreach (var entry in optionContainer)
            {
                SearchOption newOption = new SearchOption();
                if (entry.Type == KeywordType.Types.Artist)
                {
                    newOption = new Keyword.Artist(entry.Name);
                    NextOptions.Add(newOption);
                }
                if (entry.Type == KeywordType.Types.Title)
                {
                    newOption = new Keyword.Title(entry.Name);
                    NextOptions.Add(newOption);
                }
            }

            return NextOptions;
        }

        public void Confirm()
        {

        }

        public List<SearchOption> ReadBreadcrumb()
        {
            throw new NotImplementedException();
        }
        */
        public void ReturnToLast()
        {

        }

        public void Reset()
        {
            //NextOptions = new List<SearchOption>();
            //LastOptions = new List<List<SearchOption>>();
        }
    }
}
