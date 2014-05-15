using System.ComponentModel;
using System.Waf.Applications;
using Waf.Writer.Applications.Services;

namespace Waf.Writer.Applications.ViewModels
{
    public abstract class ZoomViewModel<T> : ViewModel<T> where T : IView
    {
        private readonly IShellService shellService;
        private bool isVisible;
        private double zoom = 1;


        protected ZoomViewModel(T view, IShellService shellService) : base(view)
        {
            this.shellService = shellService;
            AddWeakEventListener(shellService, ShellServicePropertyChanged);
        }


        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    RaisePropertyChanged("IsVisible");

                    if (isVisible)
                    {
                        shellService.Zoom = Zoom;
                    }
                }
            }
        }

        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (zoom != value)
                {
                    zoom = value;
                    RaisePropertyChanged("Zoom");

                    if (IsVisible)
                    {
                        shellService.Zoom = zoom;
                    }
                }
            }
        }


        private void ShellServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Zoom" && IsVisible)
            {
                Zoom = shellService.Zoom;
            }
        }
    }
}
