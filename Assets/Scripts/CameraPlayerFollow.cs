using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 offset = new Vector3(0, -.8f, -10);

    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, player.localPosition.y + offset.y, player.localPosition.z + offset.z);
    }
}
