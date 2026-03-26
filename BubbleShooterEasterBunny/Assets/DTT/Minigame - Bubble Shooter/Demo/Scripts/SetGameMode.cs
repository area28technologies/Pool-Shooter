using A28.PlatformSdk;
using UnityEngine;
using UnityEngine.UI;

public class SetGameMode : MonoBehaviour
{
    [SerializeField] private Button modeFreeBtn;
    [SerializeField] private Button modeLeaderboardBtn;
    
    private void OnEnable()
    {
        modeFreeBtn.onClick.AddListener(SetModeFree);
        modeLeaderboardBtn.onClick.AddListener(SetModeLeaderboard);
    }

    private void OnDisable()
    {
        modeFreeBtn.onClick.RemoveListener(SetModeFree);
        modeLeaderboardBtn.onClick.RemoveListener(SetModeLeaderboard);
    }

    private void SetModeFree()
    {
        A28Sdk.Instance.GameMode = GameMode.Free;
       
    }
    private void SetModeLeaderboard()
    {
        A28Sdk.Instance.GameMode = GameMode.Leaderboard;
    }
}
