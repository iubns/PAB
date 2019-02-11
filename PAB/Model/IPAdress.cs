using System.Net.NetworkInformation;

namespace PAB.Model
{
    static class IPAdress
    {
        public static string GetMacAdress (){
            NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
            return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
        }
    }
}
