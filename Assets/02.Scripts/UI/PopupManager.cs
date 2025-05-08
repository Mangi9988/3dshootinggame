using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPopupType
{
    UI_OptionPopup,
    UI_CreditPopup
}

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("팝업 UI 참조")]
    public List<UI_Popup> Popups;
    private Stack<UI_Popup> _opendPopups = new Stack<UI_Popup>(); // null은 아니지만 비어있는 리스트
    // 1. 다른 개발자에게 데이터의 끝 부분만 다룬다는 것과 후입선출이라는 것을 명시적으로 알린다.
    // 2. 그 구조가 보인다. + 제한적인 내용만 쓰는 경우네는 편하다.(추상화가 좀 더 높다)
    // 스택(마지막), 큐(앞), 데크(앞, 마지막) -> 리스트(어레이)의 제한적인 버전이다.
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_opendPopups.Count > 0)
            {
                while (true)
                {
                    UI_Popup popup = _opendPopups.Pop();
                    bool opend = popup.isActiveAndEnabled;
                    popup.Close();
                    
                    // 열려있는 팝업을 닫았거나 || 더이상 닫을 팝업이 없으면 탈출
                    if (opend || _opendPopups.Peek() == null)
                    {
                        break;
                    }
                }
            }
            else
            {
                GameManager.Instance.Pause();
            }
        }
    }

    public void Open(EPopupType type, Action closeCallback = null)
    {
        Open(type.ToString(),  closeCallback);
    }
    
    private void Open(string popupName, Action closeCallback)
    {
        foreach (UI_Popup popup in Popups)
        {
            if (popup.gameObject.name == popupName)
            {
                popup.Open(closeCallback);
                // 팝업을 열 때 마다 담는다
                _opendPopups.Push(popup);
                break;
            }
        }
    }
}
