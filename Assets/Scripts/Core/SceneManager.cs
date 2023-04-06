using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Runtime;
using TMPro;
using AnotherFileBrowser;
using AnotherFileBrowser.Windows;
using System;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour
{
    CrossScenecManager CSM = CrossScenecManager.Instance;

    public bool isDebugEnabled = false;

    [SerializeField]
    private GameObject pointEditor;
    [SerializeField]
    private GameObject typeEditorDropdown;
    [SerializeField]
    private GameObject descriptionEditor;
    [SerializeField]
    private GameObject sceneEditorDropdown;

    [SerializeField]
    private GameObject userCamera;
    public GameObject UserCamera { get => userCamera; }


    private TMP_InputField descriptionInput;
    private TMP_Dropdown typeDropdown;
    private TMP_Dropdown sceneDropdown;

    [SerializeField]
    private TMP_Text sceneIdText;
    [SerializeField]
    private TMP_Text sceneNameText;
    [SerializeField]
    private TMP_Text scenesCountText;

    [SerializeField]
    private RectTransform contentRect;
    [SerializeField]
    private GameObject contentScroll;
    [SerializeField]
    private GameObject pointUIPrefab;

    [SerializeField]
    private Renderer pano;
    [SerializeField]
    private GameObject roller;

    [SerializeField]
    private Texture2D startPano;

    [SerializeField]
    private GameObject infoCanvas;
    [SerializeField]
    private GameObject typeCanvas;
    public Point CurrentPoint { get; set; }

    private protected List<Scene> _sceneList = new List<Scene>();
    [SerializeField]
    private protected Scene _currentScene;
    [SerializeField]
    public List<Scene> Scenes { get => _sceneList; set { } }
    public Scene CurrentScene { get => _currentScene; set { if (_currentScene != value) _currentScene = value; } }
    public static SceneManager Instance { get; set; }
    public GameObject prefabPoint;
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance == this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        if (isDebugEnabled)
        {
            Scene s = new Scene("StartScene", startPano, CheckUniquieID("asdasd"), "", "");
            Scene s1 = new Scene("StartScene2", startPano, CheckUniquieID("12"), "", "");
            Scene s2 = new Scene("StartScene3", startPano, CheckUniquieID("32"), "", "");
            _currentScene = s;
            _sceneList.Add(s);
            _sceneList.Add(s1);
            _sceneList.Add(s2);
            InitializeManager(s);
            return;
        }
        CrossScenecManager CSM = CrossScenecManager.Instance;
        if (CSM.IsNewTour)
            InitializeManager(CSM.StartScene);
        else
        {
            _sceneList = CSM.TourData.scenes;
            InitializeManager(CSM.TourData.StartScene);
        }
    }

    private void InitializeManager(Scene startScene)
    {
        _currentScene = startScene;
        if (!isDebugEnabled && CSM.IsNewTour)
            _sceneList.Add(startScene);
        InitializeScene(startScene);
    }
    public void ChangeSceneData(string name, string author)
    {
        _currentScene.Author = author;
        _currentScene.Name = name;
        SceneInfo();
        TourInfo();
        ScenesList();
    }
    public void ChangeSceneData(Scene scene, string name, string author)
    {
        scene.Author = author;
        scene.Name = name;
        SceneInfo();
        TourInfo();
        ScenesList();
    }
    public void CreateScene(string name, string id, Texture2D texture, string author = "", string description = "")
    {
        Scene s = new Scene(name, texture, CheckUniquieID(id), author, description);
        _sceneList.Add(s);
        TourInfo();
        ScenesList();
    }
    public void DeleteScene()
    {
        _sceneList.Remove(_currentScene);
        InitializeScene(_sceneList[0]);
    }
    public void LoadScene(Scene scene)
    {
        foreach (var _p in _currentScene.Points)
        {
            Destroy(_p.GameObject);
            _p.GameObject = null;
        }
        //Debug.Log(scene.Texture);
        _currentScene = scene;
        pano.material.mainTexture = scene.Texture;
        foreach (var p in scene.Points)
        {
            roller.transform.eulerAngles = new Vector3(-p.Pitch, p.Yaw, 0);
            Vector3 pos = roller.transform.forward * 29;
            GameObject point = Instantiate(prefabPoint);
            point.transform.SetPositionAndRotation(pos, roller.transform.rotation);
            p.GameObject = point;
            if (p is TransitionPoint)
                p.GameObject.GetComponent<ScenesHandler>().SetTransSprite();
        }
    }
    internal bool CheckUniquieID(Scene scene)
        => _sceneList.Any(x => x.Id == scene.Id);
    internal string CheckUniquieID(string id)
    {
        if (!_sceneList.Any(x => x.Id == id))
            return id;
        else
            return Random.Range(0, 2500000).ToString();
    }
    public void InitializeScene(Scene scene)
    {
        LoadScene(scene);
        SceneInfo();
        TourInfo();
        ScenesList();
    }
    private void SceneInfo()
    {
        sceneNameText.text = _currentScene.Name;
        sceneIdText.text = _currentScene.Id;
    }
    private void TourInfo()
    {
        scenesCountText.text = _sceneList.Count.ToString();
    }
    public void ScenesList()
    {
        for (int i = 0; i < contentScroll.transform.childCount; i++)
            Destroy(contentScroll.transform.GetChild(i).gameObject);
        foreach (var item in _sceneList)
        {
            GameObject gObj = Instantiate(pointUIPrefab);
            gObj.GetComponent<SceneUIScript>().InitializeUIScene(item.Name, item.Id);
            if (item == _currentScene) gObj.GetComponent<SceneUIScript>().CurrentScene();
            gObj.transform.parent = contentScroll.transform;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentRect.sizeDelta.y + 40f);
        }
    }
    public void CreatePoint(Transform position)
    {
        Vector3 pos = position.forward * 29;
        GameObject point = Instantiate(prefabPoint);
        point.transform.position = pos;
        point.transform.rotation = position.rotation;
        _currentScene.AddPoint(point);
    }
    public void DeletePoint(GameObject gObj)
    {
        _currentScene.RemovePoint(gObj);
        Destroy(gObj);
    }
    public Point FindPoint(GameObject gObj) => CurrentScene.Points[FindPointIndex(gObj)];
    public int FindPointIndex(GameObject gObj) => CurrentScene.Points.FindIndex(x => x.GameObject == gObj);
    public Scene FindSceneById(string Id) => _sceneList.Find(x => x.Id == Id);
    public Scene FindSceneByNameId(string NameId) => _sceneList.Find(x => x.Id == NameId.Substring(NameId.IndexOf('#')+1));
    public void ChangePointInfo(GameObject gObj)
    {
        sceneDropdown = sceneEditorDropdown.GetComponent<TMP_Dropdown>();
        typeDropdown = typeEditorDropdown.GetComponent<TMP_Dropdown>();
        descriptionInput = descriptionEditor.GetComponent<TMP_InputField>();

        CurrentPoint = FindPoint(gObj);
        if(CurrentPoint is InfoPoint)
        {
            InfoPoint tPoint = CurrentPoint as InfoPoint;
            typeDropdown.SetValueWithoutNotify(0);
            descriptionInput.text = tPoint.Description;
            pointEditor.SetActive(true);
            infoCanvas.SetActive(true);
            typeCanvas.SetActive(false);
        }
        if(CurrentPoint is TransitionPoint)
        {
            TransitionPoint tPoint = CurrentPoint as TransitionPoint;
            RefreshScenes();
            typeDropdown.SetValueWithoutNotify(1);
            if (_sceneList.Count > 1 && tPoint.TransitionScene is not null)
            {
                int val = sceneDropdown.options.FindIndex(x => x.text.EndsWith(tPoint.TransitionScene.Id));
                sceneDropdown.value = val > -1 ? val : 0;
            }
            else
            {
                sceneDropdown.value = 0;
            }
            pointEditor.SetActive(true);
            infoCanvas.SetActive(false);
            typeCanvas.SetActive(true);
        }
    }
    public void RefreshScenes()
    {
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        foreach (var item in _sceneList.Where(x => x.Id != CurrentScene.Id))
            list.Add(new TMP_Dropdown.OptionData(item.Name + " #" + item.Id));
        sceneDropdown.options = list;
    }
    public void SaveTour()
    {
        string destination = "";
        new FileBrowser().OpenPathBrowser(path => { destination = path + $"/Panoramic_Save_{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.panor"; });
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        TourData data = new TourData(_sceneList, _sceneList[0]);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        Debug.Log(file.Position);
        Debug.Log(destination);
        file.Close();
    }
    public void SaveToPannellumConfig()
    {
        string pathToSave = Application.persistentDataPath + "/pannellum";
        if (Directory.Exists(pathToSave))
            Directory.CreateDirectory(pathToSave);

        string configText = "{ \"default\":{\"firstScene\":" + $"\"{_sceneList[0].Id}\"" + "},\"scenes\":{";
        for (int i = 0; i < _sceneList.Count; i++)
        {
            Scene item = _sceneList[i];
            configText += $"\"{item.Id}\":" + "{\"title\":" + $"\"{item.Name}\"," + "\"type\":\"equirectangular\"," + $"\"panorama\": \"photos/{item.Name}{item.Id}.png\",\"yaw\":{item.StartYaw.ToString(System.Globalization.CultureInfo.InvariantCulture)},\"pitch\":{item.StartPitch.ToString(System.Globalization.CultureInfo.InvariantCulture)},\"hotSpots\":[";

            //Debug.Log(item.Points.Count);
            for (int j = 0; j < item.Points.Count; j++)
            {
                Point p = item.Points[j];
                configText += '{';
                if (p is InfoPoint point)
                {
                    string descr = point.Description is not null ? point.Description : "";
                    Debug.Log("*");
                    configText += "\"type\":\"info\", \"text\":\"" + descr + "\",";
                }
                if (p is TransitionPoint tPoint)
                {
                    if (_sceneList.Count == 1) { configText.Remove(configText.Length - 1); continue; }
                    Debug.Log("$$");
                    Scene tScene = tPoint.TransitionScene is null ? _sceneList[1] != item ? _sceneList[1] : _sceneList[0] : tPoint.TransitionScene;
                    configText += $"\"type\":\"scene\",\"sceneId\":\"{tScene.Id}\",\"text\":\"{tScene.Name}\",";
                }
                configText += $"\"yaw\":{p.Yaw.ToString(System.Globalization.CultureInfo.InvariantCulture)},\"pitch\":{p.Pitch.ToString(System.Globalization.CultureInfo.InvariantCulture)}" + "}";
                configText += j + 1 == item.Points.Count ? "" : ",";
            }
            configText += "]}";
            configText += i + 1 == _sceneList.Count ? "" : ",";
        }
        configText += "}}";
        StreamWriter writer = new StreamWriter(pathToSave + "/config.json", true);
        writer.WriteLine(configText);
        writer.Close();
    }
    public void SaveToPannellumPhotos()
    {
        string pathToSave = Application.persistentDataPath + "/pannellum";
        if (Directory.Exists(pathToSave))
            Directory.CreateDirectory(pathToSave);
        for (int i = 0; i < _sceneList.Count; i++)
        {
            Scene item = _sceneList[i];
            Debug.Log(item.Texture.height);
            byte[] bytes = item.Texture.EncodeToPNG();
            var dirPhotoPath = pathToSave + "/photos";
            if (!Directory.Exists(dirPhotoPath))
                Directory.CreateDirectory(dirPhotoPath);

            FileStream file;
            if (File.Exists(dirPhotoPath + $"/{item.Name}{item.Id}" + ".png")) file = File.OpenWrite(dirPhotoPath + $"/{item.Name}{item.Id}" + ".png");
            else file = File.Create(dirPhotoPath + $"/{item.Name}{item.Id}" + ".png");

            file.Write(bytes);
            //File.WriteAllBytes(dirPhotoPath + $"/{item.Name}{item.Id}" + ".png", bytes);
            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPhotoPath);
            file.Close();
        }
    }
    public void SaveToPannellum()
    {
        SaveToPannellumConfig();
        SaveToPannellumPhotos();
    }
}
[System.Serializable]
public class Scene
{
    [SerializeField]
    private List<Point> _points = new List<Point>();
    private protected string _name;
    private protected string _id;
    private protected string _author;
    private protected string _description;
    private protected byte[] _bytes;
    private protected float _startYaw;
    private float _startPitch;
    [System.NonSerialized]
    private protected Texture2D _texture;
    public string Name { get => _name; set { if (value != _name) _name = value; } }
    public string Id { get => _id; set { 
            if (value != _id && !SceneManager.Instance.CheckUniquieID(this))
                _id = value; 
            else 
                Id = Random.Range(0, 1000).ToString(); 
        } 
    }
    public string Author { get => _author; set { if (value != _author) _author = value; } }
    public string Description { get => _description; set { if (value != _description) _description = value; } }
    public float StartYaw { get => _startYaw; set { if (value != _startYaw) _startYaw = value; } }
    public float StartPitch { get => _startPitch; set { if (value != _startPitch) _startPitch = value; } }
    public List<Point> Points { get => _points; }
    public Texture2D Texture { get => _texture; set { if (value != _texture) _texture = value; } }
    public Scene(string name, Texture2D texture, string id, string author, string description)
    {
        _name = name;
        _id = id;
        _texture = texture;
        _author = author;
        _description = description;
        _bytes = _texture.EncodeToPNG();
        _startPitch = 0f;
        _startPitch = 0f;
    }
    public byte[] ByteTexture { get => _bytes; }

