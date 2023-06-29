public class KlondikeSolitaireVars
{
    public enum GameType {
        Draw1,
        Draw3,
        NULL
    }

    private static GameType type;

    public static GameType Type {
        get {
            return type;
        }
        set {
            type = value;
        }
    }
}
