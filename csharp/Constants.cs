using UnityEngine;

public static class Constants
{
    public const string DUCK_ENTER = "DUCK_ENTER";
    public const string DUCK_EXIT = "DUCK_EXIT";

    public static readonly Vector3 North = new Vector3(0, 0, 1);
    public static readonly Vector3 South = new Vector3(0, 0, -1);
    public static readonly Vector3 East = new Vector3(1, 0, 0);
    public static readonly Vector3 West = new Vector3(-1, 0, 0);

    public static readonly Vector3 NorthEast = (North + East).normalized;
    public static readonly Vector3 NorthWest = (North + West).normalized;
    public static readonly Vector3 SouthEast = (South + East).normalized;
    public static readonly Vector3 SouthWest = (South + West).normalized;

    public static readonly Vector3[] CardinalDirections = { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };

    public static float TRANSPARENT_COLOR = 0.5f;
}
