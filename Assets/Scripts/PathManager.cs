using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pathPrefabs;
    [SerializeField] private GameObject firstPath;
    [SerializeField] private int pathCount;
    public List<GameObject> pathList = new List<GameObject>();
    private float zPathSize = 10f;
    private const float positionBais = 0f;

    public float distroyDistance;

    public int listPathIndex = 0;

    [SerializeField] private OtherCarManager otherCarManager;

    void Start()
    {
        zPathSize = firstPath.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z;
        pathList.Add(firstPath);
        SpawnPath();
        StartCoroutine(RepositionPath());
    }

    void SpawnPath()
    {
        for (int i = 0; i < pathCount; i++)
        {
            Vector3 pathPos = pathList[pathList.Count - 1].transform.position + Vector3.forward * zPathSize;
            pathPos.z += positionBais;
            GameObject path = Instantiate(pathPrefabs[Random.Range(0, pathPrefabs.Length)], pathPos, Quaternion.identity);
            path.transform.parent = transform;
            pathList.Add(path);
        }
    }


    // endless path
    IEnumerator RepositionPath()
    {
        while (true)
        {
            distroyDistance = Camera.main.transform.position.z - 15f;
            if (pathList[listPathIndex].transform.position.z < distroyDistance)
            {
                Vector3 nextPathPos = FarestPath().transform.position + Vector3.forward * zPathSize; // find next path
                nextPathPos.z += positionBais;

                pathList[listPathIndex].transform.position = nextPathPos; // move path

                otherCarManager.CheckAndDisableCarPath();

                listPathIndex++; 
                if (listPathIndex == pathList.Count)
                {
                    listPathIndex = 0;
                }
            }
            otherCarManager.FindCarAndReset();
            yield return null;
        }
    }

    GameObject FarestPath()
    {
        GameObject thisPath = pathList[0];
        for (int i = 0; i < pathList.Count; i++)
        {
            if (pathList[i].transform.position.z > thisPath.transform.position.z)
            {
                thisPath = pathList[i];
            }
        }
        return thisPath;
    }
}
