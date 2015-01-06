using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public UnityEngine.UI.Text time;
    public UnityEngine.UI.Text scr;
    public GameObject spawner;
    public float timer;
    public ToySpawner toy;

    private bool isSend = false;
    private int score;

	void Start () {
        score = 0;	
	}

    public void AddScore(int nbr, bool adding)
    {
        if (adding)
            score += nbr;
        else
            score -= nbr;
        scr.text = score.ToString("00");
    }

    void Update () {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            time.text = timer.ToString("00.00");
        }
        else if (!isSend)
        {
            isSend = true;
            toy.Stop();
        }
        else
            time.text = "00";
	}
}
