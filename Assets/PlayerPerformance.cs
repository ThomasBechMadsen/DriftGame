using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerformance : MonoBehaviour
{

    public CarController cc;
    public Camera camera;
    public AudioSource musicSource;
    public Transform musicVizualizer;

    public float mediumDriftVelocity;
    public float greatDriftVelocity;

    public float mediumSpeedVelocity;
    public float greatSpeedVelocity;

    public float mediumDriftMultiplier;
    public float greatDriftMultiplier;

    public TrailRenderer leftTrail;
    public TrailRenderer rightTrail;

    public int level2Fov;
    public int normalFov;
    public int poundFov;

    public float streakTimeout;
    public int totalStreakLevels;

    int currentStreakLevel = 0;
    Coroutine timeoutRoutine;
    Coroutine transitionRoutine;

    public Level[] levels = new Level[5];
    private Level0 l0 = new Level0();
    private Level1 l1 = new Level1();
    private Level2 l2 = new Level2();
    private Level3 l3 = new Level3();
    private Level4 l4 = new Level4();

    void Start()
    {
        l0.pp = this;
        l1.pp = this;
        l2.pp = this;
        l3.pp = this;
        l4.pp = this;
        levels[0] = l0;
        levels[1] = l1;
        levels[2] = l2;
        levels[3] = l3;
        levels[4] = l4;

        Color trailC = new Color(leftTrail.startColor.r, leftTrail.startColor.g, leftTrail.startColor.b, 0);
        leftTrail.startColor = trailC;
        rightTrail.startColor = trailC;

        Color musicBarStartColor = musicVizualizer.GetChild(0).GetComponent<MeshRenderer>().material.color;
        for (int i = 0; i < musicVizualizer.childCount; i++)
        {
            Color c = new Color(musicBarStartColor.r, musicBarStartColor.g, musicBarStartColor.b, 0);
            musicVizualizer.GetChild(i).GetComponent<MeshRenderer>().material.color = c;
        }
        musicVizualizer.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float driftMultiplier = 1;
        int driftStreakContribution = 0;
        int speedStreakContribution = 0;

        if (cc.sidewaysVel > mediumDriftVelocity)
        {
            driftStreakContribution = 1;
            driftMultiplier = mediumDriftMultiplier;

            if (cc.sidewaysVel > greatDriftVelocity)
            {
                driftStreakContribution = 2;
                driftMultiplier = greatDriftMultiplier;
            }
        }

        if (cc.forwardVel > mediumSpeedVelocity)
        {
            speedStreakContribution = 1;
            if (cc.forwardVel > greatSpeedVelocity)
            {
                speedStreakContribution = 2;
            }
        }

        int newStreakLevel = driftStreakContribution + speedStreakContribution;

        //print("StreakLevel: " + currentStreakLevel + " ForwardSpeed: " + forwardVel + " SidewaysSpeed: " + sidewaysVel);
        if (newStreakLevel >= currentStreakLevel)
        {
            if (newStreakLevel > currentStreakLevel) {
                transitionLevel(currentStreakLevel, newStreakLevel);
            }
            if (timeoutRoutine != null)
            {
                StopCoroutine(timeoutRoutine);
            }
            timeoutRoutine = StartCoroutine(StreakTimeout());

            currentStreakLevel = newStreakLevel;
        }
    }

    void transitionLevel(int oldLevel, int newLevel)
    {
        if (newLevel > oldLevel)
        {
            for (int i = oldLevel + 1; i <= newLevel; i++)
            {
                StartCoroutine(levels[i].turnOn());
            }
        }
        else
        {
            for (int i = oldLevel; i > newLevel; i--)
            {
                StartCoroutine(levels[i].turnOff());
            }
        }
    }

    IEnumerator StreakTimeout()
    {
        yield return new WaitForSeconds(streakTimeout);
        //print("Streak ended");
        if (currentStreakLevel > 0)
        {
            transitionLevel(currentStreakLevel, --currentStreakLevel);
            timeoutRoutine = StartCoroutine(StreakTimeout());
        }
    }

    Coroutine musicTransitionRoutine;
    public void transitionMusic(float newVolume)
    {
        if (musicTransitionRoutine != null)
        {
            StopCoroutine(musicTransitionRoutine);
        }
        musicTransitionRoutine = StartCoroutine(transitionMusicRoutine(newVolume));
    }

    IEnumerator transitionMusicRoutine(float newVolume)
    {
        float volumeStartValue = musicSource.volume;

        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(volumeStartValue, newVolume, progress);
            yield return null;
        }
    }

    Coroutine rovPoundRoutine;
    public void startFovPound()
    {
        if (rovPoundRoutine != null)
        {
            StopCoroutine(rovPoundRoutine);
        }
        rovPoundRoutine = StartCoroutine(poundFOV(true));
    }

    public void stopFovPound()
    {
        if (rovPoundRoutine != null)
        {
            StopCoroutine(rovPoundRoutine);
        }
        rovPoundRoutine = StartCoroutine(poundFOV(false));
    }

    IEnumerator poundFOV(bool start)
    {
        float startFov = camera.fieldOfView;
        float fovPoundSmoothTransition = 0;

        float progress = 0;
        while (true)
        {
            if (progress < 1) {
                progress += Time.deltaTime;
                if (start)
                {
                    fovPoundSmoothTransition = Mathf.Lerp(0, 1, progress);
                }
                else
                {
                    fovPoundSmoothTransition = Mathf.Lerp(1, 0, progress);
                }
            }
            camera.fieldOfView = level2Fov + (poundFov * musicVizualizer.GetComponent<MusicVisualizer>().bands[2].audioBandBuffer * fovPoundSmoothTransition);
            yield return null;
        }
    }

    [System.Serializable]
    public abstract class Level
    {
        public PlayerPerformance pp;

        public abstract IEnumerator turnOn();
        public abstract IEnumerator turnOff();
    }

    /// <summary>
    /// Effects:
    /// Music 25%
    /// </summary>
    [System.Serializable]
    public class Level0 : Level
    {
        public override IEnumerator turnOn()
        {
            print("Entering level 0");
            yield return null;
        }

        public override IEnumerator turnOff()
        {
            print("Exiting level 0");
            yield return null;
        }
    }

    /// <summary>
    /// Effects:
    /// Music 75%
    /// </summary>
    [System.Serializable]
    public class Level1 : Level
    {

        public override IEnumerator turnOn()
        {
            print("Entering level 1");
            pp.transitionMusic(0.75f);
            yield return null;
        }

        public override IEnumerator turnOff()
        {
            print("Exiting level 1");
            pp.transitionMusic(0.25f);
            yield return null;
        }
    }

    /// <summary>
    /// Effects:
    /// Fov increase
    /// </summary>
    [System.Serializable]
    public class Level2 : Level
    {

        public override IEnumerator turnOn()
        {
            print("Entering level 2");

            float fovStartValue = pp.camera.fieldOfView;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime;
                pp.camera.fieldOfView = Mathf.Lerp(fovStartValue, pp.level2Fov, progress);
                yield return null;
            }
        }

        public override IEnumerator turnOff()
        {
            print("Exiting level 2");

            float fovStartValue = pp.camera.fieldOfView;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime;
                pp.camera.fieldOfView = Mathf.Lerp(fovStartValue, pp.normalFov, progress);
                yield return null;
            }
        }
    }
    /// <summary>
    /// Effects:
    /// Music 100%
    /// MusicVisualization
    /// </summary>
    [System.Serializable]
    public class Level3 : Level //TODO: Fix musicvisualizer sometimes not coming back
    {
        public override IEnumerator turnOn()
        {
            print("Entering level 3");
            pp.transitionMusic(1);

            pp.musicVizualizer.gameObject.SetActive(true);
            Color startColour = pp.musicVizualizer.GetChild(0).GetComponent<MeshRenderer>().material.color;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime;
                for (int i = 0; i < pp.musicVizualizer.childCount; i++)
                {
                    pp.musicVizualizer.GetChild(i).GetComponent<MeshRenderer>().material.color = startColour.withAlpha(Mathf.Lerp(startColour.a, 1, progress));
                }
                yield return null;
            }
        }

        public override IEnumerator turnOff()
        {
            print("Exiting level 3");
            pp.transitionMusic(0.75f);

            Color startColour = pp.musicVizualizer.GetChild(0).GetComponent<MeshRenderer>().material.color;

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime;
                for (int i = 0; i < pp.musicVizualizer.childCount; i++)
                {
                    pp.musicVizualizer.GetChild(i).GetComponent<MeshRenderer>().material.color = startColour.withAlpha(Mathf.Lerp(startColour.a, 0, progress));
                }
                yield return null;
            }
            pp.musicVizualizer.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Effects:
    /// Fov pounding
    /// Light trails
    /// </summary>
    [System.Serializable]
    public class Level4 : Level
    {
        Coroutine r;

        public override IEnumerator turnOn()
        {
            print("Entering level 4");

            Color startColor = pp.leftTrail.startColor;

            pp.startFovPound(); //TODO: Smooth transition

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime;
                pp.leftTrail.startColor = startColor.withAlpha(Mathf.Lerp(startColor.a, 1, progress));
                pp.rightTrail.startColor = startColor.withAlpha(Mathf.Lerp(startColor.a, 1, progress));
                yield return null;
            }
        }

        public override IEnumerator turnOff()
        {
            print("Exiting level 4");

            Color startColor = pp.leftTrail.startColor;

            pp.stopFovPound();

            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime;
                pp.leftTrail.startColor = startColor.withAlpha(Mathf.Lerp(startColor.a, 0, progress));
                pp.rightTrail.startColor = startColor.withAlpha(Mathf.Lerp(startColor.a, 0, progress));
                yield return null;
            }
        }
    }
}

public static class Util
{
    public static Color withAlpha(this Color color, float alpha) //Fancy pancy extension method
    {
        color.a = alpha;
        return color;
    }
}
