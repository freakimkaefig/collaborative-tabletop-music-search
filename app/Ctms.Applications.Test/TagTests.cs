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

namespace Ctms.Applications.Test
{
    [TestClass]
    public class TagTests : BasicTests
    {
        private static ObservableCollection<TagDataModel> Tags;

        [ClassInitialize]
        public static void Init(TestContext textContext)
        {
            //BasicTests.Initialize();
            
            Tags = new ObservableCollection<TagDataModel>();
            _repository = new Repository(null, null, null, null, null, null, null, null);
            _tagFactory = new TagFactory(_repository);

            
        }

        [TestMethod]
        public void CalcCenter()
        {
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

            _tagCombinationWorker = new TagCombinationWorker(null, _repository, null);
            var point = _tagCombinationWorker.UpdateCenter(Tags);

            Assert.IsTrue(point.X == 100);
            Assert.IsTrue(point.Y == 96.667);
        }
        
        [ClassCleanup]
        public static void Cleanup()
        {

        }
    }
}
