using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public string message;

    // 이 함수는 버튼 클릭 시 호출됩니다.
    public void OnButtonClick()
    {
        Debug.Log(message);
    }
}
