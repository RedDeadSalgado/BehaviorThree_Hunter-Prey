using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TemerosoBehaviour : MonoBehaviour{

    BehaviourTree tree;

	[Range(0,100)] public float range = 5;
	[Range(0,100)] public float hunger = 0;
	[Range(0,100)] public float getHungerRate = 0;
	[Range(1,500)] public float WalkRadius = 5;

	public GameObject safeplace;
	public GameObject food;
	public GameObject backdoor;
	public GameObject frontdoor;
	public GameObject hunter;

	NavMeshAgent agent;

	public enum ActionState{ IDLE, WORKING};
	ActionState state = ActionState.IDLE;

	Node.Status treeStatus = Node.Status.RUNNING;
    
    void Start(){
        agent = this.GetComponent<NavMeshAgent>();
        
        tree = new BehaviourTree();

        Leaf wander = new Leaf("Ando bien relajado",GoToWander);

        Leaf goFood = new Leaf("Ve a comer",GoToFood);
        Leaf hasGotHunger = new Leaf("Tengo hambre?", HasHunger);
        Sequence eat = new Sequence("Tengo que comer");

        Leaf goToSafe = new Leaf("Escondete",GoToSafe);
        Leaf danger = new Leaf("Estoy en peligro?",Danger);
        Sequence hide = new Sequence("Tengo que ocultarme");
        
        Selector wuattudu = new Selector("Que voy a hacer ahora...");
        
        hide.AddChild(danger);
        hide.AddChild(goToSafe);

        eat.AddChild(hasGotHunger);
        eat.AddChild(goFood);

        wuattudu.AddChild(hide);
        wuattudu.AddChild(eat);
        wuattudu.AddChild(wander);

        tree.AddChild(wuattudu);

        tree.PrintTree();
    }

    public Node.Status GoToWander(){
    	return GoToLocation(RandomNavMeshLocation());
    }

    public Node.Status GoToFood(){
    	Node.Status s = GoToLocation(food.transform.position);
    	if(s == Node.Status.SUCCESS){
    		food.SetActive(false);
    		hunger += 10;
    		return Node.Status.SUCCESS;
    	}
    	return s;
    }

    public Node.Status HasHunger(){
    	if(hunger >= 50){
    		return Node.Status.FAILURE;
    	}
    	return Node.Status.SUCCESS;
    }

    public Node.Status Danger(){
    	float distance = Vector3.Distance(agent.transform.position, hunter.transform.position);
		if(distance <= range){
    		return Node.Status.SUCCESS;
    	}
    	return Node.Status.FAILURE;
    }

    public Node.Status GoToSafe(){
    	return GoToLocation(safeplace.transform.position);
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

    public Vector3 RandomNavMeshLocation(){
   		Vector3 finalPosition = Vector3.zero;
   		Vector3 randomPosition = Random.insideUnitSphere * WalkRadius;
   		randomPosition += transform.position;
		NavMeshHit hit;
   		if(NavMesh.SamplePosition(randomPosition, out hit, WalkRadius, 1)){
   			finalPosition = hit.position;
   		}
   		return finalPosition;
   	}

    void Update(){
    	if(treeStatus != Node.Status.SUCCESS)
        	treeStatus = tree.Process();
       	else{
       		treeStatus = tree.Process();
       	}
        if(hunger >= 0)
        	hunger -= Time.deltaTime * getHungerRate;
    }

}
