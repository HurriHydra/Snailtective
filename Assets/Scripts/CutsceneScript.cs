using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneScript : MonoBehaviour
{

    public GameObject CameraOutside;
    public GameObject CameraInside;

    public TMP_Text HelixTextC;
    public TMP_Text BennyTextC;
    public TMP_Text Tutorial;

    public GameObject HelixCharacter;

    // Start is called before the first frame update
    void Start()
    {
        HelixTextC.enabled = false;
        BennyTextC.enabled = false;
        Tutorial.enabled = false;
        StartCoroutine(BeginCutscene(7f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator BeginCutscene(float WaitTime)
    {
        BennyTextC.enabled = true;
        BennyTextC.text = "Benny: It's not worth it man.. Many people go missing from this case";

        CameraOutside.SetActive(false);
        CameraInside.SetActive(true);

        yield return new WaitForSeconds(WaitTime);

        BennyTextC.enabled = false;
        HelixTextC.enabled = true;
        HelixTextC.text = "Helix: I will get to the bottom of this, too many snails have gone missing";

        CameraOutside.SetActive(true);
        CameraInside.SetActive(false);

        yield return new WaitForSeconds(WaitTime);

        HelixTextC.text = "Helix: I'm going inside, watch my back from the police cameras..";
        HelixCharacter.transform.position += new Vector3(0f, 0f, 30f);

        yield return new WaitForSeconds(4f);

        HelixTextC.enabled = false;
        Tutorial.enabled = true;

        yield return new WaitForSeconds(WaitTime);

        SceneManager.LoadScene("MainGame");

    }
}
