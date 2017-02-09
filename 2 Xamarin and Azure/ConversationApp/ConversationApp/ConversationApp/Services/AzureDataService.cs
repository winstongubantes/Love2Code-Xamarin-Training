using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConversationApp.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace ConversationApp.Services
{
    public class AzureDataService
    {
        private MobileServiceClient _mobileServiceClient  = new MobileServiceClient("http://conversationapp.azurewebsites.net");
        IMobileServiceSyncTable<Message> _messageTable;

        public async Task Initialize()
        {
            if (_mobileServiceClient?.SyncContext?.IsInitialized ?? false)
                return;

            const string path = "syncstore.db";
            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Message>();
            await _mobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            _messageTable = _mobileServiceClient.GetSyncTable<Message>();
        }

        public async Task<IEnumerable<Message>> GetMessages()
        {
            await Initialize();
            await SyncMessage();
            return await _messageTable.ToEnumerableAsync();
        }

        public async Task AddMessage(Message msg)
        {
            await Initialize();

            await _messageTable.InsertAsync(msg);

            await SyncMessage();
        }

        public async Task SyncMessage()
        {
            try
            {
                await _messageTable.PullAsync("allMessage", _messageTable.CreateQuery());
                await _mobileServiceClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
