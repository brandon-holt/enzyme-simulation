[System.Serializable]
public class WorldData
{
    public int[] ny1, ny2, ne;
    public string[] substrate_names, enzyme_names;
    public float[,,] substrate_colors; // [i, j, rgb]
    public float[,] enzyme_colors; // [i, rgb]
    public float[,,] Kon, Koff, Kcat; // [sub, enz, side]
    public float pJump, timeStep, tMax;
    public float[] t;
    public int[,] sy1, py1, sy2, iy2, py2; // [si, timepoint]
    public string description;

    public WorldData(World data)
    {
        ny1 = data.ny1;
        ny2 = data.ny2;
        ne = data.ne;
        substrate_names = data.substrate_names;
        enzyme_names = data.enzyme_names;
        substrate_colors = data.substrate_colors;
        enzyme_colors = data.enzyme_colors;
        Kon = data.Kon;
        Koff = data.Koff;
        Kcat = data.Kcat;
        pJump = data.pJump;
        timeStep = data.timeStep;
        tMax = data.tMax;
        t = data.t;
        sy1 = data.sy1;
        py1 = data.py1;
        sy2 = data.sy2;
        iy2 = data.iy2;
        py2 = data.py2;
        description = data.description;
    }

}
