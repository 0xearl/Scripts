//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Farm/AutoAttackKillUltra.cs
using RBot;
using RBot.Monsters;
using RBot.Options;

// Bot by: 🥔 Tato 🥔

public class FollowerJoe
{
    public ScriptInterface Bot => ScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    public CoreAdvanced Adv => new();
    public AAKillUltra AAKA => new();

    public bool DontPreconfigure = true;

    public string OptionsStorage = "Follower Joe";

    public List<IOption> Options = new List<IOption>()
    {
        new Option<string>("playerName", "Player Name", "Insert the name of the player to follow", "Insert Name"),
        new Option<bool>("skipSetup", "Skip this window next time", "You will be able to return to this screen via [Options] -> [Script Options] if you wish to change anything.", false),
        new Option<LockedZones>("LockedZone", "Locked Zone", "Insert the name of the Possible Locked Zone", LockedZones.tercessuinotlim),
        new Option<string>("RoomNumber", "Room Number", "Insert the Room# of the Possible Locked Zone", "Room#"),
    };



    public void ScriptMain(ScriptInterface bot)
    {
        Core.SetOptions();

        if (!Bot.Config.Get<bool>("skipSetup"))
            Bot.Config.Configure();

        while (!Bot.ShouldExit())
        {
            Bot.Events.CellChanged += Jumper;
            Bot.Player.Goto((Bot.Config.Get<string>("playerName")));
            Bot.Sleep(2500);
            if (Bot.Monsters.CurrentMonsters.Count(m => m.Alive) > 0)
            {
                if (Bot.Map.Name == "Mobius") //map is broke for some reason \o/
                {
                    Core.Logger($"Map: \"Mobius\" is broke, and i dont feel like fixing it >.>");
                    Bot.Sleep(2500);
                    Core.Relogin();
                    Bot.Sleep(2500);
                    Bot.SendPacket($"%xt%zm%house%1%{Bot.Player.Username}%");
                    Bot.Sleep(2500);
                    Core.Logger("Sleeping for 15 seconds to wait for Followee to be finished with mobius.");
                    Bot.Sleep(15000);
                    if (Bot.Map.Name != "Mobius")
                        break;
                }

                if (Bot.Map.Name == "shadowattack")
                {
                    Core.Jump("Boss", "Left");
                    Bot.Options.AttackWithoutTarget = true;
                    Bot.Player.Kill("Death");
                    Bot.Options.AttackWithoutTarget = false;
                }

                if (Bot.Map.Name == "DoomVault" && Bot.Map.Name == "DoomVaultb")
                {
                    Monster? Target = Bot.Monsters.CurrentMonsters.MaxBy(x => x.MaxHP);
                    if (Target == null)
                    {
                        Core.Logger("No monsters found", messageBox: true);
                        return;
                    }

                    Core.SetOptions(disableClassSwap: true);

                    Core.Logger("Target: " + Target.Name);
                    while (!Bot.ShouldExit())
                        Adv.KillUltra(Bot.Map.Name, Target.Cell, "Left", Target.Name, log: false, forAuto: true);
                }
                // Core.KillMonster(Bot.Map.Name, Bot.Player.Cell, Bot.Player.Pad, "*", null, 1, false, log: false);
                Bot.Player.Attack("*");
            }
            Bot.Sleep(1500);
            Bot.Wait.ForCombatExit();
        }
    }


