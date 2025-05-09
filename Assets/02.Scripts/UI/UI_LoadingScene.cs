using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LoadingScene : MonoBehaviour
{
    // 목표 : 다음 씬을 '비동기 방식'으로 로드하고 싶다.
    // 또한 로딩 진행률을 시각적으로 표현하고 싶다.
    
    // 속성 :
    // -다음 씬 번호(인덱스)
    public int NextSceneIndex = 2;
    
    // -프로그래스 슬라이더바
    public Slider ProgressSlider;
    
    // - 프로그래스 텍스트
    public TextMeshProUGUI ProgressText;

    private void Start()
    {
        StartCoroutine(LoadNextScene_Coroutine());
    }

    private IEnumerator LoadNextScene_Coroutine()
    {
        // 지정된 씬을 비동기로 로드한다
        AsyncOperation ao = SceneManager.LoadSceneAsync(NextSceneIndex);
        ao.allowSceneActivation = false; // 비동기로 로드되는 씬의 모습이 화면에 보이지 않게 한다.

        // 로딩이 되는 동안 계속해서 반복문
        while (ao.isDone == false)
        {
            // 비동기로 실행할 코드를
            Debug.Log(ao.progress); // 0~1
            ProgressSlider.value = ao.progress;
            ProgressText.text = $"{ao.progress * 100f}%";

            if (ao.progress >= 0.9f)
            {
                ao.allowSceneActivation = true;
            }
            
            yield return null;   // 1프레임 대기
        }
    }
}
