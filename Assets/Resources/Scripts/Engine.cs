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
    private string answer_master_json_path;
    public Dictionary<string, CardInfo> cardInfos;
    public Tuple<string, int>[] id_lang_list;
    public RFID[] rfid_data;
    public AnswerData[] answer_data;
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
        answer_master_json_path = "Assets/Resources/Master/AnswerMaster.json";
        CardDataLoad(card_master_json_path, ref id_lang_list);
        Debug.Log("Card loaded.");
        RfidDataLoad(rfid_master_json_path, ref rfid_data);
        Debug.Log("RFID loaded.");
        AnswerDataLoad(answer_master_json_path, ref answer_data);
        Debug.Log("Answer loaded.");
        CreatePair(ref rfid_data, ref id_lang_list);
        Debug.Log("Created Pairs.");
        result.text = default_result;
        tmp_card_num = 0;

        foreach (KeyValuePair<string, string> e in dict)
        {
            Debug.Log(e.Key + " " + e.Value);
        }
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

    void CardDataLoad(string datapath, ref Tuple<string,int>[] id_lang_list)
    {
        StreamReader reader = new StreamReader(datapath);
        string datastr = reader.ReadToEnd();
        reader.Close();

        datastr = datastr.Trim('{', '}');
        MatchCollection matches = Regex.Matches(datastr, @"\{[\s\S]*?\}");
        cardInfos = new Dictionary<string, CardInfo>();
        id_lang_list = new Tuple<string,int>[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            CardInfo _card_info = gameObject.AddComponent<CardInfo>();
            JsonUtility.FromJsonOverwrite(matches[i].Value, _card_info);
            id_lang_list[i] = Tuple.Create(_card_info.id, _card_info.language_type);
            try{
                cardInfos.Add(_card_info.id, _card_info);
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

    void AnswerDataLoad(string datapath, ref AnswerData[] rfid_data)
    {
        StreamReader reader = new StreamReader(datapath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        datastr = datastr.Trim('{', '}');
        MatchCollection matches = Regex.Matches(datastr, @"\{[\s\S]*?\}");
        rfid_data = new AnswerData[matches.Count];
        for (int i = 0; i < matches.Count; i++)
        {
            rfid_data[i] = gameObject.AddComponent<AnswerData>();
            JsonUtility.FromJsonOverwrite(matches[i].Value, rfid_data[i]);
        }
    }

    void CreatePair(ref RFID[] rfid_data, ref Tuple<string, int>[] id_lang_list)
    {
        Shuffle(ref id_lang_list);
        dict = new Dictionary<string, string>();
        bool[] done = new bool[id_lang_list.Length];
        // AnswerShuffle(ref answer_data);
        int NUM = 10;
        for (int k = 0; k < NUM; k++)
        {
            Debug.Log(answer_data[k].answer_id);
        }
        for (int i = 0; i < Mathf.Min(id_lang_list.Length, rfid_data.Length); i++)
        {
            int j = 0;
            while (j < id_lang_list.Length  && (done[j] || !(id_lang_list[j].Item2 == 1 ^ rfid_data[i].label == "foreign") || id_lang_list[j].Item2 == 6))
            {
                j++;
            }
            bool flag = false;
            Debug.Log(id_lang_list[j].Item1);
            for (int k = 0; k < NUM; k++)
            {
                if(id_lang_list[j].Item1.Substring(id_lang_list[j].Item1.Length-2) == answer_data[k].answer_id || id_lang_list[j].Item1.Substring(id_lang_list[j].Item1.Length-1) == answer_data[k].answer_id)
                {
                    flag = true;
                    Debug.Log("TRUE");
                }
            }

            if(flag)
            {
                dict.Add(rfid_data[i].id, id_lang_list[j].Item1);
            }
            done[j] = true;
        }
    }

    void Shuffle(ref Tuple<string, int>[] s)
    {
        for (int i = s.Length - 1; i >= 0; i--)
        {
            int j = (int)UnityEngine.Random.Range(0, i + 1);
            Tuple<string, int> tmp = s[i];
            s[i] = s[j];
            s[j] = tmp;
        }
    }

    void AnswerShuffle(ref AnswerData[] s)
    {
        for (int i = s.Length - 1; i >= 0; i--)
        {
            int j = (int)UnityEngine.Random.Range(0, i + 1);
            AnswerData tmp = s[i];
            s[i] = s[j];
            s[j] = tmp;
        }
    }
}
