using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : Agent
{
    private const int CostPerChild = 5;

    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;

    private int Speed;
    private int ViewDist;
    private int Fertility;

    private int FoodEaten = 0;
    private bool bReady = false;
    private GameObject CurrentFood;
    private string[] potNames = { "lilCutie", "Fred", "Joe", "Sally", "BigCutie", "Angelina", "Jeffry", "Keila", "Brandon", "Alden", "Jaydon", "Julissa", "Geneva", "Shakayla", "Kamron", "Hayden", "Dillion"};

    public override GType CreateGType(Gene[] initGenes)
    {
        return new GType(initGenes);
    }

    public override int FitnessFunction()
    {
        return FoodEaten;
    }

    public override string PrintAgent()
    {
        string randName = potNames[Random.Range(0, potNames.Length)];
        return "Best of Generation:\n----------\nName: " + randName + "\nFitness Score {" + this.FitnessFunction() + "}:\n" + "Lifespan: " + GetGType().GetGenome()[0].GeneValue + "\nView Distance: " + ViewDist + "\nSpeed: " + Speed + "\nControl Gene: " + Fertility + "\n----------\n";
    }

    public override void InterpretGType()
    {
        Gene[] genes = GetGType().GetGenome();
        Lifespan = genes[0].GeneValue;
        ViewDist = genes[1].GeneValue;
        Speed = genes[2].GeneValue;
        Fertility = genes[3].GeneValue;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!GetDead()) SetPotentialChildren((FoodEaten/CostPerChild));
        if (bReady && gameObject.activeSelf)
        {
            //RUN BLOB
            CharacterController controller = GetComponent<CharacterController>();
            if (controller.isGrounded)
            {
                if (Owner == null || (Blob_Simulator)Owner == null) return;
                if (CurrentFood == null) CurrentFood = ((Blob_Simulator)Owner).GetFoodByViewDist(ViewDist, gameObject);
                if (CurrentFood != null)
                {
                    moveDirection = CurrentFood.transform.position - gameObject.transform.position;
                    moveDirection.Normalize();
                }
                else
                {
                    moveDirection = transform.forward;
                }
                moveDirection *= Speed;
                moveDirection.y = 0;
            }
            else
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Food>() != null)
        {
            Destroy(collision.gameObject);
            CurrentFood = null;
            FoodEaten++;
        }
    }

    public override void StartSimulation()
    {
        bReady = true;
    }
}
