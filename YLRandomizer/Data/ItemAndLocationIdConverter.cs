using YLRandomizer.Logging;

namespace YLRandomizer.Data
{
    public class ItemAndLocationIdConverter
    {
        public static PlayerMoves.Moves GetMoveFromDevName(string devName)
        {
            switch (devName)
            {
                case "Invisibility": // Camo Cloak
                    return PlayerMoves.Moves.Invisibility;
                case "Sonar Shot": // Sonar Shot
                    return PlayerMoves.Moves.SonarShot;
                case "Sonar Boom": // Sonar 'Splosion
                    return PlayerMoves.Moves.SonarBoom;
                case "Sonar Shield": // Sonar Shield
                    return PlayerMoves.Moves.SonarShield;
                case "Wheel Roll": // Reptile Roll
                    return PlayerMoves.Moves.WheelRoll;
                case "Eat Mk 1": // Slurp Shot
                    return PlayerMoves.Moves.EatMk1;
                case "Eat Mk 2": // Slurp State
                    return PlayerMoves.Moves.EatMk2;
                case "Wheel Spin Attack": // Reptile Rush
                    return PlayerMoves.Moves.WheelSpinAttack;
                case "Glide (aka FlapFloat)": // Glide
                    return PlayerMoves.Moves.Glide;
                case "Fly": // Flappy Flight
                    return PlayerMoves.Moves.Fly;
                case "Ground Pound": // Buddy Slam
                    return PlayerMoves.Moves.GroundPound;
                case "High Jump (aka CrouchJump)": // Lizard Leap
                    return PlayerMoves.Moves.HighJump;
                case "Fart Bubble": // Buddy Bubble
                    return PlayerMoves.Moves.FartBubble;
                case "Tongue Grapple Hook": // Lizard Lash
                    return PlayerMoves.Moves.TongueGrappleHook;
                case "Basic Attack (Ground)": // Tail Twirl (TODO: Also BasicAttackAir, is this used for air Tail Twirl?)
                    return PlayerMoves.Moves.BasicAttack;
                default: // TODO: Meaningful default?
                    ManualSingleton<ILogger>.instance.Warning("PlayerMoveConverter.GetMoveFromDevName(): Unknown move: " + devName);
                    return PlayerMoves.Moves.BasicAttack;
            }
        }

        public static PlayerMoves.Moves GetMoveFromLocationId(long locationId)
        {
            switch (locationId)
            {
                case Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START:
                    return PlayerMoves.Moves.BasicAttack;
                case Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 1:
                    return PlayerMoves.Moves.WheelRoll;
                case Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 2:
                    return PlayerMoves.Moves.Glide;
                case Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 3:
                    return PlayerMoves.Moves.FartBubble;
                case Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 4:
                    return PlayerMoves.Moves.Invisibility;
                case Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 5:
                    return PlayerMoves.Moves.Fly;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START:
                    return PlayerMoves.Moves.SonarShot;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 1:
                    return PlayerMoves.Moves.EatMk1;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 2:
                    return PlayerMoves.Moves.GroundPound;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 3:
                    return PlayerMoves.Moves.EatMk2;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 4:
                    return PlayerMoves.Moves.HighJump;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 5:
                    return PlayerMoves.Moves.TongueGrappleHook;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 6:
                    return PlayerMoves.Moves.SonarBoom;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 7:
                    return PlayerMoves.Moves.WheelSpinAttack;
                case Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 8:
                    return PlayerMoves.Moves.SonarShield;
                default:
                    ManualSingleton<ILogger>.instance.Warning("PlayerMoveConverter.GetMoveFromLocationId(): Unknown move: " + locationId);
                    return PlayerMoves.Moves.BasicAttack;
            }
        }

        public static long GetLocationIdFromMove(PlayerMoves.Moves move)
        {
            switch (move)
            {
                case PlayerMoves.Moves.BasicAttack:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START;
                case PlayerMoves.Moves.WheelRoll:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 1;
                case PlayerMoves.Moves.Glide:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 2;
                case PlayerMoves.Moves.FartBubble:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 3;
                case PlayerMoves.Moves.Invisibility:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 4;
                case PlayerMoves.Moves.Fly:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 5;
                case PlayerMoves.Moves.SonarShot:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START;
                case PlayerMoves.Moves.EatMk1:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 1;
                case PlayerMoves.Moves.GroundPound:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 2;
                case PlayerMoves.Moves.EatMk2:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 3;
                case PlayerMoves.Moves.HighJump:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 4;
                case PlayerMoves.Moves.TongueGrappleHook:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 5;
                case PlayerMoves.Moves.SonarBoom:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 6;
                case PlayerMoves.Moves.WheelSpinAttack:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 7;
                case PlayerMoves.Moves.SonarShield:
                    return Constants.TROWSER_PAID_ABILITY_LOCATION_ID_START + 8;
                default:
                    ManualSingleton<ILogger>.instance.Warning("PlayerMoveConverter.GetLocationIdFromMove(): Unknown move: " + move);
                    return -1;
            }
        }

