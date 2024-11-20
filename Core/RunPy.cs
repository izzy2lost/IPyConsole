using System.Windows;
using IronPython.Hosting;


namespace Core
{
    public static class RunPy
    {
        public static void Run()
        {
            var eng = Python.CreateEngine();

            eng.ExecuteFile("C:\\Users\\xuanit\\Code\\Learning\\IPyConsole\\IPyConsole\\API\\API.py");
        }
    }

}
