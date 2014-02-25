using Waf.Writer.Applications.Documents;
using Waf.Writer.Applications.Services;
using Waf.Writer.Applications.Views;

namespace Waf.Writer.Applications.ViewModels
{
    public class RichTextViewModel : ZoomViewModel<IRichTextView>
    {
        private readonly RichTextDocument document;
        private bool isBold;
        private bool isItalic;
        private bool isUnderline;
        private bool isNumberedList;
        private bool isBulletList;
        private bool isSpellCheckEnabled;

        
        public RichTextViewModel(IRichTextView view, IShellService shellService, RichTextDocument document) : base(view, shellService)
        {
            this.document = document;
        }


        public RichTextDocument Document { get { return document; } }

        public bool IsBold
        {
            get { return isBold; }
            set
            {
                if (isBold != value)
                {
                    isBold = value;
                    RaisePropertyChanged("IsBold");
                }
            }
        }

        public bool IsItalic
        {
            get { return isItalic; }
            set
            {
                if (isItalic != value)
                {
                    isItalic = value;
                    RaisePropertyChanged("IsItalic");
                }
            }
        }
        
        public bool IsUnderline
        {
            get { return isUnderline; }
            set
            {
                if (isUnderline != value)
                {
                    isUnderline = value;
                    RaisePropertyChanged("IsUnderline");
                }
            }
        }

        public bool IsNumberedList
        {
            get { return isNumberedList; }
            set
            {
                if (isNumberedList != value)
                {
                    isNumberedList = value;
                    RaisePropertyChanged("IsNumberedList");
                }
            }
        }

        public bool IsBulletList
        {
            get { return isBulletList; }
            set
            {
                if (isBulletList != value)
                {
                    isBulletList = value;
                    RaisePropertyChanged("IsBulletList");
                }
            }
        }

        public bool IsSpellCheckEnabled
        {
            get { return isSpellCheckEnabled; }
            set
            {
                if (isSpellCheckEnabled != value)
                {
                    isSpellCheckEnabled = value;
                    RaisePropertyChanged("IsSpellCheckEnabled");
                }
            }
        }
    }
}
