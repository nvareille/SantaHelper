using UnityEngine;
using System.Collections;

public class ToySpawner : MonoBehaviour {

    public GameObject[] toys;
    public int framesToProc = 120;
    public int frames = 0;

    private bool isFinished = false;

	// Use this for initialization
	void Start ()
    {
	
	}
	
    public void Stop()
    {
        isFinished = true;
    }

	// Update is called once per frame
	void Update ()
    {
        if (!isFinished)
        {
            ++frames;
            if (frames >= framesToProc)
            {
                GameObject toy = (GameObject)Instantiate(toys[(int)Random.Range(0f, 3f)]);
                toy.transform.position = new Vector3(Random.Range(-1.6f, 1.6f), 3f, 0f);
                toy.transform.localScale = new Vector3(1, 1, 1) * 0.10f;
                toy.name = "Star";
                frames = 0;
            }
        }
    }
}
