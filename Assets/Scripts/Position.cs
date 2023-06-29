using UnityEngine;

public struct Position {
    public Position(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public Position(Vector2Int position) {
        this.x = position.x;
        this.y = position.y;
    }

    public static bool operator ==(Position first, Position second) {
        return first.x == second.x && first.y == second.y;
    }

    public static bool operator !=(Position first, Position second) {
        return !(first == second);
    }

    public static Position operator +(Position first, Vector2Int second) {
        return new Position(first.x + second.x, first.y + second.y);
    }

    public static Position operator -(Position first, Vector2Int second) {
        return new Position(first.x - second.x, first.y - second.y);
    }

    public static Position operator +(Position first, Position second) {
        return new Position(first.x + second.x, first.y + second.y);
    }

    public static Position operator -(Position first, Position second) {
        return new Position(first.x - second.x, first.y - second.y);
    }

    public int x, y;
}