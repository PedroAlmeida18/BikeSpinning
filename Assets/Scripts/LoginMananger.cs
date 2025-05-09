using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase.Auth;
using System;


public class LoginMananger : MonoBehaviour
{
    public TMP_InputField userInput;
    public TMP_InputField senhaInput;
    public TextMeshProUGUI messageText;
    public Button loginButton;
    public Button CriarContaButton;
    public TMP_InputField userInputCadastro;
    public TMP_InputField senhaInputCadastro;
    public TMP_InputField confirmaSenhaInputCadastro;
    public Button SalvarConta;
    public Button CriarContaSave;

    public GameObject painelCadastro;
    public GameObject painelRecuperarSenha;

    public Button ButtonRecuperarSenhaSave;
    public Button RecuperarSenhaButton;
    public TMP_InputField recuperaUser;
    public TMP_InputField confirmaUser;
    public TextMeshProUGUI mensagemPainelSenha;
    public Button ButtonVoltarSenha;
    public Button buttonVoltarConta;

    void Awake()
    {
         
    }

    void Start()
    {
        painelCadastro.SetActive(false);
        messageText.gameObject.SetActive(false);
        CriarContaButton.onClick.AddListener(AbrirPainelCadastro);
        SalvarConta.onClick.AddListener(CriarContaFirebase);
        CriarContaSave.onClick.AddListener(DesligarPainelCadastro);
        loginButton.onClick.AddListener(LoginComFirebase);
        RecuperarSenhaButton.onClick.AddListener(AbrirPainelRecuperarSenha);
        ButtonRecuperarSenhaSave.onClick.AddListener(RecuperarSenhaFirebase);
        ButtonVoltarSenha.onClick.AddListener(FecharPainel);
        buttonVoltarConta.onClick.AddListener(FecharPainel);

    }

    void Update()
    {
    
    }
    /*public void ValidateLogin()
    {
        string username = userInput.text;
        string senhaname = senhaInput.text;

        if (username == "PedroDev" && senhaname=="123456")
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "Login bem-sucedido!";
            messageText.color = Color.green;
            SceneManager.LoadScene("QuilometragemScene");
        }
        else
        {
            messageText.gameObject.SetActive(true);
            messageText.text = "Usuário ou senha inválidos!";
            messageText.color = Color.red;
        }

    }
    */
    public void CriarContaFirebase(){
    string email = userInputCadastro.text;
    string senha = senhaInputCadastro.text;
    string confirmaSenha = confirmaSenhaInputCadastro.text;

    FirebaseManager.auth.CreateUserWithEmailAndPasswordAsync(email, senha).ContinueWith(task => 
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            
        ShowMessage("Erro ao criar conta.", Color.red);
            return;
        }

        if(senha == confirmaSenha ){
        AuthResult authResult = task.Result;
        FirebaseUser user = authResult.User;
        FirebaseManager.user = user;

        string userId = user.UserId;
        FirebaseManager.dbReference.Child("usuarios").Child(userId).Child("email").SetValueAsync(email);

        }

        
    });

    }
    public void RecuperarSenhaFirebase(){
        string email = recuperaUser.text;
        string confirmaemail = confirmaUser.text;
        try {
            if(email==confirmaemail){
             FirebaseManager.auth.SendPasswordResetEmailAsync(confirmaemail).ContinueWith(task => {
                 if (task.IsCanceled || task.IsFaulted){
               MensagemRecuperarSenha("Erro ao enviar E-mail de recuperação.", Color.red);
                return;}
                
             });
             MensagemRecuperarSenha("E-mail para recuperar senha Enviado", Color.green);
             FecharPainel();
        } 
        else{
             MensagemRecuperarSenha("Os e-mails não coincidem.", Color.red);
        }
        }
        catch (Exception e){
            MensagemRecuperarSenha("Os e-mails não coincidem.", Color.red);
        }
        
       
    }

    void ShowMessage(string msg, Color cor)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = msg;
        messageText.color = cor;
    }

    void MensagemRecuperarSenha(string mensagem,Color cor){
        mensagemPainelSenha.gameObject.SetActive(true);
        mensagemPainelSenha.text=mensagem;
        mensagemPainelSenha.color=cor;
    }
    public async void LoginComFirebase()

{
    string email = userInput.text;
    string senha = senhaInput.text;
    ShowMessage("Tentativa de login", Color.red);
    Debug.Log(email);
    Debug.Log(senha);

    try
    {
        var authResult = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, senha);
        FirebaseUser user = authResult.User;
        FirebaseManager.user = user;

        ShowMessage("Login bem-sucedido!", Color.green);

        SceneManager.LoadScene("QuilometragemScene");
    }
    catch (Exception e)
    {
        Debug.LogError("Erro de login: " + e.Message);
        ShowMessage("Usuário ou senha inválidos!", Color.red);
    }
}

    void AbrirPainelCadastro()
    {
    painelCadastro.SetActive(true);
    }

    void DesligarPainelCadastro(){
        painelCadastro.SetActive(false);
    }
  /*  void CarregarQuilometro(){
         SceneManager.LoadScene("QuilometragemScene");
    }
    */
    void AbrirPainelRecuperarSenha(){
        painelRecuperarSenha.SetActive(true);
    }
    void FecharPainel(){
        if(painelCadastro.activeSelf){
            painelCadastro.SetActive(false);

        }
        if(painelRecuperarSenha.activeSelf){
            painelRecuperarSenha.SetActive(false);
        }
      
    }
}
