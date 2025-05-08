using System;
using UnityEngine;

public class UI_Popup : MonoBehaviour
{
    private Action _closeCallback;
    
    public void Open(Action closeCallback = null) // 팝업 열려! 근데 너 닫힐 때 호출할 함수좀 등록할게
    {
        _closeCallback = closeCallback;
        
        gameObject.SetActive(true);
    }

    public void Close()
    {
        _closeCallback?.Invoke(); // 나 닫힌다, 어 닫힐때 호출할 함수가 등록되어있네? 실행해야지
        
        gameObject.SetActive(false);
    }
}
