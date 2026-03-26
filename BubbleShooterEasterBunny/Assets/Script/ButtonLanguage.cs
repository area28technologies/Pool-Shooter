using A28.PlatformSdk;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLanguage : MonoBehaviour
{
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;

    private void OnEnable()
    {
        button1.onClick.AddListener(OnClickEnglish);
        button2.onClick.AddListener(OnClickFil);
    }

    private void OnDisable()
    {
        button1.onClick.RemoveListener(OnClickEnglish);
        button2.onClick.RemoveListener(OnClickFil);
    }

    private void Start()
    {
        string language = A28Sdk.Instance.Language;
        OnLanguageChanged(language);
    }

    private void OnClickEnglish()
    {
        OnLanguageButtonClicked("en");
    }

    private void OnClickFil()
    {
        OnLanguageButtonClicked("fil");
    }

    private void OnLanguageButtonClicked(string language)
    {
        A28Sdk.Instance.Language = language;
        OnLanguageChanged(language);
    }

    private void OnLanguageChanged(string language)
    {
        if (language == "en") SelectButton(button1);
        else if (language == "fil") SelectButton(button2);
    }

    private void SelectButton(Button selected)
    {
        SetButtonDark(button1, selected == button1);
        SetButtonDark(button2, selected == button2);
    }

    private void SetButtonDark(Button btn, bool isSelected)
    {
        if (btn == null || btn.image == null)
            return;

        btn.image.color = isSelected ? Color.white : new Color(0.4f, 0.4f, 0.4f, 1f);
    }
}
