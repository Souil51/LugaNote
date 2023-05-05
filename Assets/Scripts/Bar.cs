using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    [SerializeField] List<BarLine> Lines = new List<BarLine>();

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNote()
    {
        int index = Random.Range(0, Lines.Count);
        Lines[index].SpawnNote();
    }
}
