using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


[System.Serializable]
public class Card : MonoBehaviour
{
    public CardInfo info;
    public AudioClip sound;
    public Texture2D front_image;
    public Texture2D back_image;
    public AudioSource audioSource;
    public bool rotate_flag;
    public bool reverse;
    private int side;
    public Engine engine;
    public KeyCode key;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlaySoundWhenCreated();
        rotate_flag = false;
        reverse = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(key))
        {
            audioSource.PlayOneShot(sound);
        }
        if (Input.GetKeyDown(KeyCode.Return) && Input.GetKey(key))
        {
            rotate_flag = true;
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (this.engine.tmp_card_num == this.side)
            {
                this.Destroy();
            }
        }
    }

    public static GameObject CreateCard(int side, CardInfo info, Vector3 position)
    {
        GameObject obj = (GameObject)Resources.Load("Prefabs/Card");
        obj = Instantiate(obj, position, Quaternion.identity);
        Card new_card = obj.GetComponent<Card>();
        new_card.info = info;
        new_card.sound = (AudioClip)Resources.Load("Sounds/" + info.sound_file);
        new_card.front_image = (Texture2D)Resources.Load("Images/" + info.front_image_file);
        new_card.back_image = (Texture2D)Resources.Load("Images/" + info.back_image_file);
        new_card.transform.Find("Cube").Find("Frame").GetComponent<Renderer>().material = (Material)Resources.Load("Materials/" + new_card.info.material);
        new_card.transform.Find("Cube").Find("Front").GetComponent<Renderer>().material.mainTexture = new_card.front_image;
        new_card.transform.Find("Cube").Find("Back").GetComponent<Renderer>().material.mainTexture =  new_card.back_image;
        // new_card.AttachImage();
        // new_card.AttachBackGround();
        new_card.side = side;
        new_card.key = (new_card.side==1)? KeyCode.LeftArrow : KeyCode.RightArrow;
        return obj;
    }

    public void Destroy()
    {
        if (engine.tmp_card_num > 0) engine.tmp_card_num--;
        engine.result.text = engine.default_result;
        Destroy(this.gameObject);
    }

    private void AttachImage()
    {
        transform.Find("Cube").Find("Front").GetComponent<Renderer>().material.mainTexture = this.front_image;
        transform.Find("Cube").Find("Back").GetComponent<Renderer>().material.mainTexture =  this.back_image;
    }

    private void AttachBackGround()
    {
        switch(this.info.language_type)
        {
            case 1: // japanese
                transform.Find("Cube").Find("Frame").GetComponent<Renderer>().material = (Material)Resources.Load("Materials/Orange.mat");
                break;
            case 4: // german
                transform.Find("Cube").Find("Frame").GetComponent<Renderer>().material = (Material)Resources.Load("Materials/Blue.mat");
                break;
            case 6: //spanish
                transform.Find("Cube").Find("Frame").GetComponent<Renderer>().material = (Material)Resources.Load("Materials/Green.mat");
                break;
        }
    }

    private void PlaySoundWhenCreated()
    {
        GetComponent<AudioSource>().PlayOneShot((AudioClip)Resources.Load("Sounds/card_read"));
    }

    public void Print()
    {
        Debug.Log(info.id);
    }
}
