﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Steamworks;
using Rocket.API.Serialisation;
using Rocket.Unturned;
using Logger = Rocket.Core.Logging;

namespace AntiFriendlyFire
{
    public class Plugin : RocketPlugin<Configuration>
    {
        public static Plugin Instance;

        protected override void Load()
        {
            if (!Configuration.Instance.Enabled)
            {
                Logger.Logger.Log("[AFF] Plugin is disabled in the config! Unloading . . .");
                Unload();
            }
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
        }
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
        }
        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            player.Player.life.onHurt -= onHurtEvent;
        }
        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            player.Player.life.onHurt += onHurtEvent;
        }
        private void onHurtEvent(Player player, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            UnturnedPlayer pKiller = UnturnedPlayer.FromCSteamID(killer);
            UnturnedPlayer pVictim = UnturnedPlayer.FromPlayer(player);
            if (killer == null)
            {
                //they didnt die by another player
                //other random cause
                Logger.Logger.Log("[AFF] The player (" + pVictim.DisplayName + ") died by a random cause!");
                return;
            }
            if (pKiller.CSteamID == pVictim.CSteamID)
            {
                //they're damaging themselves
                Logger.Logger.Log("[AFF] The player (" + pVictim.DisplayName + ") has died by his own hand!");
                return;
            }
            if (cause == EDeathCause.BLEEDING)
            {
                //they're bleeding. we dont need to pay attention
                Logger.Logger.Log("[AFF] The player (" + pVictim.DisplayName + ") has bled out!");
                return;
            }
            foreach (RocketPermissionsGroup r in Rocket.Core.R.Permissions.GetGroups(pKiller, true))
            {
                foreach (RocketPermissionsGroup r2 in Rocket.Core.R.Permissions.GetGroups(pVictim, true))
                {
                    if (r.Id == r2.Id)
                    {
                        player.life.askHeal(damage, true, true);
                        Logger.Logger.Log("[AFF] Healed (" + pVictim.DisplayName + ") With (" + damage + ") HP!");
                    }
                }
            }
            /*if (Rocket.Core.R.Permissions.GetGroups(pKiller, true).Any(g => g.Id == "xd") == Rocket.Core.R.Permissions.GetGroups(pVictim, true).Any())
            {
                    player.life.askHeal(damage, true, true);
            }*/
        }
    }
}
