using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterBehaviour : MonoBehaviour{
    
     BehaviourTree tree;
    public Animator animi;
    public Animator animilobo;

	[Range(0,100)] public float range = 5;
	[Range(0,100)] public float hunger = 0;
	[Range(0,100)] public float getHungerRate = 0;
	[Range(1,500)] public float WalkRadius = 5;

	public bool escapo = false;
	public bool muerto = false;

	public GameObject safeplace;
	public GameObject presa;
	public GameObject backdoor;
	public GameObject frontdoor;

	NavMeshAgent agent;
	public NavMeshAgent presita;

	public enum ActionState{ IDLE, WORKING};
	ActionState state = ActionState.IDLE;

	Node.Status treeStatus = Node.Status.RUNNING;

	void Start(){
        agent = this.GetComponent<NavMeshAgent>();
        
        tree = new BehaviourTree();

        Leaf wander = new Leaf("Ando bien relajado",GoToWander);

        Leaf presanear = new Leaf("Buscando",PresainRango);
        Leaf cazar = new Leaf("Lanzate",Persige);
        Sequence depreda = new Sequence("Podre cazar algo?");

        Selector wuattudu = new Selector("Que voy a hacer ahora...");

        depreda.AddChild(presanear);
        depreda.AddChild(cazar);

        wuattudu.AddChild(depreda);
        wuattudu.AddChild(wander);

        tree.AddChild(wuattudu);

        tree.PrintTree();
    }

    public Node.Status GoToWander(){
    	if(escapo == true)
    		return GoToLocation(safeplace.transform.position);
    	else
    		return GoToLocation(RandomNavMeshLocation());
    }

    public Node.Status PresainRango(){
    	float distance = Vector3.Distance(agent.transform.position, presa.transform.position);
		if(distance <= range){
			Debug.Log( distance + " : " + range);
    		return Node.Status.SUCCESS;
    	}
    	return Node.Status.FAILURE;
    }

    public Node.Status Persige(){
    	float distance = Vector3.Distance(presa.transform.position, agent.transform.position);
    	float distanceFront = Vector3.Distance(frontdoor.transform.position, agent.transform.position);
    	float distanceBack = Vector3.Distance(backdoor.transform.position, agent.transform.position);
    	if(distanceBack < 5f || distanceFront < 5f){
    		escapo = true;
    	}
        if(distance > 2f && escapo == false)
        {
            agent.isStopped = false;
            agent.SetDestination(presa.transform.position);
            return Node.Status.RUNNING;
        }
        else if(escapo == true){
        	range = 0;
        	return Node.Status.SUCCESS;
        }
        else{
        	presita.speed = 0;
        	muerto = true;
            agent.isStopped = true;
            return Node.Status.SUCCESS;
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
    		if(range == 0){
    			range = 10;
    			escapo = false;
    		}
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
        animi.SetBool("death",muerto);
        animilobo.SetBool("muerto",muerto);
    }
}
