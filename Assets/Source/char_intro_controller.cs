using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class char_intro_controller : MonoBehaviour
{
    private float time = 0f;
    void Start()
    {
        StartCoroutine(Co_waitforseconds(8));
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > 5f)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                var source =  FindObjectOfType<AudioSource>();
                if (source != null)
                    Destroy(source.gameObject);

                SceneManager.LoadScene(2);
            }
        }
    }

    IEnumerator Co_waitforseconds(float sdsd)
    {
        yield return new WaitForSeconds(sdsd);

        var source = FindObjectOfType<AudioSource>();
        if (source != null)
            Destroy(source.gameObject);

        SceneManager.LoadScene(2);
    }
}
