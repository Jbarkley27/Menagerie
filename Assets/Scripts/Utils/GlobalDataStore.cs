using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class GlobalDataStore : MonoBehaviour
{
    public static GlobalDataStore instance { get; private set; }
    public Image selectionOutlineImage;
    public TMP_Text selectedTileTypeText;

    [Header("Cameras")]
    public CinemachineVirtualCamera cinCam;


    [Header("Player")]
    public GameObject player;



    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found an GlobalDataReference object, destroying new one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

