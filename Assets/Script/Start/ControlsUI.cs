using UnityEngine;

public class ControlsUI : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;

    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject ManualButton;

    public void OpenControls()
    {
        StartButton.SetActive(false);
        ManualButton.SetActive(false);

        controlsPanel.SetActive(true);
    }

    public void CloseControls()
    {
        StartButton.SetActive(true);
        ManualButton.SetActive(true);

        controlsPanel.SetActive(false);
    }
}