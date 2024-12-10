using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiMovement : MonoBehaviour
{
    public Transform TargetPlayer;
    private NavMeshAgent Mice;
    private AudioSource RatAudio;
    public AudioClip RatNoise;

    // Start is called before the first frame update
    void Start()
    {
        Mice = GetComponent<NavMeshAgent>();
        RatAudio = GetComponent<AudioSource>();
        InvokeRepeating(nameof(RatSounds), 3f, 10f); // it begins the function in 3 seconds (3f), This loops every 10 seconds (5f)
    }

    // Update is called once per frame
    void Update()
    {
        Mice.destination = TargetPlayer.position;
    }

    private void RatSounds()
    {
        RatAudio.Play();
    }

}
