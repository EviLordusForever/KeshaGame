using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
	[Header("Keybinds")]

	public GameObject inventoryPanel;
	public GameObject smallInventoryPanel;
	public bool opened;

	public int selectedId;
	public int smallSelectedId;
	public int ultraSelectedId;

	public IsItem[] items;

	public GameObject[] panels;
	public GameObject[] smallPanels;

	public TextMeshProUGUI[] numberLabels;
	public TextMeshProUGUI[] smallNumberLabels;

	public GameObject selectorPanel;
	public GameObject selectorPanelPlus;
	public GameObject cursorPanel;
	public GameObject inventoryPanelParentBig;
	public GameObject inventoryPanelParentSmall;

	public Camera camera;
	public Camera showingCamera;
	public Rigidbody prb;

	public LayerMask notTransperent;

	public Sprite emptyImage;

	public GameObject numberLabelExample;

	public float throwTime;
	public float throwSize;
	public bool throwing;
	public int throwCombo;

	public Canvas canvas;

	public long lastKeyTime;
	public string state;

	public float fps;
	public GameObject fpsLabel;
	private TextMeshProUGUI fpsT;

	public GameObject allFather;

	public GameObject throwPanel;
	public GameObject throwPanelBlack;
	public GameObject throwPanelRed;

	public GameObject showingItem;
	public IsItem showingItemIsItem;
	public RawImage showingPanel;
	public GameObject showingOverlay;
	public TextMeshProUGUI showingText;
	public float showingStartTime;
	public AudioManager audioManager;

	private NeedShowItem needShowItem;

	public void Start()
	{
		audioManager.muted = true;

		items = new IsItem[36]; //
		numberLabels = new TextMeshProUGUI[36];
		smallNumberLabels = new TextMeshProUGUI[9];
		fpsT = fpsLabel.GetComponent<TextMeshProUGUI>();
		opened = true;
		state = "not started";

		showingItem = null;
		showingCamera.enabled = false;
		showingCamera.gameObject.SetActive(false);
		showingPanel.gameObject.SetActive(false);
		showingOverlay.SetActive(false);
		

		needShowItem = GetComponent<NeedShowItem>();

		StartCoroutine(LateStart(0.5f));

		IEnumerator LateStart(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);

			Vector2 canvasScale = new Vector2(canvas.transform.lossyScale.x, canvas.transform.lossyScale.y);

			for (int i = 0; i < numberLabels.Length; i++)
			{
				Transform t = panels[i].transform;
				var obj = Instantiate(numberLabelExample);
				obj.transform.parent = t;
				obj.transform.position = t.position + new Vector3(16 * canvasScale.x, 145 * canvasScale.y, 0);
				obj.transform.localScale = new Vector3(0.22f, 0.65f);

				numberLabels[i] = obj.GetComponent<TextMeshProUGUI>();
			}

			for (int i = 0; i < smallNumberLabels.Length; i++)
			{
				Transform t = smallPanels[i].transform;
				var obj = Instantiate(numberLabelExample);
				obj.transform.parent = t;
				obj.transform.position = t.position + new Vector3(11 * canvasScale.x, 104 * canvasScale.y, 0);
				obj.transform.localScale = new Vector3(0.22f, 0.65f);

				smallNumberLabels[i] = obj.GetComponent<TextMeshProUGUI>();
			}

			ultraSelectedId = -1;

			SwitchInventory();
			//SelectItem(0, true);
			SwitchInventory();
			//SelectItem(0, false);
			SwitchInventory();

			selectorPanelPlus.SetActive(false);

			numberLabelExample.SetActive(false);

			Visualize();

			audioManager.muted = false;
		}
	}

	public void Update()
	{
		MyInput();
		fps = MathF.Round(fps * 0.5f + 0.5f / Time.deltaTime);
		fpsT.text = fps.ToString();

		if (showingItem != null)
			showingItem.transform.Rotate(showingItemIsItem.showingRotation.x, showingItemIsItem.showingRotation.y, showingItemIsItem.showingRotation.z, Space.World);
	}

	public void Visualize()
	{
		for (int i = 0; i < panels.Length; i++)
			Visualise(i);
	}

	public void Visualise(int id)
	{
		if (items[id] != null)
		{
			if (needShowItem.NeedOnce(items[id].name))
				ShowItem(id);

			panels[id].GetComponent<Image>().sprite = items[id].image;

			if (items[id].count > 1)
				numberLabels[id].text = Align(items[id].count.ToString());
			else
				numberLabels[id].text = "";

			if (id < 9)
			{
				smallPanels[id].GetComponent<Image>().sprite = items[id].image;

				if (items[id].count > 1)
					smallNumberLabels[id].text = Align(items[id].count.ToString());
				else
					smallNumberLabels[id].text = "";
			}
		}
		else
		{
			panels[id].GetComponent<Image>().sprite = emptyImage;
			numberLabels[id].text = "";
			if (id < 9)
			{
				smallPanels[id].GetComponent<Image>().sprite = emptyImage;
				smallNumberLabels[id].text = "";
			}
		}

		string Align(string s)
		{
			if (s.Length >= 3)
				return s;
			else if (s.Length == 2)
				return ' ' + s;
			else
				return "  " + s;
		}
	}

	private void MyInput()
	{
		if (showingItem != null)
		{
			if (showingStartTime < Time.time - 1)
				if (Input.anyKey)
					HideShowingItem();
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I))
				SwitchInventory();
			if (Input.GetKeyDown(KeyCode.Escape) && opened)
				SwitchInventory();


			if (opened)
			{
				if (Input.GetAxis("Mouse ScrollWheel") < 0f)
					SelectItem(selectedId + 1, false);
				else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
					SelectItem(selectedId - 1, false);

				if (Input.GetKeyDown(KeyCode.Return))
				{
					SelectItem(selectedId, true);
					state = "not started";
				}
				else
				{
					long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
					long deltaTime = time - lastKeyTime;

					bool atLeastOne = false;

					if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
						Method(1, KeyCode.D);
					if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
						Method(-1, KeyCode.A);
					if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
						Method(-9, KeyCode.W);
					if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
						Method(9, KeyCode.S);

					if (!atLeastOne)
						state = "not started";

					void Method(int idDelta, KeyCode keyCode)
					{
						if (state == "not started")
						{
							state = "starting";
							SelectItem(selectedId + idDelta, false);
							lastKeyTime = time;
						}
						else if (state == "starting" && deltaTime > 250)
						{
							state = "started";
							SelectItem(selectedId + idDelta, false);
							lastKeyTime = time;
						}
						else if (state == "started" && deltaTime > 45)
						{
							lastKeyTime = time;
							SelectItem(selectedId + idDelta, false);
							lastKeyTime = time;
						}

						atLeastOne = true;
					}
				}
			}
			else
			{
				if (Input.GetAxis("Mouse ScrollWheel") > 0f)
					SelectItem(smallSelectedId - 1, false);
				if (Input.GetAxis("Mouse ScrollWheel") < 0f)
					SelectItem(smallSelectedId + 1, false);

				if (Input.GetMouseButtonDown(0))
					Click();

				if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(1))
					camera.fieldOfView = 28;
				if (Input.GetKeyUp(KeyCode.C) || Input.GetMouseButtonUp(1))
					camera.fieldOfView = 76; ///////////////////////////////////
			}


			if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Q))
			{
				if (items[selectedId] != null)
				{
					if (!opened)
					{
						throwTime = 0;
						throwing = true;
						if (items[selectedId].throwable)
							throwPanelBlack.transform.localScale = new Vector3(1, 1, 0);
						else
							throwPanelRed.transform.localScale = new Vector3(1, 1, 0);
					}
					else if (items[selectedId].throwable)
						Throw(selectedId, 300);
				}
			}

			if (throwing)
			{
				if (items[selectedId] != null)
				{
					if (items[selectedId].throwable)
					{
						throwTime += Time.deltaTime * Math.Max(Math.Min(MathF.Pow(throwCombo, 0.35f), 10), 1);

						throwSize = Math.Max(throwTime * 1.2f, 0);

						throwPanel.transform.localScale = new Vector3(throwSize, 1, 0);
					}
				}
				else
					StopCombo();
			}

			if (throwing && (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.Q)))
			{
				if (items[selectedId].throwable && (throwCombo == 0 || throwSize > 0.25f))
					Throw(selectedId, throwSize * 1500);

				throwing = false;
				throwPanel.transform.localScale = new Vector3(0, 0, 0);
				throwPanelBlack.transform.localScale = new Vector3(0, 0, 0);
				throwPanelRed.transform.localScale = new Vector3(0, 0, 0);
				throwTime = 0;
				throwCombo = 0;

				throwSize = 0;
			}

			if (throwing && throwSize > 1)
			{
				if (items[selectedId] != null)
				{
					if (items[selectedId].throwable)
					{
						Throw(selectedId, throwSize * 1500);
						throwCombo++;
					}
					else
						StopCombo();
				}
				else
					StopCombo();

				throwTime = 0;
				throwSize = 0;
			}

			void StopCombo()
			{
				throwing = false;
				throwPanel.transform.localScale = new Vector3(0, 0, 0);
				throwPanelBlack.transform.localScale = new Vector3(0, 0, 0);

				throwCombo = 0;
				throwTime = 0;
				throwSize = 0;
			}

			if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
				SelectItem(0, false);
			else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
				SelectItem(1, false);
			else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
				SelectItem(2, false);
			else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
				SelectItem(3, false);
			else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
				SelectItem(4, false);
			else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
				SelectItem(5, false);
			else if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
				SelectItem(6, false);
			else if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8))
				SelectItem(7, false);
			else if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9))
				SelectItem(8, false);
		}
	}

	public void Click()
	{
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 7f, notTransperent))
		{
			IsItem isItem = hit.collider.gameObject.GetComponent<IsItem>();
			if (isItem != null)
				Take(isItem);
		}
	}

	public void Take(IsItem isItem)
	{
		if (items[smallSelectedId] == null)
		{
			items[smallSelectedId] = isItem;
			isItem.Hide();
			isItem.obj.transform.parent = allFather.transform; ///
			Visualise(smallSelectedId);
			audioManager.Play(isItem.pickUpAudioName, 1);
			return;
		}

		if (items[smallSelectedId].name == isItem.name)
		{
			items[smallSelectedId].count += isItem.count;
			isItem.transform.position += new Vector3(0, 1, 0); //
			Destroy(isItem.obj);
			Visualise(smallSelectedId);
			audioManager.Play(isItem.pickUpAudioName, 1);
			return;
		}

		int id = 0;
		for (; id < 36; id++)
			if (items[id] != null)
				if (items[id].name == isItem.name)
				{
					items[id].count += isItem.count;
					isItem.transform.position += new Vector3(0, 1, 0); //
					Destroy(isItem.obj);
					Visualise(id);
					audioManager.Play(isItem.pickUpAudioName, 1);
					return;
				}

		if (id >= 36)
			for (id = 0; id < 36; id++)
				if (items[id] == null)
				{
					items[id] = isItem;
					isItem.Hide();
					isItem.obj.transform.parent = allFather.transform; ///
					Visualise(id);
					audioManager.Play(isItem.pickUpAudioName, 1);
					return;
				}
	}

	public void ShowItem(int id)
	{
		if (items[id] != null)
		{
			if (opened)
				SwitchInventory();

			StartCoroutine(LateShow(items[id].showingDelay));

			IEnumerator LateShow(float delay)
			{
				showingCamera.enabled = true;

				yield return new WaitForSeconds(delay);

				smallInventoryPanel.SetActive(false);

				showingStartTime = Time.time;

				Vector3 position = showingCamera.transform.position;
				Vector3 direction = showingCamera.transform.forward * 3;
				Vector3 offset = new Vector3(0, items[id].showingOffset, 0);

				showingItem = Instantiate(items[id].obj, position + direction + offset, Quaternion.identity);
				showingItemIsItem = showingItem.GetComponent<IsItem>();

				showingItem.transform.eulerAngles = items[id].startShowingRotation;

				audioManager.Play(items[id].showingAudioName, 1);

				showingItem.GetComponent<Renderer>().enabled = true;
				showingCamera.enabled = true;
				showingCamera.gameObject.SetActive(true);
				showingPanel.gameObject.SetActive(true);
				showingOverlay.SetActive(true);
				showingText.text = showingItemIsItem.showingText;
			}
		}
	}

	public void HideShowingItem()
	{
		if (showingItem != null)
		{
			Destroy(showingItem);
			showingItem = null;
			showingCamera.enabled = false;
			showingCamera.gameObject.SetActive(false);
			showingPanel.gameObject.SetActive(false);
			showingOverlay.SetActive(false);
			smallInventoryPanel.SetActive(true);
			audioManager.Play("pickUp", 1.3f);
		}
	}

	public void Throw(int id, float power)
	{
		if (items[id] != null)
		{
			Vector3 position = camera.transform.position;
			Vector3 direction = camera.transform.forward;

			int buffer = items[id].count;
			items[id].count = 1;

			GameObject obj = Instantiate(items[id].obj, position + direction, Quaternion.identity);
			IsItem isItem = obj.GetComponent<IsItem>();
			isItem.Throw(position, direction, power, prb.velocity, camera.transform.rotation); //
			obj.transform.parent = GameObject.Find("Everything").transform;

			items[id].count = buffer - 1;
			items[id].Hide();

			if (items[id].count <= 0)
			{
				Destroy(items[id].obj);
				items[id] = null;
				ultraSelectedId = -1;
				selectorPanelPlus.SetActive(false);
			}

			Visualise(id);
			audioManager.Play("throw", MathF.Min(MathF.Max(MathF.Pow(throwCombo, 0.1f), 1), 5));
		}
	}

	public void SwitchInventory()
	{
		opened = !opened;

		if (opened)
		{
			Cursor.lockState = CursorLockMode.None;
			SelectItem(selectedId, false);
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			SelectItem(smallSelectedId, false);
			ultraSelectedId = -1;
			selectorPanelPlus.SetActive(false); //
		}

		Cursor.visible = opened;

		inventoryPanel.gameObject.SetActive(opened);
		smallInventoryPanel.gameObject.SetActive(!opened);
		cursorPanel.gameObject.SetActive(!opened);
	}

	public void SelectItem(int id, bool permanently)
	{	
		throwing = false;
		throwPanel.transform.localScale = new Vector3(0, 0, 0);
		throwPanelBlack.transform.localScale = new Vector3(0, 0, 0);
		throwPanelRed.transform.localScale = new Vector3(0, 0, 0);

		Vector2 canvasScale = new Vector2(canvas.transform.lossyScale.x, canvas.transform.lossyScale.y);

		audioManager.Play("inventory", 1.3f);

		if (opened)
		{
			Cut(36);

			if (id < 9)
				SelectSmall();

			SelectBig();
		}
		else
		{
			Cut(9);
			SelectBig();
			SelectSmall();
		}

		void SelectSmall()
		{
			float s = 1.3f;

			selectorPanel.transform.SetParent(inventoryPanelParentSmall.transform);
			selectorPanel.transform.SetSiblingIndex(0);

			selectorPanel.transform.position = smallPanels[id].transform.position;// + new Vector3(0, 1, 0);
			selectorPanel.transform.localScale = new Vector3(s, s, 0);

			smallSelectedId = id;
		}

		void SelectBig()
		{
			float s = 2.6f;

			selectorPanel.transform.SetParent(inventoryPanelParentBig.transform);
			
			selectorPanel.transform.SetSiblingIndex(1);

			selectorPanel.transform.position = panels[id].transform.position;
			selectorPanel.transform.localScale = new Vector3(s, s, 0);

			selectedId = id;

			if (opened && permanently)
			{
				if (ultraSelectedId != -1)
				{
					if (ultraSelectedId != id)
					{
						if (items[id] != null)
						{
							if (items[id].name != items[ultraSelectedId].name)
							{
								IsItem buffer = items[ultraSelectedId];
								items[ultraSelectedId] = items[id];
								items[id] = buffer;
							}
							else
							{
								items[id].count += items[ultraSelectedId].count;
								items[ultraSelectedId] = null;
							}
						}
						else
						{
							items[id] = items[ultraSelectedId];
							items[ultraSelectedId] = null;
						}

						Visualise(id);
						Visualise(ultraSelectedId);
					}

					ultraSelectedId = -1;
					selectorPanelPlus.SetActive(false); //
				}
				else if (items[id] != null)
				{
					
					//if (id == ultraSelectedId)
					{
						float f = 1.8f;

						ultraSelectedId = id;

						selectorPanelPlus.SetActive(true);
						selectorPanelPlus.transform.SetParent(inventoryPanelParentBig.transform);
						selectorPanel.transform.SetSiblingIndex(0);
						selectorPanelPlus.transform.SetSiblingIndex(0);///////////////
						selectorPanelPlus.transform.position = panels[id].transform.position;
						selectorPanelPlus.transform.localScale = new Vector3(f, f, 0);
					}
				}
			}
		}

		void Cut(int count)
		{
			id = (count + id) % count;
		}
	}
}
