using UnityEngine;

public class Elevator : InteractableEntity
{
    public enum ElevatorDirection
    {
        vertical, horizontal
    }

    public enum ElevatorButtonSide
    {
        left, right
    }

    private Vector3 defaultElevatorPosition;

    [Tooltip("The point towards which the elevator will travel")]
    [SerializeField] private Vector3 travelPoint;
    [Tooltip("The axis at which the elevator will travel on")]
    [SerializeField] private ElevatorDirection elevatorDirection = ElevatorDirection.vertical;
    [Tooltip("How fast should the elevator go?")]
    [SerializeField][Range(1f, 50f)] private float elevatorSpeed = 1;
    [SerializeField] private float stopDistance;
    [SerializeField] private Vector3 travelPosition;

    [Tooltip("Is the elevator moving?")]
    [SerializeField] private bool moving = false;
    [Tooltip("Did the elevator just start moving?")]
    [SerializeField] private bool aboutToMove = false;

    [SerializeField] private float distanceFromEnd;
    [SerializeField] private float distanceFromStart;

    [SerializeField] private Animator elevatorAnimator;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject elevatorButtonPrefab;

    ElevatorButton startButtonScript;
    [SerializeField] private ElevatorButtonSide startButtonSide = ElevatorButtonSide.left;
    ElevatorButton endButtonScript;
    [SerializeField] private ElevatorButtonSide endButtonSide = ElevatorButtonSide.left;

    private void Awake()
    {
        defaultElevatorPosition = transform.localPosition;
        stopDistance = elevatorSpeed / 100;

        // Fix the axis that is not used to the corresponding value of the elevator
        if (elevatorDirection == ElevatorDirection.vertical)
        {
            travelPoint.x = transform.localPosition.x;
        }
        else
        {
            travelPoint.y = transform.localPosition.y;
        }

        if (elevatorAnimator == null)
        {
            elevatorAnimator = GetComponent<Animator>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        Physics.IgnoreLayerCollision(0, 2);
        Physics.IgnoreLayerCollision(4, 9);

        // Spawn elevatorCallers at start position and end position
        SetupElevatorButtons();

        isSelfish = true;
    }

    private void Update()
    {
        if (elevatorDirection == ElevatorDirection.vertical)
        {
            //distanceFromEnd = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, travelPoint.y));
            distanceFromEnd = Vector2.Distance(new Vector2(0, transform.localPosition.y), new Vector2(0, travelPoint.y));
            //distanceFromStart = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, defaultElevatorPosition.y));
            distanceFromStart = Vector2.Distance(new Vector2(0, transform.localPosition.y), new Vector2(0, defaultElevatorPosition.y));
            //distanceFromStart = Vector2.Distance(transform.position, defaultElevatorPosition);
        }
        else
        {
            //distanceFromEnd = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(travelPoint.x, 0));
            distanceFromEnd = Vector2.Distance(new Vector2(transform.localPosition.x, 0), new Vector2(travelPoint.x, 0));
            //distanceFromStart = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(defaultElevatorPosition.x, 0));
            distanceFromStart = Vector2.Distance(new Vector2(transform.localPosition.x, 0), new Vector2(defaultElevatorPosition.x, 0));
            //distanceFromEnd = Vector2.Distance(transform.position, travelPoint);
            //distanceFromStart = Vector2.Distance(transform.position, defaultElevatorPosition);
        }

        if (distanceFromStart < stopDistance || distanceFromEnd < stopDistance)
        {
            if (distanceFromStart < stopDistance)
            {
                // disable start button
                startButtonScript.Interactable = false;
                // enable end button
                endButtonScript.Interactable = true;
            }

            if (distanceFromEnd < stopDistance)
            {
                // disable end button
                endButtonScript.Interactable = false;
                // enable start button
                startButtonScript.Interactable = true;
            }

            if (!aboutToMove)
            {
                // Prevent elevator from shutting down before reaching its destination
                moving = false;
                StopSound();
            }

            if (Input.GetKeyDown(KeyCode.E) && collidingWithPlayer)
            {
                MoveElevator();
            }
        }
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            //transform.position = transform.position + (transform.position - travelPosition).normalized * Time.deltaTime * -1 * elevatorSpeed;
            //transform.position = transform.position + (transform.position - travelPosition) * Time.deltaTime * -1 * elevatorSpeed;
            //transform.position += (travelPosition - transform.position).normalized * Time.deltaTime  * elevatorSpeed;
            //transform.position = Vector2.MoveTowards(transform.position, travelPosition, elevatorSpeed * Time.deltaTime);
            if (elevatorDirection == ElevatorDirection.vertical)
            {
                if (transform.localPosition.y - travelPosition.y < 0)
                {
                    // About to move upwards
                    elevatorAnimator.SetBool("goingUp", true);
                    elevatorAnimator.SetBool("goingDown", false);
                    elevatorAnimator.SetBool("goingLeft", false);
                    elevatorAnimator.SetBool("goingRight", false);
                }
                else
                {
                    // About to move downwards
                    elevatorAnimator.SetBool("goingUp", false);
                    elevatorAnimator.SetBool("goingDown", true);
                    elevatorAnimator.SetBool("goingLeft", false);
                    elevatorAnimator.SetBool("goingRight", false);
                }
            }
            else
            {
                if (transform.localPosition.x - travelPosition.x < 0)
                {
                    // About to move to the right
                    elevatorAnimator.SetBool("goingDown", false);
                    elevatorAnimator.SetBool("goingUp", false);
                    elevatorAnimator.SetBool("goingLeft", false);
                    elevatorAnimator.SetBool("goingRight", true);
                }
                else
                {
                    // About to move to the left
                    elevatorAnimator.SetBool("goingDown", false);
                    elevatorAnimator.SetBool("goingUp", false);
                    elevatorAnimator.SetBool("goingLeft", true);
                    elevatorAnimator.SetBool("goingRight", false);
                }
            }
            //transform.localPosition = Vector3.MoveTowards(transform.localPosition, travelPosition, elevatorSpeed * Time.deltaTime);
            MoveToPosition(travelPosition);
            aboutToMove = false;
        }
        else
        {
            elevatorAnimator.SetBool("goingUp", false);
            elevatorAnimator.SetBool("goingDown", false);
            elevatorAnimator.SetBool("goingLeft", false);
            elevatorAnimator.SetBool("goingRight", false);
        }
    }

