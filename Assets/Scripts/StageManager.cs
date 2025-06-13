using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 스테이지 관리자
/// </summary>
public class StageManager : MonoBehaviour
{
  #region Singleton Pattern
  //단 하나의 객체만 허용하는 디자인패턴
  //다른 클래스나 데이터베이스를 관리하는 클래스에 적합함.
  private static StageManager instance;
  public static StageManager Get
  {
    get
    {
      if(instance == null)
      {
        GameObject go = Resources.Load<GameObject>("StageManager");
        Instantiate<GameObject>(go);
      }

      return instance;
    }
  }

  //객체 생성후 활성화시 가장 처음 한번만 호출
  private void Awake()
  {
    //자신이 가장 먼저 생성된 단하나의 객체인지 확인.
    if (instance != null)
    {
      //아니라면 스스로 파괴해 하나의 인스턴스만 남아있도록 강제함.
      Destroy(this);
      return;
    }

    //자신을 등록함.
    instance = this;
    //별로의 파괴 메소드를 쓰지 않는 이상 파괴되지 않도록 등록.
    DontDestroyOnLoad(this);
    //클리어UI 비활성.
    clearPanel.SetActive(false);
  }
  #endregion

  #region Variables
  public GameObject clearPanel;       //스테이지 클리어시 표현할 UI패널
  public Text remainText;             //남은 수정수를 표시하는 텍스트UI
  public List<Destructables> targetList = new List<Destructables>(30);  //씬 안에 활성화되어 있는 수정 리스트
  public string SceneName;            //씬전환할 때 사용할 씬의 이름
  #endregion

  #region Public Method
  /// <summary>
  /// 초기 상태로 되돌림
  /// </summary>
  public void Reset()
  {
    //클리어 패널을 비활성
    clearPanel.SetActive(false);
    //기존 수정 리스트를 모두 제거하고 현재 씬에서 활성화되어 있는 수정들을 검색해 리스트에 새로 담는다.
    targetList.Clear();
    targetList.AddRange(FindObjectsByType<Destructables>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID));
    //담아진 수정들의 갯수를 UI에 표시한다.
    remainText.text = targetList.Count.ToString();
  }


  /// <summary>
  /// 다음 스테이지로 씬전환.
  /// </summary>
  public void Next()
  {
    if (string.IsNullOrEmpty(SceneName))
    {
      Debug.LogError("전환할 씬의 이름이 비어져 있습니다. 반드시 입력해주세요");
      return;
    }

    SceneManager.LoadScene(SceneName);
  }

  /// <summary>
  /// 수정을 파괴됨
  /// </summary>
  /// <param name="target">파괴된 수정</param>
  public void RemoveTarget(Destructables target)
  {
    if(targetList.Contains(target))
    {
      targetList.Remove(target);
      remainText.text = targetList.Count.ToString();
    }

    if(targetList.Count == 0)
    {
      clearPanel.SetActive(true);
    }
  }

  /// <summary>
  /// 플레이 중단
  /// </summary>
  public void Quit()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.ExitPlaymode();
#else
    Application.Quit();
#endif
  }
  #endregion
}
