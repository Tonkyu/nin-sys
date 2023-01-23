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
        bool response = engine.card1.GetComponent<Card>().info.answer_id == engine.card2.GetComponent<Card>().info.answer_id;
        Animator anim = engine.result.GetComponent<Animator>();
        if (response)
        {
            engine.result.text = "Correct";
            anim.Play("Base Layer.JudgeAnimation", 0, 0.0f);
            audioSource.PlayOneShot(correct_sound);
        }else
        {
            engine.result.text = "Wrong";
            anim.Play("Base Layer.JudgeAnimation", 0, 0.0f);
            audioSource.PlayOneShot(wrong_sound);
        }
    }
}
