using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Voxus.Random;
using MEC;

public class Simulate : MonoBehaviour
{
    // limit player inputs
    public int limit_subs, limit_enzs;

    // inputs
    public int[] ny1, ny2, ne;
    public string[] substrate_names, enzyme_names;
    public Color[,] substrate_colors; public Color[] enzyme_colors;
    public float[,,] Kon, Koff, Kcat; // [sub,  enz, side]
    public float pJump, timeStep, tMax;

    // outputs
    public List<float> t = new List<float>();
    public List<int>[] sy1, py1, sy2, iy2, py2;

    // variables
    public string mode;
    public bool lessData; // don't save time series data, only latest values
    private List<int[]> enzymes = new List<int[]>();
    private List<float[,,]> YES1 = new List<float[,,]>();
    private List<float[,,]> YES2 = new List<float[,,]>();
    private int max_copies, max_substrates;
    public float currentTime, max_progress;
    public bool finish;
    public string description;
    private static Simulate instance;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this); instance = this;
            Global.sim = this;
        }
        else if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    public void InitializeParameters()
    {
        RandomizeKinetics(); // if we already have Kon and its the right size, don't do anything

        ResetDataArrays(); // always do this

        // other variables used in simulation
        pJump = .2f;
        timeStep = .1f;
        tMax = 400f;
        max_copies = Mathf.Max(Mathf.Max(ny1), Mathf.Max(ny2));
        max_substrates = Mathf.Max(ny1.Length, ny2.Length);
    }

    private void RandomizeKinetics()
    {
        if (Kon != null) // if we already have Kon and its the right size, don't do anything
        { if (Kon.GetLength(0) == ny1.Length && Kon.GetLength(1) == ne.Length) { return; } }

        // initialize kinetic parameter arrays
        Kon = new float[ny1.Length, ne.Length, 2];
        Koff = new float[ny1.Length, ne.Length, 2];
        Kcat = new float[ny1.Length, ne.Length, 2];

        // randomize these arrays to start
        for (int i = 0; i < ny1.Length; i++)
        {
            for (int j = 0; j < ne.Length; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Kon[i, j, k] = Random.Range(1f, 10f);
                    Koff[i, j, k] = Random.Range(1f, 3f);
                    Kcat[i, j, k] = Random.Range(1f, 10f);
                }
            }
        }
    }

    public void ResetDataArrays()
    {
        // reset lists
        t = new List<float>();
        enzymes = new List<int[]>();
        YES1 = new List<float[,,]>();
        YES2 = new List<float[,,]>();

        // intialize array of lists for outputs
        sy1 = new List<int>[ny1.Length];
        for (int i = 0; i < sy1.Length; i++) { sy1[i] = new List<int>(); }
        py1 = new List<int>[ny1.Length];
        for (int i = 0; i < py1.Length; i++) { py1[i] = new List<int>(); }
        sy2 = new List<int>[ny2.Length];
        for (int i = 0; i < sy2.Length; i++) { sy2[i] = new List<int>(); }
        iy2 = new List<int>[ny2.Length];
        for (int i = 0; i < iy2.Length; i++) { iy2[i] = new List<int>(); }
        py2 = new List<int>[ny2.Length];
        for (int i = 0; i < py2.Length; i++) { py2[i] = new List<int>(); }

        // initialize state arrays for enzymes and substrates
        for (int i = 0; i < ne.Length; i++) { enzymes.Add(new int[ne[i]]); }
        for (int i = 0; i < ny1.Length; i++) { YES1.Add(new float[ny1[i], 1, 3]); }
        for (int i = 0; i < ny2.Length; i++) { YES2.Add(new float[ny2[i], 2, 3]); }
        ResetAllTaskAndTime(YES1);
        ResetAllTaskAndTime(YES2);

        // to begin new simulation
        currentTime = 0f; max_progress = 0f;
        finish = false;
    }

    public void Activity()
    {
        ResetDataArrays();

        System.GC.Collect();

        Timing.RunCoroutine(_SimulateActivity());
    }

    private IEnumerator<float> _SimulateActivity()
    {
        while (currentTime < tMax && !finish)
        {
            RecordData();

            UpdateProgress();

            ProcessAllSubstrates();

            currentTime += timeStep;

            yield return Timing.WaitForOneFrame;
        }

        System.GC.Collect();

        finish = true;
    }

    private void RecordData()
    {
        if (lessData)
        {
            for (int i = 0; i < max_substrates; i++)
            {
                if (py1[i].Count == 0) { py1[i].Add(0); }
                float[] indexedRangeYES1 = IndexRange1D(YES1[i], 0, YES1[i].GetLength(0) - 1, 0, 0);
                py1[i][0] = CountEqualTo(indexedRangeYES1, ne.Length + 1);
                if (py2[i].Count == 0) { py2[i].Add(0); }
                float[] indexedRangeYES2 = SumAcross(IndexRange3D(YES2[i], 0, YES2[i].GetLength(0) - 1, 0, 1, 0, 0));
                py2[i][0] = CountEqualTo(indexedRangeYES2, 2f * ne.Length + 2f);
            }
        }
        else
        {
            t.Add(currentTime);

            for (int i = 0; i < max_substrates; i++)
            {
                float[] indexedRangeYES1 = IndexRange1D(YES1[i], 0, YES1[i].GetLength(0) - 1, 0, 0);
                sy1[i].Add(CountEqualTo(indexedRangeYES1, 0f));
                py1[i].Add(CountEqualTo(indexedRangeYES1, ne.Length + 1));

                float[] indexedRangeYES2 = SumAcross(IndexRange3D(YES2[i], 0, YES2[i].GetLength(0) - 1, 0, 1, 0, 0));
                sy2[i].Add(CountEqualTo(indexedRangeYES2, 0f));
                py2[i].Add(CountEqualTo(indexedRangeYES2, 2f * ne.Length + 2f));
                iy2[i].Add(ny2[i] - sy2[i].Last() - py2[i].Last());
            }
        }

    }

    private void UpdateProgress()
    {
        for (int i = 0; i < max_substrates; i++)
        {
            float current_progress = Mathf.Max(currentTime / tMax, 1f * py1[i].Last() / (ny1[i] + 1), 1f * py2[i].Last() / (ny2[i] + 1));
            max_progress = Mathf.Max(max_progress, current_progress);

            float seconds_post_ss = 5f;
            int last_x = Mathf.RoundToInt(seconds_post_ss / timeStep);
            if (py1[i].Count < last_x) { continue; }
            bool condition_met = true;
            for (int x = py1[i].Count - last_x; x < py1[i].Count; x++)
            {
                if (py1[i][x] != ny1[i]) { condition_met = false; break; }
            }
            for (int x = py2[i].Count - last_x; x < py2[i].Count; x++)
            {
                if (py2[i][x] != ny2[i]) { condition_met = false; break; }
            }
            finish = condition_met; // if all of the above conditions are met
        }
    }

    private void ProcessAllSubstrates()
    {
        int[] perm0 = RandPerm(max_copies);
        int[] perm1 = RandPerm(max_copies);
        int[] perm2 = RandPerm(max_copies);

        for (int ci = 0; ci < max_copies; ci++)
        {
            for (int si = 0; si < max_substrates; si++)
            {
                // for the YES substrate
                int ci0 = perm0[ci];
                if (si < YES1.Count)
                {
                    if (ci0 < YES1[si].GetLength(0))
                    {
                        float[,,] vec = IndexRange3D(YES1[si], ci0, ci0, 0, 0, 0, 2);
                        vec = Process(vec, si, ci0, 0);
                        UpdateSubstrate(YES1, vec, si, ci0, 0);
                    }
                }

                // for the YES2 substrate 1
                int ci1 = perm1[ci];
                if (si < YES2.Count)
                {
                    if (ci1 < YES2[si].GetLength(0))
                    {
                        float[,,] vec = IndexRange3D(YES2[si], ci1, ci1, 0, 0, 0, 2);
                        vec = Process(vec, si, ci1, 1);
                        UpdateSubstrate(YES2, vec, si, ci1, 0);
                    }
                }

                // for the YES2 substrate 2
                int ci2 = perm2[ci];
                if (si < YES2.Count)
                {
                    if (ci2 < YES2[si].GetLength(0))
                    {
                        float[,,] vec = IndexRange3D(YES2[si], ci2, ci2, 1, 1, 0, 2);
                        vec = Process(vec, si, ci2, 2);
                        UpdateSubstrate(YES2, vec, si, ci2, 1);
                    }
                }

            }
        }

    }

    private void UpdateSubstrate(List<float[,,]> YES, float[,,] vec, int si, int ci, int j)
    { for (int k = 0; k < 3; k++) { YES[si][ci, j, k] = vec[0, 0, k]; } }

    // process this substrate and return the result
    private float[,,] Process(float[,,] vec, int si, int ci, int y2i)
    {

        // these are coded indicies
        int state = 0; int task = 1; int time = 2;

        if (!float.IsNaN(vec[0, 0, task])) // if a task is in progress
        {
            if (vec[0, 0, time] - timeStep <= 0)
            {
                int ei = (int)vec[0, 0, state] - 1;
                if (vec[0, 0, task] == 0 || vec[0, 0, task] == ne.Length + 1)
                {
                    int boundEnzyme = FindFirst(enzymes[ei], 1);
                    if (boundEnzyme == -1) { return vec; }
                    enzymes[ei][boundEnzyme] = 0; // unbind the enzyme
                    if (y2i > 0 && Random.value < pJump && YES2[si][ci, 2 - y2i, state] == 0 && float.IsNaN(YES2[si][ci, 2 - y2i, task]))
                    {
                        enzymes[ei][boundEnzyme] = 1;
                        YES2[si][ci, 2 - y2i, task] = ei + 1;
                        YES2[si][ci, 2 - y2i, time] = ExpRnd(Kon[si, ei, y2i - 1]);
                    }
                }
                vec[0, 0, state] = vec[0, 0, task];
                vec[0, 0, task] = float.NaN;
                vec[0, 0, time] = float.NaN;
            }
            else
            {
                vec[0, 0, time] = vec[0, 0, time] - timeStep;
            }

        }
        else if (vec[0, 0, state] == 0) // else if the substrate is free of enzyme
        {
            if (y2i == 0) { y2i = 1; }
            int[] free_enzymes = new int[ne.Length];
            for (int i = 0; i < ne.Length; i++) { free_enzymes[i] = CountEqualTo(enzymes[i], 0); }
            float[] cum_dist = new float[ne.Length];
            for (int i = 0; i < cum_dist.Length; i++) { cum_dist[i] = Kon[si, i, y2i - 1] * ne[i]; } // normalize to Kon
            float total = Sum(cum_dist);
            for (int i = 0; i < cum_dist.Length; i++) { cum_dist[i] = 1f * cum_dist[i] / total; } // self normalize
            int rand_e = Sample(cum_dist); // index of randomly encountered enzyme
            if (Random.value < 1f * free_enzymes[rand_e] / ne[rand_e])
            {
                int freeEnzyme = FindFirst(enzymes[rand_e], 0);
                enzymes[rand_e][freeEnzyme] = 1;
                vec[0, 0, task] = rand_e + 1;
                vec[0, 0, time] = ExpRnd(Kon[si, rand_e, y2i - 1]);
            }

        }
        else if (vec[0, 0, state] != ne.Length + 1) // else if the substrate is bound to an enzyme
        {

            if (y2i == 0) { y2i = 1; }
            int ei = (int)vec[0, 0, state] - 1;
            float t_to_finish_cutting = ExpRnd(Kcat[si, ei, y2i - 1]);
            float t_to_unbind = ExpRnd(Koff[si, ei, y2i - 1]);
            if (t_to_unbind < t_to_finish_cutting)
            {
                vec[0, 0, task] = 0;
                vec[0, 0, time] = t_to_unbind;
            }
            else
            {
                vec[0, 0, task] = ne.Length + 1;
                vec[0, 0, time] = t_to_finish_cutting;
            }

        }

        return vec;
    }

    // **********************************************************
    // ******FUNCTIONS I NEED BECAUSE ITS NOT MATLAB*************
    // **********************************************************

    private int CountEqualTo(float[] array, float val)
    {
        int count = 0;
        foreach (float a in array) { if (a == val) { count += 1; } }
        return count;
    }
    private int CountEqualTo(int[] array, int val)
    {
        int count = 0;
        foreach (int a in array) { if (a == val) { count += 1; } }
        return count;
    }

    private float[,,] IndexRange3D(float[,,] array, int i1, int i2, int j1, int j2, int k1, int k2)
    {
        int iLength = i2 - i1 + 1;
        int jLength = j2 - j1 + 1;
        int kLength = k2 - k1 + 1;
        float[,,] arrayOut = new float[iLength, jLength, kLength];

        for (int i = 0; i < iLength; i++) {
            for (int j = 0; j < jLength; j++) {
                for (int k = 0; k < kLength; k++) {
                    arrayOut[i, j, k] = array[i1 + i, j1 + j, k1 + k]; } } }

        return arrayOut;
    }

    private float[] IndexRange1D(float[,,] array, int i1, int i2, int j, int k)
    {
        int iLength = i2 - i1 + 1;
        float[] arrayOut = new float[iLength];
        for (int i = 0; i < iLength; i++)
        {
            arrayOut[i] = array[i1 + i, j, k];
        }
        return arrayOut;
    }

    private float[] SumAcross(float[,,] array)
    {
        float[] arrayOut = new float[array.GetLength(0)];

        for (int i = 0; i < array.GetLength(0); i++)
        {
            float thisSum = 0f;
            for (int j = 0; j < array.GetLength(1); j++)
            {
                thisSum += array[i, j, 0];
            }
            arrayOut[i] = thisSum;
        }

        return arrayOut;
    }

    private int[] RandPerm(int length)
    {
        int[] randperm = new int[length];
        for (int i = 0; i < length; i++) { randperm[i] = i; }

        // shuffles the ordered indicies like a deck of cards
        for (int i = 0; i < length; i++)
        {
            int rnd = Random.Range(0, length);
            int temp = randperm[rnd];
            randperm[rnd] = randperm[i];
            randperm[i] = temp;
        }

        return randperm;
    }

    private float ExpRnd(float lambda)
    {
        var rng = new RandomExponential(0, lambda);
        return rng.Get();
    }

    private void ResetAllTaskAndTime(List<float[,,]> YES)
    {
        for (int x = 0; x < YES.Count; x++)
        {
            for (int i = 0; i < YES[x].GetLength(0); i++)
            {
                for (int j = 0; j < YES[x].GetLength(1); j++)
                {
                    for (int k = 1; k < YES[x].GetLength(2); k++)
                    {
                        YES[x][i, j, k] = float.NaN;
                    }
                }
            }
        }
    }

    private int FindFirst(int[] arr, int val)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == val) { return i; }
        }
        return -1; // this is my empty or null value
    }

    //	Sum of the values in the supplied integer array.
    public int Sum(int[] ints)
    {
        int result = 0;

        for (int i = 0; i < ints.Length; i++)
        {
            result += ints[i];
        }

        return result;
    }

    //	Sum of the values in the supplied float array.	
    public float Sum(float[] floats)
    {
        float result = 0f;

        for (int i = 0; i < floats.Length; i++)
        {
            result += floats[i];
        }

        return result;
    }

    //	Choose an integer at random, according to the supplied distribution.
    public int Sample(float[] distro, float total)
    {
        float randVal = total * Random.value;

        for (int i = 0; i < distro.Length; i++)
        {
            if (randVal < distro[i])
            {
                return i;
            }

            randVal -= distro[i];
        }

        return distro.Length - 1;
    }

    //	As above, but calculate the total too.
    public int Sample(float[] distro)
    {
        float total = Sum(distro);
        return Sample(distro, total);
    }

    public string GetStandardEnzymeName(int i)
    {
        string[] enzymeNames = new string[10]
        { "MMP1", "CTSG", "CFB", "ADAM1", "F11", "GZMB",
            "KLK1", "AGA", "ASPRV1", "BACE1" };

        if (i > enzymeNames.Length - 1) { i = i % enzymeNames.Length; }

        return enzymeNames[i];
    }
}
