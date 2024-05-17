using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public abstract class UiCanvas : MonoBehaviour
{
    private Canvas _canvas;
    public abstract UIName UIName { get; }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public virtual void Show()
    {
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    public virtual void Hide()
    {
        _canvas.renderMode = RenderMode.WorldSpace;
        transform.position = new Vector3(-1000, -1000);
        transform.parent.GetComponent<LayoutGroup>().CalculateLayoutInputVertical();
        transform.parent.GetComponent<LayoutGroup>().CalculateLayoutInputHorizontal();
    }
}
