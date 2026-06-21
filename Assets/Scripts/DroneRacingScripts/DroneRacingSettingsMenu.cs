using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DroneSettingsUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject settingsRoot;

    [Header("Input Fields")]
    [SerializeField] private TMP_InputField droneSpeedInput;
    [SerializeField] private TMP_InputField boostChargesInput;
    [SerializeField] private TMP_InputField trackAmountInput;
    [SerializeField] private TMP_InputField trackSpeedInput;

    [Header("Boolean Sliders 0 = Off, 1 = On")]
    [SerializeField] private Slider damageSlider;
    [SerializeField] private Slider generateTracksSlider;
    [SerializeField] private Slider generateObstaclesSlider;
    [SerializeField] private Slider boostEnabledSlider;
    [SerializeField] private Slider generateBatteriesSlider;

    [Header("Optional")]
    [SerializeField] private int defaultTracksToPlaceForFinish = 10;

    [Header("Behaviour")]
    [SerializeField] private bool pauseGameWhenOpen = true;
    [SerializeField] private bool openWithEscape = true;
    [SerializeField] private bool lockCursorWhenClosed = true;

    private bool menuOpen;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLockState;

    private void Start()
    {
        if (settingsRoot == null)
        {
            Debug.LogError("SettingsRoot is not assigned. Sleep your settings panel/menu into this field.");
            return;
        }

        SetupUI();
        LoadCurrentSettingsIntoUI();
        CloseMenu();
    }

    private void Update()
    {
        if (!openWithEscape)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        if (menuOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void OpenMenu()
    {
        menuOpen = true;
        settingsRoot.SetActive(true);

        previousCursorVisible = Cursor.visible;
        previousCursorLockState = Cursor.lockState;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 0f;
        }
    }

    public void CloseMenu()
    {
        menuOpen = false;

        if (settingsRoot != null)
        {
            settingsRoot.SetActive(false);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }

        if (lockCursorWhenClosed)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = previousCursorVisible;
            Cursor.lockState = previousCursorLockState;
        }
    }

    public void ApplySettings()
    {
        DroneRacingRuntimeSettings.DroneSpeed = ReadFloatInput(
            droneSpeedInput,
            DroneRacingRuntimeSettings.DroneSpeed,
            1f,
            20f
        );

        DroneRacingRuntimeSettings.BoostCharges = ReadIntInput(
            boostChargesInput,
            DroneRacingRuntimeSettings.BoostCharges,
            0,
            10
        );

        DroneRacingRuntimeSettings.TrackSegmentAmount = ReadIntInput(
            trackAmountInput,
            DroneRacingRuntimeSettings.TrackSegmentAmount,
            1,
            20
        );

        DroneRacingRuntimeSettings.TrackSpeed = ReadFloatInput(
            trackSpeedInput,
            DroneRacingRuntimeSettings.TrackSpeed,
            0f,
            24f
        );

        DroneRacingRuntimeSettings.DamageEnabled = ReadBoolSlider(damageSlider);
        DroneRacingRuntimeSettings.GenerateTracks = ReadBoolSlider(generateTracksSlider);
        DroneRacingRuntimeSettings.GenerateObstacles = ReadBoolSlider(generateObstaclesSlider);
        DroneRacingRuntimeSettings.BoostEnabled = ReadBoolSlider(boostEnabledSlider);
        DroneRacingRuntimeSettings.GenerateBatteries = ReadBoolSlider(generateBatteriesSlider);

        if (DroneRacingRuntimeSettings.TracksToPlaceForFinish <= 0)
        {
            DroneRacingRuntimeSettings.TracksToPlaceForFinish = defaultTracksToPlaceForFinish;
        }

        RefreshInputTexts();

        Debug.Log("Settings applied.");
        Debug.Log($"DroneSpeed: {DroneRacingRuntimeSettings.DroneSpeed}");
        Debug.Log($"BoostCharges: {DroneRacingRuntimeSettings.BoostCharges}");
        Debug.Log($"TrackSegmentAmount: {DroneRacingRuntimeSettings.TrackSegmentAmount}");
        Debug.Log($"TrackSpeed: {DroneRacingRuntimeSettings.TrackSpeed}");
        Debug.Log($"DamageEnabled: {DroneRacingRuntimeSettings.DamageEnabled}");
        Debug.Log($"GenerateTracks: {DroneRacingRuntimeSettings.GenerateTracks}");
        Debug.Log($"GenerateObstacles: {DroneRacingRuntimeSettings.GenerateObstacles}");
        Debug.Log($"BoostEnabled: {DroneRacingRuntimeSettings.BoostEnabled}");
        Debug.Log($"GenerateBatteries: {DroneRacingRuntimeSettings.GenerateBatteries}");

        CloseMenu();
    }

    private void SetupUI()
    {
        SetupInputField(droneSpeedInput, TMP_InputField.ContentType.DecimalNumber);
        SetupInputField(trackSpeedInput, TMP_InputField.ContentType.DecimalNumber);

        SetupInputField(boostChargesInput, TMP_InputField.ContentType.IntegerNumber);
        SetupInputField(trackAmountInput, TMP_InputField.ContentType.IntegerNumber);

        SetupBoolSlider(damageSlider);
        SetupBoolSlider(generateTracksSlider);
        SetupBoolSlider(generateObstaclesSlider);
        SetupBoolSlider(boostEnabledSlider);
        SetupBoolSlider(generateBatteriesSlider);
    }

    private void SetupInputField(TMP_InputField input, TMP_InputField.ContentType contentType)
    {
        if (input == null)
        {
            return;
        }

        input.contentType = contentType;
    }

    private void SetupBoolSlider(Slider slider)
    {
        if (slider == null)
        {
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = true;
    }

    private void LoadCurrentSettingsIntoUI()
    {
        if (droneSpeedInput != null)
        {
            droneSpeedInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.DroneSpeed.ToString("0.0", CultureInfo.InvariantCulture)
            );
        }

        if (boostChargesInput != null)
        {
            boostChargesInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.BoostCharges.ToString()
            );
        }

        if (trackAmountInput != null)
        {
            trackAmountInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.TrackSegmentAmount.ToString()
            );
        }

        if (trackSpeedInput != null)
        {
            trackSpeedInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.TrackSpeed.ToString("0.0", CultureInfo.InvariantCulture)
            );
        }

        SetBoolSliderValue(damageSlider, DroneRacingRuntimeSettings.DamageEnabled);
        SetBoolSliderValue(generateTracksSlider, DroneRacingRuntimeSettings.GenerateTracks);
        SetBoolSliderValue(generateObstaclesSlider, DroneRacingRuntimeSettings.GenerateObstacles);
        SetBoolSliderValue(boostEnabledSlider, DroneRacingRuntimeSettings.BoostEnabled);
        SetBoolSliderValue(generateBatteriesSlider, DroneRacingRuntimeSettings.GenerateBatteries);
    }

    private void SetBoolSliderValue(Slider slider, bool value)
    {
        if (slider == null)
        {
            return;
        }

        slider.SetValueWithoutNotify(value ? 1f : 0f);
    }

    private float ReadFloatInput(TMP_InputField input, float fallback, float min, float max)
    {
        if (input == null)
        {
            return fallback;
        }

        string text = input.text;

        if (string.IsNullOrWhiteSpace(text))
        {
            return fallback;
        }

        text = text.Replace(',', '.');

        if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
        {
            return fallback;
        }

        return Mathf.Clamp(value, min, max);
    }

    private int ReadIntInput(TMP_InputField input, int fallback, int min, int max)
    {
        if (input == null)
        {
            return fallback;
        }

        string text = input.text;

        if (string.IsNullOrWhiteSpace(text))
        {
            return fallback;
        }

        if (!int.TryParse(text, out int value))
        {
            return fallback;
        }

        return Mathf.Clamp(value, min, max);
    }

    private bool ReadBoolSlider(Slider slider)
    {
        if (slider == null)
        {
            return false;
        }

        return Mathf.RoundToInt(slider.value) == 1;
    }

    private void RefreshInputTexts()
    {
        if (droneSpeedInput != null)
        {
            droneSpeedInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.DroneSpeed.ToString("0.0", CultureInfo.InvariantCulture)
            );
        }

        if (boostChargesInput != null)
        {
            boostChargesInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.BoostCharges.ToString()
            );
        }

        if (trackAmountInput != null)
        {
            trackAmountInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.TrackSegmentAmount.ToString()
            );
        }

        if (trackSpeedInput != null)
        {
            trackSpeedInput.SetTextWithoutNotify(
                DroneRacingRuntimeSettings.TrackSpeed.ToString("0.0", CultureInfo.InvariantCulture)
            );
        }
    }

    private void OnDestroy()
    {
        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }
}