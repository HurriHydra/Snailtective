using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButtonSD : MonoBehaviour
{
    public Button PlayButton;
    public GameObject ContinueButton;
    
    // Start is called before the first frame update
    void Start()
    {
        // Button StartButton = PlayButton.GetComponent<Button>();
        // StartButton.onClick.AddListener(ButtonPressed);
        Cursor.lockState = CursorLockMode.None;
        if (PlayerPrefs.GetInt("ContinueUnlocked") == 1) ContinueButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonPressed()
    {
        Debug.Log("Button was pressed");
        PlayerPrefs.SetInt("ContinueUnlocked", 1);
        SceneManager.LoadScene("Cutscene"); // Make this a different scene when you get home. (For cutscenes)
    }

    public void ContinuePressed()
    {
        Debug.Log("Button was pressed");
        SceneManager.LoadScene("MainGame");
    }
}
