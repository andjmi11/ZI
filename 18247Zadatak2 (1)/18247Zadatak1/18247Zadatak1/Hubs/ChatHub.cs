using Microsoft.AspNetCore.SignalR;

namespace _18247Zadatak1.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, List<int> blanko, string alg)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message,blanko, alg);
        }

        public async Task SendFile(string usernameInput, string encryptedFileByteArray, byte[] blakeHash, string selectedAlgorithm)
        {
            await Clients.All.SendAsync("ReceiveFile", usernameInput, encryptedFileByteArray, blakeHash, selectedAlgorithm);
        }
    }
}
