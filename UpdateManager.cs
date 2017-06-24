/*Copyright (c) Kristin Stock - itsMilkid
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateManager : MonoBehaviour {

	protected static UpdateManager instance;

	protected ManagedMonoBehaviour[] update = new ManagedMonoBehaviour[0];
	protected ManagedMonoBehaviour[] fixedUpdate = new ManagedMonoBehaviour[0];
	protected ManagedMonoBehaviour[] lateUpdate = new ManagedMonoBehaviour[0];

	protected int updateCount;
	protected int fixedUpdateCount;
	protected int lateUpdateCount;

	public UpdateManager(){ instance = this;}

	private void Start(){
		Scene currentScene = SceneManager.GetActiveScene();
		GameObject[] rootObjects = currentScene.GetRootGameObjects();

		//Searching the scene for objects with managed MonoBehaviours and adding them to the array
		for(int i = 0; i <rootObjects.Length; i++){
			Component[] objectBehaviours = rootObjects[i].GetComponents(typeof(ManagedMonoBehaviour));
			if(objectBehaviours.Length > 0){
				for(int j = 0;  j < objectBehaviours.Length; j++){
					ManagedMonoBehaviour newBehaviour = (ManagedMonoBehaviour) objectBehaviours[j];
					Add(newBehaviour);
				}
			}
		}
	}

	private void Update(){
			if(updateCount == 0){
			return;
		}
		
		//Iterating through all the Update calls
		for(int i = 0; i < updateCount; i++){
			if(update[i] == null){
				continue;
			}
			update[i].ManagedUpdate();
		}
	}

	private void FixedUpdate(){
			if(fixedUpdateCount == 0){
			return;
		}
		
		//Iterating through all the FixedUpdate calls
		for(int i = 0; i < fixedUpdateCount; i++){
			if(fixedUpdate[i] == null){
				continue;
			}
			fixedUpdate[i].ManagedFixedUpdate();
		}
	}

	private void LateUpdate(){
		if(lateUpdateCount == 0){
			return;
		}
		
		//Iterating through all the LateUpdate calls
		for(int i = 0; i < lateUpdateCount; i++){
			if(lateUpdate[i] == null){
				continue;
			}
			lateUpdate[i].ManagedLateUpdate();
		}
	}


	public static void Add(ManagedMonoBehaviour _behaviour){
		instance.AddToArray(_behaviour);
	}

	public static void Remove(ManagedMonoBehaviour _behaviour){
		instance.RemoveFromArray(_behaviour);
	}

	public static void RemoveAndDestroy(ManagedMonoBehaviour _behaviour){
		instance.RemoveFromArray(_behaviour);
		Destroy(_behaviour.gameObject);
	}

	private void AddToArray(ManagedMonoBehaviour _behaviour){
		if(_behaviour.GetType().GetMethod("ManagedUpdate").DeclaringType != typeof(ManagedMonoBehaviour)){
			update = ExtendArrayAndAddBehaviour(update,_behaviour);
			updateCount++;
		} else if(_behaviour.GetType().GetMethod("ManagedFixedUpdate").DeclaringType != typeof(ManagedMonoBehaviour)){
			fixedUpdate = ExtendArrayAndAddBehaviour(fixedUpdate,_behaviour);
			fixedUpdateCount++;
		} else if(_behaviour.GetType().GetMethod("ManagedLateUpdate").DeclaringType != typeof(ManagedMonoBehaviour)){
			lateUpdate = ExtendArrayAndAddBehaviour(lateUpdate,_behaviour);
			lateUpdateCount++;
		} else {
			return;
		}
	}

	private void RemoveFromArray(ManagedMonoBehaviour _behaviour){
		if(ArrayContains(update,_behaviour)){
			update = RetractArrayAndRemoveBehaviour(update,_behaviour);
			updateCount--;
		} else if(ArrayContains(fixedUpdate,_behaviour)){
			fixedUpdate = RetractArrayAndRemoveBehaviour(fixedUpdate,_behaviour);
			fixedUpdateCount--;
		} else if(ArrayContains(lateUpdate,_behaviour)){
			lateUpdate = RetractArrayAndRemoveBehaviour(lateUpdate,_behaviour);
			lateUpdateCount--;
		} else {
			return;
		}


	}

	public bool ArrayContains(ManagedMonoBehaviour[] _array, ManagedMonoBehaviour _behaviour){
		int length = _array.Length;

		for(int i = 0; i < length; i++){
			if(_behaviour == _array[i]){
				return true;
			} 
		}

		return false;
	}

	public ManagedMonoBehaviour[] ExtendArrayAndAddBehaviour(ManagedMonoBehaviour[] _array, ManagedMonoBehaviour _behaviour){
		int length = _array.Length;
		ManagedMonoBehaviour[] extendedArray = new ManagedMonoBehaviour[length + 1];

		for(int i = 0; i <length;i++){
			extendedArray[i] = _array[i];
		}

		extendedArray[extendedArray.Length - 1] = _behaviour;
		return extendedArray;
	}

	public ManagedMonoBehaviour[] RetractArrayAndRemoveBehaviour(ManagedMonoBehaviour[] _array, ManagedMonoBehaviour _behaviour){
		int length = _array.Length;
		ManagedMonoBehaviour[] retractedArray = new ManagedMonoBehaviour[length -1];

		for(int i = 0; i < length; i++){
			if(_array[i] == _behaviour){
				continue;
			}
			retractedArray[i] = _array[i];
		}

		return retractedArray;
	}
}
