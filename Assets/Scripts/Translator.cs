using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

/// <summary>
///	Handles all translation in game
/// Handles change of language on start screen
/// </summary>
public class Translator : MonoBehaviour
{
    // UI Elements
    public TMP_Dropdown dropdown;

    [Header("Building")]
    public TMP_Text buildTitle;

    [Header("Death Overlay")]
    public TMP_Text deathText;
    public TMP_Text restartButtonText;

    [Header("Loading Overlay")]
    public TMP_Text loadingTitle;
    public TMP_Text descriptionText;
    public TMP_Text helpButton;

    [Header("How To Play")]
    public TMP_Text helpTitle;
    public TMP_Text helpText;

    [Header("Others")]
    public TMP_Text pausedText;

    // Languages
    private readonly List<SystemLanguage> languages = new List<SystemLanguage>(){
        SystemLanguage.English,
        SystemLanguage.German};

    private readonly string[][] languageStringsTranslation = new string[][]
    {
        new string[]{ "English", "German" },
        new string[]{ "Englisch", "Deutsch"}
    };

    private readonly CultureInfo[] cultures = new CultureInfo[]
    {
        CultureInfo.CreateSpecificCulture("en-US"),
        CultureInfo.CreateSpecificCulture("de-DE")
    };

    // Translations
    // 0: English -> default
    // 1: German
    private readonly string[][] text = new string[][]{
        new string[]{
            "- Build -",
            "- You died -",
            "Retry",
            "- Loading -",
            "You are stranded alone in the middle of an unknown ice desert.\n\nTry to survive for as long as possible.\nBuild houses to keep you warm and unlock additional buildings.\n\nBut watch out, maybe you are not as alone as you thought...",
            "How to play",
            "- How to play -",
            "Walk by tapping on the ground\n\nChop down trees by tapping on a tree\n\nBuild/Repair Buildings by tapping on them",
            "- Paused -"},
        new string[]{
            "- Bauen -",
            "- Du bist gestorben -",
            "Neustart",
            "- Start -",
            "Du bist mitten in einer unbekannten Eiswüste gestrandet.\n\nVersuche, so lange wie möglich zu überleben.\nBaue Häuser, um dich warm zu halten und schalte zusätzliche Gebäude frei.\n\nAber sei vorsichtig, vielleicht bist du nicht so allein, wie du dachtest...",
            "Anleitung",
            "- Anleitung -",
            "Laufe durch Antippen des Bodens\n\nFälle Bäumen durch Antippen eines Baumes\n\nBaue/Repariere Gebäude durch Antippen",
            "- Pausiert -"}};

    private readonly string[][] tooltip = new string[][]{
        new string[] {
            "<b>Tap on the Building you want to build.\nTap and Hold on a Building to show a description.</b>",
            "The <i>House</i> will provide health.",
            "The <i>Foresters Hut</i> will plant new trees nearby.",
            "The <i>Guard Tower</i> will provide sight and will reveal monsters." },
        new string[]{
            "<b>Tippe auf das Gebäude, das du bauen möchtest.\nTippe und halte auf ein Gebäude, um die Beschreibung anzuzeigen.</b>",
            "Das <i>Haus</i> heilt in seinem Umkreis.",
            "Der <i>Förster</i> pflanzt neue Bäume in der Umgebung.",
            "Der <i>Wachtturm</i> bietet Sicht und deckt Monster auf."}};

    private readonly string[][] scoreText = new string[][] {
        new string[]{ "You survived for ", " minutes!" },
        new string[]{ "Du hast ", " Minuten überlebt!" }};

    private void Awake()
    {
        GameManager.instance.earlyInit += EarlyInit;
    }

    private void EarlyInit()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;

        List<string> languageStrings;
        int value = 0;

        switch (systemLanguage)
        {
            case SystemLanguage.German:
                languageStrings = new List<string>(languageStringsTranslation[1]);
                value = 1;
                break;
            default:
                languageStrings = new List<string>(languageStringsTranslation[0]);
                break;
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(languageStrings);
        dropdown.value = value;
    }

    public void SetTranslation(int index)
    {
        if (index >= text.Length)
            return;

        GameManager.instance.culture = cultures[index];
        GameManager.instance.ui.tooltip = tooltip[index];
        GameManager.instance.ui.scoreT = scoreText[index];

        // UI translations
        buildTitle.text = text[index][0];
        deathText.text = text[index][1];
        restartButtonText.text = text[index][2];
        loadingTitle.text = text[index][3];
        descriptionText.text = text[index][4];
        helpButton.text = text[index][5];
        helpTitle.text = text[index][6];
        helpText.text = text[index][7];
        pausedText.text = text[index][8];
    }
}