using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KMHelper;
using UnityEngine;
using Random = UnityEngine.Random;

public class dragonEnergy : MonoBehaviour
{
    private static int _moduleIdCounter = 1;
    public KMAudio newAudio;
    public KMBombModule module;
    public KMBombInfo info;
    private int _moduleId = 0;
    private bool _isSolved = false;

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
    public MeshRenderer indicator;

    private Word Angry, Blessing, Child, Curse, Heaven, Happiness, Dragon, Dream, Energy, Female, Force, Forest, Friend, Hate, Hope, Kindness, Longevity, Love, Loyal, Spirit, Male, Mountain, Night, Pure, Heart, River, Emotion, Soul, Urgency, Wind;
    private Word[] words;
    private int currentDisplay;
    private bool dependsOnSolvedModules;

    private HashSet<Word> correctWords = new HashSet<Word>();

    private List<string> modules;

    void Start()
    {
        _moduleId = _moduleIdCounter++;
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

        Angry = new Word(sprites[0], new Position(Level.Excluded, Circle.Green));
        Blessing = new Word(sprites[1], new Position(Level.Excluded, Circle.Purple));
        Child = new Word(sprites[2], new Position(Level.Excluded, Circle.Red));
        Curse = new Word(sprites[3], new Position(Level.Excluded, Circle.Cyan));
        Heaven = new Word(sprites[4], new Position(Level.Tertiary, Circle.GreenPurpleRed));
        Happiness = new Word(sprites[5], new Position(Level.Tertiary, Circle.GreenRedCyan));
        Dragon = new Word(sprites[6], new Position(Level.Tertiary, Circle.RedCyanPurple));
        Dream = new Word(sprites[7], new Position(Level.Secondary, Circle.CyanRed));
        Energy = new Word(sprites[8], new Position(Level.Secondary, Circle.GreenRed));
        Female = new Word(sprites[9], new Position(Level.Secondary, Circle.CyanPurple));
        Force = new Word(sprites[10], new Position(Level.Quaternary, Circle.GreenRedCyanPurple));
        Forest = new Word(sprites[11], new Position(Level.Excluded, Circle.Purple));
        Friend = new Word(sprites[12], new Position(Level.Secondary, Circle.GreenPurple));
        Hate = new Word(sprites[13], new Position(Level.Secondary, Circle.GreenPurple));
        Hope = new Word(sprites[14], new Position(Level.Excluded, Circle.Green));
        Kindness = new Word(sprites[15], new Position(Level.Tertiary, Circle.CyanPurpleGreen));
        Longevity = new Word(sprites[16], new Position(Level.Secondary, Circle.CyanPurple));
        Love = new Word(sprites[17], new Position(Level.Secondary, Circle.CyanPurple));
        Loyal = new Word(sprites[18], new Position(Level.Secondary, Circle.GreenRed));
        Spirit = new Word(sprites[19], new Position(Level.Secondary, Circle.CyanRed));
        Male = new Word(sprites[20], new Position(Level.Quaternary, Circle.GreenRedCyanPurple));
        Mountain = new Word(sprites[21], new Position(Level.Secondary, Circle.GreenRed));
        Night = new Word(sprites[22], new Position(Level.Excluded, Circle.Red));
        Pure = new Word(sprites[23], new Position(Level.Secondary, Circle.GreenPurple));
        Heart = new Word(sprites[24], new Position(Level.Secondary, Circle.CyanRed));
        River = new Word(sprites[25], new Position(Level.Excluded, Circle.Cyan));
        Emotion = new Word(sprites[26], new Position(Level.Excluded, Circle.Cyan));
        Soul = new Word(sprites[27], new Position(Level.Excluded, Circle.Purple));
        Urgency = new Word(sprites[28], new Position(Level.Excluded, Circle.Red));
        Wind = new Word(sprites[29], new Position(Level.Excluded, Circle.Green));

        words = new Word[] { Angry, Blessing, Child, Curse, Heaven, Happiness, Dragon, Dream, Energy, Female, Force, Forest, Friend, Hate, Hope, Kindness, Longevity, Love, Loyal, Spirit, Male, Mountain, Night, Pure, Heart, River, Emotion, Soul, Urgency, Wind };

        Init();
    }

