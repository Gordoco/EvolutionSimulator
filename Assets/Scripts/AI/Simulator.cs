using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Simulator : MonoBehaviour
{
    public int PopulationSize = 10;
    public int MaxIndividualGenes = 100;
    public GameObject AgentType;
    public TMP_Text TextObj;

    private List<GameObject> Population;
    private bool bReady = false;
    private bool bCollapsed = false;
    private int Generation = 0;
    private string StartText = "";
    private string GenerationText = "";
    private string BestOfText = "";
    private string EndText = "";
    private float startDelay = 0.5f;

    // Start is called before the first frame update
    void Start() {}

    protected int GetGeneration() { return Generation; }

    protected void InitializeSimulation()
    {
        GameObject Type = AgentType;
        int GenomeSize = 4;
        Population = new List<GameObject>();
        Vector3[] SpawnLocations = GetGenerationTransforms();
        //Debug.Log("Genome Size: " + GenomeSize + " MaxGeneVal: " + MaxIndividualGenes);
        StartText = "Genome Size: " + GenomeSize + "\nMaxGeneVal: " + MaxIndividualGenes + "\n";
        for (int i = 0; i < PopulationSize; i++)
        {
            GameObject NextAgent = Instantiate(Type, SpawnLocations[i], Quaternion.Euler(0, Random.Range(-180, 180), 0));
            Agent NextAgentLogic = NextAgent.GetComponent<Agent>();
            NextAgentLogic.Owner = this;
            GType InitGType = NextAgentLogic.CreateGType(GenerateRandomGenome(GenomeSize, MaxIndividualGenes));
            NextAgentLogic.AssignGType(InitGType);
            Population.Add(NextAgent);
        }
    }

    protected virtual void StartGeneration()
    {
        //Debug.Log("Generation Number: " + Generation);
        GenerationText = "Generation Number: " + Generation + "\n";
        float Avg_Speed = 0;
        float Avg_ViewDist = 0;
        float Avg_Lifespan = 0;
        float Avg_ControlGene = 0;
        for (int i = 0; i < PopulationSize; i++)
        {
            Agent Logic = Population[i].GetComponent<Agent>();
            Gene[] Genes = Logic.GetGType().GetGenome();
            Avg_Lifespan += Genes[0].GeneValue;
            Avg_ViewDist += Genes[1].GeneValue;
            Avg_Speed += Genes[2].GeneValue;
            Avg_ControlGene += Genes[3].GeneValue;
            Logic.StartSimulation();
        }
        Avg_Speed /= PopulationSize;
        Avg_ViewDist /= PopulationSize;
        Avg_Lifespan /= PopulationSize;
        Avg_ControlGene /= PopulationSize;
        //Debug.Log("Population: " + PopulationSize);
        GenerationText += "AvgSpeed: " + Avg_Speed + "\n";
        GenerationText += "AvgViewDist: " + Avg_ViewDist + "\n";
        GenerationText += "AvgLifespan: " + Avg_Lifespan + "\n";
        GenerationText += "AvgControlGene: " + Avg_ControlGene + "\n";

        GenerationText += "Population: " + PopulationSize + "\n";
    }

    GameObject CreatePopulationMember(Gene[] Genome, Vector3 Location)
    {
        GameObject Type = AgentType;
        GameObject NextAgent = Instantiate(Type, Location, Quaternion.Euler(0, Random.Range(-180, 180), 0));
        Agent NextAgentLogic = NextAgent.GetComponent<Agent>();
        NextAgentLogic.Owner = this;
        GType InitGType = NextAgentLogic.CreateGType(Genome);
        NextAgentLogic.AssignGType(InitGType);
        return NextAgent;
    }

    Gene[] GenerateRandomGenome(int GenomeSize, int MaxGenes)
    {
        Gene[] newGenes = new Gene[GenomeSize];
        for (int i = 0; i < GenomeSize; i++)
        {
            newGenes[i] = new Gene("0", Random.Range(0, MaxGenes));
        }

        return GType.ScaleGenome(newGenes, MaxGenes);
    }

    List<Agent> SelectParents()
    {
        List<Agent> Parents = new List<Agent>();
        for (int i = 0; i < GetCurrentPopulation().Length; i++)
        {
            if (Population[i] == null) continue;
            Parents.Add(Population[i].GetComponent<Agent>());
        }
        return Parents;
    }

    void DestroyCurrentPopulation()
    {
        if (Population.Count != PopulationSize)
        {
            Debug.Log(PopulationSize);
            return;
        }
        for (int i = 0; i < PopulationSize; i++)
        {
            Destroy(Population[i]);
        }
        Population.Clear();
    }

    List<GameObject> MakeBabies(List<Agent> Parents)
    {
        List<GameObject> NewPopulation = new List<GameObject>();
        int count = 0;
        while (Parents.Count > 0)
        {
            int randSum = 0;
            //Clear Parents who cannot have more kids, Sum fitness for random generation
            for (int i = Parents.Count - 1; i >= 0; i--)
            {
                if (Parents[i].GetPotentialChildren() <= 0)
                {
                    Parents.RemoveAt(i);
                    continue;
                }
                randSum += Parents[i].FitnessFunction();
            }
            int chosenParent1 = new System.Random().Next(0, randSum);
            int chosenParent2 = new System.Random().Next(0, randSum);
            Agent[] ChosenParents = GetSelectedParents(chosenParent1, chosenParent2, Parents);
            if (ChosenParents[0] == null && ChosenParents[1] == null)
            {
                //Debug.Log("COLLAPSED");
                EndText = "COLLAPSED\n";
                bCollapsed = true;
                return null;
            }
            else if (ChosenParents[0] == null)
            {
                Parents.Remove(ChosenParents[1]);
                continue;
            }
            else if (ChosenParents[1] == null)
            {
                Parents.Remove(ChosenParents[0]);
                continue;
            }
            Gene[] Genome = GType.MutateGenome(ChosenParents[0].GetGType().GetGenome(), ChosenParents[1].GetGType().GetGenome(), MaxIndividualGenes);
            NewPopulation.Add(CreatePopulationMember(Genome, GetGenerationTransform()));
            for (int j = 0; j < 2; j++)
            {
                ChosenParents[j].DecrementPotentialChildren();
                if (ChosenParents[j].GetPotentialChildren() <= 0) Parents.Remove(ChosenParents[j]);
            }
            count++;
        }
        if (NewPopulation.Count <= 0)
        {
            //Debug.Log("COLLAPSED");
            EndText = "COLLAPSED\n";
            bCollapsed = true;
            return null;
        }
        return NewPopulation;
    }

    Agent[] GetSelectedParents(int chosenParentVal1, int chosenParentVal2, List<Agent> Parents)
    {
        Agent[] arr = new Agent[2];
        bool[] chosen = { false, false };
        int sum = 0;
        for (int i = Parents.Count - 1; i >= 0; i--)
        {
            sum += Parents[i].FitnessFunction();
            if (!chosen[0] && (sum > chosenParentVal1 || sum == 0 && chosenParentVal1 == 0))
            {
                arr[0] = Parents[i];
                chosen[0] = true;
            }
            else if (!chosen[1] && (sum > chosenParentVal2 || sum == 0 && chosenParentVal2 == 0))
            {
                arr[1] = Parents[i];
                chosen[1] = true;
            }
            if (chosen[0] && chosen[1]) break;
        }
        //Debug.Log(chosenParentVal1);
        return arr;
    }

    protected GameObject[] GetCurrentPopulation()
    {
        return Population.ToArray();
    }

    //Needs to return one transform for each of PopulationSize
    protected abstract Vector3[] GetGenerationTransforms();
    protected abstract Vector3 GetGenerationTransform();

    protected abstract bool CheckConvergence();

    protected virtual void FinishGeneration()
    {
        Generation++;
        Agent FormerBest = GetBestOfGeneration();
        if (FormerBest != null) BestOfText = FormerBest.PrintAgent();
        List<Agent> Parents = SelectParents();
        List<GameObject> NewPopulation = MakeBabies(Parents);
        DestroyCurrentPopulation();
        if (!bCollapsed)
        {
            Population = NewPopulation;
            PopulationSize = NewPopulation.Count;
            StartGeneration();
        }
    }

    Agent GetBestOfGeneration()
    {
        int max = int.MinValue;
        Agent NextAgent = null;
        for (int i = 0; i < Population.Count; i++)
        {
            Agent potAgent = Population[i].GetComponent<Agent>();
            if (potAgent.FitnessFunction() > max)
            {
                NextAgent = potAgent;
                max = NextAgent.FitnessFunction();
            }
        }
        return NextAgent;
    }

    // Update is called once per frame
    void Update()
    {
        if (bReady)
        {
            TextObj.SetText(StartText + GenerationText + "\n" + BestOfText + EndText);
            if (CheckConvergence() && !bCollapsed) FinishGeneration();
        }
        else if (!bReady)
        {
            if (startDelay <= 0)
            {
                InitializeSimulation();
                bReady = true;
                StartGeneration();
            }
            else
            {
                startDelay -= Time.deltaTime;
            }
        }
    }
}
