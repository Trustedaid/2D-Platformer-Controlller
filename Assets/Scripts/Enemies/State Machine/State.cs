using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected FiniteStateMachine stateMachine;
    protected Entity entity;

    protected string animBoolName;

    public float startTime { get; protected set; }
    public State(Entity entity, FiniteStateMachine stateMachine, string animBoolName)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }
    public virtual void Enter() // means that this function can be redefined in the derived classes 
    {
        startTime = Time.time;
        // function get called whatever state it is it gonna store the start time && we can reference this start time without having set the start time
        entity.anim.SetBool(animBoolName, true);
        DoChecks();
    }
    public virtual void Exit()
    {
        entity.anim.SetBool(animBoolName, false);
    }
    public virtual void LogicUpdate()
    {

    }
    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }
    public virtual void DoChecks()
    {

    }
}
