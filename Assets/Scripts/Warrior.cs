using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// 전사 캐릭 제어
/// </summary>
public class Warrior : MonoBehaviour
{
  #region Variables
  public float speed = 4f;
  public float rSpeed = 540f;
  public float attackRange = 7f;
  public int attackPower = 1;
  public Transform aimPosition;

  public GameObject nozzleFlash;
  public AudioClip attackClip;
  public AudioClip footstepClip;

  private Animator anim;
  private Rigidbody rb;
  private AudioSource source;
  private Vector3 targetPosition;
  private Quaternion targetDirection;
  private bool isShooting = false;

  private IDamagable targetEnemy;
  #endregion

  #region Public Method
  /// <summary>
  /// 전사의 목표 위치 혹은 공격대상 지정
  /// </summary>
  /// <param name="position">이동 위치</param>
  /// <param name="target">공격 대상</param>
  public void SetTargetPosition(Vector3 position, IDamagable target = null)
  {
    //타겟Y축 높이와 캐릭터의 Y축 높이를 동일하게 맞춰줌.
    position.y = transform.position.y;
    targetPosition = position;

    //Y축이 같기 때문에 회전시킬 때, y축 기준으로만 회전함.
    targetDirection = Quaternion.LookRotation((targetPosition - transform.position).normalized);
    
    //공격대상 적용.
    targetEnemy = target;
  } 

  /// <summary>
  /// 공격 이벤트. 애니메이션 클립에 이벤트 등록되어 있음.
  /// </summary>
  public void Attack()
  {
    if (!isShooting)
      return;

    //공격시 파티클 효과와 사운드를 발생.
    nozzleFlash.SetActive(true);
    source.PlayOneShot(attackClip);


    //총구위치에서 총구가 향하는 방향으로 레이캐스팅. 공격대상 속으로 총구가 들어갈 정도의 가까운 거리가
    //에서는 적중되지 않는 문제가 생김. 그러나 그런 예외처리까지 하게 되면 너무 복잡해지므로 여기서는 다루지 않음.
    //공격대상을 이미 알고 있는대도 레이캐스팅을 하는 이유는 타겟과 일직선상 사이에 방해물이 있는지 확인하는 절차임.
    if(Physics.Raycast(aimPosition.position, aimPosition.forward, out RaycastHit hit, attackRange))
    {
      //데미지를 줄 수 있는 객체인지 확인함.
      if(hit.collider.TryGetComponent<IDamagable>(out IDamagable damagable))
      {
        //히트된 객체에 데미지를 주고 파괴까지 성공하면 true를 리턴받음
        if (damagable.TakeDamage(attackPower, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up)))
        {
          //파괴시킨 객체가 공격대상과 일치하면
          if(targetEnemy == damagable)
          {
            //타겟팅된 공격대상을 비우고 공격상태를 중지함.
            targetEnemy = null;
            anim.SetBool("IsShooting", false);
            targetPosition = transform.position;
          }
        }
      }      
    }
    else
    {
      //공격이 빗나가거나 데미지를 줄 수 없는 객체에 맞았을 경우
      Debug.Log("Miss");
    }
  }

  /// <summary>
  /// 뛰는 동작에서 발이 땅에 닿는 애니메이션 키프레임에 이벤트로 등록되어 있음
  /// </summary>
  public void Footstep()
  {
    //발동작에 맞춰 발소리가 나게 함.
    source.PlayOneShot(footstepClip);
  }
  #endregion

  #region Unity Method
  //객체가 생성되고 활성화시 처음 한번만 호출. Awake, OnEnable보다 늦게 호출됨.
  void Start()
  {
    anim = GetComponent<Animator>();
    rb = GetComponent<Rigidbody>();
    source = GetComponent<AudioSource>();
  }
  //객체가 활성화 될 때마다 호출됨
  private void OnEnable()
  {
    //전사 캐릭터를 재사용할 수 있게 활성화할 때마다 체력과 타겟 위치를 초기화해줌.
    targetPosition = transform.position;
    targetDirection = transform.rotation;
    //스테이지 매니저에 초기화 요청.
    StageManager.Get.Reset();
  }

  //물리업데이트 주기마다 호출
  private void FixedUpdate()
  {
    if (isShooting)
      return;

    //타겟 위치로 초당 스피드 기준으로 이동된 위치를 계산해 위치 갱신
    Vector3 position = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.deltaTime);
    rb.MovePosition(position);

    //만약 타겟과의 거리가 순간 이동거리보다 적으면 도착한 것으로 간주하고 애니메이션 정지

    //if((targetPosition - position).magnitude < speed *  Time.deltaTime) 마지막 수업때 사용한 코드
    if (Vector3.Distance(targetPosition, position) < float.Epsilon)
      anim.SetFloat("Speed", 0f);
    else
      anim.SetFloat("Speed", speed / 4f);
  }

  //매프레임마다 호출
  private void Update()
  {
    //공격대상이 존재하면 
    if (targetEnemy != null)
    {
      //공격 대상의 위치와 방향을 갱신해 회전
      Vector3 pos = targetEnemy.GetCenterPosition();
      pos.y = transform.position.y;
      targetPosition = pos;
      targetDirection = Quaternion.LookRotation((pos - transform.position).normalized, Vector3.up);
    }

    //타겟 방향으로 초당 회전속도 기준으로 회전시킴
    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDirection, rSpeed * Time.deltaTime);

    if (targetEnemy != null && (targetEnemy.GetCenterPosition() - transform.position).magnitude < attackRange)
    {
      anim.SetBool("IsShooting", isShooting = true);
      anim.SetFloat("Speed", 0f);
    }
    else
    {
      anim.SetBool("IsShooting", isShooting = false);
    }
  }
  #endregion
}
