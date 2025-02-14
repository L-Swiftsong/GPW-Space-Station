using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    private static FPSDisplay s_instance;

    [SerializeField] private TextMeshProUGUI fpsText;

    private float deltaTime = 0.0f;


    private void Awake()
    {
        s_instance = this;
        this.gameObject.SetActive(PlayerPrefs.GetInt("ShowFPS", 0) == 1);
    }
    private void OnDestroy()
    {
        if (s_instance == this)
            s_instance = null;
    }
    public static void SetEnabled(bool isEnabled)
    {
        if (s_instance != null)
            s_instance.gameObject.SetActive(isEnabled);
    }



    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float fps = 1.0f / deltaTime;

        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}
