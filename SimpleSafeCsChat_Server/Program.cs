using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSafeCsChat_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerManager serverManager = new ServerManager("1111");
            while (true) Thread.Sleep(10);
        }
    }
}
