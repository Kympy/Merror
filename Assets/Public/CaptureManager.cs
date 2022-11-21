using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections.ObjectModel;
using static VideoPlayerManager.VideoClips;
using Unity.VisualScripting;
using TMPro;

public class CaptureManager : MonoBehaviour
{
    public class Capture
    {
        public string name = null;
        public string subTitle = null;

        public Capture(string name, string subtitle)
        { 
            this.name = name;
            this.subTitle = subtitle;
        }
    }
    /// <summary>
    /// Defined language
    /// </summary>
    public enum LanguageCategory
    {
        ENGLISH,
        KOREAN,   // Test
    }

    [SerializeField] private LanguageCategory language = LanguageCategory.ENGLISH;
    public LanguageCategory Language { get { return language; } set { language = value; } }
    /// <summary>
    /// Defined Captions Category
    /// </summary>
    public enum Category
    {
        ITEMINFO,
        DATE,
        REMEMBER,
    }

    [SerializeField] private string offsetPath;
    [SerializeField] private DirectoryInfo textFilesInfo = null;

    [Header("Caption PathList")]
    [SerializeField] List<string> pathsList = new List<string>();
    [SerializeField] List<string> fileNameList = new List<string>();

    // Language-specific folder path.
    private Dictionary<int, string> languageFilePath = new Dictionary<int, string>();


    // Reads a file in the specified folder.
    private DirectoryInfo directoryInfo = null;

    // Combine captions as a list and store them in the caption table.
    private Dictionary<string, Dictionary<string, Capture>> captionTable = new Dictionary<string, Dictionary<string, Capture>>();

    public void Init()
    {

        offsetPath = Directory.GetCurrentDirectory() + "/Capture";
        ReadCaption(language);
    }
    private void ReadCaption(LanguageCategory language)
    {
        textFilesInfo = new DirectoryInfo(offsetPath);

        for (int i = 0; i < textFilesInfo.GetDirectories().Length; i++)
        {
            languageFilePath.Add(i, textFilesInfo.GetDirectories()[i].FullName);
        }

        // ���õ� ��� �߰�.
        directoryInfo = new DirectoryInfo(languageFilePath[(int)language]);


        // ���丮���� ���� ��� ������ �� ��θ���Ʈ�� �߰�.
        foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
        {
            if (fileInfo.Extension == ".meta") continue;
            pathsList.Add(fileInfo.FullName);
            fileNameList.Add(fileInfo.Name.Replace(".txt", ""));
        }

        // // Saved CationList After Read CationTextFile From PathsList.
        for (int i = 0; i < pathsList.Count; i++)
        {
            Dictionary<string, Capture> captionsList = new Dictionary<string, Capture>();
            using (FileStream fileStream = new FileStream(pathsList[i], FileMode.Open))
            {
                StreamReader reader = new StreamReader(fileStream);
                while (!reader.EndOfStream)
                {
                    string[] captions = reader.ReadLine().Split('|');
                    Debug.Log(captions[0]);
                    if (captions.Length > 2)
                        captionsList.Add(captions[0], new Capture(captions[1], captions[2]));
                    else captionsList.Add(captions[0], new Capture(null, captions[1]));
                }
            }
            captionTable.Add(fileNameList[i], captionsList);
        }
    }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI itemNameText = null;
    [SerializeField] private TextMeshProUGUI itemInfoText = null;

    public void GetCapture(Category category, string objName)
    {
        if (itemNameText == null || itemInfoText == null)
        {
            GameObject Parent = GameObject.Find("InventoryCanvas").transform.GetChild(1).GetChild(0).gameObject;
            itemNameText = Parent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            itemInfoText = Parent.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        string name = objName.Replace("(Clone)", "");
        itemNameText.text = captionTable[category.ToString()][name].name;
        itemInfoText.text = captionTable[category.ToString()][name].subTitle;
    }

    public string GetSubTitle(Category category, int chapter) => captionTable[category.ToString()][(chapter+1).ToString()].subTitle;
    public string GetName(Category category, string objName) => captionTable[category.ToString()][objName].name;


}
