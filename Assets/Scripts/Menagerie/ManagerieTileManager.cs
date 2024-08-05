using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using System;

public class ManagerieTileManager : MonoBehaviour
{
    private int[,] grid;
    private Vector2Int currentIndex = new Vector2Int(0, 0); // Starting index

    public int gridXAmount;
    public int gridYAmount;


    public static ManagerieTileManager instance { get; private set; }
    public enum MoveDirection { UP, DOWN, RIGHT, LEFT };

    private CinemachineVirtualCamera vCam;

    public GameObject prefabToInstantiate; // Reference to your 3D prefab
    private GameObject[,] spawnedObjects; // Array to store references to instantiated GameObjects

    public float gapX = 1f; // Gap between each GameObject
    public float gapY = 1f;

    public Vector2Int center;

    public float overviewMapCameraDistance = 115f;
    public float gameplayCameraDistance = 20f;

    public Image selectionOutlineImage;
    public float selectionOutlineOverviewSize;
    public float selectionOutlineGameplaySize;
    public float targetCameraDistance;
    public float cameraSizeChangeSpeed;
    public float targetOutlineSize;
    public float outlineSizeSpeed;

    public TMP_Text roomTypeText;
    public GameObject openOverviewScreenGroup;


    public enum ManagerieState { GAMEPLAY, OVERVIEW };
    public ManagerieState currentState;

    public float spawnHeight;

    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found a Battle State Manager object, destroying new one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        targetCameraDistance = gameplayCameraDistance;
        vCam = GlobalDataStore.instance.cinCam;
        grid = new int[gridXAmount, gridYAmount];

        InitializeGrid();

