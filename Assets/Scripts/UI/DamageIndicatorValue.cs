using UnityEngine;
using TMPro;

public class DamageIndicatorValue : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Animator _animator;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
        _text.text = "바보";
    }

    public void SetDamage(int damageAmount, bool isCritical)
    {
        if (_text != null)
        {
            if (isCritical)
            {
                //_text.color = Color.red;
                _text.text = $"CRIT {damageAmount}!";
            }
            else
            {
                //_text.color = Color.white;
                _text.text = damageAmount.ToString();
            }
        }

        if (_animator != null)
        {
            if (isCritical)
                _animator.SetTrigger("CRIT");  // 크리티컬 전용 트리거 발동
            else
                _animator.SetTrigger("Normal");    // 일반 데미지 트리거 발동
        }
    }

    public void OnAnimationEnd()
    {
        if (transform.parent != null)
            Destroy(transform.parent.gameObject);
        else
            Destroy(gameObject);
    }
}