    public void AddPoint(GameObject point) =>
        _points.Add(new InfoPoint("", point.transform.rotation.eulerAngles.x, point.transform.rotation.eulerAngles.y, "", point));
    public void AddPoint(InfoPoint p) => _points.Add(p);
    public void AddPoint(TransitionPoint p) => _points.Add(p);
    public void RemovePoint(GameObject gObj) => _points.Remove(SceneManager.Instance.FindPoint(gObj));
    public void CastingTo(GameObject gObj)
    {
        SceneManager SceneManager = SceneManager.Instance;
        int pointIndex = SceneManager.Instance.FindPointIndex(gObj);
        //Debug.Log(pointIndex);
        Point point = _points[pointIndex];
        if (point is InfoPoint)
        {
            InfoPoint newPoint = (InfoPoint)point;
            TransitionPoint tScene = (Point)newPoint as TransitionPoint;
            _points[pointIndex] = new TransitionPoint(newPoint.Name, newPoint.Pitch, newPoint.Yaw, newPoint.Description, tScene is null ? SceneManager.Scenes.FindIndex(x => x == SceneManager.CurrentScene) == 1 ? SceneManager.Scenes[0] != SceneManager.CurrentScene ? SceneManager.Scenes[0] : SceneManager.Scenes[1] : SceneManager.Scenes[1] : tScene.TransitionScene, newPoint.GameObject);
        }
        else
        {
            TransitionPoint newPoint = (TransitionPoint)point;
            InfoPoint iDescr = (Point)newPoint as InfoPoint;
            _points[pointIndex] = new InfoPoint(newPoint.Name, newPoint.Pitch, newPoint.Yaw, iDescr is null ? "" : iDescr.Description, newPoint.GameObject);
        }

        //Debug.Log(_points[pointIndex].GetType());
    }
    public void LoadTextureFromBytes()
    {
        Debug.Log(_bytes);
        Texture2D tex = new Texture2D(3040,6080);
        tex.LoadImage(_bytes);
        _texture = tex;
    }
    public void ClearPoints() => _points.Clear();
    public void SetStartPos()
    {
        GameObject camera = SceneManager.Instance.UserCamera;
        float yaw = camera.transform.rotation.eulerAngles.y;
        float pitch = camera.transform.rotation.eulerAngles.x;
        
        _startYaw = yaw;
        _startPitch = pitch >= 270 && pitch < 360 ? Mathf.Abs(pitch - 360) : -pitch;
        Debug.Log(_startYaw + " " + _startPitch);
    }
    
}
[System.Serializable]
public class Point
{
    private protected string _name;
    private protected float _pitch;
    private protected float _yaw;
    [System.NonSerialized]
    private protected GameObject? _gameObject;
    public string Name { get => _name; set { if (value != _name) _name = value; } }
    public float Pitch { get => _pitch; set { if (value != _pitch) _pitch = value; } }
    public float Yaw { get => _yaw; set { if (value != _yaw) _yaw = value; } }
    public GameObject GameObject { get => _gameObject; set { _gameObject = value; } }
    public Point(string name, float pitch, float yaw, GameObject gameObject, string description = null)
    {
        _name = name;
        _pitch = pitch >= 270 && pitch < 360 ? Mathf.Abs(pitch - 360) : -pitch;
        _yaw = yaw;
        _gameObject = gameObject;
    }
    public void MovePoint()
    {
        var userCamera = SceneManager.Instance.UserCamera;

        float pitch = userCamera.transform.rotation.eulerAngles.x;
        float yaw = userCamera.transform.rotation.eulerAngles.y;

        _pitch = pitch >= 270 && pitch < 360 ? Mathf.Abs(pitch - 360) : -pitch;
        _yaw = yaw;

        Vector3 pos = userCamera.transform.forward * 29;

        _gameObject.transform.position = pos;
        _gameObject.transform.rotation = userCamera.transform.rotation;
    }

}
[System.Serializable]
public class InfoPoint : Point
{
#nullable enable
    private protected string? _description;
#nullable disable
    public InfoPoint(string name, float pitch, float yaw, string description, GameObject gameObject) : base(name, pitch, yaw, gameObject, description) 
    {
        _description = description;
    }
    public string Description { get => _description; set { if (value != _description) _description = value; } }
    public void ChangeDescription(string description)
    {
        _description = description;
    }

}
[System.Serializable]
public class TransitionPoint : Point 
{
    [System.NonSerialized]
    private protected Scene? _transitionScene;
    public string _transitionSceneId;
    public Scene TransitionScene { get => _transitionScene; set { if (value != _transitionScene) _transitionScene = value; } }
    public string TransitionSceneId { get => _transitionSceneId; set { if (value != _transitionSceneId) _transitionSceneId = value; } }

