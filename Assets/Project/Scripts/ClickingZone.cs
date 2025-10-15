using UnityEngine;
using UnityEngine.EventSystems;

public class ClickingZone : MonoBehaviour, IPointerDownHandler
{
    void Start()
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click");
    }
}
