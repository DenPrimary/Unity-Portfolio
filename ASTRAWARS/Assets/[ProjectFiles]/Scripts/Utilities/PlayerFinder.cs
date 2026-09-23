using UnityEngine;

public class PlayerFinder
{
    private static Transform cachedPlayer;

    public static Transform Player
    {
        get {
            if (cachedPlayer == null) {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    cachedPlayer = player.transform;
            }
            return cachedPlayer;
        }
    }

    public static void Reset() { 
        cachedPlayer = null;
    }
}
