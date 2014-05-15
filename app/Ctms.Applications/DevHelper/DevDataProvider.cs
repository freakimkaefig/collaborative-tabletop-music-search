using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.Data;
using Ctms.Applications.DataModels;
using Ctms.Applications.DataFactories;
using Ctms.Domain.Objects;

namespace Ctms.Applications.DevHelper
{
    public static class DevDataProvider
    {
        private static Repository _repository;

        public static void Initialize(Repository repository)
        {
            _repository = repository;
        }

        public static void LoadResults()
        {

        }

        public static void LoadTags()
        {
            /*
            var tags = new List<Tag>();

            for (var i = 0; i < 4; i++)
            {
                var tag = new Tag()
                {
                    Id
                }
            }

            new Tag()
            {
                Id = 0
            };
            */
        }

        public static void LoadExampleTagOptions()
        {
            var factory = new TagFactory(_repository);
            for (var i = 0; i < 4; i++)
            {
                /*
                factory.CreateTagDataModel(null, i);
                {
                    _repository.AddTagDataModel(tagDataModel);
                }*/
            }
        }
    }
}
