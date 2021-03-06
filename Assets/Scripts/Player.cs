using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public void Deactivate()
    {
        GetComponent<Canon>().Deactivate();
        GetComponent<Animator>().SetTrigger("Death");
    }
}
