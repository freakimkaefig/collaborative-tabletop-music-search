using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using MusicSearch.Managers;
using System.ComponentModel;
using Ctms.Applications.ViewModels;
using System.ComponentModel.Composition;
using MusicSearch.Objects;
using Ctms.Domain;
using Ctms.Applications.DataModels;
using Ctms.Applications.Data;
using Ctms.Applications.Common;
using Ctms.Applications.DataFactories;

namespace Ctms.Applications.Workers
{
    [Export]
    public class TagCombinationWorker
    {
        private BackgroundWorkHelper _backgroundWorker;
        private SearchViewModel _searchViewModel;
        private Repository _repository;
        private InfoWorker _infoWorker;
        private TagFactory _tagFactory;

        [ImportingConstructor]
        public TagCombinationWorker(SearchViewModel searchViewModel, Repository repository, InfoWorker infoWorker)
        {
            //ViewModels
            _searchViewModel = searchViewModel;
            _repository = repository;
            _infoWorker = infoWorker;
            _tagFactory = new TagFactory(repository);
            //Workers
            //Helpers
            _backgroundWorker = new BackgroundWorkHelper();
        }

        public void Initialize()
        {
        }

        public bool CanStartSearch() { return _searchViewModel.IsValid; }

        public void CheckTagPositions(int tagId)
        {
            return;

            // get moved tag and calculate position
            var movedTag        = _repository.GetTagDMById(tagId);

            // check combination
            foreach (var compareTag in _repository.GetAllVisibleTagDMs().Where(t => t.Id != movedTag.Id))
            {
                var xDistance   = movedTag.Tag.PositionX - compareTag.Tag.PositionX;
                var yDistance   = movedTag.Tag.PositionY - compareTag.Tag.PositionY;
                var distance    = Math.Sqrt(Math.Pow(xDistance, 2.0) + Math.Pow(yDistance, 2.0));

                //Console.WriteLine("xDistance: " + xDistance);
                //Console.WriteLine("yDistance: " + yDistance);
                //Console.WriteLine("distance: " + distance);
                //Console.WriteLine("distance: " + distance);

                if (distance < CommonVal.Tag_CombineCircleDiameter)
                {
                    var tagCombis = _repository.GetAllTagCombinations();
                    var combiWithMovedTag = tagCombis.FirstOrDefault(tc => tc.Tags.Contains(movedTag));

                    if (combiWithMovedTag == null)
                    {   // not combined right now
                        var tagCombi = _tagFactory.CreateTagCombination();



                        tagCombi.Tags.Add(movedTag);
                        tagCombi.Tags.Add(compareTag);

                        _repository.AddTagCombination(tagCombi);
                    }
                    else
                    {
                        // already combined with other tags
                        throw new NotImplementedException("Adding tag to existing combination");
                    }

                    compareTag.ConfirmCircleOpacity = 0.3F;
                    movedTag.ConfirmCircleOpacity = 0.3F;

                    //var tagCombination = new TagCombinationDataModel();
                    //_repository.AddTagCombination(tagCombination);
                    //Console.WriteLine("distance<");
                }
                else
                {
                    compareTag.ConfirmCircleOpacity = 0.0F;
                    movedTag.ConfirmCircleOpacity = 0.0F;

                    //var tagCombination = 
                    //_repository.RemoveTagCombina
                    //Console.WriteLine("distance>");
                }
            }
        }

    }
}
