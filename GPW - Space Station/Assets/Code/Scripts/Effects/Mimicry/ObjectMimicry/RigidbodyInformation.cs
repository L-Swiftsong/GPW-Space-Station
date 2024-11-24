using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> A struct to store the required information of a Rigidbody when copying its values.</summary>
public struct RigidbodyInformation
{
    private float _mass;
    private float _angularDrag;
    private float _drag;

    private bool _useAutomaticCentreOfMass;
    private Vector3 _centreOfMass;

    private RigidbodyConstraints _constraints;


    private RigidbodyInformation(float mass, float angularDrag, float drag,
        bool automaticCentreOfMass, Vector3 centreOfMass,
        RigidbodyConstraints constraints)
    {
        this._mass = mass;
        this._angularDrag = angularDrag;
        this._drag = drag;

        this._useAutomaticCentreOfMass = automaticCentreOfMass;
        this._centreOfMass = centreOfMass;

        this._constraints = constraints;
    }

    public static RigidbodyInformation CreateFromRigidbody(Rigidbody rigidbody) => new RigidbodyInformation(
        mass: rigidbody.mass,
        angularDrag: rigidbody.angularDrag,
        drag: rigidbody.drag,
        automaticCentreOfMass: rigidbody.automaticCenterOfMass,
        centreOfMass: rigidbody.centerOfMass,
        constraints: rigidbody.constraints
    );


    public void ApplyToRigidbody(Rigidbody rigidbody)
    {
        rigidbody.mass = _mass;
        rigidbody.angularDrag = _angularDrag;
        rigidbody.drag = _drag;

        if (_useAutomaticCentreOfMass)
        {
            rigidbody.automaticCenterOfMass = true;
        }
        else
        {
            rigidbody.automaticCenterOfMass = false;
            rigidbody.centerOfMass = _centreOfMass;
        }

        rigidbody.constraints = _constraints;
    }
}