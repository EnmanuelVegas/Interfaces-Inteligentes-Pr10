using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace Whisper.Samples
{
  /// <summary>
  /// Record audio clip from microphone and make a transcription.
  /// </summary>
  public class MicrophoneDemo : MonoBehaviour
  {
    public WhisperManager whisper;
    public MicrophoneRecord microphoneRecord;
    public bool streamSegments = true;
    public bool printLanguage = true;

    [Header("Characters")]
    public GameObject Warrior;
    public GameObject Wizard;

    [Header("UI")]
    public Button button;
    public Text buttonText;
    public Text outputText;
    public Text timeText;
    public Dropdown languageDropdown;
    public Toggle translateToggle;
    public Toggle vadToggle;
    public ScrollRect scroll;

    private string _buffer;

    private void Awake()
    {
      whisper.OnNewSegment += OnNewSegment;
      whisper.OnProgress += OnProgressHandler;

      microphoneRecord.OnRecordStop += OnRecordStop;

      button.onClick.AddListener(OnButtonPressed);
      languageDropdown.value = languageDropdown.options
        .FindIndex(op => op.text == whisper.language);
      languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

      translateToggle.isOn = whisper.translateToEnglish;
      translateToggle.onValueChanged.AddListener(OnTranslateChanged);

      vadToggle.isOn = microphoneRecord.vadStop;
      vadToggle.onValueChanged.AddListener(OnVadChanged);
    }

    private void OnVadChanged(bool vadStop)
    {
      microphoneRecord.vadStop = vadStop;
    }

    private void OnButtonPressed()
    {
      if (!microphoneRecord.IsRecording)
      {
        microphoneRecord.StartRecord();
        buttonText.text = "Stop";
      }
      else
      {
        microphoneRecord.StopRecord();
        buttonText.text = "Record";
      }
    }

    private async void OnRecordStop(AudioChunk recordedAudio)
    {
      buttonText.text = "Record";
      _buffer = "";

      var sw = new Stopwatch();
      sw.Start();

      var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
      if (res == null || !outputText)
        return;

      var time = sw.ElapsedMilliseconds;
      var rate = recordedAudio.Length / (time * 0.001f);
      timeText.text = $"Time: {time} ms\nRate: {rate:F1}x";

      var text = res.Result;
      if (printLanguage)
        text += $"\n\nLanguage: {res.Language}";

      // Procesar comandos
      ProcessCommand(res.Result);

      outputText.text = text;
      UiUtils.ScrollDown(scroll);
    }

    private void OnLanguageChanged(int ind)
    {
      var opt = languageDropdown.options[ind];
      whisper.language = opt.text;
    }

    private void OnTranslateChanged(bool translate)
    {
      whisper.translateToEnglish = translate;
    }

    private void OnProgressHandler(int progress)
    {
      if (!timeText)
        return;
      timeText.text = $"Progress: {progress}%";
    }

    private void OnNewSegment(WhisperSegment segment)
    {
      if (!streamSegments || !outputText)
        return;

      _buffer += segment.Text;
      outputText.text = _buffer + "...";
      UiUtils.ScrollDown(scroll);
    }

    private void ProcessCommand(string recognizedText)
    {
      if (string.IsNullOrEmpty(recognizedText))
        return;

      string text = recognizedText.ToLower();
      // Expresiones regulares para detectar comandos
      // Patrón: (hechicero|guerrero)[,\s]+(salta|cambia) - permite comas, puntos y espacios intermedios
      Regex wizardJumpRegex = new Regex(@"\b(hechicero|mago|wizard)[,.\s]+(salta|saltar|jump)\b", RegexOptions.IgnoreCase);
      Regex wizardChangeRegex = new Regex(@"\b(hechicero|mago|wizard)[,.\s]+(cambia|cambiar|change)\b", RegexOptions.IgnoreCase);
      Regex warriorJumpRegex = new Regex(@"\b(guerrero|warrior)[,.\s]+(salta|saltar|jump)\b", RegexOptions.IgnoreCase);
      Regex warriorChangeRegex = new Regex(@"\b(guerrero|warrior)[,.\s]+(cambia|cambiar|change)\b", RegexOptions.IgnoreCase);

      // Verificar comandos del hechicero
      if (wizardJumpRegex.IsMatch(text))
      {
        UnityEngine.Debug.Log("Comando detectado: Hechicero Salta");
        ExecuteWizardJump();
      }
      else if (wizardChangeRegex.IsMatch(text))
      {
        UnityEngine.Debug.Log("Comando detectado: Hechicero Cambia");
        ExecuteWizardChange();
      }
      // Verificar comandos del guerrero
      else if (warriorJumpRegex.IsMatch(text))
      {
        UnityEngine.Debug.Log("Comando detectado: Guerrero Salta");
        ExecuteWarriorJump();
      }
      else if (warriorChangeRegex.IsMatch(text))
      {
        UnityEngine.Debug.Log("Comando detectado: Guerrero Cambia");
        ExecuteWarriorChange();
      }
    }

    private void ExecuteWizardJump()
    {
      if (Wizard != null)
      {
        UnityEngine.Debug.Log("Ejecutando salto del hechicero");
        Rigidbody rb = Wizard.GetComponent<Rigidbody>();
        if (rb != null)
        {
          rb.AddForce(Vector3.up * 500f); // Ajusta la fuerza según necesites
        }
      }
    }

    private void ExecuteWizardChange()
    {
      if (Wizard != null)
      {
        UnityEngine.Debug.Log("Cambiando color del hechicero");
        Transform lichMesh = Wizard.transform.Find("LichMesh");
        if (lichMesh != null)
        {
          Renderer renderer = lichMesh.GetComponent<Renderer>();
          if (renderer != null)
          {
            Color randomColor = new Color(
              Random.Range(0f, 1f),
              Random.Range(0f, 1f),
              Random.Range(0f, 1f)
            );
            renderer.material.color = randomColor;
          }
          else
          {
            UnityEngine.Debug.LogWarning("LichMesh no tiene un componente Renderer");
          }
        }
        else
        {
          UnityEngine.Debug.LogWarning("No se encontró el objeto hijo LichMesh en Wizard");
        }
      }
    }

    private void ExecuteWarriorJump()
    {
      if (Warrior != null)
      {
        UnityEngine.Debug.Log("Ejecutando salto del guerrero");
        Rigidbody rb = Warrior.GetComponent<Rigidbody>();
        if (rb != null)
        {
          rb.AddForce(Vector3.up * 500f); // Ajusta la fuerza según necesites
        }
        else
        {
          UnityEngine.Debug.LogWarning("El Warrior no tiene un componente Rigidbody");
        }
      }
    }

    private void ExecuteWarriorChange()
    {
      if (Warrior != null)
      {
        UnityEngine.Debug.Log("Cambiando color del guerrero");
        Transform footmanMesh = Warrior.transform.Find("FootmanMesh");
        if (footmanMesh != null)
        {
          Renderer renderer = footmanMesh.GetComponent<Renderer>();
          if (renderer != null)
          {
            Color randomColor = new Color(
              Random.Range(0f, 1f),
              Random.Range(0f, 1f),
              Random.Range(0f, 1f)
            );
            renderer.material.color = randomColor;
          }
          else
          {
            UnityEngine.Debug.LogWarning("FootmanMesh no tiene un componente Renderer");
          }
        }
        else
        {
          UnityEngine.Debug.LogWarning("No se encontró el objeto hijo FootmanMesh en Warrior");
        }
      }
    }
  }
}