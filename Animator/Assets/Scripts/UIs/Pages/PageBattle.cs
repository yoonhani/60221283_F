using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public partial class PageBattle : MonoBehaviour
{
	[Header("Page Root")]
	[SerializeField] GameObject _goPlay;			// 게임 진행 중 사용할 ui
	[SerializeField] GameObject _goResult;			// 결과 ui

	[Header("Play UIs")]
	[SerializeField] GameObject _goTarget, _goScore, _goRenderTexture;
	[SerializeField] TextMeshProUGUI _txtTimer, _txtScore, _txtCounter;
	[SerializeField] Image _imgTimeGauge;
	[SerializeField] Camera _camModelCamera;
	[SerializeField] Animator _animatorCounter;
	[SerializeField] float _fModelCameraDistance;

	[Header("Result UIs")]
	[SerializeField] Transform _tHistoryRoot;
	[SerializeField] TextMeshProUGUI _txtResultScore;
	[SerializeField] GameObject _goSlotBoard;

	public enum GameState
    {
		Play,
		Result,
    }

	GameState _gameState;

	GameManager _gameManager = null;
	Coroutine _coTimer = null;

	int _nTime, _nRemainTime;

	void Awake()
	{
		SetGameState(GameState.Play);
		_txtCounter.gameObject.SetActive(false);
	}

	void Initialize()
    {
		if ( null == _gameManager ) _gameManager = FindObjectOfType<GameManager>();
		_nTime = _gameManager.GetPlayTime();

		Time.timeScale = _gameState == GameState.Play ? 1f : 0f;

		_goPlay.SetActive(_gameState == GameState.Play);
		_goResult.SetActive(_gameState == GameState.Result);

		_nRemainTime = 0;
	
		if ( GameState.Play == _gameState )
			_coTimer = StartCoroutine(StartTimer());
		else if ( GameState.Result == _gameState )
			SetResult();

	}

	void SetResult()
    {
		GameObject slot = default;
		List<GameManager.st_History> _stHistory = _gameManager.GetHistory();

		ComUtil.DestroyChildren(_tHistoryRoot);

		_txtResultScore.text = $"Score : {_gameManager.GetTotalScore()}";

		for ( int i = 0; i < _stHistory.Count; i++ )
        {
			slot = Instantiate(_goSlotBoard);
			slot.transform.SetParent(_tHistoryRoot, false);
			slot.GetComponent<SlotBoard>().SetName(_stHistory[i].sName);
			slot.GetComponent<SlotBoard>().SetTime(_stHistory[i].nEventTime);
			slot.GetComponent<SlotBoard>().SetScore(_stHistory[i].nPoint);
		}
	}

	public int GetElapedTime()
    {
		return _nTime - _nRemainTime;
    }

	IEnumerator StartTimer()
    {
		int time;
		WaitForSecondsRealtime onesecond = new WaitForSecondsRealtime(1f);

		while ( _nTime >= _nRemainTime )
        {
			time = _nTime - _nRemainTime;

			_txtTimer.text = time.ToString();
			_imgTimeGauge.fillAmount = (float)time / (float)_nTime;

			yield return onesecond;
			_nRemainTime += 1;
        }

		SetGameState(GameState.Result);
	}

	public IEnumerator StartCounter(int time, Action callback = null)
    {
		WaitForSecondsRealtime onesecond = new WaitForSecondsRealtime(1f);

		_txtCounter.gameObject.SetActive(true);
		
		while (time >= 0)
		{
			_animatorCounter.SetTrigger("Pump");
			_txtCounter.text = time.ToString();
			time -= 1;

			yield return onesecond;
			_animatorCounter.ResetTrigger("Pump");
		}

		_txtCounter.gameObject.SetActive(false);

		callback?.Invoke();
	}

	void SetGameState(GameState state)
    {
		_gameState = state;
		Initialize();
    }

	public void SetModelCamera()
    {
		Vector3 directionNormalize = (UnityEngine.Random.insideUnitSphere * 2f - Vector3.one).normalized;
		directionNormalize.y = Mathf.Abs(directionNormalize.y);

		Vector3 root = _gameManager.GetTarget().transform.position + Vector3.up * 1.5f;
		_camModelCamera.transform.position = root + directionNormalize * _fModelCameraDistance; ;
		_camModelCamera.transform.LookAt(root);
	}

	public void AddScore(int score)
    {
		_txtScore.text = $"Score : {score}";
	}

	public void SetRenderTexture(bool state)
    {
		_goRenderTexture.SetActive(state);
	}

	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
