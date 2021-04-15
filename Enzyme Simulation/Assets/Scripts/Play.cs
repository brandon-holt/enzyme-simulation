using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SharpLearning.Metrics.Classification;
using SharpLearning.RandomForest.Learners;
using SharpLearning.Containers.Matrices;
using SharpLearning.CrossValidation.TrainingTestSplitters;
using MEC;
using Voxus.Random;

public class Play : MonoBehaviour
{
    // testing parameters
    [SerializeField] private int numGroups, repeats, numPerGroup;
    [SerializeField] private float firstSeconds, tStep;
    private F64Matrix subVals;
    private List<float> AUROCvalues = new List<float>();
    private int pi;
    private float startTime;
    private bool testFinished;

    // UI objects
    [SerializeField] private Button[] buttons;
    [SerializeField] private Button test;
    [SerializeField] private TextMeshProUGUI progress, time, message;
    [SerializeField] private ColorText comp, auroc, overall;
    [SerializeField] private RectTransform prog, bar;
    [SerializeField] private GameObject exampleData, classification;

    private World world;

    public void Load(string level)
    {
        Global.sim.finish = true;
        Global.load.LoadLevel(level);
    }

    public void DeactivateButton(Button b)
    {
        b.interactable = false;
    }

    public void SetDescription(string description)
    {
        if (testFinished)
        {
            GetScores(out _, out _, out float overallScore);
            Global.sim.description = "Score: " + (100 * overallScore).ToString("F0") + "\n" + description;
        }
        else
        {
            Global.sim.description = description;
        }
    }

    private void Awake()
    {
        test.onClick.AddListener(TestMethod);

        exampleData.SetActive(false);
        classification.SetActive(false);

        world = GameObject.Find("World").GetComponent<World>();

        Global.sim.description = ""; // clear from previous run
    }

    private void TestMethod()
    {
        Timing.RunCoroutine(_Test().CancelWith(gameObject));

        Timing.RunCoroutine(_ProgressUpdater().CancelWith(gameObject));
    }

    private IEnumerator<float> _Test()
    {
        testFinished = false;

        test.interactable = false;

        Global.sim.lessData = false;
        Global.sim.timeStep = tStep;
        Global.sim.tMax = firstSeconds;

        startTime = Time.unscaledTime;

        UpdateScores();

        Global.LastClassificationResults.meanSubValues.Clear();

        pi = 0;
        for (int r = 0; r < repeats; r++)
        {
            int[,] enzVals = GenerateEnzymeArray(numGroups, Global.sim.ne.Length);
            subVals = new F64Matrix(numGroups * numPerGroup, Global.sim.ny1.Length);
            double[] targets = new double[numGroups * numPerGroup]; // group index, g

            int i = 0;
            for (int g = 0; g < numGroups; g++)
            {
                Global.sim.ne = GenerateEnzymeVector(enzVals, g);

                for (int n = 0; n < numPerGroup; n++)
                {
                    Global.sim.Activity();

                    while (!Global.sim.finish) { yield return Timing.WaitForOneFrame; }

                    UpdateSubVals(i);

                    targets[i] = g;
                    
                    i += 1; pi += 1;
                }
            }

            // get AUROC and store
            AUROCvalues.Add(GetAUROC(subVals, targets));

            UpdateClassificationResults();

            UpdateScores();
        }

        testFinished = true;

        Global.LastClassificationResults.aurocValues = AUROCvalues.ToArray();
        Global.LastClassificationResults.numGroups = numGroups;
        Global.LastClassificationResults.repeats = repeats;

        UploadToServer();

        UpdateProgressBar(startTime, 1, 1);

        exampleData.SetActive(true);
        classification.SetActive(true);
    }

    private IEnumerator<float> _ProgressUpdater()
    {
        while (!testFinished)
        {
            UpdateProgressBar(startTime, pi, repeats * numGroups * numPerGroup);
            yield return Timing.WaitForOneFrame;
        }
    }

    private int[,] GenerateEnzymeArray(int numGroups, int numEnz)
    {
        int[,] protVals = new int[numGroups, numEnz];

        // healthy baseline, normally distributed
        int mean = 15; int sigma = 3;
        RandomGaussian randGaussian = new RandomGaussian(sigma, mean);
        // disease difference
        float fractionDiff = .1f; // fraction of total enzyme types that are going to be different
        int numDiff = Mathf.CeilToInt(Global.sim.limit_enzs * fractionDiff); // number of enzyme types that are different
        int delta = 1;

        for (int g = 0; g < numGroups; g++)
        {
            int[] ei = Global.RandPerm(numEnz);
            for (int e = 0; e < numEnz; e++)
            {
                int value;
                if (g == 0) { value = Mathf.Max(0, Mathf.RoundToInt(randGaussian.Get())); } // healthy baseline
                else if (e < numDiff) { value = protVals[0, ei[e]] + RandSign(delta); } // perturbation
                else { value = protVals[0, ei[e]]; } // no perturbation

                protVals[g, ei[e]] = Mathf.Max(0, value);
            }
        }

        return protVals;
    }

    private int[] GenerateEnzymeVector(int[,] enzVals, int g)
    {
        int[] ne = new int[Global.sim.ne.Length];

        for (int i = 0; i < ne.Length; i++)
        {
            ne[i] = enzVals[g, i];
        }

        return ne;
    }

