public enum SoundType
{
    // 플레이어 사운드
    PlayerFootstep,      // 발소리
    PlayerJump,          // 점프 소리
    PlayerDash,          // 대시 소리
    PlayerDeath,         // 죽을 때 음성
    PlayerHit,           // 맞을 때 음성
    
    // 적/보스 사운드
    EnemyAttack,         // 적 공격 소리 (무기 휘두르는 소리)
    EnemyRush,           // 적 돌진 소리
    EnemyHit,            // 적이 맞을 때 음성
    EnemyDeath,          // 적이 죽을 때 음성
    EnemyEnrage,         // 보스 각성 (2페이즈) 음성
    
    // 무기 사운드
    WeaponSwing,        // 무기 휘두르는 소리 (타격하지 않은 경우)
    WeaponHitFlesh,     // 타격 소리 - 살/육체
    WeaponHitMetal,     // 타격 소리 - 금속
    WeaponHitWood,      // 타격 소리 - 나무
    WeaponHitStone,     // 타격 소리 - 돌
    
    // UI 사운드
    UIButtonClick,      // 버튼 클릭
    UIButtonHover,      // 버튼 호버
    UIMenuOpen,         // 메뉴 열기
    UIMenuClose,        // 메뉴 닫기
    
    // 배경 사운드
    BackgroundMusic,    // 배경음악
    AmbientSound        // 환경음
}

