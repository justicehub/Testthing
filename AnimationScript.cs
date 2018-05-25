using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour {

    List<Sprite> SpriteList = new List<Sprite>();
    List<float> Velocity = new List<float>();
    List<float> ScaleX = new List<float>();
    List<float> ScaleY = new List<float>();
    bool fadeOut = false;
    bool flipXRandomly = false;
    int animationFrame = 0;
    float animationSpeed = 0;
    float movementSpeed = 15;
    string animationType;
    string movementType;
    string destroyType;
    float oriLifeSpan = -1;
    float lifeSpan = -1;
    float lifePhase = 0;

    GameObject Caster;
    GameObject Target;
    Vector3 targetPosition;
    bool rotateToTarget = true;

	// Use this for initialization
	void Start () {
        targetPosition = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        if (lifeSpan > 0)
        {
            lifePhase = 1 - lifeSpan / oriLifeSpan;
        }

        animationSpeed -= 1 * Time.deltaTime;
        if (animationSpeed < 0)
        {
            animationSpeed = 0.2f;
            animationFrame += 1;
            if (animationType == "Loop")
            {
                transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = SpriteList[animationFrame % SpriteList.Count];
            }
            else if (animationType == "LoopOnce")
            {
                if (animationFrame > SpriteList.Count - 1)
                {
                    animationFrame = SpriteList.Count - 1;
                }
                transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = SpriteList[animationFrame];
            }

            if (flipXRandomly == true && Random.Range(0f, 1f) > 0.5f)
            {
                if (transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX == false)
                {
                    transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    transform.Find("Sprite").GetComponent<SpriteRenderer>().flipX = false;
                }
            }
        }

        if (Target)
        {
            targetPosition = Target.transform.Find("Sprite").transform.position;
        }

        if (movementType == "Linear")
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
        else if (movementType == "StayOnTarget")
        {
            if (Target)
            {
                targetPosition = Target.transform.position;
                transform.position = targetPosition;
            }
        }
        if (rotateToTarget)
        {
            transform.LookAt(targetPosition);
        }

        // scale
        if (ScaleX.Count > 0)
        {
            float scale = SpriteListPosition(ScaleX);
            Vector3 v3Scale = transform.localScale;
            v3Scale.x = SpriteListPosition(ScaleX);
            transform.localScale = v3Scale;
        }
        if (ScaleY.Count > 0)
        {
            float scale = SpriteListPosition(ScaleY);
            Vector3 v3Scale = transform.localScale;
            v3Scale.z = SpriteListPosition(ScaleY);
            transform.localScale = v3Scale;
        }

        // fadeout
        if (fadeOut && lifePhase > 0.5f)
        {
            transform.Find("Sprite").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - (lifePhase - 0.5f) / 0.5f);
        }

        // if has sprite velocity
        if (Velocity.Count > 0)
        {
            transform.Find("Sprite").transform.localPosition += new Vector3(0, 0, Velocity[0] * Time.deltaTime);
        }

        if (destroyType == "OnTarget" && transform.position == targetPosition)
        {
            Destroy(gameObject);
        }

        if (lifeSpan > 0)
        {
            lifeSpan -= 1 * Time.deltaTime;

            if (lifeSpan <= 0)
            {
                Destroy(gameObject);
            }
        }

        List<float> aList = new List<float>();
        aList.Add(1.25f);
        aList.Add(0.25f);
        SpriteListPosition(aList);
    }

    float SpriteListPosition(List<float> List)
    {
        //0 : 1 - There are 2\\
        if (List.Count == 1)
        {
            List.Add(List[0]);
        }

        float fromNumber = List[Mathf.FloorToInt((List.Count - 1) * lifePhase)];
        float toNumber = List[Mathf.CeilToInt((List.Count - 1) * lifePhase)];
        float currentNumber = (toNumber - fromNumber) * lifePhase + fromNumber;
        return currentNumber;
        //print(currentNumber + ":" + lifePhase);
    }

    public void SetSpriteList (List<Sprite> spriteList)
    {
        SpriteList = spriteList;

        transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = SpriteList[0];
    }
    public void SetCaster(GameObject caster)
    {
        Caster = caster;
        transform.position = Caster.transform.position + new Vector3(0, 1, 0);
    }
    public void SetTarget(GameObject target)
    {
        Target = target;
    }
    public void SetRotateToTarget (bool rotate)
    {
        rotateToTarget = rotate;
    }
    public void SetAnimationType (string type)
    {
        animationType = type;
    }
    public void SetVelocity (List<float> vel)
    {
        Velocity = vel;
    }
    public void SetFadeout (bool fade)
    {
        fadeOut = fade;
    }
    public void SetRandomlyFlipSpriteX (bool flip)
    {
        flipXRandomly = flip;
    }
    public void SetMovementType (string type)
    {
        movementType = type;

        //if (movementType == "StayOnTarget" && Target.transform.Find("Sprite"))
        //{
        //    Target = Target.transform.Find("Sprite").gameObject;
        //}
    }
    public void SetDestroyType (string type)
    {
        destroyType = type;

        if (destroyType.Length > 9 && destroyType.Substring(0, 9) == "DestroyOn")
        {
            float destroyIn = int.Parse(destroyType.Substring(10, destroyType.Length - 10));
            lifeSpan = destroyIn / 1000;
            oriLifeSpan = lifeSpan;
        }
    }
    public void SetScaleX(List<float> scale)
    {
        ScaleX = scale;
    }
    public void SetScaleY(List<float> scale)
    {
        ScaleY = scale;
    }
    public void SetSpawnEffectOnCreate (List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject Effect = GameObject.Find("GlobalManager").GetComponent<AbilityEffectManager>().SpawnEffect(list[i]);
            //print("SET CASTER TO " + Caster.transform.name);
            //Effect.GetComponent<AnimationScript>().SetCaster(Caster);
            //Effect.GetComponent<AnimationScript>().SetTarget(Target);
        }
    }
}

/*

    Not yet working
    - Spawn other effect,
        Problem: setting caster and target
        solution: Just got to do it

    */