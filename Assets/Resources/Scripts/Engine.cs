using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Engine : MonoBehaviour
{
    public int game_state;
    private string card_master_json_path;
    private string rfid_master_json_path;
    public Dictionary<string, CardInfo> cardInfos;
    public string[] id_list;
    public RFID[] rfid_data;
    public Dictionary<string, string> dict; // <RFID, CardInfo.id>
    public GameObject card1;
    public GameObject card2;
    public TextMeshProUGUI result;
    public int tmp_card_num;
    public string default_result = "____";

    // Start is called before the first frame update
    void Start()
    {
        game_state = 0;
        card_master_json_path = "Assets/Resources/Master/CardMaster.json";
        rfid_master_json_path = "Assets/Resources/Master/RFIDMaster.json";
        CardDataLoad(card_master_json_path, ref id_list);
        RfidDataLoad(rfid_master_json_path, ref rfid_data);
        CreatePair(ref rfid_data, ref id_list);
        result.text = default_result;
        tmp_card_num = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateCard(string rfid_uid)
    {
        if (!dict.ContainsKey(rfid_uid))
        {
            Debug.Log("Key Not Found : " + rfid_uid);
            return;
        }

        Vector3 position;
        int side;
        if (tmp_card_num == 0)
        {
            position = new Vector3(-5.0f, 0.0f, 0.0f);
            side = 1;
            card1 = Card.CreateCard(side, cardInfos[dict[rfid_uid]], position);
            card1.GetComponent<Card>().engine = this.GetComponent<Engine>();
            tmp_card_num++;
        }else if (tmp_card_num == 1){
            position = new Vector3(5.0f, 0.0f, 0.0f);
            side = 2;
            card2 = Card.CreateCard(side, cardInfos[dict[rfid_uid]], position);
            card2.GetComponent<Card>().engine = this.GetComponent<Engine>();
            tmp_card_num++;
        }else if(tmp_card_num == 2)
        {
            Debug.Log("Can't place cards any more.");
        }
    }

    void CardDataLoad(string datapath, ref string[] id_list)
    {
        StreamReader reader = new StreamReader(datapath);
        string datastr = reader.ReadToEnd();
        reader.Close();

        datastr = datastr.Trim('{', '}');
        MatchCollection matches = Regex.Matches(datastr, @"\{[\s\S]*?\}");
        cardInfos = new Dictionary<string, CardInfo>();
        id_list = new string[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            CardInfo _card_info = gameObject.AddComponent<CardInfo>();
            JsonUtility.FromJsonOverwrite(matches[i].Value, _card_info);
            string id = _card_info.id;
            id_list[i] = id;
            try{
                cardInfos.Add(id, _card_info);
            }
            catch(ArgumentNullException e){
                Debug.Log(_card_info.id);
                Debug.Log(_card_info.answer_label);
                Debug.Log(e);
            }
        }
    }

    void RfidDataLoad(string datapath, ref RFID[] rfid_data)
    {
        StreamReader reader = new StreamReader(datapath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        datastr = datastr.Trim('{', '}');
        MatchCollection matches = Regex.Matches(datastr, @"\{[\s\S]*?\}");
        rfid_data = new RFID[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            rfid_data[i] = gameObject.AddComponent<RFID>();
            JsonUtility.FromJsonOverwrite(matches[i].Value, rfid_data[i]);
        }
    }

    void CreatePair(ref RFID[] rfid_data, ref string[] id_list)
    {
        Shuffle(ref id_list);
        dict = new Dictionary<string, string>();
        for (int i = 0; i < Mathf.Min(id_list.Length, rfid_data.Length); i++)
        {
            dict.Add(rfid_data[i].id, id_list[i]);
        }
    }

    void Shuffle(ref string[] s)
    {
        for (int i = s.Length - 1; i >= 0; i--)
        {
            int j = (int)UnityEngine.Random.Range(0, i + 1);
            string tmp = s[i];
            s[i] = s[j];
            s[j] = tmp;
        }
    }

}
