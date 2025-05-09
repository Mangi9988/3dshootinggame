using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class UI_InputFields
{
    public TextMeshProUGUI ResultText;
    public TMP_InputField IDInputField;
    public TMP_InputField PasswordInputField;
    public TMP_InputField PasswordCheckInputField;
    public Button ConfirmButton;
}

public class UI_LoginScene : MonoBehaviour
{
    [Header("패널")]
    public GameObject Loginpanel;
    public GameObject CreateAccountPanel;

    [Header("로그인")]
    public UI_InputFields LoginInputFields;
    
    [Header("회원가입")]
    public UI_InputFields CreateAccountInputFields;

    private const string PREFIX = "ID_";
    private const string SALT = "29847920";
    
    private void Start()
    {
        Loginpanel.SetActive(true);
        CreateAccountPanel.SetActive(false);
        LoginCheck();
    }
    
    public void OnClickGotoCreateAccountButton()
    {
        Loginpanel.SetActive(false);
        CreateAccountPanel.SetActive(true);
    }

    public void OnClickGotoLoginButton()
    {
        Loginpanel.SetActive(true);
        CreateAccountPanel.SetActive(false);
    }

    public void CreateAccount()
    {
        // 1. 아이디 입력을 확인한다
        string id = CreateAccountInputFields.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            CreateAccountInputFields.ResultText.text = "아이디를 입력해주세요";
            return;
        }
        
        // 2. 1차 비밀번호 입력을 확인한다.
        string password = CreateAccountInputFields.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            CreateAccountInputFields.ResultText.text = "비밀번호를 입력해주세요";
            return;
        }
        
        // 3. 2차 비밀번호 입력을 확인하고, 1차 비밀번호 입력과 같은지 확인한다.
        string password2 = CreateAccountInputFields.PasswordCheckInputField.text;
        if (string.IsNullOrEmpty(password2))
        {
            CreateAccountInputFields.ResultText.text = "비밀번호를 입력해주세요";
            return;
        }

        if (password != password2)
        {
            CreateAccountInputFields.ResultText.text = "비밀번호가 다릅니다";
            return;
        }
        
        // 4. PlayerPrefs를 이용해서 아이디와 비밀번호를 저장한다.
        PlayerPrefs.SetString(PREFIX + id, Encryption(password + SALT));
        
        // 5. 로그인 창으로 돌아간다. (이때 아이디는 자동으로 입력되어 있다.)
        OnClickGotoLoginButton();
    }

    public string Encryption(string text)
    {
        // 해시 암호화 알고리즘 인스턴스를 생성한다.
        SHA256 sha256 = SHA256.Create();
        
        // 운영체제 혹은 프로그래밍 언어별로 string 표현하는 방식이 다 다르므로
        // UTF8 버전 바이트로 배열을 바꿔야한다.
        
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        byte[] hash = sha256.ComputeHash(bytes);
        
        string resultText = string.Empty;
        foreach (byte b in hash)
        {
            // byte를 다시 string으로 바꿔서 이어붙이기
            resultText += b.ToString("x2");
        }
        
        return resultText;
    }

    public void Login()
    {
        // 1. 아이디 입력을 확인한다.
        string id = LoginInputFields.IDInputField.text;
        if (string.IsNullOrEmpty(id))
        {
            LoginInputFields.ResultText.text = "아이디를 입력해주세요";
            return;
        }
        
        // 2. 비밀번호 입력을 확인한다.
        string password = LoginInputFields.PasswordInputField.text;
        if (string.IsNullOrEmpty(password))
        {
            LoginInputFields.ResultText.text = "비밀번호를 입력해주세요";
            return;
        }
        
        // 3. PlayerPrefs.Get을 이용해서 아이디와 비밀번호가 맞는지 확인한다.
        if (!PlayerPrefs.HasKey(PREFIX + id))
        {
            LoginInputFields.ResultText.text = "아이디 혹은 비밀번호를 확인해 주세요";
            return;
        }
        
        string hashedPassword = PlayerPrefs.GetString(PREFIX + id);
        if (hashedPassword != Encryption(password + SALT))
        {
            LoginInputFields.ResultText.text = "아이디 혹은 비밀번호를 확인해 주세요";
            return;
        }
        
        // 4. 맞다면 로그인
        Debug.Log("로그인 성공!");
        SceneManager.LoadScene(1);
    }

    // 아이디와 비밀버너호 InputField값이 바뀌었을 때만 활성화
    public void LoginCheck()
    {
        string id = LoginInputFields.IDInputField.text;
        string password = LoginInputFields.PasswordInputField.text;
        
        LoginInputFields.ConfirmButton.enabled = !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(password);
    }
}
