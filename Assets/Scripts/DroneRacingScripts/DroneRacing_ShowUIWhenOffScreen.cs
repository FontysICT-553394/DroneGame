using UnityEngine;

public class ShowUIWhenOffScreen : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject uiObject;

    void Update()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(player.position);

        bool playerVisible =
            viewportPosition.z > 0 &&
            viewportPosition.x > 0 &&
            viewportPosition.x < 1 &&
            viewportPosition.y > 0 &&
            viewportPosition.y < 1;

        uiObject.SetActive(!playerVisible);
    }
}