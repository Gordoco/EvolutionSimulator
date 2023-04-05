using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct GeneBound {
    public readonly int[] lowers;
    public readonly int[] uppers;

    public GeneBound(int[] lowers, int[] uppers)
    {
        this.lowers = lowers;
        this.uppers = uppers;
    }
}

public abstract class PType : MonoBehaviour
{
    private GType CurrentGType;
    private int GenomeSize = 4;

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}

    public abstract GType CreateGType(Gene[] initGenes);

    public void AssignGType(GType newGType)
    {
        CurrentGType = newGType;
        InterpretGType();
    }

    public int GetGenomeSize()
    {
        return GenomeSize;
    }

    public GType GetGType()
    {
        return CurrentGType;
    }
    public abstract void InterpretGType();
}
