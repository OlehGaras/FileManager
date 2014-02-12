using System;
using System.Collections.Generic;

namespace FileManager
{
    public interface IErrorManager
    {
        List<Error> GetErrors();
        void AddError(Error error);
        event EventHandler<Error> ErrorsChanged;
    }
}
