using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForlornStub
{
    public class QuietExceptionHandler : IExceptionHandler
    {
        public void HandleException(Exception ex)
        {
            Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
        }
    }
}
