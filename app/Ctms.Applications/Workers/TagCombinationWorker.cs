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
using Ctms.Applications.DevHelper;
using System.Collections.ObjectModel;
using System.Windows;
using Ctms.Domain.Objects;

namespace Ctms.Applications.Workers
{
    [Export]
    public class TagCombinationWorker
    {
        private BackgroundWorkHelper _backgroundWorker;
        private SearchViewModel _searchVm;
        private Repository _repository;
        private InfoWorker _infoWorker;
        private TagFactory _tagFactory;

        [ImportingConstructor]
        public TagCombinationWorker(SearchViewModel searchViewModel, Repository repository, InfoWorker infoWorker)
        {
            //ViewModels
            _searchVm = searchViewModel;
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

        public bool CanStartSearch() { return _searchVm.IsValid; }

        public void RemoveTagFromCombi(int removeTagId)
        {
            // get moved tag and calculate position
            var removeTag = _repository.GetTagDMById(removeTagId);
            var tagCombi = _repository.GetTagCombiWithTag(removeTag);
            
            if (tagCombi != null)
            {
                tagCombi.Tags.Remove(removeTag);

                var remainingTag = tagCombi.Tags.FirstOrDefault();
                if (remainingTag != null)
                {
                    // calculate if remaining tags can still be combined
                    CheckCombisForTag(remainingTag.Id);
                }
            }
        }

        /// <summary>
        /// Check if tag is in combi distance to others.
        /// No tag can be in more than one tag combi, so check combinations with others
        /// </summary>
        /// <param name="myTagId"></param>
        public void CheckCombisForTag(int myTagId)
        {
            // get moved tag and calculate position
            var myTag    = _repository.GetTagDMById(myTagId);

            // break method if tag state is not assigend
            if (myTag.AssignState == TagDataModel.AssignStates.Editing || myTag.ExistenceState == TagDataModel.ExistenceStates.Removed)
            {
                var combi = _repository.GetTagCombiWithTag(myTag);
                if (combi != null && combi.Tags.Count >= 3)
                {
                    combi.Tags.Remove(myTag);
                    CheckCombisForTag(combi.Tags.FirstOrDefault().Id);
                    return;
                }
                else if (combi != null && combi.Tags.Count == 2)
                {
                    _repository.RemoveTagCombination(combi);
                    return;
                }
            }

            var tagCombis   = _repository.GetTagCombinations();
            var compareTags = _repository.GetAddedAndAssignedTagDMs().Where(t => t.Id != myTag.Id);

            //!!remove
            //var time = DateTime.Now.Minute + " " + DateTime.Now.Second + " " + DateTime.Now.Millisecond;

            // check combination
            foreach (var compareTag in compareTags)
            {   // calculate distance of tags (a² + b² = c²)
                var xDistance   = myTag.Tag.PositionX - compareTag.Tag.PositionX;
                var yDistance   = myTag.Tag.PositionY - compareTag.Tag.PositionY;
                var distance    = Math.Sqrt(Math.Pow(xDistance, 2.0) + Math.Pow(yDistance, 2.0));

                //LogDistanceCalc(movedTag, compareTag, xDistance, yDistance, distance);

                // get tag combination if there's one for this tag
                var combiWithMyTag   = tagCombis.FirstOrDefault(tc => tc.Tags.Contains(myTag));
                var combiWithCompareTag = tagCombis.FirstOrDefault(tc => tc.Tags.Contains(compareTag));

                if (distance < CommonVal.Tag_CombineCircleDiameter)
                {   // distance is inside combi radius
                    Log("distance < CommonVal.Tag_CombineCircleDiameter ");
                    if (combiWithMyTag == null)
                    {   // movedTag not combined with any tags right now
                        Log("combiWithMovedTag == null");
                        var possibleCombiType = GetPossibleCombiType(myTag, compareTag, combiWithMyTag, combiWithCompareTag);

                        if (possibleCombiType != CombinationTypes.None)
                        {   // movedTag and compareTag can be combined
                            Log("possibleCombiType != KeywordTypes.None ");
                            if (combiWithCompareTag == null)
                            {   // no combi of movedTag or compareTag with any other tags right now -> create new 
                                Log("combiWithCompareTag == null > create new ");

                                combiWithMyTag = CreateTagCombi(myTag, compareTag, possibleCombiType);
                                
                                // update calculation of center
                                UpdateCenter(combiWithMyTag);

                                _repository.AddTagCombination(combiWithMyTag);

                                UpdateRadiusCircle(myTag, compareTag, true);

                                //Log("count of combis: " + _searchViewModel.TagCombinations.Count);
                                //Log("count of tags in combis: " 
                                //    + _searchViewModel.TagCombinations.SelectMany(t => t.Tags).Count());

                                /*
                                Log("combiWithMovedTag.CenterX: " + combiWithMovedTag.CenterX);
                                Log("combiWithMovedTag.CenterY: " + combiWithMovedTag.CenterY);
                                Log("combiWithMovedTag.Tags[0].Tag.PositionX: " 
                                    + combiWithMovedTag.Tags[0].Tag.PositionX);
                                Log("combiWithMovedTag.Tags[0].Tag.PositionY: " 
                                    + combiWithMovedTag.Tags[0].Tag.PositionY);
                                Log("combiWithMovedTag.Tags[1].Tag.PositionX: "
                                    + combiWithMovedTag.Tags[1].Tag.PositionX);
                                Log("combiWithMovedTag.Tags[1].Tag.PositionY: "
                                    + combiWithMovedTag.Tags[1].Tag.PositionY);*/
                            }
                            else
                            {   // a combi with the tag to compare is existing -> add
                                Log("combiWithCompareTag != null -> Add to compare combi ");
                                combiWithCompareTag.Tags.Add(myTag);

                                // update calculation of center
                                UpdateCenter(combiWithCompareTag);

                                UpdateRadiusCircle(myTag, compareTag, true);
                            }
                        }
                    }
                    else if (combiWithCompareTag == null)
                    {   // movedTag is combined, but compareTag not -> add compareTag to movedTag combie
                        Log("combiWithCompareTag == null ");
                        var possibleCombiType = GetPossibleCombiType(myTag, compareTag, combiWithMyTag, combiWithCompareTag);
                        if (possibleCombiType != CombinationTypes.None)
                        {   // movedTag and compareTag can be combined
                            combiWithMyTag.Tags.Add(myTag);//!!StackOverflowException tritt hier ab und zu auf (z.b. 5 genres + bewegung)
                            Log("possibleCombiType != KeywordTypes.None > add ");

                            // update calculation of center
                            UpdateCenter(combiWithMyTag);

                            UpdateRadiusCircle(myTag, compareTag, true);
                        }
                    }
                    else if (combiWithMyTag != null)
                    {
                        Log("combiWithMovedTag != null > update center ");
                        UpdateCenter(combiWithMyTag);
                    }
                    else if (combiWithCompareTag != null)
                    {
                        Log("combiWithCompareTag != null > update center ");
                        UpdateCenter(combiWithCompareTag);
                    }
                }
                // distance is bigger than radius for combination
                else if (combiWithMyTag != null)
                {   // tag is in a combi -> remove from combi
                    Log("combiWithMovedTag != null > update center ");

                    if (combiWithMyTag.Tags.Count < 2)
                    {
                        Log("combiWithMovedTag.Tags.Count < 2 > update center ");
                        // remove combi from repository
                        _repository.RemoveTagCombination(combiWithMyTag);

                        UpdateRadiusCircle(myTag, compareTag, false);

                        // update calculation of center
                        UpdateCenter(combiWithMyTag);
                    }
                    else if (combiWithMyTag.Tags.Count >= 3)
                    {
                        Log("combiWithMovedTag.Tags.Count >= 3 > remove from combiWithMovedtag ");
                        combiWithMyTag.Tags.Remove(myTag);

                        UpdateRadiusCircle(myTag, null, false);

                        CheckCombisForTag(combiWithMyTag.Tags.FirstOrDefault().Id);
                    }
                }
            }
        }

        private static void UpdateRadiusCircle(TagDataModel myTag, TagDataModel compareTag, bool isHighlighted)
        {
            return;// removed behaviour
            var opacity = isHighlighted == true ? 1.0F : 0.0F;
            if (compareTag != null) compareTag.ConfirmCircleOpacity = opacity;
            if (myTag != null) myTag.ConfirmCircleOpacity = opacity;
        }


        private static void Log(string message)
        {
            //DevLogger.Log(message);
        }

        public Point UpdateCenter(TagCombinationDataModel combi)
        {
            // update calculation of center
            var centerPoint = CalculateCenter(combi);
            combi.CenterX = centerPoint.X;
            combi.CenterY = centerPoint.Y;
            
            //_searchVm.RaisePropertyChangedManually("TagCombinations");

            _searchVm.UpdateStoryboard(combi.Id);//!!
            //Log("UpdateCenter, centerX: " + combi.CenterX + ", centerY: " + combi.CenterY);

            return centerPoint;
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
        public CombinationTypes GetPossibleCombiType(
            TagDataModel movedTagDm, 
            TagDataModel compareTagDm, 
            TagCombinationDataModel movedCombi, 
            TagCombinationDataModel compareCombi)
        {
            var movedTagType        = movedTagDm.Tag.AssignedKeyword.KeywordType;
            var compareTagType      = compareTagDm.Tag.AssignedKeyword.KeywordType;
            var movedAttributeType  = movedTagDm.Tag.AssignedKeyword.AttributeType;
            var compareAttributeType = compareTagDm.Tag.AssignedKeyword.AttributeType;

            var resultType1 = GetPossibleCombiTypeFromOneSide(movedCombi, movedTagType, movedAttributeType, compareTagType, compareAttributeType);
            var resultType2 = GetPossibleCombiTypeFromOneSide(compareCombi, compareTagType, compareAttributeType, movedTagType, movedAttributeType);

            if (resultType1 == CombinationTypes.None)
            {
                return resultType2;
            }
            else
            {
                return resultType1;
            }
        }

        //private static CombinationTypes GetPossibleCombiTypeFromOneSide(TagCombinationDataModel combi, KeywordTypes combiTagType, KeywordTypes singleTagType)
        private static CombinationTypes GetPossibleCombiTypeFromOneSide(
            TagCombinationDataModel combi, KeywordTypes combiTagType, AttributeTypes combiAttributeType, KeywordTypes singleTagType, AttributeTypes singleAttributeType)
        {
            if (combi != null)
            {   // existing movedCombi

                if (combi.CombinationType == CombinationTypes.Genre
                    && (singleTagType == KeywordTypes.Genre || (singleTagType == KeywordTypes.Attribute && singleAttributeType == AttributeTypes.Genre))
                    && combi.Tags.Where(t => t.Tag.AssignedKeyword.KeywordType == KeywordTypes.Genre).Count() <= 5)
                {   // existing genre combi can be combined with genre (if not more than 5) or attribute
                    // base type is genre
                    return CombinationTypes.Genre;
                }
                else if (combi.CombinationType == CombinationTypes.Artist
                    && singleTagType == KeywordTypes.Attribute && singleAttributeType == AttributeTypes.Artist)
                {   // existing artist combi can be combined with attribute 
                    //(not artist because there's already an artist in a combi of the type artist)
                    return CombinationTypes.Artist;
                }
                else
                {
                    return CombinationTypes.None;
                }
            }
            // there's no combi right now
            else if (   combiTagType == KeywordTypes.Genre 
                            && (singleTagType == KeywordTypes.Genre 
                                || (singleTagType == KeywordTypes.Attribute && singleAttributeType == AttributeTypes.Genre))
                        || ((combiTagType == KeywordTypes.Attribute && combiAttributeType == AttributeTypes.Genre) && (singleTagType == KeywordTypes.Genre)))
            {   // combine genre+genre or genre+attribute(of type genre)
                return CombinationTypes.Genre;
            }
            else if ((combiTagType == KeywordTypes.Artist 
                    && (singleTagType == KeywordTypes.Attribute && singleAttributeType == AttributeTypes.Artist))
                    || (combiTagType == KeywordTypes.Attribute && combiAttributeType == AttributeTypes.Artist) && singleTagType == KeywordTypes.Artist)
            {
                return CombinationTypes.Artist;
            }
            else
            {
                return CombinationTypes.None;
            }
        }

        private static void LogDistanceCalc(TagDataModel movedTag, TagDataModel compareTag, int xDistance, int yDistance, double distance)
        {
            Log("movedTag.Tag.PositionX: " + movedTag.Tag.PositionX);
            Log("movedTag.Tag.PositionY: " + movedTag.Tag.PositionY);
            Log("compareTag.Tag.PositionX: " + compareTag.Tag.PositionX);
            Log("compareTag.Tag.PositionY: " + compareTag.Tag.PositionY);
            Log("yDistance: " + yDistance);
            Log("xDistance: " + xDistance);
            Log("distance: " + distance);
            Log("distance: " + distance);
        }

        private TagCombinationDataModel CreateTagCombi(TagDataModel movedTag, TagDataModel compareTag, CombinationTypes type)
        {
            var tagCombi = _tagFactory.CreateTagCombination(type);

            // add tags to empty combination
            tagCombi.Tags.Add(movedTag);
            tagCombi.Tags.Add(compareTag);

            //!! just for testing
            //compareTag.ConfirmCircleOpacity = 0.3F;
            //movedTag.ConfirmCircleOpacity = 0.3F;

            return tagCombi;
        }


        /// <summary>
        /// Calculate centroid of the polygon of tags. Optimized for performance
        /// </summary>
        /// <param name="tags">All tags that form the polygon</param>
        /// <returns>The centroid as point</returns>
        public Point CalculateCenter(TagCombinationDataModel tagCombi)
        {
            var tags = tagCombi.Tags;

            if (tags.Count > 2)
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
                for (i = 0; i < tags.Count - 1; i++)
                {
                    x0 = tags[i].Tag.PositionX;
                    y0 = tags[i].Tag.PositionY;
                    x1 = tags[i + 1].Tag.PositionX;
                    y1 = tags[i + 1].Tag.PositionY;

                    crossDiff = x0 * y1 - x1 * y0;
                    area += crossDiff;
                    sumX += (x0 + x1) * crossDiff;
                    sumY += (y0 + y1) * crossDiff;
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
                {
                    // area is 0. all tags are positioned in one line. 
                    // this case can be ignored at first because the probability is very low and
                    // such a order won't stay for a long time. Just use the last known center
                    return new Point(tagCombi.CenterX, tagCombi.CenterY);
                }
            }
            else if(tags.Count == 2)
            {  // calculate center of two tangibles: x = (x1+x2)/2                
                var xSum = tags[0].Tag.PositionX + tags[1].Tag.PositionX;
                var ySum = tags[0].Tag.PositionY + tags[1].Tag.PositionY;

                return new Point(xSum / (float) tags.Count, ySum / (float) tags.Count);
            }
            else
            {   // 0 or 1 tag in combi. Not valid for this method.
                throw new Exception("This method needs 2 or more tags to calculate the center");
            }
        }

    }
}