    void Init()
    {
        indicator.material = off;
        modules = info.GetModuleNames();
        setup();
    }

    private void Update()
    {
        if (dependsOnSolvedModules && info.GetSolvedModuleNames().Count > 0)
        {
            Debug.LogFormat("[Dragon Energy #{0}] You solved another module! Calculating new solution.", _moduleId);
            dependsOnSolvedModules = false;
            getCorrectAnswer();
        }
    }

    private void setup()
    {
        currentDisplay = Random.Range(0, 30);
        setupIndicator();
        getBadTimes();
        setupThreeWords();
        DisplayCurrent();
        getCorrectAnswer();
    }

    void setupIndicator()
    {
        indicatorColor = Random.Range(0, 3);
        indicator.material = colors[indicatorColor];
        var colorname = new[] { "Orange", "Cyan", "Purple" }[indicatorColor];
        Debug.LogFormat("[Dragon Energy #{0}] Indicator color: {1}.", _moduleId, colorname);
        colorblindText.text = colorname;
        if (GetComponent<KMColorblindMode>().ColorblindModeActive)
            colorblindObj.SetActive(true);
    }

    void InitSwaps()
    {
        Angry.setPosition(new Position(Level.Excluded, Circle.Green));
        Blessing.setPosition(new Position(Level.Excluded, Circle.Purple));
        Child.setPosition(new Position(Level.Excluded, Circle.Red));
        Curse.setPosition(new Position(Level.Excluded, Circle.Cyan));
        Heaven.setPosition(new Position(Level.Tertiary, Circle.GreenPurpleRed));
        Happiness.setPosition(new Position(Level.Tertiary, Circle.GreenRedCyan));
        Dragon.setPosition(new Position(Level.Tertiary, Circle.RedCyanPurple));
        Dream.setPosition(new Position(Level.Secondary, Circle.CyanRed));
        Energy.setPosition(new Position(Level.Secondary, Circle.GreenRed));
        Female.setPosition(new Position(Level.Secondary, Circle.CyanPurple));
        Force.setPosition(new Position(Level.Quaternary, Circle.GreenRedCyanPurple));
        Forest.setPosition(new Position(Level.Excluded, Circle.Purple));
        Friend.setPosition(new Position(Level.Secondary, Circle.GreenPurple));
        Hate.setPosition(new Position(Level.Secondary, Circle.GreenPurple));
        Hope.setPosition(new Position(Level.Excluded, Circle.Green));
        Kindness.setPosition(new Position(Level.Tertiary, Circle.CyanPurpleGreen));
        Longevity.setPosition(new Position(Level.Secondary, Circle.CyanPurple));
        Love.setPosition(new Position(Level.Secondary, Circle.CyanPurple));
        Loyal.setPosition(new Position(Level.Secondary, Circle.GreenRed));
        Spirit.setPosition(new Position(Level.Secondary, Circle.CyanRed));
        Male.setPosition(new Position(Level.Quaternary, Circle.GreenRedCyanPurple));
        Mountain.setPosition(new Position(Level.Secondary, Circle.GreenRed));
        Night.setPosition(new Position(Level.Excluded, Circle.Red));
        Pure.setPosition(new Position(Level.Secondary, Circle.GreenPurple));
        Heart.setPosition(new Position(Level.Secondary, Circle.CyanRed));
        River.setPosition(new Position(Level.Excluded, Circle.Cyan));
        Emotion.setPosition(new Position(Level.Excluded, Circle.Cyan));
        Soul.setPosition(new Position(Level.Excluded, Circle.Purple));
        Urgency.setPosition(new Position(Level.Excluded, Circle.Red));
        Wind.setPosition(new Position(Level.Excluded, Circle.Green));

        Debug.LogFormat("[Dragon Energy #{0}] Before swapping, the displayed words are:", _moduleId);
        for (int i = 0; i < displayed.Length; i++)
            Debug.LogFormat("[Dragon Energy #{0}] {1} = {2}", _moduleId, displayed[i].getWord(), displayed[i].getPosition().getCircle().ToReadable());

        char[] letters = info.GetSerialNumberLetters().ToArray();
        int vowelCount = 0;
        foreach (char letter in letters)
        {
            if (letter == 'A' || letter == 'E' || letter == 'I' || letter == 'O' || letter == 'U')
            {
                vowelCount++;
            }
        }

        if (info.GetBatteryCount() > 10 && (info.GetSerialNumberNumbers().ToArray()[info.GetSerialNumberNumbers().ToArray().Length - 1] == 5 || info.GetSerialNumberNumbers().ToArray()[info.GetSerialNumberNumbers().ToArray().Length - 1] == 7))
        {
            Swaps(1);
        }
        else if (info.GetPortPlateCount() > info.GetBatteryHolderCount() && (modules.Contains("Morse War") || modules.Contains("Double Color")))
        {
            Swaps(2);
        }
        else if ((info.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.SIG) && info.IsIndicatorOn(KMBombInfoExtensions.KnownIndicatorLabel.FRK)) || (info.GetOffIndicators().Count() == 3))
        {
            Swaps(3);
        }
        else if (info.GetModuleNames().Count() > 8)
        {
            Swaps(4);
        }
        else if (vowelCount >= 2)
        {
            Swaps(5);
        }
        else if (info.GetSolvedModuleNames().Count() == 0)
        {
            Swaps(6);
            dependsOnSolvedModules = true;
        }
        else
        {
            Swaps(7);
        }

