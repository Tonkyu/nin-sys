using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip correct_sound;
    public AudioClip wrong_sound;
    public AudioSource audioSource;
    public Engine engine;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Judgement()
    {
        bool response = engine.card1.GetComponent<Card>().info.answer_label == engine.card2.GetComponent<Card>().info.answer_label;
        Debug.Log(response);
        if (response)
        {
            engine.result.text = "Correct";
            audioSource.PlayOneShot(correct_sound);
        }else
        {
            engine.result.text = "Wrong";
            audioSource.PlayOneShot(wrong_sound);
        }
    }
}
