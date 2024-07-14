using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchController
{
    private static TouchController Instance;
    private ITouchable touchAble;
    private RaycastHit hit;
    private List<GraphicRaycaster> graphicRaycasters;
    private bool clicked = false;
    private GameObject SelectedObject;
    public static HashSet<GameObject> PauseObjects = new();
    public static bool IsPaused = false;

    public TouchController()
    {
#if UNITY_EDITOR
        ResetStatic();//Unity doesn't make it by itself
#endif
        Instance = this;
        //Get all canvases for blocking touches in menus. You can just pause the system, but it needs more attention
        graphicRaycasters = Object.FindObjectsByType<GraphicRaycaster>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    public void Update()
    {
        if (IsPaused || PauseObjects.Count > 0)
        {
            return;
        }
        ProcessTouch();
    }

    public static void ResetStatic()
    {
        PauseObjects = null;
        IsPaused = false;
        Clear();
    }

    public static void Clear()
    {
        Instance.SelectedObject = null;
        Instance.clicked = false;
    }

    private void ProcessTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(IsBlockedByUI(touch))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (SelectedObject == null)
            {
                var hits = Physics2D.RaycastAll(ray.origin - new Vector3(0, 0, 10), ray.direction);
                if (hits.Length == 0 || hits[0].collider == null)
                {
                    return;
                }
                SelectedObject = hits[0].collider.gameObject;
                touchAble = SelectedObject.GetComponent<ITouchable>();
            }
            if (touchAble == null)
            {
                SelectedObject = null;
                return;
            }

            ProcessActions(touch, hit);
        }
    }

    private void ProcessActions(Touch touch, RaycastHit hit)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                clicked = true;
                touchAble.OnTouchDown();
                break;
            case TouchPhase.Stationary:
                if (!clicked)
                {
                    SelectedObject = null;
                    return;
                }
                touchAble.OnTouchStatic();
                break;
            case TouchPhase.Moved:
                if (!clicked)
                {
                    SelectedObject = null;
                    return;
                }
                touchAble.OnTouchMove();
                break;
            case TouchPhase.Ended:
                if (!clicked)
                {
                    SelectedObject = null;
                    return;
                }
                touchAble.OnTouchUp();
                SelectedObject = null;
                clicked = false;
                break;
        }
    }

    private bool IsBlockedByUI(Touch touch)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };
        List<RaycastResult> results = new List<RaycastResult>();
        foreach (var graphicRaycaster in graphicRaycasters)
        {
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.tag == "BlockTouches")
                {
                    return true;
                }
            }
        }

        return false;
    }
}