        currentState = ManagerieState.GAMEPLAY;
    }

    // Update is called once per frame
    void Update()
    {
        vCam.m_Lens.OrthographicSize = Mathf.Lerp(vCam.m_Lens.OrthographicSize, targetCameraDistance, Time.deltaTime * cameraSizeChangeSpeed); // Set your desired orthographic size here

        if(selectionOutlineImage.gameObject.activeSelf) selectionOutlineImage.rectTransform.sizeDelta = Vector2.Lerp(selectionOutlineImage.rectTransform.sizeDelta,
                                                                            new Vector2(targetOutlineSize, targetOutlineSize),
                                                                            Time.deltaTime * outlineSizeSpeed);
    }

    // Function to get all adjacent tiles
    public void FocusSelectedTile(int x, int y, bool shouldFocus)
    {
        int[] adjacentTiles = new int[8];
        int index = 0;

        // Check all 8 possible directions (4 cardinal and 4 diagonal)
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // Skip the center point itself

                int nx = x + dx;
                int ny = y + dy;

                // Check if (nx, ny) is within bounds
                if (nx >= 0 && nx < grid.GetLength(0) &&
                    ny >= 0 && ny < grid.GetLength(1))
                {
                    adjacentTiles[index++] = grid[nx, ny];

                    spawnedObjects[nx, ny].gameObject.SetActive(shouldFocus);
                }
            }
        }
    }

    void InitializeGrid()
    {
        spawnedObjects = new GameObject[grid.GetLength(0), grid.GetLength(1)];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // Calculate position with gap
                Vector3 position = new Vector3((37 * i) * gapX, 0f, (47 * j) * gapY);

                // Instantiate prefab
                GameObject spawnedPrefab = Instantiate(prefabToInstantiate, position, Quaternion.identity);

                // Set the parent of the instantiated prefab to be this GameObject (ArrayWithGameObjects)
                spawnedPrefab.transform.parent = transform;

                // Store reference to instantiated prefab in the array
                spawnedObjects[i, j] = spawnedPrefab;

                spawnedPrefab.name = $"Tile X: {i} | Y: {j}";

                spawnedPrefab.GetComponent<TileData>().DisableTile();

            }
        }


        // Set the initial selected index to the center of the array
        center = new Vector2Int(grid.GetLength(0) / 2, grid.GetLength(1) / 2);
        currentIndex = center;

        InitiateGameTile();
    }


    // Example function to get GameObject reference using array indices
    public GameObject GetGameObjectAtPosition(int x, int y)
    {
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return spawnedObjects[x, y];
        }
        else
        {
            Debug.LogWarning($"Invalid indices: ({x}, {y})");
            return null;
        }
    }


    public GameObject GetGameObjectAtPosition(Vector2Int position)
    {
        return GetGameObjectAtPosition(position.x, position.y);
    }

    public TileData GetSelectedTile()
    {
        return GetGameObjectAtPosition(currentIndex.x, currentIndex.y).GetComponent<TileData>();
    }

    public void MoveToCenter()
    {
        MoveToIndex(center.x, center.y);
    }

    public void InitiateGameTile()
    {
        MoveToCenter();
        GetAdjacentTile(Direction.Left).EnableTile();
        GetAdjacentTile(Direction.Bottom).EnableTile();
        GetAdjacentTile(Direction.Top).EnableTile();
        GetAdjacentTile(Direction.Right).EnableTile();
        GetSelectedTile().EnableTile();
        GetSelectedTile().tileType = TileData.TileType.START;
        GetSelectedTile().SpawnPlayer();
    }

    public void SwitchToOverviewMap()
    {
        if (currentState == ManagerieState.OVERVIEW) return;
        AudioManager.instance.PlaySound(AudioLibrary.OpenOverview, transform.position);
        openOverviewScreenGroup.SetActive(false);
        targetCameraDistance = overviewMapCameraDistance;
        selectionOutlineImage.gameObject.SetActive(true);
        targetOutlineSize = selectionOutlineOverviewSize;
        currentState = ManagerieState.OVERVIEW;
        //roomTypeText.gameObject.SetActive(true);
        FocusSelectedTile(currentIndex.x, currentIndex.y, true);
    }


    public void SwitchToGameplayMap()
    {
        if (currentState == ManagerieState.GAMEPLAY) return;

        TileData tileData = GetSelectedTile();
        if (!tileData.isActive)
        {
            AudioManager.instance.PlaySound(AudioLibrary.Error, transform.position);
            return;
        }

        AudioManager.instance.PlaySound(AudioLibrary.SelectTile, transform.position);
        targetCameraDistance = gameplayCameraDistance;
        selectionOutlineImage.rectTransform.sizeDelta = new Vector2(0, 0);
        selectionOutlineImage.gameObject.SetActive(false);
        openOverviewScreenGroup.SetActive(true);
        currentState = ManagerieState.GAMEPLAY;
        //roomTypeText.gameObject.SetActive(false);
        if(!tileData.PlayerOnTile) tileData.SpawnPlayer();
        FocusSelectedTile(currentIndex.x, currentIndex.y, false);
    }


    // Function to move the current index to the specified position
    void MoveToIndex(int x, int y)
    {
        if (IsValidIndex(x, y))
        {
            currentIndex.x = x;
            currentIndex.y = y;


            UpdateCamera();

            Debug.Log($"Moved current index to ({currentIndex.x}, {currentIndex.y})");

            // Optionally, you can perform additional actions or update visuals based on the new index
        }
        else
        {
            Debug.LogWarning($"Invalid index: ({x}, {y})");
        }
    }

    // Helper function to check if the given indices are valid within the grid bounds
    bool IsValidIndex(int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }


    public void MoveSelection(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.UP:
                if (currentIndex.y < grid.GetLength(1) - 1) // Check bounds
                {
                    currentIndex.y++;
                    AudioManager.instance.PlaySound(AudioLibrary.Move, transform.position);
                }
                break;
            case MoveDirection.DOWN:
                if (currentIndex.y > 0) // Check bounds
                {
                    currentIndex.y--;
                    AudioManager.instance.PlaySound(AudioLibrary.Move, transform.position);
                }
                break;
            case MoveDirection.RIGHT:
                if (currentIndex.x < grid.GetLength(0) - 1) // Check bounds
                {
                    currentIndex.x++;
                    AudioManager.instance.PlaySound(AudioLibrary.Move, transform.position);
                }
                break;
            case MoveDirection.LEFT:
                if (currentIndex.x > 0) // Check bounds
                {
                    currentIndex.x--;
                    AudioManager.instance.PlaySound(AudioLibrary.Move, transform.position);
                }
                break;
        }

        UpdateCamera();
    }

    public void UpdateCamera()
    {
        vCam.Follow = GetSelectedTile().transform;
        roomTypeText.text = GetSelectedTile().tileType.ToString();
    }

    // Function to get adjacent GameObject based on direction
    public TileData GetAdjacentTile(Direction direction)
    {
        int newX = currentIndex.x;
        int newY = currentIndex.y;

        switch (direction)
        {
            case Direction.Left:
                newX--;
                break;
            case Direction.Right:
                newX++;
                break;
            case Direction.Top:
                newY++;
                break;
            case Direction.Bottom:
                newY--;
                break;
        }

        // Check if new indices are within bounds
        if (IsValidIndex(newX, newY))
        {
            return spawnedObjects[newX, newY].GetComponent<TileData>();
        }
        else
        {
            Debug.LogWarning($"No GameObject found in direction {direction} from ({currentIndex.x}, {currentIndex.y})");
            return null;
        }
    }
}
