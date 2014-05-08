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
        private BackgroundWorker bgWorker;

        //http://msdn.microsoft.com/de-de/magazine/cc163328.aspx#S4
        /// <summary>
        /// Execute the given method in background. Use Wpf background worker.
        /// </summary>
        /// <param name="workerMethod"> 
        ///     The method that has to be executed in background. Must use the schema:
        ///         public void [MethodName] (object sender, DoWorkEventArgs e)
        ///     The workerMethod can use the optional arguments like that: 
        ///         var argument = ([TypeOfArgument]) e.Argument;
        ///     And set the result for the completedHandler like that: 
        ///         e.Result = [AnyObjectORValue];
        /// </param>
        /// <param name="completedHandler">
        ///     The method that's called when work is completed.  Must use the schema:
        ///         private void [MethodName] (object sender, RunWorkerCompletedEventArgs e)
        ///     Usage of arguments:
        ///         e.Cancelled -> true/false
        ///         e.Error -> true/false
        ///         Argument: var result = ([AnyObjectORValue]) e.Result;
        /// </param>
        public BackgroundWorker DoInBackground(DoWorkEventHandler workerMethod, RunWorkerCompletedEventHandler completedHandler, object arguments = null)
        {
            bgWorker = new BackgroundWorker();

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

            return bgWorker;
        }

        public void DoInBackgroundWithParams(DoWorkEventHandler workerMethod, RunWorkerCompletedEventHandler completedHandler, params object[] arguments)
        {
            bgWorker = new BackgroundWorker();

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

        public void Stop()
        {
            bgWorker.CancelAsync();
        }

    }
}
