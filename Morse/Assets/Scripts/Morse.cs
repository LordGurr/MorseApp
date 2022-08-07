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
        //temp = temp.Substring(0, temp.Length - 1);
        for (int i = 0; i < Alfabet.Length; i++)
        {
            temp = temp.ToLower().Replace(Alfabet[i].ToLower(), MorseMotsvarighet[i] + "/");
        }
        temp = temp.Replace("/ ", "|");
        temp = temp.Replace(" ", "|");
        if (temp[^1] == '\r')
        {
            temp = temp.Remove(temp.Length - 1);
        }
        if (temp[^1] == '/')
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

    [SerializeField] private TextMeshProUGUI morseOrd;
    [SerializeField] private TextMeshProUGUI morse;
    [SerializeField] private TMP_InputField blinkMorseOrd;

    private int currentLetter = 0;

    private string nuvarandeOrdMorse;
    private bool[] gissadeKorrektBokstav;

    [SerializeField] private TextAsset[] ordListor;

    [SerializeField] private TMP_Dropdown nuvarandeOrdlistaDropdown;

    private int nuvarandeOrdlista = 0;

    [SerializeField] private GameObject[] KeyboardObjects;

    [SerializeField] private Slider sliderUnit;
    [SerializeField] private TMP_InputField inputFieldUnit;

    [SerializeField] private Slider sliderFrequency;
    [SerializeField] private TMP_InputField inputFieldFrequency;

    [SerializeField] private Slider sliderFlashUnit;
    [SerializeField] private TMP_InputField inputFieldFlashUnit;

    private char[] pausKaraktärer = new char[3]
    {
        '*',
        '/',
        '|',
    };

    // Start is called before the first frame update
    private void Start()
    {
        ord = ordListor[nuvarandeOrdlista].text.Split('\n');

        VäljNyttOrd();

        //audioSource = gameObject.AddComponent<AudioSource>();
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.Stop(); //avoids audiosource from starting to play automatically

        //proceduralAudio.enabled = false;

        sliderUnit.value = unitLength * 1000;
        inputFieldUnit.text = (unitLength * 1000).ToString();

        sliderFlashUnit.value = flashUnitLength * 1000;
        inputFieldFlashUnit.text = (flashUnitLength * 1000).ToString();

        sliderFrequency.value = frequency;
        inputFieldFrequency.text = (frequency).ToString();
    }

    private bool IsDigitsOnly(string str) //Den här kollar så att det bara finns nummer eller mellanslag i passwordet. Om inte för denna så skulle spelet krascha om du skrev en bokstav.
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                if (c != ' ')
                {
                    return false;
                }
        }
        return true;
    }

    public void UpdateSoundFrequencyFromInputfield()
    {
        if ((inputFieldFrequency.text.Length > 0))
        {
            frequency = float.Parse(inputFieldFrequency.text);
            sliderFrequency.value = frequency;
        }
        else
        {
            inputFieldFrequency.text = (frequency).ToString();
        }
    }

    public void UpdateSoundFrequencyFromSlider()
    {
        frequency = sliderFrequency.value;
        inputFieldFrequency.text = (frequency).ToString();
    }

    public void UpdateUnitLengthFromInputfield()
    {
        if ((inputFieldUnit.text.Length > 0))
        {
            unitLength = float.Parse(inputFieldUnit.text) / 1000;
            sliderUnit.value = unitLength * 1000;
        }
        else
        {
            inputFieldUnit.text = (unitLength * 1000).ToString();
        }
    }

    public void UpdateUnitLengthFromSlider()
    {
        unitLength = sliderUnit.value / 1000;
        inputFieldUnit.text = (unitLength * 1000).ToString();
    }

    public void UpdateFlashUnitLengthFromInputfield()
    {
        if ((inputFieldFlashUnit.text.Length > 0))
        {
            flashUnitLength = float.Parse(inputFieldFlashUnit.text) / 1000;
            sliderFlashUnit.value = flashUnitLength * 1000;
        }
        else
        {
            inputFieldFlashUnit.text = (flashUnitLength * 1000).ToString();
        }
    }

    public void UpdateFlashUnitLengthFromSlider()
    {
        flashUnitLength = sliderFlashUnit.value / 1000;
        inputFieldFlashUnit.text = (flashUnitLength * 1000).ToString();
    }

    public void UpdateWordListFromDropdown()
    {
        nuvarandeOrdlista = nuvarandeOrdlistaDropdown.value;

        ord = ordListor[nuvarandeOrdlista].text.Split('\n');

        VäljNyttOrd();
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

        if (currentLetter >= nuvarandeOrd.Length)
        {
            VäljNyttOrd();
        }
    }

    private void VäljNyttOrd()
    {
        currentLetter = 0;
        nuvarandeOrd = ord[UnityEngine.Random.Range(0, ord.Length)];
        morseOrd.text = nuvarandeOrd;
        morse.text = string.Empty;
        nuvarandeOrdMorse = ConvertToMorse(nuvarandeOrd);
        if (nuvarandeOrd[^1] == '\r')
        {
            nuvarandeOrd = nuvarandeOrd.Remove(nuvarandeOrd.Length - 1);
        }
        nuvarandeOrdMorseMedPaus = nuvarandeOrdMorse;
        for (int i = 1; i < nuvarandeOrdMorseMedPaus.Length; i++)
        {
            if (pausKaraktärer.Any(o => o == nuvarandeOrdMorseMedPaus[i]) || (i - 1 >= 0 && pausKaraktärer.Any(o => o == nuvarandeOrdMorseMedPaus[i - 1])) /*|| (i + 1 < nuvarandeOrdMorseMedPaus.Length && pausKaraktärer.Any(o => o == nuvarandeOrdMorseMedPaus[i + 1]))*/)
            {
                continue;
            }
            else
            {
                nuvarandeOrdMorseMedPaus = nuvarandeOrdMorseMedPaus.Insert(i, "*");
            }
        }
        gissadeKorrektBokstav = new bool[nuvarandeOrd.Length];
        SetWordColour();
    }

    private void SetWordColour()
    {
        morseOrd.text = string.Empty;
        for (int i = 0; i < nuvarandeOrd.Length; i++)
        {
            if (currentLetter <= i)
            {
                morseOrd.text += "<color=yellow>" + nuvarandeOrd[i] + "</color>";
            }
            else if (gissadeKorrektBokstav[i])
            {
                morseOrd.text += "<color=green>" + nuvarandeOrd[i] + "</color>";
            }
            else
            {
                morseOrd.text += "<color=red>" + nuvarandeOrd[i] + "</color>";
            }
        }
    }

    private string SetWordColour(string input)
    {
        string temp = string.Empty;
        for (int i = 0; i < input.Length; i++)
        {
            if (i < nuvarandeOrd.Length)
            {
                if (input[i] == nuvarandeOrd[i])
                {
                    temp += "<color=green>" + nuvarandeOrd[i] + "</color>";
                }
                else
                {
                    temp += "<color=red>" + nuvarandeOrd[i] + "</color>";
                }
            }
            else
            {
                break;
            }
        }
        return temp;
    }

    public void FlashWordFinished()
    {
        blinkMorseOrd.text = SetWordColour(blinkMorseOrd.text);
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
        //proceduralAudio.gain = 0;
    }

    public void TurnOffBeep()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        //proceduralAudio.gain = 0;
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

    public void PlayingMorse()
    {
        playingMorse = !playingMorse;
        if (!playingMorse)
        {
            morseBlinkare.SetActive(false);
        }
        FinishedMorseChar();
    }

    public void PauseMorse()
    {
        playingMorse = false;
        morseBlinkare.SetActive(false);
        FinishedMorseChar();
    }

    public void RepeatMorse()
    {
        timePlayedChar = 0;
        currentMorseChar = 0;
        playingMorse = true;
        FinishedMorseChar();
    }

    private bool heldDown = false;
    private float timeHeldDown = 0;

    private float unitLength = 0.04f;

    private float flashUnitLength = 0.5f;

    [SerializeField] private ProceduralAudio proceduralAudio;

    private bool playingMorse;

    private float timePlayedChar = 0;

    private int currentMorseChar;

    [SerializeField] private GameObject morseBlinkare;

    private string nuvarandeOrdMorseMedPaus;

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
                }
                //proceduralAudio.gain = 2;
            }
            else if ((Input.touchCount <= 0 && !Input.GetMouseButton(0)) && heldDown)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                //proceduralAudio.gain = 0;
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
        if (playingMorse)
        {
            timePlayedChar += Time.deltaTime;
            if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '.' && timePlayedChar > flashUnitLength)
            {
                currentMorseChar++;
                FinishedMorseChar();
            }
            else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '-' && timePlayedChar > flashUnitLength * 3)
            {
                currentMorseChar++;
                FinishedMorseChar();
            }
            else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '*' && timePlayedChar > flashUnitLength)
            {
                currentMorseChar++;
                FinishedMorseChar();
            }
            else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '/' && timePlayedChar > flashUnitLength * 3)
            {
                currentMorseChar++;
                FinishedMorseChar();
            }
            else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '|' && timePlayedChar > flashUnitLength * 6)
            {
                currentMorseChar++;
                FinishedMorseChar();
            }
        }
    }

    private void FinishedMorseChar()
    {
        timePlayedChar = 0;
        if (currentMorseChar < nuvarandeOrdMorseMedPaus.Length)
        {
            if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '.' || nuvarandeOrdMorseMedPaus[currentMorseChar] == '-')
            {
                morseBlinkare.SetActive(true);
            }
            else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '|' || nuvarandeOrdMorseMedPaus[currentMorseChar] == '/' || nuvarandeOrdMorseMedPaus[currentMorseChar] == '*')
            {
                morseBlinkare.SetActive(false);
            }
        }
        else
        {
            playingMorse = false;
            morseBlinkare.SetActive(false);
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