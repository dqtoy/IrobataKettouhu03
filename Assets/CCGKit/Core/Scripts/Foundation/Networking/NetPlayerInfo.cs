
using UnityEngine.Networking;

namespace CCGKit
{
    public struct NetPlayerInfo
    {
        public int id;
        public NetworkInstanceId netId;
        public NetStat[] stats;
        public NetStaticZone[] staticZones;
        public NetDynamicZone[] dynamicZones;
    }
}