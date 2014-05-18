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

        /// <summary>
        /// Check if tag is in radius for combination with tags.
        /// If yes create a new combi or if the compare tag is already in another combi add myTag to that.
        /// No tag can be in more than one tag combi, so check combinations with others.
        /// </summary>
        /// <param name="myTagId"></param>
        public void CheckCombisForTag(int myTagId)
        {
            Log("CheckCombisForTag: " + myTagId);

            // get moved tag and calculate position
            var myTag = _repository.GetTagDMById(myTagId);

            // remove myTag from any combi if myTag is in edit or removed state
            if (myTag.AssignState == TagDataModel.AssignStates.Editing || myTag.ExistenceState == TagDataModel.ExistenceStates.Removed)
            {
                var combi = _repository.GetTagCombiWithTag(myTag);
                if (combi != null && combi.Tags.Count >= 3)
                {
                    Log("Editing/Removed, >= 3");
                    // remove tag from combi because there are more than 1 tags left
                    combi.Tags.Remove(myTag);
                    // check if remaining tags may still be in a combi
                    CheckCombisForTag(combi.Tags.FirstOrDefault().Id); 
                    
                    if (combi.Tags.Count >= 2)
                        UpdateCenter(combi);
                    return;
                }
                else if (combi != null && combi.Tags.Count == 2)
                {
                    Log("Editing/Removed, == 2");
                    // delete complete combi because there is just one tag left
                    _repository.RemoveTagCombination(combi);

                    return;
                }
            }

            var compareTags = _repository.GetAddedAndAssignedTagsWithout(myTag.Id);
            int xDistance;
            int yDistance;
            double distance;
            TagCombinationDataModel combiWithMyTag;
            TagCombinationDataModel combiWithCompareTag;
            var tagCombis = _repository.GetTagCombinations();

            // check combination
            foreach (var compareTag in compareTags)
            {
                Log("for myTag " + myTag.Id + ", compareTag " + compareTag.Id);
                // get tag combination if there's one for this tag
                combiWithMyTag = _repository.GetTagCombiWithTag(myTag);
                combiWithCompareTag = _repository.GetTagCombiWithTag(compareTag);

                //Log("combiWithMyTag: " + combiWithMyTag);
                //Log("combiWithCompareTag: " + combiWithCompareTag);

                if (combiWithMyTag == null && combiWithCompareTag == null)
                {   // tags are not in a combi right now
                    Log("combiWithMyTag == null && combiWithCompareTag == null");
                    CalculateTagToTagDistance(myTag, compareTag, out xDistance, out yDistance, out distance);

                    if (distance < CommonVal.Tag_CombineCircleDiameter)
                    {   // tags are in combi radius
                        Log("distance < CommonVal.Tag_CombineCircleDiameter");
                        // check if combi is possible
                        var possibleCombiType = GetPossibleCombiType(myTag, compareTag, combiWithMyTag, combiWithCompareTag);

                        if (possibleCombiType != CombinationTypes.None)
                        {
                            combiWithMyTag = CreateTagCombi(myTag, compareTag, possibleCombiType);

                            _repository.AddTagCombination(combiWithMyTag);
                            Log("possibleCombiType != CombinationTypes.None");
                            break;
                        }
                    }
                }
                else if (combiWithMyTag == null && combiWithCompareTag != null)
                {
                    Log("combiWithMyTag == null && combiWithCompareTag != null");
                    
                    // check if myTag can be combined with compareCombi
                    CalculateTagToCombiCenterDistance(myTag, combiWithCompareTag, out xDistance, out yDistance, out distance);

                    if (distance < CommonVal.Tag_CombineCircleDiameter)
                    {
                        Log("distance < CommonVal.Tag_CombineCircleDiameter");
                        // check if combi is possible
                        var possibleCombiType = GetPossibleCombiType(myTag, compareTag, combiWithMyTag, combiWithCompareTag);

                        if (possibleCombiType != CombinationTypes.None)
                        {
                            combiWithCompareTag.Tags.Add(myTag);
                            Log("possibleCombiType != CombinationTypes.None");
                            break;
                        }
                    }
                    else
                    {
                        //combiWithCompareTag.Tags.Remove(myTag);
                        //Log("else");
                        //break;
                    }
                }
                else if (combiWithMyTag != null && combiWithCompareTag == null)
                {
                    Log("combiWithMyTag != null && combiWithCompareTag == null");

                    CalculateTagToCombiCenterDistance(compareTag, combiWithMyTag, out xDistance, out yDistance, out distance);

                    if (distance < CommonVal.Tag_CombineCircleDiameter)
                    {
                        Log("distance < CommonVal.Tag_CombineCircleDiameter");
                        combiWithMyTag.Tags.Add(compareTag);
                        break;
                    }
                    else
                    {
                        Log("else");
                        combiWithMyTag.Tags.Remove(myTag);
                        break;
                    }
                }
                else if (combiWithMyTag != null && combiWithMyTag == combiWithCompareTag)
                {   // the tags are in the same combi

                    Log("combiWithMyTag != null && combiWithMyTag == combiWithCompareTag");

                    // check if myTag is too far from its combi center
                    if (combiWithMyTag.Tags.Count == 2)
                    {   // just 2 tags, so calculate direct distance
                        CalculateTagToTagDistance(myTag, compareTag, out xDistance, out yDistance, out distance);
                        Log("combiWithMyTag.Tags.Count == 2");
                    }
                    else
                    {   // more than 2 tags, so calculate distance to combi center
                        CalculateTagToCombiCenterDistance(myTag, combiWithMyTag, out xDistance, out yDistance, out distance);
                        Log("else");
                    }

                    if (distance > CommonVal.Tag_CombineCircleDiameter)
                    {
                        Log("distance > CommonVal.Tag_CombineCircleDiameter");
                        if (combiWithMyTag.Tags.Count == 2)
                        {
                            Log("combiWithMyTag.Tags.Count == 2");
                            // only those 2 tags in this combi. delete it.
                            _repository.RemoveTagCombination(combiWithMyTag);
                            break;
                        }
                        else
                        {
                            Log("else");
                            // more than 2 tags in this  combi. remove myTag.
                            combiWithMyTag.Tags.Remove(myTag);
                            break;
                        }
                    }
                    
                }
                else
                {
                    // if both tags are already in different combinations don't do anything. 
                    // makes the algorithm much easier and is better usable
                }
            }

            // update center of all tag combis
            foreach (var combi in _repository.GetTagCombinations())
            {
                if (combi.Tags.Count >= 2)
                    UpdateCenter(combi);
            }
        }

        #region Combinability

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


        /// <summary>
        /// Calculate if combination is possible from the view of one tag(combi)
        /// </summary>
        /// <param name="combi">The combi of side 1</param>
        /// <param name="combiTagType">The type of combi's tag of side 1</param>
        /// <param name="combiAttributeType">The attribute type of combi 1</param>
        /// <param name="singleTagType">The type of tag of side2</param>
        /// <param name="singleAttributeType">The attributeType of tag of side2</param>
        /// <returns></returns>
        private static CombinationTypes GetPossibleCombiTypeFromOneSide(
            TagCombinationDataModel combi, KeywordTypes combiTagType, AttributeTypes combiAttributeType, KeywordTypes singleTagType, AttributeTypes singleAttributeType)
        {
            if (combi != null)
            {   // existing combi

                if (combi.CombinationType == CombinationTypes.Genre
                    && (singleTagType == KeywordTypes.Genre || (singleTagType == KeywordTypes.Attribute && singleAttributeType == AttributeTypes.Genre))
                    && combi.Tags.Where(t => t.Tag.AssignedKeyword.KeywordType == KeywordTypes.Genre).Count() < 5)
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
                {   // no combi possible
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
            {   // no combi possible
                return CombinationTypes.None;
            }
        }

        #endregion Combinability


        #region CRUD combis

        private TagCombinationDataModel CreateTagCombi(TagDataModel movedTag, TagDataModel compareTag, CombinationTypes type)
        {
            var tagCombi = _tagFactory.CreateTagCombination(type);

            // add tags to empty combination
            tagCombi.Tags.Add(movedTag);
            tagCombi.Tags.Add(compareTag);

            return tagCombi;
        }


        /// <summary>
        /// Remove a tag from its combination
        /// </summary>
        /// <param name="removeTagId">Id of the tag to remove</param>
        public void RemoveTagFromCombi(int removeTagId)
        {
            // get moved tag and calculate position
            var removeTag = _repository.GetTagDMById(removeTagId);
            var tagCombi = _repository.GetTagCombiWithTag(removeTag);

            if (tagCombi != null)
            {
                if (tagCombi.Tags.Count > 2)
                {
                    tagCombi.Tags.Remove(removeTag);
                }
                else
                {
                    _repository.RemoveTagCombination(tagCombi);
                }
            }
        }

        #endregion CRUD combis


        #region Distance calculation

        /// <summary>
        /// Calculate distance between tags. Use primitive variables with out to improve performance
        /// </summary>
        private static void CalculateTagToTagDistance(TagDataModel myTag, TagDataModel compareTag, out int xDistance, out int yDistance, out double distance)
        {
            // calculate distance of tags by pythagoras formula (a² + b² = c²)
            xDistance = myTag.Tag.PositionX - compareTag.Tag.PositionX;
            yDistance = myTag.Tag.PositionY - compareTag.Tag.PositionY;
            distance = Math.Sqrt(Math.Pow(xDistance, 2.0) + Math.Pow(yDistance, 2.0));
        }


        /// <summary>
        /// Calculate distance between a tag and center of compareCombi. Use primitive variables with out to improve performance
        /// </summary>
        private static void CalculateTagToCombiCenterDistance(TagDataModel myTag, TagCombinationDataModel compareCombi, out int xDistance, out int yDistance, out double distance)
        {
            // calculate distance of tags by pythagoras formula (a² + b² = c²)
            xDistance = (int)(myTag.Tag.PositionX - compareCombi.CenterX);
            yDistance = (int)(myTag.Tag.PositionY - compareCombi.CenterY);
            distance = Math.Sqrt(Math.Pow(xDistance, 2.0) + Math.Pow(yDistance, 2.0));
        }

        #endregion Distance calculation


        #region Center calculation

        /// <summary>
        /// Update the center point of a tag combination
        /// </summary>
        /// <param name="combi">The combi which contains the tags between the center shall be calculated</param>
        /// <returns></returns>

        public Point UpdateCenter(TagCombinationDataModel combi)
        {
            // update calculation of center
            var centerPoint = GetCenterOfPolygon(combi);
            combi.CenterX = centerPoint.X;
            combi.CenterY = centerPoint.Y;

            return centerPoint;
        }

        /// <summary>
        /// Calculate centroid of the polygon of tags. Optimized for performance
        /// </summary>
        /// <param name="tags">All tags that form the polygon</param>
        /// <returns>The centroid as point</returns>
        
        public Point GetCenterOfPolygon(TagCombinationDataModel combi)
        {
            var xSum = 0;
            var ySum = 0;

            foreach (var tag in combi.Tags)
            {
                xSum += tag.Tag.PositionX;
                ySum += tag.Tag.PositionY;
            }

            return new Point(xSum / combi.Tags.Count, ySum / combi.Tags.Count);
        }

        #endregion Center calculation

        #region Logging

        private static void Log(string message)
        {
            DevLogger.Log(message);
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

        #endregion Logging


        #region Unused methods

        /// <summary>
        /// Calculate centroid of the polygon of tags. Optimized for performance
        /// </summary>
        /// <param name="tags">All tags that form the polygon</param>
        /// <returns>The centroid as point</returns>
        public Point CalculateCentroid(TagCombinationDataModel tagCombi)
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

                return new Point(xSum / 2.0F, ySum / 2.0F);
            }
            else
            {   // 0 or 1 tag in combi. Not valid for this method.
                throw new Exception("This method needs 2 or more tags to calculate the center");
            }
        }

        #endregion UnusedMethods

    }
}
