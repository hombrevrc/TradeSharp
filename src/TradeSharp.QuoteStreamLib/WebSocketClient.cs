using System;
using TradeSharp.Util;
using System.Linq;
using System.Text;
using WebSocket4Net;
using SuperSocket.ClientEngine;

namespace TradeSharp.QuoteStreamLib
{
    public class WebSocketClient : IDisposable
    {
        #region constructor

        #endregion

        #region properties

        public event Action<string> MessageRecieved;
        public event Action Opened;
        public event Action<Exception> ErrorRecieved;

        #endregion

        #region protected

        protected virtual void OnMessageRecieved(string obj)
        {
            MessageRecieved?.Invoke(obj);
        }

        protected virtual void OnErrorRecieved(Exception obj)
        {
            ErrorRecieved?.Invoke(obj);
        }

        protected virtual void OnOpened()
        {
            Opened?.Invoke();
        }

        #endregion

        #region fields

        private WebSocket _webSocket;
        private volatile bool _isStopping;

        #endregion

        #region private


        private void WebSocketOnOpened(object sender, EventArgs eventArgs)
        {
            OnOpened();
        }


        private void WebSocket_Closed(object sender, EventArgs e)
        {
            Reopen();
        }

        private void Reopen()
        {
            lock (_lock)
            {
                try
                {
                    if (!_isStopping && _webSocket.State != WebSocketState.Connecting) //восстанавливаем соединение
                        _webSocket.Open();
                }
                catch (Exception ex) //перехватываем исключение когда повторно пытаемся открыть сессию
                {
                    Logger.Error(ex.GetType().Name + " " + ex.Message);
                }
            }
        }

        private readonly object _lock = new object();

        #endregion

        #region public

        public void Setup(string uri)
        {
            _webSocket = new WebSocket(uri) { AllowUnstrustedCertificate = true };
        }

        public void Start()
        {
            if (_webSocket.State == WebSocketState.Open)
                return;
            _isStopping = false;
            _webSocket.MessageReceived += WebSocketOnMessageReceived;
            _webSocket.Closed += WebSocket_Closed;
            _webSocket.Opened += WebSocketOnOpened;
            _webSocket.Error += WebSocketOnError;
            _webSocket.Open();
        }

        private void WebSocketOnError(object sender, ErrorEventArgs e)
        {
            OnErrorRecieved(e.Exception);
        }

        private void WebSocketOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            MessageRecieved(e.Message);
        }

        public void Stop()
        {
            _isStopping = true;
            _webSocket.MessageReceived -= WebSocketOnMessageReceived;
            _webSocket.Closed -= WebSocket_Closed;
            _webSocket.Opened -= WebSocketOnOpened;
            _webSocket.Error -= WebSocketOnError;
            _webSocket.Close();
        }

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
