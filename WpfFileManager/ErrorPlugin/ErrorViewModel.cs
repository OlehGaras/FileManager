using System.Collections.ObjectModel;
using FileManager;

namespace ErrorPlugin
{
    public class ErrorViewModel:ViewModelBase
    {
        private readonly IErrorManager mErrorManager;

        public ErrorViewModel(IErrorManager errorManager)
        {
            mErrorManager = errorManager;
            mErrorManager.ErrorsChanged += ReloadErrorsView;
        }

        private void ReloadErrorsView(object sender, Error e)
        {
            Errors = new ObservableCollection<Error>(mErrorManager.GetErrors());
        }

        private ObservableCollection<Error> mErrors = new ObservableCollection<Error>();
        public ObservableCollection<Error> Errors
        {
            get { return mErrors; }
            set
            {
                if (value != mErrors)
                {
                    mErrors = value;
                    OnPropertyChanged("Errors");
                }
            }
        }
    }
}
