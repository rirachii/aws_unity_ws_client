using UnityEngine;
using WebSocketSharp;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

public class GetData : MonoBehaviour
{
     private WebSocket _ws;
    //  private void Start()
     private async void Start()
     {
        string accToken = await GetCredsAsync(); //Get aws cognito access token
        string url = $"ws://localhost/user?authorization={accToken}";
        // string url = "ws://localhost:5000/ws";

        _ws = new WebSocket(url);
        Debug.Log("Initial State : " + _ws.ReadyState);
        
        _ws.OnMessage += (sender, e) =>
        {
            Debug.Log($"Received {e.Data} from " + ((WebSocket)sender).Url + "");
        };

        _ws.OnOpen += (sender,e) =>{
            Debug.Log("Initial State : " + _ws.ReadyState);
            string msg = "{\"action\":\"subscribe\", \"event_source\":\"fyi1kq9hc8\", \"event_type\":\"upload_data\"}";
            _ws.Send(msg);
        };
        
        _ws.Connect();

        }

     private void Update()
     {
         if(_ws == null) 
         {
              return;
         }

         if(Input.GetKeyDown(KeyCode.Space))
         {
              _ws.Send("Unity data");
         }
     }

    private async Task<string> GetCredsAsync()
    {
        Debug.Log("Getting Creds from AWS...");
        AmazonCognitoIdentityProviderClient provider =
            new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials());
        CognitoUserPool userPool = new CognitoUserPool("poolID", "clientID", provider);
        CognitoUser user = new CognitoUser("username", "clientID", userPool, provider);
        InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
        {
            Password = "userPassword"
        };
        AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
        // Debug.Log(authResponse.AuthenticationResult.AccessToken);
        // return authResponse.AuthenticationResult.AccessToken;
        string accessToken = authResponse.AuthenticationResult.AccessToken;
        return accessToken;
    }
}