    private void PlaySound()
    {
        //audioSource.time = Random.Range(0f, audioSource.clip.length);
        audioSource.time = Random.Range(0f, audioSource.clip.length);
        audioSource.Play();
    }

    private void StopSound()
    {
        audioSource.Stop();
    }

    void MoveToPosition(Vector3 travelPosition)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, travelPosition, elevatorSpeed * Time.deltaTime);
    }
    public void MoveElevator()
    {
        // Activate elevator
        if (distanceFromEnd < stopDistance)
        {
            // We are in the destination point
            if (elevatorDirection == ElevatorDirection.vertical)
            {
                travelPosition = new Vector3(transform.localPosition.x, defaultElevatorPosition.y, transform.localPosition.z);
            }
            else
            {
                travelPosition = new Vector3(defaultElevatorPosition.x, transform.localPosition.y, transform.localPosition.z);
            }
            moving = true;
            aboutToMove = true;

            PlaySound();
        }
        else if (distanceFromStart < stopDistance)
        {
            // We are in the deafult point
            if (elevatorDirection == ElevatorDirection.vertical)
            {
                travelPosition = new Vector3(transform.localPosition.x, travelPoint.y, transform.localPosition.z);
            }
            else
            {
                travelPosition = new Vector3(travelPoint.x, transform.localPosition.y, transform.localPosition.z);
            }
            moving = true;
            aboutToMove = true;

            PlaySound();
        }
    }

    private void SetupElevatorButtons()
    {
        // Start
        GameObject startButton = Instantiate(elevatorButtonPrefab, transform.position, Quaternion.identity);
        startButton.transform.parent = transform.parent;
        //startButton.transform.position = transform.position;
        // End
        //GameObject endButton = Instantiate(elevatorButtonPrefab, transform);
        GameObject endButton = Instantiate(elevatorButtonPrefab, transform.position, Quaternion.identity);
        endButton.transform.parent = transform.parent;

        //endButton.transform.position = transform.localPosition;
        if (elevatorDirection == ElevatorDirection.vertical)
        {
            //travelPoint.x = transform.position.x;
            Vector3 newPos = new Vector3(
                endButton.transform.position.x,
                endButton.transform.position.y - Vector2.Distance(new Vector2(0, endButton.transform.localPosition.y), new Vector2(0, travelPoint.y)),
                endButton.transform.position.z);
            //endButton.transform.position.x = transform.localPosition.x;
            endButton.transform.position = newPos;
        }
        else
        {
            //travelPoint.y = transform.position.y;
            //endButton.transform.position.y = transform.localPosition.y;
            Vector3 newPos = new Vector3(endButton.transform.position.x + travelPoint.x, endButton.transform.position.y, endButton.transform.position.z);
            //endButton.transform.position.x = transform.localPosition.x;
            endButton.transform.position = newPos;
        }

        float elevatorWidth = GetComponent<BoxCollider2D>().size.x;
        //Debug.Log($"ELEVATOR WIDTH: {elevatorWidth}");

        Vector3 pushToSide;

        if (startButtonSide == ElevatorButtonSide.left)
        {
            pushToSide = new Vector3(
            startButton.transform.position.x - elevatorWidth / 4,
            startButton.transform.position.y,
            startButton.transform.position.z
            );
        }
        else
        {
            pushToSide = new Vector3(
            startButton.transform.position.x + elevatorWidth / 4,
            startButton.transform.position.y,
            startButton.transform.position.z
            );
        }
        startButton.transform.position = pushToSide;

        if (endButtonSide == ElevatorButtonSide.left)
        {
            pushToSide = new Vector3(
            endButton.transform.position.x - elevatorWidth / 4,
            endButton.transform.position.y,
            endButton.transform.position.z
            );
        }
        else
        {
            pushToSide = new Vector3(
            endButton.transform.position.x + elevatorWidth / 4,
            endButton.transform.position.y,
            endButton.transform.position.z
            );
        }
        endButton.transform.position = pushToSide;

        startButtonScript = startButton.GetComponent<ElevatorButton>();
        startButtonScript.Elevator = this;
        startButtonScript.Position = transform.position;
        endButtonScript = endButton.GetComponent<ElevatorButton>();
        endButtonScript.Elevator = this;
        endButtonScript.Position = travelPoint;
    }

}
