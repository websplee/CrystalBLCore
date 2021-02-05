using CrystalBLCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrystalBLCore.BusinessServices.Interfaces
{
    public interface IEmailService
    {
        Task Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
