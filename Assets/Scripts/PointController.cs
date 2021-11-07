using UnityEngine;

public class PointController : MonoBehaviour
{
    [HideInInspector] public Vector3 destination;

    [SerializeField] [Range(0f, 2f)] private float animationDuration = .5f;

    private float timePassed;

    private Vector3 start;

    // this is to normalize the animation duration to a t value between 0 and 1 while lerping 
    private float tMultiplier;

    void Start()
    {
        tMultiplier = 1f / animationDuration;
        start = transform.position;
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        float t = timePassed * tMultiplier;
        transform.position = Vector3.Lerp(start, destination, t);
    }
}