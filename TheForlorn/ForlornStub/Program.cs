using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForlornStub
{
    class Program
    {
        public static bool StayAlive = true;

        static void Main(string[] args)
        {
            ForlornStubController fsc = new ForlornStubController();
            fsc.Begin();

            while (StayAlive)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
