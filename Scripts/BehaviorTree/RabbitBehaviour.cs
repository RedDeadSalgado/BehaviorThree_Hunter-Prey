using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RabbitBehaviour : MonoBehaviour{

	BehaviourTree tree;

	[Range(0,100)] public int hunger = 100;

	public GameObject safeplace;
	public GameObject food;
	public GameObject backdoor;
	public GameObject frontdoor;

	NavMeshAgent agent;

	public enum ActionState{ IDLE, WORKING};
	ActionState state = ActionState.IDLE;

	Node.Status treeStatus = Node.Status.RUNNING;
    
    void Start(){
        agent = this.GetComponent<NavMeshAgent>();
        
        tree = new BehaviourTree();
        Sequence hide = new Sequence("Tengo que ocultarme");
        Leaf hasGotHunger = new Leaf("Tiene hambre?", HasHunger);
        Leaf goToBackDoor = new Leaf("Ve a puerta trasera",GoToBackDoor);
        Leaf goToSafe = new Leaf("Escondete",GoToSafe);
        Leaf goToFrontDoor = new Leaf("Ve a puerta delantera",GoToFrontDoor);
        Leaf getHungry = new Leaf("Ve a comer",GoToFood);
        Selector opendoor = new Selector("Abrir puerta");
		
		opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);
        
        hide.AddChild(hasGotHunger);
        hide.AddChild(opendoor);
        hide.AddChild(goToSafe);
        hide.AddChild(getHungry);
        hide.AddChild(goToSafe);
        tree.AddChild(hide);

        tree.PrintTree();
    }

    public Node.Status HasHunger(){
    	if(hunger >= 50){
    		return Node.Status.FAILURE;
    	}
    	return Node.Status.SUCCESS;
    }

    public Node.Status GoToFrontDoor(){
    	return GoToDoor(frontdoor);      //Es aqui donde se toma la accion donde verdaderamente se pregunta que hacer
    }
    public Node.Status GoToBackDoor(){
    	return GoToDoor(backdoor);
    }

    public Node.Status GoToSafe(){
    	return GoToLocation(safeplace.transform.position);
    }

    public Node.Status GoToFood(){
    	Node.Status s = GoToLocation(food.transform.position);
    	if(s == Node.Status.SUCCESS){
    		food.SetActive(false);
    		return Node.Status.SUCCESS;
    	}
    	return s;
    }

    public Node.Status GoToDoor(GameObject door){
    	Node.Status s = GoToLocation(door.transform.position); //Para preguntar si si llegamos
    	if(s == Node.Status.SUCCESS){
    		if(!door.GetComponent<Lock>().isLocked){  //Ahora si preguntamos si esta loqueada
    			door.SetActive(false);
    			return Node.Status.SUCCESS;
    		}
    		return Node.Status.FAILURE;
    	}
    	else{
    		return s;
    	}
    }

    Node.Status GoToLocation(Vector3 destination){
    	float distanceToTarget = Vector3.Distance(destination, this.transform.position);
    	if(state == ActionState.IDLE){
    		agent.SetDestination(destination);
    		state = ActionState.WORKING;
    	}
    	else if(Vector3.Distance(agent.pathEndPosition, destination) >= 2){
    		state = ActionState.IDLE;
    		return Node.Status.FAILURE;
    	}
    	else if(distanceToTarget < 2){
    		state = ActionState.IDLE;
    		return Node.Status.SUCCESS;
    	}
    	return Node.Status.RUNNING;
    }

    // Update is called once per frame
    void Update(){
    	if(treeStatus != Node.Status.SUCCESS)
        	treeStatus = tree.Process();
    }
}
