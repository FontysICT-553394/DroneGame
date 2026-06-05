using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HighscoreEntry
{
    public string minigameName;
    public string playerName;
    public int score;
    public string timestampUtc; // optional, for sorting ties or display
}

[System.Serializable]
public class HighscoreData
{
    public List<HighscoreEntry> entries = new List<HighscoreEntry>();
}
