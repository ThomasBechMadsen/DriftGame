using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerformance : MonoBehaviour {

    public CarController cc;
    public Camera camera;

    public float smallDriftVelocity;
    public float mediumDriftVelocity;
    public float greatDriftVelocity;

    public float smallSpeedVelocity;
    public float mediumSpeedVelocity;
    public float greatSpeedVelocity;

    public float smallDriftMultiplier;
    public float mediumDriftMultiplier;
    public float greatDriftMultiplier;

    public AudioSource musicSource;
    public float maxFOV;
    public float minFOV;

    public float streakTimeout;
    public int totalStreakLevels;


    int currentStreakLevel = 0;
    Coroutine timeoutRoutine;
    Coroutine transitionRoutine;
	
	// Update is called once per frame
	void Update () {
        float driftMultiplier = 1;
        int driftStreakContribution = 0;
        int speedStreakContribution = 0;

        if (cc.sidewaysVel > smallDriftVelocity)
        {
            driftStreakContribution = 1;
            driftMultiplier = smallDriftMultiplier;

            if (cc.sidewaysVel > mediumDriftVelocity)
            {
                driftStreakContribution = 2;
                driftMultiplier = mediumDriftMultiplier;

                if (cc.sidewaysVel > greatDriftVelocity)
                {
                    driftStreakContribution = 3;
                    driftMultiplier = greatDriftMultiplier;
                }
            }
        }

        if (cc.forwardVel > smallSpeedVelocity)
        {
            speedStreakContribution = 1;

            if (cc.forwardVel > mediumSpeedVelocity)
            {
                speedStreakContribution = 2;

                if (cc.forwardVel > greatSpeedVelocity)
                {
                    speedStreakContribution = 3;
                }
            }
        }

        int newStreakLevel = driftStreakContribution + speedStreakContribution;

        //print("StreakLevel: " + currentStreakLevel + " ForwardSpeed: " + forwardVel + " SidewaysSpeed: " + sidewaysVel);
        if (newStreakLevel >= currentStreakLevel)
        {
            if (transitionRoutine != null)
            {
                StopCoroutine(transitionRoutine);
            }
            transitionRoutine = StartCoroutine(transitionStreakLevel(currentStreakLevel, newStreakLevel));
            if (timeoutRoutine != null)
            {
                StopCoroutine(timeoutRoutine);
            }
            timeoutRoutine = StartCoroutine(StreakTimeout());

            currentStreakLevel = newStreakLevel;
        }
    }

    IEnumerator transitionStreakLevel(int oldLevel, int newLevel)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime;

            //Transition music volume
            float musicStartVolume = musicSource.volume;
            musicSource.volume = Mathf.Lerp(musicStartVolume, (float)newLevel / totalStreakLevels, progress);

            //Transition FOV
            float fovStartValue = camera.fieldOfView;
            camera.fieldOfView = Mathf.Lerp(fovStartValue, Mathf.Clamp(maxFOV * (float)newLevel / totalStreakLevels, minFOV, maxFOV), progress);
            

            if (newLevel > 5)
            {
                cc.rearLights.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = true;
                cc.rearLights.transform.GetChild(1).GetComponent<TrailRenderer>().enabled = true;
            }
            else
            {
                cc.rearLights.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = false;
                cc.rearLights.transform.GetChild(1).GetComponent<TrailRenderer>().enabled = false;
            }

            yield return null;
        }
    }

    IEnumerator StreakTimeout()
    {
        yield return new WaitForSeconds(streakTimeout);
        //print("Streak ended");
        if (currentStreakLevel > 0)
        {
            if (transitionRoutine != null)
            {
                StopCoroutine(transitionRoutine);
            }
            transitionRoutine = StartCoroutine(transitionStreakLevel(currentStreakLevel, currentStreakLevel--));
            StartCoroutine(StreakTimeout());
        }
    }

    private abstract class Level{
        public abstract IEnumerator transitionIn();
        public abstract IEnumerator transitionOut();
    }

    private class Level0 : Level
    {
        public override IEnumerator transitionIn()
        {
            yield return null;
        }

        public override IEnumerator transitionOut()
        {
            yield return null;
        }
    }

    private class Level1 : Level
    {
        public override IEnumerator transitionIn()
        {
            yield return null;
        }

        public override IEnumerator transitionOut()
        {
            yield return null;
        }
    }
}
