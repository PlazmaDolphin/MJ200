using UnityEngine;

public class GameUIScreen : UIScreen
{
    [Space(20f), Header("Game States")]
    [SerializeField] private GameState targetGameState;
    [SerializeField] private Tolerance pausedTolerance = Tolerance.Whatever;

    private enum Tolerance
    {
        Whatever,
        True,
        False,
    }

    protected override void Start()
    {
        CheckVisibility(true);
    }

    protected override void Awake()
    {
        base.Awake();

        GameStateManager.Instance.OnGameStateChanged += (_) => CheckVisibility();
        GameStateManager.Instance.OnPausedStateChanged += (_) => CheckVisibility();
    }

    protected virtual void OnDestroy()
    {
        if (!GameStateManager.HasInstance) return;

        GameStateManager.Instance.OnGameStateChanged -= (_) => CheckVisibility();
        GameStateManager.Instance.OnPausedStateChanged -= (_) => CheckVisibility();
    }

    private void CheckVisibility(bool isImmediate = false)
    {
        bool canShow = CanShowGameScreen();

        if (!isImmediate && canShow == IsVisible)
        {
            return;
        }

        if (isImmediate)
        {
            SetImmediately(canShow);
        }
        else
        {
            SetWithTransition(canShow);
        }
    }

    protected virtual bool CanShowGameScreen()
    {
        if (!GameStateManager.HasInstance)
        {
            Debug.LogError($"{gameObject.name} game UI screen trying to acess null game state manager");
            return false;
        }

        if (targetGameState != null && GameStateManager.Instance.GetGameState() != targetGameState)
            return false;

        switch (pausedTolerance)
        {
            case Tolerance.True:
                return GameStateManager.Instance.GetIsPaused();
            case Tolerance.False:
                return !GameStateManager.Instance.GetIsPaused();
            case Tolerance.Whatever:
                break;
        }

        return true;
    }
}