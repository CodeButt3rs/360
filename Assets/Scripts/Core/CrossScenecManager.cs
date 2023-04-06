using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using AnotherFileBrowser.Windows;


public class CrossScenecManager : MonoBehaviour
{
    private bool _isNewTour = false;
    private Scene _startScene;
    public Scene StartScene { get => _startScene; } 
    public static CrossScenecManager Instance { get; set; }
    public bool IsNewTour { get => _isNewTour; }

    private TourData _tourData;
    public TourData TourData { get => _tourData; }

    [SerializeField]
    private GameObject sceneName;
    [SerializeField]
    private GameObject sceneImage;
    [SerializeField]
    private GameObject sceneAuthor;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance == this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    public void CreateTour(Scene scene)
    {
        _startScene = scene;
        UnityEngine.SceneManagement.SceneManager.LoadScene("PanoramicView");
    }
    public void CreateScene()
    {
        _isNewTour = true;
        TMP_InputField name = sceneName.GetComponent<TMP_InputField>();
        TMP_InputField image = sceneImage.GetComponent<TMP_InputField>();
        TMP_InputField author = sceneAuthor.GetComponent<TMP_InputField>();
        Texture2D texture;
        if (string.IsNullOrEmpty(name.text) && name.text.Trim().Length == 0)
            return;
        if (string.IsNullOrEmpty(author.text) && author.text.Trim().Length == 0)
            return;
        if (string.IsNullOrEmpty(image.text) && image.text.Trim().Length == 0)
            return;
        WWW www = new WWW(image.text);
        Debug.Log(image.text);
        while (!www.isDone)
            continue;
        texture = www.texture;
        Scene s = new(name.text, texture, Random.Range(0, 1000).ToString(), "", "");
        CreateTour(s);
    }
    public void LoadTour()
    {
        _isNewTour = false;

        var bp = new BrowserProperties();
        bp.filter = "Дамп тура (*.panor) | *.panor";
        string destination = "";
        new FileBrowser().OpenFileBrowser(bp, path => { destination = path; });
        FileStream file;
        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }
        BinaryFormatter bf = new BinaryFormatter();
        TourData data = (TourData)bf.Deserialize(file);
        foreach (var i in data.scenes)
        {
            i.ClearPoints();
            i.LoadTextureFromBytes();
            Debug.Log(i.Texture);
        }
        foreach(var item in data.iPoints)
        {
            PointScene ps = data.pointScene.Find(x => x.Point == item);
            Scene s = data.scenes.Find(x => x == ps.Scene);
            s.AddPoint(item);
        }
        foreach (var item in data.tPoints)
        {
            Debug.Log(item.TransitionSceneId);
            item.TransitionScene = data.scenes.Find(x => x.Id == item.TransitionSceneId);
            PointScene ps = data.pointScene.Find(x => x.Point == item);
            Scene s = data.scenes.Find(x => x == ps.Scene);
            Debug.Log(s.Name + " | " + item.TransitionScene.Name);
            s.AddPoint(item);
        }
        file.Close();
        _tourData = data;
        CreateTour(data.StartScene);
    }
}
