using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 남은 체력을 UI로 표시
/// </summary>
public class Healthbar : MonoBehaviour
{
  #region Variables
  public Transform following;
  public Vector3 offset;
  public Slider progressbar;
  public Image fill;
  public Gradient gradient;
  #endregion

  #region Public Method
  /// <summary>
  /// 체력바 초기화
  /// </summary>
  /// <param name="following">따라다녀야할 게임오브젝트의 Transform</param>
  /// <param name="offset">위치 보정값</param>
  public void Initialize(Transform following, Vector3 offset)
  {
    this.following = following;
    this.offset = offset;
    //캔버스라는 게임오브젝트를 찾아 그 자식으로 들어감.
    transform.SetParent(GameObject.Find("Canvas").transform);
    //체력바가 카메라를 향해 보기 위해 방향 조절(쿼터뷰 시점이기 때문에 고정됨)
    transform.rotation = Camera.main.transform.rotation;
    //동적으로 생성된 UI는 캔버스의 스케일값에 따라 스케일값변동됨. 스케일을 1로 고정
    transform.localScale = Vector3.one;
    //체력이 가득차 있을 때는 체력바가 보이지 않게 함.
    gameObject.SetActive(false);
  }

  /// <summary>
  /// 남은 체력의 퍼센테이지 적용.
  /// </summary>
  /// <param name="health">남은 체력(0.0 ~ 1.0))</param>
  public void SetHealthbar(float health)
  {
    progressbar.value = health;
    //남은 체력에 따라 바의 색이 바뀌도록 함.
    fill.color = gradient.Evaluate(health);
  }

  /// <summary>
  /// 체력바의 On/Off 
  /// </summary>
  /// <param name="isOn"></param>
  public void TurnOn(bool isOn)
  {
    gameObject.SetActive(isOn);
  }
  #endregion

  #region Unity Method
  //Update메소드가 모두 호출된 이후 호출됨
  private void LateUpdate()
  {
    //활성화되어 있는 동안 연결된 게임오브젝트를 따라다님.
    transform.position = following.position + offset;
  }
  #endregion
}
