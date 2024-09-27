using UnityEngine;
using UnityEngine.UIElements;

public class Minimap : MonoBehaviour
{
    public UIDocument minimapUIDoc;
    public RenderTexture minimapRenderTexture;

    private VisualElement rootVisualElement;
    private Image minimapView;
    void Start()
    {
        rootVisualElement = minimapUIDoc.rootVisualElement;
        minimapView = rootVisualElement.Q<Image>("minimapView");
        minimapView.image = minimapRenderTexture;
    }

    private void Update()
    {

    }
}