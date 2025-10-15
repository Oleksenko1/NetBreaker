using UnityEngine;
using UnityEngine.EventSystems;

public class ClickingZone : MonoBehaviour, IPointerDownHandler
{
    private ClickPressed_event clickPressed_Event;
    void Start()
    {
        clickPressed_Event = new ClickPressed_event();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        EventBus.Publish(clickPressed_Event);
    }
}
