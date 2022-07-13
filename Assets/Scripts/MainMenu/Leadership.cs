using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SaveScores
{
    public List<int> scores;
}

public class Leadership : MonoBehaviour
{
    #region Variables

    [SerializeField] private ScoreLine scoreLinePrefab;
    [SerializeField] private Transform scoreLineHolder;
    [SerializeField] private Color firstColor, secondColor, thirdColor;

    [SerializeField] private List<ScoreLine> scores = new List<ScoreLine>();

    #endregion

    #region StaticVariables

    public static int currentSessionScore = 0;
    private static string saveKey = "clickerSaves";
    private static List<int> scoresValues = new List<int>();

    #endregion

    #region StaticFunctions

    public static void Save()
    {
        SaveScores ss = new SaveScores { scores = scoresValues };
        string res = JsonUtility.ToJson(ss);
        Debug.Log(res);
        PlayerPrefs.SetString(saveKey, res);
    }

    public static void Load()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string res = PlayerPrefs.GetString(saveKey);
            SaveScores ss = JsonUtility.FromJson<SaveScores>(res);

            scoresValues = ss.scores;
        }
    }

    public static void SortScores()
    {
        scoresValues.Sort((a, b) => b.CompareTo(a));
    }

    public static void AddScores(int scores)
    {
        if (scores == 0)
            return;

        scoresValues.Add(scores);
        SortScores();

        while (scoresValues.Count > 5)
            scoresValues.RemoveAt(5);

    }

    #endregion

    #region Functions

    public void Populate()
    {
        for (int i = 0; i < 5; i++)
        {
            if (scoresValues.Count > i)
                scores[i].Init(i + 1, scoresValues[i]);
        }
    }

    #endregion
}
