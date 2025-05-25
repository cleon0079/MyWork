using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPuzzleIdSetUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (i < 9)
            {
                this.transform.GetChild(i).GetComponent<PaperPuzzleItem>().SetPuzzleId(i);
            }
            else if (i < 18)
            {
                this.transform.GetChild(i).GetComponent<PaperPuzzleItem>().SetPuzzleId(i - 9);
            }
            else
            {
                this.transform.GetChild(i).GetComponent<PaperPuzzleItem>().SetPuzzleId(i - 18);
            }
        }
    }
}
