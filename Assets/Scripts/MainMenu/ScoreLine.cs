using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreLine : MonoBehaviour
{
    #region Variables

    [SerializeField] private TMP_Text numberText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image backgroundImage;

    #endregion

    #region Functions

    public void Init(int num, int scr)
    {
        numberText.text = num.ToString();
        scoreText.text = scr.ToString();
    }

    public void SetColor(Color col)
    {
        backgroundImage.color = col;
    }

    #endregion
}
