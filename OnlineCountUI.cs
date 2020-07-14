using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using System;
using Logger = Rocket.Core.Logging.Logger;
using SDG.Unturned;
using Rocket.Unturned;
using Rocket.Core;
using Rocket.Core.Permissions;
using Rocket.API.Serialisation;

namespace TPlugins.OnlineCountUI
{
    public class OnlineCountUI : RocketPlugin<OnlineCountUIConfig>
    {
        public static OnlineCountUI Instance;
        public string Version = "1.0.0";
        public DateTime LastUIRefreshDate;

        //On plugin has loaded
        protected override void Load()
        {
            Instance = this;
            U.Events.OnPlayerConnected += Join;
            U.Events.OnPlayerDisconnected += Disconnect;
            LastUIRefreshDate = DateTime.Now;
            Logger.Log("####################################", color: ConsoleColor.Yellow);
            Logger.Log("#       Plugin Version: " + Version + "       #", color: ConsoleColor.Yellow);
            Logger.Log("####################################", color: ConsoleColor.Yellow);
            Logger.Log("");
            Logger.Log("OnlineCountUI has successfully loaded!", color: ConsoleColor.Green);
        }

        //On plugin has unloaded
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Join;
            U.Events.OnPlayerDisconnected -= Disconnect;
            Logger.Log("OnlineCountUI has successfully unloaded!", color: ConsoleColor.Green);
        }

        //On player joined to the server
        private void Join(UnturnedPlayer p)
        {
            UpdateUI(p, true);
            foreach (SteamPlayer sp in Provider.clients)
            {
                UnturnedPlayer p2 = UnturnedPlayer.FromSteamPlayer(sp);
                UpdateUI(p2);
            }
        }

        //On player left from the server
        private void Disconnect(UnturnedPlayer p)
        {
            foreach (SteamPlayer sp in Provider.clients)
            {
                UnturnedPlayer p2 = UnturnedPlayer.FromSteamPlayer(sp);
                UpdateUI(p2);
            }
        }

        //Translations
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    {"ui_text", "Online {0}: {1}"},
                    {"police", "policemans"},
                    {"medic", "medics"},
                    {"mechanic", "mechanics"},
                    {"admin", "admins"}
                };
            }
        }

        //Updates the UI
        public void UpdateUI(UnturnedPlayer p, bool firstsend = false)
        {
            short effectshortID = (short)Configuration.Instance.UIEffectID;
            if (firstsend)
                EffectManager.sendUIEffect(Configuration.Instance.UIEffectID, effectshortID, p.CSteamID, true);

            int admin_members_integer = 0;
            int police_members_integer = 0;
            int mechanic_members_integer = 0;
            int medic_members_integer = 0;

            string admin_members_string = null;
            string police_members_string = null;
            string mechanic_members_string = null;
            string medic_members_string = null;

            var config = Configuration.Instance;

            RocketPermissionsManager Permissions = R.Instance.GetComponent<RocketPermissionsManager>();
            RocketPermissionsGroup group_pol = R.Permissions.GetGroup(config.PoliceGroup);
            RocketPermissionsGroup group_med = R.Permissions.GetGroup(config.MedicGroup);
            RocketPermissionsGroup group_mec = R.Permissions.GetGroup(config.MechanicGroup);
            RocketPermissionsGroup group_ad = R.Permissions.GetGroup(config.PoliceGroup);

            Provider.clients.ForEach(client =>
            {
                string clientid = client.playerID.steamID.m_SteamID.ToString();
                if (group_pol.Members.Contains(clientid))
                {
                    police_members_integer = police_members_integer + 1;
                }
                else if (group_med.Members.Contains(clientid))
                {
                    medic_members_integer = medic_members_integer + 1;
                }
                else if (group_mec.Members.Contains(clientid))
                {
                    mechanic_members_integer = mechanic_members_integer + 1;
                }
                else if (group_ad.Members.Contains(clientid) || client.isAdmin)
                {
                    admin_members_integer = admin_members_integer + 1;
                }
            });

            if (config.ShowOfflineMembersFromtheGroup)
            {
                police_members_string = police_members_integer.ToString() + "/" + group_pol.Members.Count;
                medic_members_string = medic_members_integer.ToString() + "/" + group_med.Members.Count;
                mechanic_members_string = mechanic_members_integer.ToString() + "/" + group_mec.Members.Count;
                admin_members_string = admin_members_integer.ToString() + "/" + group_ad.Members.Count;
            }
            else
            {
                police_members_string = police_members_integer.ToString();
                medic_members_string = medic_members_integer.ToString();
                mechanic_members_string = mechanic_members_integer.ToString();
                admin_members_string = admin_members_integer.ToString();
            }

            EffectManager.sendUIEffectText(effectshortID, p.CSteamID, true, "police_members_count_text", Translate("ui_text", Translate("police"), police_members_string));
            EffectManager.sendUIEffectText(effectshortID, p.CSteamID, true, "medic_members_count_text", Translate("ui_text", Translate("medic"), medic_members_string));
            EffectManager.sendUIEffectText(effectshortID, p.CSteamID, true, "mechanic_members_count_text", Translate("ui_text", Translate("mechanic"), mechanic_members_string));
            EffectManager.sendUIEffectText(effectshortID, p.CSteamID, true, "admin_members_count_text", Translate("ui_text", Translate("admin"), admin_members_string));
        }

        //Auto UI Refresh
        public void Update()
        {
            if (Configuration.Instance.EnableAutoRefresh)
            {
                if (LastUIRefreshDate <= DateTime.Now)
                {
                    foreach (SteamPlayer sp in Provider.clients)
                    {
                        UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(sp);
                        UpdateUI(p);
                    }
                    LastUIRefreshDate = DateTime.Now.AddSeconds(Configuration.Instance.AutoRefreshIntervalSecs);
                }
            }
        }
    }
}