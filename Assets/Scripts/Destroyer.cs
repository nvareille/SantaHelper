using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {



    void OnCollisionEnter(Collision col)
    {
        Destroy(col.gameObject);
        
    }

}
