using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;

    public float overviewMapCameraDistance = 115f;
    public float gameplayCameraDistance = 20f;

    public Image overviewGroup;
    public float overviewOutlineSize;
    private float targetCameraDistance;
    public float cameraSizeChangeSpeed;
    private float targetOutlineSize;
    public float outlineSizeSpeed;

    public TMP_Text roomTypeText;
    public GameObject openOverviewScreenGroup;


    public enum ManagerieState { GAMEPLAY, OVERVIEW };
    public ManagerieState currentState;

    public Vector2Int CurrentIndex;

    [Header("Needed Modules")]
    public TileManager tileManager;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CurrentIndex = tileManager.GetCurrentTileIndex();

        vCam.m_Lens.OrthographicSize = Mathf.Lerp(vCam.m_Lens.OrthographicSize, targetCameraDistance, Time.deltaTime * cameraSizeChangeSpeed); // Set your desired orthographic size here

        if (overviewGroup.gameObject.activeSelf) overviewGroup.rectTransform.sizeDelta = Vector2.Lerp(overviewGroup.rectTransform.sizeDelta,
                                                                            new Vector2(targetOutlineSize, targetOutlineSize),
                                                                            Time.deltaTime * outlineSizeSpeed);

        roomTypeText.text = tileManager.GetSelectedTile().tileType.ToString();
    }

    public void Initialize()
    {
        switch(currentState)
        {
            case ManagerieState.GAMEPLAY:
                SwitchToGameplayMap(true);
                break;
            case ManagerieState.OVERVIEW:
                SwitchToOverviewMap(true);
                break;
        }

        TileData tileData = tileManager.GetSelectedTile();
        tileData.tileType = TileData.TileType.START;
    }

    public void UpdateUI()
    {
        Utils.Log("Updating UI");
        UpdateCamera();
    }

    public void SwitchToOverviewMap(bool initialize = false)
    {
        if (currentState == ManagerieState.OVERVIEW && !initialize) return;
        if (tileManager.GetSelectedTile().Locked)
        {
            Utils.Log("Tile Still Locked, Complete the Objective");
            return;
        }

        AudioManager.instance.PlaySound(AudioLibrary.OpenOverview, transform.position);
        openOverviewScreenGroup.SetActive(false);
        targetCameraDistance = overviewMapCameraDistance;
        overviewGroup.gameObject.SetActive(true);
        targetOutlineSize = overviewOutlineSize;
        currentState = ManagerieState.OVERVIEW;
        if (!initialize) tileManager.FocusSelectedTile(CurrentIndex.x, CurrentIndex.y, true);
    }

    public bool CanSwitchToOverview()
    {
        return TileManager.instance.GetSelectedTile().isActive;
    }

    public void SwitchToGameplayMap(bool initialize = false)
    {
        if (currentState == ManagerieState.GAMEPLAY && !initialize) return;

        TileData tileData = tileManager.GetSelectedTile();
        if (!tileData.isActive)
        {
            AudioManager.instance.PlaySound(AudioLibrary.Error, transform.position);
            return;
        }

        AudioManager.instance.PlaySound(AudioLibrary.SelectTile, transform.position);
        targetCameraDistance = gameplayCameraDistance;
        overviewGroup.rectTransform.sizeDelta = new Vector2(0, 0);
        overviewGroup.gameObject.SetActive(false);
        openOverviewScreenGroup.SetActive(true);
        currentState = ManagerieState.GAMEPLAY;
        if(!tileData.PlayerOnTile) tileData.SpawnPlayer();
        //tileManager.FocusSelectedTile(tileManager.GetCurrentTileIndex().x, tileManager.GetCurrentTileIndex().y, false);
    }

  
    

    public void UpdateCamera()
    {
        if (vCam == null) vCam = GlobalDataStore.instance.cinCam;
        vCam.Follow = tileManager.GetSelectedTile().transform;
        roomTypeText.text = tileManager.GetSelectedTile().tileType.ToString();
    }

}
