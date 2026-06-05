using System;
using System.Collections.Generic;
using UnityEngine;

public class RacingTrackGenerator : MonoBehaviour
{
    private GameObject initialTrackSegment;
    

    [SerializeField] private GameObject trackSegmentPrefab;
    [SerializeField] private GameObject finishLinePrefab;


    [SerializeField] private GenerateObstacles generateObstacles;
    public List<GameObject> TrackSegments => trackSegments;
    private List<GameObject> trackSegments = new List<GameObject>();
    public int trackSegmentAmount = 5;
    public int trackSegmentCounter = 1;
    public float trackSegmentLength = -64.05f;

    private int tracksPlaced = 0;
    [SerializeField] private int tracksToPlaceForFinish = 10;
    private bool stopPlacingTracks = false;
    private bool finishPlaced = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTrackSegment = Instantiate(trackSegmentPrefab, Vector3.zero, Quaternion.identity);
        initialTrackSegment.transform.position = new Vector3(0f, 0f, 0f);
        trackSegments.Add(initialTrackSegment);
    }

    private void AddTrackSegment()
    {
        if (stopPlacingTracks) return;
        Debug.Log("AddTrackSegment called");

        GameObject newTrackSegment = Instantiate(
            trackSegmentPrefab,
            new Vector3(trackSegments.Count * trackSegmentLength, 0f, 0f),
            Quaternion.identity
        );

        trackSegments.Add(newTrackSegment);

        if (generateObstacles == null)
        {
            Debug.LogError("GenerateObstacles reference is NULL in inspector!");
            return;
        }

        Debug.Log("Calling GenerateObstaclesForTrack on: " + newTrackSegment.name);

        generateObstacles.GenerateObstaclesForTrack(newTrackSegment);

        tracksPlaced++;
    }

    private void PlaceFinishLine()
    {
        if (finishPlaced) return;

        finishPlaced = true;
        stopPlacingTracks = true;

        GameObject lastTrack = trackSegments[trackSegments.Count - 1];

        Vector3 finishPos = lastTrack.transform.position + new Vector3(trackSegmentLength, 0f, 0f);

        GameObject finishLine = Instantiate(finishLinePrefab, finishPos, Quaternion.identity);

        Rigidbody rb = finishLine.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.right * 8f;
    }

    void Update()
    {
        foreach (var trackSegment in trackSegments)
        {
            if (trackSegment == null) continue;

            Rigidbody trackSegmentRb = trackSegment.GetComponent<Rigidbody>();
            trackSegmentRb.linearVelocity = Vector3.right * 8f;
        }

        if (tracksPlaced >= tracksToPlaceForFinish)
        {
            PlaceFinishLine();
            return;
        }

        if (stopPlacingTracks) return;

        if (trackSegments.Count >= trackSegmentAmount) return;

        for (trackSegmentCounter = trackSegments.Count; trackSegmentCounter < trackSegmentAmount; trackSegmentCounter++)
        {
            AddTrackSegment();
        }
    }
}
