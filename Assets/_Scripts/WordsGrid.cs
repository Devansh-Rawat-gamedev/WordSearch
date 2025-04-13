using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class WordsGrid :MonoBehaviour
{   
    [SerializeField]GameManager gameManager;
    private List<GridElement> selectedGridElements = new ();
    private Vector2Int direction;
    public bool isSelecting;
    private GridLayoutGroup gridLayout;
    private int rows;
    private int cols;

    private void Start()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        gridLayout.GetGridRowsAndColumn(out rows, out cols);
        for(int i=0;i<rows;i++)
        {
            for(int j=0;j<cols;j++)
            {
                GridElement element = gridLayout.transform.GetChild(i * cols + j).GetComponent<GridElement>();
                element.gridPosition = new Vector2Int(j, i);
                element.OnClicked += StartWordSelection;
                element.OnOver += TryAddLetter;
                element.OnSelected += EndWordSelection;
            }
        }
    }
    private void StartWordSelection(GridElement element)
    {
        isSelecting = true;
        selectedGridElements.Clear();
        direction = Vector2Int.zero;
        AddToWord(element);

    }

    private void TryAddLetter(GridElement element)
    {
        if ( !isSelecting || selectedGridElements.Contains(element)) return;
        var newDir = element.gridPosition - selectedGridElements[^1].gridPosition;

        if (selectedGridElements.Count == 1)
        {
            if (IsValidDirection(newDir))
            {
                direction = newDir;
                AddToWord(element);
            }
        }
        else
        {
            if (newDir == direction)
            {
                AddToWord(element);
            }
        }
    }

    private void AddToWord(GridElement element)
    {
        selectedGridElements.Add(element);
        element.HighlightColor();

    }

    private void EndWordSelection()
    {
        isSelecting = false;
        StringBuilder formedWord = new ();
        foreach (var elem in selectedGridElements)
            formedWord.Append(elem.letter);

        // Checks if the word matches any target here
        if (gameManager.CompareWord(formedWord.ToString()))
        {
            foreach (var elem in selectedGridElements)
            {
                elem.SelectedColor();
            }
        }
        else
        {
            foreach (var elem in selectedGridElements)
            {
                elem.ResetColor();
            }
        }
    }

    private bool IsValidDirection(Vector2Int dir)
    {
        return dir is { x: 1, y: 0 } or { y: 1, x: 0 };
    }
}

internal static class GridLayoutGroupExtension
{
    public static void GetGridRowsAndColumn(this GridLayoutGroup gridLayout,out int rows, out int cols)
    {
        int childCount = gridLayout.transform.childCount;
        int constraintCount = gridLayout.constraintCount;

        if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            rows = constraintCount;
            cols = Mathf.CeilToInt(childCount / (float)rows);
        }
        else if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            cols = constraintCount;
            rows = Mathf.CeilToInt(childCount / (float)cols);
        }
        else // Flexible (approximate)
        {
            // Estimate based on cell size and grid dimensions
            RectTransform rect = gridLayout.GetComponent<RectTransform>();
            float width = rect.rect.width;
            float cellWidth = gridLayout.cellSize.x + gridLayout.spacing.x;
            cols = Mathf.FloorToInt(width / cellWidth);
            rows = Mathf.CeilToInt(childCount / (float)cols);
        }
    }
}
