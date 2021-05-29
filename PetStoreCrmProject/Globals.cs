using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace PetStoreCrmProject
{
    public class Globals
    {
        public static DataLayer.PetStoreDbContext ctx;

        // RegEx validation
        public static string nameRegEx = (@"^[a-zA-Z\d %_\(\),\.\/\\-]{0,50}$"); // {0,50} because animal name can be null
        public static string zipRegEx = (@"^[a-zA-Z1-9]{3}\s[a-zA-Z1-9]{3}$");
        public static string phoneRegEx = (@"^[(][0-9]{3}[)][0-9]{3}-[0-9]{4}$");
        public static string emailRegEx = (@"^[a-zA-Z\d]{1,15}\@ourshelter\.com$");
        public static string SIN = (@"^[\d]{3}\s[\d]{3}\s[\d]{3}$");
        public static bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        //Format Sytem.Datetime?
        //public const string Format = "dd.MM.yyyy";

        /*
        public static DateTime Formatter(System.DateTime? x)
        {
            string date = x.ToString();
            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            //return x.ToString("yyyy.MM.dd");
        }
        */
    }
}
