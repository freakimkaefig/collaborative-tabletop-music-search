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

                                SetCenter(combiWithMovedTag);

                                _repository.AddTagCombination(combiWithMovedTag);

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

        private void SetCenter(TagCombinationDataModel combiWithMovedTag)
        {
            // update calculation of center
            var centerPoint = UpdateCenter(combiWithMovedTag.Tags);
            combiWithMovedTag.CenterX = centerPoint.X;
            combiWithMovedTag.CenterY = centerPoint.Y;
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


        /// <summary>
        /// Calculate centroid of the polygon of tags. Optimized for performance
        /// </summary>
        /// <param name="tags">All tags that form the polygon</param>
        /// <returns>The centroid as point</returns>
        public Point UpdateCenter(ObservableCollection<TagDataModel> tags)
        {
            var area = 0.0F;
            var sumX = 0.0F;
            var sumY = 0.0F;
            var crossDiff = 0.0F;
            byte i = 0;
            short x0;
            short y0;
            short x1;
            short y1;
            
            // calculate interim steps for all tag positions            
            for (i = 0; i < tags.Count-1; i++)
            {
                x0 = tags[i].Tag.PositionX;
                y0 = tags[i].Tag.PositionY;
                x1 = tags[i + 1].Tag.PositionX;
                y1 = tags[i + 1].Tag.PositionY;

                crossDiff   = x0 * y1 - x1 * y0;
                area        += crossDiff;
                sumX   += (x0 + x1) * crossDiff;
                sumY   += (y0 + y1) * crossDiff;
            }

            // calc last connection
            x0 = tags[tags.Count - 1].Tag.PositionX;
            y0 = tags[tags.Count - 1].Tag.PositionY;
            x1 = tags[0].Tag.PositionX;
            y1 = tags[0].Tag.PositionY;
            
            crossDiff = x0 * y1 - x1 * y0;
            area += crossDiff;
            sumX += (x0 + x1) * crossDiff;
            sumY += (y0 + y1) * crossDiff;

            if (area != 0.0)
            {   // calculate center x and y and return it
                return new Point((1 / (3.0F * area)) * sumX, (1 / (3.0F * area)) * sumY);
            }
            else
            {   // area is 0. all tags are positioned in one line. 
                // this case can be ignored at first because the probability is very low and
                // such a order won't stay for a long time.
                return new Point(sumX / 6, sumY / 6);
            } 
        }

    }
}
