using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using KMHelper;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class dragonEnergy : MonoBehaviour {

    private static int _moduleIdCounter = 1;
    public KMAudio newAudio;
    public KMBombModule module;
    public KMBombInfo info;
    private int _moduleId = 0;
    private bool _isSolved = false, _lightsOn = false, manualColorblindMode = false, specialFourth = false, specialFourthDragon = false;

    public KMSelectable submit, left, right;

    public GameObject[] sprites;
    public GameObject[] displaySprites;

    private Word[] displayed;

    public bool logSVG = false;

    private int[] badTimes;

    public Material[] colors; //0=orange,1=cyan,2=purple
    public Material off, on;
    private int indicatorColor;

    public TextMesh colorblindText;
    public GameObject colorblindObj;

    public MeshRenderer stage1, stage2, stage3, stage4, indicator;

    private Word Ambition, Anger, Beauty, Brave, Courage, Crisis, Death, Destiny, Devotion, DoubleHappiness, Dragon, Dream, Energy, Eternity, Female, Fortune, Freedom, GoodLuck, Happiness, Hate, Health, Honor, Kind, Life, Longevity, Love, Male, Soul, Wisdom, Wood;
    private Word[] words;
    private Word[] swapped = new Word[] { };
    private int currentDisplay, stage, stage1submit, stage2submit, stage3submit;

    private Word[] correctWords = new Word[]{};

    private List<string> modules;

    void Start()
    {
        _moduleId = _moduleIdCounter++;
        module.OnActivate += Activate;
    }

    void Activate()
    {
        Init();
        _lightsOn = true;
    }

    private void Awake()
    {
        submit.OnInteract += delegate
        {
            handleSubmit();
            return false;
        };
        left.OnInteract += delegate
        {
            handleLeft(); 
            return false;
        };
        right.OnInteract += delegate
        {
            handleRight();
            return false;
        };
    }

    void Init()
    {
        stage = 1;
        setupWords();
        words = new Word[] { Ambition, Anger, Beauty, Brave, Courage, Crisis, Death, Destiny, Devotion, DoubleHappiness, Dragon, Dream, Energy, Eternity, Female, Fortune, Freedom, GoodLuck, Happiness, Hate, Health, Honor, Kind, Life, Longevity, Love, Male, Soul, Wisdom, Wood };
        stage1.material = off;
        stage2.material = off;
        stage3.material = off;
        stage4.material = off;
        indicator.material = off;
        setupIndicator();
        currentDisplay = Random.Range(0, 30);
        setupThreeWords();
        DisplayCurrent();
        modules = info.GetModuleNames();
        if (!logSVG)
        {
            logWordPositions();
        }
        Debug.LogFormat("[DragonEnergy #{0}] Note: Answer is not calculated until submit is pressed.", _moduleId);
    }

    void setupIndicator()
    {
        indicatorColor = Random.Range(0, 3);
        indicator.material = colors[indicatorColor];
        switch (indicatorColor)
        {
            case 0:
                Debug.LogFormat("[DragonEnergy #{0}] Indicator color: Orange.", _moduleId);
                colorblindText.text = "Orange";
                break;
            case 1:
                Debug.LogFormat("[DragonEnergy #{0}] Indicator color: Cyan.", _moduleId);
                colorblindText.text = "Cyan";
                break;
            case 2:
                Debug.LogFormat("[DragonEnergy #{0}] Indicator color: Purple.", _moduleId);
                colorblindText.text = "Purple";
                break;
        }
        if(GetComponent<KMColorblindMode>().ColorblindModeActive || manualColorblindMode)
        {
            colorblindObj.SetActive(true);
            Debug.LogFormat("[DragonEnergy #{0}] Colorblind Mode enabled.", _moduleId);
        }
        else
        {
            colorblindObj.SetActive(false);
        }
    }

    void InitSwaps()
    {
        char[] letters = info.GetSerialNumberLetters().ToArray();
        int vowelCount = 0;
        foreach(char letter in letters)
        {
            if(letter == 'A' || letter == 'E' || letter == 'I' || letter == 'O' || letter == 'U')
            {
                vowelCount++;
            }
        }
        if(info.GetBatteryCount() > 10 && (info.GetSerialNumberNumbers().ToArray()[info.GetSerialNumberNumbers().ToArray().Length-1] == 5 || info.GetSerialNumberNumbers().ToArray()[info.GetSerialNumberNumbers().ToArray().Length - 1] == 7))
        {
            Swaps(1);
        }
        else if (info.GetPortPlateCount()>info.GetBatteryHolderCount() && (modules.Contains("Morse War") || modules.Contains("Double Color")))
        {
            Swaps(2);
        } else if((info.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.SIG) && info.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.FRK)) || (info.GetOffIndicators().Count() == 3))
        {
            Swaps(3);
        } else if (info.GetModuleNames().Count() > 8)
        {
            
            Swaps(4);
        } else if (vowelCount >= 2)
        {
            Swaps(5);
        } else if (info.GetSolvedModuleNames().Count() == 0)
        {
            Swaps(6);
        } else
        {
            Swaps(7);
        }
    }

    void Swaps(int swap)
    {
        switch (swap){
            case 1:

                SwapSet(Circle.GREENPURPLE, Circle.GREEN);
                SwapSet(Circle.GREENRED, Circle.RED);
                SwapSet(Circle.CYANRED, Circle.CYAN);
                SwapSet(Circle.PURPLE, Circle.CYANPURPLE);

                Debug.LogFormat("[DragonEnergy #{0}] Swap 1 has occurred!", _moduleId);
                logWordPositions();
                break;
            case 2:

                SwapSet(Circle.CYANRED, Circle.GREENPURPLE);
                SwapSet(Circle.GREENRED, Circle.CYANPURPLE);

                Debug.LogFormat("[DragonEnergy #{0}] Swap 2 has occurred!", _moduleId);
                logWordPositions();
                break;
            case 3:

                SwapSet(Circle.GREENPURPLE, Circle.CYAN);
                SwapSet(Circle.GREENRED, Circle.PURPLE);
                SwapSet(Circle.CYANRED, Circle.GREEN);
                SwapSet(Circle.RED, Circle.CYANPURPLE);

                Debug.LogFormat("[DragonEnergy #{0}] Swap 3 has occurred!", _moduleId);
                logWordPositions();
                break;
            case 4:

                SwapSet(Circle.GREENPURPLERED, Circle.GREENREDCYAN);
                SwapSet(Circle.CYANPURPLEGREEN, Circle.REDCYANPURPLE);
                SwapSet(Circle.GREENRED, Circle.GREENREDCYANPURPLE);
                SwapSet(Circle.PURPLE, Circle.CYAN);

                Debug.LogFormat("[DragonEnergy #{0}] Swap 4 has occurred!", _moduleId);
                logWordPositions();
                break;
            case 5:

                SwapSet(Circle.GREEN, Circle.RED);
                SwapSet(Circle.CYANPURPLE, Circle.GREENREDCYANPURPLE);
                SwapSet(Circle.CYAN, Circle.CYANRED);

                Debug.LogFormat("[DragonEnergy #{0}] Swap 5 has occurred!", _moduleId);
                logWordPositions();
                break;
            case 6:

                SwapSet(Circle.GREENPURPLE, Circle.GREENREDCYANPURPLE);

                Word.Swap(Wisdom, Love);

                if (!swapped.Contains(Wisdom))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Wisdom;
                }
                if (!swapped.Contains(Love))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Love;
                }
                Debug.LogFormat("[DragonEnergy #{0}] Swap 6 has occurred!", _moduleId);
                logWordPositions();
                break;
            case 7:

                Word.Swap(Wood, Dream);

                if (!swapped.Contains(Wood))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Wood;
                }
                if (!swapped.Contains(Dream))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Dream;
                }
                Word.Swap(Courage, Hate);

                if (!swapped.Contains(Courage))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Courage;
                }
                if (!swapped.Contains(Hate))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Hate;
                }
                Word.Swap(Freedom, Honor);

                if (!swapped.Contains(Freedom))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Freedom;
                }
                if (!swapped.Contains(Honor))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Honor;
                }
                Word.Swap(Female, Dragon);

                if (!swapped.Contains(Female))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Female;
                }
                if (!swapped.Contains(Dragon))
                {
                    Array.Resize(ref swapped, swapped.Length + 1);
                    swapped[swapped.Length - 1] = Dragon;
                }
                int last = info.GetSerialNumberNumbers().ToArray()[info.GetSerialNumberNumbers().ToArray().Length - 1];
                Debug.LogFormat("[DragonEnergy #{0}] Swap 7 has occurred!", _moduleId);
                logWordPositions();
                if (last == 0 || last == 8 || last == 9 || last == 7) { break; }
                Swaps(last);
                break;
        }
    }

    void getCorrectAnswer(int s)
    {
        correctWords = new Word[] { };
        switch (s)
        {
            case 1:
                InitSwaps();
                Circle[] correctPrimaries = new Circle[] { };
                foreach(Circle circle in getPrimarys(displayed[0]))
                {
                    Circle one = circle;
                    foreach(Circle nextCircle in getPrimarys(displayed[1]))
                    {
                        Circle two = nextCircle;
                        foreach (Circle lastCircle in getPrimarys(displayed[2]))
                        {
                            Circle three = nextCircle;
                            if (one == two && one == three)
                            {
                                if (!correctPrimaries.Contains(three))
                                {
                                    Array.Resize(ref correctPrimaries, correctPrimaries.Length + 1);
                                    correctPrimaries[correctPrimaries.Length - 1] = three;
                                }
                            }
                        }
                    }
                }
                if(correctPrimaries.Length > 0)
                {
                    foreach(Circle primary in correctPrimaries)
                    {
                        foreach(Word word in words)
                        {
                            if (getPrimarys(word).Contains(primary))
                            {
                                if (!correctWords.Contains(word))
                                {
                                    Array.Resize(ref correctWords, correctWords.Length + 1);
                                    correctWords[correctWords.Length - 1] = word;
                                }
                            }
                        }
                    }
                    return;
                }
                int secondary = 0;
                foreach(Word word in displayed)
                {
                    if (word.getPosition().getLevel() == Level.SECONDARY)
                    {
                        secondary++;
                    }
                }
                bool second = false;
                if(secondary > 1)
                {
                    if(displayed[0].getPosition().getLevel() == Level.SECONDARY)
                    {
                        if(displayed[0].getPosition().getCircle() == displayed[1].getPosition().getCircle())
                        {
                            second = true;
                        } else if (displayed[0].getPosition().getCircle() == displayed[2].getPosition().getCircle())
                        {
                            second = true;
                        } else if (displayed[1].getPosition().getLevel() == Level.SECONDARY && displayed[1].getPosition().getCircle() == displayed[2].getPosition().getCircle())
                        {
                            second = true;
                        }
                    }
                }
                if (second)
                {
                    foreach(Word word in words)
                    {
                        if(word.getPosition().getLevel() == Level.SECONDARY)
                        {
                            if (!correctWords.Contains(word))
                            {
                                Array.Resize(ref correctWords, correctWords.Length + 1);
                                correctWords[correctWords.Length - 1] = word;
                            }
                        }
                    }
                    return;
                }
                bool sharePrimary = false;
                foreach (Circle circle in getPrimarys(displayed[0]))
                {
                    Circle one = circle;
                    foreach (Circle nextCircle in getPrimarys(displayed[1]))
                    {
                        Circle two = nextCircle;
                        foreach (Circle lastCircle in getPrimarys(displayed[2]))
                        {
                            Circle three = nextCircle;
                            if (one == two || one == three || two == three)
                            {
                                sharePrimary = true;
                            }
                        }
                    }
                }
                if (!sharePrimary)
                {
                    foreach (Word word in words)
                    {
                        if (word.getPosition().getLevel() == Level.QUATERNARY)
                        {
                            if (!correctWords.Contains(word))
                            {
                                Array.Resize(ref correctWords, correctWords.Length + 1);
                                correctWords[correctWords.Length - 1] = word;
                            }
                        }
                    }
                    return;
                }
                foreach (Word word in words)
                {
                    if (word.getPosition().getLevel() == Level.TERTIARY)
                    {
                        if (!correctWords.Contains(word))
                        {
                            Array.Resize(ref correctWords, correctWords.Length + 1);
                            correctWords[correctWords.Length - 1] = word;
                        }
                    }
                }
                return;
            case 2:
                int q = 0;
                int t = 0;
                Position tPos;
                int se = 0;
                Position pos1 = displayed[0].getPosition();
                Position pos2 = displayed[1].getPosition();
                Position pos3 = displayed[2].getPosition();
                foreach (Word word in displayed)
                {
                    if (word.getPosition().getLevel() == Level.QUATERNARY)
                    {
                        q++;
                    }
                    else if (word.getPosition().getLevel() == Level.TERTIARY)
                    {
                        if (t == 0)
                        {
                            tPos = word.getPosition();
                        }
                        t++; 
                    } else if (word.getPosition().getLevel() == Level.SECONDARY)
                    {
                        se++;
                    }
                }
                if (se == 1 && t == 1 && q == 1)
                {
                    foreach (Word word in words)
                    {
                        if (word.getPosition().getLevel() == Level.TERTIARY /*word.getPosition() == tPos*/)
                        {
                            if (!correctWords.Contains(word))
                            {
                                Array.Resize(ref correctWords, correctWords.Length + 1);
                                correctWords[correctWords.Length - 1] = word;
                            }
                        }
                    }
                    return;
                }
                if(pos1.getLevel() == Level.SECONDARY && pos2.getLevel() == Level.SECONDARY && pos3.getLevel() == Level.SECONDARY)
                {
                    foreach (Word word in words)
                    {
                        if (word.getPosition().getLevel() == Level.SECONDARY)
                        {
                            if (!correctWords.Contains(word))
                            {
                                Array.Resize(ref correctWords, correctWords.Length + 1);
                                correctWords[correctWords.Length - 1] = word;
                            }
                        }
                    }
                    return;
                }
                int exc = 0;
                foreach (Word word in displayed)
                {
                    if (word.getPosition().getLevel() == Level.EXCLUDED)
                    {
                        exc++;
                    }
                }
                if (exc == 2)
                {
                    foreach (Word word in words)
                    {
                        if (word.getPosition().getLevel() == Level.QUATERNARY)
                        {
                            if (!correctWords.Contains(word))
                            {
                                Array.Resize(ref correctWords, correctWords.Length + 1);
                                correctWords[correctWords.Length - 1] = word;
                            }
                        }
                    }
                    return;
                }
                foreach (Word word in words)
                {
                    if (word.getPosition().getLevel() == Level.EXCLUDED)
                    {
                        if (!correctWords.Contains(word))
                        {
                            Array.Resize(ref correctWords, correctWords.Length + 1);
                            correctWords[correctWords.Length - 1] = word;
                        }
                    }
                }
                return;
            case 3:
                int swap;
                switch (stage1submit)
                {
                    case 7:
                        swap = 1;
                        break;
                    case 8:
                        swap = 2;
                        break;
                    case 9:
                        swap = 3;
                        break;
                    case 0:
                        swap = 4;
                        break;
                    default:
                        swap = stage1submit;
                        break;
                }
                Swaps(swap);
                bool swapThreeTimes = false;
                foreach(Word word in words)
                {
                    if(word.getSwapCount() >= 3)
                    {
                        swapThreeTimes = true;
                        break;
                    }
                }
                if (swapThreeTimes)
                {
                    foreach (Word word in words)
                    {
                        if (word.getPosition().getLevel() == Level.TERTIARY)
                        {
                            if (!correctWords.Contains(word))
                            {
                                Array.Resize(ref correctWords, correctWords.Length + 1);
                                correctWords[correctWords.Length - 1] = word;
                            }
                        }
                    }
                    return;
                }
                foreach (Word word in words)
                {
                    if (word.getPosition().getLevel() == Level.EXCLUDED)
                    {
                        if (!correctWords.Contains(word))
                        {
                            Array.Resize(ref correctWords, correctWords.Length + 1);
                            correctWords[correctWords.Length - 1] = word;
                        }
                    }
                }
                return;
            case 4:
                if(Dragon.getPosition().getLevel() == Level.QUATERNARY && Energy.getPosition().getLevel() == Level.QUATERNARY)
                {
                    specialFourth = true;
                    return;
                }
                if(Dragon.getPosition().getLevel() == Level.QUATERNARY)
                {
                    if (!correctWords.Contains(Dragon))
                    {
                        Array.Resize(ref correctWords, correctWords.Length + 1);
                        correctWords[correctWords.Length - 1] = Dragon;
                    }
                    return;
                }
                foreach(Word word in words)
                {
                    if(word.getSwapCount() == 0)
                    {
                        if (!correctWords.Contains(word))
                        {
                            Array.Resize(ref correctWords, correctWords.Length + 1);
                            correctWords[correctWords.Length - 1] = word;
                        }
                    }
                }
                if(correctWords.Length != 0)
                {
                    return;
                }
                if (!correctWords.Contains(Wisdom))
                {
                    Array.Resize(ref correctWords, correctWords.Length + 1);
                    correctWords[correctWords.Length - 1] = Wisdom;
                }
                return;
        }
    }

    void handleSubmit()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submit.transform);
        if (!_lightsOn || _isSolved) return;
        if (!specialFourth)
        {
            getCorrectAnswer(stage);
        }
        if(info.GetBatteryHolderCount() == info.GetPortPlateCount())
        {
            switch (indicatorColor)
            {
                case 0:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 5, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 3, 4, 6 };
                            break;
                        default:
                            badTimes = new int[] { 0, 2, 7, 8 };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 2, 3, 6 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 2, 5, 6, 7 };
                            break;
                        default:
                            badTimes = new int[] { 1, 3, 4, 7 };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 3, 4, 7, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 5, 6, 8, 9 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2, 9 };
                            break;
                    }
                    break;
            }
        } else if(info.GetBatteryHolderCount() > info.GetPortPlateCount())
        {
            switch (indicatorColor)
            {
                case 0:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 2, 4, 6, 8 };
                            break;
                        case 1:
                            badTimes = new int[] { 3, 4, 6 };
                            break;
                        default:
                            badTimes = new int[] { 0, 2, 7, 8 };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 1, 3, 5, 7, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 5, 8, 9 };
                            break;
                        default:
                            badTimes = new int[] { 1, 3, 4, 7 };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 2, 3, 4, 5 };
                            break;
                        case 1:
                            badTimes = new int[] { 5, 6, 8, 9 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2 };
                            break;
                    }
                    break;
            }
        } else
        {
            switch (indicatorColor)
            {
                case 0:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 2, 7, 8, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 1, 2, 4, 5, 7, 8 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 4, 5, 6, 7, 8 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 4, 6, 7, 9 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2, 3, 4, 5, 7, 8, 9 };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 1, 3, 4, 6, 7, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 4, 5, 6, 7 };
                            break;
                        default:
                            badTimes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                            break;
                    }
                    break;
            }
        }
        if(badTimes.Contains((int)(info.GetTime()%10)))
        {
            module.HandleStrike();
            Debug.LogFormat("[DragonEnergy #{0}] Submit pressed with {1} in last digit of timer.", _moduleId, (int)(info.GetTime() % 10));
            reset();
            return;
        }
        if (specialFourth)
        {
            if (!specialFourthDragon)
            {
                if (words[currentDisplay].getWord().Equals(Dragon.getWord()))
                {
                    Debug.LogFormat("[DragonEnergy #{0}] Special 4th rule in play, Dragon submitted.", _moduleId);
                    specialFourthDragon = true;
                } else
                {
                    module.HandleStrike();
                    string incorrect = words[currentDisplay].getWord();
                    string correct = "Dragon";
                    Debug.LogFormat("[DragonEnergy #{0}] Incorrect answer submitted. Inputted: {1}. Correct: {2}.", _moduleId, incorrect, correct);
                    reset();
                }
            } else
            {
                if (words[currentDisplay].getWord().Equals(Energy.getWord()))
                {
                    Debug.LogFormat("[DragonEnergy #{0}] Special 4th rule in play, Energy submitted. Module Solved!", _moduleId);
                    stage4.material = on;
                    indicator.material = off;
                    colorblindObj.SetActive(false);
                    module.HandlePass();
                    _isSolved = true;
                }
                else
                {
                    module.HandleStrike();
                    string incorrect = words[currentDisplay].getWord();
                    string correct = "Energy";
                    Debug.LogFormat("[DragonEnergy #{0}] Incorrect answer submitted. Inputted: {1}. Correct: {2}.", _moduleId, incorrect, correct);
                    reset();
                }
            }
            return;
        }
        int secondSubmitted = (int)(info.GetTime() % 10);
        if (correctWords.Contains(words[currentDisplay]))
        {
            stage++;
            switch (stage)
            {
                case 2:
                    stage1.material = on;
                    setupThreeWords();
                    Debug.LogFormat("[DragonEnergy #{0}] Stage 1 solved, word submitted: {1}. Second submitted at: {2}", _moduleId, words[currentDisplay].getWord(), secondSubmitted);
                    stage1submit = secondSubmitted;
                    break;
                case 3:
                    stage2.material = on;
                    setupThreeWords();
                    Debug.LogFormat("[DragonEnergy #{0}] Stage 2 solved, word submitted: {1}. Second submitted at: {2}", _moduleId, words[currentDisplay].getWord(), secondSubmitted);
                    stage2submit = secondSubmitted;
                    break;
                case 4:
                    stage3.material = on;
                    setupThreeWords();
                    Debug.LogFormat("[DragonEnergy #{0}] Stage 3 solved, word submitted: {1}. Second submitted at: {2}", _moduleId, words[currentDisplay].getWord(), secondSubmitted);
                    stage3submit = secondSubmitted;
                    break;
                case 5:
                    stage4.material = on;
                    indicator.material = off;
                    colorblindObj.SetActive(false);
                    module.HandlePass();
                    Debug.LogFormat("[DragonEnergy #{0}] Module solved!", _moduleId);
                    _isSolved = true;
                    break;
            }

        } else
        {
            module.HandleStrike();
            string incorrect = words[currentDisplay].getWord();
            string correct = "";
            foreach(Word word in correctWords)
            {
                correct += word.getWord();
                correct += ", ";
            }
            if (correct.Length > 2)
            {
                correct = correct.Substring(0, correct.Length - 2);
            }
            Debug.LogFormat("[DragonEnergy #{0}] Incorrect answer submitted. Inputted: {1}. Any of the following were correct: {2}.", _moduleId, incorrect, correct);
            reset();
        }
    }
    
    void setupThreeWords()
    {
        foreach(Word word in words)
        {
            word.getSprite().SetActive(false);
        }
        int one = Random.Range(0, 30);
        int two = Random.Range(0, 30);
        do
        {
            two = Random.Range(0, 30);
        } while (two == one);
        int three = Random.Range(0, 30);
        do
        {
            three = Random.Range(0, 30);
        } while (three == one || three == two);

        displayed = new Word[] { words[one], words[two], words[three] };

        words[one].display(1);
        words[two].display(2);
        words[three].display(3);

        Debug.LogFormat("[DragonEnergy #{0}] The words displayed are: BL: {1}, TL: {2}, TR: {3}.", _moduleId, words[one].getWord(), words[two].getWord(), words[three].getWord());
    }

    void handleLeft()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, left.transform);
        if (!_lightsOn || _isSolved) return;
        currentDisplay--;
        if (currentDisplay == -1)
        {
            currentDisplay = 29;
        }
        DisplayCurrent();
    }

    void handleRight()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, right.transform);
        if (!_lightsOn || _isSolved) return;

        currentDisplay++;
        if (currentDisplay == 30)
        {
            currentDisplay = 0;
        }
        DisplayCurrent();
    }

    void setupWords()
    {
        Ambition = new Word(sprites[0], new Position(Level.EXCLUDED, Circle.GREEN), "Ambition");
        Anger = new Word(sprites[1], new Position(Level.EXCLUDED, Circle.PURPLE), "Anger");
        Beauty = new Word(sprites[2], new Position(Level.EXCLUDED, Circle.RED), "Beauty");
        Brave = new Word(sprites[3], new Position(Level.EXCLUDED, Circle.CYAN), "Brave");
        Courage = new Word(sprites[4], new Position(Level.TERTIARY, Circle.GREENPURPLERED), "Courage");
        Crisis = new Word(sprites[5], new Position(Level.TERTIARY, Circle.GREENREDCYAN), "Crisis");
        Death = new Word(sprites[6], new Position(Level.TERTIARY, Circle.REDCYANPURPLE), "Death");
        Destiny = new Word(sprites[7], new Position(Level.SECONDARY, Circle.CYANRED), "Destiny");
        Devotion = new Word(sprites[8], new Position(Level.SECONDARY, Circle.GREENRED), "Devotion");
        DoubleHappiness = new Word(sprites[9], new Position(Level.SECONDARY, Circle.CYANPURPLE), "Double Happiness");
        Dragon = new Word(sprites[10], new Position(Level.QUATERNARY, Circle.GREENREDCYANPURPLE), "Dragon");
        Dream = new Word(sprites[11], new Position(Level.EXCLUDED, Circle.PURPLE), "Dream");
        Energy = new Word(sprites[12], new Position(Level.SECONDARY, Circle.GREENPURPLE), "Energy");
        Eternity = new Word(sprites[13], new Position(Level.SECONDARY, Circle.GREENPURPLE), "Eternity");
        Female = new Word(sprites[14], new Position(Level.EXCLUDED, Circle.GREEN), "Female");
        Fortune = new Word(sprites[15], new Position(Level.TERTIARY, Circle.CYANPURPLEGREEN), "Fortune");
        Freedom = new Word(sprites[16], new Position(Level.SECONDARY, Circle.CYANPURPLE), "Freedom");
        GoodLuck = new Word(sprites[17], new Position(Level.SECONDARY, Circle.CYANPURPLE), "Good Luck");
        Happiness = new Word(sprites[18], new Position(Level.SECONDARY, Circle.GREENRED), "Happiness");
        Hate = new Word(sprites[19], new Position(Level.SECONDARY, Circle.CYANRED), "Hate");
        Health = new Word(sprites[20], new Position(Level.QUATERNARY, Circle.GREENREDCYANPURPLE), "Health");
        Honor = new Word(sprites[21], new Position(Level.SECONDARY, Circle.GREENRED), "Honor");
        Kind = new Word(sprites[22], new Position(Level.EXCLUDED, Circle.RED), "Kind");
        Life = new Word(sprites[23], new Position(Level.SECONDARY, Circle.GREENPURPLE), "Life");
        Longevity = new Word(sprites[24], new Position(Level.SECONDARY, Circle.CYANRED), "Longevity");
        Love = new Word(sprites[25], new Position(Level.EXCLUDED, Circle.CYAN), "Love");
        Male = new Word(sprites[26], new Position(Level.EXCLUDED, Circle.CYAN), "Male");
        Soul = new Word(sprites[27], new Position(Level.EXCLUDED, Circle.PURPLE), "Soul");
        Wisdom = new Word(sprites[28], new Position(Level.EXCLUDED, Circle.RED), "Wisdom");
        Wood = new Word(sprites[29], new Position(Level.EXCLUDED, Circle.GREEN), "Wood");
    }

    public Circle[] getPrimarys(Word word)
    {
        switch (word.getPosition().getCircle())
        {
            case Circle.GREEN:
                return new Circle[] { Circle.GREEN };
            case Circle.GREENRED:
                return new Circle[] { Circle.GREEN, Circle.RED };
            case Circle.GREENPURPLERED:
                return new Circle[] { Circle.GREEN, Circle.RED, Circle.PURPLE };
            case Circle.GREENREDCYANPURPLE:
                return new Circle[] { Circle.GREEN, Circle.RED, Circle.PURPLE, Circle.CYAN };
            case Circle.RED:
                return new Circle[] { Circle.RED };
            case Circle.CYANRED:
                return new Circle[] { Circle.RED, Circle.CYAN };
            case Circle.REDCYANPURPLE:
                return new Circle[] { Circle.RED, Circle.CYAN, Circle.PURPLE };
            case Circle.GREENREDCYAN:
                return new Circle[] { Circle.GREEN, Circle.RED, Circle.CYAN };
            case Circle.CYAN:
                return new Circle[] { Circle.CYAN };
            case Circle.PURPLE:
                return new Circle[] { Circle.PURPLE };
            case Circle.CYANPURPLE:
                return new Circle[] { Circle.CYAN, Circle.PURPLE };
            case Circle.CYANPURPLEGREEN:
                return new Circle[] { Circle.GREEN, Circle.CYAN, Circle.PURPLE };
            case Circle.GREENPURPLE:
                return new Circle[] { Circle.GREEN, Circle.PURPLE };
        }
        return new Circle[] { };
    }

    void DisplayCurrent()
    {
        for(int i = 0; i<30; i++)
        {
            displaySprites[i].SetActive(false);
        }
        displaySprites[currentDisplay].SetActive(true);
    }

    void SwapSet(Circle first, Circle second)
    {
        Word[] setOne = new Word[] { };
        Word[] setTwo = new Word[] { };

        foreach (Word word in words)
        {
            Circle circle = word.getPosition().getCircle();
            if (circle == first)
            {
                Array.Resize(ref setOne, setOne.Length + 1);
                setOne[setOne.Length - 1] = word;

            }
            else if (circle == second)
            {
                Array.Resize(ref setTwo, setTwo.Length + 1);
                setTwo[setTwo.Length - 1] = word;

            }
            if (!swapped.Contains(word))
            {
                Array.Resize(ref swapped, swapped.Length + 1);
                swapped[swapped.Length - 1] = word;
            }
        }

        if (setOne.Length > setTwo.Length)
        {
            for (int i = 0; i < setTwo.Length; i++)
            {
                Word.Swap(setTwo[i], setOne[i]);
                Debug.LogFormat("[DragonEnergy #{0}] Swapped " + setTwo[i].getWord() + " and " + setOne[i].getWord(), _moduleId);
            }
            setOne.Last().setPosition(setOne[0].getPosition());
            setOne.Last().wordSwapped();
            Debug.LogFormat("[DragonEnergy #{0}] " + setOne.Last().getWord() + " position set to " + setOne.Last().getPosition().getName(), _moduleId);
        }
        else if (setOne.Length < setTwo.Length)
        {
            for (int i = 0; i < setOne.Length; i++)
            {
                Word.Swap(setTwo[i], setOne[i]);
                Debug.LogFormat("[DragonEnergy #{0}] Swapped " + setTwo[i].getWord() + " and " + setOne[i].getWord(), _moduleId);
            }
            setTwo.Last().setPosition(setTwo[0].getPosition());
            setTwo.Last().wordSwapped();
            Debug.LogFormat("[DragonEnergy #{0}] " + setTwo.Last().getWord() + " position set to " + setTwo.Last().getPosition().getName(), _moduleId);
        }
        else
        {
            for (int i = 0; i < setOne.Length; i++)
            {
                Word.Swap(setTwo[i], setOne[i]);
                Debug.LogFormat("[DragonEnergy #{0}] Swapped " + setTwo[i].getWord() + " and " + setOne[i].getWord(), _moduleId);
            }
        }
    }

    void reset()
    {
        stage = 1;
        setupWords();
        currentDisplay = Random.Range(0, 30);
        setupThreeWords();
        DisplayCurrent();
        setupIndicator();
        specialFourthDragon = false;
        specialFourth = false;
        stage1.material = off;
        stage2.material = off;
        stage3.material = off;
        stage4.material = off;
        Debug.LogFormat("[DragonEnergy #{0}] Words reset.",_moduleId);
        logWordPositions();
    }

    void logWordPositions()
    {

        if (logSVG == true)
        {
            String[] prim1 = new String[3] { "", "", "" };
            String[] prim2 = new String[3] { "", "", "" };
            String[] prim3 = new String[3] { "", "", "" };
            String[] prim4 = new String[3] { "", "", "" };

            String[] sec1 = new String[3] { "", "", "" };
            String[] sec2 = new String[3] { "", "", "" };
            String[] sec3 = new String[3] { "", "", "" };
            String[] sec4 = new String[3] { "", "", "" };

            String tert1 = "";
            String tert2 = "";
            String tert3 = "";
            String tert4 = "";

            String[] quart = new String[3] { "", "", "" };

            foreach (Word word in words)
            {
                if (word.getPosition().getCircle() == Circle.GREEN)
                {
                    if (prim1[0] == "")
                    {
                        prim1[0] = word.getWord();
                    }
                    else if (prim1[1] == "")
                    {
                        prim1[1] = word.getWord();
                    }
                    else
                    {
                        prim1[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.PURPLE)
                {
                    if (prim2[0] == "")
                    {
                        prim2[0] = word.getWord();
                    }
                    else if (prim2[1] == "")
                    {
                        prim2[1] = word.getWord();
                    }
                    else
                    {
                        prim2[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.RED)
                {
                    if (prim3[0] == "")
                    {
                        prim3[0] = word.getWord();
                    }
                    else if (prim3[1] == "")
                    {
                        prim3[1] = word.getWord();
                    }
                    else
                    {
                        prim3[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.CYAN)
                {
                    if (prim4[0] == "")
                    {
                        prim4[0] = word.getWord();
                    }
                    else if (prim4[1] == "")
                    {
                        prim4[1] = word.getWord();
                    }
                    else
                    {
                        prim4[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.GREENPURPLE)
                {
                    if (sec1[0] == "")
                    {
                        sec1[0] = word.getWord();
                    }
                    else if (sec1[1] == "")
                    {
                        sec1[1] = word.getWord();
                    }
                    else
                    {
                        sec1[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.CYANPURPLE)
                {
                    if (sec2[0] == "")
                    {
                        sec2[0] = word.getWord();
                    }
                    else if (sec2[1] == "")
                    {
                        sec2[1] = word.getWord();
                    }
                    else
                    {
                        sec2[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.CYANRED)
                {
                    if (sec3[0] == "")
                    {
                        sec3[0] = word.getWord();
                    }
                    else if (sec3[1] == "")
                    {
                        sec3[1] = word.getWord();
                    }
                    else
                    {
                        sec3[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.GREENRED)
                {
                    if (sec4[0] == "")
                    {
                        sec4[0] = word.getWord();
                    }
                    else if (sec4[1] == "")
                    {
                        sec4[1] = word.getWord();
                    }
                    else
                    {
                        sec4[2] = word.getWord();
                    }
                }
                else if (word.getPosition().getCircle() == Circle.GREENPURPLERED)
                {
                    tert1 = word.getWord();
                }
                else if (word.getPosition().getCircle() == Circle.CYANPURPLEGREEN)
                {
                    tert2 = word.getWord();
                }
                else if (word.getPosition().getCircle() == Circle.REDCYANPURPLE)
                {
                    tert3 = word.getWord();
                }
                else if (word.getPosition().getCircle() == Circle.GREENREDCYAN)
                {
                    tert4 = word.getWord();
                }
                else if (word.getPosition().getCircle() == Circle.GREENREDCYANPURPLE)
                {
                    if (quart[0] == "")
                    {
                        quart[0] = word.getWord();
                    }
                    else if (quart[1] == "")
                    {
                        quart[1] = word.getWord();
                    }
                    else
                    {
                        quart[2] = word.getWord();
                    }
                }

            }

            String svg = "<svg xmlns:osb='http://www.openswatchbook.org/uri/2009/osb' xmlns:dc='http://purl.org/dc/elements/1.1/' xmlns:cc='http://creativecommons.org/ns#' xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#' xmlns:svg='http://www.w3.org/2000/svg' xmlns='http://www.w3.org/2000/svg' xmlns:sodipodi='http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd' xmlns:inkscape='http://www.inkscape.org/namespaces/inkscape' width='7in' height='7in' viewBox='0 0 177.8 177.8' version='1.1' id='svg8' inkscape:version='0.92.3 (2405546, 2018-03-11)' sodipodi:docname='VennDiagramDragon.svg' inkscape:export-xdpi='96' inkscape:export-ydpi='96'>   <defs  id='defs2'> <linearGradient id='linearGradient830' osb:paint='solid'>   <stop style='stop-color:#000000;stop-opacity:1;' offset='0' id='stop828' /> </linearGradient>   </defs>   <sodipodi:namedview  id='base'  pagecolor='#ffffff'  bordercolor='#666666'  borderopacity='1.0'  inkscape:pageopacity='0.0'  inkscape:pageshadow='2'  inkscape:zoom='0.98994949'  inkscape:cx='331.05441'  inkscape:cy='281.47017'  inkscape:document-units='mm'  inkscape:current-layer='layer1'  showgrid='false'  units='in'  height='15.5in'  inkscape:window-width='1145'  inkscape:window-height='900'  inkscape:window-x='1063'  inkscape:window-y='327'  inkscape:window-maximized='0' />   <metadata  id='metadata5'> <rdf:RDF>   <cc:Work rdf:about=''>  <dc:format>image/svg+xml</dc:format>  <dc:type   rdf:resource='http://purl.org/dc/dcmitype/StillImage' />  <dc:title></dc:title>   </cc:Work> </rdf:RDF>   </metadata>   <g  inkscape:label='Layer 1'  inkscape:groupmode='layer'  id='layer1'  transform='translate(0,-119.2)'> <ellipse style='fill:#ff0000;fill-opacity:0.21568627;fill-rule:nonzero;stroke:none;stroke-width:0.06025632;stroke-miterlimit:4;stroke-dasharray:none' id='path2479-7' cx='61.296223' cy='235.54463' rx='60.262226' ry='60.440083' /> <ellipse style='fill:#0000ff;fill-opacity:0.21568627;fill-rule:nonzero;stroke:none;stroke-width:0.06025632;stroke-miterlimit:4;stroke-dasharray:none' id='path2479-7-7' cx='116.13956' cy='180.19061' rx='60.262226' ry='60.440083' /> <ellipse style='opacity:1;fill:#00ff00;fill-opacity:0.19607843;fill-rule:nonzero;stroke:none;stroke-width:0.06016936;stroke-miterlimit:4;stroke-dasharray:none' id='path2479-7-9' cx='61.021038' cy='180.53941' rx='60.088352' ry='60.440083' /> <ellipse style='fill:#03aeff;fill-opacity:0.21568627;fill-rule:nonzero;stroke:none;stroke-width:0.06025632;stroke-miterlimit:4;stroke-dasharray:none' id='path2479-7-2' cx='116.13956' cy='235.64622' rx='60.262226' ry='60.440083' /> <text xml:space='preserve' style='font-style:normal;font-weight:normal;font-size:4.87569237px;line-height:1.25;font-family:sans-serif;letter-spacing:0px;word-spacing:0px;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' x='34.54678' y='194.01733' id='text2559' transform='scale(0.99852753,1.0014746)'><tspan sodipodi:role='line' id='tspan2557' x='34.54678' y='194.01733' style='stroke-width:0.12189227'>" + sec4[0] + "</tspan></text> <text xml:space='preserve' style='font-style:normal;font-weight:normal;font-size:4.87569237px;line-height:1.25;font-family:sans-serif;letter-spacing:0px;word-spacing:0px;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' x='42.160892' y='135.13184' id='text2563' transform='scale(0.99852753,1.0014746)'><tspan sodipodi:role='line' id='tspan2561' x='42.160892' y='135.13184' style='stroke-width:0.12189227'>" + prim1[0] + "</tspan><tspan sodipodi:role='line' x='42.160892' y='141.22646' style='stroke-width:0.12189227' id='tspan2565' /></text> <flowRoot xml:space='preserve' id='flowRoot2567' style='font-style:normal;font-weight:normal;font-size:40px;line-height:1.25;font-family:sans-serif;letter-spacing:0px;word-spacing:0px;fill:#000000;fill-opacity:1;stroke:none' transform='matrix(0.26458333,0,0,0.26458333,0,-84)'><flowRegion id='flowRegion2569'><rect   id='rect2571'   width='88.893425'   height='66.670067'   x='1563.7162'   y='351.05554' /></flowRegion><flowPara id='flowPara2573'>Amger</flowPara></flowRoot> <text xml:space='preserve' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' x='32.930943' y='154.01598' id='text2563-1' transform='scale(0.99852753,1.0014746)'><tspan sodipodi:role='line' id='tspan2918' x='32.930943' y='154.01598' style='stroke-width:0.12189227'>" + prim1[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2922' y='169.63661' x='13.849557' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan2924' x='13.849557' y='169.63661' style='stroke-width:0.12189227'>" + prim1[2] + "</tspan></text> <text xml:space='preserve' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' x='23.185192' y='209.85809' id='text2559-1' transform='scale(0.99852753,1.0014746)'><tspan sodipodi:role='line' id='tspan2952' x='23.185192' y='209.85809' style='stroke-width:0.12189227'>" + sec4[1] + "</tspan></text> <text xml:space='preserve' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' x='31.318241' y='223.91666' id='text2559-5' transform='scale(0.99852753,1.0014746)'><tspan sodipodi:role='line' id='tspan2954' x='31.318241' y='223.91666' style='stroke-width:0.12189227'>" + sec4[2] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958' y='249.53448' x='21.621145' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3187' x='21.621145' y='249.53448' style='stroke-width:0.12189227'>" + prim4[0] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-8' y='264.85458' x='33.027348' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3189' x='33.027348' y='264.85458' style='stroke-width:0.12189227'>" + prim4[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-7' y='282.97455' x='46.478165' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3191' x='46.478165' y='282.97455' style='stroke-width:0.12189227'>" + prim4[2] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-9' y='249.23396' x='84.015312' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3209' x='84.015312' y='249.23396' style='stroke-width:0.12189227'>" + sec3[0] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-94' y='260.1684' x='86.830597' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3207' x='86.830597' y='260.1684' style='stroke-width:0.12189227'>" + sec3[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-2' y='272.04007' x='80.261597' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3205' x='80.261597' y='272.04007' style='stroke-width:0.12189227'>" + sec3[2] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-1' y='248.60912' x='156.89993' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3197' x='156.89993' y='248.60912' style='stroke-width:0.12189227'>" + prim3[0] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-85' y='264.85458' x='139.3826' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3195' x='139.3826' y='264.85458' style='stroke-width:0.12189227'>" + prim3[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-4' y='284.22421' x='124.99335' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3193' x='124.99335' y='284.22421' style='stroke-width:0.12189227'>" + prim3[2] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5' y='135.82814' x='126.5574' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3203' x='126.5574' y='135.82814' style='stroke-width:0.12189227'>" + prim2[0] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-3' y='154.88535' x='137.50574' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3201' x='137.50574' y='154.88535' style='stroke-width:0.12189227'>" + prim2[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-39' y='173.00525' x='154.08466' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3199' x='154.08466' y='173.00525' style='stroke-width:0.12189227'>" + prim2[2] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-2' y='195.18654' x='128.74707' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3233' x='128.74707' y='195.18654' style='stroke-width:0.12189227'>" + sec2[0] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-28' y='209.24515' x='120.30122' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3231' x='120.30122' y='209.24515' style='stroke-width:0.12189227'>" + sec2[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-9' y='223.61612' x='126.24461' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3235' x='126.24461' y='223.61612' style='stroke-width:0.12189227'>" + sec2[2] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-8' y='186.43901' x='99.655792' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3217' x='99.655792' y='186.43901' style='stroke-width:0.12189227'>" + tert2 + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-0' y='232.36369' x='103.40951' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3211' x='103.40951' y='232.36369' style='stroke-width:0.12189227'>" + tert3 + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-33' y='186.1266' x='58.990543' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3215' x='58.990543' y='186.1266' style='stroke-width:0.12189227'>" + tert1 + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-00' y='196.43623' x='80.887215' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3225' x='80.887215' y='199.43623' style='stroke-width:0.12189227'>" + quart[0] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-00' y='196.43623' x='80.887215' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3225' x='80.887215' y='209.43623' style='stroke-width:0.12189227'>" + quart[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-97' y='221.11682' x='84.015305' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3229' x='79.948784' y='219.11682' style='stroke-width:0.12189227'>" + quart[2] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-1' y='231.11401' x='61.180202' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3213' x='61.180202' y='231.11401' style='stroke-width:0.12189227'>" + tert4 + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-22' y='149.2619' x='79.948784' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3221' x='79.948784' y='149.2619' style='stroke-width:0.12189227'>" + sec1[0] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-4' y='159.88393' x='84.64093' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3219' x='84.64093' y='159.88393' style='stroke-width:0.12189227'>" + sec1[1] + "</tspan></text> <text transform='scale(0.99852753,1.0014746)' id='text2958-5-44' y='171.44318' x='81.200035' style='font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:4.87558413px;line-height:1.25;font-family:sans-serif;-inkscape-font-specification:'sans-serif, Normal';font-variant-ligatures:normal;font-variant-caps:normal;font-variant-numeric:normal;font-feature-settings:normal;text-align:start;letter-spacing:0px;word-spacing:0px;writing-mode:lr-tb;text-anchor:start;fill:#000000;fill-opacity:1;stroke:none;stroke-width:0.12189227' xml:space='preserve'><tspan sodipodi:role='line' id='tspan3223' x='81.200035' y='171.44318' style='stroke-width:0.12189227'>" + sec1[2] + "</tspan></text></g></svg>";
            Debug.LogFormat("[DragonEnergy #{0}]=svg[Current word positions:]{1}", _moduleId, svg);
        } else
        {
            Debug.LogFormat("[DragonEnergy #{0}] Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());
        }
    }

#pragma warning disable 414

    private string TwitchHelpMessage = "Submit a word with !{0} [word]. Use !{0} colorblind to turn on colorblind mode.";

#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string input)
    {
        if (input.ToLowerInvariant().Equals("colorblind"))
        {
            yield return null;
            colorblindObj.SetActive(true);
            Debug.LogFormat("[Double Color #{0}] Colorblind mode enabled via TP.", _moduleId);
            manualColorblindMode = true;
            yield break;
        }
        bool found = false;
        Word inputted = Ambition;
        foreach (Word word in words)
        {
            if (input.ToLowerInvariant() == word.getWord().ToLowerInvariant()){
                inputted = word;
                found = true;
            }
            
        }
        if (!found)
        {
            yield return "sendtochaterror Invalid word entered: " + input + "!";
            yield break;
        }
        while (words[currentDisplay].getWord() != inputted.getWord())
        {
            left.OnInteract();
        }
        yield return null;
        if (info.GetBatteryHolderCount() == info.GetPortPlateCount())
        {
            switch (indicatorColor)
            {
                case 0:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 5, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 3, 4, 6 };
                            break;
                        default:
                            badTimes = new int[] { 0, 2, 7, 8 };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 2, 3, 6 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 2, 5, 6, 7 };
                            break;
                        default:
                            badTimes = new int[] { 1, 3, 4, 7 };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 3, 4, 7, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 5, 6, 8, 9 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2, 9 };
                            break;
                    }
                    break;
            }
        }
        else if (info.GetBatteryHolderCount() > info.GetPortPlateCount())
        {
            switch (indicatorColor)
            {
                case 0:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 2, 4, 6, 8 };
                            break;
                        case 1:
                            badTimes = new int[] { 3, 4, 6 };
                            break;
                        default:
                            badTimes = new int[] { 0, 2, 7, 8 };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 1, 3, 5, 7, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 5, 8, 9 };
                            break;
                        default:
                            badTimes = new int[] { 1, 3, 4, 7 };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 2, 3, 4, 5 };
                            break;
                        case 1:
                            badTimes = new int[] { 5, 6, 8, 9 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2 };
                            break;
                    }
                    break;
            }
        }
        else
        {
            switch (indicatorColor)
            {
                case 0:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 0, 1, 2, 7, 8, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 1, 2, 4, 5, 7, 8 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 4, 5, 6, 7, 8 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 4, 6, 7, 9 };
                            break;
                        default:
                            badTimes = new int[] { 0, 1, 2, 3, 4, 5, 7, 8, 9 };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { 1, 3, 4, 6, 7, 9 };
                            break;
                        case 1:
                            badTimes = new int[] { 0, 4, 5, 6, 7 };
                            break;
                        default:
                            badTimes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                            break;
                    }
                    break;
            }
        }
        while (badTimes.Contains((int)(info.GetTime() % 10))) yield return "trycancel Submit wasn't pressed due to request to cancel.";
        submit.OnInteract();
    }
}

public class Word
{
    private GameObject sprite;
    private Position position;
    private String name;
    private int swapCount = 0;

    private static float x1 = -0.05256132f;
    private static float y1 = 0.02664061f;
    private static float z1 = -0.007111584f;
    private static float x2 = -0.05256132f;
    private static float y2 = 0.02664061f;
    private static float z2 = 0.04768842f;
    private static float x3 = 0.0014386f;
    private static float y3 = 0.02664061f;
    private static float z3 = 0.0476884f;

    public Word(GameObject sprite, Position pos, String name)
    {
        this.sprite = sprite;
        this.position = pos;
        this.name = name;
    }

    public static void Swap(Word first, Word second)
    {
        Position temp = first.getPosition();
        first.setPosition(second.getPosition());
        second.setPosition(temp);
        first.wordSwapped();
        second.wordSwapped();
    }

    public void wordSwapped()
    {
        this.swapCount++;
    }

    public void display(int location)
    {
        switch (location) {
            case 1:
                sprite.transform.position = new Vector3(x1, y1, z1);
                break;
            case 2:
                sprite.transform.position = new Vector3(x2, y2, z2);
                break;
            case 3:
                sprite.transform.position = new Vector3(x3, y3, z3);
                break;
        }
        sprite.SetActive(true);
    }

    public Position getPosition()
    {
        return this.position;
    }

    public GameObject getSprite()
    {
        return this.sprite;
    }

    public String getWord()
    {
        return this.name;
    }

    public int getSwapCount()
    {
        return this.swapCount;
    }

    public void setPosition(Position pos)
    {
        this.position = pos;
    }
}

public class Position
{
    Level level;
    Circle circle;

    public Position(Level level, Circle circle)
    {
        this.level = level;
        this.circle = circle;
    }

    public Circle getCircle()
    {
        return circle;
    }

    public Level getLevel()
    {
        return level;
    }

    public string getName()
    {
        String name = "";
        switch (level)
        {
            case Level.EXCLUDED:
                name += "Excluded, ";
                switch (circle)
                {
                    case Circle.RED:
                        name += "Red";
                        break;
                    case Circle.GREEN:
                        name += "Green";
                        break;
                    case Circle.CYAN:
                        name += "Cyan";
                        break;
                    case Circle.PURPLE:
                        name += "Purple";
                        break;
                }
                break;
            case Level.SECONDARY:
                name += "Secondary, ";
                switch (circle)
                {
                    case Circle.GREENPURPLE:
                        name += "Green + Purple";
                        break;
                    case Circle.CYANRED:
                        name += "Cyan + Red";
                        break;
                    case Circle.GREENRED:
                        name += "Green + Red";
                        break;
                    case Circle.CYANPURPLE:
                        name += "Cyan + Purple";
                        break;
                }
                break;
            case Level.TERTIARY:
                name += "Tertiary, ";
                switch (circle)
                {
                    case Circle.CYANPURPLEGREEN:
                        name += "Green + Purple + Cyan";
                        break;
                    case Circle.GREENPURPLERED:
                        name += "Green + Purple + Red";
                        break;
                    case Circle.GREENREDCYAN:
                        name += "Green + Red + Cyan";
                        break;
                    case Circle.REDCYANPURPLE:
                        name += "Cyan + Purple + Red";
                        break;
                }
                break;
            case Level.QUATERNARY:
                name += "Quaternary";
                break;

        }
        return name;
    }
}

public enum Level
{
    EXCLUDED,
    SECONDARY,
    TERTIARY,
    QUATERNARY
}

public enum Circle
{
    GREEN,
    PURPLE,
    RED,
    CYAN,
    GREENPURPLE,
    GREENRED,
    CYANRED,
    CYANPURPLE,
    GREENPURPLERED,
    GREENREDCYAN,
    REDCYANPURPLE,
    CYANPURPLEGREEN,
    GREENREDCYANPURPLE
}