using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridElement : MonoBehaviour , IPointerDownHandler ,IPointerEnterHandler,IPointerUpHandler
{
    public char letter ;
    public Vector2Int gridPosition;
    private Image image;
    private Color imageColor;
    
    #region events
    public event Action<GridElement> OnClicked;
    public event Action<GridElement> OnOver;
    public event Action OnSelected;
    #endregion

    private void Start()
    {
        letter =char.ToUpper( GetComponentInChildren<TextMeshProUGUI>().text[0]);
        image = GetComponent<Image>();
        imageColor = image.color;
    }

    public void OnPointerDown(PointerEventData eventData)=> OnClicked?.Invoke(this);
    public void OnPointerEnter(PointerEventData eventData)=> OnOver?.Invoke(this);
    public void OnPointerUp(PointerEventData eventData)=> OnSelected?.Invoke();
    
    public void HighlightColor()
    {
        image.color = Color.yellow;
    }
    public void ResetColor()
    {
        image.color = imageColor;
    }
    public void SelectedColor()
    {
        imageColor = Color.green;
        ResetColor();
    }


}
