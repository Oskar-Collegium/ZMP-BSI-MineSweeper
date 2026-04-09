using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int x, y;

    [Header("Tile Sprites")]
    [SerializeField] Sprite hiddenTile;
    [SerializeField] Sprite flagSprite;
    [SerializeField] Sprite clickedMineSprite;
    [SerializeField] Sprite mineSprite;
    [SerializeField] Sprite emptySprite;
    [SerializeField] List<Sprite> numberSprites;

    // Tile information
    [HideInInspector] public bool HasMine = false;
    [HideInInspector] public int AdjacentMines = 0;
    [HideInInspector] public bool IsRevealed = false;
    [HideInInspector] public bool IsFlagged = false;

    // Reference Variables
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsRevealed && !IsFlagged) MinesweeperManager.instance.RevealTile(x, y);
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!IsRevealed)
            {
                if (IsFlagged)
                {
                    IsFlagged = false;
                    spriteRenderer.sprite = hiddenTile;
                }
                else
                {
                    IsFlagged = true;
                    spriteRenderer.sprite = flagSprite;
                }
            }
        }
    }

    public void Initialize(int gridX, int gridY)
    {
        x = gridX;
        y = gridY;
    }

    public void Reveal()
    {
        if (IsRevealed) return;
        IsRevealed = true;

        if (HasMine)
        {
            spriteRenderer.sprite = mineSprite;
        }
        else if (AdjacentMines > 0)
        {
            spriteRenderer.sprite = numberSprites[AdjacentMines - 1];
        }
        else
        {
            spriteRenderer.sprite = emptySprite;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
