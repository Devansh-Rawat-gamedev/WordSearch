using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//gamemanager or some sorta unitmanager
public class GameManager : MonoBehaviour
{
    [SerializeField]private SuggestedWords suggestedWords;
    [SerializeField]private Transform suggestedWordsLayout;
    [SerializeField]private GameObject suggestedWordPrefab;
    [SerializeField]private GridLayoutGroup gridLayoutGroup;
    private readonly Dictionary<string,(TextMeshProUGUI text,bool accessed)> _suggestedWordText=new();
    

    private WordsGrid _wordsGrid;

    private void Awake()
    {
        foreach (var word in suggestedWords.suggestedWordsList)
        {
            GameObject wordInstance = Instantiate(suggestedWordPrefab, suggestedWordsLayout);
            TextMeshProUGUI textComponent = wordInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent)
            {
                textComponent.text = word;
                _suggestedWordText.Add(word.ToUpper(), (textComponent,false));
            }
        }
        _wordsGrid = new WordsGrid(this, gridLayoutGroup);
    }

    public bool CompareWord(string word)
    {
        string upperWord = word.ToUpper();
        if (_suggestedWordText.TryGetValue(upperWord, out var value)  && !value.Item2)
        {
            value.Item1.fontStyle |= FontStyles.Strikethrough;
            value.Item2 = true;
            _suggestedWordText[upperWord] = value;
            return true;
        }
        
        return false;
    }
}
