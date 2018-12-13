package AdditionalClasses;

public class ConnectionCreator{
    private int port = 10000;
    private String ipAddress;

    public ConnectionCreator(String ipAddress) {
        if (ipAddress == null)
        {
            throw new NullPointerException("IP address is null");
        }

        this.ipAddress = ipAddress;
    }
    public ConnectionCreator(String ipAddress, int port){
        this(ipAddress);
        if(port < 0 || port > 65535)
        {
            throw new IllegalArgumentException("Wrong port value");
        }

        this.port = port;
    }

    public void ExecuteCommand(byte command)
    {
        SenderThread sender = new SenderThread(ipAddress, port, command);
        sender.execute();
    }
}
