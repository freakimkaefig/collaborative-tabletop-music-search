﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ctms.Applications.DataModels;
using Ctms.Applications.Common;
using Ctms.Applications.DataFactories;
using Ctms.Applications.Data;
using Ctms.Applications.Services;
using Ctms.Applications.ViewModels;
using Ctms.Domain;
using System.Collections.ObjectModel;
using Ctms.Applications.Workers;
using Ctms.Domain.Objects;

namespace Ctms.Applications.Test
{
    [TestClass]
    public class TagTests : BasicTests
    {
        private static ObservableCollection<TagDataModel> DataModels;
        private static ObservableCollection<Tag> Tags;
        private static ObservableCollection<TagOption> TagOptions;
        private static ObservableCollection<Keyword> Keywords;
        private static ObservableCollection<TagCombinationDataModel> TagCombinations;

        [ClassInitialize]
        public static void Init(TestContext textContext)
        {
            //BasicTests.Initialize();

            DataModels = new ObservableCollection<TagDataModel>();
            _repository = new Repository(null, null, null, null, null, null, null, null);
            _tagFactory = new TagFactory(_repository);

            

            TagCombinations = new ObservableCollection<TagCombinationDataModel>()
            {
                new TagCombinationDataModel(20)
                {
                    Tags = new ObservableCollection<TagDataModel>()
                    {
                        new TagDataModel()
                        {
                            Tag = new Tag()
                            {
                                PositionX = 100,
                                PositionY = 100
                            }
                        },                        
                        new TagDataModel()
                        {
                            Tag = new Tag()
                            {
                                PositionX = 120,
                                PositionY = 130
                            }
                        },                        
                        new TagDataModel()
                        {
                            Tag = new Tag()
                            {
                                PositionX = 80,
                                PositionY = 60
                            }
                        }
                    }
                }
            };
        }

        [TestMethod]
        public void CalcCenter()
        {
            /*
            var tag = _tagFactory.CreateTagDataModel(0);
            tag.Tag.PositionX = 100;
            tag.Tag.PositionY = 100;
            Tags.Add(tag);

            var tag2 = _tagFactory.CreateTagDataModel(0);
            tag2.Tag.PositionX = 120;
            tag2.Tag.PositionY = 130;
            Tags.Add(tag2);

            var tag3 = _tagFactory.CreateTagDataModel(0);
            tag3.Tag.PositionX = 80;
            tag3.Tag.PositionY = 60;
            Tags.Add(tag3);
            */

            var tagCombination = new TagCombinationDataModel(20)
            {
                Tags = new ObservableCollection<TagDataModel>()
                {
                    new TagDataModel()
                    {
                        Tag = new Tag()
                        {
                            PositionX = 100,
                            PositionY = 100
                        }
                    },                        
                    new TagDataModel()
                    {
                        Tag = new Tag()
                        {
                            PositionX = 120,
                            PositionY = 130
                        }
                    },                        
                    new TagDataModel()
                    {
                        Tag = new Tag()
                        {
                            PositionX = 80,
                            PositionY = 60
                        }
                    }
                }
            };

            _tagCombinationWorker = new TagCombinationWorker(null, _repository, null);

            var point = _tagCombinationWorker.UpdateCenter(tagCombination);

            Assert.IsTrue((int)point.X == 100, "x is not 100. it's " + point.X);
            Assert.IsTrue((int)point.Y == 96, "x is not 96. it's " + (int)point.Y);
        }

        [ClassCleanup]
        public static void Cleanup()
        {

        }
    }
}
