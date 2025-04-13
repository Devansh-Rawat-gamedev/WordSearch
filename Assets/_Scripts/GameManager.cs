using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]private List<string> hiddenWordsList;
    [SerializeField]private Transform suggestedWordsLayout;
    [SerializeField]private GameObject suggestedWordPrefab;
    
    private Dictionary<string,TextMeshProUGUI> suggestedWordText=new();

    private void Start()
    {
        foreach (var word in hiddenWordsList)
        {
            GameObject wordInstance = Instantiate(suggestedWordPrefab, suggestedWordsLayout);
            TextMeshProUGUI textComponent = wordInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent)
            {
                textComponent.text = word;
                suggestedWordText.Add(word.ToUpper(), textComponent);
            }
        }
    }

    public bool CompareWord(string word)
    {
        string upperWord = word.ToUpper();
        if (suggestedWordText.ContainsKey(upperWord))
        {
            suggestedWordText[upperWord].fontStyle |= FontStyles.Strikethrough;
            return true;
        }
        
        return false;
    }
}
