using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WordsGrid 
{
    private readonly GameManager _gameManager;
    private readonly List<GridElement> _selectedGridElements = new ();
    private Vector2Int _direction;
    private bool _isSelecting;

    private StringBuilder _formedWord=new();
    private bool _isCorrect;
    private Vector2Int _newDir= Vector2Int.zero;
    private RectTransform rect;
    public WordsGrid(GameManager gameManager , GridLayoutGroup gridLayout)
    {
        _gameManager = gameManager;
        gridLayout.GetGridRowsAndColumn(out var rows, out var cols);
        GridElement element;
        for(int i=0;i<rows;i++)
        {
            for(int j=0;j<cols;j++)
            {
                element = gridLayout.transform.GetChild(i * cols + j).GetComponent<GridElement>();
                element.gridPosition = new Vector2Int(j, i);
                element.OnClicked += StartWordSelection;
                element.OnOver += TryAddLetter;
                element.OnSelected += EndWordSelection;
            }
        }
    }
    private void StartWordSelection(GridElement element)
    {
        _isSelecting = true;
        _selectedGridElements.Clear();
        _direction = Vector2Int.zero;
        AddToWord(element);
        rect = element.GetComponent<RectTransform>();
        _gameManager.StartCoroutine(Interpolate(rect, new Vector2(1.5f, 1.5f), 0.1f));
    }
    private void TryAddLetter(GridElement element)
    {
        if ( !_isSelecting || _selectedGridElements.Contains(element)) return;
        _newDir = element.gridPosition - _selectedGridElements[^1].gridPosition;

        if (_selectedGridElements.Count == 1)
        {
            if (IsValidDirection(_newDir))
            {
                _direction = _newDir;
                AddToWord(element);
                rect = element.GetComponent<RectTransform>();
                _gameManager.StartCoroutine(Interpolate(rect, new Vector2(1.5f, 1.5f), 0.1f));
            }
        }
        else
        {
            if (_newDir == _direction)
            {
                AddToWord(element);
                rect = element.GetComponent<RectTransform>();
                _gameManager.StartCoroutine(Interpolate(rect, new Vector2(1.5f, 1.5f), 0.1f));
            }
        }
    }
    private void AddToWord(GridElement element)
    {
        _selectedGridElements.Add(element);
        element.HighlightColor();

    }
    private void EndWordSelection()
    {
        _isSelecting = false;
        _formedWord.Clear();
        foreach (var elem in _selectedGridElements)
            _formedWord.Append(elem.letter);

        _isCorrect = _gameManager.CompareWord(_formedWord.ToString());

        MarkCorrect();
    }
    private bool IsValidDirection(Vector2Int dir)
    {
        return dir is { x: 1, y: 0 } or { y: 1, x: 0 };
    }
    private async Task MarkCorrect()
    {
        foreach (var elem in _selectedGridElements)
        {
            await Task.Delay(0100);
            if (_isCorrect)
                elem.SelectedColor();
            else
                elem.ResetColor();
            
        }

    }


    private IEnumerator Interpolate(RectTransform rect , Vector2 end , float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            rect.localScale = Vector2.Lerp(rect.localScale, end, t);
            yield return null;
        }

        _gameManager.StartCoroutine(Interpolate(rect, Vector2.one, 0.1f));
    }
}
static class GridLayoutGroupExtension
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
        else 
        {
            RectTransform rect = gridLayout.GetComponent<RectTransform>();
            float width = rect.rect.width;
            float cellWidth = gridLayout.cellSize.x + gridLayout.spacing.x;
            cols = Mathf.FloorToInt(width / cellWidth);
            rows = Mathf.CeilToInt(childCount / (float)cols);
        }
    }
}
