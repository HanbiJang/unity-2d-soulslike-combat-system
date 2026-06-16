using System.Collections.Generic;
using UnityEngine;

public enum BufferableInput { Attack, Jump, Dash }

// 플레이어 입력을 짧은 시간 동안 보관했다가 상태가 받아줄 수 있을 때 꺼내 씀.
// 공격 중 점프를 누르면 공격이 끝난 뒤 자동으로 점프가 실행되는 식.
public class InputBuffer
{
    public const float AttackWindow = 0.2f;  // 콤보 이음에 여유를 줌
    public const float JumpWindow   = 0.15f;
    public const float DashWindow   = 0.15f;

    private struct Entry
    {
        public BufferableInput type;
        public float time;
    }

    // 최대 2개. 더 많으면 조작이 밀리는 느낌
    private const int MaxSize = 2;
    private readonly List<Entry> _entries = new List<Entry>(MaxSize);

    // 입력 추가. 같은 타입이 이미 있으면 타임스탬프만 갱신
    public void Add(BufferableInput type)
    {
        for (int i = 0; i < _entries.Count; i++)
        {
            if (_entries[i].type != type) continue;
            _entries[i] = new Entry { type = type, time = Time.time };
            return;
        }

        if (_entries.Count >= MaxSize) _entries.RemoveAt(0);
        _entries.Add(new Entry { type = type, time = Time.time });
    }

    // 해당 타입의 유효한 입력이 있으면 소비하고 true 반환
    public bool Consume(BufferableInput type, float window)
    {
        float now = Time.time;
        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            if (e.type != type || now - e.time > window) continue;
            _entries.RemoveAt(i);
            return true;
        }
        return false;
    }

    public void Clear() => _entries.Clear();
}