    public TransitionPoint(string name, float pitch, float yaw, string description, Scene transitionScene, GameObject gameObject) : base(name, -pitch, yaw, gameObject, description)
    {
        _transitionScene = transitionScene;
        _transitionSceneId = _transitionScene.Id;
    }
    public void SetTransitionScene(string name) { _transitionScene = SceneManager.Instance.FindSceneByNameId(name); _transitionSceneId = _transitionScene.Id; Debug.Log(name); }
    public void RefreshScenes()
    => _gameObject.GetComponent<ScenesHandler>().ChangeScenes(SceneManager.Instance.Scenes.Where(x => SceneManager.Instance.CurrentScene.Id != x.Id).ToList());
}
[System.Serializable]
public class TourData
{
    public List<Scene> scenes;
    public Scene StartScene;
    public List<InfoPoint> iPoints;
    public List<TransitionPoint> tPoints;
    public List<PointScene> pointScene;

    public TourData(List<Scene> scenesToSave, Scene StartSceneToSave)
    {
        List<TransitionPoint> transitionPoints = new();
        List<InfoPoint> infoPoints = new();
        List<PointScene> pointScenes = new();
        scenes = scenesToSave;
        StartScene = StartSceneToSave;
        foreach (var scene in scenesToSave)
        {
            foreach (var p in scene.Points)
            {
                if (p is TransitionPoint point)
                    transitionPoints.Add(point);
                if (p is InfoPoint iPoint)
                    infoPoints.Add(iPoint);
                pointScenes.Add(new PointScene(scene, p));
            }
        }
        iPoints = infoPoints;
        tPoints = transitionPoints;
        pointScene = pointScenes;
    }
}
[System.Serializable]
public struct PointScene
{
    public Scene Scene;
    public Point Point;
    public PointScene(Scene s, Point p)
    {
        Scene = s;
        Point = p;
    }
}