using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// 마우스의 클릭위치를 파악해 파티클로 위치를 표시하고 전사에게 정보 제공.
/// </summary>
public class Picker : MonoBehaviour
{
  #region Variables
  public LayerMask mask;
  public GameObject pickEffect;

  private InputAction moveAction;
  private InputAction positionAction;
  private Warrior warrior;
  private bool prevPressed;
  #endregion

  #region Unity Method
  //객체화되고 활성화시 맨 처음 한번만 호출됨.
  private void Awake()
  {
    moveAction = InputSystem.actions.FindAction("Move");
    positionAction = InputSystem.actions.FindAction("ScreenPosition");
    warrior = GetComponent<Warrior>();
  }

  //매 프레임마다 호출. 실시간으로 업데이트하고 싶은 코드를 넣을 수 있음.
  private void Update()
  {
    //이벤트시스템 컴포넌트가 씬에 존재하고 마우스 포인터가 UI오브젝트 위에 있다면 클릭을 무시해라.
    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
      return;


    //Move액션에 할당된 키가 눌러져 있는지 확인
    bool press = moveAction.IsPressed();
    //누르고 있는 동안에는 
    if (!prevPressed && press)
    {
      Vector2 position = positionAction.ReadValue<Vector2>();
      Ray ray = Camera.main.ScreenPointToRay(position);

      //마지막 강의때 제가 매개변수를 잘못 넣어서 일부 오작동하는 현상이 있어 학생들에게 수정된 코드로 다시 알려주시기 바랍니다.
      //if (Physics.Raycast(ray, out RaycastHit hit, mask)) 기존에 학생들에게 알려줬던 코드
      if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
      {  
        IDamagable damagable = hit.collider.GetComponent<IDamagable>();
        warrior.SetTargetPosition(hit.point, damagable);
        
        if (damagable == null)
          Instantiate<GameObject>(pickEffect, hit.point, pickEffect.transform.rotation);
      }
    }

    prevPressed = press;
  }
  #endregion
}
