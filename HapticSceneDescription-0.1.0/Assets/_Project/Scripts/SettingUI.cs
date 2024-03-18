using HapticSceneDescription.Test;
using UnityEngine;

namespace HapticSceneDescription
{
    public class SettingUI : MonoBehaviour
    {
        public void SetInputType(int inputType)
        {
            App.inputType = (PlayerController.InputType)inputType;
        }

        public void SetOutputType(int outputType)
        {
            App.outputType = (App.OutputType)outputType;
        }

        public void ConfirmAndPlayButton()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
        }
    }
}