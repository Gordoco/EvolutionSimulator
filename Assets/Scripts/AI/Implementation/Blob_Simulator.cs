using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob_Simulator : Simulator
{
    public GameObject FoodType;
    public int NumFood = 100;

    private List<GameObject> Food;

    protected override void StartGeneration()
    {
        SpawnFood();
        base.StartGeneration();
    }

    void SpawnFood()
    {
        if (Food != null)
        {
            for (int i = 0; i < Food.Count; i++)
            {
                Destroy(Food[i]);
            }
            Food.Clear();
        }
        Food = new List<GameObject>();
        for (int i = 0; i < NumFood; i++)
        {
            Food.Add(Instantiate(FoodType, new Vector3(Random.Range(-200, 200), 70, Random.Range(-200, 200)), Quaternion.identity));
        }
    }

    public GameObject GetFoodByViewDist(int ViewDist, GameObject Entity)
    {
        for (int i = Food.Count - 1; i >= 0; i--)
        {
            if (Food[i] == null)
            {
                Food.RemoveAt(i);
                continue;
            }
            if (Vector3.Distance(Entity.transform.position, Food[i].transform.position) <= (ViewDist)) return Food[i];
        }
        return null;
    }

    protected override bool CheckConvergence()
    {
        bool bDone = true;
        for (int i = 0; i < GetCurrentPopulation().Length; i++)
        {
            if (GetCurrentPopulation()[i] == null) continue;
            if (!GetCurrentPopulation()[i].GetComponent<Agent>().GetDead()) bDone = false;
        }
        return bDone;
    }

    protected override Vector3[] GetGenerationTransforms()
    {
        Vector3[] Locations = new Vector3[PopulationSize];
        for (int i = 0; i < PopulationSize; i++)
        {
            Locations[i] = new Vector3(Random.Range(-200, 200), 70, Random.Range(-200, 200));
        }
        return Locations;
    }

    protected override Vector3 GetGenerationTransform()
    {
        return new Vector3(Random.Range(-200, 200), 70, Random.Range(-200, 200));
    }
}
