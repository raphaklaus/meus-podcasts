using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace MeusPodcasts.Classes
{
    class Util
    {
        public static bool PossuiConexaoInternet()
        {
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            IReadOnlyList<ConnectionProfile> connectionProfile = NetworkInformation.GetConnectionProfiles();
            if (InternetConnectionProfile == null)
                return false;
            else
                return true;
        }
    }
}
