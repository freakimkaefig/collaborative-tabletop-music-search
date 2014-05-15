using System.ComponentModel;
using System.Waf.Foundation;

namespace Waf.InformationManager.Common.Domain
{
    /// <summary>
    /// Defines the base class for a model with validation support.
    /// </summary>
    public abstract class ValidationModel : Model, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport dataErrorInfoSupport;


        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationModel"/> class.
        /// </summary>
        protected ValidationModel()
        {
            dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }


        string IDataErrorInfo.Error { get { return dataErrorInfoSupport.Error; } }

        string IDataErrorInfo.this[string columnName] { get { return dataErrorInfoSupport[columnName]; } }
    }
}
