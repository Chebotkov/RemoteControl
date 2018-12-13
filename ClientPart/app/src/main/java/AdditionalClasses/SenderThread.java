package AdditionalClasses;

import android.os.AsyncTask;

import java.io.DataOutputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;

public class SenderThread extends AsyncTask<Void, Void, Void> {
    private byte command ;
    private int port;
    private String ipAddress;

    public SenderThread(String ipAddress, int port, byte command)
    {
        if (ipAddress == null)
        {
            throw new NullPointerException("IP address is null");
        }

        this.ipAddress = ipAddress;
        this.command = command;
        this.port = port;
    }

    @Override
    protected Void doInBackground(Void... params) {
        try {
            Socket socket = null;
            try {
                InetAddress ip = InetAddress.getByName(ipAddress);
                socket = new Socket(ip, port);

                OutputStream outputStream = socket.getOutputStream();
                DataOutputStream out = new DataOutputStream(outputStream);

                if (command != 0)
                {
                    out.write(command);
                }
            }
            catch (java.io.IOException e) {
                e.printStackTrace();
            }
            finally {
                if (socket != null) {
                    socket.close();
                }
            }
        }
        catch(java.io.IOException e)
        {
            e.printStackTrace();
        }

        return null;
    }
}
