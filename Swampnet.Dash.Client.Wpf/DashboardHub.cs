using Microsoft.AspNet.SignalR.Client;
using Serilog;
using System;

namespace Swampnet.Dash.Client.Wpf
{
    class DashboardHub : IDisposable
    {
        private const string DASHBOARD_HUB = "DashboardHub";
        private const string HUB_URL = "http://localhost:8080/";

        private readonly HubConnection _hubConnection;
        private readonly IHubProxy _proxy;
        private readonly string _dashId;

        public IHubProxy Proxy => _proxy;

        public DashboardHub(string dashId)
        {
            _dashId = dashId;
            _hubConnection = new HubConnection(HUB_URL); // @TODO: From cfg I guess
            _proxy = _hubConnection.CreateHubProxy(DASHBOARD_HUB);

            // Hook up SignalR
            _hubConnection.Closed += HubConnectionClosed;
            _hubConnection.ConnectionSlow += HubConnectionConnectionSlow;
            _hubConnection.Error += HubConnectionError;
            _hubConnection.Received += HubConnectionReceived;
            _hubConnection.Reconnected += HubConnectionReconnected;
            _hubConnection.Reconnecting += HubConnectionReconnecting;
            _hubConnection.StateChanged += HubConnectionStateChanged;
        }

        internal async void Start()
        {
            // Connect to server and join a group based on the dashboard id
            await _hubConnection.Start()
                    .ContinueWith((x) => _proxy.Invoke("JoinGroup", _dashId));
        }

        public void Dispose()
        {
            _hubConnection.Closed -= HubConnectionClosed;
            _hubConnection.ConnectionSlow -= HubConnectionConnectionSlow;
            _hubConnection.Error -= HubConnectionError;
            _hubConnection.Received -= HubConnectionReceived;
            _hubConnection.Reconnected -= HubConnectionReconnected;
            _hubConnection.Reconnecting -= HubConnectionReconnecting;
            _hubConnection.StateChanged -= HubConnectionStateChanged;

            _hubConnection.Dispose();
        }

        #region SignalR Events

        private void HubConnectionStateChanged(StateChange obj)
        {
            Log.Debug("HubConnectionStateChanged {oldState} -> {newState}", obj.OldState, obj.NewState);
        }

        private void HubConnectionReconnecting()
        {
            Log.Debug("HubConnectionReconnecting");
        }

        private void HubConnectionReconnected()
        {
            Log.Debug("HubConnectionReconnected");
        }

        private void HubConnectionReceived(string obj)
        {
            Log.Debug("HubConnectionReceived"); // 'obj' is the entire message
        }

        private void HubConnectionError(Exception ex)
        {
            Log.Error(ex, "HubConnectionError");
        }

        private void HubConnectionConnectionSlow()
        {
            Log.Debug("HubConnectionConnectionSlow");
        }

        private void HubConnectionClosed()
        {
            Log.Debug("HubConnectionClosed");
        }


        #endregion
    }
}
