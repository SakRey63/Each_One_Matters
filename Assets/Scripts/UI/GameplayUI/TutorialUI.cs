using TMPro;
using UnityEngine;
using YG;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private MouseTutorial _mouseTutorial;
    [SerializeField] private SwipeTutorial _swipeTutorial;

    public void PlayGameGuide()
    {
        if (YG2.envir.isDesktop == false)
        {
            _swipeTutorial.gameObject.SetActive(true);
        }
        else
        {
            _mouseTutorial.gameObject.SetActive(true);
        }
    }

    public void CloseGameGuide()
    {
        _swipeTutorial.gameObject.SetActive(false);
        _mouseTutorial.gameObject.SetActive(false);
    }
}