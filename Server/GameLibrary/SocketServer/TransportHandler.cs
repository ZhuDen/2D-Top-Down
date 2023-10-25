using System;
using System.Collections.Generic;
using System.Text;
using GameLibrary.SocketServer;

public static class TransportHandler
{
    private static Transport _Transport;

    public static Transport init => _Transport = new Transport();

    public static Transport Transport => _Transport;

}
