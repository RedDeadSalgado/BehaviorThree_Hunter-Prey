using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node{
	//Puede tener 3 estados cada nodo.
	public enum Status{SUCCESS, RUNNING, FAILURE};
	//Aqui guardamos el estado.
	public Status status;
	//Cada  nodo puede tener hijos, asique ponemos su respectiva  lista.
	public List<Node> children = new List<Node>();
	//Para saber en que hijo se encuentra, comenzamos en 0.
	public int currentChild = 0;
	//Le daremos un nombre a cada nombre para ver que onda.
	public string name;

	//Construcctor sin nada. C# things.
	public Node(){}
	//Constructor
	public Node(string n){
		name = n;
	}

	public virtual Status Process(){
		return children[currentChild].Process();
	}

	//Aqui agregamos a los hijos.
	public void AddChild(Node n){
		children.Add(n);
	}
}
