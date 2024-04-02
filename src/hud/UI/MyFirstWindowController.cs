namespace hud.UI;

using KSP.UI.Binding;
using Unity.Runtime;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;

public class MyFirstWindowController : MonoBehaviour
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
        _window = GetComponent<UIDocument>();

        // Get the root element of the window.
        // Since we're cloning the UXML tree from a VisualTreeAsset, the actual root element is a TemplateContainer,
        // so we need to get the first child of the TemplateContainer to get our actual root VisualElement.
        _rootElement = _window.rootVisualElement[0];
        _rootElement.CenterByDefault();
        ActivateCloseWindow(_rootElement);

        var partialIds = new List<string>()
        {
            "horizontal",
            "vertical"
        };

        foreach (var partialId in partialIds)
        {
            ActivateButtons(
                _rootElement,
                _rootElement.Q<TextField>(partialId + "-free-input"),
                _rootElement.Q<Label>(partialId + "-value"),
                new Dictionary<string, int>()
                {
                    { partialId + "-add-one", 1 },
                    { partialId + "-add-five", 5 },
                    { partialId + "-add-ten", 10 },
                    { partialId + "-minus-one", -1 },
                    { partialId + "-minus-five", -5 },
                    { partialId + "-minus-ten", -10 },
                });
        }
    }

    private void ActivateCloseWindow(VisualElement rootElement)
    {
        var closeButton = rootElement.Q<Button>("close-button");
        closeButton.clicked += () => IsWindowOpen = false;
    }

    private void ActivateButtons(VisualElement rootElement, TextField source, Label destination, Dictionary<string, int> increments)
    {
        foreach (var (buttonId, increment) in increments)
        {
            rootElement.Q<Button>(buttonId).clicked += () => UpdateValue(source, destination, increment);
        }
    }

    private void UpdateValue(TextField source, Label destination, int increment)
    {
        var input = source.value;
        var angle = int.Parse(input) + increment;
        destination.text = angle.ToString();
    }
}
