using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Artisan.Helpers
{
    interface IEmailSender
    {
        Task Send(string destination, string subject, string body);
    }
}
