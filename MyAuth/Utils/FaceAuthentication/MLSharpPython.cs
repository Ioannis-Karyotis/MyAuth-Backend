﻿using MyAuth.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZetaLongPaths;

namespace MyAuth.Utils.FaceAuthentication
{
    public class MLSharpPython : IMLSharpPython
    {
        public readonly string filePythonExePath;
        /// <summary>
        /// ML Sharp Python class constructor
        /// </summary>
        /// <param name="exePythonPath">Python EXE file path</param>
        public MLSharpPython(string exePythonPath)
        {
            filePythonExePath = exePythonPath;
        }
        /// <summary>
        /// Execute Python script file
        /// </summary>
        /// <param name="filePythonScript">Python script file and input parameter(s)</param>
        /// <param name="standardError">Output standard error</param>
        /// <returns>Output text result</returns>
        public string ExecutePythonScript(string filePythonScript, string arguments, out string standardError)
        {
            string outputText = string.Empty;
            standardError = string.Empty;

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo(filePythonExePath)
                    {
                        FileName = filePythonScript,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    process.Start();
                    outputText = process.StandardOutput.ReadToEnd();
                    outputText = outputText.Replace(Environment.NewLine, string.Empty);
                    standardError = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
            }
            return outputText;
        }
    }
}
