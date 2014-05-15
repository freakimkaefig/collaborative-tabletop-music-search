using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Waf.Applications;

namespace Waf.Writer.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    internal class ShellService : DataModel, IShellService
    {
        private readonly ReadOnlyCollection<string> readOnlyDefaultZooms;
        private double zoom;
        private string documentName;
        private object shellView;


        [ImportingConstructor]
        public ShellService()
        {
            readOnlyDefaultZooms = new ReadOnlyCollection<string>(new double[] { 2, 1.5, 1.25, 1, 0.75, 0.5 }
                .Select(d => string.Format(CultureInfo.CurrentCulture, "{0:P0}", d)).ToArray());
            zoom = 1;
        }


        public IEnumerable<string> DefaultZooms { get { return readOnlyDefaultZooms; } }

        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (zoom != value)
                {
                    zoom = Math.Max(value, 0.25);
                    zoom = Math.Min(zoom, 16);
                    RaisePropertyChanged("Zoom");
                }
            }
        }

        public string DocumentName
        {
            get { return documentName; }
            set
            {
                if (documentName != value)
                {
                    documentName = value;
                    RaisePropertyChanged("DocumentName");
                }
            }
        }

        public object ShellView
        {
            get { return shellView; }
            set
            {
                if (shellView != value)
                {
                    shellView = value;
                    RaisePropertyChanged("ShellView");
                }
            }
        }
    }
}
