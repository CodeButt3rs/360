using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using CodeMonkey.Utils;
using System;

public class SceneGraph : MonoBehaviour
{
    private List<GraphPoint> _graphPoints = new();
    private List<GraphLine> _graphLines = new();
    [SerializeField]
    private RectTransform canvas;
    [SerializeField]
    private Sprite circleSprite;
    [SerializeField]
    private Sprite arrowSprite;

    [SerializeField]
    private GameObject pointPref;

    private SceneManager SceneManager;
    private Dictionary<Scene, List<Scene>> sceneTransitions = new();
    private Dictionary<Scene, List<Scene>> sceneTransitionsAfter = new();

    Vector2 PreviousAnchor = new Vector2(0, 40);
    public static SceneGraph Instance { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance == this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.D))
                BuildGraph(SceneManager.Instance.Scenes);
            if (Input.GetKeyDown(KeyCode.A))
                ClearCanvas();
            if (Input.GetKeyDown(KeyCode.W))
                UpdateGraph(SceneManager.Instance.Scenes);
        }
    }
    public void BuildGraph(List<Scene> scenesList)
    {
        SceneManager = SceneManager.Instance;
        Debug.Log("Building graph");
        Vector2 delta = new(60, 0);
        foreach(var scene in scenesList)
        {
            sceneTransitions.Add(scene, new List<Scene>());
            sceneTransitionsAfter.Add(scene, new List<Scene>());
            foreach (var point in scene.Points)
            {
                if(point is TransitionPoint tPoint && SceneManager.Scenes.Count > 1)
                    sceneTransitions[scene].Add(tPoint.TransitionScene is not null ? tPoint.TransitionScene : SceneManager.Scenes.FindIndex(x => x == SceneManager.CurrentScene) == 1 ? SceneManager.Scenes[0] != scene ? SceneManager.Scenes[0] : SceneManager.Scenes[1] : SceneManager.Scenes[1]);
            }
            CreatePoint(GetNextAnchor(), scene);
        }
        foreach(var point in _graphPoints)
        {
            foreach(var scenePoint in sceneTransitions[point.TransitionScene])
            {
                if (!sceneTransitionsAfter[scenePoint].Any(x=> x == point.TransitionScene) || !sceneTransitionsAfter[point.TransitionScene].Any(x => x == scenePoint))
                {
                    CreateLine(point, FindPoint(scenePoint));
                    //Debug.Log(point.TransitionScene.Name + " " + FindPoint(scenePoint).TransitionScene.Name);
                    sceneTransitionsAfter[scenePoint].Add(point.TransitionScene);
                    sceneTransitionsAfter[point.TransitionScene].Add(scenePoint);
                }
            }
        }
    }
    public void UpdateGraph(List<Scene> scenesList)
    {
        Debug.Log("Building graph");
        Vector2 delta = new(60, 0);
        sceneTransitions.Clear();
        sceneTransitionsAfter.Clear();
        foreach (var scene in scenesList)
        {
            sceneTransitions.Add(scene, new List<Scene>());
            sceneTransitionsAfter.Add(scene, new List<Scene>());
            foreach (var point in scene.Points)
            {
                if (point is TransitionPoint tPoint)
                    sceneTransitions[scene].Add(tPoint.TransitionScene is not null ? tPoint.TransitionScene : SceneManager.Scenes.FindIndex(x => x == SceneManager.CurrentScene) == 1 ? SceneManager.Scenes[0] != scene ? SceneManager.Scenes[0] : SceneManager.Scenes[1] : SceneManager.Scenes[1]);
            }
            if(!_graphPoints.Any(x=> x.TransitionScene == scene))
            {
                CreatePoint(GetNextAnchor(), scene);
            }
        }
        _graphPoints.ToList().ForEach(x => {
            x.RemoveLines();
            if (!scenesList.Any(y => x.TransitionScene == y))
            {
                x.RemovePoint();
                _graphPoints.Remove(x);
                sceneTransitions.Remove(x.TransitionScene);
                foreach (var key in sceneTransitions.Keys)
                    sceneTransitions[key].Remove(x.TransitionScene);
                foreach (var key in sceneTransitionsAfter.Keys)
                    sceneTransitions[key].Remove(x.TransitionScene);
            }
        });
        foreach (var point in _graphPoints)
        {
            foreach (var scenePoint in sceneTransitions[point.TransitionScene])
            {
                if (!sceneTransitionsAfter[scenePoint].Any(x => x == point.TransitionScene) && !sceneTransitionsAfter[point.TransitionScene].Any(x => x == scenePoint))
                {
                    CreateLine(point, FindPoint(scenePoint));
                    sceneTransitionsAfter[scenePoint].Add(point.TransitionScene);
                    sceneTransitionsAfter[point.TransitionScene].Add(scenePoint);
                }
            }
        }
    }
    private Vector2 GetNextAnchor()
    {
        Vector2 delta = new(120, 0);
        if (PreviousAnchor.x >= 1400.0f)
        {
            PreviousAnchor.y += 80;
            PreviousAnchor.x = 0;
        }
        PreviousAnchor += delta;
        return PreviousAnchor;
    }
    public GraphPoint CreatePoint(Vector2 anchoredPos, Scene scene = null)
    {
        GameObject gameObject = Instantiate(pointPref);

        gameObject.transform.SetParent(canvas.transform, false);

        gameObject.GetComponent<GraphPointEvents>().SetSceneName(scene.Name);

        RectTransform rT = gameObject.GetComponent<RectTransform>();
        rT.anchoredPosition = anchoredPos;
        rT.anchorMax = Vector2.zero;
        rT.anchorMin = Vector2.zero;

        GraphPoint gPoint = new GraphPoint(gameObject, scene);
        _graphPoints.Add(gPoint);

        return gPoint;
    }
    public GraphLine CreateLine(GraphPoint startPoint, GraphPoint endPoint)
    {
        Vector2 startPos = startPoint.RectTransform.anchoredPosition;
        Vector2 endPos = endPoint.RectTransform.anchoredPosition;

        GameObject gameObject = new GameObject("line", typeof(Image));
        gameObject.transform.SetParent(canvas.transform, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

        gameObject.transform.SetSiblingIndex(0);

        RectTransform rT = gameObject.GetComponent<RectTransform>();
        Vector2 direction = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);

        rT.anchorMax = Vector2.zero;
        rT.anchorMin = Vector2.zero;
        rT.sizeDelta = new Vector2(distance, 3);
        rT.anchoredPosition = startPos + direction * distance * 0.5f;
        rT.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));

        GraphLine gLine = new GraphLine(startPoint, endPoint, gameObject);

        LineDirection lineDirection = LineDirection.None;
        if (sceneTransitions[startPoint.TransitionScene].Any(x => x == endPoint.TransitionScene) && sceneTransitions[endPoint.TransitionScene].Any(x => x == startPoint.TransitionScene))
            lineDirection = LineDirection.TwoSided;
        else if (sceneTransitions[endPoint.TransitionScene].Any(x => x == startPoint.TransitionScene))
            lineDirection = LineDirection.SecondToFirst;
        else if (sceneTransitions[startPoint.TransitionScene].Any(x => x == endPoint.TransitionScene))
            lineDirection = LineDirection.FirstToSecond;

        gLine.Direction = lineDirection;

        gLine.RenderLine();

        _graphLines.Add(gLine);

        return gLine;
    }
    public GameObject CreateArrow()
    {

        GameObject gameObject = new GameObject("arrow", typeof(Image));
        gameObject.transform.SetParent(canvas.transform, false);
        gameObject.GetComponent<Image>().sprite = arrowSprite;

        return gameObject;
    }
    public GraphPoint FindPoint(GameObject gObj) => _graphPoints.Find(x => x.GameObject == gObj);
    public GraphPoint FindPoint(Scene scene) => _graphPoints.Find(x => x.TransitionScene == scene);
    public void ClearCanvas()
    {
        PreviousAnchor = new Vector2(0, 40);
        foreach (var item in _graphPoints)
        {
            item.RemoveLines();
            Destroy(item.GameObject);
        }
        _graphPoints.Clear();
        sceneTransitions.Clear();
        sceneTransitionsAfter.Clear();
    }
    public List<GraphPoint> GetConnectedPoints(GraphPoint gP)
    {
        List<GraphLine> connectedLines = new List<GraphLine>();
        List<GraphPoint> connectedPoints = new List<GraphPoint>();

        for (int i = 0; i < _graphLines.Count; i++)
            if (_graphLines[i].GraphPoints.Any(x=> x == gP))
                connectedLines.Add(_graphLines[i]);

        for (int i = 0; i < connectedLines.Count; i++)
            for (int j = 0; j < 2; j++)
                connectedPoints.Add(connectedLines[i].GraphPoints[j]);

        Debug.Log(sceneTransitionsAfter[gP.TransitionScene].Count + " " + connectedPoints.Distinct().ToList().Count);
        return connectedPoints;
    }
}

