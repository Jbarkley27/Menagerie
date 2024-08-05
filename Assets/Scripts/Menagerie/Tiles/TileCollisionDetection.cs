using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCollisionDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("TileCollision"))
        {
            Utils.Log("In Area");

            TileData tileData;

            Utils.Log(collision.transform.parent.parent.gameObject.name);
            collision.transform.parent.parent.gameObject.TryGetComponent(out tileData);

            if (tileData) tileData.PlayerOnTile = true;


            //PlayerOnTile = true;
        }
    }


    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("TileCollision"))
        {
            Utils.Log("Left Area");

            TileData tileData;

            collision.transform.parent.parent.gameObject.TryGetComponent(out tileData);

            if (tileData) tileData.PlayerOnTile = false;

            //PlayerOnTile = false;
        }
    }
}
