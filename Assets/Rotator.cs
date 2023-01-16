using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Rotator : MonoBehaviour
{
    private Card card;
    public float rotate_speed_y;

    // Start is called before the first frame update
    void Start()
    {
        rotate_speed_y = 120;
        card = GetComponent<Card>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!card.rotate_flag) return;
        float default_degree = rotate_speed_y * Time.deltaTime;
        float rest_degree;
        if (!card.reverse)
        {
            rest_degree = 180 - gameObject.transform.eulerAngles.y;
        }else
        {
            rest_degree = 360 - gameObject.transform.eulerAngles.y;
        }
        float degree;
        if (default_degree < rest_degree)
        {
            degree = default_degree;
        }else
        {
            degree = rest_degree;
            card.rotate_flag = false;
            card.reverse = !card.reverse;
        }
        gameObject.transform.Rotate(new Vector3(0, 1, 0) * degree);
    }
}
