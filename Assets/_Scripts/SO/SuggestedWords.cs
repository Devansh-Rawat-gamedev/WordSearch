using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SuggestedWords", menuName = "Scriptable Objects/SuggestedWords")]
public class SuggestedWords : ScriptableObject
{
    [SerializeField]public List<string> suggestedWordsList = new ();
    public List<string> SuggestedWordsList => suggestedWordsList;
    
    private void OnValidate()
    {
        for (int i = 0; i < suggestedWordsList.Count; i++)
        {
            if (!string.IsNullOrEmpty(suggestedWordsList[i]))
                suggestedWordsList[i] = suggestedWordsList[i].ToUpper();
        }
    }
}
