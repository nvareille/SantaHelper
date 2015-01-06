using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour {

    public int point;

    private GameManager game;

    void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnCollisionEnter(Collision col)
    {
        Destroy(col.gameObject);
        game.AddScore(point, true);
    }
}
