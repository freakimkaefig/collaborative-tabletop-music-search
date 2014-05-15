﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.Infrastructure.Interfaces.Applications;
using System.Waf.Applications;

namespace Test.InformationManager.Infrastructure.Modules.Applications.Services
{
    public class MockNavigationNode : DataModel, INavigationNode
    {
        public MockNavigationNode(string name, Action showAction, Action closeAction, double group, double order)
        {
            this.Name = name;
            this.ShowAction = showAction;
            this.CloseAction = closeAction;
            this.Group = group;
            this.Order = order;
        }
        

        public string Name { get; private set; }

        public Action ShowAction { get; private set; }

        public Action CloseAction { get; private set; }

        public double Group { get; private set; }

        public double Order { get; private set; }

        public int? ItemCount { get; set; }
    }
}
