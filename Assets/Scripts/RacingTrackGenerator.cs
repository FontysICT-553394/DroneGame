using System;
using System.Collections.Generic;
using UnityEngine;

public class RacingTrackGenerator : MonoBehaviour
{
    private GameObject initialTrackSegment;
    

    [SerializeField] private GameObject trackSegmentPrefab;
    [SerializeField] private GenerateObstacles generateObstacles;
    public List<GameObject> TrackSegments => trackSegments;
    private List<GameObject> trackSegments = new List<GameObject>();
    public int trackSegmentAmount = 5;
    public int trackSegmentCounter = 1;
    public float trackSegmentLength = -64.05f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTrackSegment = Instantiate(trackSegmentPrefab, Vector3.zero, Quaternion.identity);
        initialTrackSegment.transform.position = new Vector3(0f, 0f, 0f);
        trackSegments.Add(initialTrackSegment);
    }

    private void AddTrackSegment()
    {
        GameObject newTrackSegment = Instantiate(trackSegmentPrefab, new Vector3(trackSegments.Count * trackSegmentLength, 0f, 0f), Quaternion.identity);
        trackSegments.Add(newTrackSegment);
        generateObstacles.GetAllObstacleSegments(newTrackSegment);
    }

    void Update()
    {
        foreach (var trackSegment in trackSegments)
        {
            if (trackSegment == null)
            {
                Debug.LogWarning("Track segment is null, skipping.");
                continue;
            }
            Rigidbody trackSegmentRb = trackSegment.GetComponent<Rigidbody>();
            trackSegmentRb.linearVelocity = Vector3.right * 10f;
        }

        if (trackSegments.Count >= trackSegmentAmount) return; 

        for (trackSegmentCounter = trackSegments.Count; trackSegmentCounter < trackSegmentAmount; trackSegmentCounter++)
        {
            AddTrackSegment();
        }
    }
}
