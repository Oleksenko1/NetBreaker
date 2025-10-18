using TMPro;
using UnityEngine;
using VContainer;

public class UIBitsPerSecond : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    void Awake()
    {
        textField.SetText("00.0/s");

        EventBus.Subscribe<BitsIncomeTrackerUpdated_event>(OnTrackerUpdated);
    }
    private void OnTrackerUpdated(BitsIncomeTrackerUpdated_event e)
    {
        textField.SetText($"{e.bitsPerSecond.ToString()}/s");
    }
}
