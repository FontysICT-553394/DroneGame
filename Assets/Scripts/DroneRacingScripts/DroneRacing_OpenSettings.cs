using UnityEngine;
using UnityEngine.InputSystem;

public class DroneRacing_OpenSettings : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;

    private bool isOpen;

    private void Start()
    {
        settingsMenu.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }

    public void OpenSettings(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        isOpen = !isOpen;

        settingsMenu.SetActive(isOpen);

        if (isOpen)
        {
            Time.timeScale = 0f; // Pause game
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f; // Resume game
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}