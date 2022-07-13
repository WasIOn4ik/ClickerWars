using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Variables

    [SerializeField] private TMP_Text value;
    [SerializeField] private RectTransform fill;

    private int maxValue;
    private int currentValue;

    #endregion

    #region Functions

    public void SetMaxValue(int max)
    {
        maxValue = max;
    }

    public void SetCurrentValue(int cur)
    {
        currentValue = cur;
    }

    public void UpdateDisplay()
    {
        value.text = currentValue + " / " + maxValue;
        fill.localScale = new Vector3((float)currentValue / maxValue, 1f, 1f);
    }

    #endregion
}
