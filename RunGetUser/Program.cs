using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginForm;
using LoginForm.CCMAlogin;

namespace LoginForm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString());

            LoginForm.AdressChg adressChg = new AdressChg(); //4321
            adressChg.AddAgent();
            //adressChg.NewAdress();
            Console.WriteLine(DateTime.Now.ToString());

            Console.WriteLine("             *****************       exit             ***************** ");
            Console.ReadLine();

        }
    }
}
