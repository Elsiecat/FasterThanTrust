using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button button;

    private System.Action onSelect;

    public void Initialize(string title, string description, System.Action onSelectCallback)
    {
        titleText.text = title;
        descriptionText.text = description;
        onSelect = onSelectCallback;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked()
    {
        onSelect?.Invoke();
    }
}
