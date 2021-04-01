using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class AntimatterDims : MonoBehaviour {
	public KMBombModule module;
	public KMAudio audio;
	public KMBombInfo bomb;
	public TextMesh text;
	public KMSelectable[] dims;
	public TextMesh[] amounts;
	public KMSelectable tickspeed;
	public TextMesh ticks;
	// ! For going to upgrades :).
	public GameObject[] SwitchSides;
	public KMSelectable switchOfSides;
	public TextMesh SwitchText;
	public KMSelectable[] Upgrades;
	public MeshRenderer[] UpgradeColor;
	public TextMesh[] dimBoostText;
	public KMSelectable BuyDimBoost;
	private static int moduleId = 0;
	double antimatter = 1.00E0;
	double multiplier = 0;
	static double multiplierStuff = 0.1;
	double[] numberOfDims = { 0, 0, 0, 0, 0, 0, 0, 0 };
	bool[] isMax = { false, false, false, false, false, false, false, false, false }; // Dim1, Dim2, Dim3, Dim4, Dim5, Dim6, Dim7, Dim8, Ticks.
	float tickSpeed = 0.1f;
	bool[] isDone = { false, false, false, false, false, false, false};
	bool MainIsOn = true, UpgradesIsOn = false, DimBoostIsOn = false;
	int newMulti = 1;
	int eighthDimMulti = 1;
	int upgradeThreeValid = 0;
	int upgradeFourValid = 0;
	int upgradeFiveValid = 0;
	int upgradeSixValid = 0;
	double[] priceOf = {1.00E0, 1.00E4, 1.00E7, 1.00E10, 1.00E13, 1.00E16, 1.00E19, 1.00E22};
	double[] priceOfDims = {1.00E0, 1.00E4, 1.00E7, 1.00E10, 1.00E13, 1.00E16, 1.00E19, 1.00E22};
	DimBoosting boost = new DimBoosting();
	private static readonly Vector3 underModule = new Vector3(0, -0.0061f, 0), overModule = new Vector3(0, 0, 0);
	#region mainObject
	IEnumerator antimatterGainer(){
		yield return null;
		while (!(antimatter >= 1.00E308)){
			antimatter+=(1+multiplier)*(1+numberOfDims[0])*(1+numberOfDims[1])*(1+numberOfDims[2])*(1+numberOfDims[3])*(1+numberOfDims[4])*(1+numberOfDims[5])*(1+numberOfDims[6])*(1+numberOfDims[7]);
			text.text = antimatter.ToString("E2");
			changeMultiplier();
			yield return new WaitForSeconds(tickSpeed);
		}
	}
	IEnumerator giveDims(){
		yield return null;
		while (!(antimatter >= 1.00E308)) {
			numberOfDims [0] += numberOfDims [1]*(1+((boost.getAmount())*1000))*(1+(upgradeThreeValid*numberOfDims[7]))*(1+(antimatter%1.00E20*upgradeFourValid) * (1+(upgradeFiveValid*10)));	
			numberOfDims [1] += numberOfDims [2]*(1+((boost.getAmount())*1000));
			numberOfDims [2] += numberOfDims [3]*(1+((boost.getAmount())*1000));
			numberOfDims [3] += numberOfDims [4]*(1+((boost.getAmount())*1000));
			numberOfDims [4] += numberOfDims [5]*(1+((boost.getAmount())*1000));
			numberOfDims [5] += numberOfDims [6]*(1+((boost.getAmount())*(1000*(1+(10*upgradeSixValid)))));
			numberOfDims [6] += numberOfDims [7]*eighthDimMulti*(1+(boost.getAmount()*1000));
			for (int i = 0; i < amounts.Length; i++) {
				amounts[i].text = numberOfDims [i].ToString("E2");
				if (double.Parse (amounts [i].text) == 1.7E308)
					isMax [i] = true;
			}
			ticks.text = tickSpeed.ToString ("E2");
			yield return new WaitForSeconds (tickSpeed);
		}
		module.HandlePass();
	}
	void Awake(){
		moduleId++;
		dimBoostText[0].text = boost.getAmount().ToString();
		dimBoostText[1].text = boost.getPrice().ToString("E2");
	}
	void Start(){
		StartCoroutine(antimatterGainer());
		StartCoroutine(giveDims());
		switchOfSides.OnInteract += delegate() {
			if (MainIsOn){
				SwitchSides[0].transform.localPosition = underModule;
				SwitchSides[1].transform.localPosition = overModule;
				SwitchSides[2].transform.localPosition = underModule;
				SwitchText.text = "Upgrades";
				MainIsOn = false;
				UpgradesIsOn = true;
			}
			else if (UpgradesIsOn){
				SwitchSides[0].transform.localPosition = underModule;
				SwitchSides[1].transform.localPosition = underModule;
				SwitchSides[2].transform.localPosition = overModule;
				SwitchText.text = "Dim Boost";
				UpgradesIsOn = false;
				DimBoostIsOn = true;
			}
			else{
				SwitchSides[0].transform.localPosition = overModule;
				SwitchSides[1].transform.localPosition = underModule;
				SwitchSides[2].transform.localPosition = underModule;
				SwitchText.text = "Main Game";
				DimBoostIsOn = false;
				MainIsOn = true;
			}
			return false;
		};
		dims [0].OnInteract += delegate() {
			if (!isMax[0] && antimatter >= priceOfDims[0])numberOfDims [0]+=10;
			priceOfDims[0] += 1.00E3;
			return false;
		};
		dims [1].OnInteract += delegate() {
			if (!isMax[1] && antimatter >= priceOfDims[1])numberOfDims [1]+=10;
			priceOfDims[1] += 1.00E3;
			return false;
		};
		dims [2].OnInteract += delegate() {
			if (!isMax[2] && antimatter >= priceOfDims[2])numberOfDims [2]+=10;
			priceOfDims[2] += 1.00E3;
			return false;
		};
		dims [3].OnInteract += delegate() {
			if (!isMax[3] && antimatter >= priceOfDims[3])numberOfDims [3]+=10;
			priceOfDims[3] += 1.00E3;
			return false;
		};
		dims [4].OnInteract += delegate() {
			if (!isMax[4] && antimatter >= priceOfDims[4])numberOfDims [4]+=10;
			priceOfDims[4] += 1.00E3;
			return false;
		};
		dims [5].OnInteract += delegate() {
			if (!isMax[5] && antimatter >= priceOfDims[5])numberOfDims [5]+=10;
			priceOfDims[5] += 1.00E3;
			return false;
		};
		dims [6].OnInteract += delegate() {
			if (!isMax[6] && antimatter >= priceOfDims[6])numberOfDims [6]+=10;
			priceOfDims[6] += 1.00E3;
			return false;
		};
		dims [7].OnInteract += delegate() {
			if (!isMax[7] && antimatter >= priceOfDims[7])numberOfDims [7]+=10;
			priceOfDims[7] += 1.00E3;
			return false;
		};
		tickspeed.OnInteract += delegate() {
			if (!isMax[8])tickSpeed /= 1.5f;
			return false;
		};
		Upgrades[0].OnInteract += delegate() {
			if (!isDone[0] && antimatter >= 1.00E10) Upgrade1();
			return false;
		};
		Upgrades[1].OnInteract += delegate() {
			if (!isDone[1] && antimatter >= 1.00E30) Upgrade2();
			return false;
		};
		Upgrades[2].OnInteract += delegate() {
			if (!isDone[2] && antimatter >= 1.00E60) Upgrade3();
			return false;
		};
		Upgrades[3].OnInteract += delegate() {
			if (!isDone[3] && antimatter >= 1.00E80) Upgrade4();
			return false;
		};
		Upgrades[4].OnInteract += delegate() {
			if (!isDone[4] && antimatter >= 1.00E110) Upgrade5();
			return false;
		};
		Upgrades[5].OnInteract += delegate() {
			if (!isDone[5] && antimatter >= 1.00E210) Upgrade6();
			return false;
		};
		Upgrades[6].OnInteract += delegate() {
			if (!isDone[6] && antimatter >= 1.00E300) Upgrade7();
			return false;
		};
		BuyDimBoost.OnInteract += delegate() {
			if (antimatter >= boost.getPrice()){
				boost.changeAmount();
				boost.priceChange();
				dimBoostText[0].text = boost.getAmount().ToString();
				dimBoostText[1].text = boost.getPrice().ToString("E2");
				antimatter = 0;
				multiplier = 0;
				multiplierStuff = 0.1;
				for (int i = 0; i < numberOfDims.Length; i++){numberOfDims[i]=0; priceOfDims[i]=priceOf[i];}

			}
			return false;
		};
	}
	void changeMultiplier(){
		multiplier += (newMulti*numberOfDims [0] * multiplierStuff);
	}
	#endregion
	#region Upgrades
	/// <summary>
	/// Implements the 1st upgrade button when you press the switch button.
	/// </summary>
	void Upgrade1(){
		newMulti = 100;
		UpgradeColor[0].material.color = new Color32(0, 75, 0, 255);
		isDone[0] = true;
	}
	/// <summary>
	/// Implements the 2nd upgrade button when you press the switch button.
	/// </summary>
	void Upgrade2(){
		eighthDimMulti = 80;
		UpgradeColor[1].material.color = new Color32(0, 75, 0, 255);
		isDone[1] = true;
	}
	/// <summary>
	/// Implements the 3rd upgrade button when you press the switch button.
	/// </summary>
	void Upgrade3(){
		upgradeThreeValid = 1;
		UpgradeColor[2].material.color = new Color32(0, 75, 0, 255);
		isDone[2] = true;
	}
	/// <summary>
	/// Implements the 4th upgrade button when you press the switch button.
	/// </summary>
	void Upgrade4(){
		upgradeFourValid = 1;
		UpgradeColor[3].material.color = new Color32(0, 75, 0, 255);
		isDone[3] = true;
	}
	/// <summary>
	/// Implements the 5th upgrade button when you press the switch button.
	/// </summary>
	void Upgrade5(){
		upgradeFiveValid = 1;
		UpgradeColor[4].material.color = new Color32(0, 75, 0, 255);
		isDone[4] = true;
	}
	/// <summary>
	/// Implements the 6th upgrade button when you press the switch button.
	/// </summary>
	void Upgrade6(){
		upgradeSixValid = 1;
		UpgradeColor[5].material.color = new Color32(0, 75, 0, 255);
		isDone[5] = true;
	}
	// Apparently useless fucking code.
	/// <summary>
	/// Implements the buy infinity button.
	/// </summary>
	void Upgrade7(){
		module.HandlePass();
		UpgradeColor[6].material.color = new Color32(0, 75, 0, 255);
		isDone[6] = true;
	}
	#endregion
	#region DimBoosting
	class DimBoosting{
		public DimBoosting(){amountOfDimBoost=0; price=1.00E20;}
		public DimBoosting(int data){this.data = data;}
		public void changeAmount(){amountOfDimBoost++;}
		public void priceChange(){price*=((amountOfDimBoost+1)*100);}
		public int getAmount(){return amountOfDimBoost;}
		public double getPrice(){return price;}
		private int amountOfDimBoost;
		private double price;
		// Ignore this bit.
		private int data; // 0;
	};
	/*
	DimBoosting boost = new DimBoosting();
	boost.getAmount(); // returns 0
	DimBoosting newBoost = new DimBoosting();
	newBoost.getAmount(); // 0;
	boost.amountOfDimBoost = 2;
	newBoost.amountOfDimBoost = 3;
	boost.getAmount() // 2 // + newBoost.getAmount(); // 3 // returns 5
	*/
	#endregion
	#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} dim [1-9] (Presses the dimension you want.) | !{0} tick (Presses the tickspeed button) | These first two are only valid if you are on the main game section of the module.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*Dim|tick\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {   
            yield return null;
            if (parameters.Length < 2 && !Regex.IsMatch(parameters[0], @"^\s*tick\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            {
                yield return "sendtochaterror Please specify what number you would like to press!";
            }
            else if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many arguements!";
            }
            else if (!MainIsOn)
            {
				yield return "sendtochaterror You must be on the main game to press a Dimension.";
            }
			else if (parameters[0] == "dim")
			{
				int j = 0;
				int.TryParse(parameters[1], out j);
				if (j >= 1 && j <= 8) dims[j-1].OnInteract();
				else yield return "sendtochaterror Either too small or too large!";
			}
			else{
				tickspeed.OnInteract();
			}
        }
		else if (Regex.IsMatch(parameters[0], @"^\s*change\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
			yield return null;
			if (parameters.Length > 1){
				yield return "sendtochaterror You don't need any arguments to change the section!";
			}
			else{
				switchOfSides.OnInteract();
			}
		}
		else if (Regex.IsMatch(parameters[0], @"^\s*upgrade\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
			yield return null;
            if (parameters.Length < 2)
            {
                yield return "sendtochaterror Please specify what number you would like to press!";
            }
            else if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many arguements!";
			}
			else if (!UpgradesIsOn){
				yield return "sendtochaterror Be on the upgrades section!";
			}
			else{
				int j = 0;
				int.TryParse(parameters[1], out j);
				if (j >= 1 && j <= 8) Upgrades[j-1].OnInteract();
				else yield return "sendtochaterror Either too small or too large!";
			}
		}
		else if (Regex.IsMatch(parameters[0], @"^\s*boost\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
			yield return null;
			if (parameters.Length > 1){
				yield return "sendtochaterror Why are you buying multiple?";
			}
			else{
				BuyDimBoost.OnInteract();
				switchOfSides.OnInteract();
			}
		}
    }
    IEnumerator TwitchHandleForcedSolve()
    {
		yield return null;
    }

}