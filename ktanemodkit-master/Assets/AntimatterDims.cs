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
    public KMAudio Audio;
    public KMBombInfo bomb;
    public TextMesh text;
    public KMSelectable[] dims;
    public KMSelectable[] singledims;
    public TextMesh[] singleprices;
    public TextMesh[] until10;
    public TextMesh[] amounts;
    public TextMesh[] multis;
    public TextMesh tickpricebutitstext;
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
	public KMSelectable buyMax;
    private static int moduleId = 0;
    double antimatter = 1.00E1;
	double multiplier = 0E0;
    static double multiplierStuff = 0.1;
	double[] numberOfDims = { 0E0, 0E0, 0E0, 0E0, 0E0, 0E0, 0E0, 0E0};
    bool[] isMax = { false, false, false, false, false, false, false, false, false }; // Dim1, Dim2, Dim3, Dim4, Dim5, Dim6, Dim7, Dim8, Ticks.
    float tickSpeed = 0.1f;
    int tickManager = 10;
    bool[] isDone = { false, false, false, false, false, false, false };
    bool MainIsOn = true, UpgradesIsOn = false, DimBoostIsOn = false;
    int newMulti = 1;
    int eighthDimMulti = 1;
    int upgradeThreeValid = 0;
    int upgradeFourValid = 0;
    int upgradeFiveValid = 0;
    int upgradeSixValid = 0;
    double[] priceOf = { 1.00E1, 1.00E2, 1.00E4, 1.00E6, 1.00E9, 1.00E13, 1.00E18, 1.00E24 };
    double[] priceOfDims = { 1.00E1, 1.00E2, 1.00E4, 1.00E6, 1.00E9, 1.00E13, 1.00E18, 1.00E24 };
	double[] priceOfDimsx10 = { 1.00E2, 1.00E3, 1.00E5, 1.00E7, 1.00E10, 1.00E14, 1.00E19, 1.00E25 }; // Only used for buy max.
    double[] Multipliers = new double[8] {1, 1, 1, 1, 1, 1, 1, 1};
    double tickPrice = 1.00E3;
    double[] UpgradeCount = new double[8];
	bool buyMaxToggle = false;
	DimBoosting boost = new DimBoosting();
	private static readonly Vector3 underModule = new Vector3(0, -0.0061f, 0), overModule = new Vector3(0, 0, 0);
	#region mainObject
	IEnumerator antimatterGainer(){
		yield return null;
		while (!(antimatter >= 1.00E308)){
			antimatter+=numberOfDims[0]*Multipliers[0]*0.1*newMulti;
			text.text = antimatter.ToString("E2");
			yield return new WaitForSeconds(tickSpeed);
		}
	}
	IEnumerator giveDims(){
		yield return null;
		while (!(antimatter >= 1.00E308)) {
            tickManager++;
            if (tickManager >= 10)
            {
                numberOfDims[0] += numberOfDims[1] * Multipliers[1] * ((boost.getAmount()) * 1000) * (1 + (upgradeThreeValid * numberOfDims[7])) * (1 + (antimatter % 1.00E20 * upgradeFourValid) * (1 + (upgradeFiveValid * 10)));
                numberOfDims[1] += numberOfDims[2] * Multipliers[2] * ((boost.getAmount()) * 1000);
                numberOfDims[2] += numberOfDims[3] * Multipliers[3] * ((boost.getAmount()) * 1000);
                numberOfDims[3] += numberOfDims[4] * Multipliers[4] * ((boost.getAmount()) * 1000);
                numberOfDims[4] += numberOfDims[5] * Multipliers[5] * ((boost.getAmount()) * 1000);
				numberOfDims[5] += numberOfDims[6] * Multipliers[6] * ((boost.getAmount()*1000) * (10 * (1 + (10 * upgradeSixValid))));
                numberOfDims[6] += numberOfDims[7] * Multipliers[7] * eighthDimMulti * (boost.getAmount() * 1000);
                tickManager = 0;
            }
            tickpricebutitstext.text = tickPrice.ToString("E0");
            for (int i = 0; i < amounts.Length; i++) {
				amounts[i].text = numberOfDims [i].ToString("E2");
				multis[i].text = "(                     x" + Multipliers[i].ToString("E0") + ")";
                singleprices[i].text = priceOfDims[i].ToString("E0");
                until10[i].text = (priceOfDims[i] * (10 - UpgradeCount[i])).ToString("E0");
                if (double.Parse (amounts [i].text) == 1.7E308)
					isMax [i] = true;
			}
			ticks.text = tickSpeed.ToString("E2");
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
        singledims[0].OnInteract += delegate () {
            if (MainIsOn)
            if (!isMax[0] && antimatter >= priceOfDims[0])
            {
                numberOfDims[0]++;
                UpgradeCount[0]++;
                antimatter = antimatter - priceOfDims[0];
                if (UpgradeCount[0] == 10)
                {
                    Multipliers[0] = Multipliers[0] * 2;
                    UpgradeCount[0] = 0;
                    priceOfDims[0] = priceOfDims[0] * 1.00E3;
                }
            }
            return false;
        };
        singledims[1].OnInteract += delegate () {
            if (MainIsOn)
                if (!isMax[1] && antimatter >= priceOfDims[1])
            {
                numberOfDims[1]++;
                UpgradeCount[1]++;
                antimatter = antimatter - priceOfDims[1];
                if (UpgradeCount[1] == 10)
                {
                    Multipliers[1] = Multipliers[1] * 2;
                    UpgradeCount[1] = 0;
                    priceOfDims[1] = priceOfDims[1] * 1.00E4;
                }
            }
            return false;
        };
        singledims[2].OnInteract += delegate () {
            if (MainIsOn)
                if (!isMax[2] && antimatter >= priceOfDims[2])
            {
                numberOfDims[2]++;
                UpgradeCount[2]++;
                antimatter = antimatter - priceOfDims[2];
                if (UpgradeCount[2] == 10)
                {
                    Multipliers[2] = Multipliers[2] * 2;
                    UpgradeCount[2] = 0;
                    priceOfDims[2] = priceOfDims[2] * 1.00E5;
                }
            }
            return false;
        };
        singledims[3].OnInteract += delegate () {
            if (MainIsOn)
                if (!isMax[3] && antimatter >= priceOfDims[3])
            {
                numberOfDims[3]++;
                UpgradeCount[3]++;
                antimatter = antimatter - priceOfDims[3];
                if (UpgradeCount[3] == 10)
                {
                    Multipliers[3] = Multipliers[3] * 2;
                    UpgradeCount[3] = 0;
                    priceOfDims[3] = priceOfDims[3] * 1.00E6;
                }
            }
            return false;
        };
        singledims[4].OnInteract += delegate () {
            if (MainIsOn)
                if (!isMax[4] && antimatter >= priceOfDims[4])
            {
                numberOfDims[4]++;
                UpgradeCount[4]++;
                antimatter = antimatter - priceOfDims[4];
                if (UpgradeCount[4] == 10)
                {
                    Multipliers[4] = Multipliers[4] * 2;
                    UpgradeCount[4] = 0;
                    priceOfDims[4] = priceOfDims[4] * 1.00E8;
                }
            }
            return false;
        };
        singledims[5].OnInteract += delegate () {
            if (MainIsOn)
                if (!isMax[5] && antimatter >= priceOfDims[5])
            {
                numberOfDims[5]++;
                UpgradeCount[5]++;
                antimatter = antimatter - priceOfDims[5];
                if (UpgradeCount[5] == 10)
                {
                    Multipliers[5] = Multipliers[5] * 2;
                    UpgradeCount[5] = 0;
                    priceOfDims[5] = priceOfDims[5] * 1.00E10;
                }
            }
            return false;
        };
        singledims[6].OnInteract += delegate () {
            if (MainIsOn)
                if (!isMax[6] && antimatter >= priceOfDims[6])
            {
                numberOfDims[6]++;
                UpgradeCount[6]++;
                antimatter = antimatter - priceOfDims[6];
                if (UpgradeCount[6] == 10)
                {
                    Multipliers[6] = Multipliers[6] * 2;
                    UpgradeCount[6] = 0;
                    priceOfDims[6] = priceOfDims[6] * 1.00E12;
                }
            }
            return false;
        };
        singledims[7].OnInteract += delegate () {
            if (MainIsOn)
                if (!isMax[7] && antimatter >= priceOfDims[7])
            {
                numberOfDims[7]++;
                UpgradeCount[7]++;
                antimatter = antimatter - priceOfDims[7];
                if (UpgradeCount[7] == 10)
                {
                    Multipliers[7] = Multipliers[7] * 2;
                    UpgradeCount[7] = 0;
                    priceOfDims[7] = priceOfDims[7] * 1.00E15;
                }
            }
            return false;
        };
        dims [0].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[0] && antimatter >= priceOfDims[0] * (10 - UpgradeCount[0]))
            {
                numberOfDims[0] += 10 - UpgradeCount[0];
                antimatter = antimatter - priceOfDims[0] * (10 - UpgradeCount[0]);
                Multipliers[0] = Multipliers[0] * 2;
                UpgradeCount[0] = 0;
                priceOfDims[0] = priceOfDims[0] * 1.00E3;
            }
			return false;
		};
		dims [1].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[1] && antimatter >= priceOfDims[1] * (10 - UpgradeCount[1]))
            {
                numberOfDims[1] += 10 - UpgradeCount[1];
                antimatter = antimatter - priceOfDims[1] * (10 - UpgradeCount[1]);
                Multipliers[1] = Multipliers[1] * 2;
                UpgradeCount[1] = 0;
                priceOfDims[1] = priceOfDims[1] * 1.00E4;
            }
			return false;
		};
		dims [2].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[2] && antimatter >= priceOfDims[2] * (10 - UpgradeCount[2]))
            {
                numberOfDims[2] += 10 - UpgradeCount[2];
                antimatter = antimatter - priceOfDims[2] * (10 - UpgradeCount[2]);
                Multipliers[2] = Multipliers[2] * 2;
                UpgradeCount[2] = 0;
                priceOfDims[2] = priceOfDims[2] * 1.00E5;
            }
			return false;
		};
		dims [3].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[3] && antimatter >= priceOfDims[3] * (10 - UpgradeCount[3]))
            {
                numberOfDims[3] += 10 - UpgradeCount[3];
                antimatter = antimatter - priceOfDims[3] * (10 - UpgradeCount[3]);
                Multipliers[3] = Multipliers[3] * 2;
                UpgradeCount[3] = 0;
                priceOfDims[3] = priceOfDims[3] * 1.00E6;
            }
			return false;
		};
		dims [4].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[4] && antimatter >= priceOfDims[4] * (10 - UpgradeCount[4]))
            {
                numberOfDims[4] += 10 - UpgradeCount[4];
                antimatter = antimatter - priceOfDims[4] * (10 - UpgradeCount[4]);
                Multipliers[4] = Multipliers[4] * 2;
                UpgradeCount[4] = 0;
                priceOfDims[4] = priceOfDims[4] * 1.00E8;
            }
			return false;
		};
		dims [5].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[5] && antimatter >= priceOfDims[5] * (10 - UpgradeCount[5]))
            	{
                	numberOfDims[5] += 10 - UpgradeCount[5];
                	antimatter = antimatter - priceOfDims[5] * (10 - UpgradeCount[5]);
                	Multipliers[5] = Multipliers[5] * 2;
                	UpgradeCount[5] = 0;
                	priceOfDims[5] = priceOfDims[5] * 1.00E10;
            	}
			return false;
		};
		dims [6].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[6] && antimatter >= priceOfDims[6] * (10 - UpgradeCount[6]))
            {
                numberOfDims[6] += 10 - UpgradeCount[6];
                antimatter = antimatter - priceOfDims[6] * (10 - UpgradeCount[6]);
                Multipliers[6] = Multipliers[6] * 2;
                UpgradeCount[6] = 0;
                priceOfDims[6] = priceOfDims[6] * 1.00E12;
            }
			return false;
		};
		dims [7].OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[7] && antimatter >= priceOfDims[7] * (10 - UpgradeCount[7]))
            {
                numberOfDims[7] += 10 - UpgradeCount[7];
                antimatter = antimatter - priceOfDims[7] * (10 - UpgradeCount[7]);
                Multipliers[7] = Multipliers[7] * 2;
                UpgradeCount[7] = 0;
                priceOfDims[7] = priceOfDims[7] * 1.00E15;
            }
			return false;
		};
		tickspeed.OnInteract += delegate() {
            if (MainIsOn)
                if (!isMax[8] && antimatter >= tickPrice) { tickSpeed /= 100f; antimatter = antimatter - tickPrice; tickPrice = tickPrice * 10; }
			return false;
		};
		Upgrades[0].OnInteract += delegate() {
            if (UpgradesIsOn)
                if (!isDone[0] && antimatter >= 1.00E10) { Upgrade1(); antimatter = antimatter - 1.00E10; }
			return false;
		};
		Upgrades[1].OnInteract += delegate() {
            if (UpgradesIsOn)
                if (!isDone[1] && antimatter >= 1.00E30) { Upgrade2(); antimatter = antimatter - 1.00E30; }
            return false;
		};
		Upgrades[2].OnInteract += delegate() {
            if (UpgradesIsOn)
                if (!isDone[2] && antimatter >= 1.00E60) { Upgrade3(); antimatter = antimatter - 1.00E60; }
            return false;
		};
		Upgrades[3].OnInteract += delegate() {
            if (UpgradesIsOn)
                if (!isDone[3] && antimatter >= 1.00E80) { Upgrade4(); antimatter = antimatter - 1.00E80; }
            return false;
		};
		Upgrades[4].OnInteract += delegate() {
            if (UpgradesIsOn)
                if (!isDone[4] && antimatter >= 1.00E110) { Upgrade5(); antimatter = antimatter - 1.00E110; }
            return false;
		};
		Upgrades[5].OnInteract += delegate() {
            if (UpgradesIsOn)
                if (!isDone[5] && antimatter >= 1.00E210) { Upgrade6(); antimatter = antimatter - 1.00E210; }
            return false;
		};
		Upgrades[6].OnInteract += delegate() {
            if (UpgradesIsOn)
                if (!isDone[6] && antimatter >= 1.00E300) { Upgrade7(); antimatter = antimatter - 1.00E300; }
            return false;
		};
		BuyDimBoost.OnInteract += delegate() {
            if (DimBoostIsOn)
                if (antimatter >= boost.getPrice()){
				boost.changeAmount();
                for (int i = 0; i < 8; i++)
                {
                    UpgradeCount[i] = 0;
                    Multipliers[i] = 1;
                    multis[i].text = "(                     x1.0)";
                }
                    boost.priceChange();
				dimBoostText[0].text = boost.getAmount().ToString("E2");
				dimBoostText[1].text = boost.getPrice().ToString("E2");
				antimatter = 1.00E1;
                tickSpeed = 0.1f;
                tickPrice = 1.00E3;
                multiplier = 0;
				multiplierStuff = 0.1;
                for (int i = 0; i < numberOfDims.Length; i++){numberOfDims[i]=0; priceOfDims[i]=priceOf[i];}
				Debug.LogFormat("Number of dim boosts: {0}", boost.getAmount());
			}
			return false;
		};
		buyMax.OnInteract += delegate() {
			if (MainIsOn){
				double highestPrice = priceOfDimsx10.Max();
				while (antimatter >= highestPrice){
					for (int i = 0; i < 8; ++i){
						dims[i].OnInteract();
					}
				}
			}
			return false;
		};
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
		public DimBoosting(){amountOfDimBoost=1E0; price=1.00E20;}
		public DimBoosting(int data){this.data = data;}
		public void changeAmount(){amountOfDimBoost *= 1000;}
		public void priceChange(){price*=((amountOfDimBoost+1)*100);}
		public double getAmount(){return amountOfDimBoost;}
		public double getPrice(){return price;}

		[SerializeField]
		private double amountOfDimBoost;

		private double price;
		// Ignore this bit.
		private int data; // 0;
	};
	/*
	DimBoosting boost = new DimBoosting();
	boost.getAmount(); // returns 0ich
	DimBoosting newBoost = new DimBoosting();
	newBoost.getAmount(); // 0;
	boost.amountOfDimBoost = 2;
	newBoost.amountOfDimBoost = 3;
	boost.getAmount() // 2 // + newBoost.getAmount(); // 3 // returns 5
	*/
	#endregion
	#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} dim [1-8] (temp #) (Presses the single dimension you want a # of times.) | !{0} dimx10 [1-8] (temp #) (Presses the x10 button of the dimension given a # of times.) | !{0} tick (Presses the tickspeed button) | These first two are only valid if you are on the main game section of the module. | !{0} change (presses the button on the bottom to change the screen) | !{0} upgrade [1-8] (presses the upgrade from 1 to 8, and must be on the upgrade screen) | !{0} boost (presses the boost button, must be on the dim boost page)";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*Dim|tick\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {   
            yield return null;
			int l = parameters.Length;
			if (l < 2 && !Regex.IsMatch (parameters [0], @"^\s*tick\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
				yield return "sendtochaterror Please specify what number you would like to press!";
			} else if (l > 3) {
				yield return "sendtochaterror Too many arguements!";
			} else if (!MainIsOn) {
				yield return "sendtochaterror You must be on the main game to press a Dimension.";
			} else if (parameters [0] == "dim") {
				int j = 0;
				int.TryParse (parameters [1], out j);
				if (j >= 1 && j <= 8) {
					if (l == 2)
						singledims [j - 1].OnInteract ();
					else {
						int k = 0;
						int.TryParse (parameters [2], out k);
						for (int i = 0; i < k; i++) {
							singledims [j - 1].OnInteract ();
						}
					}
				}
				else
					yield return "sendtochaterror Either too small or too large!";
			} else if (parameters [0] == "dimx10") {
				int j = 0;
				int.TryParse (parameters [1], out j);
				if (j >= 1 && j <= 8)
				if (l == 2)
					dims [j - 1].OnInteract ();
				else {
					int k = 0;
					int.TryParse (parameters [2], out k);
					for (int i = 0; i < k; i++) {
						dims [j - 1].OnInteract ();
					}
				}
				else
					yield return "sendtochaterror Either too small or too large!";
			}
			else{
				int j = 0;
				int.TryParse (parameters [1], out j);
				for (int i = 0; i < j; i++)
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