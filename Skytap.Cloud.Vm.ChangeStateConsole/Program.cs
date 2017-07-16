using System;
using System.Configuration;
using System.Threading;
using Skytap.Cloud.Vm.ChangeState;

namespace Skytap.Cloud.Vm.ChangeStateConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] Vms = { "20361700", "20361698" };
            var login = ConfigurationSettings.AppSettings["login"];
            var psw = ConfigurationSettings.AppSettings["psw"];
            var baseurl = ConfigurationSettings.AppSettings["baseurl"];
            var configid = ConfigurationSettings.AppSettings["configid"];
            VmState vMState = new VmState(baseurl, configid);
            foreach(var vm in Vms)
                {
                Console.WriteLine("Starting :... ");
                Console.WriteLine(vMState.Run(login, psw, vm));

                Console.WriteLine("Suspending :... ");
                Console.WriteLine(vMState.Suspend(login, psw, vm));

                Console.WriteLine("Resumeing :... ");
                Console.WriteLine(vMState.Resume(login, psw, vm));

                Console.WriteLine("SwitchOffing :... ");
                Console.WriteLine(vMState.SwitchOff(login, psw, vm));
            }

        
            Console.ReadKey();
        }
 
    }
}
    