using System.Collections;
using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Modding.Utils;
using HutongGames.PlayMaker;
using Vasi;
using System;

namespace Soul_Paladin
{
    public class PaladinControl : MonoBehaviour
    {


        private void Start()
        {
           PlayMakerFSM PaladinControl = gameObject.LocateMyFSM("Mage Knight");
            if (PaladinControl != null)
            {
                ControlSetup(PaladinControl);
            }
        }

        private static PlayMakerFSM controlFsm;
        private bool audioplayed = true;
        private void ControlSetup(PlayMakerFSM controlfsm)
        {
            controlFsm = controlfsm;
            //HP.
            gameObject.GetComponent<HealthManager>().hp = 1500;

            //Contact damage.
            FsmState wakestate = controlfsm.GetState("Wake");
            wakestate.GetAction<SetDamageHeroAmount>().damageDealt = 2;

            //doubling slash damage
            foreach (DamageHero damageHero in gameObject.GetComponentsInChildren<DamageHero>())
            {
                damageHero.damageDealt = 2;
            }

            //Stomp speed.
            FsmState stompstate = controlfsm.GetState("Stomp Air");
            stompstate.GetAction<SetVelocity2d>().y = -85f;

            //Slash recover speedup
            FsmState slashrecover = controlfsm.GetState("Slash Recover");
            replaceAniTriggerEvent(slashrecover, 0);

            //Stomp antic speedup
            FsmState stompantic = controlfsm.GetState("Stomp Antic");
            replaceAnimationEvent(stompantic, 2, 0.25f);

            //Side teleport speedup
            FsmState sidetele = controlfsm.GetState("Side Tele");
            replaceAnimationEvent(sidetele, 7, 0.15f);

            //Up tele speedup
            FsmState uptele = controlfsm.GetState("Up Tele");
            replaceAnimationEvent(uptele, 5, 0.15f);

            //Tele antic speedup
            FsmState teleantic = controlfsm.GetState("Tele Antic");
            replaceAnimationEvent(teleantic, 1, 0.15f);

            //Horizontal slash antic speedup
            FsmState slashantic = controlfsm.GetState("Slash Antic");
            replaceAnimationEvent(slashantic, 0, 0.45f);

            //Stomp recover - add radiance lasers.
            FsmState stomprecover = controlfsm.GetState("Stomp Recover");
            replaceWatchAnimationEvent(stomprecover, 0, 0.8f);
            addLasers(stomprecover);

            //laser audio
            FsmState idle = controlFsm.GetState("Idle");
            addAudio(idle);

            //add to the shooty attack
            FsmState shoot = controlfsm.GetState("Shoot");
            addZaps(shoot);

            //Variable edits.
            controlfsm.FsmVariables.FindFsmFloat("Dash Speed").Value = 65f;
            controlfsm.FsmVariables.FindFsmFloat("Fire Speed").Value = 30f;
        }

        private void replaceAnimationEvent(FsmState state, int actionindex, float waittime)
        {
            FsmEvent previousevent = state.GetAction<Tk2dPlayAnimationWithEvents>(actionindex).animationCompleteEvent;
            state.GetAction<Tk2dPlayAnimationWithEvents>(actionindex).animationCompleteEvent = new FsmEvent("");
            Wait waitaction = new Wait();
            waitaction.time = waittime;
            waitaction.realTime = false;
            waitaction.finishEvent = previousevent;
            state.AddAction(waitaction);
        }

        private void replaceWatchAnimationEvent(FsmState state, int actionindex, float waittime)
        {
            FsmEvent previousevent = state.GetAction<Tk2dWatchAnimationEvents>(actionindex).animationCompleteEvent;
            state.GetAction<Tk2dWatchAnimationEvents>(actionindex).animationCompleteEvent = new FsmEvent("");
            Wait waitaction = new Wait();
            waitaction.time = waittime;
            waitaction.realTime = false;
            waitaction.finishEvent = previousevent;
            state.AddAction(waitaction);
        }

        private void replaceAniTriggerEvent(FsmState state, int actionindex)
        {
            FsmEvent previousevent = state.GetAction<Tk2dWatchAnimationEvents>(actionindex).animationTriggerEvent;
            state.GetAction<Tk2dWatchAnimationEvents>(actionindex).animationTriggerEvent = new FsmEvent("");
            Wait waitaction = new Wait();
            waitaction.time = 0f;
            waitaction.realTime = false;
            waitaction.finishEvent = previousevent;
            state.AddAction(waitaction);
        }

        private void addAudio(FsmState state)
        {
            AudioPlaySimple shotaudio = new AudioPlaySimple();
            shotaudio.gameObject = controlFsm.GetAction<AudioPlaySimple>("Up Tele", 0).gameObject;
            shotaudio.volume = 0f;
            shotaudio.oneShotClip = ResourceLoader.laserburstsound;
            state.AddAction(shotaudio);
            state.InsertMethod(0,() =>
            { 
                if (audioplayed == false)
                {
                    audioplayed = true;
                    shotaudio.volume = 1f;
                } else
                {
                    shotaudio.volume = 0f;
                }
            });
        }

        private void addLasers(FsmState state)
        {
            state.AddMethod(() =>
            {
                for (int i = 1; i < 25; i++)
                {
                    GameObject laser = Instantiate(ResourceLoader.radiancelaser);
                    Vector2 warriorpos = transform.position;
                    float xvalue = (float)(i * 2.5);
                    laser.transform.SetPosition2D((warriorpos.x - 25)+xvalue, 0);
                    laser.SetActive(true);
                    PlayMakerFSM fsm = laser.GetComponent<PlayMakerFSM>();
                    StartCoroutine(activateLaser(fsm,laser));
                    audioplayed = false;
                }
            });

            //also sound effects
            AudioPlaySimple prepareaudio = new AudioPlaySimple();
            prepareaudio.gameObject = controlFsm.GetAction<AudioPlaySimple>("Up Tele", 0).gameObject;
            prepareaudio.volume = 1f;
            prepareaudio.oneShotClip = ResourceLoader.laserpreparesound;

            state.InsertAction(0,prepareaudio);

        }

        private IEnumerator activateLaser(PlayMakerFSM fsm, GameObject laser)
        {
            
            fsm.SetState("Antic");
            yield return new WaitForSeconds(0.75f);
            fsm.SetState("Fire");
            yield return new WaitForSeconds(0.2f);
            fsm.SetState("End");
            yield return new WaitForSeconds(0.2f);
            Destroy(laser);
        }

        private void addZaps(FsmState state)
        {
            state.AddMethod(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    GameObject zap = Instantiate(ResourceLoader.multizap);
                    zap.GetComponent<DamageHero>().damageDealt = 2;
                    StartCoroutine(createZap(i,zap));
                }
            });
        }

        private IEnumerator createZap(int i, GameObject zap) 
        {
            yield return new WaitForSeconds(0.5f * i);
            zap.transform.position = HeroController.instance.transform.position;
            zap.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            zap.SetActive(false);
            yield return new WaitForSeconds(1f);
            Destroy(zap);
        }
    }
}