using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSquare : MonoBehaviour
{
    PathAgent Target;
    public GameObject Reticle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            transform.position = Target.transform.position + new Vector3(0, 1.5f, 0);
        }
    }

    public void Select(PathAgent agent)
    {
        Target = agent;
        Reticle.SetActive(true);
    }

    public void Deselect()
    {
        Target = null;
        Reticle.SetActive(false);
    }
}
