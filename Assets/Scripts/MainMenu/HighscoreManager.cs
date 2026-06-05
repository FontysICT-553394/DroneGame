using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] TMP_Text highscoreListText;
    [SerializeField] private int highscoreCount = 10;
    [SerializeField] TMP_InputField highscoreNameInput;
    [SerializeField] private string minigameName;
    
    [SerializeField] private Button submitButton;

    private string SavePath => Path.Combine(Application.persistentDataPath, "Highscores.json");

    private void Start()
    {
        WriteScoreList(GetScoresForMinigame(minigameName));
    }
    
    private HighscoreData Load()
    {
        if (!File.Exists(SavePath)) return new HighscoreData();
        var json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<HighscoreData>(json) ?? new HighscoreData();
    }

    private void Save(HighscoreData data)
    {
        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public void AddScore()
    {
        var digits = new string(highscoreText.text.Where(char.IsDigit).ToArray());
        int score = 0;
        if (!string.IsNullOrEmpty(digits)) int.TryParse(digits, out score);
        
        var data = Load();
        data.entries.Add(new HighscoreEntry {
            minigameName = this.minigameName,
            playerName = highscoreNameInput.text,
            score = score,
            timestampUtc = System.DateTime.UtcNow.ToString("o")
        });
        Save(data);

        submitButton.interactable = false;
    }

    public List<HighscoreEntry> GetAllScores()
    {
        var data = Load();
        return data.entries
            .OrderByDescending(e => e.score)
            .ToList();
    }

    public List<HighscoreEntry> GetScoresForMinigame(string minigameName)
    {
        var data = Load();
        return data.entries
            .Where(e => e.minigameName == minigameName)
            .OrderByDescending(e => e.score)
            .ToList();
    }
    
    public void RefreshListForCurrentMinigame()
    {
        var scores = GetScoresForMinigame(minigameName);
        WriteScoreList(scores);
    }

    public void RefreshListAll()
    {
        var scores = GetAllScores();
        WriteScoreList(scores);
    }

    private void WriteScoreList(List<HighscoreEntry> scores)
    {
        if (highscoreListText == null) return;

        var topScores = scores
            .OrderByDescending(s => s.score)
            .Take(highscoreCount)
            .ToList();

        if (topScores.Count == 0)
        {
            highscoreListText.text = "No scores yet.";
            return;
        }

        var lines = new System.Text.StringBuilder();
        for (int i = 0; i < topScores.Count; i++)
        {
            var s = topScores[i];
            lines.AppendLine($"{i + 1}. {s.playerName} - {s.score}");
        }

        highscoreListText.text = lines.ToString();
    }

}
