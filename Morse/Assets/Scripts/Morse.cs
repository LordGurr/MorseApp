using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Morse : MonoBehaviour
{
    private string[] Alfabet = new string[]
    {
        "A",
        "B",
        "C",
        "D",
        "E",
        "F",
        "G",
        "H",
        "I",
        "J",
        "K",
        "L",
        "M",
        "N",
        "O",
        "P",
        "Q",
        "R",
        "S",
        "T",
        "U",
        "V",
        "W",
        "X",
        "Y",
        "Z",
        //Svenska karaktärer
        "Å",
        "Ä",
        "Ö",
        //Nummer
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        //Special karaktärer
        /*".",
        "-",
        "(",
        ")",
        "@",
        @"""",
        "%",
        "'",
        ";",
        ":",*/
    };

    private string[] MorseMotsvarighet = new string[]
    {
        ".-",
        "-...",
        "-.-.",
        "-..",
        ".",
        "..-.",
        "--.",
        "....",
        "..",
        ".---",
        "-.-",
        ".-..",
        "--",
        "-.",
        "---",
        ".--.",
        "--.-",
        ".-.",
        "...",
        "-",
        "..-",
        "...-",
        ".--",
        "-..-",
        "-.--",
        "--..",
        //Svenska karaktärer
        ".--.-",
        ".-.-",
        "---.",
        //Nummer
        "-----",
        ".----",
        "..---",
        "...--",
        "....-",
        ".....",
        "-....",
        "--...",
        "---..",
        "----.",
        //Special karaktärer
        /*".-.-.-", // .
        "-....-", // -
        "-.--.",  // (
        "-..-.-", // )
        ".--.-.", // @
        ".-..-.", // "
        ".--..", // %
        ".----.", // '
        "-.-.-.", // ;
        "---...", // :*/
    };

    private string ConvertToMorse(string input)
    {
        string temp = input.Replace('.', '_');
        for (int i = 0; i < Alfabet.Length; i++)
        {
            temp = temp.ToLower().Replace(Alfabet[i].ToLower(), MorseMotsvarighet[i] + "/");
        }
        temp = temp.Replace("/ ", "|");
        temp = temp.Replace(" ", "|");
        if (temp[temp.Length - 1] == '/')
        {
            temp = temp.Remove(temp.Length - 1);
        }
        return temp;
    }

    private string ConvertToAlfabet(string input)
    {
        string temp = string.Empty;
        input = input.Replace("|", "/|/");
        input = input.Replace("_", "/_/");

        string[] alfabet = input.Split('/');
        for (int i = 0; i < alfabet.Length; i++)
        {
            int index = MorseMotsvarighet.ToList().FindIndex(o => o == alfabet[i].Trim());
            if (index > -1)
            {
                alfabet[i] = Alfabet[index].ToLower();
            }
        }
        for (int i = 0; i < alfabet.Length; i++)
        {
            //temp = temp.Replace(MorseMotsvarighet[i], Alfabet[i].ToLower());
            temp += alfabet[i];
        }
        temp = temp.Replace('_', '.');
        temp = temp.Replace('|', ' ');
        return temp;
    }

    private string[] ord;
    private string nuvarandeOrd;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private TextMeshProUGUI morse;

    private int currentLetter = 0;

    private string nuvarandeOrdMorse;
    private bool[] gissadeKorrektBokstav;

    [SerializeField] private TextAsset textAsset;

    [SerializeField] private GameObject[] KeyboardObjects;

    [SerializeField] private Slider sliderUnit;
    [SerializeField] private TMP_InputField inputFieldUnit;

    [SerializeField] private Slider sliderFrequency;
    [SerializeField] private TMP_InputField inputFieldFrequecny;

    // Start is called before the first frame update
    private void Start()
    {
        ord = textAsset.text.Split('\n');

        nuvarandeOrd = ord[UnityEngine.Random.Range(0, ord.Length)];
        textMesh.text = nuvarandeOrd;
        morse.text = string.Empty;
        nuvarandeOrdMorse = ConvertToMorse(nuvarandeOrd);
        gissadeKorrektBokstav = new bool[nuvarandeOrd.Length];
        SetWordColour();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically

        sliderUnit.value = unitLength * 1000;
        inputFieldUnit.text = (unitLength * 1000).ToString();

        sliderFrequency.value = frequency;
        inputFieldFrequecny.text = (frequency).ToString();
    }

    public void UpdateSoundFrequencyFromInputfield()
    {
        frequency = float.Parse(inputFieldFrequecny.text);
        sliderFrequency.value = frequency;
    }

    public void UpdateSoundFrequencyFromSlider()
    {
        frequency = sliderFrequency.value;
        inputFieldFrequecny.text = (frequency).ToString();
    }

    public void UpdateUnitLengthFromInputfield()
    {
        unitLength = float.Parse(inputFieldUnit.text) / 1000;
        sliderUnit.value = unitLength * 1000;
    }

    public void UpdateUnitLengthFromSlider()
    {
        unitLength = sliderUnit.value / 1000;
        inputFieldUnit.text = (unitLength * 1000).ToString();
    }

    public void DotPressed()
    {
        morse.text += ".";
        UpdateWord();
    }

    private void UpdateWord()
    {
        if (ConvertToMorse(nuvarandeOrd[currentLetter].ToString())[morse.text.Length - 1] != morse.text[morse.text.Length - 1])
        {
            //nuvarandeOrd = ord[Random.Range(0, ord.Length)];
            //textMesh.text = nuvarandeOrd;
            morse.text = string.Empty;
            gissadeKorrektBokstav[currentLetter] = false;
            currentLetter++;
            SetWordColour();
        }
        else if (ConvertToMorse(nuvarandeOrd[currentLetter].ToString()) == morse.text)
        {
            morse.text = string.Empty;
            gissadeKorrektBokstav[currentLetter] = true;
            currentLetter++;
            SetWordColour();
        }

        if (currentLetter >= nuvarandeOrd.Length - 1)
        {
            nuvarandeOrd = ord[UnityEngine.Random.Range(0, ord.Length)];
            textMesh.text = nuvarandeOrd;
            morse.text = string.Empty;
            nuvarandeOrdMorse = ConvertToMorse(nuvarandeOrd);
            gissadeKorrektBokstav = new bool[nuvarandeOrd.Length];
            currentLetter = 0;
            SetWordColour();
        }
    }

    private void SetWordColour()
    {
        textMesh.text = string.Empty;
        for (int i = 0; i < nuvarandeOrd.Length; i++)
        {
            if (currentLetter <= i)
            {
                textMesh.text += "<color=yellow>" + nuvarandeOrd[i] + "</color>";
            }
            else if (gissadeKorrektBokstav[i])
            {
                textMesh.text += "<color=green>" + nuvarandeOrd[i] + "</color>";
            }
            else
            {
                textMesh.text += "<color=red>" + nuvarandeOrd[i] + "</color>";
            }
        }
    }

    private bool keyboard = true;

    public void KeyboardSwitch()//Switch from keyboard to press.
    {
        keyboard = !keyboard;
        timeHeldDown = 0;
        for (int i = 0; i < KeyboardObjects.Length; i++)
        {
            KeyboardObjects[i].SetActive(keyboard);
        }
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void TurnOffBeep()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        keyboard = true;
        timeHeldDown = 0;
        for (int i = 0; i < KeyboardObjects.Length; i++)
        {
            KeyboardObjects[i].SetActive(keyboard);
        }
    }

    public void DashPressed()
    {
        morse.text += "-";
        UpdateWord();
    }

    private bool heldDown = false;
    private float timeHeldDown = 0;

    private float unitLength = 0.04f;

    // Update is called once per frame
    private void Update()
    {
        if (!keyboard)
        {
            //#if UNITY_ANDROID
            //            if (Input.touchCount > 0)
            //#elif UNITY_EDITOR
            if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !heldDown)
            {
                if (!audioSource.isPlaying)
                {
                    //timeIndex = 0;  //resets timer before playing sound
                    audioSource.Play();
                    //proceduralAudio.enabled = true;
                }
            }
            else if ((Input.touchCount <= 0 && !Input.GetMouseButton(0)) && heldDown)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            //#endif
            {
                heldDown = true;
                timeHeldDown += Time.deltaTime;

                //if (!audioSource.isPlaying)
                //{
                //    //timeIndex = 0;  //resets timer before playing sound
                //    audioSource.Play();
                //}
                //else
                //{
                //    audioSource.Stop();
                //}
            }
            else if (timeHeldDown > 0)
            {
                if (timeHeldDown > unitLength * 3)
                {
                    DashPressed();
                }
                else
                {
                    DotPressed();
                }
                timeHeldDown = 0;
                heldDown = false;
            }
        }
    }

    [Range(1, 20000)]  //Creates a slider in the inspector
    public float frequency = 100;

    public float sampleRate = 44100;
    private AudioSource audioSource;

    private float phase = 0;

    //void Start()
    //{
    //}

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //    }
    //}

    private void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += 2 * Mathf.PI * frequency / sampleRate;

            data[i] = Mathf.Sin(phase);

            if (phase >= 2 * Mathf.PI)
            {
                phase -= 2 * Mathf.PI;
            }
        }
    }

    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
    }
}