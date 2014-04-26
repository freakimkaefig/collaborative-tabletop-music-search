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
using System.Collections.ObjectModel;
using System.Windows;

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

        /// <summary>
        /// Check if tag is in combi distance to others.
        /// No tag can be in more than one tag combi, so check combinations with others
        /// </summary>
        /// <param name="movedTagId"></param>
        public void CheckMovedTagCombi(int movedTagId)
        {
            // get moved tag and calculate position
            var movedTag    = _repository.GetTagDMById(movedTagId);

            // break method if tag state is not assigend
            if (movedTag.State != TagDataModel.States.Assigned) return;

            var tagCombis   = _repository.GetAllTagCombinations();
            var compareTags = _repository.GetAddedTagDMs().Where(t => t.Id != movedTag.Id && t.State == TagDataModel.States.Assigned);

            // check combination
            foreach (var compareTag in compareTags)
            {   // calculate distance of tags (a² + b² = c²)
                var xDistance   = movedTag.Tag.PositionX - compareTag.Tag.PositionX;
                var yDistance   = movedTag.Tag.PositionY - compareTag.Tag.PositionY;
                var distance    = Math.Sqrt(Math.Pow(xDistance, 2.0) + Math.Pow(yDistance, 2.0));

                //LogDistanceCalc(movedTag, compareTag, xDistance, yDistance, distance);

                // get tag combination if there's one for this tag
                var combiWithMovedTag   = tagCombis.FirstOrDefault(tc => tc.Tags.Contains(movedTag));
                var combiWithCompareTag = tagCombis.FirstOrDefault(tc => tc.Tags.Contains(compareTag));

                if (distance < CommonVal.Tag_CombineCircleDiameter)
                {   // distance is inside combi radius

                    if (combiWithMovedTag == null)
                    {   // movedTag not combined with any tags right now

                        var possibleCombiType = GetPossibleCombiType(movedTag, compareTag, combiWithMovedTag, combiWithCompareTag);

                        if (possibleCombiType != KeywordTypes.None)
                        {   // movedTag and compareTag can be combined

                            if (combiWithCompareTag == null)
                            {   // no combi of movedTag or compareTag with any other tags right now -> create new                                
                                Console.WriteLine("CreateTagCombi");
                                combiWithMovedTag = CreateTagCombi(movedTag, compareTag, possibleCombiType);

                                // update calculation of center
                                UpdateCenter(combiWithMovedTag.Tags);
                            }
                            else
                            {   // a combi with the tag to compare is existing -> add
                                Console.WriteLine("Add tag to compareCombi");
                                combiWithCompareTag.Tags.Add(movedTag);

                                // update calculation of center
                                UpdateCenter(combiWithCompareTag.Tags);
                            }
                        }

                    }
                    else if (combiWithCompareTag == null)
                    {   // movedTag is combined, but compareTag not -> add compareTag to movedTag combie

                        var possibleCombiType = GetPossibleCombiType(movedTag, compareTag, combiWithMovedTag, combiWithCompareTag);
                        if (possibleCombiType != KeywordTypes.None)
                        {   // movedTag and compareTag can be combined
                            combiWithMovedTag.Tags.Add(movedTag);

                            // update calculation of center
                            UpdateCenter(combiWithMovedTag.Tags);
                        }
                    }
                }
                // distance is bigger than radius for combination
                else if (combiWithMovedTag != null) 
                {   // tag is in a combi -> remove from combi

                    if (combiWithMovedTag.Tags.Count <= 2)
                    {
                        // remove combi from repository
                        _repository.RemoveTagCombination(combiWithMovedTag);

                        //!! just for testing
                        compareTag.ConfirmCircleOpacity = 0.0F;
                        movedTag.ConfirmCircleOpacity = 0.0F;
                    }
                    else
                    {
                        combiWithMovedTag.Tags.Remove(movedTag);
                    }

                    // update calculation of center
                    UpdateCenter(combiWithMovedTag.Tags);
                }
            }
        }

        /// <summary>
        /// Calculate which combinations are allowed.
        /// Possible:
        ///     5 genres with * attributes
        ///     1 artist with * attributes
        /// The base type of a combination is either genre or artist.
        /// </summary>
        /// <param name="movedTagDm"></param>
        /// <param name="compareTagDm"></param>
        /// <param name="movedCombi"></param>
        /// <param name="compareCombi"></param>
        /// <returns>Possible combination type (KeywordType)</returns>
        public KeywordTypes GetPossibleCombiType(
            TagDataModel movedTagDm, 
            TagDataModel compareTagDm, 
            TagCombinationDataModel movedCombi, 
            TagCombinationDataModel compareCombi)
        {
            var movedTagType   = movedTagDm.Tag.AssignedKeyword.Type;
            var compareTagType = compareTagDm.Tag.AssignedKeyword.Type;

            var resultType1 = GetPossibleCombiTypeFromOneSide(movedCombi, movedTagType, compareTagType);
            var resultType2 = GetPossibleCombiTypeFromOneSide(compareCombi, compareTagType, movedTagType);

            if (resultType1 == KeywordTypes.None)
            {
                return resultType2;
            }
            else
            {
                return resultType1;
            }
        }

        private static KeywordTypes GetPossibleCombiTypeFromOneSide(TagCombinationDataModel combi1, KeywordTypes combi1TagType, KeywordTypes tag2Type)
        {
            if (combi1 != null)
            {   // existing movedCombi

                if (combi1.CombinationType == KeywordTypes.Genre
                    && (tag2Type == KeywordTypes.Genre || tag2Type == KeywordTypes.Attribute)
                    && combi1.Tags.Where(t => t.Tag.AssignedKeyword.Type == KeywordTypes.Genre).Count() <= 5)
                {   // existing genre combi can be combined with genre (if not more than 5) or attribute
                    // base type is genre
                    return KeywordTypes.Genre;
                }
                else if (combi1.CombinationType == KeywordTypes.Artist
                    && tag2Type == KeywordTypes.Attribute)
                {   // existing artist combi can be combined with attribute 
                    //(not artist because there's already an artist in a combi of the type artist)
                    return KeywordTypes.Artist;
                }
                else
                {
                    return KeywordTypes.None;
                }
            }
            // there's no combi right now
            else if ((combi1TagType == KeywordTypes.Genre && (tag2Type == KeywordTypes.Genre || tag2Type == KeywordTypes.Attribute))
                || (combi1TagType == KeywordTypes.Attribute && (tag2Type == KeywordTypes.Genre)))
            {   // combine genre with genre or attribute
                return KeywordTypes.Genre;
            }
            else if ((combi1TagType == KeywordTypes.Artist && tag2Type == KeywordTypes.Attribute)
                || combi1TagType == KeywordTypes.Attribute && tag2Type == KeywordTypes.Artist)
            {
                return KeywordTypes.Artist;
            }
            else
            {
                return KeywordTypes.None;
            }
        }

        private static void LogDistanceCalc(TagDataModel movedTag, TagDataModel compareTag, int xDistance, int yDistance, double distance)
        {
            Console.WriteLine("movedTag.Tag.PositionX: " + movedTag.Tag.PositionX);
            Console.WriteLine("movedTag.Tag.PositionY: " + movedTag.Tag.PositionY);
            Console.WriteLine("compareTag.Tag.PositionX: " + compareTag.Tag.PositionX);
            Console.WriteLine("compareTag.Tag.PositionY: " + compareTag.Tag.PositionY);
            Console.WriteLine("yDistance: " + yDistance);
            Console.WriteLine("xDistance: " + xDistance);
            Console.WriteLine("distance: " + distance);
            Console.WriteLine("distance: " + distance);
        }

        private TagCombinationDataModel CreateTagCombi(TagDataModel movedTag, TagDataModel compareTag, KeywordTypes type)
        {
            var tagCombi = _tagFactory.CreateTagCombination(type);

            // add tags to empty combination
            tagCombi.Tags.Add(movedTag);
            tagCombi.Tags.Add(compareTag);

            // add combi to repository
            _repository.AddTagCombination(tagCombi);

            //!! just for testing
            compareTag.ConfirmCircleOpacity = 0.3F;
            movedTag.ConfirmCircleOpacity = 0.3F;

            return tagCombi;
        }

        public Point UpdateCenter(ObservableCollection<TagDataModel> tags)
        {
            //throw new NotImplementedException();
            // area = 1/2 * (sum of [(x1 * y2 - x2 * y1) + (x2 * y3 - x3 * y2)...])

            var area        = 0.0F;
            var centroidX   = 0.0F;
            var centroidY   = 0.0F;
            byte i          = 0;
            var crossDiff   = 0.0F;

            // calculate interim steps for all tag positions
            for (i = 0; i < tags.Count-1; i++)
            {   
                crossDiff   = tags[i].Tag.PositionX * tags[i+1].Tag.PositionY - tags[i+1].Tag.PositionX * tags[i].Tag.PositionY;
                area        += crossDiff;
                centroidX   += (tags[i].Tag.PositionX + tags[i + 1].Tag.PositionY) * crossDiff;
                centroidY   += (tags[i].Tag.PositionY + tags[i + 1].Tag.PositionX) * crossDiff;
            }
            if (area != 0.0)
            {
                centroidX = (1 / (3 * area)) * centroidX;
                centroidY = (1 / (3 * area)) * centroidY;

                return new Point(centroidX, centroidY);
            }
            else
            {   // area is 0. all tags are positioned in one line.

            }
            return new Point(0, 0);
            // centroid = (1/(6*area)) * (sum of [(x1 + 2x) * (x1 * y 2 - x2 * y1)...])
            //var centroidX = (1/(6*area)) * 
        }

    }
}
