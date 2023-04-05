using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GType
{
    private List<Gene> Genome;

    public static Gene[] ScaleGenome(Gene[] inGenome, int MaxGenes)
    {
        int GenomeSize = inGenome.Length;
        int[] newGenes = new int[GenomeSize];
        int sum = 0;
        //Debug.Log(inGenome.Length);
        for (int i = 0; i < GenomeSize; i++)
        {
            sum += inGenome[i].GeneValue;
        }
        //Debug.Log("SUM: " + sum);

        int sum2 = 0;
        for (int i = 0; i < GenomeSize; i++)
        {
            newGenes[i] = (MaxGenes * inGenome[i].GeneValue) / sum;
            sum2 += newGenes[i];
        }
        //Debug.Log("SUM2: " + sum2);

        int remainder = MaxGenes - sum2;
        //Debug.Log("REMAINDER: " + remainder);
        if (remainder < 0)
        {
            for (int i = 0; i < (remainder * -1); i++)
            {
                int randPos = Random.Range(0, GenomeSize);
                newGenes[randPos] -= 1;
            }
        }
        else if (remainder > 0)
        {
            for (int i = 0; i < remainder; i++)
            {
                int randPos = Random.Range(0, GenomeSize);
                newGenes[randPos] += 1;
            }
        }

        Gene[] returnArr = new Gene[GenomeSize];
        for (int i = 0; i < GenomeSize; i++)
        {
            returnArr[i] = new Gene("0", newGenes[i]);
        }
        return returnArr;
    }

    public GType(Gene[] initGenes)
    {
        Genome = new List<Gene>(initGenes);
    }

    public Gene GetGeneAt(int index)
    {
        return Genome[index];
    }

    public Gene[] GetGenome()
    {
        return Genome.ToArray();
    }

    public void SetGenome(Gene[] newGenes)
    {
        Genome = new List<Gene>(newGenes);
    }

    public static Gene[] MutateGenome(Gene[] Genome1, Gene[] Genome2, int MaxGeneValue)
    {
        return Mutate(Crossover(Genome1, Genome2), MaxGeneValue);
    }

    static Gene[] Crossover(Gene[] Genome1, Gene[] Genome2)
    {
        int SplitPoint = Random.Range(0, Genome1.Length);
        Gene[] arr = new Gene[Genome1.Length];
        for (int i = 0; i < Genome1.Length; i++)
        {
            if (i < SplitPoint)
            {
                arr[i] = new Gene(Genome1[i].GeneName, Genome1[i].GeneValue);
            }
            else
            {
                arr[i] = new Gene(Genome2[i].GeneName, Genome2[i].GeneValue);
            }
        }
        return arr;
    }

    static Gene[] Mutate(Gene[] Genome, int MaxGeneValue)
    {
        Gene[] arr = Genome;
        //Generate Mutation Positions
        int pos = Random.Range(0, Genome.Length);

        //Roll for Random
        if (Random.Range(0, Genome.Length) == 0)
        {
            int newVal = Random.Range(0, MaxGeneValue);
            arr[pos] = new Gene(arr[pos].GeneName, newVal);
        }
        arr = ScaleGenome(arr, MaxGeneValue);
        return arr;
    }
}
