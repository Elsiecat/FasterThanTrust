using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{

    [SerializeField]private TextMeshProUGUI _currentStateTxt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentStateTxt != null)
            _currentStateTxt.text = $"Game State: {Managers.Game.CurrentState}";
    }
}
