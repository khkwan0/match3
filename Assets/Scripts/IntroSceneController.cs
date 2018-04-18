using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSceneController : MonoBehaviour {

    public GameObject flag;
    public GameObject monkey;
    public GameObject monkey_uhoh;
    public GameObject particles;
    public GameObject startButton;
    public GameObject sun;
    public GameObject sunAngry;
    public GameObject elephant;
    public GameObject elephant_uhoh;

	// Use this for initialization
	void Start () {
        //GameController.GetGameController().BackToWorld();
        StartCoroutine(DoSequence());
        startButton.transform.Find("Button").GetComponent<Button>().onClick.AddListener(HandleStart);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator DoSequence()
    {

        if (GameController.GetGameController().PlayerSeenIntro() == 1)
        {
            startButton.SetActive(true);
        }
        yield return new WaitForSeconds(2);

        // raise the flag
        while (flag.transform.position.y < .8f)
        {
            flag.transform.position += new Vector3(0f, 0.05f, 0f);
            yield return null;
        }
        GameController.GetGameController().PlaySpringInMyStep();
        // pause
        yield return new WaitForSeconds(2);
        Vector3 monkeyOriginalPosition = monkey.transform.position;
        monkey.GetComponent<Rigidbody>().velocity = new Vector3(0f, 10f, 0f);

        // jump the monkey
        while (monkey.transform.position.y != monkeyOriginalPosition.y)
        {
            yield return null;
        }

        //pause
        yield return new WaitForSeconds(3);

        // shake the sun
        float timeLapsed = 0f;
        float shakeTime = 5f;
        Vector3 sunPos = sun.transform.position;
        float shakeMagnitude = 0f;
        while (timeLapsed < shakeTime)
        {
            shakeMagnitude = timeLapsed / shakeTime;

            sun.transform.position += Random.insideUnitSphere * shakeMagnitude * 0.1f;
            sun.transform.position = new Vector3(sun.transform.position.x, sun.transform.position.y, sunPos.z);
            timeLapsed += Time.deltaTime;
            yield return null;
        }
        sun.transform.position = sunPos;
        sun.SetActive(false);
        sunAngry.SetActive(true);
        GameController.GetGameController().PlayCreep();
        yield return new WaitForSeconds(2);
        monkey.SetActive(false);
        monkey_uhoh.SetActive(true);
        elephant.SetActive(false);
        elephant_uhoh.SetActive(true);
        yield return new WaitForSeconds(2);
        particles.SetActive(true);
        if (GameController.GetGameController().PlayerSeenIntro() == 0)
        {
            yield return new WaitForSeconds(3);
            startButton.SetActive(true);
            Vector3 originalPos = startButton.transform.position;
            startButton.transform.position = new Vector3(startButton.transform.position.x, startButton.transform.position.y + 10f, 13f);
            Vector3 startPos = startButton.transform.position;
            timeLapsed = 0f;
            float buttonAppearTime = 5f;
            float perc = 0f;
            while (timeLapsed < buttonAppearTime)
            {
                perc = timeLapsed / buttonAppearTime;
                startButton.transform.position = Vector3.Lerp(startPos, originalPos, perc);
                timeLapsed += Time.deltaTime;
                yield return null;
            }

            GameController.GetGameController().SetPlayerSeenIntro(1);
        }
    }

    private void HandleStart()
    {
        GameController.GetGameController().IntermediateScreen();
    }
}