public class GraphPoint : MonoBehaviour
{
    private GameObject _gameObject;
    private RectTransform _rectTransform;
    private List<GraphPoint> _graphPoints = new();
    private List<GraphLine> _lines = new();
    private SceneGraph sGraph = SceneGraph.Instance;
    private Scene _scene;
    public GameObject GameObject { get => _gameObject; }
    public List<GraphPoint> Points { get => _graphPoints; }
    public RectTransform RectTransform { get => _rectTransform; }
    public Scene TransitionScene { get => _scene; }
    public GraphPoint(GameObject gObj, Scene scene)
    {
        _scene = scene;
        _gameObject = gObj;
        _rectTransform = gObj.GetComponent<RectTransform>();
    }
    public void AddPoint(GraphPoint gPoint) =>
        _graphPoints.Add(gPoint);
    public void AddLine(GraphLine gObj) =>
        _lines.Add(gObj);
    public void RenderLines()
    {
        for (int i = 0; i < _lines.Count; i++)
            _lines[i].RenderLine();
    }
    public void RemoveLine(GraphLine line) =>
        line.DeleteLine();
    public void RemoveLineFromList(GraphLine line) =>
        _lines.Remove(line);
    public void RemoveLines() 
    {
        _lines.ToList().ForEach(x => x.DeleteLine());
        _lines.Clear();
    }
    public void RemovePoint()
    {
        for (int i = 0; i < _lines.Count; i++)
            this.RemoveLine(_lines[i]);
        Destroy(_gameObject);
    }
    public void HighlightConnected()
    {
        HighlightPoint();
        if(_lines.Count > 0)
            SceneGraph.Instance.GetConnectedPoints(this).ForEach(x => x.HighlightPoint());
    }
    public void UnhighlightConnected() 
    {
        UnhighlightPoint();
        if (_lines.Count > 0)
            SceneGraph.Instance.GetConnectedPoints(this).ForEach(x => x.UnhighlightPoint());
    }
    public void HighlightPoint() =>
        _rectTransform.GetComponent<Image>().color = new Color(1.0f, 0.3563678f, 0f, 1f);  
    public void UnhighlightPoint() =>
        _rectTransform.GetComponent<Image>().color = Color.white;
}

