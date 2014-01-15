using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class BackgroundWorkHelper
    {
        //http://msdn.microsoft.com/de-de/magazine/cc163328.aspx#S4
        /// <summary>
        /// Execute the given method in background. Use Wpf background worker.
        /// </summary>
        /// <param name="workerMethod">The method that has to be executed in background</param>
        /// <param name="completedHandler">The method that's called when work is completed</param>
        public void DoInBackground(DoWorkEventHandler workerMethod, RunWorkerCompletedEventHandler completedHandler, object arguments = null)
        {
            BackgroundWorker bgWorker = new BackgroundWorker();

            // Set up the Background Worker Events
            bgWorker.DoWork += workerMethod;

            // Decide which method shall be executed when work is completed
            bgWorker.RunWorkerCompleted += completedHandler;

            // Run the Background Worker
            if (arguments != null)
            {
                bgWorker.RunWorkerAsync(arguments);
            }
            else
            {
                bgWorker.RunWorkerAsync();
            }
        }

    }
}
