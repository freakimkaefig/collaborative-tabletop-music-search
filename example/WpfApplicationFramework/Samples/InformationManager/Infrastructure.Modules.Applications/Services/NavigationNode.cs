using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Waf.Applications;
using Waf.InformationManager.Infrastructure.Interfaces.Applications;

namespace Waf.InformationManager.Infrastructure.Modules.Applications.Services
{
    public class NavigationNode : DataModel, INavigationNode
    {
        private readonly Action showAction;
        private readonly Action closeAction;
        private int? itemCount;
        private bool isSelected;
        private bool isFirstItemOfNewGroup;


        public NavigationNode(string name, Action showAction, Action closeAction, double group, double order)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException("name must not be null or empty."); }
            if (showAction == null) { throw new ArgumentNullException("showAction"); }
            if (closeAction == null) { throw new ArgumentNullException("closeAction"); }
            if (group < 0) { throw new ArgumentException("group must be equal or greater than 0."); }
            if (order < 0) { throw new ArgumentException("order must be equal or greater than 0."); }
            
            this.Name = name;
            this.showAction = showAction;
            this.closeAction = closeAction;
            this.Group = group;
            this.Order = order;
        }
        
      
        public string Name { get; private set; }

        public double Group { get; private set; }

        public double Order { get; private set; }

        public int? ItemCount
        {
            get { return itemCount; }
            set
            {
                if (itemCount != value)
                {
                    itemCount = value;
                    RaisePropertyChanged("ItemCount");
                }
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    if (isSelected)
                    {
                        closeAction();
                    }
                    
                    isSelected = value;
                    RaisePropertyChanged("IsSelected");

                    if (isSelected)
                    {
                        showAction();
                    }
                }
            }
        }

        public bool IsFirstItemOfNewGroup
        {
            get { return isFirstItemOfNewGroup; }
            set
            {
                if (isFirstItemOfNewGroup != value)
                {
                    isFirstItemOfNewGroup = value;
                    RaisePropertyChanged("IsFirstItemOfNewGroup");
                }
            }
        }
    }
}
