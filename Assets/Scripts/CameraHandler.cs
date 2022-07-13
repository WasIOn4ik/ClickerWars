using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
{
    #region Variables

    [Header("Settings")]

    [Tooltip("Чувствительность перемещения камеры")]
    [SerializeField] private float cameraSens = 1f;
    [Tooltip("Масштаб маркеров врагов")]
    [SerializeField] private float markerScaleFactor = 1.2f;

    [Header("References")]

    [SerializeField] private Camera cam;
    [SerializeField] private Transform LUMarker, RDMarker;
    [SerializeField] private MarkerHandler markerPrefab;
    [SerializeField] private Canvas hud;

    private List<MarkerHandler> markers = new List<MarkerHandler>();
    private float swipeStartPosX, swipeStartPosZ;

    #endregion

    #region UnityCallbacks

    public void Update()
    {
        if (!GameBase.instance)
            return;

        if (!GameBase.instance.gameActive)
            return;

#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPosX = Input.mousePosition.x;
            swipeStartPosZ = Input.mousePosition.y;
        }
        else if (Input.GetMouseButton(0))
        {
            float x = (Input.mousePosition.x - swipeStartPosX) * cameraSens / 50;
            float z = (Input.mousePosition.y - swipeStartPosZ) * cameraSens / 50;
            swipeStartPosX = Input.mousePosition.x;
            swipeStartPosZ = Input.mousePosition.y;

            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x - x, LUMarker.position.x, RDMarker.position.x), cam.transform.position.y, Mathf.Clamp(cam.transform.position.z - z, RDMarker.position.z, LUMarker.position.z));
        }

#endif

#if UNITY_ANDROID

        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            swipeStartPosX = Input.GetTouch(0).position.x;
            swipeStartPosZ = Input.GetTouch(0).position.y;
        }
        else if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            float x = (Input.GetTouch(0).position.x - swipeStartPosX) * cameraSens / 50;
            float z = (Input.GetTouch(0).position.y - swipeStartPosZ) * cameraSens / 50;
            swipeStartPosX = Input.GetTouch(0).position.x;
            swipeStartPosZ = Input.GetTouch(0).position.y;

            cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x - x, LUMarker.position.x, RDMarker.position.x), cam.transform.position.y, Mathf.Clamp(cam.transform.position.z - z, RDMarker.position.z, LUMarker.position.z));
        }

#endif

    }

    public void FixedUpdate()
    {
        UpdateMarkers();
    }

    #endregion

    #region Functions

    public void UpdateMarkers()
    {
        foreach (var el in GameBase.instance.activeEnemies)
        {
            Vector3 vec = cam.WorldToScreenPoint(el.transform.position);
            if (vec.z < 0)
            {
                vec *= -1f;
            }

            if (el.GetMarker().transform is RectTransform r)
            {
                if (vec.x < 0 || vec.x > cam.pixelWidth || vec.y < 0 || vec.y > cam.pixelHeight)
                {
                    float x = vec.x;
                    float y = vec.y;
                    vec.x = Mathf.Clamp(vec.x, 0, cam.pixelWidth);
                    vec.y = Mathf.Clamp(vec.y, 0, cam.pixelHeight);
                    Vector2 local = new Vector2(vec.x / hud.scaleFactor, vec.y / hud.scaleFactor);

                    r.anchoredPosition = local;
                    el.GetMarker().gameObject.SetActive(true);
                }
                else
                {
                    el.GetMarker().gameObject.SetActive(false);
                }
                //Debug.Log(el.name + ": local:" + local.ToString() + " screen:" + vec.ToString() + " source:" + x + ";" + y + " pos:" + el.transform.position.ToString());
            }

        }
    }

    public MarkerHandler FindAvailableMarker()
    {
        foreach (var el in markers)
        {
            if (el.bAvailable)
            {
                el.Lock();
                return el;
            }
        }

        var newMarker = Instantiate(markerPrefab, hud.transform);
        markers.Add(newMarker);
        newMarker.transform.localScale = Vector3.one * (cam.pixelWidth * cam.pixelHeight / 500000 * markerScaleFactor);
        newMarker.Lock();

        return newMarker;
    }

    #endregion
}
