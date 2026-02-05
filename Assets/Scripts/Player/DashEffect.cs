using System.Collections;
using UnityEngine;

public class DashEffect : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem dashParticles;
    [SerializeField] private float trailDuration = 0.3f;
    
    private void Start()
    {
        if (trail == null)
            trail = GetComponent<TrailRenderer>();
        
        if (trail != null)
            trail.emitting = false;
    }

    public void PlayDashEffect()
    {
        // 启用拖尾效果
        if (trail != null)
        {
            trail.emitting = true;
            StartCoroutine(StopTrail());
        }
        
        // 播放粒子效果
        if (dashParticles != null)
        {
            dashParticles.Play();
        }
    }

    private IEnumerator StopTrail()
    {
        yield return new WaitForSeconds(trailDuration);
        
        if (trail != null)
            trail.emitting = false;
    }
}
