using UnityEngine;

/// <summary>
/// 실시간으로 타겟 게임오브젝트를 따라다님.
/// </summary>
public class Follower : MonoBehaviour
{
  #region Variables
  public Transform targetTransform;   //타겟의 트랜스폼
  public Vector3 offset;              //위치 보정값
  public float lerp;                  //지연 보정값
  #endregion

  #region Unity Method
  //객체가 생성되고 활성화 되어 있으면 처음 한번만 호출됨. Awake, OnEnable보다 늦게 호출됨.
  void Start()
  {
    //처음 켜졌을 때 타겟 위치로 바로 이동.
    transform.position = targetTransform.position + offset;
  }
  //객체과 활성화 되어 있는 동안 매프레임 호출됨. Update보다 늦게 호출됨.
  void LateUpdate()
  {
    //타겟의 위치로 지연값을 적용해 따라감.
    transform.position = Vector3.Lerp(transform.position, targetTransform.position + offset, lerp * Time.deltaTime);
  }
  #endregion
}
