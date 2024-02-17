using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Timer : MonoBehaviour
{
    public float tEstudo1, tPausa1, tPausa2;
    private int min = 60, cronom, tempE;
    public int QsecoesEstudo;
    public enum fase {Stop, E1, P1, E2, P2, E3};
    public fase atual;
    public GameObject tempo, seeTime, seeP1, seeP2, estado, getE1, getP1, getP2, setQsecoesEstudo, UIconfig, UIbase, UItag, UIfim, UIsessoes, UIinfo, tags, addTag, label, BtnNvAtv, BtnVerAtv, BtnDark, Cam;
    public Transform localLabel;
    public AudioSource lofi, alarme;

    public List<string> categorias;
    Save s = new Save();
    public static List<Session> secoesEstudo;
    public Session sessaoAtual, ultimaSessao;
    
    public string diaHoje;
    public bool resgatado, isDark = false;
    void Awake()
    {
        s = new Save();
        string path = Application.persistentDataPath;
        if (File.Exists(path + "/savegame.save")){
            categorias = LoadStudySession().allCategories;
            secoesEstudo = LoadStudySession().allSessions;
        } else {
            categorias.Add("Leitura");
            categorias.Add("Estudo");
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }
    void Start()
    {
        resgatado = false;
        atual = fase.Stop;
        tEstudo1 = 0;
        tPausa1 = 0;
        tPausa2 = 0;
        UIbase.SetActive (true);
        UIconfig.SetActive (false);
        UIfim.SetActive (false);
        UIsessoes.SetActive (false);
        QsecoesEstudo = 1;
        BtnDark.SetActive(false);
        //categorias.Add("Leitura");
        //categorias.Add("Estudo");
    }

    void Update()
    {
        if (UIconfig.activeInHierarchy == true){
            seeTime.GetComponent<Text>().text = getE1.GetComponent<Slider>().value.ToString();
            seeP1.GetComponent<Text>().text = getP1.GetComponent<Slider>().value.ToString();
            seeP2.GetComponent<Text>().text = getP2.GetComponent<Slider>().value.ToString();

            switch (QsecoesEstudo){
                case 1:
                getP1.GetComponent<Slider>().interactable = false;
                getP2.GetComponent<Slider>().interactable = false;
                break;
                case 2:
                getP1.GetComponent<Slider>().interactable = true;
                getP2.GetComponent<Slider>().interactable = false;
                break;
                case 3:
                getP1.GetComponent<Slider>().interactable = true;
                getP2.GetComponent<Slider>().interactable = true;
                break;
            }
    
        }
        if (categorias.Count <= 0){
            categorias.Add("Estudo");
            tags.GetComponent<Dropdown>().ClearOptions();
            foreach (string cat in categorias){
                tags.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() {text = cat});
            }

        }


        if(cronom >= 60){
            int m = (int)cronom/60;
            int seg = cronom%60;
            tempo.GetComponent<Text>().text = m.ToString().PadLeft(2, '0') + ':' + seg.ToString().PadLeft(2, '0');
        }else{
            tempo.GetComponent<Text>().text = "00:" + cronom.ToString().PadLeft(2, '0');
        }

        if (tEstudo1 <= 0){
            switch (atual){
                case fase.E1:
                alarme.Play();
                if (isDark){
                    DarkMode();
                }
                SaveStudySession (s);
                UIbase.SetActive (false);
                UIconfig.SetActive (false);
                UIfim.SetActive (true);
                atual = fase.Stop;
                break;
                case fase.E2:
                atual = fase.P2;
                alarme.Play();
                lofi.Pause();
                break;
                case fase.E3:
                atual = fase.P1;
                alarme.Play();
                lofi.Pause();
                break;
            }
        }

        switch (atual)
        {
            case fase.E1:
            case fase.E2:
            case fase.E3:
            //lofi.Play();
            estado.GetComponent<Text>().text = sessaoAtual.categoria;
            tEstudo1 -= 1*Time.deltaTime;
            cronom = (int)tEstudo1;
            if (tEstudo1 < 0){
                tEstudo1 = 0;
            }
            BtnNvAtv.SetActive(false);
            BtnVerAtv.SetActive(false);
            BtnDark.SetActive(true);
            break;
            case fase.P1:
            tEstudo1 = tempE;
            tPausa1 -= 1*Time.deltaTime;
            cronom = (int)tPausa1;
            estado.GetComponent<Text>().text = "Primeira Pausa";
            if (tPausa1 < 0){
                tPausa1 = 0;
                atual = fase.E2;
                lofi.Play();
            }
            BtnNvAtv.SetActive(false);
            BtnVerAtv.SetActive(false);
            BtnDark.SetActive(true);
            break;
            case fase.P2:
            tEstudo1 = tempE;
            tPausa2 -= 1*Time.deltaTime;
            cronom = (int)tPausa2;
            estado.GetComponent<Text>().text = "Segunda Pausa";
            if (tPausa2 < 0){
                tPausa2 = 0;
                atual = fase.E1;
                lofi.Play();
            }
            BtnNvAtv.SetActive(false);
            BtnVerAtv.SetActive(false);
            BtnDark.SetActive(true);
            break;
            case fase.Stop:
            estado.GetComponent<Text>().text = "Vamos Estudar!";
            BtnNvAtv.SetActive(true);
            BtnVerAtv.SetActive(true);
            BtnDark.SetActive(false);
            lofi.Stop();
            break;
        }
    }

    public void Iniciar(){
        lofi.Play();
        //tEstudo1 = int.Parse(getE1.GetComponent<InputField>().text) * min;
        //tempE = int.Parse(getE1.GetComponent<InputField>().text) * min;
        tEstudo1 = getE1.GetComponent<Slider>().value * min;
        tempE = (int)getE1.GetComponent<Slider>().value * min;
        switch (QsecoesEstudo){
            case 1:
            atual = fase.E1;
            tPausa1 = 0.1f;
            tPausa2 = 0.1f;
            break;
            case 2:
            atual = fase.E2;
            tPausa2 = 0.1f;
            tPausa1 = getP1.GetComponent<Slider>().value * min;
            break;
            case 3:
            atual = fase.E3;
            tPausa1 = getP1.GetComponent<Slider>().value * min;
            tPausa2 = getP2.GetComponent<Slider>().value * min;
            break;
        }
        UIconfig.SetActive (false);
        UIbase.SetActive (true);
        sessaoAtual.categoria = tags.GetComponent<Dropdown>().GetComponentInChildren<Text>().text;
        sessaoAtual.tempoEstudo = (int)tempE*QsecoesEstudo/min;
        sessaoAtual.hora = System.DateTime.Now.ToString("HH:mm");
        sessaoAtual.data = System.DateTime.Now.ToString("dd/MM/yyyy");
        sessaoAtual.Id = (int)System.DateTime.Now.ToBinary();
        Debug.Log(sessaoAtual.Id);
        secoesEstudo.Add(sessaoAtual);
        s = new Save();
        s.allSessions = secoesEstudo;
        s.allCategories = categorias;
        SaveStudySession(s);
    }
    public void Configurar(){
        UIbase.SetActive (false);
        UIconfig.SetActive (true);
        UIfim.SetActive (false);
        tags.GetComponent<Dropdown>().options.Clear();
        foreach (string cat in categorias){
            tags.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() {text = cat});
        }
    }
    public void AddTag(){
        //ategorias = new string [Qtags];
        //categorias[Qtags-1] = addTag.GetComponent<Text>().text;
        string entrada = addTag.GetComponent<Text>().text;
        tags.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() {text = entrada});
        categorias.Add(entrada);
        //Qtags ++;
        s = new Save();
        s.allSessions = secoesEstudo;
        s.allCategories = categorias;
        SaveStudySession (s);
        UIbase.SetActive (false);
        UItag.SetActive (false);
        UIconfig.SetActive (true);
    }
    public void RemoveTag(){
        string termo = tags.GetComponentInChildren<Text>().text;
        categorias.Remove(termo);
        tags.GetComponent<Dropdown>().ClearOptions();
        foreach (string cat in categorias){
            tags.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() {text = cat});
        }
    }
    public void CriarTag (){
        UIbase.SetActive (false);
        UIconfig.SetActive (false);
        UItag.SetActive (true);
        addTag.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
    }
    public void VerAtividade(){
        UIbase.SetActive (false);
        UIconfig.SetActive (false);
        UItag.SetActive (false);
        UIsessoes.SetActive (true);
        if (!resgatado){
            foreach(Session sessao in secoesEstudo){
            GameObject labelNew = Instantiate (label) as GameObject;
            labelNew.transform.SetParent (localLabel, false);
            labelNew.GetComponent<Label>().data.text = sessao.data;
            labelNew.GetComponent<Label>().hora.text = sessao.hora;
            labelNew.GetComponent<Label>().categoria.text = sessao.categoria;
            labelNew.GetComponent<Label>().tempo.text = sessao.tempoEstudo.ToString();
            labelNew.GetComponent<Label>().thisID = sessao.Id;
            resgatado = true;
        }
        }
    }
    public void Home(){
        UIbase.SetActive (true);
        UIconfig.SetActive (false);
        UItag.SetActive (false);
        UIsessoes.SetActive (false);
        UIfim.SetActive(false);
        UIinfo.SetActive(false);
    }

    public void EscolherQSessoes (int QtSess)
    {
        QsecoesEstudo = QtSess;
    }

    public void SaveStudySession(Save s){
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath;
        FileStream file = File.Create(path + "/savegame.save");

        bf.Serialize (file, s);
        file.Close();
        Debug.Log ("Dados Salvos");
    }
    public Save LoadStudySession(){
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath;
        FileStream file;

        if(File.Exists(path + "/savegame.save")){
            file = File.Open (path + "/savegame.save", FileMode.Open);
            Save load = (Save)bf.Deserialize(file);
            file.Close();
            return load;
        }else{
            return null;
        }
    }

    public void DarkMode()
    {
        if (Cam.GetComponent<Camera>().backgroundColor == Color.black)
        {
            Cam.GetComponent<Camera>().backgroundColor = new Color32(159, 212, 174, 220);
            estado.GetComponent<Text>().color = Color.gray;
            tempo.GetComponent<Text>().color = Color.black;
            tempo.GetComponentInParent<Image>().color = Color.white;
            isDark = false;
        }
        else
        {
            Cam.GetComponent<Camera>().backgroundColor = Color.black;
            estado.GetComponent<Text>().color = Color.white;
            tempo.GetComponent<Text>().color = Color.white;
            tempo.GetComponentInParent<Image>().color = Color.gray;
            isDark = true;
        }
    }

    public void AtivarTela(Transform ativado)
    {
        ativado.gameObject.SetActive(true);
    }
    public void DesativarTela(Transform desativado)
    {
        desativado.gameObject.SetActive(false);
    }
    public void AtivarButton(Transform botao)
    {
        botao.GetComponent<Button>().interactable = true;
    }
    public void DesativarButton(Transform botao)
    {
        botao.GetComponent<Button>().interactable = false;
    }
}
