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
    private bool _isSolved = false, _lightsOn = false;

    public KMSelectable submit, left, right;

    public GameObject[] sprites;
    public GameObject[] displaySprites;

    private Word[] displayed;

    private int[] badTimes;

    public Material[] colors; //0=orange,1=cyan,2=purple
    public Material off, on;
    private int indicatorColor;

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
        Debug.LogFormat("[DragonEnergy #{0}] Initial position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());
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
                break;
            case 1:
                Debug.LogFormat("[DragonEnergy #{0}] Indicator color: Cyan.", _moduleId);
                break;
            case 2:
                Debug.LogFormat("[DragonEnergy #{0}] Indicator color: Purple.", _moduleId);
                break;
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
                Word[] greenpurple1 = new Word[] { };
                Word[] green1 = new Word[] { };

                Word[] greenred1 = new Word[] { };
                Word[] red1 = new Word[] { };

                Word[] redcyan1 = new Word[] { };
                Word[] cyan1 = new Word[] { };

                Word[] purplecyan1 = new Word[] { };
                Word[] purple1 = new Word[] { };
                foreach (Word word in words)
                {
                    Word[] temp = new Word[] { word };
                    Circle circle = word.getPosition().getCircle();
                    if (circle == Circle.GREENRED)
                    {
                        greenred1.Concat(temp);
                    }
                    else if (circle == Circle.RED)
                    {
                        red1.Concat(temp);
                    }
                    else if (circle == Circle.GREENPURPLE)
                    {
                        greenpurple1.Concat(temp);
                    }
                    else if (circle == Circle.GREEN)
                    {
                        green1.Concat(temp);
                    }
                    else if (circle == Circle.BLUERED)
                    {
                        redcyan1.Concat(temp);
                    }
                    else if (circle == Circle.BLUE)
                    {
                        cyan1.Concat(temp);
                    }
                    else if (circle == Circle.BLUEPURPLE)
                    {
                        purplecyan1.Concat(temp);
                    }
                    else if (circle == Circle.PURPLE)
                    {
                        purple1.Concat(temp);
                    }
                    if (!swapped.Contains(word))
                    {
                        Array.Resize(ref swapped, swapped.Length + 1);
                        swapped[swapped.Length - 1] = word;
                    }
                }

                for (int i = 0; i < greenpurple1.Length; i++)
                {
                    Word.Swap(greenpurple1[i], green1[i]);
                }

                for (int i = 0; i < greenred1.Length; i++)
                {
                    Word.Swap(greenred1[i], red1[i]);
                }

                for (int i = 0; i < redcyan1.Length; i++)
                {
                    Word.Swap(redcyan1[i], cyan1[i]);
                }

                for (int i = 0; i < purplecyan1.Length; i++)
                {
                    Word.Swap(purplecyan1[i], purple1[i]);
                }
                Debug.LogFormat("[DragonEnergy #{0}] Swap 1 has occurred! Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());

                break;
            case 2:
                Word[] greenpurple2 = new Word[] { };
                Word[] redcyan2 = new Word[] { };

                Word[] purplecyan2 = new Word[] { };
                Word[] greenred2 = new Word[] { };

                foreach(Word word in words)
                {
                    Word[] temp = new Word[] { word };
                    Circle circle = word.getPosition().getCircle();
                    if (circle == Circle.GREENRED)
                    {
                        greenred2.Concat(temp);
                    }
                    else if (circle == Circle.GREENPURPLE)
                    {
                        greenpurple2.Concat(temp);
                    }
                    else if (circle == Circle.BLUERED)
                    {
                        redcyan2.Concat(temp);
                    }
                    else if (circle == Circle.BLUEPURPLE)
                    {
                        purplecyan2.Concat(temp);
                    }
                    if (!swapped.Contains(word))
                    {
                        Array.Resize(ref swapped, swapped.Length + 1);
                        swapped[swapped.Length - 1] = word;
                    }
                }

                for (int i = 0; i < greenpurple2.Length; i++)
                {
                    Word.Swap(greenpurple2[i], redcyan2[i]);
                }

                for (int i = 0; i < purplecyan2.Length; i++)
                {
                    Word.Swap(purplecyan2[i], greenred2[i]);
                }
                Debug.LogFormat("[DragonEnergy #{0}] Swap 2 has occurred! Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());

                break;
            case 3:
                Word[] greenpurple3 = new Word[] { };
                Word[] cyan3 = new Word[] { };

                Word[] greenred3 = new Word[] { };
                Word[] purple3 = new Word[] { };

                Word[] redcyan3 = new Word[] { };
                Word[] green3 = new Word[] { };

                Word[] purplecyan3 = new Word[] { };
                Word[] red3 = new Word[] { };
                foreach (Word word in words)
                {
                    Word[] temp = new Word[] { word };
                    Circle circle = word.getPosition().getCircle();
                    if (circle == Circle.GREENRED)
                    {
                        greenred3.Concat(temp);
                    }
                    else if (circle == Circle.RED)
                    {
                        red3.Concat(temp);
                    }
                    else if (circle == Circle.GREENPURPLE)
                    {
                        greenpurple3.Concat(temp);
                    }
                    else if (circle == Circle.GREEN)
                    {
                        green3.Concat(temp);
                    }
                    else if (circle == Circle.BLUERED)
                    {
                        redcyan3.Concat(temp);
                    }
                    else if (circle == Circle.BLUE)
                    {
                        cyan3.Concat(temp);
                    }
                    else if (circle == Circle.BLUEPURPLE)
                    {
                        purplecyan3.Concat(temp);
                    }
                    else if (circle == Circle.PURPLE)
                    {
                        purple3.Concat(temp);
                    }
                    if (!swapped.Contains(word))
                    {
                        Array.Resize(ref swapped, swapped.Length + 1);
                        swapped[swapped.Length - 1] = word;
                    }
                }

                for (int i = 0; i < greenpurple3.Length; i++)
                {
                    Word.Swap(greenpurple3[i], cyan3[i]);
                }

                for (int i = 0; i < greenred3.Length; i++)
                {
                    Word.Swap(greenred3[i], purple3[i]);
                }

                for (int i = 0; i < redcyan3.Length; i++)
                {
                    Word.Swap(redcyan3[i], green3[i]);
                }

                for (int i = 0; i < purplecyan3.Length; i++)
                {
                    Word.Swap(purplecyan3[i], red3[i]);
                }
                Debug.LogFormat("[DragonEnergy #{0}] Swap 3 has occurred! Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());

                break;
            case 4:
                Word[] greenpurplered4 = new Word[] { };
                Word[] greenredcyan4 = new Word[] { };

                Word[] greenpurplecyan4 = new Word[] { };
                Word[] redcyanpurple4 = new Word[] { };

                Word[] quarter4 = new Word[] { };
                Word[] greenred4 = new Word[] { };

                Word[] purple4 = new Word[] { };
                Word[] cyan4 = new Word[] { };
                foreach (Word word in words)
                {
                    Word[] temp = new Word[] { word };
                    Circle circle = word.getPosition().getCircle();
                    if (circle == Circle.GREENRED)
                    {
                        greenred4.Concat(temp);
                    }
                    else if (circle == Circle.GREENPURPLERED)
                    {
                        greenpurplered4.Concat(temp);
                    }
                    else if (circle == Circle.GREENREDBLUE)
                    {
                        greenredcyan4.Concat(temp);
                    }
                    else if (circle == Circle.BLUEPURPLEGREEN)
                    {
                        greenpurplecyan4.Concat(temp);
                    }
                    else if (circle == Circle.REDBLUEPURPLE)
                    {
                        redcyanpurple4.Concat(temp);
                    }
                    else if (circle == Circle.BLUE)
                    {
                        cyan4.Concat(temp);
                    }
                    else if (circle == Circle.GREENREDBLUEPURPLE)
                    {
                        quarter4.Concat(temp);
                    }
                    else if (circle == Circle.PURPLE)
                    {
                        purple4.Concat(temp);
                    }
                    if (!swapped.Contains(word))
                    {
                        Array.Resize(ref swapped, swapped.Length + 1);
                        swapped[swapped.Length - 1] = word;
                    }
                }

                for (int i = 0; i < greenpurplered4.Length; i++)
                {
                    Word.Swap(greenpurplered4[i], greenredcyan4[i]);
                }

                for (int i = 0; i < greenpurplecyan4.Length; i++)
                {
                    Word.Swap(greenpurplecyan4[i], redcyanpurple4[i]);
                }

                if (quarter4.Length > greenred4.Length)
                {
                    for (int i = 0; i < greenred4.Length; i++)
                    {
                        Word.Swap(greenred4[i], quarter4[i]);
                    }
                    quarter4.Last().setPosition(quarter4[0].getPosition());
                }
                else if (quarter4.Length < greenred4.Length)
                {
                    for (int i = 0; i < quarter4.Length; i++)
                    {
                        Word.Swap(greenred4[i], quarter4[i]);
                    }
                    greenred4.Last().setPosition(greenred4[0].getPosition());
                }
                else
                {
                    for (int i = 0; i < quarter4.Length; i++)
                    {
                        Word.Swap(greenred4[i], quarter4[i]);
                    }
                }

                for (int i = 0; i < purple4.Length; i++)
                {
                    Word.Swap(purple4[i], cyan4[i]);
                }
                Debug.LogFormat("[DragonEnergy #{0}] Swap 4 has occurred! Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());

                break;
            case 5:// good
                Word[] green5 = new Word[] { };
                Word[] red5 = new Word[] { };

                Word[] purplecyan5 = new Word[] { };
                Word[] quarter5 = new Word[] { };

                Word[] redcyan5 = new Word[] { };
                Word[] cyan5 = new Word[] { };

                foreach (Word word in words)
                {
                    Circle circle = word.getPosition().getCircle();
                    if (circle == Circle.GREENREDBLUEPURPLE)
                    {
                        Array.Resize(ref quarter5, quarter5.Length + 1);
                        quarter5[quarter5.Length - 1] = word;
                    }
                    else if (circle == Circle.GREEN)
                    {
                        Array.Resize(ref green5, green5.Length + 1);
                        green5[green5.Length - 1] = word;
                    }
                    else if (circle == Circle.BLUERED)
                    {
                        Array.Resize(ref redcyan5, redcyan5.Length + 1);
                        redcyan5[redcyan5.Length - 1] = word;
                    }
                    else if (circle == Circle.BLUEPURPLE)
                    {
                        Array.Resize(ref purplecyan5, purplecyan5.Length + 1);
                        purplecyan5[purplecyan5.Length - 1] = word;
                    }
                    else if (circle == Circle.BLUE)
                    {
                        Array.Resize(ref cyan5, cyan5.Length + 1);
                        cyan5[cyan5.Length - 1] = word;
                    }
                    else if (circle == Circle.RED)
                    {
                        Array.Resize(ref red5, red5.Length + 1);
                        red5[red5.Length - 1] = word;
                    }
                    if (!swapped.Contains(word))
                    {
                        Array.Resize(ref swapped, swapped.Length + 1);
                        swapped[swapped.Length - 1] = word;
                    }
                }

                if (green5.Length > red5.Length)
                {
                    for (int i = 0; i < red5.Length; i++)
                    {
                        Word.Swap(red5[i], green5[i]);
                        Debug.Log("Swapped " + red5[i].getWord() + " and " + green5[i].getWord());
                    }
                    green5.Last().setPosition(green5[0].getPosition());
                    Debug.Log(green5.Last().getWord() + " position set to " + green5.Last().getPosition().getName());
                }
                else if (green5.Length < red5.Length)
                {
                    for (int i = 0; i < green5.Length; i++)
                    {
                        Word.Swap(red5[i], green5[i]);
                        Debug.Log("Swapped " + red5[i].getWord() + " and " + green5[i].getWord());
                    }
                    red5.Last().setPosition(red5[0].getPosition());
                    Debug.Log(red5.Last().getWord() + " position set to " + red5.Last().getPosition().getName());
                }
                else
                {
                    for (int i = 0; i < green5.Length; i++)
                    {
                        Word.Swap(red5[i], green5[i]);
                        Debug.Log("Swapped " + red5[i].getWord() + " and " + green5[i].getWord());
                    }
                }

                if (quarter5.Length > purplecyan5.Length)
                {
                    for (int i = 0; i < purplecyan5.Length; i++)
                    {
                        Word.Swap(purplecyan5[i], quarter5[i]);
                        Debug.Log("Swapped " + purplecyan5[i].getWord() + " and " + quarter5[i].getWord());
                    }
                    quarter5.Last().setPosition(quarter5[0].getPosition());
                    Debug.Log(quarter5.Last().getWord() + " position set to " + quarter5.Last().getPosition().getName());
                }
                else if (quarter5.Length < purplecyan5.Length)
                {
                    for (int i = 0; i < quarter5.Length; i++)
                    {
                        Word.Swap(purplecyan5[i], quarter5[i]);
                        Debug.Log("Swapped " + purplecyan5[i].getWord() + " and " + quarter5[i].getWord());
                    }
                    purplecyan5.Last().setPosition(purplecyan5[0].getPosition());
                    Debug.Log(purplecyan5.Last().getWord() + " position set to " + purplecyan5.Last().getPosition().getName());
                }
                else
                {
                    for (int i = 0; i < quarter5.Length; i++)
                    {
                        Word.Swap(purplecyan5[i], quarter5[i]);
                        Debug.Log("Swapped " + purplecyan5[i].getWord() + " and " + quarter5[i].getWord());
                    }
                }

                if (cyan5.Length > redcyan5.Length)
                {
                    for (int i = 0; i < redcyan5.Length; i++)
                    {
                        Word.Swap(redcyan5[i], cyan5[i]);
                        Debug.Log("Swapped " + redcyan5[i].getWord() + " and " + cyan5[i].getWord());
                    }
                    cyan5.Last().setPosition(cyan5[0].getPosition());
                    Debug.Log(cyan5.Last().getWord() + " position set to " + cyan5.Last().getPosition().getName());
                }
                else if (cyan5.Length < redcyan5.Length)
                {
                    for (int i = 0; i < cyan5.Length; i++)
                    {
                        Word.Swap(redcyan5[i], cyan5[i]);
                        Debug.Log("Swapped " + redcyan5[i].getWord() + " and " + quarter5[i].getWord());
                    }
                    redcyan5.Last().setPosition(redcyan5[0].getPosition());
                    Debug.Log(redcyan5.Last().getWord() + " position set to " + redcyan5.Last().getPosition().getName());
                }
                else
                {
                    for (int i = 0; i < cyan5.Length; i++)
                    {
                        Word.Swap(redcyan5[i], cyan5[i]);
                        Debug.Log("Swapped " + redcyan5[i].getWord() + " and " + cyan5[i].getWord());
                    }
                }
                Debug.LogFormat("[DragonEnergy #{0}] Swap 5 has occurred! Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());

                break;
            case 6:// good
                Word[] quarter6 = new Word[] { };
                Word[] greenpurple6 = new Word[] { };

                foreach (Word word in words)
                {
                    Circle circle = word.getPosition().getCircle();
                    if (circle == Circle.GREENREDBLUEPURPLE)
                    {
                        Array.Resize(ref quarter6, quarter6.Length + 1);
                        quarter6[quarter6.Length - 1] = word;

                    }
                    else if (circle == Circle.GREENPURPLE)
                    {
                        Array.Resize(ref greenpurple6, greenpurple6.Length + 1);
                        greenpurple6[greenpurple6.Length - 1] = word;

                    }
                    if (!swapped.Contains(word))
                    {
                        Array.Resize(ref swapped, swapped.Length + 1);
                        swapped[swapped.Length - 1] = word;
                    }
                }

                Debug.Log(quarter6.Length);
                Debug.Log(greenpurple6.Length);

                if (quarter6.Length > greenpurple6.Length)
                {
                    for (int i = 0; i < greenpurple6.Length; i++)
                    {
                        Word.Swap(greenpurple6[i], quarter6[i]);
                        Debug.Log("Swapped " + greenpurple6[i].getWord() +" and "+quarter6[i].getWord());
                    }
                    quarter6.Last().setPosition(quarter6[0].getPosition());
                    Debug.Log(quarter6.Last().getWord() + " position set to " + quarter6.Last().getPosition().getName());
                }
                else if (quarter6.Length < greenpurple6.Length)
                {
                    for (int i = 0; i < quarter6.Length; i++)
                    {
                        Word.Swap(greenpurple6[i], quarter6[i]);
                        Debug.Log("Swapped " + greenpurple6[i].getWord() + " and " + quarter6[i].getWord());
                    }
                    greenpurple6.Last().setPosition(greenpurple6[0].getPosition());
                    Debug.Log(greenpurple6.Last().getWord() + " position set to " + greenpurple6.Last().getPosition().getName());
                }
                else
                {
                    for (int i = 0; i < quarter6.Length; i++)
                    {
                        Word.Swap(greenpurple6[i], quarter6[i]);
                        Debug.Log("Swapped " + greenpurple6[i].getWord() + " and " + quarter6[i].getWord());
                    }
                }

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
                Debug.LogFormat("[DragonEnergy #{0}] Swap 6 has occurred! Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());

                break;
            case 7: // good

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
                if(last == 0 || last==8 || last == 9 || last == 7) { break; }
                Swaps(last);
                Debug.LogFormat("[DragonEnergy #{0}] Swap 7 has occurred! Current position of words: Ambition: {1}; Anger: {2}; Beauty: {3}; Brave: {4}; Courage: {5}; Crisis: {6}; Death: {7}; Destiny: {8}; Devotion: {9}; DoubleHappiness: {10}; Dragon: {11}; Dream: {12}; Energy: {13}; Eternity: {14}; Female: {15}; Fortune: {16}; Freedom: {17}; GoodLuck: {18}; Happiness: {19}; Hate: {20}; Health: {21}; Honor: {22}; Kind: {23}; Life: {24}; Longevity: {25}; Love: {26}; Male: {27}; Soul: {28}; Wisdom: {29}; Wood: {30}.", _moduleId, Ambition.getPosition().getName(), Anger.getPosition().getName(), Beauty.getPosition().getName(), Brave.getPosition().getName(), Courage.getPosition().getName(), Crisis.getPosition().getName(), Death.getPosition().getName(), Destiny.getPosition().getName(), Devotion.getPosition().getName(), DoubleHappiness.getPosition().getName(), Dragon.getPosition().getName(), Dream.getPosition().getName(), Energy.getPosition().getName(), Eternity.getPosition().getName(), Female.getPosition().getName(), Fortune.getPosition().getName(), Freedom.getPosition().getName(), GoodLuck.getPosition().getName(), Happiness.getPosition().getName(), Hate.getPosition().getName(), Health.getPosition().getName(), Honor.getPosition().getName(), Kind.getPosition().getName(), Life.getPosition().getName(), Longevity.getPosition().getName(), Love.getPosition().getName(), Male.getPosition().getName(), Soul.getPosition().getName(), Wisdom.getPosition().getName(), Wood.getPosition().getName());
                break;
        }
    }

    void getCorrectAnswer(int s)
    {
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
                                    correctPrimaries.Concat(new Circle[] { three });
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
                                    correctWords.Concat(new Word[] { word });
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
                                correctWords.Concat(new Word[] { word });
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
                        if (word.getPosition().getLevel() == Level.QUARTERNARY)
                        {
                            if (!correctWords.Contains(word))
                            {
                                correctWords.Concat(new Word[] { word });
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
                            correctWords.Concat(new Word[] { word });
                        }
                    }
                }
                return;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    void handleSubmit()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submit.transform);
        if (!_lightsOn || _isSolved) return;
        getCorrectAnswer(stage);
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
            setupThreeWords();
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
                    Debug.LogFormat("[DragonEnergy #{0}] Stage 1 solved, word submitted: {1}.", _moduleId, words[currentDisplay].getWord());
                    stage1submit = secondSubmitted;
                    break;
                case 3:
                    stage2.material = on;
                    setupThreeWords();
                    Debug.LogFormat("[DragonEnergy #{0}] Stage 2 solved, word submitted: {1}.", _moduleId, words[currentDisplay].getWord());
                    stage2submit = secondSubmitted;
                    break;
                case 4:
                    stage3.material = on;
                    setupThreeWords();
                    Debug.LogFormat("[DragonEnergy #{0}] Stage 3 solved, word submitted: {1}.", _moduleId, words[currentDisplay].getWord());
                    stage3submit = secondSubmitted;
                    break;
                case 5:
                    stage4.material = on;
                    indicator.material = off;
                    module.HandlePass();
                    Debug.LogFormat("[DragonEnergy #{0}] Module solved!", _moduleId);
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
            Debug.LogFormat("[DragonEnergy #{0}] Incorrect answer submitted. Inputted: {1}. Any of the following are correct: {2}.", _moduleId, incorrect, correct);
            setupThreeWords();
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
        Brave = new Word(sprites[3], new Position(Level.EXCLUDED, Circle.BLUE), "Brave");
        Courage = new Word(sprites[4], new Position(Level.TERTIARY, Circle.GREENPURPLERED), "Courage");
        Crisis = new Word(sprites[5], new Position(Level.TERTIARY, Circle.GREENREDBLUE), "Crisis");
        Death = new Word(sprites[6], new Position(Level.TERTIARY, Circle.REDBLUEPURPLE), "Death");
        Destiny = new Word(sprites[7], new Position(Level.SECONDARY, Circle.BLUERED), "Destiny");
        Devotion = new Word(sprites[8], new Position(Level.SECONDARY, Circle.GREENRED), "Devotion");
        DoubleHappiness = new Word(sprites[9], new Position(Level.SECONDARY, Circle.BLUEPURPLE), "Double Happiness");
        Dragon = new Word(sprites[10], new Position(Level.QUARTERNARY, Circle.GREENREDBLUEPURPLE), "Dragon");
        Dream = new Word(sprites[11], new Position(Level.EXCLUDED, Circle.PURPLE), "Dream");
        Energy = new Word(sprites[12], new Position(Level.SECONDARY, Circle.GREENPURPLE), "Energy");
        Eternity = new Word(sprites[13], new Position(Level.SECONDARY, Circle.GREENPURPLE), "Eternity");
        Female = new Word(sprites[14], new Position(Level.EXCLUDED, Circle.GREEN), "Female");
        Fortune = new Word(sprites[15], new Position(Level.TERTIARY, Circle.BLUEPURPLEGREEN), "Fortune");
        Freedom = new Word(sprites[16], new Position(Level.SECONDARY, Circle.BLUEPURPLE), "Freedom");
        GoodLuck = new Word(sprites[17], new Position(Level.SECONDARY, Circle.BLUEPURPLE), "Good Luck");
        Happiness = new Word(sprites[18], new Position(Level.SECONDARY, Circle.GREENRED), "Happiness");
        Hate = new Word(sprites[19], new Position(Level.SECONDARY, Circle.BLUERED), "Hate");
        Health = new Word(sprites[20], new Position(Level.QUARTERNARY, Circle.GREENREDBLUEPURPLE), "Health");
        Honor = new Word(sprites[21], new Position(Level.SECONDARY, Circle.GREENRED), "Honor");
        Kind = new Word(sprites[22], new Position(Level.EXCLUDED, Circle.RED), "Kind");
        Life = new Word(sprites[23], new Position(Level.SECONDARY, Circle.GREENPURPLE), "Life");
        Longevity = new Word(sprites[24], new Position(Level.SECONDARY, Circle.BLUERED), "Longevity");
        Love = new Word(sprites[25], new Position(Level.EXCLUDED, Circle.BLUE), "Love");
        Male = new Word(sprites[26], new Position(Level.EXCLUDED, Circle.BLUE), "Male");
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
            case Circle.GREENREDBLUEPURPLE:
                return new Circle[] { Circle.GREEN, Circle.RED, Circle.PURPLE, Circle.BLUE };
            case Circle.RED:
                return new Circle[] { Circle.RED };
            case Circle.BLUERED:
                return new Circle[] { Circle.RED, Circle.BLUE };
            case Circle.REDBLUEPURPLE:
                return new Circle[] { Circle.RED, Circle.BLUE, Circle.PURPLE };
            case Circle.GREENREDBLUE:
                return new Circle[] { Circle.GREEN, Circle.RED, Circle.BLUE };
            case Circle.BLUE:
                return new Circle[] { Circle.BLUE };
            case Circle.PURPLE:
                return new Circle[] { Circle.PURPLE };
            case Circle.BLUEPURPLE:
                return new Circle[] { Circle.BLUE, Circle.PURPLE };
            case Circle.BLUEPURPLEGREEN:
                return new Circle[] { Circle.GREEN, Circle.BLUE, Circle.PURPLE };
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
}

public class Word
{
    private GameObject sprite;
    private Position position;
    private String name;

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
                    case Circle.BLUE:
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
                    case Circle.BLUERED:
                        name += "Cyan + Red";
                        break;
                    case Circle.GREENRED:
                        name += "Green + Red";
                        break;
                    case Circle.BLUEPURPLE:
                        name += "Cyan + Purple";
                        break;
                }
                break;
            case Level.TERTIARY:
                name += "Tertiary, ";
                switch (circle)
                {
                    case Circle.BLUEPURPLEGREEN:
                        name += "Green + Purple + Cyan";
                        break;
                    case Circle.GREENPURPLERED:
                        name += "Green + Purple + Red";
                        break;
                    case Circle.GREENREDBLUE:
                        name += "Green + Red + Cyan";
                        break;
                    case Circle.REDBLUEPURPLE:
                        name += "Cyan + Purple + Red";
                        break;
                }
                break;
            case Level.QUARTERNARY:
                name += "Quarternary, Green + Red + Cyan + Purple";
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
    QUARTERNARY
}

public enum Circle
{
    GREEN,
    PURPLE,
    RED,
    BLUE,
    GREENPURPLE,
    GREENRED,
    BLUERED,
    BLUEPURPLE,
    GREENPURPLERED,
    GREENREDBLUE,
    REDBLUEPURPLE,
    BLUEPURPLEGREEN,
    GREENREDBLUEPURPLE
}