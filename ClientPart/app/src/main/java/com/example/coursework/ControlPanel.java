package com.example.coursework;

import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import AdditionalClasses.Commands;

public class ControlPanel extends AppCompatActivity implements View.OnTouchListener {
    private float deltaX = (float)129.111;
    private float deltaY = (float)-133.5111;
    private byte command;
    private String ipAddress;
    private int port = 10000;
    private ControlPanel view = this;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Bundle arguments = getIntent().getExtras();
        ipAddress = arguments.get("IP").toString();

        setContentView(R.layout.activity_control_panel);

        ImageView image = findViewById(R.id.TouchPad);
        image.setOnTouchListener(this);
    }

    @Override
    public boolean onTouch(View v, MotionEvent event) {
            if (event.getAction() == MotionEvent.ACTION_MOVE) {
                //deltaX = event.getX() - deltaX;
                //deltaY = event.getY() - deltaY;
            }
            if (event.getAction() == MotionEvent.ACTION_DOWN) {
                command = Commands.moveMouse;
                new SenderThread().execute();
            }
        return true;
    }

    class SenderThread extends AsyncTask<Void, Void, Void> {
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
                    parcel[1] = command;
                    out.write(parcel);
                    out.writeFloat(deltaX);
                    out.writeFloat(deltaY);
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
}
