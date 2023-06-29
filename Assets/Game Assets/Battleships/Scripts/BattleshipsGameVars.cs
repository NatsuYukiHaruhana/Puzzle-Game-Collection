public class BattleshipsGameVars
{
    public enum GameType {
        AgainstHumanLocal,
        AgainstHumanOnline,
        AgainstCPU,
        NULL
    }

    private static GameType type = GameType.AgainstCPU;

    public static GameType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }
}
