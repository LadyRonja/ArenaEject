using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasicButton : MonoBehaviour
{
    public enum ButtonType { UNASSIGNED, START, OPEN_JOIN, CREDITS, OPTIONS, QUIT };
    [SerializeField] private ButtonType buttonType = ButtonType.UNASSIGNED;
    public ButtonType MyType { get => buttonType; }

    private void Start()
    {
        if (TryGetComponent<Button>(out Button btn))
        {
            btn.onClick.RemoveAllListeners();
            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>();

            switch (buttonType)
            {
                case ButtonType.UNASSIGNED:
                    {
                        Debug.LogError("Basic Button Unassigned");
                        break;
                    }
                case ButtonType.START:
                    {
                        btn.onClick.AddListener(StartScreenManager.Instance.StartGame);
                        if (txt != null)
                        {

                            txt.text = "Start Game";
                            btn.interactable = false;
                        }
                        break;
                    }
                case ButtonType.OPEN_JOIN:
                    {
                        btn.onClick.AddListener(StartScreenManager.Instance.GoToJoin);
                        if (txt != null)
                        {

                            txt.text = "Play";
                        }
                        break;
                    }
                case ButtonType.CREDITS:
                    {
                        btn.onClick.AddListener(StartScreenManager.Instance.GoToCredits);
                        if (txt != null)
                        {

                            txt.text = "Credits";
                        }
                        break;
                    }
                case ButtonType.OPTIONS:
                    {
                        btn.onClick.AddListener(StartScreenManager.Instance.GoToOptions);
                        if (txt != null)
                        {

                            txt.text = "Options";
                        }
                        break;
                    }
                case ButtonType.QUIT:
                    {
                        btn.onClick.AddListener(QuitGame);
                        if (txt != null) {

                            txt.text = "Quit";
                        }
                        break;
                    }
                default:
                    {
                        Debug.LogError("End of Switch statement reached, forgot to add new case?");
                        break;
                    }
            }


        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