        Debug.LogFormat("[Dragon Energy #{0}] After swapping, the displayed words are:", _moduleId);
        for (int i = 0; i < displayed.Length; i++)
            Debug.LogFormat("[Dragon Energy #{0}] {1} = {2}", _moduleId, displayed[i].getWord(), displayed[i].getPosition().getCircle().ToReadable());
    }

    void Swaps(int swap)
    {
        switch (swap)
        {
            case 1:
                SwapSet(Circle.GreenPurple, Circle.Green);
                SwapSet(Circle.GreenRed, Circle.Red);
                SwapSet(Circle.CyanRed, Circle.Cyan);
                SwapSet(Circle.Purple, Circle.CyanPurple);
                Debug.LogFormat("[Dragon Energy #{0}] Swap 1 has occurred: GP ↔ G, GR ↔ R, CR ↔ C, P ↔ CP", _moduleId);
                break;
            case 2:
                SwapSet(Circle.CyanRed, Circle.GreenPurple);
                SwapSet(Circle.GreenRed, Circle.CyanPurple);
                Debug.LogFormat("[Dragon Energy #{0}] Swap 2 has occurred: CR ↔ GP, GR ↔ CP", _moduleId);
                break;
            case 3:
                SwapSet(Circle.GreenPurple, Circle.Cyan);
                SwapSet(Circle.GreenRed, Circle.Purple);
                SwapSet(Circle.CyanRed, Circle.Green);
                SwapSet(Circle.Red, Circle.CyanPurple);
                Debug.LogFormat("[Dragon Energy #{0}] Swap 3 has occurred: GP ↔ C, GR ↔ P, CR ↔ G, R ↔ CP", _moduleId);
                break;
            case 4:
                SwapSet(Circle.GreenPurpleRed, Circle.GreenRedCyan);
                SwapSet(Circle.CyanPurpleGreen, Circle.RedCyanPurple);
                SwapSet(Circle.GreenRed, Circle.GreenRedCyanPurple);
                SwapSet(Circle.Purple, Circle.Cyan);
                Debug.LogFormat("[Dragon Energy #{0}] Swap 4 has occurred: GPR ↔ GRC, CPG ↔ RCP, P ↔ C", _moduleId);
                break;
            case 5:
                SwapSet(Circle.Green, Circle.Red);
                SwapSet(Circle.CyanPurple, Circle.GreenRedCyanPurple);
                SwapSet(Circle.Cyan, Circle.CyanRed);
                Debug.LogFormat("[Dragon Energy #{0}] Swap 5 has occurred: G ↔ R, CP ↔ GRCP, C ↔ CR", _moduleId);
                break;
            case 6:
                SwapSet(Circle.GreenPurple, Circle.GreenRedCyanPurple);
                SwapWords(Urgency, River);
                Debug.LogFormat("[Dragon Energy #{0}] Swap 6 has occurred: GP ↔ GRCP, Urgency ↔ River", _moduleId);
                break;
            case 7:
                SwapWords(Wind, Forest);
                SwapWords(Heaven, Spirit);
                SwapWords(Longevity, Mountain);
                SwapWords(Hope, Force);
                Debug.LogFormat("[Dragon Energy #{0}] Swap 7 has occurred: Wind ↔ Forest, Heaven ↔ Magic, Longevity ↔ Mountain, Hope ↔ Force", _moduleId);
                int last = info.GetSerialNumberNumbers().Last();
                if (last != 0 && last != 7 && last != 8 && last != 9)
                    Swaps(last);
                break;
        }
    }

    void SwapWords(Word first, Word second)
    {
        Position temp = first.getPosition();
        first.setPosition(second.getPosition());
        second.setPosition(temp);
        first.wordSwapped();
        second.wordSwapped();
    }

    void getCorrectAnswer()
    {
        correctWords.Clear();
        InitSwaps();

        // If all three words on the module are in the same primary section, submit a word from the same primary section.
        var correctPrimaries = new HashSet<Circle>();
        foreach (Circle one in getPrimaries(displayed[0]))
            foreach (Circle two in getPrimaries(displayed[1]))
                if (one == two)
                    foreach (Circle three in getPrimaries(displayed[2]))
                        if (one == three)
                            correctPrimaries.Add(three);
        if (correctPrimaries.Count > 0)
        {
            Debug.LogFormat(@"[Dragon Energy #{0}] All three words are in the same primary section ({1}) → submit a word from the same primary section.", _moduleId, string.Join(", ", correctPrimaries.Select(c => c.ToString()).ToArray()));
            foreach (Circle primary in correctPrimaries)
                foreach (Word word in words)
                    if (getPrimaries(word).Contains(primary))
                        correctWords.Add(word);
        }
        else
        {
            // Otherwise, if two words are in the same secondary section, submit a word from any secondary section.
            Circle? secondary = null;
            for (int a = 0; a < displayed.Length; a++)
                if (displayed[a].getPosition().getLevel() == Level.Secondary)
                    for (int b = a + 1; b < displayed.Length; b++)
                        if (displayed[b].getPosition().getLevel() == Level.Secondary && displayed[a].getPosition().getCircle() == displayed[b].getPosition().getCircle())
                            secondary = displayed[a].getPosition().getCircle();

            if (secondary != null)
            {
                Debug.LogFormat(@"[Dragon Energy #{0}] Two words are in the same secondary section ({1}) → submit a word from any secondary section.", _moduleId, secondary);
                foreach (Word word in words)
                    if (word.getPosition().getLevel() == Level.Secondary)
                        correctWords.Add(word);
            }
            else
            {
                // Otherwise, if no words share a primary, submit a word from a quaternary section.
                bool noSharePrimary = true;
                for (int a = 0; a < displayed.Length; a++)
                    for (int b = a + 1; b < displayed.Length; b++)
                        if (getPrimaries(displayed[a]).Intersect(getPrimaries(displayed[b])).Any())
                            noSharePrimary = false;

                if (noSharePrimary)
                {
                    Debug.LogFormat(@"[Dragon Energy #{0}] No words share a primary → submit a word from a quaternary section.", _moduleId);
                    foreach (Word word in words)
                        if (word.getPosition().getLevel() == Level.Quaternary)
                            correctWords.Add(word);
                }
                else
                {
                    // Otherwise, submit a word from a tertiary section.
                    Debug.LogFormat(@"[Dragon Energy #{0}] Otherwise rule → submit a word from a tertiary section.", _moduleId);
                    foreach (Word word in words)
                        if (word.getPosition().getLevel() == Level.Tertiary)
                            correctWords.Add(word);
                }
            }
        }

        Debug.LogFormat("[Dragon Energy #{0}] Any of the following would be a correct answer: {1}.", _moduleId, string.Join(", ", correctWords.Select(w => w.getWord()).ToArray()));
    }

    void handleSubmit()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submit.transform);
        if (_isSolved) return;

        var time = (int) (info.GetTime() % 10);
        getBadTimes();
        Debug.LogFormat("[Dragon Energy #{0}] You submitted {1} (which is in {2}) when the last digit of the timer was {3}.", _moduleId, words[currentDisplay].getWord(), words[currentDisplay].getPosition(), time);

        if (badTimes.Contains(time))
        {
            module.HandleStrike();
            Debug.LogFormat("[Dragon Energy #{0}] That last digit is forbidden. Strike.", _moduleId);
            Debug.LogFormat("[Dragon Energy #{0}] If you feel that this is a bug, please do not hesitate to contact @AAces#0908 on discord so we can get this sorted out. Be sure to send along a copy of this logfile.", _moduleId);
            reset();
        }
        else if (!correctWords.Contains(words[currentDisplay]))
        {
            module.HandleStrike();
            Debug.LogFormat("[Dragon Energy #{0}] Incorrect answer submitted.", _moduleId);
            Debug.LogFormat("[Dragon Energy #{0}] If you feel that this is a bug, please do not hesitate to contact @AAces#0908 on Discord so we can get this sorted out. Be sure to send along a copy of this logfile.", _moduleId);
            reset();
        }
        else
        {
            indicator.material = off;
            colorblindObj.SetActive(false);
            module.HandlePass();
            Debug.LogFormat("[Dragon Energy #{0}] Module solved!", _moduleId);
            _isSolved = true;
        }
    }

    void getBadTimes()
    {
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
        Debug.LogFormat("[Dragon Energy #{0}] You have {1} strikes. Forbidden digits for submitting are: {2}.", _moduleId, info.GetStrikes(), string.Join(", ", badTimes.Select(t => t.ToString()).ToArray()));
    }

    void setupThreeWords()
    {
        foreach (Word word in words)
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
    }

    void handleLeft()
    {
        newAudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, left.transform);
        if (_isSolved) return;
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
        if (_isSolved) return;

        currentDisplay++;
        if (currentDisplay == 30)
        {
            currentDisplay = 0;
        }
        DisplayCurrent();
    }

    Circle[] getPrimaries(Word word)
    {
        switch (word.getPosition().getCircle())
        {
            case Circle.Green:
                return new Circle[] { Circle.Green };
            case Circle.GreenRed:
                return new Circle[] { Circle.Green, Circle.Red };
            case Circle.GreenPurpleRed:
                return new Circle[] { Circle.Green, Circle.Red, Circle.Purple };
            case Circle.GreenRedCyanPurple:
                return new Circle[] { Circle.Green, Circle.Red, Circle.Purple, Circle.Cyan };
            case Circle.Red:
                return new Circle[] { Circle.Red };
            case Circle.CyanRed:
                return new Circle[] { Circle.Red, Circle.Cyan };
            case Circle.RedCyanPurple:
                return new Circle[] { Circle.Red, Circle.Cyan, Circle.Purple };
            case Circle.GreenRedCyan:
                return new Circle[] { Circle.Green, Circle.Red, Circle.Cyan };
            case Circle.Cyan:
                return new Circle[] { Circle.Cyan };
            case Circle.Purple:
                return new Circle[] { Circle.Purple };
            case Circle.CyanPurple:
                return new Circle[] { Circle.Cyan, Circle.Purple };
            case Circle.CyanPurpleGreen:
                return new Circle[] { Circle.Green, Circle.Cyan, Circle.Purple };
            case Circle.GreenPurple:
                return new Circle[] { Circle.Green, Circle.Purple };
        }
        return null;
    }

    void DisplayCurrent()
    {
        for (int i = 0; i < 30; i++)
            displaySprites[i].SetActive(i == currentDisplay);
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
        }

        if (setOne.Length > setTwo.Length)
        {
            for (int i = 0; i < setTwo.Length; i++)
                SwapWords(setTwo[i], setOne[i]);
            setOne.Last().setPosition(setOne[0].getPosition());
            setOne.Last().wordSwapped();
        }
        else if (setOne.Length < setTwo.Length)
        {
            for (int i = 0; i < setOne.Length; i++)
                SwapWords(setTwo[i], setOne[i]);
            setTwo.Last().setPosition(setTwo[0].getPosition());
            setTwo.Last().wordSwapped();
        }
        else
        {
            for (int i = 0; i < setOne.Length; i++)
                SwapWords(setTwo[i], setOne[i]);
        }
    }

    void reset()
    {
        foreach (Word word in words)
            word.resetSwapCount();

        Debug.LogFormat("[Dragon Energy #{0}] Module reset.", _moduleId);

        setup();
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = "!{0} energy 5 [submit word “energy” when last digit of countdown timer is 5] | !{0} colorblind";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string input)
    {
        if (input.ToLowerInvariant().Equals("colorblind"))
        {
            yield return null;
            Debug.LogFormat("[Dragon Energy #{0}] Colorblind mode enabled via TP.", _moduleId);
            colorblindObj.SetActive(true);
            yield break;
        }

        var inputtedArray = input.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        if (inputtedArray.Length != 2)
            yield break;

        if (inputtedArray[1].Length != 1 || !"0123456789".Contains(inputtedArray[1]))
        {
            yield return string.Format("sendtochaterror “{0}” is not a valid digit.", inputtedArray[1]);
            yield break;
        }

        yield return null;
        int time = int.Parse(inputtedArray[1]);
        for (int i = 0; i < words.Length && words[currentDisplay].getWord().ToLowerInvariant() != inputtedArray[0]; i++)
        {
            left.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        if (words[currentDisplay].getWord().ToLowerInvariant() != inputtedArray[0])
        {
            yield return string.Format("sendtochaterror “{0}” is not a valid word.", inputtedArray[0]);
            yield break;
        }
        while (time != (int) info.GetTime() % 10)
            yield return "trycancel Submit wasn’t pressed due to request to cancel.";
        submit.OnInteract();
    }
}

