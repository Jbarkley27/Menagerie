using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileData : MonoBehaviour
{
    public GameObject activeTile;
    public GameObject disabledTile;
    public bool isActive;
    public enum TileType { START = 0, BOSS = 1, COMBAT = 2, MYSTERY = 3, RITUAL = 4, SHOP = 5, NPC = 6, TREASURE = 7, LOCKED_ROOM = 8, MINI_GAME = 9 };
    public TileType tileType;
    public Transform SpawnPoint;
    public bool PlayerOnTile = false;
    public bool Locked;

    void Start()
    {

    }

    public void EnableTile()
    {
        disabledTile.SetActive(false);
        activeTile.SetActive(true);
        isActive = true;
    }

    public void DisableTile()
    {
        activeTile.SetActive(false);
        isActive = false;
        disabledTile.SetActive(true);
    }

    public void SpawnPlayer()
    {
        if (GlobalDataStore.instance.player == null) Utils.Log("Can't Spawn Player, Global Instance Is NULL");
        StartCoroutine(Spawn());
    }


    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0);
        GlobalDataStore.instance.player.GetComponent<PlayerMovementModule>().Transfer(
            new Vector3(gameObject.transform.position.x,
                        5f,
                        gameObject.transform.position.z)
        );
    }

    public void UnlockTile()
    {
        Locked = false;
    }

    public void AssignType()
    {
        TileType randomType = (TileType)Random.Range(1, Enum.GetValues(typeof(TileType)).Length);
        tileType = randomType;

        Locked = true;
    }
}