        public static PlayerMoves.Moves GetMoveFromItemId(long itemId)
        {
            switch (itemId)
            {
                case Constants.ABILITY_ITEM_ID_START:
                    return PlayerMoves.Moves.BasicAttack;
                case Constants.ABILITY_ITEM_ID_START + 1:
                    return PlayerMoves.Moves.WheelRoll;
                case Constants.ABILITY_ITEM_ID_START + 2:
                    return PlayerMoves.Moves.Glide;
                case Constants.ABILITY_ITEM_ID_START + 3:
                    return PlayerMoves.Moves.FartBubble;
                case Constants.ABILITY_ITEM_ID_START + 4:
                    return PlayerMoves.Moves.Invisibility;
                case Constants.ABILITY_ITEM_ID_START + 5:
                    return PlayerMoves.Moves.Fly;
                case Constants.ABILITY_ITEM_ID_START + 6:
                    return PlayerMoves.Moves.SonarShot;
                case Constants.ABILITY_ITEM_ID_START + 7:
                    return PlayerMoves.Moves.EatMk1;
                case Constants.ABILITY_ITEM_ID_START + 8:
                    return PlayerMoves.Moves.GroundPound;
                case Constants.ABILITY_ITEM_ID_START + 9:
                    return PlayerMoves.Moves.EatMk2;
                case Constants.ABILITY_ITEM_ID_START + 10:
                    return PlayerMoves.Moves.HighJump;
                case Constants.ABILITY_ITEM_ID_START + 11:
                    return PlayerMoves.Moves.TongueGrappleHook;
                case Constants.ABILITY_ITEM_ID_START + 12:
                    return PlayerMoves.Moves.SonarBoom;
                case Constants.ABILITY_ITEM_ID_START + 13:
                    return PlayerMoves.Moves.WheelSpinAttack;
                case Constants.ABILITY_ITEM_ID_START + 14:
                    return PlayerMoves.Moves.SonarShield;
                default:
                    ManualSingleton<ILogger>.instance.Warning("PlayerMoveConverter.GetMoveFromItemId(): Unknown move: " + itemId);
                    return PlayerMoves.Moves.BasicAttack;
            }
        }

        public static long GetItemIdFromMove(PlayerMoves.Moves move)
        {
            switch (move)
            {
                case PlayerMoves.Moves.BasicAttack:
                    return Constants.ABILITY_ITEM_ID_START;
                case PlayerMoves.Moves.WheelRoll:
                    return Constants.ABILITY_ITEM_ID_START + 1;
                case PlayerMoves.Moves.Glide:
                    return Constants.ABILITY_ITEM_ID_START + 2;
                case PlayerMoves.Moves.FartBubble:
                    return Constants.ABILITY_ITEM_ID_START + 3;
                case PlayerMoves.Moves.Invisibility:
                    return Constants.ABILITY_ITEM_ID_START + 4;
                case PlayerMoves.Moves.Fly:
                    return Constants.ABILITY_ITEM_ID_START + 5;
                case PlayerMoves.Moves.SonarShot:
                    return Constants.ABILITY_ITEM_ID_START + 6;
                case PlayerMoves.Moves.EatMk1:
                    return Constants.ABILITY_ITEM_ID_START + 7;
                case PlayerMoves.Moves.GroundPound:
                    return Constants.ABILITY_ITEM_ID_START + 8;
                case PlayerMoves.Moves.EatMk2:
                    return Constants.ABILITY_ITEM_ID_START + 9;
                case PlayerMoves.Moves.HighJump:
                    return Constants.ABILITY_ITEM_ID_START + 10;
                case PlayerMoves.Moves.TongueGrappleHook:
                    return Constants.ABILITY_ITEM_ID_START + 11;
                case PlayerMoves.Moves.SonarBoom:
                    return Constants.ABILITY_ITEM_ID_START + 12;
                case PlayerMoves.Moves.WheelSpinAttack:
                    return Constants.ABILITY_ITEM_ID_START + 13;
                case PlayerMoves.Moves.SonarShield:
                    return Constants.ABILITY_ITEM_ID_START + 14;
                default:
                    ManualSingleton<ILogger>.instance.Warning("PlayerMoveConverter.GetItemIdFromMove(): Unknown move: " + move);
                    return -1;
            }
        }

        public static PlayerMoves.Moves GetRequiredAbilityForWorldId(int worldId)
        {
            // World Info
            // 0 = Nothing
            // 1 = Hivory Towers
            // 2 = Tribalstack Tropics
            // 3 = Galleon Galaxy
            // 4 = Capital Cashino
            // 5 = Glitterglaze Glacier
            // 6 = Moodymaze Marsh
            // 7 = Nothing
            switch (worldId)
            {
                case 2:
                    return PlayerMoves.Moves.WheelRoll;
                case 5:
                    return PlayerMoves.Moves.Glide;
                case 6:
                    return PlayerMoves.Moves.FartBubble;
                case 4:
                    return PlayerMoves.Moves.Invisibility;
                case 3:
                    return PlayerMoves.Moves.Fly;
                default:
                    return PlayerMoves.Moves.Fly;
            }
        }

        public static long GetLocationIdFromEGameStat(EGameStats stat)
        {
            switch (stat)
            {
                case EGameStats.PagiesCollectedGlacier:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 2; // Glide
                case EGameStats.PagiesCollectedSwamp:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 3; // Buddy Bubble
                case EGameStats.PagiesCollectedCasino:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 4; // Camo Cloak
                case EGameStats.PagiesCollectedSpace:
                    return Constants.TROWSER_FREE_ABILITY_LOCATION_ID_START + 5; // Flappy Flight
                default:
                    return -1;
            }
        }
    }
}