    private void UpdateSubVals(int i)
    {
        for (int si = 0; si < Global.sim.ny1.Length; si++)
        {
            subVals[i, si] = Global.sim.py1[si].Last() + Global.sim.py2[si].Last();
        }
    }

    private void UpdateProgressBar(float startTime, int i, int tot)
    {
        float fracComplete = 1f;

        if (i != tot) { fracComplete = (i + Global.sim.max_progress) / tot; }

        bar.sizeDelta = new Vector2(fracComplete * prog.sizeDelta.x, bar.sizeDelta.y);

        progress.text = (100f * fracComplete).ToString("F1") + "%";

        time.text = FormatTime(Time.unscaledTime - startTime);
    }

    private void GetScores(out float meanAUROC, out float compressionRatio, out float overallScore)
    {
        meanAUROC = Global.sim.Sum(AUROCvalues.ToArray()) / AUROCvalues.Count;
        compressionRatio = 1f * Global.sim.ny1.Length / Global.sim.ne.Length;
        overallScore = OverallScore(meanAUROC, compressionRatio);
    }

    private void UpdateScores()
    {
        GetScores(out float meanAUROC, out float compressionRatio, out float overallScore);
        comp.SetValue(100 * compressionRatio, "F0", "%");
        auroc.SetValue(100 * meanAUROC, "F0", "%");
        overall.SetValue(100 * overallScore, "F0", "");
    }

    private float GetAUROC(F64Matrix observations, double[] targets)
    {
        // print the raw data being used for classification
        PrintArray(observations);
        PrintVector(targets);

        // split the data into training and test set
        var splitter = new RandomTrainingTestIndexSplitter<double>(trainingPercentage: 0.5);
        var trainingTestSplit = splitter.SplitSet(observations, targets);
        var trainSet = trainingTestSplit.TrainingSet;
        var testSet = trainingTestSplit.TestSet;

        // train the model
        var learner = new ClassificationRandomForestLearner();
        var model = learner.Learn(trainSet.Observations, trainSet.Targets);

        // make the predictions from the test set
        var testPredictions = model.PredictProbability(testSet.Observations);

        // create the metric and measure the error
        var metric = new RocAucClassificationProbabilityMetric(1);
        var testError = (float)metric.Error(testSet.Targets, testPredictions);
        if (testError < .5f) { testError = 1f - testError; }

        return testError;
    }

    private void PrintArray(F64Matrix observations)
    {
        string printed = "";
        for (int i = 0; i < observations.RowCount; i++)
        {
            for (int j = 0; j < observations.ColumnCount; j++)
            {
                printed += " " + observations[i, j].ToString() + " ";
            }

            printed += "\n";
        }
        Debug.Log(printed);
    }

    private void PrintVector(double[] targets)
    {
        string printed = "";
        for (int i = 0; i < targets.Length; i++)
        {
            printed += targets[i].ToString() + "\n";
        }
        Debug.Log(printed);
    }

    private float OverallScore(float auroc, float compression)
    {
        return .5f * ((1f - compression) + (2 * auroc) - 1f);
    }

    private string FormatTime(float time)
    {
        int t = Mathf.RoundToInt(time);
        int m = t / 60;
        int s = t % 60;
        if (m == 0) { return s + " s"; }
        else { return m + " m " + s + " s"; }
    }

    private int RandSign(int magnitude)
    {
        int[] signs = new int[2] { -1, 1 };
        return magnitude * signs[Random.Range(0, signs.Length)];
    }

    private void UploadToServer()
    {
        // deactivate buttons while uploading
        Timing.RunCoroutine(_ButtonsOffWhileUploading().CancelWith(gameObject));

        // saves files to local machine
        string type = "server" + PlayerPrefs.GetString("name");
        world.SaveWorld(type);

        // uploads the file we just saved
        StartCoroutine(Global.UploadSaveFile(Global.lastSavedFile));

        // uploads information to the leaderboard
        GetScores(out float meanAUROC, out float compressionRatio, out float overallScore);
        float[] scores = new float[3] { 100 * compressionRatio, 100 * meanAUROC, 100 * overallScore };
        StartCoroutine(Global.UploadToLeaderboard("scores", scores, Global.lastSavedFile));
    }

    private IEnumerator<float> _ButtonsOffWhileUploading()
    {
        yield return Timing.WaitForOneFrame;

        message.text = "Uploading scores to leaderboard, do not close...";

        foreach(Button b in buttons) { b.interactable = false; }

        while (Global.uploadingFile || Global.uploadingScore) { yield return Timing.WaitForOneFrame; }

        foreach (Button b in buttons) { b.interactable = true; }

        message.text = "Upload complete!";

        yield return Timing.WaitForSeconds(3f);

        message.text = "";
    }

    private void UpdateClassificationResults()
    {
        float[,] meanSubVals = new float[numGroups, subVals.ColumnCount];

        for (int group = 0; group < numGroups; group++)
        {
            for (int sub = 0; sub < subVals.ColumnCount; sub++)
            {
                int[] correctRows = Global.IntArray(group * numPerGroup, (group+1) * numPerGroup);

                IMatrix<double> membersOfGroup = subVals.Rows(correctRows);

                double[] substrateFromGroup = membersOfGroup.Column(sub);

                float meanVal = (float)Global.Sum(substrateFromGroup) / substrateFromGroup.Length;

                meanSubVals[group, sub] = meanVal;
            }
        }

        Global.LastClassificationResults.meanSubValues.Add(meanSubVals);
    }


}
