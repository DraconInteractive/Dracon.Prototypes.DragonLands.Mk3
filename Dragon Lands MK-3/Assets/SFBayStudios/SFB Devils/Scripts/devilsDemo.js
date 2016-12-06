#pragma strict

var animator		: Animator;							// The Animator Controller
var castObjects		: GameObject[];						// Array of objects to turn on/off for casting
var screamObjects	: GameObject[];						// Array of objects to turn on/off for screaming

function Start () {
	animator	= GetComponent.<Animator>();			// Assign the variable
}

function UpdateLocomotion(newValue : float){
	animator.SetFloat("locomotion", newValue);			// Pass new value to Animator
}

function StartCast(){
	for (var i : int; i < castObjects.Length; i++){
		if (castObjects[i].GetComponent(ParticleSystem))
			castObjects[i].GetComponent(ParticleSystem).enableEmission 	= true;
		if (castObjects[i].GetComponent(Light))
			castObjects[i].GetComponent(Light).enabled					= true;
	}
}

function StopCast(){
	for (var i : int; i < castObjects.Length; i++){
		if (castObjects[i].GetComponent(ParticleSystem))
			castObjects[i].GetComponent(ParticleSystem).enableEmission 	= false;
		if (castObjects[i].GetComponent(Light))
			castObjects[i].GetComponent(Light).enabled					= false;
	}
}

function StartScream(){
	for (var i : int; i < screamObjects.Length; i++){
		if (screamObjects[i].GetComponent(ParticleSystem))
			screamObjects[i].GetComponent(ParticleSystem).enableEmission 	= true;
		if (screamObjects[i].GetComponent(Light))
			screamObjects[i].GetComponent(Light).enabled					= true;
	}
}

function StopScream(){
	for (var i : int; i < screamObjects.Length; i++){
		if (screamObjects[i].GetComponent(ParticleSystem))
			screamObjects[i].GetComponent(ParticleSystem).enableEmission 	= false;
		if (screamObjects[i].GetComponent(Light))
			screamObjects[i].GetComponent(Light).enabled					= false;
	}
}