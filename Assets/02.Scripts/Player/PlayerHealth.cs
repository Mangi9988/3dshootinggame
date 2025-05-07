using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStatData _statData;
    
    private float _currentHealth;
    private bool _isDamaged = false;
    public float DamagedTime = 0.5f;
    
    [SerializeField] private Image bloodImage;
    [SerializeField] private float _bloodFadeSpeed = 1.5f;
    
    private void Start()
    {
        _currentHealth = _statData.MaxHealth;
        
        Color color = bloodImage.color;
        color.a = 0f;
        bloodImage.color = color;
    }
    
    public void TakeDamage(Damage damage)
    {
        if(_isDamaged)
        {
            return;
        }   
        
        _currentHealth -= damage.Value;
        if (_currentHealth <= 0)
        { 
            gameObject.SetActive(false);
            GameManager.Instance.GameOver();
        }

        StartCoroutine(Damaged_Coroutine());
        StartCoroutine(BloodEffect_Coroutine());
    }

    private IEnumerator Damaged_Coroutine()
    {
        _isDamaged = true;
        yield return new WaitForSeconds(DamagedTime);
        _isDamaged = false;
    }
    
    private IEnumerator BloodEffect_Coroutine()
    {
        // 1. 혈흔을 완전히 보이게
        Color color = bloodImage.color;
        color.a = 1f;
        bloodImage.color = color;


        // 2. 점점 알파값을 줄인다
        while (bloodImage.color.a > 0f)
        {
            color.a -= Time.deltaTime * _bloodFadeSpeed;
            bloodImage.color = color;
            yield return null;
        }

        // 3. 완전히 사라졌으면 알파값 0으로 고정
        color.a = 0f;
        bloodImage.color = color;
    }
    
    public float GetHealthRatio()
    {
        return _currentHealth / _statData.MaxHealth;
    }
}
