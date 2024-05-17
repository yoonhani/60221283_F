using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Standard Value")]
	[SerializeField] int _nTime;                    // 게임 진행 시간 설정
	[SerializeField] int _nTargetCount;
	
	public struct st_History
    {
		public string sName;
		public int nEventTime;
		public int nPoint;
    }

	PageBattle _pageBattle;
	GameObject _goTarget;

	st_History _stHistory;
	List<st_History> _liHistory = new List<st_History>();

	public TargetController _targetController;

	int _nTotalScore = 0;

    private void Awake()
    {
		_targetController = FindObjectOfType<TargetController>();
		_pageBattle = FindObjectOfType<PageBattle>();
	}

    private void Start()
    {
		AddScore();
	}

    public int GetPlayTime()
    {
		return _nTime;
    }

	public void SetTarget()
    {
		_goTarget = _targetController.ResetTarget(_nTargetCount);
		_pageBattle.SetRenderTexture(true);
		_pageBattle.SetModelCamera();
	}

	void ClearTarget()
    {
		_targetController.ClearTargets();
	}

	public int GetTotalScore()
    {
		return _nTotalScore;
    }

	public void AddScore(int score = 0)
	{
		_nTotalScore += score;

		if ( score > 0 )
        {
			_stHistory.sName = GetTarget().GetComponent<NPCController>()?.GetName();
			_stHistory.nEventTime = _pageBattle.GetElapedTime();
			_stHistory.nPoint = score;
			_liHistory.Add(_stHistory);
		}

		_pageBattle.AddScore(_nTotalScore);
		_pageBattle.SetRenderTexture(false);

		ClearTarget();

		StartCoroutine(_pageBattle.StartCounter(5, SetTarget));
    }

	public GameObject GetTarget()
    {
		return _goTarget;
    }

	public int GetTargetCount()
    {
		return _nTargetCount;
    }

	public List<st_History> GetHistory()
    {
		return _liHistory;
    }
}
