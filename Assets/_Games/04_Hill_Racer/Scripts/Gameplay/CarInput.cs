using UnityEngine;
using System.Collections;

public class CarInput : MonoBehaviour {

	public static CarInput Instance = null;

	public CarController carController;

    private void Awake()
    {
		if (Instance == null)
			Instance = this;
    }

    IEnumerator Start ()
	{
		yield return new WaitForSeconds (.3f);
		carController = GameObject.FindObjectOfType<CarController> ();
	}

	public void Gas ()
	{
		carController.Acceleration ();
	}

	public void Brake ()
	{
		carController.Brake ();
	}

	public void ReleaseGasBrake ()
	{
		carController.GasBrakeRelease ();
	}
}
