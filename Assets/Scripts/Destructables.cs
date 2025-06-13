using System.Collections;
using UnityEngine;


/// <summary>
/// 데미지를 주고 파괴할 수 있는 구조물
/// </summary>
public class Destructables : MonoBehaviour, IDamagable
{  
  #region Variables
  public GameObject hitEffect;        //맞을 때 효과
  public GameObject destroyEffect;    //파괴될 때 효과
  public AudioClip hitClip;           //맞을 때 효과음
  public AudioClip destroyClip;       //파괴될 때 효과음
  public Healthbar uiPrefab;          //체력바 프리팹
  public Vector3 uiOffset;            //체력바 위치 오프셋
  public int maxHealth;               //최대 체력

  private int currentHealth;          //현재 체력
  private Healthbar healthbar;        //연결된 체력바
  private AudioSource source;         //사운드 재생 컴포넌트
  #endregion

  #region IDamagable Method
  public Vector3 GetCenterPosition()
  {
    return transform.position;
  }

  public bool TakeDamage(int damage, Vector3 hitPoint, Quaternion hitRotation)
  {
    if (currentHealth == maxHealth && damage > 0)
      healthbar.TurnOn(true);

    //맞은 위치와 방향으로 이펙트 및 사운드 생성
    Instantiate<GameObject>(hitEffect, hitPoint, hitRotation);
    source.PlayOneShot(hitClip);

    //현재 체력을 깍음.
    currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
    healthbar.SetHealthbar((float)currentHealth / maxHealth);
    
    //깍인 체력이 0이 되면
    if (currentHealth == 0)
    {
      //체력바끄기
      healthbar.TurnOn(false);
      //스테이지 매니저에게 자신이 제거되었음을 알림
      StageManager.Get.RemoveTarget(this);
      //파괴 연출 시작
      StartCoroutine(Dying());
      return true;
    }

    return false;
  }
  #endregion

  #region Dead Method
  /// <summary>
  /// 파괴 연출 과정을 담은 코루틴 메소드
  /// </summary>
  /// <returns></returns>
  IEnumerator Dying()
  {
    //파괴효과 파티클 및 사운드 생성
    Instantiate<GameObject>(destroyEffect, transform.position, destroyEffect.transform.rotation);
    source.PlayOneShot(destroyClip);

    yield return null;
    //파괴 연출이 모두 끝나 실제로 파괴시킴
    OnDead();
  }
  /// <summary>
  /// 죽었을 때 실행할 코드
  /// </summary>
  public void OnDead()
  {
    gameObject.SetActive(false);
  }
  #endregion

  #region Unity Method
  //객체화되고 활성화시 맨 처음 한번만 호출
  private void Awake()
  {
    source = GetComponent<AudioSource>();      
    healthbar = GameObject.Instantiate<Healthbar>(uiPrefab);
    healthbar.Initialize(transform, uiOffset);
  }

  //게임오브젝트를 활성화 시킬 때마다 호출
  private void OnEnable()
  {
    currentHealth = maxHealth;
  }
  #endregion
}
