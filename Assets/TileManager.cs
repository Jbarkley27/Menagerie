using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileData;
using Random = UnityEngine.Random;

// HANDLES ALL METHODS AROUND THE MENAGERIE TILES - INITIALIZATION, INDEX FUNCTIONS
public class TileManager : MonoBehaviour
{
    [Header("Grid")]
    private TileData[,] grid;
    private Vector2Int currentIndex = new Vector2Int(0, 0); // Starting index

    public int gridXAmount;
    public int gridYAmount;

    public float gapX = 1f;
    public float gapY = 1f;

    public Vector2Int center;

    public static TileManager instance { get; private set; }

    public enum MoveDirection { UP, DOWN, RIGHT, LEFT };
    public enum Direction
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [Header("Needed Modules")]
    private TileLibrary tileLibrary;
    private UIManager uiManager;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found a Tile Manager object, destroying new one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        tileLibrary = GetComponent<TileLibrary>();
        uiManager = GetComponent<UIManager>();
        

        InitializeGrid();
    }



    // INITIALIZATION ----------------------------------------------------------
    void InitializeGrid()
    {
        grid = new TileData[gridXAmount, gridYAmount];
        

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                // Calculate position with gap
                Vector3 position = new Vector3((37 * i) * gapX, 0f, (47 * j) * gapY);

                // Instantiate prefab
                GameObject spawnedPrefab = Instantiate(tileLibrary.GetRandomTile(), position, Quaternion.identity);

                grid[i, j] = spawnedPrefab.GetComponent<TileData>();

                spawnedPrefab.transform.parent = transform;

                spawnedPrefab.GetComponent<TileData>().AssignType();

                spawnedPrefab.name = $"Tile X: {i} | Y: {j}";

                spawnedPrefab.GetComponent<TileData>().DisableTile();
            }
        }

        center = new Vector2Int(grid.GetLength(0) / 2, grid.GetLength(1) / 2);

        Utils.Log("Created Grid - Center is " + center);

        currentIndex = center;

        GetSelectedTile().EnableTile();

        grid[currentIndex.x, currentIndex.y].tileType = TileType.START;

        Utils.Log("Current Index is " + currentIndex);

        

        InitiateGameTile();
    }



    public void InitiateGameTile()
    {
        Utils.Log("Initiating Game State");
        UpdateUI();
        TileData selectedTile = GetSelectedTile();
        EnableAdjacentTiles();
        selectedTile.tileType = TileData.TileType.START;
        uiManager.Initialize();
    }




    // TILE HELPER FUNCTIONS

    public void EnableAdjacentTiles()
    {
        GetAdjacentTile(Direction.Left).EnableTile();
        GetAdjacentTile(Direction.Bottom).EnableTile();
        GetAdjacentTile(Direction.Top).EnableTile();
        GetAdjacentTile(Direction.Right).EnableTile();
    }



    public TileData[,] GetGrid()
    {
        return grid;
    }



    public Vector2Int GetCurrentTileIndex()
    {
        return currentIndex;
    }



    // Moves the selection to the center tile of the grid
    public void MoveToCenter()
    {
        MoveToIndex(center.x, center.y);
    }



    public TileData GetTileAtPosition(int x, int y)
    {
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        else
        {
            Debug.LogWarning($"Invalid indices: ({x}, {y})");
            return null;
        }
    }




    public TileData GetSelectedTile()
    {
        return GetTileAtPosition(currentIndex.x, currentIndex.y);
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

        UpdateUI();
    }



    // Move the current index to the specified position
    void MoveToIndex(int x, int y)
    {
        if (IsValidIndex(x, y))
        {
            currentIndex.x = x;
            currentIndex.y = y;


            // Update UI
            UpdateUI();


            Debug.Log($"Moved current index to ({currentIndex.x}, {currentIndex.y})");
        }
        else
        {
            Debug.LogWarning($"Invalid index: ({x}, {y})");
        }
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
            return grid[newX, newY];
        }
        else
        {
            Debug.LogWarning($"No GameObject found in direction {direction} from ({currentIndex.x}, {currentIndex.y})");
            return null;
        }
    }



    // Function to get all adjacent tiles
    public void FocusSelectedTile(int x, int y, bool shouldFocus)
    {
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
                    grid[nx, ny].gameObject.SetActive(shouldFocus);
                }
            }
        }

    }



    // Helper function to check if the given indices are valid within the grid bounds
    bool IsValidIndex(int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }



    public void UpdateUI()
    {
        uiManager.UpdateUI();
    }
}
