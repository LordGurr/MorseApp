using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;

internal enum Skärm
{
    Menu,
    Inställningar,
    OrdTillMorse,
    BlinkTillOrd,
    BlinkTillBokstav,
}

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
    [SerializeField] private TextMeshProUGUI blinkMorseOrdResult;

    [SerializeField] private TMP_InputField blinkMorseBokstav;
    [SerializeField] private TextMeshProUGUI blinkMorseBokstavResult;

    private int currentLetter = 0;

    private string nuvarandeOrdMorse;
    private bool[] gissadeKorrektBokstav;

    private string nuvarandeBokstavMorse;

    [SerializeField] private TextAsset[] ordListor;

    [SerializeField] private TMP_Dropdown nuvarandeOrdlistaDropdown;
    [SerializeField] private TMP_Dropdown språk;

    private int nuvarandeOrdlista = 0;

    [SerializeField] private GameObject[] KeyboardObjects;

    [SerializeField] private Slider sliderUnit;
    [SerializeField] private TMP_InputField inputFieldUnit;

    [SerializeField] private Slider sliderFrequency;
    [SerializeField] private TMP_InputField inputFieldFrequency;

    [SerializeField] private Slider sliderFlashUnit;
    [SerializeField] private TMP_InputField inputFieldFlashUnit;

    [SerializeField] private SaveData saveData;

    private char[] pausKaraktärer = new char[3]
    {
        '*',
        '/',
        '|',
    };

    private Skärm skärm = Skärm.Menu;

    // Start is called before the first frame update
    private void Start()
    {
        ord = ordListor[nuvarandeOrdlista].text.Split('\n');
        ResetGui();
        VäljNyttOrd();
        VäljNyBokstav();
    }

    private void ResetGui()
    {        //audioSource = gameObject.AddComponent<AudioSource>();
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
        blinkMorseOrdResult.text = string.Empty;
        blinkMorseBokstavResult.text = string.Empty;
        nuvarandeOrdlistaDropdown.value = nuvarandeOrdlista;
        SetAllMorseBlinkareActive(false);
    }

    public void SetSkärm(int input)
    {
        skärm = (Skärm)input;
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
        nuvarandeOrdMorseMedPaus = LäggTillPaus(nuvarandeOrdMorse);
        gissadeKorrektBokstav = new bool[nuvarandeOrd.Length];
        SetWordColour();
    }

    private void VäljNyBokstav()
    {
        nuvarandeBokstav = Alfabet[UnityEngine.Random.Range(0, Alfabet.Length)];

        nuvarandeBokstavMorse = LäggTillPaus(ConvertToMorse(nuvarandeBokstav));
    }

    private string LäggTillPaus(string input)
    {
        for (int i = 1; i < input.Length; i++)
        {
            if (pausKaraktärer.Any(o => o == input[i]) || (i - 1 >= 0 && pausKaraktärer.Any(o => o == input[i - 1])) /*|| (i + 1 < nuvarandeOrdMorseMedPaus.Length && pausKaraktärer.Any(o => o == nuvarandeOrdMorseMedPaus[i + 1]))*/)
            {
                continue;
            }
            else
            {
                input = input.Insert(i, "*");
            }
        }
        return input;
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
        input = input.Replace("<color=green>", string.Empty);
        input = input.Replace("<color=red>", string.Empty);
        input = input.Replace("</color>", string.Empty);

        string temp = string.Empty;
        for (int i = 0; i < input.Length; i++)
        {
            if (i < nuvarandeOrd.Length && input[i] == nuvarandeOrd[i])
            {
                temp += "<color=green>" + input[i] + "</color>";
            }
            else
            {
                temp += "<color=red>" + input[i] + "</color>";
            }
        }
        return temp;
    }

    private string SetLetterColour(string input)
    {
        input = input.Replace("<color=green>", string.Empty);
        input = input.Replace("<color=red>", string.Empty);
        input = input.Replace("</color>", string.Empty);

        string temp = string.Empty;
        for (int i = 0; i < input.Length; i++)
        {
            if (i < nuvarandeBokstav.Length && input[i] == nuvarandeBokstav[i])
            {
                temp += "<color=green>" + nuvarandeBokstav[i] + "</color>";
            }
            else
            {
                temp += "<color=red>" + nuvarandeBokstav[i] + "</color>";
            }
        }
        return temp;
    }

    public void FlashWordFinished()
    {
        blinkMorseOrdResult.text = SetWordColour(blinkMorseOrd.text.ToLower());
        blinkMorseOrd.text = string.Empty;
        PauseMorse();
        VäljNyttOrd();
        timePlayedChar = 0;
        currentMorseChar = 0;
    }

    public void FlashLetterFinished()
    {
        blinkMorseBokstavResult.text = SetLetterColour(nuvarandeBokstav.ToUpper());
        blinkMorseBokstav.text = string.Empty;
        PauseMorse();
        VäljNyBokstav();
        timePlayedChar = 0;
        currentMorseChar = 0;
    }

    public void ValidateInputString(TMP_InputField component)
    {
        string temp = string.Empty;
        temp = component.text;
        for (int i = 0; i < temp.Length; i++)
        {
            if (!Char.IsLetter(temp[i]))
            {
                temp = temp.Remove(i, 1);
            }
        }
        component.text = temp;
    }

    //public void ValidateInputString(TextMeshProUGUI component)
    //{
    //    string temp = string.Empty;
    //    temp = component.text;
    //    for (int i = 0; i < temp.Length; i++)
    //    {
    //        if (!Char.IsLetter(temp[i]))
    //        {
    //            temp = temp.Remove(i, 1);
    //        }
    //    }
    //}

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
        heldDown = false;
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

    private void SetAllMorseBlinkareActive(bool input)
    {
        for (int i = 0; i < morseBlinkare.Length; i++)
        {
            morseBlinkare[i].SetActive(input);
        }
    }

    public void PlayingMorse()
    {
        playingMorse = !playingMorse;
        FinishedMorseChar(skärm == Skärm.BlinkTillOrd ? nuvarandeOrdMorseMedPaus : nuvarandeBokstavMorse);
        if (!playingMorse)
        {
            SetAllMorseBlinkareActive(false);
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void PauseMorse()
    {
        playingMorse = false;
        SetAllMorseBlinkareActive(false);
        FinishedMorseChar(skärm == Skärm.BlinkTillOrd ? nuvarandeOrdMorseMedPaus : nuvarandeBokstavMorse);
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void RepeatMorse()
    {
        timePlayedChar = 0;
        currentMorseChar = 0;
        playingMorse = true;
        FinishedMorseChar(skärm == Skärm.BlinkTillOrd ? nuvarandeOrdMorseMedPaus : nuvarandeBokstavMorse);
    }

    private bool heldDown = false;
    private float timeHeldDown = 0;

    private float unitLength = 0.04f;

    private float flashUnitLength = 0.5f;

    [SerializeField] private ProceduralAudio proceduralAudio;

    private bool playingMorse;

    private float timePlayedChar = 0;

    private int currentMorseChar;

    [SerializeField] private GameObject[] morseBlinkare;

    private string nuvarandeOrdMorseMedPaus;

    private string nuvarandeBokstav;

    // Update is called once per frame
    private void Update()
    {
        if (skärm == Skärm.OrdTillMorse && !keyboard)
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
        if ((skärm == Skärm.BlinkTillOrd || skärm == Skärm.BlinkTillBokstav) && playingMorse)
        {
            timePlayedChar += Time.deltaTime;
            if (skärm == Skärm.BlinkTillOrd)
            {
                if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '.' && timePlayedChar > flashUnitLength)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeOrdMorseMedPaus);
                }
                else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '-' && timePlayedChar > flashUnitLength * 3)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeOrdMorseMedPaus);
                }
                else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '*' && timePlayedChar > flashUnitLength)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeOrdMorseMedPaus);
                }
                else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '/' && timePlayedChar > flashUnitLength * 3)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeOrdMorseMedPaus);
                }
                else if (nuvarandeOrdMorseMedPaus[currentMorseChar] == '|' && timePlayedChar > flashUnitLength * 6)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeOrdMorseMedPaus);
                }
            }
            else if (skärm == Skärm.BlinkTillBokstav)
            {
                if (nuvarandeBokstavMorse[currentMorseChar] == '.' && timePlayedChar > flashUnitLength)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeBokstavMorse);
                }
                else if (nuvarandeBokstavMorse[currentMorseChar] == '-' && timePlayedChar > flashUnitLength * 3)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeBokstavMorse);
                }
                else if (nuvarandeBokstavMorse[currentMorseChar] == '*' && timePlayedChar > flashUnitLength)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeBokstavMorse);
                }
                else if (nuvarandeBokstavMorse[currentMorseChar] == '/' && timePlayedChar > flashUnitLength * 3)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeBokstavMorse);
                }
                else if (nuvarandeBokstavMorse[currentMorseChar] == '|' && timePlayedChar > flashUnitLength * 6)
                {
                    currentMorseChar++;
                    FinishedMorseChar(nuvarandeBokstavMorse);
                }
            }
        }
    }

    private void FinishedMorseChar(string input)
    {
        timePlayedChar = 0;
        if (currentMorseChar < input.Length)
        {
            if (input[currentMorseChar] == '.' || input[currentMorseChar] == '-')
            {
                SetAllMorseBlinkareActive(true);
                if (!audioSource.isPlaying)
                {
                    //timeIndex = 0;  //resets timer before playing sound
                    audioSource.Play();
                }
            }
            else if (input[currentMorseChar] == '|' || input[currentMorseChar] == '/' || input[currentMorseChar] == '*')
            {
                SetAllMorseBlinkareActive(false);
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
        else
        {
            playingMorse = false;
            SetAllMorseBlinkareActive(false);
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void LoadSave(SaveData saveData)
    {
        unitLength = saveData.unitLength;
        flashUnitLength = saveData.flashUnitLength;
        frequency = saveData.soundFrequency;

        if (nuvarandeOrdlista != saveData.wordList)
        {
            nuvarandeOrdlista = saveData.wordList;
            ord = ordListor[nuvarandeOrdlista].text.Split('\n');
            VäljNyttOrd();
        }
        ResetGui();
        språk.value = saveData.language;
        if (!active)
        {
            StartCoroutine(SetLocale(saveData.language));
        }
    }

    public void Save(ref SaveData saveData)
    {
        saveData.unitLength = unitLength;
        saveData.flashUnitLength = flashUnitLength;
        saveData.soundFrequency = frequency;
        saveData.wordList = nuvarandeOrdlista;
        saveData.language = språk.value;
    }

    private bool active = false;

    private IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
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