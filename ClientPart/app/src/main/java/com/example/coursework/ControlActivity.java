package com.example.coursework;

import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

import java.io.DataOutputStream;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;

import AdditionalClasses.Commands;
import AdditionalClasses.ConnectionCreator;

public class ControlActivity extends AppCompatActivity {
    private String ipAddress;
    private int port = 10000;
    private byte command;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Bundle arguments = getIntent().getExtras();
        ipAddress = arguments.get("IP").toString();

        setContentView(R.layout.activity_control);
    }

    public void onClick (View view) {
        switch (view.getId())
        {
            case R.id.StartPresentation:
            {
                command = Commands.startPresentation;
                new SenderThread().execute();
                break;
            }
            case R.id.EndPresentation:
            {
                command = Commands.stopPresentation;
                new SenderThread().execute();
                break;
            }
            case R.id.NextSlide:
            {
                command = Commands.nextSlide;
                new SenderThread().execute();
                break;
            }
            case R.id.PreviousSlide:
            {
                command = Commands.previousSlide;
                new SenderThread().execute();
                break;
            }
        }
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

                    out.write(command);
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
}
