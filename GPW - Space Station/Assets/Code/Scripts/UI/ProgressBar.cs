using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    private static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/LinearProgressBar"));
        obj.transform.SetParent(Selection.activeTransform, false);
    }

    [MenuItem("GameObject/UI/Radial Progress Bar")]
    private static void AddRadialProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/RadialProgressBar"));
        obj.transform.SetParent(Selection.activeTransform, false);
    }
#endif



    [Header("Mask")]
    [SerializeField] private Image _mask = null;


    [Header("Fill")]
    [SerializeField] private Image _fill = null;
    [SerializeField] private Gradient _fillColour = new Gradient();


    [Header("Text")]
    [SerializeField] private TMP_Text _progressText = null;
    [SerializeField] private bool _showExactValue = false; // False = Shows percentage value. True = Shows value of currentValue.
    [SerializeField] private string _progressTextSuffix = string.Empty;


    [Header("Bar Settings")]
    [SerializeField] private float _maximumValue = 100.0f;
    [SerializeField] private float _minimumValue = 0.0f;
    private float _currentValue = 0.0f;


    private float GetCurrentFillPercentage() => Mathf.Clamp01((_currentValue - _minimumValue) / (_maximumValue - _minimumValue));
    private void UpdateCurrentFill()
    {
        float fillPercentage = GetCurrentFillPercentage();
        _mask.fillAmount = fillPercentage;

        _fill.color = _fillColour.Evaluate(fillPercentage);

        UpdateFillText();
    }
    private void UpdateFillText()
    {
        if (_progressText == null)
        {
            // We don't have any text we wish to update.
            return;
        }

        _progressText.text = (_showExactValue ? _currentValue.ToString("0") : (GetCurrentFillPercentage() * 100.0f).ToString("0")) + _progressTextSuffix;
    }


    #region Setting Values

    #region Standard Setting

    public ProgressBar SetValues(float current = 0.0f, float min = 0.0f, float max = 100.0f)
    {
        _minimumValue = min;
        _maximumValue = max;
        _currentValue = current;

        // Update the progress bar.
        UpdateCurrentFill();
        return this;
    }
    public ProgressBar SetCurrentValue(float newValue)
    {
        _currentValue = Mathf.Clamp(newValue, _minimumValue, _maximumValue);
        UpdateCurrentFill();
        return this;
    }
    public ProgressBar SetMaximumValue(float newMaximum)
    {
        _maximumValue = newMaximum;
        UpdateCurrentFill();
        return this;
    }
    public ProgressBar SetMinimumValue(float newMinimum)
    {
        _minimumValue = newMinimum;
        UpdateCurrentFill();
        return this;
    }

    #endregion

    #region Preserve Percentage

    /// <summary> Set the maximumValue, changing the currentValue to preserve its percentage relative to the maximumValue and minimumValue.</summary>
    public ProgressBar SetMaximumValuePreservePercentage(float newMaximum)
    {
        float currentFillPercentage = GetCurrentFillPercentage();
        _maximumValue = newMaximum;
        _currentValue = ((newMaximum - _minimumValue) * currentFillPercentage) + _minimumValue;

        // Update the progress bar.
        UpdateCurrentFill();
        return this; // Return ourself for fluent interface.
    }

    #endregion

    #region PreserveOffset

    /// <summary> Set the maximumValue, changing the currentValue to preserve its offset to the maximumValue.</summary>
    public ProgressBar SetMaximumValuePreserveOffset(float newMaximum)
    {
        float currentFillOffset = _maximumValue - _currentValue;
        _maximumValue = newMaximum;
        _currentValue = newMaximum - currentFillOffset;

        // Update the progress bar.
        UpdateCurrentFill();
        return this; // Return ourself for fluent interface.
    }

    #endregion

    #endregion
}