class Word
{
    private GameObject sprite;
    private Position position;
    private int swapCount = 0;

    const float x1 = 0.0453f;
    const float y1 = 0.042f;
    const float z1 = -0.0114f;
    const float x2 = 0.0453f;
    const float y2 = 0.042f;
    const float z2 = -0.0639f;
    const float x3 = -0.0072f;
    const float y3 = 0.042f;
    const float z3 = -0.0639f;

    public Word(GameObject sprite, Position pos)
    {
        this.sprite = sprite;
        position = pos;
    }

    public void wordSwapped()
    {
        swapCount++;
    }

    public void display(int location)
    {
        switch (location)
        {
            case 1:
                sprite.transform.localPosition = new Vector3(x1, y1, z1);
                break;
            case 2:
                sprite.transform.localPosition = new Vector3(x2, y2, z2);
                break;
            case 3:
                sprite.transform.localPosition = new Vector3(x3, y3, z3);
                break;
        }
        sprite.SetActive(true);
    }

    public Position getPosition()
    {
        return position;
    }

    public GameObject getSprite()
    {
        return sprite;
    }

    public string getWord()
    {
        return sprite.name;
    }

    public int getSwapCount()
    {
        return swapCount;
    }

    public void setPosition(Position pos)
    {
        position = pos;
    }

