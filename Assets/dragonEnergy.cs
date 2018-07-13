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
    private int currentDisplay, stage = 1;

    private Word correctWord;

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
        setupWords();
        words = new Word[] { Ambition, Anger, Beauty, Brave, Courage, Crisis, Death, Destiny, Devotion, DoubleHappiness, Dragon, Dream, Energy, Eternity, Female, Fortune, Freedom, GoodLuck, Happiness, Hate, Health, Honor, Kind, Life, Longevity, Love, Male, Soul, Wisdom, Wood };
        stage1.material = off;
        stage2.material = off;
        stage3.material = off;
        stage4.material = off;
        indicator.material = off;
        indicatorColor = Random.Range(0, 3);
        indicator.material = colors[indicatorColor];
        currentDisplay = Random.Range(0, 30);
        setupThreeWords();
        DisplayCurrent();
        modules = info.GetModuleNames();
        Debug.LogFormat("[DragonEnergy #{0}] Note: Answer is not calculated until submit is pressed.", _moduleId);
    }

    void InitSwaps()
    {
        if(info.GetBatteryCount() > 10 && (info.GetSerialNumberNumbers().ToArray()[info.GetSerialNumberNumbers().ToArray().Length-1] == 5 || info.GetSerialNumberNumbers().ToArray()[info.GetSerialNumberNumbers().ToArray().Length - 1] == 7))
        {
            Swaps(1);
        }
        else if (info.GetPortPlateCount()>info.GetBatteryHolderCount() && (modules.Contains("Morse War") || modules.Contains("Double Color")))
        {
            Swaps(2);
        }
    }

    void Swaps(int swap)
    {
        switch (swap){
            case 1:
                Word[] greenpurple = new Word[] { };
                Word[] green = new Word[] { };

                Word[] greenred = new Word[] { };
                Word[] red = new Word[] { };

                Word[] redcyan = new Word[] { };
                Word[] cyan = new Word[] { };

                Word[] purplecyan = new Word[] { };
                Word[] purple = new Word[] { };
                foreach (Word word in words)
                {
                    Word[] temp = new Word[] { word };
                    Circle circle = word.getPosition().getCircle();
                    if (circle == Circle.GREENRED)
                    {
                        greenred.Concat(temp);
                    }
                    else if (circle == Circle.RED)
                    {
                        red.Concat(temp);
                    }
                    else if (circle == Circle.GREENPURPLE)
                    {
                        greenpurple.Concat(temp);
                    }
                    else if (circle == Circle.GREEN)
                    {
                        green.Concat(temp);
                    }
                    else if (circle == Circle.BLUERED)
                    {
                        redcyan.Concat(temp);
                    }
                    else if (circle == Circle.BLUE)
                    {
                        cyan.Concat(temp);
                    }
                    else if (circle == Circle.BLUEPURPLE)
                    {
                        purplecyan.Concat(temp);
                    }
                    else if (circle == Circle.PURPLE)
                    {
                        purple.Concat(temp);
                    }

                }
                for (int i = 0; i < greenpurple.Length; i++)
                {
                    Word.Swap(greenpurple[i], green[i]);
                }

                for (int i = 0; i < greenred.Length; i++)
                {
                    Word.Swap(greenred[i], red[i]);
                }

                for (int i = 0; i < redcyan.Length; i++)
                {
                    Word.Swap(redcyan[i], cyan[i]);
                }

                for (int i = 0; i < purplecyan.Length; i++)
                {
                    Word.Swap(purplecyan[i], purple[i]);
                }
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
        }
    }

    void getCorrectAnswer(int stage)
    {
        switch (stage)
        {
            case 1:
                InitSwaps();
                break;
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
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
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
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
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
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
                            break;
                    }
                    break;
                case 1:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
                            break;
                    }
                    break;
                case 2:
                    switch (info.GetStrikes())
                    {
                        case 0:
                            badTimes = new int[] { };
                            break;
                        case 1:
                            badTimes = new int[] { };
                            break;
                        default:
                            badTimes = new int[] { };
                            break;
                    }
                    break;
            }
        }
        if(badTimes.Contains((int)(info.GetTime() % 60) % 10))
        {
            module.HandleStrike();
            Debug.LogFormat("[DragonEnergy #{0}] Submit pressed with {1} in last digit of timer.", _moduleId, ((int)(info.GetTime() % 60) % 10));
            setupThreeWords();
        }
        else if(words[currentDisplay].getWord() == correctWord.getWord())
        {
            stage++;
            switch (stage)
            {
                case 2:
                    stage1.material = on;
                    setupThreeWords();
                    break;
                case 3:
                    stage2.material = on;
                    setupThreeWords();
                    break;
                case 4:
                    stage3.material = on;
                    setupThreeWords();
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
            Debug.LogFormat("[DragonEnergy #{0}] Incorrect input. Recieved: {1}. Expected: {2}.", _moduleId, words[currentDisplay].getWord(), correctWord.getWord());
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

    Circle[] getPrimarys(Word word)
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