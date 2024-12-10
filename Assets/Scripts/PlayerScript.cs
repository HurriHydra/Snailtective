using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Allows me to add the Ui such as images or text on the screen
using UnityEngine.SceneManagement; // This allows me to access other scenes in my game, good for separating main menus, gameplay, and more
using TMPro; // Lets me use text mesh pro stuff. 

public class PlayerScript : MonoBehaviour
{
    // (Variables for the player) \\
    Rigidbody rb;
    public float Evidence;
    public GameObject Screen1;
    public AudioClip Capture;
    public AudioClip Efound;
    public AudioClip DoorOpen;
    AudioSource Sounds;

    // (Variables for cutscenes, I use GameObject so I can use SetActive to enable and disable) \\
    public GameObject PlayerCamera;

    public GameObject FinalCamera1;
    public GameObject FinalCamera2;
    public GameObject EndCamera;
    public GameObject HelixSnail;
    public GameObject HelixBag;
    public GameObject Mouse;
    public GameObject MouseCutscene;

    // (Variables for the player again) \\
    public float speed = 3;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 6;
    public KeyCode runningKey = KeyCode.LeftShift;
    public float MaxStamina = 200;
    public float StaminaDrain = 1;
    public float StaminaRecover = 10;
    public float CurrentStamina;

    // (Text for the Ui) \\
    public TMP_Text StaminaText;
    public TMP_Text EvidenceTotal;
    public TMP_Text EvidenceNoti;
    public TMP_Text HelixText;
    public TMP_Text BennyText;

    private string[] EvidenceItems = new string[5]; // This is optional, I'm only showing my understandings of arrays

    private Coroutine FixedCoroutineStaminaRecover = null; // This is a variable so whenever I call my coroutine, it doesnt spam it.

    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();


    void Awake()
    {
        // Get the rigidbody on this.
        rb = GetComponent<Rigidbody>();
        Sounds = GetComponent<AudioSource>();
        EvidenceNoti.enabled = false;
        HelixText.enabled = false;
        BennyText.enabled = false;
        PlayerCamera.SetActive(true);
        CurrentStamina += MaxStamina; // Always updates the stamina / sets the stamina to max before beginning

        EvidenceItems[0] = "Slice of Cheese";
        EvidenceItems[1] = "Snail Head";
        EvidenceItems[2] = "Police Walkie";
        EvidenceItems[3] = "Chef Hat";
        EvidenceItems[4] = "Note: BENNYS MURDER LIST!";
    }

    private void Update() // This update is for updating my evidence, stamina, and death screen Ui without delay,
    {
        EvidenceTotal.text = "Evidence: " + Evidence + "/5";
        StaminaText.text = "Stamina: " + CurrentStamina;

        if (Screen1 != null)
        {
            if (Screen1.GetComponent<Image>().color.a > 0)
            {
                var color = Screen1.GetComponent<Image>().color;

                color.a += 0.002f;

                Screen1.GetComponent<Image>().color = color;
            }
        }

    }

