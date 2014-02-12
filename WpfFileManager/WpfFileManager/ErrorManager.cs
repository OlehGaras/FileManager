using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using FileManager;

namespace WpfFileManager
{
    [Export(typeof(IErrorManager))]

    public class ErrorManager:IErrorManager
    {
        private readonly List<Error> mErrors = new List<Error>(); 

        public List<Error> GetErrors()
        {
            return new List<Error>(mErrors);
        }

        public void AddError(Error error)
        {
            mErrors.Add(error);
            OnErrorsChanged(error);
        }

        public event EventHandler<Error> ErrorsChanged;

        protected virtual void OnErrorsChanged(Error e)
        {
            var handler = ErrorsChanged;
            if (handler != null) handler(this, e);
        }
    }
}
