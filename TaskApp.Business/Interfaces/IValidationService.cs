using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskApp.Business.Interfaces
{
    public interface IValidationService
    {
        bool EmailValidation(string str);
    }
}