    public void resetSwapCount()
    {
        swapCount = 0;
    }
}

class Position
{
    readonly Level level;
    readonly Circle circle;

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

    public override string ToString()
    {
        return string.Format("{0} — {1}", level, circle.ToReadable());
    }
}

enum Level
{
    Excluded,
    Secondary,
    Tertiary,
    Quaternary
}

enum Circle
{
    Green,
    Purple,
    Red,
    Cyan,
    GreenPurple,
    GreenRed,
    CyanRed,
    CyanPurple,
    GreenPurpleRed,
    GreenRedCyan,
    RedCyanPurple,
    CyanPurpleGreen,
    GreenRedCyanPurple
}

static class CircleUtils
{
    public static string ToReadable(this Circle circle)
    {
        switch (circle)
        {
            case Circle.Red: return "Red";
            case Circle.Green: return "Green";
            case Circle.Cyan: return "Cyan";
            case Circle.Purple: return "Purple";
            case Circle.GreenPurple: return "Green + Purple";
            case Circle.CyanRed: return "Cyan + Red";
            case Circle.GreenRed: return "Green + Red";
            case Circle.CyanPurple: return "Cyan + Purple";
            case Circle.CyanPurpleGreen: return "Cyan + Purple + Green";
            case Circle.GreenPurpleRed: return "Green + Purple + Red";
            case Circle.GreenRedCyan: return "Green + Red + Cyan";
            case Circle.RedCyanPurple: return "Red + Cyan + Purple";
            case Circle.GreenRedCyanPurple: return "Green + Red + Cyan + Purple";
        }
        return null;
    }
}
