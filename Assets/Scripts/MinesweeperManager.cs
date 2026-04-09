using UnityEngine;
using System.Collections.Generic;

public class MinesweeperManager : MonoBehaviour
{
    // move win/lose logic

    [Header("Grid Variables")]
    [SerializeField] Transform gridTransform;
    [SerializeField] int gridWidth;
    [SerializeField] int gridHeight;
    [SerializeField] int mineCount;

    [Header("Tile Variables")]
    [SerializeField] float tileSize = 1.0f;
    [SerializeField] Vector2 gridOffset;

    private Tile[,] grid;
    private bool gameOver = false;
    private int revealedCount = 0;

    public static MinesweeperManager instance; 

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        grid = new Tile[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 position = new Vector2(
                    x * tileSize + gridOffset.x,
                    y * tileSize + gridOffset.y
                );

                GameObject tilePrefab = Resources.Load<GameObject>("tiles/TilePrefab");
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, gridTransform);
                tileObject.name = $"Tile_{x}_{y}";

                Tile tile = tileObject.GetComponent<Tile>();
                if (tile == null)
                {
                    tile = tileObject.AddComponent<Tile>();
                }

                tile.Initialize(x, y);
                grid[x, y] = tile;
            }
        }
    }

    private void PlaceMines(int first_x, int first_y)
    {
        int minesPlaced = 0;
        System.Random random = new System.Random();

        while (minesPlaced < mineCount)
        {
            int x = random.Next(gridWidth);
            int y = random.Next(gridHeight);

            bool placeClear = false;
            if (!grid[x, y].HasMine && !grid[x, y].IsRevealed 
                && ReturnMinePlacementValid(x, y, first_x, first_y)) placeClear = true;

            if (placeClear)
            {
                grid[x, y].HasMine = true;
                minesPlaced++;
            }
        }
    }

    private bool ReturnMinePlacementValid(int x, int y, int first_x, int first_y)
    { // returns true if not adjesant to first revealed tile. Used in PlaceMines()
        bool returnValue = false;

        int calc_x = Mathf.Abs(x - first_x);
        int calc_y = Mathf.Abs(y - first_y);

        if (calc_x > 1 || calc_y > 1) returnValue = true;
        return returnValue;
    }

    private void CalculateAdjacentNumbers()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!grid[x, y].HasMine)
                {
                    int adjacentMines = CountAdjacentMines(x, y);
                    grid[x, y].AdjacentMines = adjacentMines;
                }
            }
        }
    }

    private int CountAdjacentMines(int x, int y)
    {
        int count = 0;

        for (int offset_x = -1; offset_x <= 1; offset_x++)
        {
            for (int offset_y = -1; offset_y <= 1; offset_y++)
            {
                if (offset_x == 0 && offset_y == 0) continue; // ignore middle tile

                int calc_x = x + offset_x;
                int calc_y = y + offset_y;

                if (calc_x >= 0 && calc_x < gridWidth && calc_y >= 0 && calc_y < gridHeight)
                    if (grid[calc_x, calc_y].HasMine) count++;
            }
        }

        return count;
    }

    public void RevealTile(int x, int y)
    {
        if (gameOver || grid[x, y].IsRevealed || grid[x, y].IsFlagged) return;

        grid[x, y].Reveal();
        revealedCount++;

        if (revealedCount <= 1)
        {
            PlaceMines(x, y);
            CalculateAdjacentNumbers();
        }

        if (grid[x, y].HasMine) // lose
        {
            Debug.Log("boo womp");
            RevealAllMines();
            grid[x, y].SetSprite(Resources.Load<Sprite>("Sprites/TileExploded"));
            gameOver = true;
            return;
        }

        if (grid[x, y].AdjacentMines == 0)
        {
            RevealAdjacentTiles(x, y);
        }

        int totalSafeTiles = gridWidth * gridHeight - mineCount;
        if (revealedCount >= totalSafeTiles) // win
        {
            Debug.Log("le win");
            RevealAllTiles();
            gameOver = true;
        }
    }

    private void RevealAdjacentTiles(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int newX = x + dx;
                int newY = y + dy;

                if (newX >= 0 && newX < gridWidth && newY >= 0 && newY < gridHeight)
                {
                    if (!grid[newX, newY].IsRevealed && !grid[newX, newY].HasMine)
                    {
                        RevealTile(newX, newY);
                    }
                }
            }
        }
    }

    private void RevealAllMines()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].HasMine && !grid[x, y].IsRevealed)
                {
                    grid[x, y].Reveal();
                }
            }
        }
    }

    private void RevealAllTiles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (!grid[x, y].IsRevealed)
                {
                    grid[x, y].Reveal();
                }
            }
        }
    }
}