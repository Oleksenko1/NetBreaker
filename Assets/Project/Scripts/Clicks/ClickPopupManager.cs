using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class ClickPopupManager : MonoBehaviour
{
    [SerializeField] private int startingPoolAmount;
    [SerializeField] private RectTransform spawningZone;
    [SerializeField] private Transform popupPf;
    [Inject] private ClickingManager clickingManager;
    private Queue<ClickPopup> popupQueue = new Queue<ClickPopup>();
    private Vector3[] spawningZoneCorners = new Vector3[4];
    void Start()
    {
        spawningZone.GetWorldCorners(spawningZoneCorners);

        EventBus.Subscribe<ClickPressed_event>(OnClickPressed);

        InitializePool();
    }
    private void OnClickPressed(ClickPressed_event e)
    {
        ClickPopup clickPopup;
        if (popupQueue.Count > 0)
        {
            clickPopup = popupQueue.Dequeue();
        }
        else
        {
            clickPopup = Instantiate(popupPf, transform).GetComponent<ClickPopup>();

            clickPopup.Initialize(this);
        }

        if (clickPopup != null)
        {
            BigNum bitsAmount = clickingManager.GetClickingStats().GetBitsPerClick();
            Vector3 position = GetRandomPosition();
            clickPopup.ShowPopup(bitsAmount, position);
        }
    }
    private void InitializePool()
    {
        for (int i = 0; i < startingPoolAmount; i++)
        {
            var popup = Instantiate(popupPf, transform).GetComponent<ClickPopup>();

            popup.Initialize(this);

            popupQueue.Enqueue(popup);
        }
    }
    public void ReturnObject(ClickPopup clickPopup)
    {
        popupQueue.Enqueue(clickPopup);

        clickPopup.gameObject.SetActive(false);
    }
    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(spawningZoneCorners[0].x, spawningZoneCorners[2].x);
        float y = Random.Range(spawningZoneCorners[0].y, spawningZoneCorners[2].y);

        return new Vector3(x, y, 0);
    }
}
