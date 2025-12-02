using UnityEngine;
using System.Collections.Generic;

public class OtherCarManager : MonoBehaviour
{
    [SerializeField] private GameObject activeCars;
    [SerializeField] private GameObject unactiveCars;
    [SerializeField] private GameObject[] carArray;
    [SerializeField] private int initialCarNumber = 50;

    [Range(0f, 1f)] public float carfreq = 0.5f;
    [Range(0f, 1f)] public float reverseCarfreq = 0.2f;

    [SerializeField] private PathManager pathManager;

    private Vector3 previousCarPos = Vector3.zero;

    void Start()
    {
        for (int i = 0; i < initialCarNumber; i++)
        {
            GameObject car = Instantiate(carArray[Random.Range(0, carArray.Length)]);
            car.SetActive(false);
            car.transform.parent = unactiveCars.transform;
        }
    }

    List<Vector3> ListCarPos(GameObject thePath)
    {
        List<Vector3> listPathPos = new List<Vector3>();
 
        for (int i = 0; i < thePath.transform.childCount; i++)
        {
            Vector3 pos = thePath.transform.GetChild(i).position + new Vector3(0, 1f, 0);

            listPathPos.Add(pos);
        }

        return listPathPos;
    }

    public void CheckAndDisableCarPath()
    {
        List<Vector3> listCarPos = ListCarPos(pathManager.pathList[pathManager.listPathIndex]);

            // create car pos
        if (Random.value <= carfreq)
        {
            Vector3 carPos = listCarPos[Random.Range(0, listCarPos.Count)];
            while (carPos.x == previousCarPos.x)
            {
                carPos = listCarPos[Random.Range(0, listCarPos.Count)];
            }

            previousCarPos = carPos;

            // random car
            GameObject car = GetRandomCar();
            car.transform.position = carPos;

            if (Random.value <= reverseCarfreq)
            {
                car.transform.rotation = Quaternion.Euler(0, 180f, 0);
            }

            car.transform.parent = activeCars.transform;
        }
    }

// get random car 
    private GameObject GetRandomCar()
    {
        GameObject car = unactiveCars.transform.GetChild(Random.Range(0, unactiveCars.transform.childCount)).gameObject;
        car.transform.parent = null;
        car.SetActive(true); 
        return car;
    }

    public void FindCarAndReset()
    {
        // Find all the cars out of camera and remove them
        for (int i = 0; i < activeCars.transform.childCount; i++)
        {
            if (activeCars.transform.GetChild(i).position.z < pathManager.distroyDistance)
            {
                GameObject theCar = activeCars.transform.GetChild(i).gameObject;
                theCar.gameObject.SetActive(false);
                theCar.transform.parent = unactiveCars.transform;
            }
        }
    }


}
