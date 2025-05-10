using System;
using System.Diagnostics;

namespace DummyTestProject
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            // Usage inside a test:
            var currentProcess = Process.GetCurrentProcess();
            var parentProcess = GetParentProcess(currentProcess);
            Console.WriteLine("Parent process: " + parentProcess?.ProcessName);

        }
    }





}