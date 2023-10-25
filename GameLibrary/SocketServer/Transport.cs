using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;
using System.Threading.Tasks;
using GameLibrary.Tools;
using GameLibrary.SocketServer;
using GameLibrary.Common;
using System.Linq;

public class Transport : PeerPack
{
    public async Task SendTo(DataPacket packet)
    {
        try
        {
            var serializedPacket = await Serializer.SerializeAsync(packet);

            // Получите размер данных пакета в виде байтового массива
            byte[] sizeBytes = BitConverter.GetBytes(serializedPacket.Length);

            // Объедините размер данных и сам пакет в один буфер
            byte[] buffer = sizeBytes.Concat(serializedPacket).ToArray();

            //Console.WriteLine("BUFFER: " + buffer.Length);

            // Отправьте данные клиенту
            await Socket.SendAsync(buffer, SocketFlags.None);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException occurred while sending data to : {ex.SocketErrorCode}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IOException occurred while sending data to: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while sending data to : {ex.Message}");
        }
    }
    public override void Close()
    {
        throw new NotImplementedException();
    }
    public override void Connect()
    {
        throw new NotImplementedException();
    }
    public override void Disconnect()
    {
        throw new NotImplementedException();
    }
    public override bool IsConnected()
    {
        throw new NotImplementedException();
    }
    public override bool IsDisconnected()
    {
        throw new NotImplementedException();
    }
}
