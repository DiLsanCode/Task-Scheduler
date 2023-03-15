using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TaskApp.Business.Interfaces;

namespace TaskApp.Business.Services
{
    public class ValidationService : IValidationService
    {
        public bool EmailValidation(string str)
        {
            try
            {
                MailAddress m = new MailAddress(str);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
