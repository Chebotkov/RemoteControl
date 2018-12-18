package com.example.coursework;

import android.annotation.SuppressLint;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.ImageDecoder;
import android.media.Image;
import android.media.ImageReader;
import android.os.AsyncTask;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.Toast;

import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.lang.reflect.Array;
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;
import java.nio.ByteBuffer;
import java.security.cert.PKIXRevocationChecker;

import AdditionalClasses.Commands;
import AdditionalClasses.ConnectionCreator;
import ru.yandex.speechkit.Language;
import ru.yandex.speechkit.OnlineModel;
import ru.yandex.speechkit.OnlineRecognizer;
import ru.yandex.speechkit.PhraseSpotter;
import ru.yandex.speechkit.RecognizerListener;
import ru.yandex.speechkit.SpeechKit;

import static ru.yandex.speechkit.OnlineModel.QUERIES;

public class ControlActivity extends AppCompatActivity {
    private String ipAddress;
    private int port = 10000;
    private byte command;
    private int bufferSize = 8;
    private byte[] totalBytes;
    private boolean isSendingMust;
    OnlineRecognizer recognizer;

    @SuppressLint("ResourceType")
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        /*try {
            SpeechKit.getInstance().init(getApplicationContext(), "36ecfd72-ff5c-4ad7-9d7f-7fb93932218b");
            SpeechKit.getInstance().setUuid("36ecfd72ff25c4ad579d7f87fb9e3932218b");
        }
        catch (ru.yandex.speechkit.SpeechKit.LibraryInitializationException e)
        {
            Toast toast = Toast.makeText(this,e.getMessage(), Toast.LENGTH_LONG);
            toast.show();
        }
        OnlineRecognizer recognizer = new OnlineRecognizer.Builder(Language.RUSSIAN, OnlineModel.QUERIES, (RecognizerListener) this)
                .setDisableAntimat(false)
                .setEnablePunctuation(true)
                .build(); // 1
        recognizer.prepare(); // 2*/

        Bundle arguments = getIntent().getExtras();
        ipAddress = arguments.get("IP").toString();
        setContentView(R.layout.activity_control);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.settings, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if(item.isChecked()) item.setChecked(true);

        return super.onOptionsItemSelected(item);
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
                @SuppressLint("ResourceType") Toast toast = Toast.makeText(this, String.valueOf(((MenuItem)findViewById(R.menu.settings)).isChecked()), Toast.LENGTH_LONG);
                toast.show();
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
                    DataInputStream in = new DataInputStream(socket.getInputStream());

                    out.write(command);

                    if(command == Commands.nextSlide)
                    {
                        InputStreamReader inputStreamReader = new InputStreamReader(socket.getInputStream());

                        BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
                        int receivedBytesLength = Integer.parseInt(bufferedReader.readLine());
                        totalBytes = new byte[receivedBytesLength];
                        try {
                            Thread.sleep(50);
                        } catch (InterruptedException e) {
                            // TODO Auto-generated catch block
                            e.printStackTrace();
                        }
                        int currentPosition = 0;
                        byte[] receivedBytes = new byte[bufferSize];
                        while ((in.read(receivedBytes)) > -1) {
                            for (int i = currentPosition, j = 0; j < receivedBytes.length; i++, j++)
                            {
                                if (i >= totalBytes.length)
                                {
                                    break;
                                }

                                totalBytes[i] = receivedBytes[j];
                            }

                            currentPosition+=receivedBytes.length;
                        }
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

        @Override
        protected void onPostExecute(Void aVoid) {
            if (command == Commands.nextSlide) {
                try {
                    ImageView image = findViewById(R.id.Screenshot);
                    Bitmap bmp = BitmapFactory.decodeByteArray(totalBytes, 0, totalBytes.length);
                    image.setImageBitmap(Bitmap.createScaledBitmap(bmp, bmp.getWidth(),
                            bmp.getHeight(), false));
                }
                catch (Exception e) {
                }
            }
        }
    }
}
