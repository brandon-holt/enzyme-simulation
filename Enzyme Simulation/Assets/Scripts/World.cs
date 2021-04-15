using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class World : MonoBehaviour
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

    public void SaveWorld(string type)
    {
        string file = type + "_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".enzyme";

        Save(file);
    }

    public void SaveWorldWithFullFileName(string fullName) { Save(fullName); }

    private void Save(string file)
    {
        Global.lastSavedFile = file;

        // grab data from Global.sim instance and put in correct variables

        ny1 = Global.sim.ny1;
        ny2 = Global.sim.ny2;
        ne = Global.sim.ne;
        substrate_names = Global.sim.substrate_names;
        enzyme_names = Global.sim.enzyme_names;
        Kon = Global.sim.Kon;
        Koff = Global.sim.Koff;
        Kcat = Global.sim.Kcat;
        pJump = Global.sim.pJump;
        timeStep = Global.sim.timeStep;
        tMax = Global.sim.tMax;
        t = Global.sim.t.ToArray();
        description = Global.sim.description;

        enzyme_colors = new float[Global.sim.enzyme_colors.GetLength(0), 3];
        for (int i = 0; i < Global.sim.enzyme_colors.GetLength(0); i++)
        {
            enzyme_colors[i, 0] = Global.sim.enzyme_colors[i].r;
            enzyme_colors[i, 1] = Global.sim.enzyme_colors[i].g;
            enzyme_colors[i, 2] = Global.sim.enzyme_colors[i].b;
        }

        substrate_colors = new float[Global.sim.substrate_colors.GetLength(0), Global.sim.substrate_colors.GetLength(1), 3];
        for (int i = 0; i < Global.sim.substrate_colors.GetLength(0); i++)
        {
            for (int j = 0; j < Global.sim.substrate_colors.GetLength(1); j++)
            {
                substrate_colors[i, j, 0] = Global.sim.substrate_colors[i, j].r;
                substrate_colors[i, j, 1] = Global.sim.substrate_colors[i, j].g;
                substrate_colors[i, j, 2] = Global.sim.substrate_colors[i, j].b;
            }
        }

        sy1 = new int[Global.sim.sy1.Length, Global.sim.sy1[0].Count];
        py1 = new int[Global.sim.sy1.Length, Global.sim.sy1[0].Count];
        sy2 = new int[Global.sim.sy1.Length, Global.sim.sy1[0].Count];
        iy2 = new int[Global.sim.sy1.Length, Global.sim.sy1[0].Count];
        py2 = new int[Global.sim.sy1.Length, Global.sim.sy1[0].Count];
        for (int i = 0; i < Global.sim.sy1.Length; i++)
        {
            for (int j = 0; j < Global.sim.sy1[0].Count; j++)
            {
                sy1[i, j] = Global.sim.sy1[i][j];
                py1[i, j] = Global.sim.py1[i][j];
                sy2[i, j] = Global.sim.sy2[i][j];
                iy2[i, j] = Global.sim.iy2[i][j];
                py2[i, j] = Global.sim.py2[i][j];
            }
        }

        SaveSystem.SaveScores(this, "/" + file);
    }

    public bool LoadWorld(string file)
    {

        WorldData data = SaveSystem.LoadScores("/" + file);

        if (data == null) { return false; }

        // load from data into Global.sim instance
        description = data.description;

        Global.sim.ny1 = data.ny1;
        Global.sim.ny2 = data.ny2;
        Global.sim.ne = data.ne;
        Global.sim.substrate_names = data.substrate_names;
        Global.sim.enzyme_names = data.enzyme_names;
        Global.sim.Kon = data.Kon;
        Global.sim.Koff = data.Koff;
        Global.sim.Kcat = data.Kcat;
        Global.sim.pJump = data.pJump;
        Global.sim.timeStep = data.timeStep;
        Global.sim.tMax = data.tMax;
        Global.sim.description = data.description;

        Global.sim.ResetDataArrays();

        Global.sim.t = new List<float>();
        for (int i = 0; i < data.t.Length; i++) { Global.sim.t.Add(data.t[i]); }

        Global.sim.enzyme_colors = new Color[data.enzyme_colors.GetLength(0)];
        for (int i = 0; i < data.enzyme_colors.GetLength(0); i++)
        { Global.sim.enzyme_colors[i] = new Color(data.enzyme_colors[i, 0], data.enzyme_colors[i, 1], data.enzyme_colors[i, 2]); }

        Global.sim.substrate_colors = new Color[data.substrate_colors.GetLength(0), data.substrate_colors.GetLength(1)];
        for (int i = 0; i < data.substrate_colors.GetLength(0); i++)
        {
            for (int j = 0; j < data.substrate_colors.GetLength(1); j++)
            {
                Global.sim.substrate_colors[i, j] = new Color(data.substrate_colors[i, j, 0], data.substrate_colors[i, j, 1], data.substrate_colors[i, j, 2]);
            }
        }

        for (int i = 0; i < data.sy1.GetLength(0); i++)
        {
            for (int j = 0; j < data.sy1.GetLength(1); j++)
            {
                Global.sim.sy1[i].Add(data.sy1[i, j]);
                Global.sim.py1[i].Add(data.py1[i, j]);
                Global.sim.sy2[i].Add(data.sy2[i, j]);
                Global.sim.iy2[i].Add(data.iy2[i, j]);
                Global.sim.py2[i].Add(data.py2[i, j]);
            }
        }
        
        return true;
    }

    public void DeleteWorld(string file)
    {
        //file = Application.persistentDataPath + "/" + file;

        File.Delete(file);
    }


}
