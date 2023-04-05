using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : PType
{
    private GameObject PrefabType;
    private int PotentialChildren = 0;
    private bool bDead = false;

    public Simulator Owner;
    protected float Lifespan = 10f; //(seconds)

    // Start is called before the first frame update
    void Start()
    { }

    // Update is called once per frame
    protected virtual void Update()
    {
        Lifespan -= (2*Time.deltaTime);
        if (Lifespan <= 0) Die();
    }

    protected virtual void Die()
    {
        bDead = true;
        gameObject.SetActive(false);
    }

    public bool GetDead() { return bDead; }

    public abstract void StartSimulation();

    public abstract int FitnessFunction();

    public abstract string PrintAgent();

    public int GetPotentialChildren() { return PotentialChildren; }
    public void DecrementPotentialChildren() { PotentialChildren--; }
    protected void SetPotentialChildren(int val) { PotentialChildren = val; }

    public GameObject GetObjectType() { return PrefabType; }
}