    void FixedUpdate() // This update is for my movement, having rigidbody stuff in FixedUpdate is important so it works on all computers.
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }


        // Get targetVelocity from input.
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rb.velocity = transform.rotation * new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.y);

        if (IsRunning)
        {
            CurrentStamina -= StaminaDrain;
            if (CurrentStamina == 0)
            {
                canRun = false;
            }
        }

        else if (CurrentStamina < MaxStamina)
        {
            if (FixedCoroutineStaminaRecover == null) 
            {
                FixedCoroutineStaminaRecover = StartCoroutine(StaminaRegen()); // Will only play if it's empty
            }
        }


    }


    void OnCollisionEnter(Collision detect)
    {
        if (detect.gameObject.CompareTag("Item"))
        {

            Debug.Log("Item Collected:" + detect.gameObject.name);

            EvidenceNoti.enabled = true;
            EvidenceNoti.text = "Evidence Collected: " + detect.gameObject.name;
            Sounds.PlayOneShot(Efound);
            Evidence += 1;
            Destroy(detect.gameObject);

            switch (detect.gameObject.name) 
            {
                case "Slice of Cheese":
                    Debug.Log(EvidenceItems[0]); // Prints item depending on it's given number, an example being Slice of Cheese = 0
                    HelixText.enabled = true; // Makes Helix text visible to the player
                    HelixText.text = "Helix: What's a slice of cheese doing in here still "; // Him talking lol / character dialogue
                    StartCoroutine(HelixTalk(5.2f)); // My function that waits a little while before making the text invisible
                    break;
                case "Snail Head":
                    Debug.Log(EvidenceItems[1]);
                    HelixText.enabled = true;
                    HelixText.text = "Helix: This is disgusting, I'm going to catch the freak who did this";
                    StartCoroutine(HelixTalk(5.2f));
                    break;
                case "Police Walkie":
                    Debug.Log(EvidenceItems[2]);
                    HelixText.enabled = true;
                    HelixText.text = "Helix: This looks like one of the Snail Department walkies";
                    StartCoroutine(HelixTalk(5.2f));
                    break;
                case "Chef Hat":
                    Debug.Log(EvidenceItems[3]);
                    HelixText.enabled = true;
                    HelixText.text = "Helix: It's all bloody, is this another victim?";
                    StartCoroutine(HelixTalk(5.2f));
                    break;
                case "Note: BENNYS MURDER LIST!":
                    Debug.Log(EvidenceItems[4]);
                    HelixText.enabled = true;
                    HelixText.text = "Helix: I can't believe this. I'm turning you in Benny and this is all the evidence I need";
                    StartCoroutine(HelixTalk(5.2f));
                    break;
                default:
                    Debug.Log("Nothing Happened");
                    break;
            }

            StartCoroutine(TaskWait(2f));

        }

        if (detect.gameObject.CompareTag("Mice"))
        {
            // (This activates whenever you touch the mouse, he will make the screen red and teleport you back to the main menu) \\
            speed = 0;
            runSpeed = 0;
            Sounds.PlayOneShot(Capture);
            var color = Screen1.GetComponent<Image>().color;
            color.a = 0.2f; // Changes the alpha of the image (the red image I set)

            Screen1.GetComponent<Image>().color = color;

            StartCoroutine(MiceCapture(2f));
        }

        if (detect.gameObject.CompareTag("Door1"))
        {
            if (Evidence == 5)
            {
                Debug.Log("You win");
                StartCoroutine(FinalScene(7f)); // How I call the function and the float is the WaitTime number
            }

            else if (Evidence < 5) // Can't leave until you collect 5 evidence to win the game
            {
                Debug.Log("Needs more evidence.");
                HelixText.enabled = true;
                HelixText.text = "I need to collect more evidence, otherwise people won't believe me";
                StartCoroutine(HelixTalk(5.2f));
            }
        }

        if (detect.gameObject.CompareTag("Door2"))
        {
                if (Evidence == 4)
                {
                    transform.localPosition = new Vector3(-2.33f, 1.939f, -41.039f);
                    Sounds.PlayOneShot(DoorOpen);
                    Debug.Log("oh");
                }

                else if (Evidence < 4) // Can't leave until you collect 4 evidence to go in the kitchen
            {
                  Debug.Log("Needs more evidence.");
                  HelixText.enabled = true;
                  HelixText.text = "Door is stuck, I'll take a risk when I have more evidence";
                  StartCoroutine(HelixTalk(5.2f));
            }

        }

        if (detect.gameObject.CompareTag("DoorExit"))
        {
            transform.localPosition = new Vector3(-2.33f, 1.939f, -37.465f);
            Sounds.PlayOneShot(DoorOpen);
        }
    }
        

        private IEnumerator MiceCapture(float WaitTime)
        {
            yield return new WaitForSeconds(WaitTime);

            SceneManager.LoadScene("MainMenu");

        }

        private IEnumerator TaskWait(float WaitTime)
        {
            yield return new WaitForSeconds(WaitTime);
            EvidenceNoti.enabled = false;
        }

        private IEnumerator StaminaRegen() 
        {
            yield return new WaitForSeconds(0.5f);
            canRun = true;
            if (CurrentStamina < MaxStamina)
            {
                CurrentStamina += StaminaRecover;
            }
            FixedCoroutineStaminaRecover = null;
        }
        private IEnumerator HelixTalk(float WaitTime)
         {
            yield return new WaitForSeconds(WaitTime);
            HelixText.enabled = false;
         }

        private IEnumerator FinalScene(float WaitTime)
         {
            speed = 0;
            runSpeed = 0;
            PlayerCamera.SetActive(false);
            FinalCamera1.SetActive(true);
            Destroy(Mouse);
            transform.localPosition = new Vector3(-23.52f, 1.939f, -33);
            HelixSnail.transform.localPosition = new Vector3(-18.31f, 1.763f, -32.248f);
            EvidenceTotal.enabled = false;
            HelixText.enabled = true;
            StaminaText.enabled = false;

        // (THIS ENTIRE SECTION IS PAINFUL DIALOGUE) \\

            HelixText.text = "Helix: Wait why is the door stuck? at a time like this!.";

            yield return new WaitForSeconds(WaitTime); // This is how the code pauses, it stops at WaitTime (can be any number depending on what number you put when you call the function) see line 198

            HelixText.enabled = false;
            BennyText.enabled = true;
            BennyText.text = "Benny: So you found out my secret huh.";

            yield return new WaitForSeconds(WaitTime);

            BennyText.text = "Benny: After all these years pretending to be your sidekick.";

            yield return new WaitForSeconds(WaitTime);

            BennyText.text = "Benny: I tried hiding my secret of being a killer chef and got rid of anyone who found out.";

            yield return new WaitForSeconds(WaitTime);

            BennyText.text = "Benny: I had to be a cop to cover up my dirty work for this abandoned restaurant.";

            yield return new WaitForSeconds(WaitTime);

            BennyText.text = "Benny: Tried to warn you but you didnt listen.";

            yield return new WaitForSeconds(WaitTime);

            BennyText.text = "Benny: My REAL partner in crime was the mouse. Fed him cheese so he can cooperate.";

            yield return new WaitForSeconds(WaitTime);

            HelixText.enabled = true;
            BennyText.enabled = false;
            HelixText.text = "Helix: I can't believe you, you're not getting away with this...";

            yield return new WaitForSeconds(WaitTime);

            FinalCamera1.SetActive(false);
            FinalCamera2.SetActive(true);
            Destroy(HelixSnail);
            MouseCutscene.transform.localPosition = new Vector3(-12.88f, 1.742f, -31.97f);
            HelixText.enabled = false;
            BennyText.enabled = true;
            BennyText.text = "Benny: It's time to say goodbye, get em mice.";

            yield return new WaitForSeconds(WaitTime);
            BennyText.enabled = false;

            MouseCutscene.transform.position = new Vector3(-15.61f, 1.742f, -31.97f);
            Sounds.PlayOneShot(Capture);
            var color = Screen1.GetComponent<Image>().color;
            color.a = 0.2f;

            Screen1.GetComponent<Image>().color = color;

            yield return new WaitForSeconds(3f);

            Screen1.SetActive(false);
            FinalCamera2.SetActive(false);
            EndCamera.SetActive(true);
            HelixBag.SetActive(true);

            yield return new WaitForSeconds(5f);

            EndCamera.transform.rotation = Quaternion.Euler(12.905f, 249.272f, -3.439f);

            yield return new WaitForSeconds(5f);

            EndCamera.transform.position = new Vector3(-9.58f, 4.34f, -44.54f);

            yield return new WaitForSeconds(WaitTime);

            SceneManager.LoadScene("MainMenu");
    }

}