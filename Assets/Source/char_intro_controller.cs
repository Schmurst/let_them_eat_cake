using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class char_intro_controller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Co_waitforseconds(8));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Co_waitforseconds(float sdsd)
    {
        yield return new WaitForSeconds(sdsd);
        SceneManager.LoadScene(2);
    }
}
