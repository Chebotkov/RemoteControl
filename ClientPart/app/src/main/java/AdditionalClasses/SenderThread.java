package AdditionalClasses;

import android.os.AsyncTask;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;

public class SenderThread extends AsyncTask<Void, Void, Void> {
    private int port;
    private String ipAddress;

    public SenderThread(String ipAddress, int port)
    {
        if (ipAddress == null)
        {
            throw new NullPointerException("IP address is null");
        }

        if(port < 0 || port > 65535)
        {
            throw new IllegalArgumentException("Wrong port value");
        }

        this.ipAddress = ipAddress;
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
                DataInputStream in = new DataInputStream(socket.getInputStream());

                byte[] parcel = new byte[2];
                parcel[0] = 0;
                out.write(parcel);
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

    @Override
    protected void onPostExecute(Void aVoid) {

    }
}
