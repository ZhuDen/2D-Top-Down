using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GameLibrary;
using GameLibrary.Tools;
using System.IO;
using GameLibrary.Common;
using GameLibrary.Extension;
using System.Collections.Generic;
using System.Threading;
using GameLibrary.SocketServer;
using GameLibrary.Extension;

public class GameClient : MonoBehaviour
{
    public UnityEngine.UI.Text UINameText, TextInfo;

    public static GameClient instance; //46.148.235.140 //127.0.0.1
    private const string ServerAddress = "46.148.235.140";
    private const int ServerPort = 8888;

    private Socket clientSocket;
    private byte[] receiveBuffer;
    Task reseiv;
    private CancellationTokenSource cancellationTokenSource;

    private void Awake()
    {
        _ = TransportHandler.init;
        instance = this;
    }

    private async void Start()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        receiveBuffer = new byte[8192];

        await ConnectToServer();
    }

    private async Task ConnectToServer()
    {
        int attempt = 0;

        while (attempt < 5)
        {
            try
            {
                await clientSocket.ConnectAsync(ServerAddress, ServerPort);
                TextInfo.text = "Сервер найден, соединение успешно произведено.";
                TransportHandler.Transport.Socket = clientSocket;
                //await Autorisation();
                reseiv = Task.Run(ReceiveData);
                await Connection();

                // Выход из цикла, если подключение и обработка данных успешно завершены
                break;
            }
            catch (Exception ex)
            {
                TextInfo.text = $"Попытка соединения с сервером (Попытка: {attempt + 1})";
                attempt++;
            }

            // Ожидание 5 секунд перед следующей попыткой подключения
            await Task.Delay(5000);
        }

        if (attempt == 5)
        {
            TextInfo.text = "Соединение с сервером не удалось, возможно он не доступен или отключен. Проверьте подключение к интернету.";
        }
    }

    private async Task Connection()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Connect, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Connection" } }));
    }

    /*private async Task StartReceiving()
    {
        cancellationTokenSource = new CancellationTokenSource();
        await ReceiveData(cancellationTokenSource.Token);
    }*/

    private async void ReceiveData()
    {
        List<byte> receivedData = new List<byte>();

        while (true)
        {
            try
            {
                if (clientSocket.Connected)
                {
                    Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
                    int bytesRead = await clientSocket.ReceiveAsync(receiveBuffer, SocketFlags.None);

                    if (bytesRead > 0)
                    {
                        //Debug.Log("Длина: " + bytesRead);

                        // Добавить принятые байты в receivedData
                        for (int i = 0; i < bytesRead; i++)
                        {
                            receivedData.Add(receiveBuffer[i]);
                        }

                        // Проверить, достаточно ли получено байт для обработки сообщения
                        while (receivedData.Count >= sizeof(int))
                        {
                            // Прочитать размер данных из заголовка
                            int dataSize = BitConverter.ToInt32(receivedData.ToArray(), 0);

                            if (receivedData.Count - sizeof(int) >= dataSize)
                            {
                                // Извлечь данные из receivedData
                                byte[] messageBuffer = receivedData.GetRange(sizeof(int), dataSize).ToArray();
                                receivedData.RemoveRange(0, sizeof(int) + dataSize);

                                // Обработка сообщения
                                var inputBuffer = await Serializer.DeserializeAsync<DataPacket>(messageBuffer);
                                Handled.HandleReceivedData(inputBuffer);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Соединение закрыто, выполните необходимые действия
                        break;
                    }
                }
                else
                {
                    // Соединение закрыто, выполните необходимые действия
                    break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error receiving data: {ex.Message}");
                // Обработайте исключение и выполните необходимые действия
                // Например, продолжить цикл прослушивания на следующей итерации
            }
        }
    }



    private void OnDestroy()
    {
        TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Disconnect, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Desconnected" } }, SendClientFlag.Me));
        reseiv.Dispose();
        clientSocket?.Disconnect(false);
        clientSocket?.Dispose();
        clientSocket?.Close();
        
    }
}