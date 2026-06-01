using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothershipManager : MonoBehaviour
{
    [SerializeField] private GameObject mothershipPrefab;
    [SerializeField] private GameObject exclemationMarkPrefab;
    [SerializeField] private List<GameObject> motherShipSpawnLocations;
    [SerializeField] private float spawnIntervalMin = 15f;
    [SerializeField] private float spawnIntervalMax = 30f;
    
    private GameObject mothership;
    private GameObject exclemationMark;
    
    void Start()
    {
        StartCoroutine(SpawnMothership());
    }
    
    private IEnumerator SpawnMothership()
    {
        if (motherShipSpawnLocations.Count == 0)
            yield break;

        float delay = Random.Range(spawnIntervalMin, spawnIntervalMax);
        yield return new WaitForSeconds(delay);
    
        int spawnIndex = Random.Range(0, motherShipSpawnLocations.Count);
        GameObject spawnLocation = motherShipSpawnLocations[spawnIndex];
        exclemationMark = Instantiate(exclemationMarkPrefab, spawnLocation.transform.position, Quaternion.identity);
        yield return StartCoroutine(QuestionMarkBlink());
    
        mothership = Instantiate(mothershipPrefab, spawnLocation.transform.position, Quaternion.identity);
        if(spawnIndex % 2 == 0)
            mothership.GetComponent<Mothership>().moveDirection = Vector2.right;
        else
            mothership.GetComponent<Mothership>().moveDirection = Vector2.left;
        
        StartCoroutine(SpawnMothership());
    }
    
    private IEnumerator QuestionMarkBlink(int blinkAmount = 3)
    {
        
        for (int i = 0; i < blinkAmount; i++)
        {
            exclemationMark.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            exclemationMark.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
        
        Destroy(exclemationMark);
        yield return new WaitForSeconds(0.5f);
    }
    
}
