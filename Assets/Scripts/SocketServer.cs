using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketServer : MonoBehaviour
{
    private Socket listener;
    private byte[] buffer = new byte[1024];

    private void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        try
        {
            // Crea un socket TCP en el puerto 12345
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, 12345));
            listener.Listen(10);

            Debug.Log("Servidor socket iniciado en el puerto 12345");

            // Comienza a escuchar conexiones entrantes en un hilo separado
            listener.BeginAccept(new AsyncCallback(HandleClientConnection), null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al iniciar el servidor socket: " + e.Message);
        }
    }

    private void HandleClientConnection(IAsyncResult ar)
    {
        try
        {
            Socket clientSocket = listener.EndAccept(ar);

            // Aquí puedes implementar la lógica para manejar la comunicación con el cliente
            // Por ejemplo, recibir y enviar datos al cliente

            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
        }
        catch (Exception e)
        {
            Debug.LogError("Error al manejar la conexión del cliente: " + e.Message);
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        Socket clientSocket = (Socket)ar.AsyncState;
        int bytesRead = clientSocket.EndReceive(ar);

        if (bytesRead > 0)
        {
            // Procesar los datos recibidos del cliente aquí
            string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Debug.Log("Datos recibidos del cliente: " + receivedData);

            // Puedes responder al cliente si es necesario
            byte[] response = Encoding.ASCII.GetBytes("Respuesta desde el servidor");
            clientSocket.BeginSend(response, 0, response.Length, SocketFlags.None, new AsyncCallback(SendCallback), clientSocket);
        }

        // Continuar esperando más datos del cliente
        clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), clientSocket);
    }

    private void SendCallback(IAsyncResult ar)
    {
        Socket clientSocket = (Socket)ar.AsyncState;
        int bytesSent = clientSocket.EndSend(ar);
    }

    private void OnDestroy()
    {
        if (listener != null)
        {
            listener.Close();
        }
    }
}