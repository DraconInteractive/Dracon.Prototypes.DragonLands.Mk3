
var speed	: float		= 10.0;			// Speed of movement;

function Update(){
	transform.Translate(Vector3.forward * (Time.deltaTime * speed));
}