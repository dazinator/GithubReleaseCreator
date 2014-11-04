using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubReleaseCreator
{
    public class Logger
    {

        private bool _Verbose = false;
        public Logger(bool verbose)
        {
            _Verbose = verbose;
        }

        public void LogVerbose(string message)
        {
            if (_Verbose)
            {
                Console.WriteLine(message);
            }
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void LogError(string message)
        {
            Console.WriteLine(message);
        }

    }
}
