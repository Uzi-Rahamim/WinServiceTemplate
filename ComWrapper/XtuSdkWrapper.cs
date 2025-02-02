using System.Runtime.InteropServices;

namespace ComWrapper
{
    [ComVisible(true)]
    [Guid("8D3FA27A-0742-437B-A287-D8140387EEC4")]
    [ClassInterface(ClassInterfaceType.None)]
    public class XtuSdkWrapper : IXtuSdkWrapper
    {
        public XtuSdkWrapper() { }

        public string Activate(string name)
        {
            return $"XtuSdkWrapper .NET Framework 4.8, {name}!";
        }
    }

    // Interface definition for the COM-visible class
    [ComVisible(true)]
    [Guid("C90085FC-4CA4-4AF2-8713-AE1B2794ABF5")]
    public interface IXtuSdkWrapper
    {
        string Activate(string name);
    }

}
