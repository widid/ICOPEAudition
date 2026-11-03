using System.Collections.Generic;
using ICOPE.Immersive.Progress;
using Managers;
using PatientData;
using TMPro;
using UI.ScoreContents;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LevelSelector{
    /// <summary>
    /// Manages level selection, patient case buttons, and game start logic in the level selection menu.
    /// </summary>
    /// <remarks>
    /// Handles UI elements for selecting levels and patients, including dynamically instantiating buttons,
    /// updating image, and launching the game with the selected configuration.
    /// </remarks>
    public class LevelSelector : MonoBehaviour{
        // buttons, patient name and descriptions per levels
        /// <summary>
        /// Container for each level's difficulty button.
        /// </summary>
        [SerializeField, Header("Ui elements")]
        private GameObject levelSelector; // Use to desactivate levelSelector.

        [SerializeField] private Image imageVisualisation; // visualisation perso ui.

        [SerializeField] private GameObject[]
            patientsList; // Uis to desactive when tuto selected (scroll view gameObject + Separator gameObject).

        [Header("Button")] [SerializeField] private GameObject levelContainer;

        [SerializeField] private GameObject patientContainer;
        [SerializeField] private GameObject buttonPref;
        [SerializeField] private Button startGameButton;

        private List<ButtonPressDetector> _levelCompos;
        private readonly List<ButtonPressDetector> _patientButtons = new();

        [HideInInspector] public LevelsData levelsData;
        private int _currentIndexLevelSelected;
        private int _currentIndexPatientCase;

        private bool _init;

        private void OnEnable(){
            if (!_init) {
                _init = true;

                // add listerner to stats button
                startGameButton.onClick.AddListener(LoadPatientCase);

                ActualizeLevelList();
            }

            GameStateManager gameState = GameManager.Instance.GameStateManager;
            ShowSelection(gameState.GetCurrentLevel(), gameState.GetCurrentPatientCase());
        }

        /// <summary>
        /// Selects the last unlocked patient across every level and sets it in the game state.
        /// </summary>
        public void SelectLastUnlockedPatient(){
            levelsData = GameManager.Instance.levelsData;
            IPatientProgressProvider progress = GameManager.Instance.PatientProgress;
            _currentIndexLevelSelected = 0;
            _currentIndexPatientCase = 0;

            for (int level = 0; level < levelsData.patientByLevel.Count; level++) {
                List<PatientData.PatientData> patients = levelsData.patientByLevel[level].patientsCase;

                for (int patient = 0; patient < patients.Count; patient++) {
                    if (!progress.IsUnlocked(patients[patient].caseId)) continue;

                    _currentIndexLevelSelected = level;
                    _currentIndexPatientCase = patient;
                }
            }

            ApplySelection();
        }

        public void ActualizeLevelList(){
            foreach (Transform child in levelContainer.transform) {
                Destroy(child.gameObject);
            }

            levelsData = GameManager.Instance.levelsData;
            _levelCompos = new List<ButtonPressDetector>();
            int index = 0;
            foreach (PatientCaseLevel pcl in levelsData.patientByLevel) {
                ButtonPressDetector bpd = Instantiate(buttonPref, levelContainer.transform)
                    .GetComponent<ButtonPressDetector>();
                bpd.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = pcl.name;
                int i = index;
                bpd.onPress.AddListener(delegate{ ChangeLevel(i); });
                index++;
                _levelCompos.Add(bpd);
            }
        }

        /// <summary>
        /// Activates or deactivates patient-related UI elements (e.g., scroll view and separators).
        /// </summary>
        /// <param name="active">Whether to enable or disable the patient UI elements.</param>
        private void ActivePatientUiElements(bool active){
            foreach (GameObject patientElem in patientsList) {
                patientElem.SetActive(active);
            }
        }

        /// <summary>
        /// Changes the selected level and updates UI elements accordingly, such as patient buttons and description.
        /// </summary>
        /// <param name="index">Index of the selected level.</param>
        private void ChangeLevel(int index){
            ShowSelection(index, 0);
        }

        /// <summary>
        /// Shows the patients of a level with one of them selected.
        /// </summary>
        private void ShowSelection(int level, int patient){
            _currentIndexLevelSelected = level;

            // assign current description and patient buttons
            ShowPatientButtons(levelsData.patientByLevel[level].patientsCase.Count, level);
            ActivePatientUiElements(true);
            ChangeImage(patient);
            ButtonsManager.SetButtonFocused(_levelCompos[level]);
            ButtonsManager.SetButtonFocused(_patientButtons[patient]);
        }

        /// <summary>
        /// Instantiates and configures patient selection buttons for the selected level.
        /// </summary>
        /// <param name="nbPatient">Number of patient cases to display for the selected level.</param>
        /// <param name="level">Index of the selected level.</param>
        private void ShowPatientButtons(int nbPatient, int level){
            foreach (Transform child in patientContainer.transform) {
                Destroy(child.gameObject);
            }

            IPatientProgressProvider progress = GameManager.Instance.PatientProgress;
            _patientButtons.Clear();

            // Get nb patient case
            for (int i = 0; i < nbPatient; i++) {
                GameObject go = Instantiate(buttonPref, patientContainer.transform);
                _patientButtons.Add(go.GetComponent<ButtonPressDetector>());

                PatientData.PatientData patient = levelsData.patientByLevel[level].patientsCase[i];
                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = patient.firstName;

                var i1 = i;
                go.GetComponent<ButtonPressDetector>().onPress.AddListener(delegate{ ChangeImage(i1); });
                go.gameObject.SetActive(true);

                if (!progress.IsUnlocked(patient.caseId))
                    go.GetComponent<ButtonPressDetector>().AssignState(ButtonPressDetector.ButtonState.Disable);
                else
                    go.GetComponent<ButtonPressDetector>().AssignState(ButtonPressDetector.ButtonState.None, true);
            }
        }

        /// <summary>
        /// Updates the description text for the currently selected patient case.
        /// </summary>
        /// <param name="indexPatientCase">Index of the selected patient case within the level.</param>
        private void ChangeImage(int indexPatientCase){
            _currentIndexPatientCase = indexPatientCase;
            imageVisualisation.sprite = levelsData.patientByLevel[_currentIndexLevelSelected].patientsCase[indexPatientCase].characterSprites[0];
        }

        /// <summary>
        /// Visually updates the difficulty button for the current level to reflect its selected state.
        /// </summary>
        public void UpdateLevelButton(){
            int currentLevel = GameManager.Instance.GameStateManager.GetCurrentLevel();
            _levelCompos[currentLevel].AssignState(ButtonPressDetector.ButtonState.None, true);
        }

        /// <summary>
        /// Sets the selected level and patient case in the game state and transitions to the game menu.
        /// </summary>
        // Call on "commencer" button click by player
        private void LoadPatientCase(){
            ApplySelection();
            // Re-load game menus (need to change patient sprite)
            GameManager.Instance.LoadGameMenu();
        }

        /// <summary>
        /// Sets the selected level and patient case in the game state.
        /// </summary>
        private void ApplySelection(){
            GameManager.Instance.GameStateManager.SetLevel(_currentIndexLevelSelected);
            GameManager.Instance.GameStateManager.SetPatientCase(_currentIndexPatientCase,
                levelsData.patientByLevel[_currentIndexLevelSelected].patientsCase[_currentIndexPatientCase]);
        }
    }
}