using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


//use: regasm MyLibrary.dll /tlb:MyLibrary.tlb
//to generate tlb file
namespace ClientSDK.ComInterop
{
    [ComVisible(true)]
    [Guid("BA1D9D45-1D76-4C8C-87D5-D6558B007EC1")]
    [ClassInterface(ClassInterfaceType.None)]
    public class ComDemo : IComDemo
    {
        public void Test(string name)
        {
            Console.WriteLine($"Hello, {name} from COM!");
        }
    }

    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("7C16F961-5800-4B1B-824F-F1F67BCEFDFF")]
    public interface IComDemo
    {
        void Test(string name);
    }
}
