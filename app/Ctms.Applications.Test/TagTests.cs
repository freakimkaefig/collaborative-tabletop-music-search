using System;
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
using System.Windows;

namespace Ctms.Applications.Test
{
    [TestClass]
    public class TagTests : BasicTests
    {

        [ClassInitialize]
        public static void Init(TestContext textContext)
        {
            _repository = new Repository(null, null, null, null, null, null, null, null);
            _tagFactory = new TagFactory(_repository);
        }

        [TestMethod]
        public void CalcCenterOf3()
        {
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

            var point = _tagCombinationWorker.GetCenterOfPolygon(tagCombination);

            Assert.IsTrue((int)point.X == 100, "x is not 100. it's " + point.X);
            Assert.IsTrue((int)point.Y == 96, "x is not 96. it's " + (int)point.Y);
        }

        [TestMethod]
        public void CalcCenterOf4()
        {
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
                    },                        
                    new TagDataModel()
                    {
                        Tag = new Tag()
                        {
                            PositionX = 77,
                            PositionY = 88
                        }
                    }
                }
            };

            _tagCombinationWorker = new TagCombinationWorker(null, _repository, null);

            var point = _tagCombinationWorker.GetCenterOfPolygon(tagCombination);

            Assert.IsTrue((int)point.X == 94, "x is not 94. it's " + (int)point.X);
            Assert.IsTrue((int)point.Y == 94, "x is not 94. it's " + (int)point.Y);
        }

        private Point Compare()
        {
            var points = new List<Point>()
            {
                new Point(100,100),
                new Point(120,130),
                new Point(80,60),
                new Point(77,88),
            };

            Point centroid =
                    points.Aggregate(
                    new { xSum = 0.0, ySum = 0.0, n = 0 },
                    (acc, p) => new
                    {
                        xSum = acc.xSum + p.X,
                        ySum = acc.ySum + p.Y,
                        n = acc.n + 1
                    },
                    acc => new Point(acc.xSum / acc.n, acc.ySum / acc.n));
            return centroid;
        }

        [ClassCleanup]
        public static void Cleanup()
        {

        }
    }
}
