using System.Transactions;
using hud.Coordinates;
using hud.Input;

namespace hud.UI;

using KSP.UI.Binding;
using Unity.Runtime;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;

public class HudWindowController : MonoBehaviour
{
    private UIDocument _window;
    private VisualElement _rootElement;
    private bool _isWindowOpen;

    public bool IsWindowOpen
    {
        get => _isWindowOpen;
        set
        {
            _isWindowOpen = value;
            _rootElement.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;

            GameObject.Find(hudPlugin.ToolbarFlightButtonID)
                ?.GetComponent<UIValue_WriteBool_Toggle>()
                ?.SetValue(value);
        }
    }

    private void OnEnable()
    {
        UpdateWindowAndRootElement();

        var config = hudPlugin.Instance.HudConfig();
        var attitudeControlOverride = hudPlugin.Instance.AttitudeControlOverride();

        ActivateCloseWindow(_rootElement);

        ActivateToggleDisplay(_rootElement, config);
        ActivateToogleAttitudeControlOverride(_rootElement, attitudeControlOverride);
        
        Action<int> setHorizontal = (angle) =>
        {
            var fixedAngle = FixAngle(angle);
            attitudeControlOverride.HorizontalAngle = fixedAngle;
            _rootElement.Q<TextField>("horizontal-free-input").value = fixedAngle.ToString();
        };
        
        Action<int> setVertical = (angle) =>
        {
            var fixedAngle = FixAngle(angle);
            attitudeControlOverride.VerticalAngle = fixedAngle;
            _rootElement.Q<TextField>("vertical-free-input").value = fixedAngle.ToString();
        };

        Action<int> incrementHorizontal = (increment) =>
        {
            setHorizontal(attitudeControlOverride.HorizontalAngle + increment);
        };
        Action<int> incrementVertical = (increment) =>
        {
            setVertical(attitudeControlOverride.VerticalAngle + increment);
        };

        var partialIds = new Dictionary<string, Action<int>>()
        {
            { "horizontal", incrementHorizontal },
            { "vertical", incrementVertical },
        };

        foreach (var (partialId, update) in partialIds)
        {
            ActivateIncrementButtons(
                _rootElement,
                new Dictionary<string, int>()
                {
                    { partialId + "-add-one", 1 },
                    { partialId + "-add-five", 5 },
                    { partialId + "-add-ten", 10 },
                    { partialId + "-minus-one", -1 },
                    { partialId + "-minus-five", -5 },
                    { partialId + "-minus-ten", -10 },
                },
                update
                );
        }

        var horizontalTargets = new Dictionary<string, Action>()
        {
            { "target-north", () => setHorizontal(0) },
            { "target-east", () => setHorizontal(90) },
            { "target-south", () => setHorizontal(180) },
            { "target-west", () => setHorizontal(-90) },
        };

        foreach (var (buttonId, action) in horizontalTargets)
        {
            _rootElement.Q<Button>(buttonId).clicked += action;
        }

        _rootElement.Q<TextField>("vertical-free-input").value = attitudeControlOverride.VerticalAngle.ToString();
        _rootElement.Q<TextField>("horizontal-free-input").value = attitudeControlOverride.HorizontalAngle.ToString();

        _rootElement.Q<Button>("set-vertical").clicked += () =>
        {
            var input = _rootElement.Q<TextField>("vertical-free-input").value;
            setVertical(int.Parse(input));
        };
        
        _rootElement.Q<Button>("set-horizontal").clicked += () =>
        {
            var input = _rootElement.Q<TextField>("horizontal-free-input").value;
            setHorizontal(int.Parse(input));
        };

        _rootElement.Q<Label>("vertical-value").text = attitudeControlOverride.VerticalAngle.ToString();
        _rootElement.Q<Label>("horizontal-value").text = attitudeControlOverride.HorizontalAngle.ToString();
    }
    
    private void OnGUI()
    {
        var vessel = KSP.Game.GameManager.Instance.Game.ViewController.GetActiveSimVessel();
        LocalCoordinates coord = null;
        if (vessel is not null)
        {
            coord = new LocalCoordinates(vessel);

            _rootElement.Q<Label>("vertical-value").text = coord.VerticalAngle.ToString();
            _rootElement.Q<Label>("horizontal-value").text = coord.HorizontalAngle.ToString();
        }
    }

    private int FixAngle(int Angle)
    {
        switch (Angle)
        {
            case > 180:
                return Angle - 360;
            case < -180:
                return Angle + 360;
            default:
                return Angle;
        }
    }

    private void UpdateWindowAndRootElement()
    {
        _window = GetComponent<UIDocument>();

        // Get the root element of the window.
        // Since we're cloning the UXML tree from a VisualTreeAsset, the actual root element is a TemplateContainer,
        // so we need to get the first child of the TemplateContainer to get our actual root VisualElement.
        _rootElement = _window.rootVisualElement[0];
        _rootElement.CenterByDefault();
    }

    private void ActivateToggleDisplay(VisualElement rootElement, HudConfig config)
    {
        var toggleDisplay = rootElement.Q<Toggle>("display-toggle");
        toggleDisplay.value = config.HudIsEnabled.Value;
        toggleDisplay.RegisterCallback<ChangeEvent<bool>>((evt) =>
        {
            config.HudIsEnabled.Value = evt.newValue;
        });
    }

    private void ActivateToogleAttitudeControlOverride(VisualElement rootElement, AttitudeControlOverride attitudeControlOverride)
    {
        var toggleAttitude = rootElement.Q<Toggle>("attitude-toggle");
        toggleAttitude.value = attitudeControlOverride.IsEnabled;
        toggleAttitude.RegisterCallback<ChangeEvent<bool>>((evt) =>
        {
            attitudeControlOverride.IsEnabled = evt.newValue;
        });
    }

    private void ActivateCloseWindow(VisualElement rootElement)
    {
        var closeButton = rootElement.Q<Button>("close-button");
        closeButton.clicked += () => IsWindowOpen = false;
    }

    private void ActivateIncrementButtons(VisualElement rootElement, Dictionary<string, int> increments, Action<int> update)
    {
        foreach (var (buttonId, increment) in increments)
        {
            rootElement.Q<Button>(buttonId).clicked += () => update(increment);
        }
    }
}
