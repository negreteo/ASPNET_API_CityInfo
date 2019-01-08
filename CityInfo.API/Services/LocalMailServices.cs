using System.Diagnostics;

namespace CityInfo.API.Services
{
   public class LocalMailServices : IMailServices
   {
      // private string _mailTo = "admin@mycompany.com";
      // private string _mailFrom = "noreply@mycompany.com";

      private string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
      private string _mailFrom = Startup.Configuration["mailSettings:mailToAddress"];

      public void Send (string subject, string message)
      {
         // send email - output to debug winwdow
         Debug.WriteLine ($"Mail from {_mailFrom} to {_mailTo}, with Local Service");
         Debug.WriteLine ($"Subject: {subject}");
         Debug.WriteLine ($"Message: {message}");
      }
   }
}
