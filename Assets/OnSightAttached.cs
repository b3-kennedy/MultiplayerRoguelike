using System.Collections.Generic;
using UnityEngine;

public class OnSightAttached : MonoBehaviour
{
    public List<GameObject> gameObjectsToHideOrShow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Gun>().sightAttached.AddListener(OnAttached);
    }

    void OnAttached()
    { 
        foreach (var obj in gameObjectsToHideOrShow)
        {
            obj.SetActive(false);
        }
    }
}
