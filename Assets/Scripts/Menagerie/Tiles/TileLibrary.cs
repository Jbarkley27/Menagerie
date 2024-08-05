using UnityEngine;
using System.Collections.Generic;

public class TileLibrary : MonoBehaviour
{
	public List<GameObject> tileCollection = new List<GameObject>();

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

	public GameObject GetRandomTile()
	{
		return tileCollection[Random.Range(0, tileCollection.Count)];
	}
}

