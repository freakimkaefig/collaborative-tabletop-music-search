using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.DataModels;
using Ctms.Applications.Common;
using Ctms.Applications.DataFactories;
using Ctms.Applications.Data;
using Ctms.Applications.Services;
using Ctms.Applications.ViewModels;
using Ctms.Applications.Workers;

namespace Ctms.Applications.Test
{
    public class BasicTests
    {
        protected static List<TagDataModel> _tags;
        protected static TagFactory         _tagFactory;
        protected static EntityService      _entityService;
        protected static SearchViewModel    _searchVm;
        protected static DetailViewModel    _detailVm;
        protected static MenuViewModel      _menuVm;
        protected static PlaylistViewModel  _playlistVm;
        protected static ResultViewModel    _resultVm;
        protected static SearchTagViewModel _searchTagVm;
        protected static InfoViewModel      _infoVm;
        protected static ShellViewModel     _shellVm;
        protected static Repository         _repository;
        protected static InfoWorker         _infoWorker;
        protected static TagCombinationWorker _tagCombinationWorker;

        protected static void Initialize()
        {
            _searchVm       = new SearchViewModel(null);
            _searchTagVm    = new SearchTagViewModel(null);
            _detailVm       = new DetailViewModel(null);
            _menuVm         = new MenuViewModel(null);
            _playlistVm     = new PlaylistViewModel(null);
            _resultVm       = new ResultViewModel(null);
            _repository     = new Repository(_searchVm, _detailVm, _menuVm, _playlistVm, _resultVm, _searchTagVm, _infoVm, _shellVm);
            _tagFactory     = new TagFactory(_repository);
            _tagCombinationWorker = new TagCombinationWorker(_searchVm, _repository, null);
        }
    }
}
