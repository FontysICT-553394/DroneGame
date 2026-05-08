using UnityEngine;

public class DestoryRacingTrack : MonoBehaviour
{
    private RacingTrackGenerator racingTrackGenerator;
    private GameObject currentTrackSegment;
    [SerializeField] private GameObject destroyeTrackTrigger;
    private Collider destroyeTrackTriggerCollider;
    private Collider currentTrackSegmentCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        racingTrackGenerator = GetComponent<RacingTrackGenerator>();
        destroyeTrackTriggerCollider = destroyeTrackTrigger.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTrackSegment = racingTrackGenerator.TrackSegments.Count > 0 ? racingTrackGenerator.TrackSegments[0] : null;

        if (currentTrackSegment == null) return;

        currentTrackSegmentCollider = currentTrackSegment.GetComponentInChildren<Collider>();

        if (destroyeTrackTriggerCollider.bounds.Intersects(currentTrackSegmentCollider.bounds))
        {
            racingTrackGenerator.TrackSegments.Remove(currentTrackSegment);
            Destroy(currentTrackSegment);
        }
    }
}
