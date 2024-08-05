using UnityEngine;

public class PlayerSpawnModule : MonoBehaviour
{
    [Header("Needed Modules")]
    public TileManager tileManager;


    public void SpawnPlayerAtTile(TileData tileData)
    {
        tileData.SpawnPlayer();
    }
}