public class GraphLine : MonoBehaviour
{
    private GraphPoint[] _graphPoints = new GraphPoint[2];
    private GameObject _gameObject;
    private RectTransform _rectTransform;
    private LineDirection _direction = LineDirection.None;
    private List<GameObject> _arrows = new();
    public GraphPoint[] GraphPoints { get => _graphPoints; }
    public GraphPoint FirstPoint { get =>_graphPoints[0]; }
    public GraphPoint SecondPoint { get => _graphPoints[1]; }
    public GameObject GameObject { get => _gameObject; }
    public RectTransform RectTransform { get => _rectTransform; }
    public LineDirection Direction { get => _direction; set => _direction = value; }
    public GraphLine(GraphPoint p1, GraphPoint p2, GameObject gObj)
    {
        p1.AddLine(this);
        p2.AddLine(this);

        _graphPoints[0] = p1;
        _graphPoints[1] = p2;

        _gameObject = gObj;
        _rectTransform = gObj.GetComponent<RectTransform>();
    }
    public void RenderLine()
    {
        Vector2 startPos = FirstPoint.RectTransform.anchoredPosition;
        Vector2 endPos = SecondPoint.RectTransform.anchoredPosition;

        Vector2 direction = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);

        _rectTransform.anchorMax = Vector2.zero;
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.sizeDelta = new Vector2(distance, 3);
        _rectTransform.anchoredPosition = startPos + direction * distance * 0.5f;
        _rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));

         RenderArrows();
    }
    private void RenderArrows()
    {
        if(_direction == LineDirection.None)
        {
            if(_arrows.Count != 0)
                for (int i = 0; i < _arrows.Count; i++)
                    Destroy(_arrows[i]);
            _arrows.Clear();
            return;
        }

        if (_arrows.Count == 2 && (_direction == LineDirection.FirstToSecond || _direction == LineDirection.SecondToFirst))
        {
            var a = _arrows[1];
            Destroy(a);
            _arrows.Remove(a);
        }
        int idx = 0;
        if (_arrows.Count == 0)
            _arrows.Add(SceneGraph.Instance.CreateArrow());
        //Debug.Log(Direction);
        var gameObject = _arrows[idx];

        Vector2 startPos = FirstPoint.RectTransform.anchoredPosition;
        Vector2 endPos = SecondPoint.RectTransform.anchoredPosition;

        RectTransform rT = gameObject.GetComponent<RectTransform>();
        Vector2 direction = Vector2.zero;

        rT.anchorMax = Vector2.zero;
        rT.anchorMin = Vector2.zero;
        rT.sizeDelta = new Vector2(20, 20);
        switch (Direction)
        {
            case LineDirection.None:
                break;
            case LineDirection.FirstToSecond:

                direction = (endPos - startPos).normalized;
                rT.anchoredPosition = startPos + direction * 80;
                rT.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));

                break;
            case LineDirection.SecondToFirst:

                direction = (startPos - endPos).normalized;
                rT.anchoredPosition = endPos + direction * -80;
                rT.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));

                break;
            case LineDirection.TwoSided:

                idx += 1;
                if (_arrows.Count != 2)
                    _arrows.Add(SceneGraph.Instance.CreateArrow());

                direction = (endPos - startPos).normalized;
                rT.anchoredPosition = startPos + direction * 80;
                rT.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));

                gameObject = _arrows[idx];

                rT = gameObject.GetComponent<RectTransform>();

                direction = (startPos - endPos).normalized;
                rT.anchorMax = Vector2.zero;
                rT.anchorMin = Vector2.zero;
                rT.sizeDelta = new Vector2(20, 20);
                rT.anchoredPosition = endPos + direction * 80;
                rT.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));

                break;
            default:
                break;
        }
    }
    public void DeleteLine()
    {
        for (int i = 0; i < _arrows.Count; i++)
            Destroy(_arrows[i]);
        
        Destroy(_gameObject);
        _graphPoints[0].RemoveLineFromList(this);
        _graphPoints[1].RemoveLineFromList(this);
    }
}
public enum LineDirection{ None, FirstToSecond, SecondToFirst, TwoSided }