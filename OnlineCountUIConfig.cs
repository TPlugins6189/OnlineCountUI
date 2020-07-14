using Rocket.API;

namespace TPlugins.OnlineCountUI
{
    public class OnlineCountUIConfig : IRocketPluginConfiguration
    {
        public ushort UIEffectID;
        public bool ShowOfflineMembersFromtheGroup;
        public string PoliceGroup;
        public string MedicGroup;
        public string MechanicGroup;
        public string AdminGroup;
        public bool EnableAutoRefresh;
        public int AutoRefreshIntervalSecs;

        public void LoadDefaults()
        {
            UIEffectID = 0;
            ShowOfflineMembersFromtheGroup = false;
            PoliceGroup = "police";
            MedicGroup = "medic";
            MechanicGroup = "mechanic";
            AdminGroup = "admin";
            EnableAutoRefresh = true;
            AutoRefreshIntervalSecs = 300;
        }
    }
}