    public void Jumper(ScriptInterface bot, string? map = null, string? cell = null, string? pad = null)
    {
        Bot.Events.ExtensionPacketReceived += CantGotoPlayer;
        while (!Bot.ShouldExit() && !Bot.Map.PlayerExists((Bot.Config.Get<string>("playerName"))))
        {
            Core.Logger($"Teleporting to {Bot.Config.Get<string>("playerName")}");
            Core.JumpWait();
            Bot.Sleep(Core.ActionDelay);
            Bot.Player.Goto((Bot.Config.Get<string>("playerName")));
            Bot.Sleep(Core.ActionDelay);
        }

        while (!Bot.ShouldExit() && Bot.Map.GetPlayer((Bot.Config.Get<string>("playerName"))) != null && Bot.Map.GetPlayer((Bot.Config.Get<string>("playerName"))).Cell != Bot.Player.Cell)
        {
            Core.Logger($"Cant Find {Bot.Config.Get<string>("playerName")}, Jumping");
            Core.JumpWait();
            Bot.Sleep(Core.ActionDelay);
            Bot.Player.Jump(Bot.Map.GetPlayer((Bot.Config.Get<string>("playerName"))).Cell, Bot.Map.GetPlayer((Bot.Config.Get<string>("playerName"))).Pad);
            Bot.Sleep(Core.ActionDelay);
        }
    }

    public void CantGotoPlayer(ScriptInterface Bot, dynamic packet)
    {
        int i = 0;
        string type = packet["params"].type;
        dynamic data = packet["params"].dataObj;
        if (type == "packet")
        {
            string str = data.strMessage;
            switch (str)
            {
                case "Cannot goto to player in a Locked zone.":
                    Core.Logger("Player is in a Locked Map.");
                    Core.Logger($"Joining {Bot.Config.Get<LockedZones>("LockedZone").ToString()}");
                    Core.JumpWait();
                    Bot.Wait.ForCombatExit();
                    Bot.Sleep(Core.ActionDelay);
                    Core.Logger($"Waiting for {Bot.Config.Get<string>("playerName")} Delayed(in seconds): {2 + (i++)}");
                    Core.Join(Bot.Config.Get<LockedZones>("LockedZone").ToString());
                    break;
            }
        }
        Bot.Events.ExtensionPacketReceived -= CantGotoPlayer;
    }

    public enum LockedZones
    {
        banzai,
        battlegrounda,
        battlegroundb,
        battlegroundc,
        battlegroundd,
        battlegrounde,
        battlegroundf,
        binky,
        chaoslord,
        darkoviaforest,
        doomvault,
        doomvaultb,
        doomwoodforest,
        hollowdeep,
        hyperiumstarship,
        ledgermayne,
        moonyard,
        moonyardb,
        shadowlordpast,
        shadowrealm,
        shadowrealmpast,
        superlowe,
        tercessuinotlim,
        lair,
        willowcreek,
        zephyrus
    }

    public enum IssueMaps
    {
        Mobius,
        Shadowattack
        //add maps here if they softlock.
    }
}
//
//                                  ▒▒▒▒▒▒▒▒▒▒▒▒▒▒░░                    
//                              ▓▓▓▓████████████████▓▓▓▓▒▒              
//                         ▓▓▓▓████░░░░░░░░░░░░░░░░██████▓▓            
//                      ▓▓████░░░░░░░░░░░░░░░░░░░░░░░░░░████          
//                   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██        
//                ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██      
//              ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██      
//             ▓▓██░░░░░░▓▓██░░  ░░░░░░░░░░░░░░░░░░░░▓▓██░░  ░░██    
//           ▓▓██░░░░░░░░██████░░░░░░░░░░░░░░░░░░░░░░██████░░░░░░██  
//          ▓▓██░░░░░░░░██████▓▓░░░░░░██░░░░██░░░░░░██████▓▓░░░░██  
//         ▓▓██▒▒░░░░░░░░▓▓████▓▓░░░░░░████████░░░░░░▓▓████▓▓░░░░░░██
//       ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░██░░░░██░░░░░░░░░░░░░░░░░░░░██
//      ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//       ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//     ░░▓▓▒▒░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//     ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//      ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//     ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██
//   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░father░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//   ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░i hunger░░░░░░░░░░░░░░░░░░░░░░░░░░██    
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██    
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██    
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██    
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██    
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//  ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//   ░░▓▓▓▓░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██  
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██░░  
//     ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██    
//      ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██      
//    ▓▓██░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██        
//      ▓▓████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░██          
//        ▓▓▓▓████████░░░░░░░░░░░░░░░░░░░░░░░░████████░░          
//        ░░░░▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░    