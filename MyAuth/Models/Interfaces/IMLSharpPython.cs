using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Interfaces
{
    public interface IMLSharpPython
    {
        string ExecutePythonScript(string filePythonScript, string arguments, out string standardError);
    }
}
