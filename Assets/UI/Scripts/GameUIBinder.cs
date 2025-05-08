using UnityEngine;
using UnityEngine.UI;

public class GameUIBinder : MonoBehaviour
{
    [SerializeField] private GameUIController controller;

    private void Start()
    {
        controller.raiseButton.onClick.AddListener(OnRaise);
        controller.callButton.onClick.AddListener(OnCall);
        controller.foldButton.onClick.AddListener(OnFold);
        controller.checkButton.onClick.AddListener(OnCheck);
        controller.allInButton.onClick.AddListener(OnAllIn);
    }

    private void OnRaise()
    {
        Debug.Log("Raise button clicked");
        controller.raiseInputField.gameObject.SetActive(true);
        controller.confirmRaiseButton.gameObject.SetActive(true);
    }

    private void OnConfirmRaise()
    {
        int raiseValue;
        if (int.TryParse(controller.raiseInputField.text, out raiseValue))
        {
            Debug.Log($"Confirmed Raise: {raiseValue}");
        }
    }

    private void OnCall() => Debug.Log("Call button clicked");
    private void OnFold() => Debug.Log("Fold button clicked");
    private void OnCheck() => Debug.Log("Check button clicked");
    private void OnAllIn() => Debug.Log("All-in button clicked");
}
