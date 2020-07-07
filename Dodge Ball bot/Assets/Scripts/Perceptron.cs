using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TrainingSet
{
	public double[] input;
	public double output;
}

public class Perceptron : MonoBehaviour {

	List <TrainingSet> ts =  new List<TrainingSet>();
	double[] weights = {0,0};
	double bias = 0;
	double totalError = 0;

    public GameObject npc;

    public void SendInput(double i1, double i2, double o)
    {
        double result = CalcOutput(i1, i2);
        Debug.Log(result);

        if(result == 0) //duck for cover
        {
            npc.GetComponent<Animator>().SetTrigger("Crouch");
            npc.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            npc.GetComponent<Rigidbody>().isKinematic = true;
        }

        //learn from it for the next time
        TrainingSet s = new TrainingSet();
        s.input = new double[2] { i1, i2 };
        s.output = o;
        ts.Add(s);
        Train();

    }

    double DotProductBias(double[] v1, double[] v2) 
	{
		if (v1 == null || v2 == null)
			return -1;
	 
		if (v1.Length != v2.Length)
			return -1;
	 
		double d = 0;
		for (int x = 0; x < v1.Length; x++)
		{
			d += v1[x] * v2[x];
		}

		d += bias;
	 
		return d;
	}

	double CalcOutput(int i)
	{
		return(ActivationFunction(DotProductBias(weights,ts[i].input)));
	}

	double CalcOutput(double i1, double i2)
	{
		double[] inp = new double[] {i1, i2};
		return(ActivationFunction(DotProductBias(weights,inp)));
	}

	double ActivationFunction(double dp)
	{
		if(dp > 0) return (1);
		return(0);
	}

	void InitialiseWeights()
	{
		for(int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f,1.0f);
		}
		bias = Random.Range(-1.0f,1.0f);
	}

	void UpdateWeights(int j)
	{
		double error = ts[j].output - CalcOutput(j);
		totalError += Mathf.Abs((float)error);
		for(int i = 0; i < weights.Length; i++)
		{			
			weights[i] = weights[i] + error*ts[j].input[i]; 
		}
		bias += error;
	}

	void Train()
	{
		for(int t = 0; t < ts.Count; t++)
		{
			UpdateWeights(t);
        }
	}

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.red;
        GUI.BeginGroup(new Rect(10, 10, 370, 200));
        GUI.Box(new Rect(0, 0, 140, 140), " Press 1 for red sphere", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Press 2 for green sphere", guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), "Press 3 for red cube", guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Press 4 for green cube", guiStyle);
        GUI.Label(new Rect(10, 100, 200, 30), "Press space for resetting", guiStyle);
        GUI.Label(new Rect(10, 125, 200, 30), "Press s for save weights", guiStyle);
        GUI.Label(new Rect(10, 150, 200, 30), "Press l for loading saved weights", guiStyle);
        GUI.EndGroup();
    }

    void LoadWeights()
    {
        string path = Application.dataPath + "/weights.txt";
        if(File.Exists(path))
        {
            var sr = File.OpenText(path);
            string line = sr.ReadLine();
            string[] w = line.Split(',');
            weights[0] = System.Convert.ToDouble(w[0]);
            weights[1] = System.Convert.ToDouble(w[1]);
            bias = System.Convert.ToDouble(w[2]);
            Debug.Log("loading");
        }
    }


    void SaveWeights()
    {
        string path = Application.dataPath + "/weights.txt";
        var sr = File.CreateText(path);
        sr.WriteLine(weights[0] + "," + weights[1] + "," + bias);
        sr.Close();
    }

    void Start ()
    {
        InitialiseWeights();
	}
	
	void Update () {
		if(Input.GetKeyDown("space"))
        {
            InitialiseWeights();
            ts.Clear();
        }

        else if (Input.GetKeyDown("s"))
        {
            SaveWeights();
        }

        else if (Input.GetKeyDown("l"))
        {
            LoadWeights();
        }
    }
}